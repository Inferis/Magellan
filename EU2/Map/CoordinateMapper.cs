using System;
using System.Drawing;

namespace EU2.Map
{
	/// <summary>
	/// Summary description for CoordinateMapper.
	/// </summary>
	public class CoordinateMapper {
		public CoordinateMapper( int zoom ) {
			this.zoom = zoom;
		}

		#region ActualToZoomed
		public Point ActualToZoomed( Point point ) {
			return new Point( point.X >> zoom, point.Y >> zoom );
		}

		public Size ActualToZoomed( Size size ) {
			return new Size( size.Width >> zoom, size.Height >> zoom );
		}

		public Rectangle ActualToZoomed( Rectangle rect ) {
			return new Rectangle( rect.X >> zoom, rect.Y >> zoom, rect.Width >> zoom, rect.Height >> zoom );
		}
		#endregion

		#region ZoomedToActual
		public Point ZoomedToActual( Point point ) {
			return new Point( point.X << zoom, point.Y << zoom );
		}

		public Size ZoomedToActual( Size size ) {
			return new Size( size.Width << zoom, size.Height << zoom );
		}

		public Rectangle ZoomedToActual( Rectangle rect ) {
			return new Rectangle( rect.X << zoom, rect.Y << zoom, rect.Width << zoom, rect.Height << zoom );
		}
		#endregion

		#region ActualToBlocks
		public int ActualToBlocks( int v ) {
			return v >> Lightmap.BlockFactor;
		}

		public Point ActualToBlocks( Point point ) {
			return new Point( point.X >> Lightmap.BlockFactor, point.Y >> Lightmap.BlockFactor );
		}

		public Size ActualToBlocks( Size size ) {
			return new Size( size.Width >> Lightmap.BlockFactor, size.Height >> Lightmap.BlockFactor );
		}

		public Rectangle ActualToBlocks( Rectangle rect ) {
			return new Rectangle( 
				rect.X >> Lightmap.BlockFactor, rect.Y >> Lightmap.BlockFactor, 
				rect.Width >> Lightmap.BlockFactor, rect.Height >> Lightmap.BlockFactor );
		}
		#endregion

		#region BlocksToActual
		public Point BlocksToActual( Point point ) {
			return new Point( point.X << Lightmap.BlockFactor, point.Y << Lightmap.BlockFactor );
		}

		public Size BlocksToActual( Size size ) {
			return new Size( size.Width << Lightmap.BlockFactor, size.Height << Lightmap.BlockFactor );
		}

		public Rectangle BlocksToActual( Rectangle rect ) {
			return new Rectangle( rect.X << Lightmap.BlockFactor, rect.Y << Lightmap.BlockFactor, rect.Width << Lightmap.BlockFactor, rect.Height << Lightmap.BlockFactor );
		}
		#endregion
			
		#region ActualToZoomedBlocks
		public int ActualToZoomedBlocks( int v ) {
			return v >> (zoom+Lightmap.BlockFactor);
		}

		public Point ActualToZoomedBlocks( Point point ) {
			return new Point( point.X >> (zoom+Lightmap.BlockFactor), point.Y >> (zoom+Lightmap.BlockFactor) );
		}

		public Size ActualToZoomedBlocks( Size size ) {
			return new Size( size.Width >> (zoom+Lightmap.BlockFactor), size.Height >> (zoom+Lightmap.BlockFactor) );
		}

		public Rectangle ActualToZoomedBlocks( Rectangle rect ) {
			return new Rectangle( 
				rect.X >> (zoom+Lightmap.BlockFactor), rect.Y >> (zoom+Lightmap.BlockFactor), 
				rect.Width >> (zoom+Lightmap.BlockFactor), rect.Height >> (zoom+Lightmap.BlockFactor) );
		}
		#endregion

		#region ZoomedBlocksToActual
		public Point ZoomedBlocksToActual( Point point ) {
			return new Point( point.X << (zoom+Lightmap.BlockFactor), point.Y << (zoom+Lightmap.BlockFactor) );
		}

		public Size ZoomedBlocksToActual( Size size ) {
			return new Size( size.Width << (zoom+Lightmap.BlockFactor), size.Height << (zoom+Lightmap.BlockFactor) );
		}

		public Rectangle ZoomedBlocksToActual( Rectangle rect ) {
			return new Rectangle( 
				rect.X << (zoom+Lightmap.BlockFactor), rect.Y << (zoom+Lightmap.BlockFactor), 
				rect.Width << (zoom+Lightmap.BlockFactor), rect.Height << (zoom+Lightmap.BlockFactor) );
		}
		#endregion
			
		#region FitToGrid
		public Point FitToGrid( Point point ) {
			return new Point( 
				(point.X >> (Lightmap.BlockFactor)) << (Lightmap.BlockFactor), 
				(point.Y >> (Lightmap.BlockFactor)) << (Lightmap.BlockFactor) );
		}

		public Size FitToGrid( Size size ) {
			return new Size( 
				((size.Width + ((Lightmap.BlockSize>>1))) >> Lightmap.BlockFactor) << Lightmap.BlockFactor, 
				((size.Height + ((Lightmap.BlockSize>>1))) >> Lightmap.BlockFactor) << Lightmap.BlockFactor );
		}

		public Rectangle FitToGrid( Rectangle rect ) {
			Rectangle result = rect;
			result.X = (rect.X >> Lightmap.BlockFactor) << Lightmap.BlockFactor;
			result.Y = (rect.Y >> Lightmap.BlockFactor) << Lightmap.BlockFactor;
			result.Width = (((rect.Right + (Lightmap.BlockSize-1)) >> Lightmap.BlockFactor) << Lightmap.BlockFactor) - result.Left;
			result.Height = (((rect.Bottom + (Lightmap.BlockSize-1)) >> Lightmap.BlockFactor) << Lightmap.BlockFactor) - result.Top;

			return result;
		}
		#endregion

		#region FitToZoomedGrid
		public Point FitToZoomedGrid( Point point ) {
			return new Point( 
				(point.X >> (Lightmap.BlockFactor+zoom)) << (Lightmap.BlockFactor+zoom), 
				(point.Y >> (Lightmap.BlockFactor+zoom)) << (Lightmap.BlockFactor+zoom) );
		}

		public Size FitToZoomedGrid( Size size ) {
			return new Size( 
				((size.Width + ((Lightmap.BlockSize>>1)<<zoom)) >> (Lightmap.BlockFactor+zoom)) << (Lightmap.BlockFactor+zoom), 
				((size.Height + ((Lightmap.BlockSize>>1)<<zoom)) >> (Lightmap.BlockFactor+zoom)) << (Lightmap.BlockFactor+zoom) );
		}

		public Rectangle FitToZoomedGrid( Rectangle rect ) {
			Rectangle result = rect;
			result.X = (rect.X >> (Lightmap.BlockFactor+zoom)) << (Lightmap.BlockFactor+zoom);
			result.Y = (rect.Y >> (Lightmap.BlockFactor+zoom)) << (Lightmap.BlockFactor+zoom);
			result.Width = (((rect.Right + (Lightmap.BlockSize-1)) >> (Lightmap.BlockFactor+zoom)) << (Lightmap.BlockFactor+zoom)) - result.Left;
			result.Height = (((rect.Bottom + (Lightmap.BlockSize-1)) >> (Lightmap.BlockFactor+zoom)) << (Lightmap.BlockFactor+zoom)) - result.Top;

			return result;
		}
		#endregion

		private int zoom;
	}
}
