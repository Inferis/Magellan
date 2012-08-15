using System;
using System.Drawing;
using System.Drawing.Imaging;
using EU2MapEditorControls;
using EU2.Map;
using EU2.Map.Drawing;
using EU2.Map.Codec;

namespace MapToolsLib.LayerPainters
{
	/// <summary>
	/// Summary description for ColorLayerPainter.
	/// </summary>
	public class MapLayerPainter : LayerPainterBase
	{
		private ShaderProxy shaderproxy;
		private Lightmap[] sources;
		private int srcindex;

		public MapLayerPainter( ) {
			shaderproxy = null;
			sources = new Lightmap[3];
			srcindex = 0;
		}

		public Lightmap[] Sources {
			get { return sources; }
		}

		public Lightmap this[int index] {
			get {
				if ( index < 1 || index > 3 ) throw new ArgumentOutOfRangeException();
				return sources[index-1];
			}
			set {
				if ( index < 1 || index > 3 ) throw new ArgumentOutOfRangeException();
				sources[index-1] = value;
			}
		}

		public int CurrentSource {
			get { 
				return srcindex+1;
			}
			set {
				if ( value < 1 || value > 3 ) throw new ArgumentOutOfRangeException();
				srcindex = value-1;
				OnPropertyChange( EventArgs.Empty );
			}
		}

		public ShaderProxy ShaderProxy  {
			get { return shaderproxy; }
			set { 
				shaderproxy = value; 
				OnPropertyChange( EventArgs.Empty ); 
			}
		}

		#region LayerPainterBase Members

		public override void QuickPaint(System.Drawing.Graphics g, EU2.Map.ILightmapDimensions m, System.Drawing.Rectangle area) {
			Paint( g, m, area );
		}

		public override void Paint(System.Drawing.Graphics g, EU2.Map.ILightmapDimensions m, System.Drawing.Rectangle area) {
			if ( sources[srcindex] == null || shaderproxy == null ) return;

			Rectangle actualarea = m.CoordMap.BlocksToActual( area );

			bool decomp = sources[srcindex].VolatileDecompression;
			sources[srcindex].VolatileDecompression = true;
			RawImage raw = sources[srcindex].DecodeImage( actualarea );

			Bitmap canvas = new Bitmap( raw.Bounds.Width, raw.Bounds.Height, PixelFormat.Format32bppRgb );
			BitmapData cdata = canvas.LockBits( new Rectangle( new Point( 0, 0 ), canvas.Size ), ImageLockMode.WriteOnly, canvas.PixelFormat );
			int[] lightbuf = shaderproxy.Shade32( raw );
			System.Runtime.InteropServices.Marshal.Copy( lightbuf, 0, cdata.Scan0, lightbuf.Length );

			canvas.UnlockBits( cdata );

			g.DrawImageUnscaled( canvas, 0, 0 );
			sources[srcindex].VolatileDecompression = decomp;
		}

		public override string Name { get { return "Map"; } }

		#endregion
	}
}
