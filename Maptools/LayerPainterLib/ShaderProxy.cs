using System;
using EU2.Map.Codec;
using EU2.Map.Drawing;

namespace LayerPainter {
	/// <summary>
	/// Summary description for ShaderProxy.
	/// </summary>
	public abstract class ShaderProxy : IShader1632 {
		public abstract short[] Shade16( RawImage image ); 
		public abstract int[] Shade32( RawImage image ); 

		public bool DrawBorders {
			get { return drawborders; }
			set { drawborders = value; }
		}

		protected bool drawborders;
	}
}
