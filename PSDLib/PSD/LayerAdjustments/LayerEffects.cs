using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerIDAdjustment.
	/// </summary>
	public class LayerEffects : LayerAdjustment
	{
		public const string KeyValue = "lrFX";

		public LayerEffects( int size, BinaryReader reader ) {
			int version = IPAddress.NetworkToHostOrder( reader.ReadInt16() );
			int count = IPAddress.NetworkToHostOrder( reader.ReadInt16() );
			size -= 4;

			for ( int i=0; i<count; ++i ) {
				Utils.CheckSignature( reader, "8BIM", new InvalidBlendSignature() );
				string blendkey = new string( reader.ReadChars( 4 ) );
				int len = IPAddress.NetworkToHostOrder( reader.ReadInt32() );

				switch ( blendkey ) {
					case "cmnS":
						break;
					case "dsdw":
						break;
					case "isdw":
						break;
					case "oglw":
						break;
					case "iglw":
						break;
					case "bevl":
						break;
					case "sofi":
						break;
					default:
						throw new InvalidLayerEffectKeyException( blendkey );
				}

				byte[] data = reader.ReadBytes( len );
				size -= len + 12;
			}

			if ( size > 0 ) reader.ReadBytes( size );
		}

		public override string Key {
			get { return KeyValue; }
		}
	}

	public class InvalidLayerEffectKeyException : Exception {
		public InvalidLayerEffectKeyException() : base( "Invalid layer effect key." ) {}
		public InvalidLayerEffectKeyException( string key ) : base( String.Format( "Invalid layer effect key '{0}'.", key ) ) {}
	}
}
