using System;

namespace EU2.Data
{
	/// <summary>
	/// Summary description for TerrainVariant.
	/// </summary>
	public struct TerrainVariant {
		System.Drawing.Point location;
		int variant;

		public TerrainVariant( int x, int y, int variant ) {
			location = new System.Drawing.Point( x, y );
			this.variant = variant;
		}

		public TerrainVariant( System.Drawing.Point location, int variant ) {
			this.location = location;
			this.variant = variant;
		}

		public int X {
			get { return location.X; }
		}

		public int Y {
			get { return location.Y; }
		}
	
		public System.Drawing.Point Location {
			get { return location; }
		}

		public int Variant {
			get { return variant; }
		}
	}
}
