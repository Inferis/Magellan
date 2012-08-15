using System;
using System.IO;
using System.Drawing;
using System.Collections;
using EU2.Data;

namespace EU2.Map
{
	/// <summary>
	/// Summary description for ID.
	/// </summary>
	public abstract class ScanlineCodedMap : EU2.IO.IStreamWriteable
	{
		public ScanlineCodedMap() {
			SetZeroID(); 
			EmptyMap();
		}

		public ScanlineCodedMap( BinaryReader reader ) : this() {
			ReadFrom( reader );
		}

		public ScanlineCodedMap( string path ) : this() {
			FileStream stream = null;
			try {
				stream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read );
				ReadFrom( new BinaryReader( stream ) );
			}
			finally {
				if ( stream != null ) stream.Close();
			}
		}

		public abstract void SetZeroID();
		public virtual bool CheckValue( int value ) {
			return true;
		}

		public virtual void EmptyMap() {
			lines = new Scanline[Lightmap.BaseHeight];
			for ( int i=0; i<Lightmap.BaseHeight; ++i ) {
				lines[i] = new Scanline( ZeroID );
			}
		}

		public virtual void ReadFrom( BinaryReader reader ) {
			int[] offsets = new int[Lightmap.BaseHeight+1];
			// Store the total zone size at MAPHEIGHT+1
			offsets[Lightmap.BaseHeight] = reader.ReadInt32();

			// Read offsets firsts
			for ( int i=0; i<Lightmap.BaseHeight; ++i ) {
				offsets[i] = reader.ReadInt32();
			}

			// Read the zones
			for ( int y=0; y<Lightmap.BaseHeight; ++y ) {
				lines[y].ReadFrom( reader, offsets[y+1]-offsets[y] );
			}
		}

		public virtual void WriteTo( BinaryWriter writer ) {
			// Find out total zonecount
			int zoneCount = 0;
			for ( int y=0; y<Lightmap.BaseHeight; ++y ) {
				zoneCount += lines[y].Count;
			}
			writer.Write( zoneCount );

			// Write offsets
			int offset = 0;
			for ( int y=0; y<Lightmap.BaseHeight; ++y ) {
				writer.Write( offset );
				offset += lines[y].Count;
			}

			// Write zones
			for ( int y=0; y<Lightmap.BaseHeight; ++y ) {
				lines[y].WriteTo( writer );
			}
		}

	
		public virtual void WriteTo( Stream stream ) {
			WriteTo( new BinaryWriter( stream ) );
		}

		public ushort this[Point location] {
			get { return this[location.X, location.Y]; }
			set { this[location.X, location.Y] = value; }
		}

		public ushort this[int x, int y] {
			get {
				if ( y<0 || y>=Lightmap.BaseHeight ) return ZeroID;
				return lines[y][x];
			}
			set {
				if ( y<0 || y>=Lightmap.BaseHeight ) return; // No action
				lines[y][x] = value;
			}
		}

		public ushort[] ExportBitmapBuffer( int x, int y, int width, int height ) {
			if ( width < 0 || width > Lightmap.BaseWidth ) throw new ArgumentOutOfRangeException();

			ushort[] buffer = new ushort[width*height];
			int bufindex = 0;

			for ( int iy=0; iy<height; ++iy ) {
				int py = y+iy;
				if ( py < 0 || py >= Lightmap.BaseHeight ) {
					for ( int ix = 0; ix<width; ++ix ) {
						buffer[bufindex++] = ZeroID;
					}
				}
				else {
					lines[py].ExportBitmapBuffer( buffer, ref bufindex, x, width ); 
				}
			}

			return buffer;
		}

		public ushort[] ExportBitmapBuffer( Point location, Size size ) {
			return ExportBitmapBuffer( location.X, location.Y, size.Width, size.Height );
		}

		public ushort[] ExportBitmapBuffer( Rectangle area ) {
			return ExportBitmapBuffer( area.X, area.Y, area.Width, area.Height );
		}

		public ushort[,] ExportBitmapGrid( int x, int y, int width, int height ) {
			ushort[] buffer = ExportBitmapBuffer( x, y, width, height );

			int bufindex = 0;
			ushort[,] grid = new ushort[width,height];
			for ( int iy=0; iy<height; ++iy ) {
				for ( int ix=0; ix<width; ++ix ) {
					grid[ix,iy] = buffer[bufindex++];
				}
			}

			return grid;
		}

		public ushort[,] ExportBitmapGrid( Point location, Size size ) {
			return ExportBitmapGrid( location.X, location.Y, size.Width, size.Height );
		}

		public ushort[,] ExportBitmapGrid( Rectangle area ) {
			return ExportBitmapGrid( area.X, area.Y, area.Width, area.Height );
		}

		public void ImportBitmapBuffer( int x, int y, int width, int height, ushort[] buffer ) {
			int[] intbuffer = new int[buffer.Length];
			Array.Copy( buffer, 0, intbuffer, 0, buffer.Length );

			ImportBitmapBuffer( x, y, width, height, intbuffer );
		}

		public void ImportBitmapBuffer( int x, int y, int width, int height, bool[] buffer ) {
			int[] intbuffer = new int[buffer.Length];
			Array.Copy( buffer, 0, intbuffer, 0, buffer.Length );

			ImportBitmapBuffer( x, y, width, height, intbuffer );
		}

		public void ImportBitmapBuffer( int x, int y, int width, int height, int[] buffer ) {
			if ( width < 1 || width > Lightmap.BaseWidth ) throw new ArgumentOutOfRangeException();
			if ( height < 1 || height > Lightmap.BaseHeight ) throw new ArgumentOutOfRangeException();

			if ( y+height > Lightmap.BaseHeight ) height = Lightmap.BaseHeight - y;
			int left = Lightmap.BaseNormalizeX( x );
			int right = left + width;
			int top = y;
			int bottom = y + height;

			// Check if the block has to be split in two or not (edge of the map)
			if ( right > Lightmap.BaseWidth ) {
				// Yes. We'll split the buffer into 2 and call ourself again, twice.
				int[] leftbuffer, rightbuffer;
				int leftwidth = Lightmap.BaseWidth-left;
				int rightwidth = Lightmap.BaseWidth-left;

				leftbuffer = new int[leftwidth*height];
				rightbuffer = new int[rightwidth*height];

				for ( int by=0; by<height; ++by ) {
					Array.Copy( buffer, by*width, leftbuffer, by*leftwidth, leftwidth );
					Array.Copy( buffer, by*width + leftwidth, rightbuffer, by*rightwidth, rightwidth );
				}

				ImportBitmapBuffer( x, y, leftwidth, height, leftbuffer );
				ImportBitmapBuffer( 0, y, rightwidth, height, rightbuffer );

				return;
			}

			// No, one contigious block. Good.
			// Walk over each line, adding zones as needed
			for ( int py=top; py<bottom; ++py ) {
				int bufstart = (py-top)*width;
				int bufend = (py-top+1)*width;

				lines[py].ImportBitmapBuffer( buffer, bufstart, width, left, right );
			}
		}

		public void ImportBitmapBuffer( Point location, Size size, bool[] buffer ) {
			ImportBitmapBuffer( location.X, location.Y, size.Width, size.Height, buffer );
		}

		public void ImportBitmapBuffer( Point location, Size size, ushort[] buffer ) {
			ImportBitmapBuffer( location.X, location.Y, size.Width, size.Height, buffer );
		}

		public void ImportBitmapBuffer( Point location, Size size, int[] buffer ) {
			ImportBitmapBuffer( location.X, location.Y, size.Width, size.Height, buffer );
		}

		public void ImportBitmapBuffer( Rectangle bounds, bool[] buffer ) {
			ImportBitmapBuffer( bounds.X, bounds.Y, bounds.Width, bounds.Height, buffer );
		}

		public void ImportBitmapBuffer( Rectangle bounds, ushort[] buffer ) {
			ImportBitmapBuffer( bounds.X, bounds.Y, bounds.Width, bounds.Height, buffer );
		}

		public void ImportBitmapBuffer( Rectangle bounds, int[] buffer ) {
			ImportBitmapBuffer( bounds.X, bounds.Y, bounds.Width, bounds.Height, buffer );
		}

		public void ImportBitmapGrid( int x, int y, ushort[,] grid ) {
			int width = grid.GetLength(0);
			int height = grid.GetLength(1);
			ushort[] buffer = new ushort[width*height];

			int bufindex = 0;
			for ( int iy=0; iy<height; ++iy ) {
				for ( int ix=0; ix<width; ++ix ) {
					buffer[bufindex++] = grid[ix,iy];
				}
			}

			ImportBitmapBuffer( x, y, width, height, buffer );
		}

		public void ImportBitmapGrid( Point location, ushort[,] grid ) {
			ImportBitmapGrid( location.X, location.Y, grid );
		}
		
		protected  int LookupZoneIndex( int x, int y ) {
			return lines[y].LookupZoneIndex( x );
		}

		protected Scanline[] lines;
		protected ushort ZeroID;

		protected struct IDZone {
			internal static IDZone Empty = new IDZone( -1, 0 );

			public IDZone( short x, ushort provinceID ) {
				this.X = x;
				this.ID = provinceID;
			}

			// for convience
			internal IDZone( int x, int provinceID ) {
				this.X = (short)x;
				this.ID = (ushort)provinceID;
			}

			public IDZone( BinaryReader reader ) {
				this.X = -1;
				this.ID = 0;
				ReadFrom( reader );
			}

			public void ReadFrom( BinaryReader reader ) {
				this.X = reader.ReadInt16();
				this.ID = (ushort)reader.ReadInt16();
			}

			public void WriteTo( BinaryWriter writer ) {
				writer.Write( X );
				writer.Write( (short)ID );
			}

			/*public short X {
				get { return x; }
				set { x = value; }
			}

			public ushort ID {
				get { return id; }
				set { id = value; }
			}
			*/

			public short X;
			public ushort ID;

		}

		protected class Scanline : ICollection {
			public Scanline( ushort zero ) {
				zones =  new IDZone[2] { new IDZone( (short)0, zero ), new IDZone( (short)Lightmap.BaseWidth, zero ) } ;
				zeroId = zero;
			}

			public Scanline( ushort zero, IDZone[] zones ) {
				this.zones = new IDZone[zones.Length];
				Array.Copy( zones, 0, this.zones, 0, zones.Length );
				zeroId = zero;
			}

			public virtual void ReadFrom( BinaryReader reader, int size ) {
				zones =  new IDZone[size];

				for ( int i=0; i<size; ++i ) {
					zones[i] = new IDZone( reader );
				}
			}

			public virtual void WriteTo( BinaryWriter writer ) {
				for ( int x=0; x<zones.Length; ++x ) {
					zones[x].WriteTo( writer );
				}
			}

			public int LookupZoneIndex( int x ) {
				if ( x > Lightmap.BaseWidth ) x = Lightmap.BaseNormalizeX( x );

				for ( int i=0; i<zones.Length; ++i ) {
					if ( x == zones[i].X ) return i;
					if ( x < zones[i].X ) return i-1;
				}

				return -1;
			}

			public IDZone LookupZone( int x ) {
				int idx = LookupZoneIndex( x );
				if ( idx >= 0 ) return zones[idx];

				return new IDZone( (short)-1, zeroId );
			}

			public IDZone[] GetZones( ) {
				return zones;
			}

			public IDZone GetZone( int idx ) {
				return zones[idx];
			}

			public void SetZone( int idx, IDZone zone ) {
				zones[idx] = zone;
			}

			public int GetZoneLength( int idx ) {
				if ( idx >= zones.Length ) return 0;
				if ( idx == zones.Length - 1 ) {
					int len = Lightmap.BaseWidth - zones[idx].X;
					if ( idx > 1 && zones[0].ID == zones[idx].ID ) len += zones[1].X;
					return len;
				}

				return zones[idx+1].X - zones[idx].X;
			}

			public ushort this[ int x ] {
				get {
					return LookupZone( x ).ID;
				}
				set {
					x = Lightmap.BaseNormalizeX( x );

					// I think this is the longest set { } implementation I've ever written... :) ~ Inferis

					// Transform x coordinate into a zoneindex
					int index = LookupZoneIndex( x );
					if ( index < 0 ) throw new ArgumentOutOfRangeException();

					// Skip if we're trying to put the same value
					IDZone zone = zones[index];
					if ( zone.ID == value ) return;

					// Find the length out
					int length = index+1 < zones.Length ? (zones[index+1].X - zone.X) : (Lightmap.BaseWidth - zone.X);
					IDZone[] list;

					if ( length == 1 ) {
						// Special case. We don't have to bother about resizing the zone, but we might have to join surrounding zones if they contain the same values.
						zone.ID = value;

						bool leftOk = (index > 0 && zones[index-1].ID == value);
						bool rightOk = (index+1 < zones.Length && zones[index+1].ID == value);
						if ( leftOk && !rightOk ) {
							// Left equals us, but not the right... Just remove this block (it is joined with the left, left.X remains valid).
							list = new IDZone[zones.Length-1];
							Array.Copy( zones, 0, list, 0, index );
							Array.Copy( zones, index+1, list, index, zones.Length-index-1 );
							zones = list;
						}
						else if ( !leftOk && rightOk ) {
							// Right equals us, but not the left... Keep this block, and remove the right (as our X remains valid)
							list = new IDZone[zones.Length-1];
							Array.Copy( zones, 0, list, 0, index+1 );
							Array.Copy( zones, index+2, list, index+1, zones.Length-index-2 );
							zones = list;
						}
						else if ( leftOk && rightOk ) {
							// Both left and right equal us. Move all three into one block (keep left, as it's X remains valid)
							list = new IDZone[zones.Length-2];
							Array.Copy( zones, 0, list, 0, index );
							Array.Copy( zones, index+2, list, index, zones.Length-index-2 );
							zones = list;
						}
						else {
							// None of the blocks beside us are equal. Simplest case, just change the value!
							zones[index].ID = value;
						}

						return;
					}

					// Okay, our zone is longer than 1 pixel. Is it...				
					// ... At the start of this zone?
					if ( x == zone.X ) {
						// Yes, check if previous zone exists and if it has the same value...
						if ( index == 0 || zones[index-1].ID != value ) {
							// Nope, create a new zonelist in a temporary array, and place the new zone at the correct place
							list = new IDZone[zones.Length+1];
							Array.Copy( zones, 0, list, 0, index );
							list[index] = new IDZone( x, value );
							Array.Copy( zones, index, list, index+1, zones.Length-index );
							// Assign new array
							zones = list;
							// Move index
							++index;
						}

						// Then, shorten this zone
						zones[index].X += 1;
						return;
					}

					// Not at the start, maybe at the end?
					if ( x == zone.X+length-1 ) {
						// Yeps, check if next zone exists and if it has the same value
						if ( index+1 < zones.Length && zones[index+1].ID == value ) {
							// Yes... Lengthen
							zones[index+1].X -= 1;
						}
						else {
							// Nope, create a new zonelist in a temporary array, and place the new zone at the correct place
							list = new IDZone[zones.Length+1];
							if ( index+1 == zones.Length ) {
								// We're at the end, just copy the list and add the proc behind 
								zones.CopyTo( list, 0 );
							}
							else {
								Array.Copy( zones, 0, list, 0, index+1 );
								Array.Copy( zones, index+1, list, index+2, zones.Length-index-1 );
								// Assign new array
							}
							list[index+1] = new IDZone( x, value );
							zones = list;
						}

						return;
					}

					// So, somewhere in the middle. Split the zone in three.
					IDZone left = new IDZone( zone.X, zone.ID );
					IDZone right = new IDZone( x+1, zone.ID );
					zone = new IDZone( x, value );

					list = new IDZone[zones.Length+2];
					Array.Copy( zones, 0, list, 0, index );
					list[index] = left;
					list[index+1] = zone;
					list[index+2] = right;
					Array.Copy( zones, index+1, list, index+3, zones.Length-index-1 );
			
					zones = list;
				}

			}

			public void ImportBitmapBuffer( int[] buffer, int bufstart, int bufwidth, int left, int right ) {
				ArrayList newZones = new ArrayList( bufwidth/8 );
				bufwidth += bufstart;

				IDZone currentZone = new IDZone( left, buffer[bufstart] );
				for ( int bx = bufstart+1, px = left+1; bx<bufwidth; ++bx, ++px ) {
					if ( buffer[bx] != currentZone.ID ) {
						// ID changes, so add this zone and start a new one
						newZones.Add( currentZone );
						currentZone = new IDZone( px, buffer[bx] );
					}
				}
				// Add last zone
				newZones.Add( currentZone );
			
				int leftindex = LookupZoneIndex( left );
				int rightindex = LookupZoneIndex( right );

				//System.Diagnostics.Debug.Assert( leftindex <= rightindex, "invalid left/right index" );

				if ( zones[rightindex].ID == ((IDZone)newZones[newZones.Count-1]).ID ) {
					// If the last new zone equals the original right zone, we can safely drop the original right zone. 
					// That means that rightindex is included in the "to be removed" area.
					++rightindex;
				}
				else if ( leftindex < rightindex ) {
					// Otherwise, we'll have to adjust the X value of that original right zone, and shift
					// rightindex one to the left.
					zones[rightindex].X = (short)right;
				}
				else {
					newZones.Add( new IDZone( right, zones[rightindex].ID ) );
					++rightindex;
				}

				if ( zones[leftindex].ID == ((IDZone)newZones[0]).ID ) {
					// If the first new zone equals the original left zone, we'll need to adjust the first zone.
					IDZone tmp = ((IDZone)newZones[0]);
					tmp.X = zones[leftindex].X;
					newZones[0] = tmp;
				}
				else {
					// Otherwise, we start at the next zone, but only if the previous zone is different.
					if ( leftindex > 0 && zones[leftindex-1].ID == ((IDZone)newZones[0]).ID ) {
						newZones.RemoveAt( 0 );
					}
					else if ( zones[leftindex].X != ((IDZone)newZones[0]).X ) {
						++leftindex;
					}
				}

				// leftindex and rightindex now define the bounds of the zones we need to get rid of
				int nzi = 0;
				for ( int zi=leftindex; zi<rightindex; ++zi ) {
					if ( nzi < newZones.Count ) {
						// we have new zones left, copy one over
						zones[zi] = (IDZone)newZones[nzi++];
					}
					else {
						// no more new zones left, shift the rest of the bunch to the left
						for ( int shift=0; shift < zones.Length-rightindex; ++shift ) {
							zones[zi+shift] = zones[rightindex+shift];
						}
						// Resize the array
						IDZone[] zones2 = new IDZone[zi+zones.Length-rightindex];
						Array.Copy( zones, 0, zones2, 0, zones2.Length );
						zones = zones2;
						break;
					}
				}
				if ( nzi < newZones.Count ) {
					// We still need to copy some
					IDZone[] zones2 = new IDZone[zones.Length+newZones.Count-nzi];
					Array.Copy( zones, 0, zones2, 0, rightindex );
					Array.Copy( zones, rightindex, zones2, rightindex+newZones.Count-nzi, zones.Length-rightindex );
					
					while ( nzi < newZones.Count ) {
						zones2[rightindex++] = (IDZone)newZones[nzi++];
					}
					zones = zones2;
				}
			}

			public void ExportBitmapBuffer( ushort[] buffer, ref int bufindex, int x, int width ) {
				x = Lightmap.BaseNormalizeX( x );
				int zoneindex = LookupZoneIndex( x );
				int sx = x;
					
				for ( int ix = 0; ix < width; ) {
					if ( zoneindex >= zones.Length - 1 ) {
						// Last zone = bad, skip to first
						zoneindex = 0;
						sx = 0;
					}

					ushort id = zones[zoneindex].ID;
					int len = zones[zoneindex+1].X - (zones[zoneindex].X >= sx ? zones[zoneindex].X : sx );
					if ( ix+len > width ) len = width-ix;
					//System.Diagnostics.Debug.Assert( len > 0 );
					for ( int bx=0; bx<len; ++bx ) buffer[bufindex++] = id;

					zoneindex++;
					ix += len;
				}
			}

			public void TagIDs( int[] tagged, int left, int right, bool inclusive ) {
				int z = LookupZoneIndex( left );
				if ( inclusive ) {
					z++; // force next zone when doing inclusive matching
					if ( z == zones.Length-1 ) { // Wrap around!
						z = 0;
						right -= Lightmap.BaseWidth;
					}
				}

				while ( zones[inclusive ? ((z+1)%zones.Length) : z].X <= right ) {
					if ( z == zones.Length-1 ) { // Wrap around!
						z = 0;
						right -= Lightmap.BaseWidth;
					}
					++tagged[zones[z++].ID];
				}
			}

			#region ICollection Members

			public bool IsSynchronized {
				get {
					return zones.IsSynchronized;
				}
			}

			public int Count {
				get {
					return zones.Length;
				}
			}

			public void CopyTo( Array array, int index ) {
				zones.CopyTo( array, index );
			}

			public object SyncRoot {
				get {
					return zones.SyncRoot;
				}
			}

			#endregion

			#region IEnumerable Members

			public IEnumerator GetEnumerator() {
				return zones.GetEnumerator();
			}

			#endregion

			IDZone[] zones;
			ushort zeroId;
		}

	}
}
