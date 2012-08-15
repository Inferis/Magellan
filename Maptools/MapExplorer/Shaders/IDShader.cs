using System;
using System.Drawing;
using EU2.Map.Codec;
using EU2.Map.Drawing;
using EU2.Map;
using MapToolsLib;

namespace MapExplorer
{
	/// <summary>
	/// Summary description for GrayScaledShader.
	/// </summary>
	public class IDShader : IShader1632
	{
		public IDShader( IIDConvertor idconvertor ) {
			idc = idconvertor;
		}

		public short[] Shade16( RawImage image ) {
			short[] buffer = new short[image.PixelCount];
			int bufidx = 0;
			Pixel[,] memory = image.Memory;
			int height = image.Size.Height;
			int width = image.Size.Width;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					buffer[bufidx++] = (short)idc.ConvertID( memory[x,y].ID, IDConvertorMode.RGB16 );
				}
			}

			return buffer;
		}

		public int[] Shade32( RawImage image ) {
			int[] buffer = new int[image.PixelCount];
			int bufidx = 0;
			Pixel[,] memory = image.Memory;
			int height = image.Size.Height;
			int width = image.Size.Width;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					buffer[bufidx++] = idc.ConvertID( memory[x,y].ID );
				}
			}

			return buffer;
		}

		private IIDConvertor idc;
	}
}
