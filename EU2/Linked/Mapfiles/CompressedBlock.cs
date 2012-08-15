using System;
using System.IO;

namespace EU2.Data.Mapfiles
{
	/// <summary>
	/// Summary description for CompressedBlock.
	/// </summary>
	public class CompressedBlock {
		byte[] data;
		int offset, size;
		Lightmap parent;

		public CompressedBlock( int offset, int size, Lightmap parent )  {
			this.parent = parent;
			this.offset = offset;
			this.size = size;
			this.data = null;
		}

		public CompressedBlock( Stream stream, int size, Lightmap parent  ) {
			this.parent = parent;
			this.offset = (int)stream.Position;
			this.size = size;
			this.data = null;

			Read( stream );
		}

		public void Read( Stream stream ) {
			if ( offset < 0 || size <= 0 ) return;

			this.data = new byte[size];
			stream.Seek( offset, SeekOrigin.Begin );
			stream.Read( data, 0, size );

			// Set offset and size to -1 to mark that this block is read and contains data.
			offset = -1;
			size = -1;
		}

		public void Write( Stream stream ) {
			if ( this.data == null ) return;

			stream.Write( data, 0, data.Length );
		}

		public byte[] Data {
			get {
				return data;
			}
		}

		public Lightmap Parent {
			get {
				return parent;
			}
		}

		public int Size {
			get {
				if ( data != null ) 
					return data.Length;
				else
					return size < 0 ? 0 : size;
			}
		}

		/// <summary>
		/// Decodes (or "decompresses") this block. This will convert the raw data stored in this block to a usable DecompressedBlock object.
		/// This actually just creates a DecompressedBlock and passes the raw data to it.
		/// </summary>
		/// <returns>A DecompressedBlock object</returns>
		public DecompressedBlock Decode( ) {
			return new DecompressedBlock( this );
		}

		/// <summary>
		/// Encode (or "compresses") a DecompressedBlock to raw data to be stored in the lightmap file. This will overwrite any 
		/// current data in this compressed block.
		/// </summary>
		/// <param name="block"></param>
		public void Encode( DecompressedBlock block ) {
			block.EncodeTo( this );
		}
	}
}
