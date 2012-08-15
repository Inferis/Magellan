using System;
using System.IO;
using System.Drawing;

namespace EU2.Map
{
	/// <summary>
	/// Summary description for ProvinceBoundBox.
	/// </summary>
	public struct ProvinceBoundBox {
		int left, top, right, bottom;
		ushort provinceID;

		public ProvinceBoundBox( Rectangle box, ushort id ) {
			left = box.Left;
			right = box.Right;
			top = box.Top;
			bottom = box.Bottom;
			provinceID = id;
		}

		public ProvinceBoundBox( Point topleft, Point bottomright, ushort id ) {
			left = topleft.X;
			right = bottomright.X;
			top = topleft.Y;
			bottom = bottomright.Y;
			provinceID = id;
		}

		public ProvinceBoundBox( int left, int top, int right, int bottom, ushort id ) {
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
			provinceID = id;
		}

		public void ReadFrom( BinaryReader reader ) {
			left = reader.ReadInt32();
			top = reader.ReadInt32();
			right = reader.ReadInt32();
			bottom = reader.ReadInt32();
		}

		public void WriteTo( BinaryWriter writer ) {
			writer.Write( left );
			writer.Write( top );
			writer.Write( right );
			writer.Write( bottom );
		}

		public ushort ProvinceID {
			get { return provinceID; }
			set { provinceID = value; }
		}

		public Rectangle Box {
			get { return Rectangle.FromLTRB( left, top, right, bottom ); }
			set { 
				left = value.Left;
				top = value.Top;
				right = value.Right;
				bottom = value.Bottom;
			}
		}

		public Point TopLeft {
			get { return new Point( left, top ); }
			set { 
				left = value.X;
				top = value.Y;
			}
		}

		public Point BottomRight {
			get { return new Point( right, bottom ); }
			set { 
				right = value.X;
				bottom = value.Y;
			}
		}

		public int Left {
			get { return left; }
			set { left = value; }
		}
	
		public int Top {
			get { return top; }
			set { top = value; }
		}

		public int Right {
			get { return right; }
			set { right = value; }
		}

		public int Bottom {
			get { return bottom; }
			set { bottom = value; }
		}

		public void Sanitize() {
			int tmp;
			
			if ( left > right ) {
				tmp = left;
				left = right;
				right = tmp;
			}

			if ( top > bottom ) {
				tmp = top;
				top = bottom;
				bottom = tmp;
			}
		}

	}
}
