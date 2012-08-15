using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class LayerIDAdjustment : LayerAdjustment
	{
		public const string KeyValue = "lyid";

		public LayerIDAdjustment( int size, BinaryReader reader ) {
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
