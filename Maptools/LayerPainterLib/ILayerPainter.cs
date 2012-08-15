using System;
using EU2.Map;
using System.Drawing;

namespace LayerPainter
{
	/// <summary>
	/// Summary description for ILayerPainter.
	/// </summary>
	public interface ILayerPainter
	{
		string Name { get; }
        bool Enabled { get; set; }

		void QuickPaint( Graphics g, ILightmapDimensions m, Rectangle area );
		void Paint( Graphics g, ILightmapDimensions m, Rectangle area );
	}
}
