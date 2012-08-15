using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class ReferencePointSetting : LayerAdjustment
	{
		public const string KeyValue = "fxrp";

		public ReferencePointSetting( int size, BinaryReader reader ) {
			refs = new double[2];
			for ( int i=0; i<2; ++i )
				refs[i] = (double)IPAddress.NetworkToHostOrder( reader.ReadInt64() );
		}

		public override string Key {
			get { return KeyValue; }
		}

		public double[] ReferencePoints {
			get { return refs; }
		}

		private double[] refs;
	}
}
