using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class GenericAdjustment : LayerAdjustment
	{
		public GenericAdjustment( string key, int size, BinaryReader reader ) {
			this.key = key;

			data = reader.ReadBytes( size );
		}

		public override string Key {
			get { return key; }
		}

		public byte[] Data {
			get { return data; }
		}

		private string key;
		private byte[] data;
	}
}
