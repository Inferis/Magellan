using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PSD {
	/// <summary>
	/// Summary description for Channels.
	/// </summary>
	public class Channels {
		public Channels() {
			channels = null;
			mask = alpha = red = green = blue = -1;
		}

		public Channels( Size size, byte[] reddata, byte[] greendata, byte[] bluedata ) {
			channels = new Channel[3];
			mask = -1;
			alpha = -1;
			red = 0;
			green = 1;
			blue = 2;

			channels[red] = new Channel( ChannelType.Red, "", size, reddata );
			channels[green] = new Channel( ChannelType.Green, "", size, greendata );
			channels[blue] = new Channel( ChannelType.Blue, "", size, bluedata );
		}

		public Channels( Size size, byte[] alphadata, byte[] reddata, byte[] greendata, byte[] bluedata ) {
			channels = new Channel[4];
			mask = -1;
			alpha = 0;
			red = 1;
			green = 2;
			blue = 3;

			channels[alpha] = new Channel( ChannelType.Alpha, "", size, alphadata );
			channels[red] = new Channel( ChannelType.Red, "", size, reddata );
			channels[green] = new Channel( ChannelType.Green, "", size, greendata );
			channels[blue] = new Channel( ChannelType.Blue, "", size, bluedata );
		}

		public Channels( BinaryReader reader ) : this() {
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			int count = IPAddress.NetworkToHostOrder( reader.ReadInt16() );

			// Reset channels
			mask = alpha = red = green = blue = -1;

			// read channels
			channels = new Channel[count];
			for ( int i=0; i<count; ++i ) {
				channels[i] = new Channel( reader );
				switch ( channels[i].Type ) {
					case ChannelType.Mask: mask = i; break;
					case ChannelType.Alpha: alpha = i; break;
					case ChannelType.Red: red = i; break;
					case ChannelType.Green: green = i; break;
					case ChannelType.Blue: blue = i; break;
					default:
						throw new InvalidChannelTypeException( channels[i].Type );
				}
			}
		}

		public void ReadPixelDataFrom( BinaryReader reader ) {
			for ( int i=0; i<channels.Length; ++i ) {
				channels[i].DecompressDataFrom( reader );
			}
		}

		public void WriteTo( BinaryWriter writer ) {
			writer.Write( IPAddress.HostToNetworkOrder( (short)channels.Length ) );
			for ( int i=0; i<channels.Length; ++i ) {
				channels[i].WriteTo( writer );
			}
		}

		public void WritePixelDataTo( BinaryWriter writer ) {
			for ( int i=0; i<channels.Length; ++i ) {
				channels[i].CompressDataTo( writer, true );
			}
		}
		

		public Bitmap Combine( ) {
			return Combine( false );
		}
		
		public Bitmap Combine( bool createMask ) {
			// Make sure all sizes are equal
			if ( channels.Length == 0 ) return null;

			int mask = -1;
			if ( createMask ) {
				// Look for mask channel 
				for ( int i=1; i<channels.Length; ++i ) {
					if ( channels[i].Type == ChannelType.Mask && channels[i].Size.Width > 0 && channels[i].Size.Height> 0 ) { 
						mask = i;
						break;
					}
				}

				// No mask present, skip
				if ( mask == -1 ) return null;
			}

			Bitmap result;
			int[] img;
			Size size;
			if ( mask >= 0 ) {
				size = channels[mask].Size;
				result = new Bitmap( size.Width, size.Height, PixelFormat.Format32bppArgb );
				img = new int[size.Width*size.Height];

				int pixelindex = 0;
				byte[] chmask= channels[mask].Data;
				for ( int y=0; y<size.Height; y++ ) {
					for ( int x=0; x<size.Width; x++ ) {
						img[pixelindex] = Color.FromArgb( 
							chmask[pixelindex],
							chmask[pixelindex], 
							chmask[pixelindex], 
							chmask[pixelindex] ).ToArgb(); 
						pixelindex++;
					}
				}
			}
			else {
				size = channels[0].Size;
				for ( int i=1; i<channels.Length; ++i ) {
					if ( channels[i].Type == ChannelType.Mask ) continue;
					if ( channels[i].Size != size ) throw new InvalidOperationException();
				}

				if ( size.Width == 0 || size.Height == 0 ) return null;

				img = new int[size.Width*size.Height];
				int pixelindex = 0;

				byte[] chred = channels[red].Data;
				byte[] chgreen = channels[green].Data;
				byte[] chblue = channels[blue].Data;
				if ( alpha < 0 ) {
					result = new Bitmap( size.Width, size.Height, PixelFormat.Format32bppArgb );
					for ( int y=0; y<size.Height; y++ ) {
						for ( int x=0; x<size.Width; x++ ) {
							img[pixelindex] = Color.FromArgb( 
								255,
								chred[pixelindex], 
								chgreen[pixelindex], 
								chblue[pixelindex] ).ToArgb(); 
							pixelindex++;
						}
					}
				}
				else {
					result = new Bitmap( size.Width, size.Height, PixelFormat.Format32bppArgb );
					byte[] chalpha = channels[alpha].Data;
					for ( int y=0; y<size.Height; y++ ) {
						for ( int x=0; x<size.Width; x++ ) {
							img[pixelindex] = Color.FromArgb( 
								chalpha[pixelindex], 
								chred[pixelindex], 
								chgreen[pixelindex], 
								chblue[pixelindex] ).ToArgb(); 
							pixelindex++;
						}
					}
				}

			}

			BitmapData mem = result.LockBits( new Rectangle( new Point( 0, 0 ), size ), ImageLockMode.WriteOnly, result.PixelFormat );
			Marshal.Copy( img, 0, mem.Scan0, size.Width*size.Height ); 
			result.UnlockBits( mem );

			return result;
		}

		public static Channels FromImage( Bitmap image ) {
			if ( image == null || (image.PixelFormat != PixelFormat.Format32bppArgb && image.PixelFormat != PixelFormat.Format32bppRgb) ) return null;

			int[] mem = new int[image.Width*image.Height];
			BitmapData img = image.LockBits( new Rectangle( 0, 0, image.Width, image.Height ), ImageLockMode.ReadOnly, image.PixelFormat );
			Marshal.Copy( img.Scan0, mem, 0, mem.Length ); 
			image.UnlockBits( img );

			byte[] chalpha;
			byte[] chred = new byte[mem.Length];
			byte[] chgreen = new byte[mem.Length];
			byte[] chblue = new byte[mem.Length];

			if ( image.PixelFormat == PixelFormat.Format32bppArgb ) {
				chalpha = new byte[mem.Length];
				for ( int i=0; i<mem.Length; ++i ) {
					chalpha[i] = (byte)((mem[i] >> 24) & 0xFF);
 					chred[i] = (byte)((mem[i] >> 16) & 0xFF);
					chgreen[i] = (byte)((mem[i] >> 8) & 0xFF);
					chblue[i] = (byte)(mem[i] & 0xFF);
				}

				return new Channels( image.Size, chalpha, chred, chgreen, chblue );
			}
			else {
				for ( int i=0; i<mem.Length; ++i ) {
					chred[i] = (byte)((mem[i] >> 16) & 0xFF);
					chgreen[i] = (byte)((mem[i] >> 8) & 0xFF);
					chblue[i] = (byte)(mem[i] & 0xFF);
				}

				return new Channels( image.Size, chred, chgreen, chblue );
			}

		}

		#region Public Properties
		public Channel Mask {
			get { return mask >= 0 ? channels[mask] : null; }
		}

		public Channel Alpha {
			get { return alpha >= 0 ? channels[alpha] : null; }
		}

		public Channel Red {
			get { return red >= 0 ? channels[red] : null; }
		}
		
		public Channel Green {
			get { return green >= 0 ? channels[green] : null; }
		}
		
		public Channel Blue {
			get { return blue >= 0 ? channels[blue] : null; }
		}
		#endregion

		#region Private Fields
		private Channel[] channels;
		private int mask;
		private int alpha;
		private int red;
		private int green;
		private int blue;
		#endregion
	}

	public class InvalidChannelCountException : Exception {
		public InvalidChannelCountException() : base( "An invalid number of channels was specified." ) {}
	}
}
