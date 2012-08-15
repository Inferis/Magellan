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
	public class IDMapShader : IShader16
	{
		public IDMapShader( IDMap idmap ) {
			this.idmap = idmap;
			this.diff = false;
		}

		public IDMapShader( IDMap idmap, bool diff ) {
			this.idmap = idmap;
			this.diff = diff;
		}

		public short[] Shade16( RawImage image ) {
			if ( diff ) return Shade16Diff( image );

			// Normal version
			int bufidx = 0;
			int height = image.Size.Height;
			int width = image.Size.Width;
			ushort[] prebuffer = idmap.ExportBitmapBuffer( image.Location, image.Size );
			short[] buffer = new short[prebuffer.Length];

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					buffer[bufidx] = (short)(prebuffer[bufidx] << 4);
					bufidx++;
				}
			}

			return buffer;
		}

		private short[] Shade16Diff( RawImage image ) {
			// Diff version
			int bufidx = 0;
			int height = image.Size.Height;
			int width = image.Size.Width;
			ushort[] prebuffer = idmap.ExportBitmapBuffer( image.Location, image.Size );
			short[] buffer = new short[prebuffer.Length];
			Pixel[,] memory = image.Memory;

			for ( int y=0; y<height; ++y ) {
				for ( int x=0; x<width; ++x ) {
					buffer[bufidx] = (short)((prebuffer[bufidx]-memory[x,y].ID) != 0 ? (0xFF << 10) : 0);
					bufidx++;
				}
			}

			return buffer;
		}

		private IDMap idmap;
		private bool diff;
	}
}
