using System;
using System.Drawing;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerMask.
	/// </summary>
	public class LayerMask
	{
		public LayerMask( Rectangle bounds, Bitmap image ) {
			if ( bounds.Size != image.Size ) throw new ArgumentOutOfRangeException();
			this.location = bounds.Location;
			this.image = image;
		}

		public Rectangle Bounds {
			get { return new Rectangle( location, image == null ? Size.Empty : image.Size ); }
		}

		public int X {
			get { return location.X; }
			set { location.X = value; }
		}

		public int Y {
			get { return location.Y; }
			set { location.Y = value; }
		}

		public int Width {
			get { return image == null ? 0 : image.Width; }
		}

		public int Height {
			get { return image == null ? 0 : image.Height; }
		}

		public Bitmap Image {
			get { return image; }
			set { image = value; }
		}

		Point location;
		Bitmap image;
	}
}
