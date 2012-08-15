using System;
using System.Drawing;
using System.Drawing.Imaging;
using EU2.Map.Codec;

namespace EU2.Map.Drawing
{
	public class IncognitaShader : IShader16
	{
		public IncognitaShader( IncognitaGrid incognita, IDGrid idgrid )
		{
			this.incognita = incognita;
			this.idgrid = idgrid;
		}

		#region IShader16 Members

		public short[] Shade16( EU2.Map.Codec.RawImage image ) {
			short[] buffer = new short[image.PixelCount];
			int bufidx = 0;
			Pixel[,] memory = image.Memory;
			int height = image.Size.Height;
			int width = image.Size.Width;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					byte color = (byte)(0x1F-(memory[x,y].Color>>2));
					buffer[bufidx++] = (short)((color << 10) | (color << 5) | color);
				}
			}

			Bitmap canvas = new Bitmap( image.Width, image.Height, PixelFormat.Format16bppRgb555 );
			Graphics g = Graphics.FromImage( canvas );
			g.DrawImageUnscaled( Visualiser.CreateImage16( buffer, image.Size ), 0, 0 ); 
			Font f = new Font( "Small Fonts", 5 );

			for ( int y=0; y<image.Height; y += Lightmap.BlockSize ) {
				for ( int x=0; x<image.Width; x += Lightmap.BlockSize ) {
					int ix = image.X + x;
					int iy = image.Y + y;
					IncognitaGridItem data = incognita[ix >> Lightmap.BlockFactor, iy >> Lightmap.BlockFactor];

					int id = idgrid.RealID( ix, iy, data[0].ID ); 
					g.DrawString( id.ToString() + "/" + data[0].Weight.ToString( "X2" ), f, Brushes.Black, x+2, y+2 );

					id = idgrid.RealID( ix, iy, data[1].ID ); 
					g.DrawString( id.ToString() + "/" + data[1].Weight.ToString( "X2" ), f, Brushes.Black, x+2, y+8 );

					id = idgrid.RealID( ix, iy, data[2].ID ); 
					g.DrawString( id.ToString() + "/" + data[2].Weight.ToString( "X2" ), f, Brushes.Black, x+2, y+14 );

					id = idgrid.RealID( ix, iy, data[3].ID ); 
					g.DrawString( id.ToString() + "/" + data[3].Weight.ToString( "X2" ), f, Brushes.Black, x+2, y+20 );
				}
			}

			f.Dispose();
			g.Dispose();

			return Visualiser.CreateBuffer16( canvas );
		}

		#endregion

		IncognitaGrid incognita;
		IDGrid idgrid;
	}
}
