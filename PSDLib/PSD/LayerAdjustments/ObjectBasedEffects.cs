using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class ObjectBasedEffects : LayerAdjustment
	{
		public const string KeyValue = "lfx2";

		public ObjectBasedEffects( int size, BinaryReader reader ) {
			int val = reader.ReadInt32();
			if ( val != 0 ) throw new BadImageFormatException(); // 0
			val = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			if ( val != 16 ) throw new BadImageFormatException(); // 16

			byte[] data = reader.ReadBytes( size-8 );
		}

		public override string Key {
			get { return KeyValue; }
		}
	}
}
