using System;
using System.IO;
using System.Net;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerAdjustment.
	/// </summary>
	public abstract class LayerAdjustment {
		public LayerAdjustment() {
		}

		public abstract string Key { get; }

		public static LayerAdjustment Construct( BinaryReader reader ) {
			Utils.CheckSignature( reader, "8BIM", new InvalidBlendSignature() );
			string adjkey = new string( reader.ReadChars( 4 ) );
			int adjsize = IPAddress.NetworkToHostOrder( reader.ReadInt32() );

			switch ( adjkey ) {
				// Photoshop 4
				case "levl": case "curv": case "brit": case "blnc": case "hue ": case "hue2": case "selc": case "thrs": case "nvrt": case "post":
					return new LayerAdjustmentLayer( adjkey, adjsize, reader );

				// Photoshop 5
				case LayerEffects.KeyValue: 
					return new LayerEffects( adjsize, reader );
				
				case "tySH": 
					return new GenericAdjustment( "tySH", adjsize, reader );;

				case UnicodeLayerNameAdjustment.KeyValue: 
					return new UnicodeLayerNameAdjustment( adjsize, reader );

				case LayerIDAdjustment.KeyValue: 
					return new LayerIDAdjustment( adjsize, reader );

				// Photoshop 6
				case BlendClippingElementsEffect.KeyValue: 
					return new BlendClippingElementsEffect( adjsize, reader );

				case BlendInteriorElementsEffect.KeyValue: 
					return new BlendInteriorElementsEffect( adjsize, reader );

				case KnockoutSetting.KeyValue: 
					return new KnockoutSetting( adjsize, reader );

				case LayerNameSourceSetting.KeyValue: 
					return new LayerNameSourceSetting( adjsize, reader );

				case ProtectionSetting.KeyValue: 
					return new ProtectionSetting( adjsize, reader );

				case SheetColorSetting.KeyValue: 
					return new SheetColorSetting( adjsize, reader );

				case ReferencePointSetting.KeyValue: 
					return new ReferencePointSetting( adjsize, reader );

				case SectionDividerSetting.KeyValue: 
					return new SectionDividerSetting( adjsize, reader );

				case ObjectBasedEffects.KeyValue:
					return new ObjectBasedEffects( adjsize, reader );

				default:
					throw new InvalidAdjustmentKeyException( adjkey );
			}
		}
	}

	public class InvalidAdjustmentKeyException : Exception {
		public InvalidAdjustmentKeyException() : base( "Invalid layer adjustment key." ) {}
		public InvalidAdjustmentKeyException( string key ) : base( String.Format( "Invalid layer adjustment key '{0}'.", key ) ) {}
	}
}
