using System;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PSD
{
	public enum LayerClipping {
		Base,
		NonBase
	}

	/// <summary>
	/// Summary description for Layer.
	/// </summary>
	public class Layer
	{
		public Layer( File file ) : this( "", Point.Empty, null ) {
			this.file = file;
		}

		public Layer( string name, int x, int y, Bitmap src ) : this( name, new Point( x, y ), src ) {
		}
			
		public Layer( string name, Point location, Bitmap src ) {
			this.file = null;
			this.name = name;
			this.location = location;
			image = src == null ? null : ((Bitmap)src.Clone());
			clipping = LayerClipping.Base;
			opacity = 255;
			visible = true;
			protecttrans = false;
			mode = LayerMode.Normal;
			mask = null;
			adjustments = new LayerAdjustment[0];
		}

		#region Public Properties
		public string Name {
			get { return name; }
			set { name = value; }
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

		public int Opacity {
			get { return opacity; }
			set { opacity = value; if ( opacity < 0 ) opacity = 0; if ( opacity > 255 ) opacity = 255; }
		}

		public float OpacityF {
			get { return ((float)opacity)/255; }
			set { opacity = (int)(value*255); if ( opacity < 0 ) opacity = 0; if ( opacity > 255 ) opacity = 255; }
		}

		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}

		public bool ProtectTransparancy {
			get { return protecttrans; }
			set { protecttrans = value; }
		}

		public LayerMode Mode {
			get { return mode; }
			set { mode = value; }
		}
		public LayerClipping Clipping {
			get { return clipping; }
			set { clipping = value; }
		}

		public Bitmap Image {
			get { return image; }
			set { if ( image != null ) image.Dispose(); image = value; }
		}

		public File File {
			get { return file; }
			set { file = value; }
		}

		public LayerMask Mask {
			get { return mask; }
			set { mask = value; }
		}

		public LayerAdjustment[] Adjustments {
			get { return adjustments; }
			set { adjustments = value; }
		}

		#endregion

		public static Layer FromProtoLayer( ProtoLayer proto, File file ) {
			Layer result = new Layer( file );
			result.name = proto.Name;
			result.location = proto.Bounds.Location;
			result.clipping = proto.Clipping;
			result.visible = proto.Visible;
			result.opacity = proto.Opacity;
			result.protecttrans = proto.ProtectTransparancy;
			result.image = proto.BuildImage();
			result.mode = LayerMode.FromKey( proto.Blendkey );
			result.mask = proto.BuildMask();
			result.adjustments = new LayerAdjustment[proto.Adjustments.Length];
			proto.Adjustments.CopyTo( result.adjustments, 0 );

			return result;
		}


		#region Private Fields
		File file;
		string name;
		private Point location;
		private int opacity;
		private LayerClipping clipping;
		private bool protecttrans;
		private bool visible;
		private Bitmap image; 
		private LayerMode mode;
		private LayerMask mask;
		private LayerAdjustment[] adjustments;
		#endregion
	}

}
