using System;

namespace EU2
{
	/// <summary>
	/// Summary description for ScanlineValueOutOfRangeException.
	/// </summary>
	public class ScanlineValueOutOfRangeException : Exception
	{
		public ScanlineValueOutOfRangeException( int scanvalue, int x, int y ) : this( (short)scanvalue, x, y ) {
		}

		public ScanlineValueOutOfRangeException( short scanvalue, int x, int y ) 
			: base( String.Format( "The scanline value \"{2}\" at ({0},{1}) was out of range.", x, y, scanvalue ) ) {
			this.scanvalue = scanvalue;
			this.x = x;
			this.y = y;
		}

		public short ScanValue {
			get { return scanvalue; }
		}

		public int X {
			get { return x; }
		}
	
		public int Y {
			get { return y; }
		}

		private short scanvalue;
		private int x, y;
	}
}
