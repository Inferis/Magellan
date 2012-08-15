using System;

namespace MapToolsLib
{
	public enum IDConvertorMode {
		RGB32,
		RGB16
	}

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public interface IIDConvertor
	{
		int ConvertID( ushort id );
		int ConvertID( ushort id, IDConvertorMode mode );
		ushort ConvertRGB( int rgb );
	}
}
