using System;
using System.Drawing;
using EU2.Data;
using EU2.Map.Codec;

namespace EU2.Map.Drawing
{
	/// <summary>
	/// Summary description for GrayScaledShader.
	/// </summary>
	public class TerrainShader : IShader32
	{
		public TerrainShader( ColorScales scales, ProvinceList provinces, IDMap idmap ) {
			this.shades = scales.Shades;
			this.provinces = provinces;

			// Build the range check array
			rangecheck = new byte[512];
			for( int r=0; r<512; r++ ) {
				if ( r >= 256 ) rangecheck[r] = 127;
				else if ( r >= 132 ) rangecheck[r] = (byte)(r-128);
				else rangecheck[r] = 4;
			}
		}

		public int[] Shade32( RawImage image ) {
			int[] buffer = new int[image.PixelCount];
			int bufidx = 0;

			for ( int y=0; y<image.Size.Height; ++y ) {
				for ( int x=0; x<image.Size.Width; ++x ) {
					buffer[bufidx++] = shades[((rangecheck[(image[x,y].Color-0+128+32)]<<6) | 
						               (int)(provinces[image[x,y].ID].Terrain.Color) )];
				}
			}

			return buffer;
		}

		private byte[] rangecheck;		// for drawing shades (used in derived classes)
		private int[] shades;
		private ProvinceList provinces;
	}
}
