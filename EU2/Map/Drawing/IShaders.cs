using System;
using System.Drawing;
using EU2.Map.Codec;

namespace EU2.Map.Drawing
{
	/// <summary>
	/// Interface for shading to a 16bpp image.
	/// This interface exports exactly one method: Shade16()
	/// </summary>
	public interface IShader16 {
		short[] Shade16( RawImage image );
	}

	/// <summary>
	/// Interface for shading to a 32bpp (argb) image.
	/// This interface exports exactly one method: Shade32()
	/// </summary>
	public interface IShader32 {
		int[] Shade32( RawImage image );
	}


	/// <summary>
	/// Interface for shading to a 32bpp (argb) image.
	/// This interface exports exactly one method: Shade32()
	/// </summary>
	public interface IMultiShader16 {
		void MultiShade16( RawImage image, out short[] colorbuffer, out ushort[] idbuffer, out byte[] borderbuffer );
	}


	/// <summary>
	/// Interface for shading to a 32bpp (argb) image.
	/// This interface exports exactly one method: Shade32()
	/// </summary>
	public interface IMultiShader32 {
		void MultiShade32( RawImage image, out int[] colorbuffer, out ushort[] idbuffer, out byte[] borderbuffer );
	}


	/// <summary>
	/// </summary>
	public interface IUnshader16 {
		RawImage Unshade16( int zoom, Rectangle bounds, short[] colorbuffer, ushort[] idbuffer, byte[] borderbuffer );
	}

	/// <summary>
	/// </summary>
	public interface IUnshader32 {
		RawImage Unshade32( int zoom, Rectangle bounds, int[] colorbuffer, ushort[] idbuffer, byte[] borderbuffer );
	}

	public interface IShader1632 : IShader16, IShader32 {
	}

	public interface IUnshader1632 : IUnshader16, IUnshader32 {
	}

}
