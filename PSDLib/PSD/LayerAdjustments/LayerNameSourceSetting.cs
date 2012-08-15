using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class LayerNameSourceSetting : LayerAdjustment
	{
		public const string KeyValue = "lnsr";

		public LayerNameSourceSetting( int size, BinaryReader reader ) {
			id = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
		}

		public override string Key {
			get { return KeyValue; }
		}

		public int ID {
			get { return id; }
		}

		private int id;
	}
}
