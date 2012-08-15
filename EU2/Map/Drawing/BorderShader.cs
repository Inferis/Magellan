using System;
using EU2.Map.Codec;

namespace EU2.Map.Drawing
{
	/// <summary>
	/// Summary description for BorderShader.
	/// </summary>
	public class BorderShader : IShader32 {

		public int[] Shade32( RawImage image ) {
			int[] buffer = new int[image.PixelCount];
			int bufidx = 0;
			Pixel[,] memory = image.Memory;
			int height = image.Size.Height;
			int width = image.Size.Width;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					unchecked {
						buffer[bufidx++] =  memory[x,y].Border == 0 ? 0 : (int)(0xFFFF0000);
					}
				}
			}

			return buffer;
		}

	}
}
