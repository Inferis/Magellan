using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class ProtectionSetting : LayerAdjustment
	{
		public const string KeyValue = "lspf";

		public ProtectionSetting( int size, BinaryReader reader ) {
			flags = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
		}

		public override string Key {
			get { return KeyValue; }
		}

		public bool Transparancy {
			get { return (flags & 1) > 0; }
		}

		public bool Composite {
			get { return (flags & 2) > 0; }
		}

		public bool Position {
			get { return (flags & 4) > 0; }
		}

		private int flags;
	}
}
