using System;
using System.Drawing;
using EU2.Map;

namespace EU2
{
	/// <summary>
	/// Summary description for RegionSizeException.
	/// </summary>
	public class RegionSizeOverflowException : Exception {
		public RegionSizeOverflowException( Rectangle regionrect ) : base( "Region size overflow" ) {
			rect = regionrect;
			if ( rect.Right > Lightmap.BaseWidth ) rect.Width -= rect.Right-Lightmap.BaseWidth;
			if ( rect.Bottom > Lightmap.BaseHeight ) rect.Height -= rect.Bottom - Lightmap.BaseHeight;
		}

		public Rectangle Region {
			get { return rect; }
		}

		Rectangle rect;
	}
}
