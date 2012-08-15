using System;
using EU2.Map.Drawing;
using EU2.Map.Codec;
using LayerPainter;

namespace MapExplorer
{
	/// <summary>
	/// Summary description for DefaultShaderProxy.
	/// </summary>
	public class GenericShaderProxy : ShaderProxy {
		public GenericShaderProxy( IShader1632 shader ) {
			this.shader = shader;
		}

		public override short[] Shade16( RawImage image ) {
			return shader.Shade16( image );
		}

		public override int[] Shade32( RawImage image ) {
			return shader.Shade32( image );
		}

		private IShader1632 shader;
	}
}
