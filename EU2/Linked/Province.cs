using System;
using System.Drawing;
using System.Collections;
using System.Text.RegularExpressions;
using EU2.Data.Mapfiles;

namespace EU2.Linked
{
	/// <summary>
	/// 
	/// </summary>
	public class Province : EU2.CSVBasedItem
	{
		public const ushort MinID = 0;
		public const ushort MaxID = 1615;

		private ushort id;
		private string name;
		private EU2.Data.PIWName piw;
		private EU2.Data.Religion religion;
		private EU2.Data.Culture culture;
		private int sizeModifier, pashas, climate;
		private int ice, storm, galleys, manpower, income;
		private TerrainType terrain;
		private bool mine;
		private int minevalue;
		private EU2.Data.Goods goods;
		private bool upgradable;
		private int cotHistoricalModifier, colonizationDifficulty, nativeCombatStrength;
		private int ferocity, nativeCombatEfficiency, tradingPostsNegotiation;
		private int nativesTolerance;
		private System.Drawing.Point cityPos, armyPos, portPos, manufactoryPos, fillCoord;
		private TerrainVariant[] terrainVariants;
		private string area, region, continent, capital;
		private string unknown1, unknown2;
		private string[] extraRiverDescLink;
		private Adjacent[] neighbors;

		public Province( EU2.Install install, ushort id ) : base( install ) {
			this.id = id;
		}

		public Province( EU2.Install install, System.IO.StreamReader reader ) : base( install, reader ) {
		}

		public Province GetNeighbor(int index) {
			LoadNeighbors();
			if ( index >= neighbors.Length ) return null;
			return install.Provinces[neighbors[index].ID];
		}

		#region CSVBasedItem Implementation
		public override bool ReadFrom(System.IO.StreamReader reader) {
			// Get a line from the stream, leave if we're at the end
			string line = reader.ReadLine();
			if ( line == null ) return false;

			// Convert line to csv values
			string[] data = EU2.CSVUtils.CSVValues( line );

			// fill properties
			int tmpid = int.Parse( data[0] );
			if ( tmpid < 0 ) throw new IgnoreItemExpection();
			id = (ushort)tmpid;
			name = data[1];
			terrain = Install.TerrainTypes[int.Parse( data[13] )];
			area = data[46];
			region = data[47];
			continent = data[48];
			fillCoord = new Point( int.Parse( data[54] ), int.Parse( data[55] ) );

			return true;
		}

		public override void WriteTo(System.IO.StreamWriter writer) {
		
		}

		public override object CSVBasedKey {
			get { return id; }
		}
		#endregion
		
		#region CSV Properties
		// Id;Name;PIW name;Religion;Culture;SizeModifier;Pashas;Climate;Ice?;Storm?;Galleys;
		// Manpower;Income;Terrain;Mine(1) ?;MineValue;Goods;Uprgadable;CoT Historical Modifier;
		// Difficulty for Colonization;Native Combat Strength;Ferocity;Efficiency of Natives in combat;
		// Negotiation Value for Trading Posts;Natives Tolerance value ;City XPos;City YPos;
		// Army XPos;ArmyYPos;PortXPos;Port YPos;Manufactory XPos; Manufactory YPos;
		// Port/sea Adjacency;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y; 
		// Terrain variant;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant; 
		// Area;Region;Continent;;Extra River Desc Link 1;Extra River Desc Link 2; // 
		// Extra River Desc Link 3;;Fill coord X;Fill coord Y;;;;;;;;;;;;;;;;
		// ---
		// 340;Zeeland;coastal;reformed;dutch;0;0;3;0;0;0;
		// 5;15;4;0;0;fish;1;2;
		// 0;0;0;0;
		// 0;0;9550;1590;
		// 9610;1618;9517;1574;9574;1629;
		// 938;9532;1618;1;9626;1628;
		// 1;-100;-100;0;-100;-100;0;
		// Low Countries;Western Europe;Europe;Vlissingen;0;0;
		// 0;1;9555;1595;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1

		public ushort ID {
			get { return id; }
		}

		public string Name {
			get { return name; }
		}
		
		public EU2.Data.PIWName PIWName {
			get { return piw; }
		}
		
		public EU2.Data.Religion Religion {
			get { return religion; }
		}
		
		public EU2.Data.Culture Culture {
			get { return culture; }
		}
		
		public int SizeModifier {
			get { return sizeModifier; }
		}
		
		public int Pashas {
			get { return pashas; }
		}
		
		public int Climate {
			get { return climate; }
		}
		
		public int Ice {
			get { return ice; }
		}

		public int Storm {
			get { return storm; }
		}
		
		public int Galleys {
			get { return galleys; }
		}

		public int Manpower {
			get { return manpower; }
		}

		public int Income {
			get { return income; }
		}

		public TerrainType Terrain {
			get { return terrain; }
		}

		public bool Mine {
			get { return mine; }
		}

		public int MineValue {
			get { return MineValue; }
		}
		
		public EU2.Data.Goods Goods {
			get { return goods; }
		}

		public bool Upgradable {
			get { return upgradable; }
		}

		public int COTHistoricalModifier {
			get { return cotHistoricalModifier; }
		}
		
		public int ColonizationDifficulty {
			get { return colonizationDifficulty; }
		}
		
		public int NativeCombatStrength {
			get { return nativeCombatStrength; }
		}
		
		public int Ferocity {
			get { return ferocity; }
		}
		
		public int NativeCombatEfficiency {
			get { return nativeCombatEfficiency; }
		}
		
		public int TradingPostsNegotiation {
			get { return tradingPostsNegotiation; }
		}
		
		public int NativesTolerance {
			get { return nativesTolerance; }
		}
		
		public System.Drawing.Point CityPosition {
			get { return cityPos; }
		}
		
		public System.Drawing.Point ArmyPosition {
			get { return armyPos; }
		}
		
		public System.Drawing.Point PortPosition {
			get { return portPos; }
		}
		
		public System.Drawing.Point ManufactoryPosition {
			get { return manufactoryPos; }
		}

		public System.Drawing.Point FillCoord {
			get { return fillCoord; }
		}

		public TerrainVariant[] TerrainVariants {
			get { return terrainVariants; }
		}

		public string Area {
			get { return area; }
		}
		
		public string Region {
			get { return region; }
		}
		
		public string Continent {
			get { return continent; }
		}
		
		public string Capital {
			get { return capital; }
		}
		
		public string[] ExtraRiverDescLink {
			get { return extraRiverDescLink; }
		}
		#endregion

		#region Other Properties
		public Rectangle BoundBox {
			get { 
				try {
					return Install.Provinces.BoundBoxes[id].Box;
				} 
				catch ( ArgumentOutOfRangeException ) {
					return new Rectangle( 0, 0, 0, 0 );
				}
			}
		}

		public EU2.Drawing.Mapview GetMapview( int level ) {
			return new EU2.Drawing.Mapview( this, level );
		}

		#endregion

		private void LoadNeighbors() {
			if ( neighbors != null ) return;

			neighbors = Install.Provinces.Adjacencies[id];
		}
	}

	public struct TerrainVariant {
		System.Drawing.Point location;
		int variant;

		TerrainVariant( int x, int y, int variant ) {
			location = new System.Drawing.Point( x, y );
			this.variant = variant;
		}

		TerrainVariant( System.Drawing.Point location, int variant ) {
			this.location = location;
			this.variant = variant;
		}

		public int X {
			get { return location.X; }
		}

		public int Y {
			get { return location.Y; }
		}
	
		public int Variant {
			get { return variant; }
		}
	}

}
