using System;
using System.Drawing;
using EU2.Map.Codec;
using EU2.Map.Drawing;
using EU2.Map;

namespace BlockVisNew
{
	/// <summary>
	/// Summary description for GrayScaledShader.
	/// </summary>
	public class IDShader : IShader16
	{
		ushort pid;

		public IDShader( ) {
			this.pid = 0;
		}

		public IDShader( ushort pid ) {
			this.pid = pid;
		}

		public short[] Shade16( RawImage image ) {
			short[] buffer = new short[image.PixelCount];
			int bufidx = 0;
			Pixel[,] memory = image.Memory;
			int height = image.Size.Height;
			int width = image.Size.Width;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					buffer[bufidx++] = (short)(memory[x,y].ID << 4);
				}
			}

			return buffer;
		}
	}
}
