using System;
using System.IO;
using System.Drawing;

namespace EU2.Data.Mapfiles
{
	/// <summary>
	/// Summary description for Lightmap.
	/// </summary>
	public class Lightmap : InstallLinkedObject {
		public const int BlockFactor = 5;
		public const int BlockSize = 1 << BlockFactor;

		private CompressedBlock[] blocks;
		private int zoom;
		private string sourceFilename;

		public Lightmap( int zoom, string filename, Install install ) : base( install ) {
			this.zoom = zoom;
			blocks = null;
			ReadBlocks( filename );
		}

		public Lightmap( int zoom, Stream stream, Install install ) : base( install ) {
			this.zoom = zoom;
			blocks = null;
			ReadBlocks( stream );
		}

		public string SourceFilename {
			get { return sourceFilename; }
		}

		public int Zoom {
			get { return zoom; }
		}

		public Point Resolution {
			get { return new Point( Map.Width >> (zoom), Map.Height >> (zoom) ); }
		}

		public Point BlockResolution {
			get { return new Point( Map.Width >> (BlockFactor+zoom), Map.Height >> (BlockFactor+zoom) ); }
		}

		public void ReadBlocks( string filename ) {
			FileStream stream = new System.IO.FileStream(filename,FileMode.Open, FileAccess.Read, FileShare.Read );

			try {
				ReadBlocks( stream );
			}
			catch ( Exception e ) {
				blocks = null;
				sourceFilename = "";
				throw e;
			}
			finally {
				stream.Close();
			}
		}
			
		public void ReadBlocks( Stream stream ) {
			// Calculate the number of blocks to read. This depends on the zoomlevel.
			int blockcount = ((Map.Width >> (5+zoom)) * (Map.Height >> (5+zoom)));

			blocks = new CompressedBlock[blockcount];
			int offsetStart = 0, offsetEnd = 0;
			int baseOffset = (blockcount+1) * 3 + 1;
			for ( int i=0; i<blockcount; ++i ) {
				if ( i == 0 ) {
					offsetStart = stream.ReadByte() + (stream.ReadByte()<<8) + (stream.ReadByte()<<16);
					offsetEnd = stream.ReadByte() + (stream.ReadByte()<<8) + (stream.ReadByte()<<16);
				}
				else {
					offsetStart = offsetEnd;
					offsetEnd = stream.ReadByte() + (stream.ReadByte()<<8) + (stream.ReadByte()<<16);
				}

				blocks[i] = new CompressedBlock( baseOffset+offsetStart, offsetEnd-offsetStart, this );      // Defer reading the block data till later.
			}

			// Read another byte. The last offset is stored as a 4-byte integer, but only 3 bytes where read in the loop before.
			int zero = stream.ReadByte();			

			// Read all the actual data afterwards
			for ( int i=0; i<blockcount; ++i ) {
				blocks[i].Read( stream );
			}

			sourceFilename = stream is FileStream ? ((FileStream)stream).Name : "";
		}

		/// <summary>
		/// Write all the compressed blocks in this lightmap object to the file they originally came from. 
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// The filename to write the data to is not available. This could be because the data came from a MemoryStream, for example where no filename was available when the data was read.
		/// </exception>
		public void WriteBlocks( ) {
			if ( sourceFilename != null && sourceFilename.Length > 0 ) 
				WriteBlocks( sourceFilename );
			else
				throw new InvalidOperationException( "There is no filename set.");
		}

		/// <summary>
		/// Write all the compressed blocks in this lightmap object to a file. This will build 2 parts: the offsets to the blocks, and
		/// the block data itself. The file will be overwritten if it exists.
		/// </summary>
		/// <param name="filename">The full pathname of the file to write to blocks to</param>
		public void WriteBlocks( string filename ) {
			FileStream stream = new System.IO.FileStream(filename,FileMode.Create, FileAccess.Write, FileShare.None );

			try {
				WriteBlocks( stream );
			}
			catch ( Exception e ) {
				stream.Close();
				stream = null;
				if ( File.Exists( filename ) ) File.Delete( filename );
				throw e;
			}
			finally {
				if ( stream != null ) stream.Close();
			}
		}

		/// <summary>
		/// Write all the compressed blocks in this lightmap object to a stream. This will build 2 parts: the offsets to the blocks, and
		/// the block data itself.
		/// </summary>
		/// <param name="stream">The stream to write the blocks to</param>
		public void WriteBlocks( Stream stream ) {
			int blockcount = blocks.Length;

			// First run: write all the offsets. 
			int offset = 0;
			for ( int i=0; i<blockcount; ++i ) {
				stream.Write( new byte[3] { (byte)(offset & 0xff), 
											(byte)((offset >> 8) & 0xff), 
											(byte)((offset >> 16) & 0xff) }, 
					          0, 3 );
				offset += blocks[i].Size;
			}
			// Write the last offset (actually, the total size) as a 4byte integer.
			byte[] sizebuf = new byte[4] { (byte)(offset & 0xff), (byte)((offset>>8) & 0xff), (byte)((offset>>16) & 0xff), 0 };
			stream.Write( sizebuf, 0, 4 );

			// Second run: write all the block data. 
			for ( int i=0; i<blockcount; ++i ) {
				blocks[i].Write( stream );
			}
		}

		public CompressedBlock this[int index] {
			get {
				return blocks[index];
			}
		}

		public int BlockCount {
			get { return blocks.Length; }
		}
	}
}
