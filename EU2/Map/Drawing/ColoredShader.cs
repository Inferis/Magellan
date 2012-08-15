using System;
using System.Drawing;
using EU2.Data;
using EU2.Map.Codec;

namespace EU2.Map.Drawing
{
	/// <summary>
	/// Summary description for GrayScaledShader.
	/// </summary>
	public class ColoredShader : IShader1632
	{
		public ColoredShader( ColorScales scales, ProvinceList provinces, bool drawborders ) {
			this.shades = scales.Shades;
			this.provinces = provinces;
			this.drawborders = drawborders;

			this.rangecheck = new byte[512];
			for( int r=0; r<512; r++ ) {
				if ( r >= 256 ) rangecheck[r] = 127;
				else if ( r >= 132 ) rangecheck[r] = (byte)(r-128);
				else rangecheck[r] = 4;
			}		
		}

		public int[] Shade32( RawImage image ) {
			int[] buffer = new int[image.PixelCount];
			int bufidx = 0;
			Pixel[,] memory = image.Memory;
			int height = image.Size.Height;
			int width = image.Size.Width;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					ushort owner = memory[x,y].IsBorder() && drawborders ? Province.BorderID : memory[x,y].ID;
					buffer[bufidx++] = shades[((rangecheck[(memory[x,y].Color-0+128+32)]<<6) | (int)(provinces[owner].Terrain.Color) )];
				}
			}

			return buffer;
		}

		public short[] Shade16( RawImage image ) {
			short[] buffer = new short[image.PixelCount];
			int bufidx = 0;
			Pixel[,] memory = image.Memory;
			int height = image.Size.Height;
			int width = image.Size.Width;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					ushort owner = memory[x,y].IsBorder() && drawborders ? Province.BorderID : memory[x,y].ID;
					int color32 = shades[((rangecheck[(memory[x,y].Color-0+128+32)]<<6) | (int)(provinces[owner].Terrain.Color) )];
					buffer[bufidx++] = (short)((((color32 >> 19) & 31) << 10) | (((color32 >> 11) & 31) << 5) | (((color32 >> 3) & 31)));
				}
			}

			return buffer;
		}

		private byte[] rangecheck;
		private bool drawborders;
		private int[] shades;
		private ProvinceList provinces;
	}
}
