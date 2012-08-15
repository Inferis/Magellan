using System;
using System.Drawing;
using EU2.Map.Codec;

namespace EU2.Map.Drawing
{
	/// <summary>
	/// Summary description for GrayScaledShader.
	/// </summary>
	public class GrayScaledShader : IShader1632, IMultiShader32, IUnshader32
	{
		public GrayScaledShader() : this( false ) {
		}

		public GrayScaledShader( bool drawBorders ) {
			this.drawBorders = drawBorders;
		}

		public int[] Shade32( RawImage image ) {
			int[] buffer = new int[image.PixelCount];
			int bufidx = 0;
			Pixel[,] memory = image.Memory;
			int height = image.Size.Height;
			int width = image.Size.Width;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					byte color = (byte)(0xFF-(memory[x,y].Color << 2));
					if ( memory[x,y].IsBorder() && drawBorders ) {
						buffer[bufidx++] = 0xFF0000;
					}
					else {
						buffer[bufidx++] = (color << 16) | (color << 8) | color;
					}
				}
			}

			return buffer;
		}


		public void MultiShade32( RawImage image, out int[] colorbuffer, out ushort[] idbuffer, out byte[] borderbuffer ) {
			colorbuffer = new int[image.PixelCount];
			idbuffer = new ushort[image.PixelCount];
			borderbuffer = new byte[image.PixelCount];

			int height = image.Size.Height;
			int width = image.Size.Width;
			Pixel[,] memory = image.Memory;

			int bufidx = 0;
			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					byte color = (byte)(0xFF-(memory[x,y].Color << 2));
					colorbuffer[bufidx] = (color << 16) | (color << 8) | color;
					idbuffer[bufidx] = memory[x,y].ID;
					borderbuffer[bufidx++] = memory[x,y].Border;
				}
			}
		}


		public RawImage Unshade32( int zoom, Rectangle bounds, int[] colorbuffer, ushort[] idbuffer, byte[] borderbuffer ) {
			if ( bounds.Width*bounds.Height != colorbuffer.Length ) 
				throw new ArgumentOutOfRangeException( "lightbuf", "The colorbuffer and bounds parameters have mismatching sizes" );
			if ( colorbuffer.Length != idbuffer.Length || colorbuffer.Length != borderbuffer.Length ) 
				throw new ArgumentOutOfRangeException( "lightbuf", "The various buffers have mismatching sizes." );
			
			int bufidx = 0;
			Pixel[,] memory = new Pixel[bounds.Width, bounds.Height];
			for ( int y=0; y<bounds.Height; ++y ) {
				for ( int x=0; x<bounds.Width; ++x ) {
					memory[x,y].Color = (byte)((0xFF-((colorbuffer[bufidx] >> 16) & 0xFF))>>2);
					memory[x,y].ID = idbuffer[bufidx];
					memory[x,y].Border = borderbuffer[bufidx++];
				}
			}

			return new RawImage( zoom, bounds, memory );
		}


		public short[] Shade16( RawImage image ) {
			short[] buffer = new short[image.PixelCount];
			int bufidx = 0;
			Pixel[,] memory = image.Memory;
			int height = image.Size.Height;
			int width = image.Size.Width;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					byte color = (byte)(0x1F-(memory[x,y].Color>>1));
					if ( memory[x,y].IsBorder() && drawBorders ) {
						buffer[bufidx++] = (short)(0x1F << 10);
					}
					else {
						buffer[bufidx++] = (short)((color << 10) | (color << 5) | color);
					}
				}
			}

			return buffer;
		}

		private bool drawBorders;
	}
}
