using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class SheetColorSetting : LayerAdjustment
	{
		public const string KeyValue = "lclr";

		public SheetColorSetting( int size, BinaryReader reader ) {
			settings = new short[4];
			for ( int i=0; i<4; ++i )
				settings[i] = IPAddress.NetworkToHostOrder( reader.ReadInt16() );
		}

		public override string Key {
			get { return KeyValue; }
		}

		public short[] Settings {
			get { return settings; }
		}

		private short[] settings;
	}
}
