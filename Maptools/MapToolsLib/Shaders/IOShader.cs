using System;
using System.Drawing;
using EU2.Map.Codec;

namespace MapToolsLib
{
	/// <summary>
	/// Summary description for GrayScaledShader.
	/// </summary>
	public class IOShader
	{
		public IOShader( MapToolsLib.IIDConvertor idconvertor ) {
			idc = idconvertor;
		}

		public void MultiShade32( RawImage image, out int[] colorbuffer, out int[] idbuffer, out int[] borderbuffer ) {
			colorbuffer = new int[image.PixelCount];
			idbuffer = new int[image.PixelCount];
			borderbuffer = new int[image.PixelCount];

			int height = image.Size.Height;
			int width = image.Size.Width;
			Pixel[,] memory = image.Memory;

			int bufidx = 0;
			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					// shading
					byte color = (byte)(0xFF-(memory[x,y].Color << 2));
					colorbuffer[bufidx] = (color << 16) | (color << 8) | color;

					// don't bother for id, as it won't be used anyway.
					//idbuffer[bufidx] = idc.ConvertID( memory[x,y].ID );
					
					// Border
					unchecked {
						borderbuffer[bufidx] = memory[x,y].Border == 0 ? 0 : (int)(0xFFFF0000);
					}

					bufidx++;
				}
			}
		}


		public RawImage Unshade32( int zoom, Rectangle bounds, int[] colorbuffer, int[] idbuffer, int[] borderbuffer ) {
			if ( bounds.Width*bounds.Height != colorbuffer.Length ) 
				throw new ArgumentOutOfRangeException( "colorbuffer", "The colorbuffer and bounds parameters have mismatching sizes" );
			if ( colorbuffer.Length != idbuffer.Length || colorbuffer.Length != borderbuffer.Length ) 
				throw new ArgumentOutOfRangeException( "colorbuffer", "The various buffers have mismatching sizes." );
			
			int bufidx = 0;
			Pixel[,] memory = new Pixel[bounds.Width, bounds.Height];
			for ( int y=0; y<bounds.Height; ++y ) {
				for ( int x=0; x<bounds.Width; ++x ) {
					// Shading
					memory[x,y].Color = (byte)((0xFF-((colorbuffer[bufidx] >> 16) & 0xFF))>>2);
					
					// ID
					memory[x,y].ID = idc.ConvertRGB( idbuffer[bufidx] );

					// Border
					unchecked {
						memory[x,y].Border = (byte)((borderbuffer[bufidx] & (int)0xFF0000) > 0 ? 2 : 0);
					}

					bufidx++;
				}
			}

			return new RawImage( zoom, bounds, memory );
		}

		private MapToolsLib.IIDConvertor idc;
	}
}
