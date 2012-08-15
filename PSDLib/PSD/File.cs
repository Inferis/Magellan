#define RLE
using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace PSD {
	public enum ImageMode {
		Bitmap = 0,
		Grayscale = 1,
		IndexedColor = 2, 
		RGBColor = 3,
		CMYKColor = 4,
		Multichannel = 7,
		Duotone = 8,
		LabColor = 9
	}

	public enum CompressionMethod {
		Raw = 0,
		RLE = 1,
		Empty = 142
	}

	/// <summary>
	/// Summary description for File.
	/// </summary>
	public class File {
		protected File() {
			numchannels = 0;
			imgsize = Size.Empty;
			bpp = 8;
			mode = ImageMode.RGBColor;
			colormap = new Colormap();
			resources = new Resources();
			layers = new Layers( this );
			image = null;
		}

		public File( Size size ) : this() {
			imgsize = size;
		}

		public File( int width, int height ) : this() {
			imgsize = new Size( width, height );
		}

		public File( Bitmap src ) : this() {
			imgsize = src.Size;
			image = src;
		}

		public File( string path ) : this() {
			ReadFrom( path );
		}

		public void ReadFrom( BinaryReader reader ) {
			ReadHeader( reader );
			colormap = new Colormap( reader );
			resources = new Resources( reader );
			layers = new Layers( this, reader );

			CompressionMethod compression = (CompressionMethod)IPAddress.NetworkToHostOrder( reader.ReadInt16() );
			byte[][] data = new byte[3][];
			switch ( compression ) {
				case CompressionMethod.Raw:
					// read each line
 
					for ( int c=0; c<3; c++ ) {
						//data[c] = new byte[imgsize.Width*imgsize.Height];
						//int i = 0;
						//while ( i < imgsize.Width*imgsize.Height ) i += reader.Read( data[c], i, (imgsize.Width*imgsize.Height)-i );
						data[c] = new byte[imgsize.Width*imgsize.Height];
						reader.Read( data[c], 0, data[c].Length );
					}
					image = new Channels( imgsize, data[0], data[1], data[2] ).Combine();
					break;

				case CompressionMethod.RLE:
					// Throw widthlist away...
					reader.ReadBytes( imgsize.Height * numchannels  * 2 );

					// read each line
					for ( int c=0; c<3; c++ ) {
						data[c] = new byte[imgsize.Width*imgsize.Height];
						for ( int y=0, offset = 0; y<imgsize.Height; ++y ) {
							offset += Utils.UnpackRLELine( reader, imgsize.Width, data[c], offset );
						}
					}
					image = new Channels( imgsize, data[0], data[1], data[2] ).Combine();

					break;
				default:
					throw new InvalidCompressionMethodException( compression );
			}
		}

		public void WriteTo( BinaryWriter writer ) {
			// if no image is set, create one from the layer composite
			if ( image == null ) {
				image = layers.CreateFlattenedImage( Color.White );
			}

			WriteHeader( writer );
			colormap.WriteTo( writer );
			resources.WriteTo( writer );
			layers.WriteTo( writer );

			Channels channels = Channels.FromImage( image );
#if RLE
			writer.Write( IPAddress.HostToNetworkOrder( (short)CompressionMethod.RLE ) );

			byte[] data = new byte[channels.Red.Data.Length + channels.Green.Data.Length + channels.Blue.Data.Length];
			Array.Copy( channels.Red.Data, 0, data, 0, channels.Red.Data.Length );
			Array.Copy( channels.Green.Data, 0, data, channels.Red.Data.Length, channels.Green.Data.Length );
			Array.Copy( channels.Blue.Data, 0, data, channels.Red.Data.Length + channels.Green.Data.Length, channels.Blue.Data.Length );

			Utils.WriteImageRLE( writer, data, image.Width, image.Height*3 );
#else
			writer.Write( IPAddress.HostToNetworkOrder( (short)CompressionMethod.Raw ) );
			writer.Write( channels.Red.Data );
			writer.Write( channels.Green.Data );
			writer.Write( channels.Blue.Data );
#endif
		}

		public void ReadFrom( string path ) {
			FileStream stream = null;
			
			try {
				stream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read );
				ReadFrom( new BinaryReader( stream ) );
			}
			finally {
				if ( stream != null ) stream.Close();
			}
		}

		private void ReadHeader( BinaryReader reader ) {
			Utils.CheckSignature( reader, "8BPS", new InvalidPhotoshopFileException() );
			short version = IPAddress.NetworkToHostOrder( reader.ReadInt16() );
			if ( version != 1 ) throw new InvalidFileVersionException( version );

			reader.ReadBytes( 6 ); // reserved

			numchannels = IPAddress.NetworkToHostOrder( reader.ReadInt16() );
			int height =  IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			int width =  IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			imgsize = new Size( width, height );
			bpp = IPAddress.NetworkToHostOrder( reader.ReadInt16() );
			mode = (ImageMode)IPAddress.NetworkToHostOrder( reader.ReadInt16() );

			if ( mode != ImageMode.RGBColor ) throw new UnsupportedImageModeException();
		}

		private void WriteHeader( BinaryWriter writer ) {
			writer.Write( new char[4] { '8', 'B', 'P', 'S' } );
			writer.Write( IPAddress.HostToNetworkOrder( (short)1 ) );
			writer.Write( new byte[6] );

			writer.Write( IPAddress.HostToNetworkOrder( (short)3 ) );
			writer.Write( IPAddress.HostToNetworkOrder( imgsize.Height ) );
			writer.Write( IPAddress.HostToNetworkOrder( imgsize.Width ) );
			writer.Write( IPAddress.HostToNetworkOrder( (short)bpp ) );
			writer.Write( IPAddress.HostToNetworkOrder( (short)mode ) );
		}

		#region Public Properties
		public Layers Layers {
			get { return layers; }
		}

		public Size ImageSize {
			get { return imgsize; }
			set { imgsize = value; }
		}

		public Bitmap Image {
			get { return image; }
			set { image = value; }
		}
		#endregion

		#region Private Fields
		private int numchannels;
		private Size imgsize;
		private short bpp;
		private ImageMode mode;
		private Colormap colormap;
		private Resources resources;
		private Layers layers;
		private Bitmap image;
		#endregion
	}

	public class InvalidPhotoshopFileException : Exception {
		public InvalidPhotoshopFileException() : base( "This file is not a photoshop file (invalid signature)." ) {}
	}

	public class InvalidFileVersionException : Exception {
		public InvalidFileVersionException() : base( "Bad version number." ) {}
		public InvalidFileVersionException( int version ) : base( String.Format( "Bad version number '{0}'.", version ) ) {}
	}

	public class InvalidCompressionMethodException : Exception {
		public InvalidCompressionMethodException() : base( "An invalid compression method was specified." ) {}
		public InvalidCompressionMethodException( CompressionMethod method ) : this( (int)method ) {}
		public InvalidCompressionMethodException( int method ) : base( "An invalid compression method '" + method + "' was specified." ) {}
	}

	public class UnsupportedImageModeException : NotSupportedException {
		public UnsupportedImageModeException() : base( "Only RGB mode is supported." ) {}
	}
}