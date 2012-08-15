using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerAdjustment.
	/// </summary>
	public class UnicodeLayerNameAdjustment : LayerAdjustment
	{
		public const string KeyValue = "luni";

		public UnicodeLayerNameAdjustment( int size, BinaryReader reader ) {
			int len = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			name = new string( new BinaryReader( reader.BaseStream, System.Text.UnicodeEncoding.BigEndianUnicode ).ReadChars( len ) );
			size -= 4 + len*2;
			reader.ReadBytes( size );
		}

		public override string Key {
			get { return KeyValue; }
		}

		public string Name {
			get { return name; }
		}

		private string name;
	}
}
