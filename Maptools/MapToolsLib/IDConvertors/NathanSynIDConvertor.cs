using System;

namespace MapToolsLib
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class NathanSynIDConvertor : IIDConvertor
	{
		public int ConvertID( ushort id, IDConvertorMode mode ) {
			if ( mode == IDConvertorMode.RGB32 ) 
				return ConvertID32( id );
			else
				return ConvertID16( id );
		}

		public int ConvertID( ushort id ) {
			return ConvertID32( id );
		}
			
		public int ConvertID32( ushort id ) {
			int result =
				(((id % 13) * 19) << 16) |
				(((id % 17) * 15) << 8) |
				((id % 19) * 13);

			unchecked {
				return result | (int)0xFF000000;
			}
		}

		public int ConvertID16( ushort id ) {
			throw new NotImplementedException();
		}

		public ushort ConvertRGB( int rgb ) {
			int id = rgb;

			id = (((((rgb >> 16) & 0xFF) / 21) << 4) / 13) +
				(((((rgb >> 8) & 0xFF) / 21) << 4) / 15) +
				((((rgb & 0xFF) / 21) << 4) / 17);

			return (ushort)id;
		}

		public override string ToString() {
			return "NathanSyn";
		}

	}
}
