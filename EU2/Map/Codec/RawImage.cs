using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using EU2.Map.Drawing;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for RawImage.
	/// </summary>
	public class RawImage
	{
		public RawImage( int zoom, Rectangle bounds ) : this( zoom, bounds, new Pixel[bounds.Width, bounds.Height] ) {
		}

		public RawImage( int zoom, Rectangle bounds, Pixel[,] memory ) {
			this.zoom = zoom;
			this.bounds = bounds;
			if ( memory.GetLength(0) != bounds.Width || memory.GetLength(1) != bounds.Height ) throw new ArgumentOutOfRangeException( "memory" );
			this.memory = memory;
		}

		
		public RawImage Shrink() {
			Rectangle smallbounds = bounds;
			smallbounds.X /= 2;
			smallbounds.Y /= 2;
			smallbounds.Width /= 2;
			smallbounds.Height /= 2;
			
			RawImage result = new RawImage( this.zoom+1, smallbounds );

			int[] colorbuffer;
			ushort[] idbuffer;
			byte[] borderbuffer;
			new GrayScaledShader().MultiShade32( this, out colorbuffer, out idbuffer, out borderbuffer );

			Bitmap orig, small;

			orig = Visualiser.CreateImage32( colorbuffer, bounds.Size );
			small = ResizeBitmap( orig, InterpolationMode.HighQualityBicubic );
			colorbuffer = Visualiser.CreateBuffer32( small );
			orig.Dispose(); small.Dispose();

			short[] idbuffercopy = new short[idbuffer.Length];
			for ( int i=0; i<idbuffer.Length; ++i ) idbuffercopy[i] = (short)idbuffer[i];
			orig = Visualiser.CreateImage16( idbuffercopy, bounds.Size );
			small = ResizeBitmap( orig, InterpolationMode.NearestNeighbor );
			idbuffercopy = Visualiser.CreateBuffer16( small );
			idbuffer = new ushort[idbuffercopy.Length];
			for ( int i=0; i<idbuffer.Length; ++i ) idbuffer[i] = (ushort)idbuffercopy[i];
			orig.Dispose(); small.Dispose();

			orig = Visualiser.CreateImage8( borderbuffer, bounds.Size );
			small = ResizeBitmap( orig, InterpolationMode.NearestNeighbor );
			borderbuffer = Visualiser.CreateBuffer8( small );
			orig.Dispose(); small.Dispose();
			
			return new GrayScaledShader().Unshade32( result.zoom+1, smallbounds, colorbuffer, idbuffer, borderbuffer );
		}

		private static Bitmap ResizeBitmap( Bitmap orig, InterpolationMode mode ) {
			Size smallsize = orig.Size;
			smallsize.Width /= 2;
			smallsize.Height /= 2;

			Bitmap result = new Bitmap( orig.Width / 2, orig.Height / 2, orig.PixelFormat );
			if ( orig.PixelFormat == PixelFormat.Format1bppIndexed ) return result;
			if ( orig.PixelFormat == PixelFormat.Format4bppIndexed ) return result;
			if ( orig.PixelFormat == PixelFormat.Format8bppIndexed ) return result;

			using ( Graphics g = Graphics.FromImage( result ) ) {
				g.InterpolationMode = mode;
				g.DrawImage( orig, new Rectangle( new Point( 0, 0 ), result.Size ), 0, 0, orig.Width, orig.Height, GraphicsUnit.Pixel ); 
		   }

			return result;
		}

		#region Public Properties
		public int X { get { return bounds.X; } }
		public int Y { get { return bounds.Y; } }
		public int Width { get { return bounds.Width; } }
		public int Height { get { return bounds.Height; } }

		public Point Location {get { return bounds.Location; } }
		public Size Size { get { return bounds.Size; } }

		public Rectangle Bounds { get { return bounds; } }
		public Rectangle ActualBounds { get { return new CoordinateMapper( zoom ).ZoomedToActual( bounds ); } }

		public int PixelCount {
			get { return memory.GetLength(0) * memory.GetLength(1); }
		}

		public Pixel[,] Memory {
			get { return memory; }
			set { memory = value; }
		}

		public Pixel this[int x, int y] {
			get { return memory[x,y]; }
		}

		#endregion

		#region Private Fields
		private int zoom;
		private Rectangle bounds;
		private Pixel[,] memory;
		#endregion
	}

	public enum CombineOperation {
		DiscardBlue
	}
}
