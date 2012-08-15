using System;

namespace MapToolsLib
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Inferis2IDConvertor : IIDConvertor
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
			// Move bits around
			int result = (((id & 0x200)>>1) | ((id & 0x40)<<3) | ((id & 0x8)<<7) | ((id & 0x1)<<11) |
				((id & 0x400)>>6) | ((id & 0x80)>>2) | ((id & 0x10)<<2) | ((id & 0x2)<<6) |
				((id & 0x800)>>11) | ((id & 0x100)>>7) | ((id & 0x20)>>3) | ((id & 0x4)<<1));

			// Distribute over R, G and B.
			result = ((result & 0xF00) << 12) | ((result & 0xF0) << 8) | ((result & 0xF) << 4);

			unchecked {
				return result | (int)0xFF000000;
			}
		}

		public int ConvertID16( ushort id ) {
			// Move bits around
			int result = (((id & 0x200)>>1) | ((id & 0x40)<<3) | ((id & 0x8)<<7) | ((id & 0x1)<<11) |
				((id & 0x400)>>6) | ((id & 0x80)>>2) | ((id & 0x10)<<2) | ((id & 0x2)<<6) |
				((id & 0x800)>>11) | ((id & 0x100)>>7) | ((id & 0x20)>>3) | ((id & 0x4)<<1));

			// Distribute over R, G and B.
			result = ((result & 0xF00) << 3) | ((result & 0xF0) << 2) | ((result & 0xF) << 1);

			return result;
		}

		public ushort ConvertRGB( int rgb ) {
			// Invert if necessary
			//if ( (rgb & 0xF) > 0 ) rgb ^= 0xFFFFFF;

			// Compact distributed bits
			rgb = ((rgb & 0xF00000) >> 12) | ((rgb & 0xF000) >> 8) | ((rgb & 0xF0) >> 4);

			// And move them to their correct places.
			rgb = (((rgb & 0x8)>>1) | ((rgb & 0x4)<<3) | ((rgb & 0x2)<<7) | ((rgb & 0x1)<<11) |
				((rgb & 0x80)>>6) | ((rgb & 0x40)>>2) | ((rgb & 0x20)<<2) | ((rgb & 0x10)<<6) |
				((rgb& 0x800)>>11) | ((rgb & 0x400)>>7) | ((rgb & 0x200)>>3) | ((rgb & 0x100)<<1));
			
			return (ushort)rgb;
		}

		public override string ToString() {
			return "Inferis2";
		}

	}
}
