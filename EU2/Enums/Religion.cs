using System;

namespace EU2.Enums
{
	/// <summary>
	/// Summary description for Religion.
	/// </summary>
	public class Religion {
		private string name;

		public Religion( string name ) {
			this.name = name;
		}

		public string Name { get { return name; } }

		#region Static Stuff
		public static Religion FromName( string name ) {
			switch ( name.ToLower() ) {
				case "buddhism":			return Buddhism;
				case "catholic":			return Catholic;			
				case "crc":					return CounterReformCatholic;		
				case "exotic":				return Exotic;
				case "hinduism":			return Hinduism;
				case "confucianism":		return Confucianism;
				case "protestant":			return Protestant;
				case "reformed":			return Reformed;
				case "orthodox":			return Orthodox;
				case "pagan":				return Exotic;
				case "shiite":				return Shiite;
				case "sunni":				return Sunni;
				default:					return Exotic;
			}
		}

		public static Religion Buddhism					{ get { return buddhism; } }
		public static Religion Catholic					{ get { return catholic; } }
		public static Religion CounterReformCatholic	{ get { return crc; } }
		public static Religion Exotic					{ get { return exotic; } }
		public static Religion Hinduism					{ get { return hinduism; } }
		public static Religion Confucianism				{ get { return confucianism; } }
		public static Religion Protestant				{ get { return protestant; } }
		public static Religion Reformed					{ get { return reformed; } }
		public static Religion Orthodox					{ get { return orthodox; } }
		public static Religion Pagan					{ get { return exotic; } }
		public static Religion Shiite					{ get { return shiite; } }
		public static Religion Sunni					{ get { return sunni; } }

		private static Religion buddhism = new Religion( "buddhism" );
		private static Religion confucianism = new Religion( "confucianism" );
		private static Religion exotic = new Religion( "exotic" );
		private static Religion catholic = new Religion( "catholic" );
		private static Religion crc = new Religion( "counterreform" );
		private static Religion hinduism = new Religion( "hinduism" );
		private static Religion orthodox = new Religion( "orthodox" );
		private static Religion protestant = new Religion( "protestant" );
		private static Religion reformed = new Religion( "reformed" );
		private static Religion sunni = new Religion( "sunni" );
		private static Religion shiite = new Religion( "shiite" );
		#endregion
	}
}
