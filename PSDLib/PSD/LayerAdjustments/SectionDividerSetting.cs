using System;
using System.IO;
using System.Net;

namespace PSD
{
	public enum SectionDividerType {
		OtherLayer = 0,
		OpenFolder = 1,
		ClosedFolder = 2,
		Bounding = 3
	}

	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class SectionDividerSetting : LayerAdjustment
	{
		public const string KeyValue = "lsct";

		public SectionDividerSetting( int size, BinaryReader reader ) {
			type = (SectionDividerType)IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			if ( size == 4 ) return;
			Utils.CheckSignature( reader, "8BIM", new InvalidBlendSignature() );
			blendkey = new string( reader.ReadChars( 4 ) );
		}

		public override string Key {
			get { return KeyValue; }
		}

		public SectionDividerType Type {
			get { return type; }
		}

		public string Blendkey {
			get { return blendkey; }
		}

		private SectionDividerType type;
		private string blendkey;
	}
}
