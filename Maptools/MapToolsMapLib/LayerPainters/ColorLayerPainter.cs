using System;
using System.Drawing;
using LayerPainter;

namespace MapToolsLib.LayerPainters
{
	/// <summary>
	/// Summary description for ColorLayerPainter.
	/// </summary>
	public class ColorLayerPainter : ILayerPainter
	{
		public ColorLayerPainter()
		{
		}

		#region ILayerPainter Members

		public void QuickPaint(System.Drawing.Graphics g, EU2.Map.ILightmapDimensions m, System.Drawing.Rectangle area) {
			g.FillRectangle( Brushes.Red, new Rectangle( Point.Empty, m.CoordMap.BlocksToActual( area.Size ) ) );
		}

		public void Paint(System.Drawing.Graphics g, EU2.Map.ILightmapDimensions m, System.Drawing.Rectangle area) {
			g.FillRectangle( Brushes.Green, new Rectangle( Point.Empty, m.CoordMap.BlocksToActual( area.Size ) ) );
		}

		public string Name { get { return "ColorLayerPainter"; } }

		#endregion

        #region ILayerPainter Members


        public bool Enabled {
            get {
                return true;
            }
            set {
            }
        }

        #endregion
    }
}
