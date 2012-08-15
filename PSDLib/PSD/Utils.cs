using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PSD
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public sealed class Utils
	{
		public static string ReadPascalString( BinaryReader reader, int pad ) {
			byte len = reader.ReadByte();
			string result = "";

			if ( len > 0 ) result = new string( reader.ReadChars( len ) );

			if ( (result.Length+1) % pad > 0 ) reader.ReadBytes( (pad-((result.Length+1) % pad)) );
			return result;
		}

		public static void WritePascalString( BinaryWriter writer, string data ) {
			if ( data.Length > 255 ) data = data.Substring( 0, 255 );
			byte len = (byte)data.Length;

			writer.Write( (byte)len );
			if ( len > 0 ) 
				writer.Write( data.ToCharArray() );
		}

		public static void WritePascalString( BinaryWriter writer, string data, int pad ) {
			WritePascalString( writer, data );

			int len = GetPascalLength( data );
			if ( len % pad > 0 ) for ( int i=0; i<(pad-(len % pad)); ++i ) writer.Write( (byte)0 );
		}

		public static int GetPascalLength( string data ) {
			if ( data.Length > 255 ) return 256;
			return data.Length+1;
		}

		public static int GetPascalLength( string data, int pad ) {
			int len = (data.Length > 255 ? 255 : data.Length)+1;
			return len % pad == 0 ? len : len + pad - (len % pad);
		}

		public static void CheckSignature( BinaryReader reader, string check, Exception throwthis ) {
			string val = new string( reader.ReadChars( 4 ) );
			if ( val != check ) throw throwthis;
		}

		public static int UnpackRLELine( BinaryReader reader, int linewidth, byte[] data, int index ) {
			int todo = linewidth;
			int offset = index; 

			while ( todo > 0 ) {
				int len = (sbyte)reader.ReadByte();				
				//if ( len >= 128 ) len -= 256;

				if ( len < 0 ) {
					if ( len == -128 ) continue;
					len = -len + 1;
					if ( len > todo ) len = todo;

					byte val = reader.ReadByte(); 
					for ( int i=0; i<len; ++i ) data[index++] = val;
					todo -= len;
				}
				else {
					for ( int i=-1; i<len; ++i ) {
						if ( todo-- >= 0 ) data[index++] = reader.ReadByte();
					}
				}
			}

			if ( todo < 0 ) throw new RLEUnpackException( todo );

			return index-offset;
		}

		public static byte[] PackRLELine( byte[] data, int index, int linewidth ) {
			byte[] line = new byte[linewidth*2];
			int limit = index + linewidth;
			int lineindex = 0;

			while ( index < limit ) {
				// look for matching values
				int i = 0;
				while ( i < 128 && index+i < limit && data[index+i] == data[index] )
					++i;
				
				if ( i > 1 ) { 
					// 2 or more consecutive values found!
					line[lineindex++] = (byte)(-(i-1)); // number of instances
					line[lineindex++] = data[index];
					index += i;
				}
				else {
					// Look for a row of different values
					i = 0;
					while ( i < 128 && index+i+1 < limit && (data[index+i] != data[index+i+1] || (index+i+2 < limit && data[index+i] != data[index+i+2])) )
						++i;

					if ( limit-index == 1 ) i = 1;

					if ( i > 0 ) {
						line[lineindex++] = (byte)(i-1); // number of instances
						for ( int j=0; j<i; ++j ) {
							line[lineindex++] = data[index+j];
						}
						index += i;
					}
				}
			}

			// Crop the line
			byte[] line2 = new byte[lineindex];
			Array.Copy( line, 0, line2, 0, lineindex );

			// Done!
			return line2;
		}

		public static void WriteImageRLE( BinaryWriter writer, byte[] data, int width, int height ) {
			int totallen = 0;
			byte[][] lines = new byte[height][];
			for ( int y=0, offset = 0; y<height; ++y, offset+=width ) {
				lines[y] = Utils.PackRLELine( data, offset, width );
				totallen += lines[y].Length;
			}

			for ( int y=0; y<height; ++y ) {
				writer.Write( IPAddress.HostToNetworkOrder( (short)lines[y].Length ) );
			}

			for ( int y=0; y<height; ++y ) {
				writer.Write( lines[y] );
			}

			//if ( (totallen&1) != 0 ) writer.Write( (byte)128 );
		}

		public static int[] GetBitmapData( Bitmap img ) {
			return GetBitmapData( img, new Rectangle( 0, 0, img.Width, img.Height ) );
		}

		public static int[] GetBitmapData( Bitmap img, Rectangle bounds ) {
			if ( bounds.X < 0 ) {
				bounds.Width += bounds.X;
				bounds.X = 0;
			}
			if ( bounds.Y < 0 ) {
				bounds.Height += bounds.Y;
				bounds.Y = 0;
			}
			if ( bounds.Right > img.Width ) 
				bounds.Width -= bounds.Right-img.Width;
			if ( bounds.Bottom > img.Height ) 
				bounds.Height -= bounds.Bottom-img.Height;

			BitmapData data = img.LockBits( bounds, ImageLockMode.ReadOnly, img.PixelFormat );
			int[] result = new int[bounds.Width * bounds.Height];
			for ( int i=0; i<bounds.Height; ++i ) {
				Marshal.Copy( new IntPtr(data.Scan0.ToInt32()+data.Stride*i), result, i*bounds.Width, bounds.Width );
			}
			img.UnlockBits( data );

			return result;
		}

		public static void SetBitmapData( Bitmap img, int[] buffer ) {
			SetBitmapData( img, buffer, new Rectangle( 0, 0, img.Width, img.Height ) );
		}

		public static void SetBitmapData( Bitmap img, int[] buffer, Rectangle bounds ) {
			if ( bounds.X < 0 ) {
				bounds.Width += bounds.X;
				bounds.X = 0;
			}
			if ( bounds.Y < 0 ) {
				bounds.Height += bounds.Y;
				bounds.Y = 0;
			}
			if ( bounds.Right > img.Width ) 
				bounds.Width -= bounds.Right-img.Width;
			if ( bounds.Bottom > img.Height ) 
				bounds.Height -= bounds.Bottom-img.Height;

			BitmapData data = img.LockBits( bounds, ImageLockMode.WriteOnly, img.PixelFormat );
			if ( buffer.Length != bounds.Width * bounds.Height ) throw new ArgumentOutOfRangeException( "buffer" );
			for ( int i=0; i<bounds.Height; ++i ) {
				Marshal.Copy( buffer, i*bounds.Width, new IntPtr(data.Scan0.ToInt32()+data.Stride*i), bounds.Width );
			}
			img.UnlockBits( data );
		}

		public static Rectangle LayerAdjustedBounds( Bitmap img, Rectangle bounds ) {
			Rectangle result = new Rectangle( 0, 0, bounds.Width, bounds.Height );
			if ( bounds.X < 0 ) {
				bounds.Width += bounds.X;
				result.X = -bounds.X;
				bounds.X = 0;
			}
			if ( bounds.Y < 0 ) {
				bounds.Height += bounds.Y;
				result.Y = -bounds.Y;
				bounds.Y = 0;
			}
			if ( bounds.Right > img.Width ) {
				bounds.Width -= bounds.Right-img.Width;
				result.Width = bounds.Width;
			}
			if ( bounds.Bottom > img.Height ) {
				bounds.Height -= bounds.Bottom-img.Height;
				result.Height = bounds.Height;
			}

			return result;
		}

		/// <summary>
		/// Converts a colour from HSL to RGB
		/// </summary>
		/// <remarks>Adapted from the algoritm in Foley and Van-Dam</remarks>
		/// <param name="hsl">The HSL value</param>
		/// <returns>A Color structure containing the equivalent RGB values</returns>
		public static Color HSL2RGBx( HSL hsl ) {
			double  r=0,g=0,b=0;
			double temp1,temp2;

			if(hsl.L==0) {
				r=g=b=0;
			}
			else {
				if(hsl.S==0) {
					r=g=b=hsl.L;
				}
				else {

					temp2 = ((hsl.L<=0.5) ? hsl.L*(1.0+hsl.S) : hsl.L+hsl.S-(hsl.L*hsl.S));
					temp1 = 2.0*hsl.L-temp2;
           

					double[] t3=new double[]{hsl.H+1.0/3.0,hsl.H,hsl.H-1.0/3.0};
					double[] clr=new double[]{0,0,0};

					for(int i=0;i<3;i++) {
						if(t3[i]<0)
							t3[i]+=1.0;

						if(t3[i]>1)
							t3[i]-=1.0;

						if(6.0*t3[i] < 1.0)
							clr[i]=temp1+(temp2-temp1)*t3[i]*6.0;
						else if(2.0*t3[i] < 1.0)
							clr[i]=temp2;
						else if(3.0*t3[i] < 2.0)
							clr[i]=(temp1+(temp2-temp1)*((2.0/3.0)-t3[i])*6.0);
						else
							clr[i]=temp1;
					}

					r=clr[0];
					g=clr[1];
					b=clr[2];
				}
			}

			return Color.FromArgb((int)(255*r),(int)(255*g),(int)(255*b));
		}

		public static Color HSL2RGB( HSL hsl ) {
			// by Donald (Sterex 1996), donald@xbeat.net, 20011124
			int lMax = (int)(hsl.L * 255.0);
			if ( hsl.S > 0 ) {
				int lMin = (int)((100 - hsl.S * 100.0) * (double)lMax / 100.0);
				double q = (lMax - lMin) / 60;

				int r, g, b, lMid;
				int h = (int)(hsl.H * 360);
				if ( h < 60 ) {
					lMid = (int)((h - 0) * q + lMin);
					r = lMax; g = lMid; b = lMin;
				}
				else if ( h < 120 ) {
					lMid = (int)((-(h - 120)) * q + lMin);
					r = lMid; g = lMax; b = lMin;
				}
				else if ( h < 180 ) {
					lMid = (int)((h - 120) * q + lMin);
					r = lMin; g = lMax; b = lMid;
				}
				else if ( h < 240 ) {
					lMid = (int)((-(h - 240)) * q + lMin);
					r = lMin; g = lMid; b = lMax;
				}
				else if ( h < 300 ) {
					lMid = (int)((h - 240) * q + lMin);
					r = lMid; g = lMin; b = lMax;
				}
				else {
					lMid = (int)((-(h - 360)) * q + lMin);
					r = lMax; g = lMin; b = lMid;
				}

				return Color.FromArgb( r, g, b );
			}
			else {
				return Color.FromArgb( lMax, lMax, lMax );
			}
		}

		static double[] Lum = new double[256];
		static double[] QTab = new double[256];
		static int init = 0;

		/// <summary>
		/// Converts RGB to HSL
		/// </summary>
		/// <remarks>Takes advantage of whats already built in to .NET by using the Color.GetHue, Color.GetSaturation and Color.GetBrightness methods</remarks>
		/// <param name="c">A Color to convert</param>
		/// <returns>An HSL value</returns>
		public static HSL RGB2HSL( byte R, byte G, byte B ) {
			return RGB2HSL( Color.FromArgb( R, G, B ) );
		}

		/// <summary>
		/// Converts RGB to HSL
		/// </summary>
		/// <remarks>Takes advantage of whats already built in to .NET by using the Color.GetHue, Color.GetSaturation and Color.GetBrightness methods</remarks>
		/// <param name="c">A Color to convert</param>
		/// <returns>An HSL value</returns>
		public static HSL RGB2HSL( Color c ) {
			return new HSL( c.GetHue()/360.0, c.GetSaturation(), c.GetBrightness() );
		}
	
		public static HSL RGB2HSLX( Color c ) {
			if ( init == 0 ) {
				for ( init = 2; init < 256; ++init ) {
					Lum[init] = ((double)init)/255;
				}
				for ( init = 1; init < 256; ++init ) {
					QTab[init] = 60/init;
				}
			}

			int lMax, lMin;
			if ( c.R > c.G ) {
				lMax = c.R; lMin = c.G;
			}
			else {
				lMax = c.G; lMin = c.R;
			}
			if ( c.B > lMax ) 
				lMax = c.B;
			else if ( c.B < lMin ) {
				lMin = c.B;
			}

			HSL hsl = new HSL();
			hsl.L = Lum[lMax];

			int lDifference = lMax - lMin;
			if ( lDifference > 0 ) {
				// do a 65K 2D lookup table here for more speed if needed
				hsl.S = ((double)lDifference) / lMax;
				if ( lMax == c.R ) {
					if ( c.B > c.G ) 
						hsl.H = ((QTab[lDifference] * (c.G - c.B)) / 360) + 1;
					else
						hsl.H = ((QTab[lDifference] * (c.G - c.B)) / 360);
				}
				else if ( lMax == c.G ) {
					hsl.H = (QTab[lDifference] * (c.B - c.R) + 120) / 360;
				}
				else {
					hsl.H = (QTab[lDifference] * (c.R - c.G) + 240) / 360;
				}
			}
			else {
				hsl.S = 0;
				hsl.H = 0;
			}

			return hsl;
		}
		}

	public class RLEUnpackException : Exception {
		public RLEUnpackException( int overflow ) : base( "RLE Line Unpack error (overflow = " + (-overflow) + ")" ) {}
	}
}
