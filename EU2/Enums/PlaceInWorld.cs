using System;

namespace EU2.Enums
{
	/// <summary>
	/// Summary description for PlaceInWorld.
	/// </summary>
	public class PlaceInWorld {
		string name;
		int piw;
			
		private PlaceInWorld( string name, int piwValue ) {
			this.name = name;
			this.piw = piwValue; 
		}

		#region All Places In World
		static public PlaceInWorld Inland	{ get { return inland; } }
		static public PlaceInWorld Coastal	{ get { return coastal; } }
		#endregion

		public string Name {
			get { return name; }
		}

		public int Value {
			get { return piw; }
		}

		public override string ToString() {
			return name;
		}

		static public PlaceInWorld FromName( string name ) {
			switch ( name.ToLower() ) {
				case "nowhere": 
				case "inland":		return Inland;
				case "coastal":		return Coastal;
				default:			return Inland;
			}
		}

		static private PlaceInWorld inland = new PlaceInWorld( "inland", 0 );
		static private PlaceInWorld coastal = new PlaceInWorld( "coastal", 1 );

	}

}
