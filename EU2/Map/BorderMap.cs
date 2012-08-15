using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using EU2.Map.Codec;
using EU2.Map.Drawing;

namespace EU2.Map
{
	/// <summary>
	/// Summary description for BorderMap.
	/// </summary>
	public class BorderMap : ScanlineCodedMap
	{
		private const int BuildBlockSize = Lightmap.BlockSize*16;

		public BorderMap() : base() {
		}

		public BorderMap( BinaryReader reader ) : base( reader ) {
		}

		public override void SetZeroID() {
			ZeroID = 0;
		}

		public static BorderMap Build( Lightmap map ) {
			// Should be a zero zoom lightmap
			if ( map.Zoom != 0 ) throw new InvalidZoomFactorException( map.Zoom );

			// We build the map in 512/512 blocks
			BorderMap result = new BorderMap();
			IShader32 shader = new ImportBorderShader();
			bool vd = map.VolatileDecompression;
			try {
				map.VolatileDecompression = true;
				for ( int y=0; y<Lightmap.BaseHeight; y+=BuildBlockSize ) {
					int height = (y+BuildBlockSize > Lightmap.BaseHeight) ? (Lightmap.BaseHeight-y) : BuildBlockSize;

					for ( int x=0; x<Lightmap.BaseWidth; x+=BuildBlockSize ) {
						int width = (x+BuildBlockSize > Lightmap.BaseWidth) ? (Lightmap.BaseWidth-x) : BuildBlockSize;

						// Get the bitmap from the lightmap
						// And import it into ourselves
						result.ImportBitmapBuffer( x, y, width, height, map.DecodeImage( new Rectangle( x, y, width, height ), shader ) );
					}
				}
			} 
			finally {
				map.VolatileDecompression = vd;
			}

			return result;
		}

		internal class ImportBorderShader : IShader32 {

			public int[] Shade32( RawImage image ) {
				int[] buffer = new int[image.PixelCount];
				int bufidx = 0;
				Pixel[,] memory = image.Memory;
				int height = image.Size.Height;
				int width = image.Size.Width;

				for ( int y=0; y<height; ++y ) {
					for ( int x=0; x<width; ++x ) {
						buffer[bufidx++] = memory[x,y].Border;
					}
				}

				return buffer;
			}

		}
	}

	/*
	 * internal class BorderVisualCodec : VisualCodec {
		protected override PixelData EncodePixel( int pixel, int x, int y ) {
			throw new NotSupportedException();
		}

		protected override int DecodePixel( PixelData data, int x, int y ) {
			return data.Border ? 1 : 0;
		}

		protected override void Decode4Pixels( PixelData[] data, int x, int y, ref int[] output ) {
			// No loop, this is faster
			output[0] = data[0].Border ? 1 : 0;
			output[1] = data[1].Border ? 1 : 0;
			output[2] = data[2].Border ? 1 : 0;
			output[3] = data[3].Border ? 1 : 0;
		}
	}
	*/
}
