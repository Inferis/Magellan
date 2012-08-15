using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for Resources.
	/// </summary>
	public class Colormap
	{
		public Colormap() {
			data = null;
		}

		public Colormap( BinaryReader reader ) : this() {
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			int size = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			if ( size > 0 )
				data = reader.ReadBytes( size );
			else
				data = null;
		}

		public void WriteTo( BinaryWriter writer ) {
			if ( data == null ) 
				writer.Write( (int)0 );
			else {
				writer.Write( IPAddress.HostToNetworkOrder( (int)data.Length ) );
				writer.Write( data );
			}
		}

		public int Size {
			get { return data == null ? 0 : data.Length; }
		}

		public byte[] Data {
			get { return data; }
			set { data = value; }
		}

		private byte[] data;
	}
}
