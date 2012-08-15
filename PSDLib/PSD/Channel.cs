#define RLE

using System;
using System.Net;
using System.IO;
using System.Drawing;

namespace PSD
{
	public enum ChannelType {
		MinValue = -2,
		Mask = -2,
		Alpha = -1,
		Red = 0,
		Green = 1,
		Blue = 2,
		Max = 2,
}

	/// <summary>
	/// Summary description for Channel.
	/// </summary>
	public class Channel
	{
		public Channel() {
			name = "";
			type = 0;
			size = Size.Empty;
			data = null;
		}

		public Channel( ChannelType type, string name, Size size, byte[] data ) {
			this.name = name;
			this.type = type;
			this.size = size;
			this.data = data;
		}

		public Channel( BinaryReader reader ) : this() {
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			type = (ChannelType)IPAddress.NetworkToHostOrder( reader.ReadInt16() );
			int compressedsize = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
		}

		public void DecompressDataFrom( BinaryReader reader ) {
			CompressionMethod compression = (CompressionMethod)IPAddress.NetworkToHostOrder( reader.ReadInt16() );

			switch ( compression ) {
				case CompressionMethod.Raw:
					data = reader.ReadBytes( size.Width*size.Height );
					break;
				case CompressionMethod.RLE:
					// Throw widthlist away...
					//int[] width = new int[size.Height];
					//for ( int y=0, offset = 0; y<size.Height; ++y ) {
					//	width[y] = IPAddress.NetworkToHostOrder( reader.ReadInt16() );
					//}
					reader.ReadBytes( size.Height * 2 );

					// read each line
					data = new byte[size.Width*size.Height];
					for ( int y=0, offset = 0; y<size.Height; ++y ) {
						offset += Utils.UnpackRLELine( reader, size.Width, data, offset );
					}

					break;
				default:
					throw new InvalidCompressionMethodException( compression );
			}

			//data = reader.ReadBytes( compressedsize );
		}
	
		public void WriteTo( BinaryWriter writer ) {
			writer.Write( IPAddress.HostToNetworkOrder( (short)type ) );

#if RLE
			int totallen = 0;
			byte[][] lines = new byte[size.Height][];
			for ( int y=0, offset = 0; y<size.Height; ++y, offset+=size.Width ) {
				totallen += Utils.PackRLELine( data, offset, size.Width ).Length;
			}
			//if ( (totallen&1) != 0 ) ++totallen;
			writer.Write( IPAddress.NetworkToHostOrder( (int)(totallen+(size.Height*2)+2) ) );
#else
			writer.Write( IPAddress.NetworkToHostOrder( (int)(data.Length+2) ) );
#endif
		}

		public void CompressDataTo( BinaryWriter writer, bool writeCompressionMethod ) {
#if RLE
			if ( writeCompressionMethod ) writer.Write( IPAddress.HostToNetworkOrder( (short)CompressionMethod.RLE ) );

			Utils.WriteImageRLE( writer, data, size.Width, size.Height );
#else
			if ( writeCompressionMethod ) writer.Write( IPAddress.HostToNetworkOrder( (short)CompressionMethod.Raw ) );
			writer.Write( data );
#endif
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}

		public ChannelType Type {
			get { return type; }
			set { type = value; }
		}

		public Size Size {
			get { return size; }
			set { size = value; }
		}

		public byte[] Data {
			get { return data; }
			set { data = value; }
		}

		private string name;
		private ChannelType type;
		private Size size;
		private byte[] data;
	}

	public class InvalidChannelTypeException : Exception {
		public InvalidChannelTypeException() : base( "An invalid channel type was encountered." ) {}
		public InvalidChannelTypeException( ChannelType type ) : base( "An invalid channel type '" + ((int)type) + "' was encountered." ) {}
		public InvalidChannelTypeException( int type ) : base( "An invalid channel type '" + type + "' was encountered." ) {}
	}
}
