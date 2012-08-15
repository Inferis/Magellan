using System;
using System.IO;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for CompressedBlock.
	/// </summary>
	public class CompressedBlock : GenericMapBlock {
		public enum CompareResult {
			SizeMismatch = 0,
			DataMismatch = -1,
			Equal = 1
		}

		public CompressedBlock( )  {
			this.offset = -1;
			this.size = -1;
			this.data = null;
		}

		public CompressedBlock( int offset, int size )  {
			this.offset = offset;
			this.size = size;
			this.data = null;
		}

		public CompressedBlock( byte[] data )  {
			this.offset = -1;
			this.size = -1;
			this.data = new byte[data.Length];
			data.CopyTo( this.data, 0 );
		}

		public CompressedBlock( int offset, int offset2, MemoryStream stream )  {
			stream.Seek( offset, SeekOrigin.Begin );

			int bufsize = 2048;
			int size = -1;
			byte[] buffer = null;
			while ( size < 0 && bufsize < 65000 ) {
				buffer = new byte[bufsize];
				stream.Read( buffer, 0, bufsize );

				size = MapBlock.CalculateCompressedSize( buffer );
				bufsize += 2048;
			}

			if ( size < 0 ) throw new InvalidOperationException( "Could not detect block size at offset " + offset );

			System.Diagnostics.Debug.Assert( size == offset2-offset, "Bad size calc: " + size + " vs " + (offset2-offset) );
			this.offset = -1;
			this.size = -1;
			this.data = new byte[size];
			Array.Copy( buffer, 0, data, 0, size );
		}

		/*
		 * public virtual bool Equals( CompressedBlock other ) {
			if ( data == null && other.data == null ) return true;
			if ( data == null || other.data == null ) return false;
			if ( data.Length != other.data.Length ) return false;
			if ( data != other.data ) return true;

			return true;
		}*/

		public void ReadFrom( BinaryReader reader ) {
			if ( offset < 0 || size <= 0 ) return;

			this.data = new byte[size];
			//reader.BaseStream.Seek( offset, SeekOrigin.Begin );
			int read = 0;
			while ( read < size ) {
				read += reader.Read( data, read, size-read );
			}

			// Set offset and size to -1 to mark that this block is read and contains data.
			offset = -1;
			size = -1;
		}

		public void ReadFrom2( BinaryReader reader ) {
			if ( offset < 0 ) return;

			this.data = new byte[4096];

			//reader.BaseStream.Seek( offset, SeekOrigin.Begin );
			int read = 0;
			while ( read < size ) {
				read += reader.Read( data, read, size-read );
			}

			// Set offset and size to -1 to mark that this block is read and contains data.
			offset = -1;
			size = -1;
		}

		public void WriteTo( BinaryWriter writer ) {
			if ( this.data == null ) return;

			writer.Write( data, 0, data.Length );
		}

		public byte[] Data {
			get {
				return data;
			}
			set { 
				data = null;
				// copy the byte data into a new buffer
				if ( value != null ) { 
					data = new byte[value.Length];
					value.CopyTo( data, 0 );
				}
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

		public override bool IsCompressed() { 
			return true; 
		}

		public virtual bool Equals( CompressedBlock other ) {
			return Compare( other ) == CompareResult.Equal;
		}

		public virtual CompareResult Compare( CompressedBlock other ) {
			if ( data.Length != other.data.Length ) return CompareResult.SizeMismatch;

			for ( int i=data.Length-1; i>=0; --i ) {
				if ( data[i] != other.data[i] ) return CompareResult.DataMismatch;
			}

			return CompareResult.Equal;
		}

		byte[] data;
		int offset, size;
	}
}
