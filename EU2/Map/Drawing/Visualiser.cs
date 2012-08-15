using System;
using System.Drawing;
using System.Drawing.Imaging;
using EU2.Map.Codec;

namespace EU2.Map.Drawing
{
	/// <summary>
	/// Summary description for Visualiser.
	/// </summary>
	public sealed class Visualiser
	{
		public static Bitmap CreateImage32( int[] buffer, Size size ) {
			return CreateImage32( buffer, size, false );
		}

		public static Bitmap CreateImage32( int[] buffer, Size size, bool includeAlpha ) {
			if ( buffer == null || size == Size.Empty ) return null;

			Bitmap result = null;
			BitmapData cdata = null;

			try {
				result = new Bitmap( size.Width, size.Height, includeAlpha ? PixelFormat.Format32bppArgb : PixelFormat.Format32bppRgb );
				cdata = result.LockBits( new Rectangle( new Point( 0, 0 ), size ), ImageLockMode.WriteOnly, result.PixelFormat );
				System.Runtime.InteropServices.Marshal.Copy( buffer, 0, cdata.Scan0, buffer.Length );
				result.UnlockBits( cdata );
			}
			catch ( Exception ex ) {
				if ( result != null ) {
					if ( cdata != null ) result.UnlockBits( cdata );
					result.Dispose();
				}
				throw ex;
			}

			return result;
		}

		public static Bitmap CreateImage16( short[] buffer, Size size ) {
			if ( buffer == null || size == Size.Empty ) return null;

			Bitmap result = null;
			BitmapData cdata = null;

			try {
				result = new Bitmap( size.Width, size.Height, PixelFormat.Format16bppRgb555 );
				cdata = result.LockBits( new Rectangle( new Point( 0, 0 ), size ), ImageLockMode.WriteOnly, result.PixelFormat );
				System.Runtime.InteropServices.Marshal.Copy( buffer, 0, cdata.Scan0, buffer.Length );
				result.UnlockBits( cdata );
			}
			catch ( Exception ex ) {
				if ( result != null ) {
					if ( cdata != null ) result.UnlockBits( cdata );
					result.Dispose();
				}
				throw ex;
			}

			return result;
		}

		public static Bitmap CreateImage8( byte[] buffer, Size size ) {
			if ( buffer == null || size == Size.Empty ) return null;

			Bitmap result = null;
			BitmapData cdata = null;

			try {
				result = new Bitmap( size.Width, size.Height, PixelFormat.Format8bppIndexed );
				cdata = result.LockBits( new Rectangle( new Point( 0, 0 ), size ), ImageLockMode.WriteOnly, result.PixelFormat );
				System.Runtime.InteropServices.Marshal.Copy( buffer, 0, cdata.Scan0, buffer.Length );
				result.UnlockBits( cdata );
			}
			catch ( Exception ex ) {
				if ( result != null ) {
					if ( cdata != null ) result.UnlockBits( cdata );
					result.Dispose();
				}
				throw ex;
			}

			return result;
		}


		public static int[] CreateBuffer32( Bitmap source ) {
			if ( source == null ) return null;

			if ( source.PixelFormat != PixelFormat.Format32bppRgb && source.PixelFormat != PixelFormat.Format32bppArgb ) 
				throw new InvalidOperationException();

			BitmapData cdata = source.LockBits( new Rectangle( 0, 0, source.Width, source.Height ), ImageLockMode.ReadOnly, source.PixelFormat );
			int[] result = new int[source.Width*source.Height];
			System.Runtime.InteropServices.Marshal.Copy( cdata.Scan0, result, 0, result.Length );
			source.UnlockBits( cdata );

			return result;
		}
		
		public static short[] CreateBuffer16( Bitmap source ) {
			if ( source == null ) return null;

			if ( source.PixelFormat != PixelFormat.Format16bppRgb555 && source.PixelFormat != PixelFormat.Format16bppRgb565 ) 
				throw new InvalidOperationException();

			BitmapData cdata = source.LockBits( new Rectangle( 0, 0, source.Width, source.Height ), ImageLockMode.ReadOnly, source.PixelFormat );
			short[] result = new short[source.Width*source.Height];
			System.Runtime.InteropServices.Marshal.Copy( cdata.Scan0, result, 0, result.Length );
			source.UnlockBits( cdata );

			return result;
		}
		
		public static byte[] CreateBuffer8( Bitmap source ) {
			if ( source == null ) return null;

			if ( source.PixelFormat != PixelFormat.Format8bppIndexed ) 
				throw new InvalidOperationException();

			BitmapData cdata = source.LockBits( new Rectangle( 0, 0, source.Width, source.Height ), ImageLockMode.ReadOnly, source.PixelFormat );
			byte[] result = new byte[source.Width*source.Height];
			System.Runtime.InteropServices.Marshal.Copy( cdata.Scan0, result, 0, result.Length );
			source.UnlockBits( cdata );

			return result;
		}
		
	}
}
