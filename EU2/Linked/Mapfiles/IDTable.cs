using System;
using System.IO;
using System.Drawing;

namespace EU2.Data.Mapfiles
{
	/// <summary>
	/// Summary description for ID.
	/// </summary>
	public class IDTable : InstallLinkedObject, IDisposable
	{
		private const int BLOCKSIZE = 16;

		System.IO.FileStream stream;
		Size mapsize;

		public IDTable( EU2.Install install ) : base( install ) {
			// Get the mapsize from the boundbox table
			this.mapsize = install.Map.MapSize;

			OpenStream();
		}

		~IDTable() {
			Dispose();
		}

		public void Dispose() {
			if ( stream != null ) stream.Close();
			stream = null;
		}

		public int HitTest( int x, int y ) {
			// Make sure that the X location wraps around
			x %= mapsize.Width;
			// Get the relative offset for the Y location
			int offset = Offset( y );
			
			stream.Seek( (this.mapsize.Height+1+offset)*4, System.IO.SeekOrigin.Begin );
			BinaryReader reader = new BinaryReader( stream );
			
			while ( true ) {
				IDTableItem item = new IDTableItem( reader );
				if ( item.StartX <= x && item.EndX >= x ) 
					return item.ProvinceID;
				if ( item.EndX >= this.mapsize.Width ) 
					break;
			}

			return -1;
		}

		public int HitTest( Point location ) {
			return HitTest( location.X, location.Y );
		}

		private int Offset( int index ) {
			if ( index < 0 || index >= this.mapsize.Height ) throw new ArgumentOutOfRangeException();
			stream.Seek( (index+1)*4, System.IO.SeekOrigin.Begin );
			System.IO.BinaryReader reader = new System.IO.BinaryReader( stream );
			return reader.ReadInt32();
		}

		private void OpenStream() {
			if ( stream != null ) return;

			string filename = install.GetMapFile( "id.tbl" );
			stream = new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read );
		}
	}

	internal struct IDTableItem {
		short startX;
		short provinceID;
		short endX;

		public IDTableItem( short startX, short provinceID, short endX ) {
			this.startX = startX;
			this.provinceID = provinceID;
			this.endX = endX;
		}

		public IDTableItem( BinaryReader reader ) {
			this.startX = reader.ReadInt16();
			this.provinceID = reader.ReadInt16();
			this.endX = reader.ReadInt16();
			reader.ReadInt16(); // read dummy 0
		}

		public short StartX {
			get { return startX; }
		}

		public short ProvinceID {
			get { return provinceID; }
		}

		public short EndX {
			get { return endX; }
		}
	}
}
