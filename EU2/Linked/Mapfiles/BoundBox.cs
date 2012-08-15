using System;
using System.IO;
using System.Drawing;
using System.Collections;

namespace EU2.Data.Mapfiles
{
	/// <summary>
	/// Summary description for BoundBox.
	/// </summary>
	public class BoundBox : IDisposable, IEnumerable {
		System.IO.FileStream stream;
		private const int BLOCKSIZE = 16;
		EU2.Install install;
		Hashtable cache;

		public BoundBox( EU2.Install install )
		{
			this.install = install;
			cache = new Hashtable();
			OpenStream();
		}

		~BoundBox() {
			Dispose();
		}

		public void Dispose() {
			if ( stream != null ) stream.Close();
			stream = null;
		}

		public void ClearCache() {
			cache.Clear();
		}

		public ProvinceBoundBox this[int id] {
			get {
				if ( id == 0 ) {
					Size sz = install.Map.MapSize;
					return new ProvinceBoundBox( 0, new Rectangle( 0, 0, sz.Width, sz.Height ) );
				}

				if ( id < 1 || (id+2)*BLOCKSIZE > stream.Length ) throw new ArgumentOutOfRangeException( "id", id, "The id parameter is out of the acceptable range." );
				
				if ( !cache.Contains( id ) ) {
					OpenStream();
					stream.Seek( id*BLOCKSIZE, SeekOrigin.Begin );
					BinaryReader reader = new BinaryReader( stream );
					
					cache[id] = new ProvinceBoundBox( id, ReadRectangle( reader ) );
				}

				return (ProvinceBoundBox)(cache[id]);
			}
		}

		//public Size MapSize {
		//	get {
		//		OpenStream();
		//		stream.Seek( 0, SeekOrigin.Begin );
		//		BinaryReader reader = new BinaryReader( stream );
		//		return new Size( reader.ReadInt32(), reader.ReadInt32() );
		//	}
		//}

		public System.Collections.IEnumerator GetEnumerator() {
			ArrayList list = new ArrayList();

			// Add mapsize as first item
			Size mapsize = install.Map.MapSize;
			list.Add( new ProvinceBoundBox( 0, new Rectangle( 0, 0, mapsize.Width, mapsize.Height ) ) );

			OpenStream();
			stream.Seek( BLOCKSIZE, SeekOrigin.Begin );
			BinaryReader reader = new BinaryReader( stream );
			int id = 1;
			while ( true ) {
				try {
					list.Add( new ProvinceBoundBox( id++, ReadRectangle( reader ) ) );
				}
				catch ( AtEndOfStreamException ) {
					break;
				}
				catch ( BadRectangleException ) {
					break;
				}
			}

			return list.GetEnumerator();
		}

		public ProvinceBoundBox[] IntersectingWith( Rectangle rect ) {
			ArrayList resultList = new ArrayList();
			IEnumerator boxEnum = this.GetEnumerator();

			for ( boxEnum.Reset(), boxEnum.MoveNext(); boxEnum.MoveNext(); ) {
				ProvinceBoundBox box = (ProvinceBoundBox)(boxEnum.Current);
				if ( rect.IntersectsWith( box.Box ) )
					resultList.Add( box ); 
			}

			ProvinceBoundBox[] result = new ProvinceBoundBox[resultList.Count];
			resultList.CopyTo( result );
			return result;
		}

		public ProvinceBoundBox[] IntersectingWith( Point point ) {
			return IntersectingWith( new Rectangle( point, new Size( 1, 1 ) ) );
		}
		
		public ProvinceBoundBox[] ContainedIn( Rectangle rect ) {
			ArrayList resultList = new ArrayList();
			IEnumerator boxEnum = this.GetEnumerator();

			for ( boxEnum.Reset(), boxEnum.MoveNext(); boxEnum.MoveNext(); ) {
				ProvinceBoundBox box = (ProvinceBoundBox)boxEnum.Current;
				if ( rect.Contains( box.Box ) )
					resultList.Add( box ); 
			}

			ProvinceBoundBox[] result = new ProvinceBoundBox[resultList.Count];
			resultList.CopyTo( result );
			return result;
		}

		private void OpenStream() {
			if ( stream != null ) return;

			string filename = install.MapPath + "\\boundbox.tbl";
			stream = new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read );
		}

		private Rectangle ReadRectangle( BinaryReader reader ) {
			int x = reader.ReadInt32();
			int y = reader.ReadInt32();
			int w = reader.ReadInt32()-x;
			int h = reader.ReadInt32()-y;

			if ( w < 0 || h < 0 ) 
				throw new BadRectangleException();

			return new Rectangle( x, y, w, h );
		}

	}

	public struct ProvinceBoundBox {
		Rectangle box;
		int id;

		public ProvinceBoundBox( int id, Rectangle box ) {
			this.id = id;
			this.box = box;
		}

		public int ID {
			get { return id; }
		}

		public Rectangle Box {
			get { return box; }
		}
	}

	public class BadRectangleException : Exception {};
}
