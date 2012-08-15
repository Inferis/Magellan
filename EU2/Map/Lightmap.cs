using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using EU2.Data;
using EU2.Map.Codec;
using EU2.Map.Drawing;

namespace EU2.Map
{
	/// <summary>
	/// Summary description for Lightmap.
	/// </summary>
	public class Lightmap : EU2.IO.IStreamWriteable, ILightmapDimensions {
		#region Public Constants 
		public const int BaseWidth = 18944;
		public const int BaseHeight = 7296;
        public const int BaseWidthInBlocks = BaseWidth >> BlockFactor;
        public const int BaseHeightInBlocks = BaseHeight >> BlockFactor;
        public static Size BaseSize = new Size(BaseWidth, BaseHeight);
        public const int TotalBlocks = BaseWidthInBlocks * BaseHeightInBlocks;
		public const int BlockFactor = 5;
		public const int BlockSize = 1 << BlockFactor;
        public const int PixelsPerBlock = Lightmap.BlockSize * Lightmap.BlockSize;		
        #endregion

		#region Constructors
		public static Lightmap FromStream( int zoom, BinaryReader reader ) {
			return new Lightmap( zoom, reader );
		}

		public static Lightmap FromStream( int zoom, ProvinceList provinces, AdjacencyTable adjacent, IDMap idmap, BinaryReader reader ) {
			return new Lightmap( zoom, provinces, adjacent, idmap, reader );
		}

		public static Lightmap FromFile( int zoom, string path ) {
			FileStream stream = null;
			try {
				stream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read );
				return new Lightmap( zoom, new BinaryReader( stream ) );
			}
			finally {
				if ( stream != null ) stream.Close();
			}
		}

		public static Lightmap FromFile( int zoom, ProvinceList provinces, AdjacencyTable adjacent, IDMap idmap, string path ) {
			FileStream stream = null;
			try {
				stream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read );
				return new Lightmap( zoom, provinces, adjacent, idmap, new BinaryReader( stream ) );
			}
			finally {
				if ( stream != null ) stream.Close();
			}
		}

		public static Lightmap CreateEmpty( int zoom ) {
			Lightmap result = new Lightmap( zoom, null, null, null );

			CompressedBlock block = new MapBlock().Compress( 0, 0, zoom, null, null, null );
			for ( int i=0; i<result.blocks.Length; ++i ) result.blocks[i] = block;
			return result;
		}

		public static Lightmap CreateEmpty( int zoom, ProvinceList provinces, AdjacencyTable adjacent, IDMap idmap ) {
			Lightmap result = new Lightmap( zoom, provinces, adjacent, idmap );
			for ( int i=0; i<result.blocks.Length; ++i ) result.blocks[i] = new MapBlock();
			return result;
		}

		private Lightmap( int zoom, ProvinceList provinces, AdjacencyTable adjacent, IDMap idmap ) {
			if ( zoom < 0 ) throw new InvalidZoomFactorException( zoom );
			this.zoom = zoom;
			this.provinces = provinces;
			this.adjacent = adjacent;
			this.idmap = idmap;
			this.volatiledecompression = false;

			Size sz = CoordMap.ActualToZoomedBlocks( BaseSize );
			blocks = new GenericMapBlock[sz.Width*sz.Height];
		}

		private Lightmap( int zoom, ProvinceList provinces, AdjacencyTable adjacent, IDMap idmap, BinaryReader reader ) : this( zoom, provinces, adjacent, idmap ) {
			ReadFrom( reader );
		}

		private Lightmap( int zoom, BinaryReader reader ) : this( zoom, null, null, null ) {
			ReadFrom( reader );
		}
		#endregion

		#region IO
		public void ReadFrom( BinaryReader reader ) {
			// Calculate the number of blocks to read. This depends on the zoomlevel.
			int blockcount = ((BaseWidth >> (5+zoom)) * (BaseHeight >> (BlockFactor+zoom)));

			blocks = new GenericMapBlock[blockcount];
			int offsetStart = 0, offsetEnd = 0;
			int baseOffset = (int)reader.BaseStream.Position + (blockcount+1) * 3 + 1;
			for ( int i=0; i<blockcount; ++i ) {
				if ( i == 0 ) {
					offsetStart = reader.ReadByte() + (reader.ReadByte()<<8) + (reader.ReadByte()<<16);
					offsetEnd = reader.ReadByte() + (reader.ReadByte()<<8) + (reader.ReadByte()<<16);
				}
				else {
					offsetStart = offsetEnd;
					offsetEnd = reader.ReadByte() + (reader.ReadByte()<<8) + (reader.ReadByte()<<16);
				}

				blocks[i] = new CompressedBlock( baseOffset+offsetStart, offsetEnd-offsetStart );      // Defer reading the block data till later.
			}

			// Read another byte. The last offset is stored as a 4-byte integer, but only 3 bytes where read in the loop before.
			reader.ReadByte();			

			// Read all the actual data afterwards
			for ( int i=0; i<blockcount; ++i ) {
				((CompressedBlock)blocks[i]).ReadFrom( reader );
			}
		}

		public void ReadFrom2( BinaryReader reader ) {
			// Calculate the number of blocks to read. This depends on the zoomlevel.
			int blockcount = ((BaseWidth >> (5+zoom)) * (BaseHeight >> (BlockFactor+zoom)));

			int[] offsets = new int[blockcount+1];
			for ( int i=0; i<=blockcount; ++i ) {
				offsets[i] = reader.ReadByte() + (reader.ReadByte()<<8) + (reader.ReadByte()<<16);
			}

			// Read last offset (size), which we don't need
			reader.ReadByte();			

			// Read all the remaining data into a buffer
			MemoryStream stream = new MemoryStream( blocks.Length * 6 );

			byte[] buffer = new byte[4096];
			int count = 0;
			while ( (count = reader.Read( buffer, 0, 4096 )) > 0 ) {
				stream.Write( buffer, 0, count );	
			}

			// Okay, we now have a memorystream were we can search in
			blocks = new GenericMapBlock[blockcount];
			for ( int i=0; i<blockcount; ++i ) {
				blocks[i] = new CompressedBlock( offsets[i], offsets[i+1], stream );
			}
			stream.Close();
		}
		
		/// <summary>
		/// Write all the compressed blocks in this lightmap object to a stream. This will build 2 parts: the offsets to the blocks, and
		/// the block data itself.
		/// </summary>
		/// <param name="stream">The stream to write the blocks to</param>
		public void WriteTo( BinaryWriter writer, bool postcompress ) {
			if ( postcompress ) 
				WriteToPostCompress( writer );
			else
				WriteToNormal( writer );
		}

		public void WriteTo( BinaryWriter writer ) {
			WriteToNormal( writer );
		}
			
		public void WriteToPostCompress( BinaryWriter writer ) {
			// Three runs:

			// First run: convert each decompressed block to a compressed block.
			int rx = 0, ry = 0;
			for ( int i=0; i<blocks.Length; ++i ) {
				if ( !blocks[i].IsCompressed() ) blocks[i] = ((MapBlock)blocks[i]).Compress( rx, ry, zoom, provinces, adjacent, idmap );

				rx += (BlockSize << zoom);
				if ( rx >= Lightmap.BaseWidth ) {
					rx = 0;
					ry += (BlockSize << zoom);
				}
			}

			// Second run: Now look for similar blocks
			int count = 0;
			int[] redirect = new int[blocks.Length];
			for ( int i=0; i<blocks.Length; ++i ) redirect[i] = -1;

			for ( int i=0; i<blocks.Length; ++i ) {
				if ( redirect[i] > 0 ) continue;  // already done

				redirect[i] = i; // refer to ourselves, now look for similar blocks
				for ( int i2=i+1; i2<blocks.Length; ++i2 ) {
					if ( redirect[i2] >= 0 ) continue; // already done
					if ( ((CompressedBlock)blocks[i2]).Equals( (CompressedBlock)blocks[i] ) ) {
						redirect[i2] = i;
						count++;
					}
				}
			}

			// Third run: change redirects to offsets
			int offset = 0;
			int[] offsets = new int[blocks.Length+1];
			for ( int i=0; i<blocks.Length; ++i ) {
				if ( i == redirect[i] ) { // original block, calc size
					offset += ((CompressedBlock)blocks[i]).Size;
					offsets[i] = offset;
				}
				else { // redirected block
					offsets[i] = offsets[redirect[i]];
				}
			}
			offsets[blocks.Length] = offset;

			// fourth run: write offsets
			for ( int i=0; i<blocks.Length; ++i ) {
				writer.Write( new byte[3] { (byte)(offsets[i] & 0xff), 
											  (byte)((offsets[i] >> 8) & 0xff), 
											  (byte)((offsets[i] >> 16) & 0xff) }, 
					0, 3 );
			}
			// Write the last offset (actually, the total size) as a 4byte integer.
			writer.Write( offsets[blocks.Length] );

			// Last run: write all the block data.
			// These blocks are already compressed, so no more work to do.
			for ( int i=0; i<blocks.Length; ++i ) {
				if ( i == redirect[i] ) 
					((CompressedBlock)blocks[i]).WriteTo( writer );
			}
		}
			
		public void WriteToNormal( BinaryWriter writer ) {
			int blockcount = blocks.Length;

			// First run: convert each decompressed block to a compressed block.
			// Then write the offset for this block. 
			int offset = 0;
			int rx = 0, ry = 0;
			for ( int i=0; i<blockcount; ++i ) {
				// not compressed? Compress it before storing.
				if ( !blocks[i].IsCompressed() ) 
					blocks[i] = ((MapBlock)blocks[i]).Compress( rx, ry, zoom, provinces, adjacent, idmap );

				writer.Write( new byte[3] { (byte)(offset & 0xff), 
											  (byte)((offset >> 8) & 0xff), 
											  (byte)((offset >> 16) & 0xff) }, 
					0, 3 );
				offset += ((CompressedBlock)blocks[i]).Size;
				rx += (BlockSize << zoom);
				if ( rx >= Lightmap.BaseWidth ) {
					rx = 0;
					ry += (BlockSize << zoom);
				}
			}
			// Write the last offset (actually, the total size) as a 4byte integer.
			writer.Write( offset );

			// Second run: write all the block data.
			// These blocks are already compressed, so no more work to do.
			for ( int i=0; i<blockcount; ++i ) {
				((CompressedBlock)blocks[i]).WriteTo( writer );
			}
		}

		public virtual void WriteTo( Stream stream ) {
			BinaryWriter writer = new BinaryWriter( stream );
			WriteTo( writer );
		}

		public void CheckWrite( ) {
		}

		#endregion
		
		public bool VolatileDecompression {
			get { return volatiledecompression; }
			set { volatiledecompression = value; }
		}

		public void PreCompress() {
			int blockcount = blocks.Length;

			int rx = 0, ry = 0;
			for ( int i=0; i<blockcount; ++i ) {
				// not compressed? Compress it.
				if ( !blocks[i].IsCompressed() ) 
					blocks[i] = ((MapBlock)blocks[i]).Compress( rx, ry, zoom, provinces, adjacent, idmap );

				rx += (BlockSize << zoom);
				if ( rx >= Lightmap.BaseWidth ) {
					rx = 0;
					ry += (BlockSize << zoom);
				}

			}
		}

		public void Recompress() {
			int[] indexes = new int[blocks.Length];
			for ( int i=0; i<indexes.Length; ++i ) indexes[i] = i;

			Recompress( indexes );
		}

		public void Recompress( int[] indexes ) {
			int x, y;
			for ( int i=0; i<indexes.Length; ++i ) {
				if ( indexes[i] < 0 || indexes[i] >= blocks.Length ) continue;

				if ( blocks[i].IsCompressed() ) blocks[indexes[i]] = new MapBlock( (CompressedBlock)blocks[indexes[i]], adjacent );
				x = (indexes[i] % SizeBlocks.Width) << BlockFactor;
				y = (indexes[i] / SizeBlocks.Width) << BlockFactor;
				blocks[indexes[i]] = ((MapBlock)blocks[indexes[i]]).Compress( x, y, zoom, provinces, adjacent, idmap );
			}
		}

		public Lightmap Shrink() {
			Lightmap result = new Lightmap( zoom+1, provinces, adjacent, idmap );

			Size size = result.CoordMap.ActualToZoomedBlocks( Lightmap.BaseSize );
#if true
			int stride = CoordMap.ActualToZoomedBlocks( Lightmap.BaseWidth );
			for ( int blockindex = 0, y=0; y<size.Height; ++y ) {
				for ( int x=0; x<size.Width; ++x ) {
					// Get 4 blocks
					int index = (y<<1)*stride+(x<<1);

					result.blocks[blockindex++] = MapBlock.Combine( 
						GetDecompressedBlock( index+stride+1 ),		// bottomright 
						GetDecompressedBlock( index+stride ),		// bottomleft	
						GetDecompressedBlock( index+1 ),			// topright
						GetDecompressedBlock( index )   			// topleft
					).Compress( (x << BlockFactor) << (zoom), (y << BlockFactor) << (zoom), zoom+1, provinces, adjacent, idmap );
				}
			}
#else
			// Turn on volatile decompression, so that we don't hog memory...
			bool remembercompression = volatiledecompression;
			volatiledecompression = true;

			for ( int y=size.Height-1; y>=0; --y ) {
				for ( int x=size.Width-1; x>=0; --x ) {
					result.EncodeImage( DecodeImage( new Rectangle(  x<<(BlockFactor+1), y<<(BlockFactor+1), BlockSize << 1, BlockSize << 1 ) ).Shrink() );
					//RawImage raw = DecodeImage( new Rectangle( x<<(BlockFactor+1), y<<(BlockFactor+1), BlockSize << 1, BlockSize << 1 ) );
					//raw = raw.Shrink();
					//result.EncodeImage( raw );
				}
				GC.Collect();
			}

			// Put previous decompression mode back
			volatiledecompression = remembercompression;
#endif

			return result;
		}

		public void Shrink( out Lightmap result1, out Lightmap result2 ) {
			result1 = new Lightmap( zoom+1, provinces, adjacent, idmap );
			result2 = new Lightmap( zoom+2, provinces, adjacent, idmap );

			Size size = this.CoordMap.ActualToZoomedBlocks( Lightmap.BaseSize );
			//Size size1 = result1.CoordMap.ActualToZoomedBlocks( Lightmap.BaseSize );
			//Size size2 = result2.CoordMap.ActualToZoomedBlocks( Lightmap.BaseSize );
			int stride = CoordMap.ActualToZoomedBlocks( Lightmap.BaseWidth );
			MapBlock[,] matrix = new MapBlock[4,4];
			for ( int blockindex1 = 0, blockindex2 = 0, y=0; y<size.Height; y+=2 ) {
				for ( int x=0; x<size.Width; x+=2 ) {
					// Get 4 blocks
					int index = y*stride+x;

					result1.blocks[blockindex1++] = MapBlock.Combine( 
						GetDecompressedBlock( index+stride+1 ),		// bottomright 
						GetDecompressedBlock( index+stride ),		// bottomleft	
						GetDecompressedBlock( index+1 ),			// topright
						GetDecompressedBlock( index )   			// topleft
						).Compress( (x << BlockFactor) << zoom, (y << BlockFactor) << zoom, zoom+1, provinces, adjacent, idmap );

					if ( (x % 4 == 0) && (y % 4 == 0) ) {
						matrix[3,3] = GetDecompressedBlock( index+(stride*3)+3 );
						matrix[2,3] = GetDecompressedBlock( index+(stride*3)+2 );
						matrix[1,3] = GetDecompressedBlock( index+(stride*3)+1 );
						matrix[0,3] = GetDecompressedBlock( index+(stride*3) );
						matrix[3,2] = GetDecompressedBlock( index+(stride*2)+3 );
						matrix[2,2] = GetDecompressedBlock( index+(stride*2)+2 );
						matrix[1,2] = GetDecompressedBlock( index+(stride*2)+1 );
						matrix[0,2] = GetDecompressedBlock( index+(stride*2) );
						matrix[3,1] = GetDecompressedBlock( index+stride+3 );
						matrix[2,1] = GetDecompressedBlock( index+stride+2 );
						matrix[1,1] = GetDecompressedBlock( index+stride+1 );
						matrix[0,1] = GetDecompressedBlock( index+stride );
						matrix[3,0] = GetDecompressedBlock( index+3 );
						matrix[2,0] = GetDecompressedBlock( index+2 );
						matrix[1,0] = GetDecompressedBlock( index+1 );
						matrix[0,0] = GetDecompressedBlock( index );

						result2.blocks[blockindex2++] = MapBlock.Combine( matrix )
							.Compress( (x << BlockFactor) << (zoom), (y << BlockFactor) << (zoom), zoom+2, provinces, adjacent, idmap );
					}
				}
			}
		}

		public void Attach( ProvinceList provinces, AdjacencyTable adjacent, IDMap idmap ) {
			this.provinces = provinces;
			this.adjacent = adjacent;
			this.idmap = idmap;
		}

		public void Erase( Point location ) {
			blocks[GetBlockIndex(location)] = new MapBlock();
		}

		public void Erase( int x, int y ) {
			blocks[GetBlockIndex( x, y )] = new MapBlock();
		}

		public void Erase( Rectangle area ) {
			for ( int y=area.Top; y<area.Bottom; ++y ) {
				for ( int x=area.Left; y<area.Right; ++x ) {
					blocks[GetBlockIndex( NormalizeBlockX( x ), y )] = new MapBlock();
				}
			}
		}

		#region Image import and export
		public RawImage DecodeBlockImage( MapBlock block, Point location ) {
			location = CoordMap.FitToGrid( location );
			RawImage image = new RawImage( zoom, new Rectangle( location, new Size( BlockSize, BlockSize ) ) );

			ImageDecoder decoder = new ImageDecoder( image );
			decoder.Decode1( 0, 0, block );

			return image;
		}

		public RawImage DecodeBlockImage( CompressedBlock block, Point location ) {
			return DecodeBlockImage( new MapBlock( block, adjacent ), location );
		}

		public RawImage DecodeImage( Rectangle rect ) {
			// Calculate the integral rectangle, i.e. on block boundaries
			Rectangle integralrect = CoordMap.FitToGrid( rect );
			// Get the size of the rectangle in blocks
			Size size = CoordMap.ActualToBlocks( integralrect.Size );

			// Create the image
			RawImage image = new RawImage( zoom, integralrect );
			ImageDecoder decoder = new ImageDecoder( image );

#if false 
			// Create a grid of decompressed blocks first
			Point gridloc = integralrect.Location;
			for ( int y=0; y<size.Height; ++y ) {
				for ( int x=0; x<size.Width; ++x ) {
					MapBlock self = (MapBlock)this[GetBlockIndex( gridloc )];
					MapBlock right = (MapBlock)this[GetBlockIndex( gridloc.X + BlockSize, gridloc.Y )];
					MapBlock bottom = (MapBlock)this[GetBlockIndex( gridloc.X, gridloc.Y + BlockSize )];
					MapBlock bottomright = (MapBlock)this[GetBlockIndex( gridloc.X + BlockSize, gridloc.Y + BlockSize )];

					decoder.Decode4( x << BlockFactor, y << BlockFactor, self, right, bottom, bottomright ); 

					gridloc.X = NormalizeX( gridloc.X + (BlockSize << zoom) );
				}
				gridloc.Y += BlockSize << zoom;
				gridloc.X = integralrect.Location.X;
			}
#else
			// Create a grid of decompressed blocks first
			Point gridloc = new Point( NormalizeX( integralrect.Right - BlockSize ), integralrect.Bottom - BlockSize);
			for ( int x=size.Width-1; x>=0; --x ) {
				MapBlock self = (MapBlock)this[GetBlockIndex( gridloc )];
				MapBlock right = (MapBlock)this[GetBlockIndex( gridloc.X + BlockSize, gridloc.Y )];
				MapBlock bottom = (MapBlock)this[GetBlockIndex( gridloc.X, gridloc.Y + BlockSize )];
				MapBlock bottomright = (MapBlock)this[GetBlockIndex( gridloc.X + BlockSize, gridloc.Y + BlockSize )];
				gridloc.Y -= BlockSize;

				decoder.Decode4( x << BlockFactor, (size.Height-1) << BlockFactor, self, right, bottom, bottomright ); 
				for ( int y=size.Height-2; y>=0; --y ) {
					self = (MapBlock)this[GetBlockIndex( gridloc )];
					right = (MapBlock)this[GetBlockIndex( gridloc.X + BlockSize, gridloc.Y )];
					
					decoder.Decode2( x << BlockFactor, y << BlockFactor, self, right ); 
					gridloc.Y -= BlockSize;
				}
				gridloc.X = NormalizeX( gridloc.X - BlockSize );
				gridloc.Y = integralrect.Bottom - BlockSize;
			}
#endif
			return image;
		}

		public int[] DecodeImage( Rectangle rect, IShader32 shader ) {
			return shader.Shade32( DecodeImage( rect ) );
		}

		public short[] DecodeImage( Rectangle rect, IShader16 shader ) {
			return shader.Shade16( DecodeImage( rect ) );
		}

		public void EncodeImage( RawImage image ) {
			// This should be on an integral boundary
			if ( !CoordMap.FitToGrid( image.Bounds ).Equals( image.Bounds ) ) 
				throw new ArgumentOutOfRangeException( "image.Bounds", "The bounds of the RawImage are not on grid bounds." );

			// We have to enlarge our memory because the encoding needs to have the right/bottom values.
			Pixel[,] oldmem = image.Memory;
			int w = oldmem.GetLength(0);
			int h = oldmem.GetLength(1);
			Pixel[,] newmem  = new Pixel[w+1,h+1];
			// Copy the memory
			for ( int x=0; x<w; ++x ) {
				Array.Copy( oldmem, x*h, newmem, x*(h+1), h );
				newmem[x,h] = newmem[x,h-1];
			}
			Array.Copy( newmem, (w-1)*(h+1), newmem, w*(h+1), h+1 );
			image.Memory = newmem;

			// Get the size of the rectangle in blocks
			Size size = MapToBlockCoordinates( image.Size );
			ImageEncoder encoder = new ImageEncoder( image );
			Point gridloc = image.Location;
			for ( int y=0; y<size.Height; ++y ) {
				for ( int x=0; x<size.Width; ++x ) {
					this[GetBlockIndex(gridloc)] = encoder.Encode( x << BlockFactor, y << BlockFactor );
					gridloc.X += BlockSize;
				}
				gridloc.X = image.Location.X;
				gridloc.Y += BlockSize;
			}

			// Reinstate old memory, since the caller might need it again.
			image.Memory = oldmem;
		}
		#endregion

		#region Blocks and Resolution
		public CompressedBlock GetCompressedBlock( int index ) {
			if ( index < 0 || index >= blocks.Length ) return null; 

			if ( !blocks[index].IsCompressed() ) throw new NotSupportedException();
			
			return (CompressedBlock)blocks[index];
		}

		public MapBlock GetDecompressedBlock( int index ) {
			if ( index < 0 || index >= blocks.Length ) return null; 

			if ( blocks[index].IsCompressed() )
				return new MapBlock( (CompressedBlock)blocks[index], adjacent );
				
			return (MapBlock)blocks[index];
		}

		public MapBlock this[int index] {
			get {
				if ( index < 0 || index >= blocks.Length ) return null; 
				if ( blocks[index] == null ) return null;

				// Decompress first if necessary
				//Console.WriteLine( "block {0} - {1}", index, blocks[index].IsCompressed() );
				if ( blocks[index].IsCompressed() ) {
					if ( volatiledecompression ) 
						return new MapBlock( (CompressedBlock)blocks[index], adjacent );
					else {
						CompressedBlock compressed = (CompressedBlock)blocks[index];
						blocks[index] = new MapBlock( compressed, adjacent );
					}
				}

				return (MapBlock)blocks[index];
			}
			set {
				if ( index < 0 || index >= blocks.Length ) return; 
				blocks[index] = value;
			}
		}

		public int BlockCount {
			get { return blocks.Length; }
		}

		public int Zoom {
			get { return zoom; }
		}

		public Size Size {
			get { return new Size( BaseWidth >> (zoom), BaseHeight >> (zoom) ); }
		}

		public Size SizeBlocks {
			get { return new Size( BaseWidth >> (BlockFactor+zoom), BaseHeight >> (BlockFactor+zoom) ); }
		}

		public int GetBlockIndex( Point location ) {
			Point orig = location;
			location = MapToBlockCoordinates( location );
			//Console.WriteLine( "GetBlockIndex( " + orig + " ) = ( " + location + " ) = " + (location.X + BlockResolution.X*location.Y) );

			return location.X + SizeBlocks.Width*location.Y;
		}
		public int GetBlockIndex( int x, int y ) {
			return GetBlockIndex( new Point( x, y ) );
		}

		public int[] GetBlockIndexes( Rectangle rect ) {
			rect = CoordMap.FitToGrid( rect );
			int stride = SizeBlocks.Width;
			int[] result = new int[(rect.Width>>BlockFactor)*(rect.Height>>BlockFactor)];
			int r = 0;
			for ( int y=rect.Top; y<rect.Bottom; y+=BlockSize ) {
				for ( int x=rect.Left; x<rect.Right; x+=BlockSize ) {
					result[r++] = (NormalizeX(x)>>BlockFactor) + stride*(y>>BlockFactor);
				}
			}

			return result;
		}

		public int[] GetBlockIndexes( Rectangle[] rects ) {
			bool[] tagged = new bool[blocks.Length];
			int stride = SizeBlocks.Width;
			int count = 0;

			for ( int r=0; r<rects.Length; ++r ) {
				Rectangle rect = CoordMap.FitToGrid( rects[r] );
				for ( int y=rect.Top; y<rect.Bottom; y+=BlockSize ) {
					for ( int x=rect.Left; x<rect.Right; x+=BlockSize ) {
						int idx = (NormalizeX(x)>>BlockFactor) + stride*(y>>BlockFactor);
						if ( !tagged[idx] ) {
							tagged[idx] = true;
							count++;
						}
					}
				}
			}

			int[] result = new int[count];
			count = 0;
			for ( int i=0; i<tagged.Length; ++i ) {
				if ( tagged[i] ) {
					result[count++] = i;
				}
			}

			return result;
		}

		#endregion

		#region Coordinate mapping
		public int NormalizeX( int x ) {
			x %= Size.Width;
			if ( x < 0 ) x += Size.Width;

			return x;
		}

		public int NormalizeBlockX( int x ) {
			x %= SizeBlocks.Width;
			if ( x < 0 ) x += SizeBlocks.Width;

			return x;
		}

		public static int BaseNormalizeX( int x ) {
			x %= Lightmap.BaseWidth;
			if ( x < 0 ) x += Lightmap.BaseWidth;

			return x;
		}

		public static int BaseNormalizeBlockX( int x ) {
			x %= (Lightmap.BaseWidth >> BlockFactor);
			if ( x < 0 ) x += (Lightmap.BaseWidth >> BlockFactor);

			return x;
		}

		public static int FitToGrid( int p ) {
			return (p >> BlockFactor) << BlockFactor;
		}

		public CoordinateMapper CoordMap {
			get { return new CoordinateMapper( zoom ); }
		}

		public Point MapToBlockCoordinates( Point point ) {
            point.X = NormalizeX(point.X);
			return CoordMap.ActualToBlocks( point );
			//return new Point( point.X >> (zoom+BlockFactor), point.Y >> (zoom+BlockFactor) );
		}

		public Size MapToBlockCoordinates( Size size ) {
			return CoordMap.ActualToBlocks( size );
			//return new Size( size.Width >> (zoom+BlockFactor), size.Height >> (zoom+BlockFactor) );
		}

		public Rectangle MapToBlockCoordinates( Rectangle rect ) {
            rect.X = NormalizeX(rect.X);
			return CoordMap.ActualToBlocks( rect );
			//return new Rectangle( MapToBlockCoordinates( rect.Location ), MapToBlockCoordinates( rect.Size ) );
		}

		#endregion

		#region Context
		public ProvinceList Provinces {
			get { return provinces; }
		}

		public AdjacencyTable Adjacent {
			get { return adjacent; }
		}

		public IDMap IDMap {
			get { return idmap; }
		}
		#endregion

		#region Private Fields
		private GenericMapBlock[] blocks;
		private int zoom;
		private ProvinceList provinces;
		private AdjacencyTable adjacent;
		private IDMap idmap;	
		private bool volatiledecompression;
		#endregion
	}

}
