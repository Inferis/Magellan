using System;
using System.Drawing;
using EU2MapEditorControls;

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
	}
}
