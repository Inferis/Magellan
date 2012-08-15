using System;
using System.Drawing;
using System.Collections;
using System.IO;
using EU2.Data;

namespace EU2.Map
{
	/// <summary>
	/// Summary description for Boundboxes.
	/// </summary>
	public class BoundBoxes : EU2.IO.IStreamWriteable
	{
		ProvinceBoundBox[] boxes;

		public BoundBoxes() {
			boxes = new ProvinceBoundBox[Province.InternalCount];
			for ( int i=0; i<Province.MaxValue; ++i ) boxes[i] = new ProvinceBoundBox( Rectangle.Empty, (ushort)i );
		}

		public BoundBoxes( ProvinceBoundBox[] originals ) {
			boxes = new ProvinceBoundBox[Province.InternalCount];
			originals.CopyTo( boxes, 0 );
		}

		public BoundBoxes( BinaryReader reader ) : this() {
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			int i=0;
			try {
				for ( i=0; i<Province.MaxValue; ++i ) boxes[i].ReadFrom( reader );
			}
			catch ( System.IO.EndOfStreamException ) {
				for ( ; i<Province.MaxValue; ++i ) boxes[i] = new ProvinceBoundBox( 0, 0, 0, 0, (ushort)i );
			}
		}

		public void WriteTo( BinaryWriter writer ) {
			for ( int i=0; i<Province.MaxValue; ++i ) boxes[i].WriteTo( writer );
		}

		public void WriteTo( Stream stream ) {
			WriteTo( new BinaryWriter( stream ) );
		}

		public ProvinceBoundBox this[int index] {
			get {
				if ( index < 0 || index > Province.MaxValue ) throw new ArgumentOutOfRangeException();
				return boxes[index];
			}
			set {
				if ( index < 0 || index > Province.MaxValue ) throw new ArgumentOutOfRangeException();
				boxes[index] = value;
			}
		}

		public ProvinceBoundBox[] Boxes {
			get { return boxes; }
		}

		public Rectangle[] GetBoxes( ushort[] ids ) {
			Rectangle[] result = new Rectangle[ids.Length];
			for ( int i=0; i<result.Length; ++i ) {
				result[i] = boxes[ids[i]].Box;
			}

			return result;
		}

		public ProvinceBoundBox[] GetAllIntersectingWith( Rectangle rect  ) {
			ArrayList resultList = new ArrayList();

			for ( int i=0; i<boxes.Length; ++i ) {
				if ( rect.IntersectsWith( boxes[i].Box ) ) resultList.Add( boxes[i] ); 
			}

			ProvinceBoundBox[] result = new ProvinceBoundBox[resultList.Count];
			resultList.CopyTo( result );
			return result;
		}

		public ProvinceBoundBox[] GetAllIntersectingWith( Point point ) {
			return GetAllIntersectingWith( new Rectangle( point, new Size( 1, 1 ) ) );
		}

		public ProvinceBoundBox[] GetAllIntersectingWith( ProvinceBoundBox box ) {
			return GetAllIntersectingWith( box.Box );
		}
		
		public ProvinceBoundBox[] GetAllContainedIn( Rectangle rect ) {
			ArrayList resultList = new ArrayList();

			for ( int i=0; i<boxes.Length; ++i ) {
				if ( rect.Contains( boxes[i].Box ) ) resultList.Add( boxes[i] ); 
			}

			ProvinceBoundBox[] result = new ProvinceBoundBox[resultList.Count];
			resultList.CopyTo( result );
			return result;
		}
	}
}
