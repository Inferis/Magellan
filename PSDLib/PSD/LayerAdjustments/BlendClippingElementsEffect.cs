using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class BlendClippingElementsEffect : LayerAdjustment
	{
		public const string KeyValue = "clbl";

		public BlendClippingElementsEffect( int size, BinaryReader reader ) {
			doBlend = reader.ReadBoolean();
			reader.ReadBytes( 3 ); // padding
		}

		public override string Key {
			get { return KeyValue; }
		}

		public bool BlendEnabled {
			get { return doBlend; }
		}

		private bool doBlend;
	}
}
