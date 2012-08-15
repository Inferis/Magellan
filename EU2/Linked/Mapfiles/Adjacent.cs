using System;
using System.IO;

namespace EU2.Data.Mapfiles
{
	/// <summary>
	/// Summary description for Adjacent.
	/// </summary>
	public class AdjacencyTable : InstallLinkedObject, IDisposable {
		System.IO.FileStream stream;

		public AdjacencyTable( EU2.Install install ) : base( install ) {
			OpenStream();
		}

		~AdjacencyTable() {
			Dispose();
		}

		public void Dispose() {
			if ( stream != null ) stream.Close();
			stream = null;
		}

		public Adjacent[] this[int id] {
			get {
				stream.Seek( (id) << 2, SeekOrigin.Begin );
				int ofsStart = (int)stream.ReadByte() + (int)(stream.ReadByte()<<8) + (int)(stream.ReadByte()<<16) + (int)(stream.ReadByte()<<24);
				int ofsEnd = (int)stream.ReadByte() + (int)(stream.ReadByte()<<8) + (int)(stream.ReadByte()<<16) + (int)(stream.ReadByte()<<24);

				stream.Seek( (Province.MaxID+1)*4 + (ofsStart*12), SeekOrigin.Begin );
				int count = (ofsEnd-ofsStart); 
				Adjacent[] result = new Adjacent[count];
				for ( int i=0; i<count; ++i ) {
					result[i] = new Adjacent( stream );
				}

				return result;
			}
		}

		private void OpenStream() {
			if ( stream != null ) return;

			string filename = install.GetMapFile( "adjacent.tbl" );
			stream = new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read );
		}
	}
		
	public enum AdjacencyType {
		Normal = 0,
		River = 1
	}

	public class Adjacent {
		ushort id;
		AdjacencyType type; 

		public Adjacent( ushort id, AdjacencyType type ) {
			this.id = id;
			this.type = type;
		}

		public Adjacent( Stream stream ) {
			byte[] buffer = new byte[12];
			// Read only 8 bytes, the last 4 don't matter
			stream.Read( buffer, 0, 12 );

			this.id = (ushort)(buffer[0] + (buffer[1] << 8));
			this.type = (AdjacencyType)buffer[4];
		}

		public ushort ID {
			get { return id; }
			set { id = value; }
		}

		public AdjacencyType Type {
			get { return type; }
			set { type = value; }
		}
	}
}
