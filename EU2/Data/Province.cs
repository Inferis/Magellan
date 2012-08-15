#define use2020

using System;
using System.Drawing;
using System.Xml;
using System.Collections;
using System.Text.RegularExpressions;
using EU2.IO;
using EU2.Enums;

namespace EU2.Data
{
	/// <summary>
	/// 
	/// </summary>
	public class Province : ICSVReadable, ICSVWriteable {
		public const ushort MinValue = 0;
#if use2020
		public const ushort MaxValue = 2020;
#else
		public const ushort MaxValue = 1615;
#endif
		public const ushort Count = MaxValue-1;
		public const ushort MinID = 0;
		public const ushort MaxID = Count-1;
		public const ushort InternalCount = MaxValue+2; // border outlineID ...

		// Special IDs
		public const ushort TerraIncognitaID = MinID;
		public const ushort BorderID = MaxValue;
		public const ushort BorderOutlineID = MaxValue+1;
		
		public static Province Filler = new Province( true );

		private const int TerrainVariantCount = 4;
		private const int ExtraRiverDiscoverCount = 3;

		private static ushort[] DefaultIDsHRE = new ushort[] { 302, 304, 305, 306, 310, 311, 312, 313, 314, 315, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 343, 344, 345, 346, 347, 348, 349, 350, 351, 369, 371, 372, 373, 374, 375, 377, 378, 379, 380, 387, 388, 389, 400, 401, 402, 403, 404, 405, 1612 }; 
		private static ushort[] DefaultIDsTOTSpain = new ushort[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 54, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 198, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 690, 691, 692, 693, 694, 817, 822 };
		private static ushort[] DefaultIDsTOTPortugal = new ushort[] { 183, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 568, 672, 695, 696, 697, 698, 699, 700, 701, 702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716, 717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731, 750, 751, 752, 753, 754, 755, 756, 757, 758, 759, 760, 761, 762, 763, 764, 765, 766, 767, 768, 769, 770, 771, 772, 773, 774, 775, 776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 816 }; 

		private ushort id;
		private string name;
		private PlaceInWorld piw;
		private EU2.Enums.Religion religion;
		private EU2.Enums.Culture culture;
		private int sizeModifier, pashas, climate;
		private int ice, storm;
		private bool galleys;
		private int manpower, income;
		private EU2.Enums.Terrain terrain;
		private bool isHRE, isTOTSpain, isTOTPortugal;
		private bool mine;
		private int minevalue;
		private EU2.Enums.Goods goods;
		private bool upgradable;
		private int cotHistoricalModifier, colonizationDifficulty, nativeCombatStrength;
		private float ferocity, nativeCombatEfficiency;
		private int tradingPostsNegotiation;
		private int nativesTolerance, portAdjacency;
		private Point cityPos, armyPos, portPos, manufactoryPos;
		private TerrainVariant[] terrainVariants;
		private string area, region, continent, defaultCityName;
		private int[] extraRiverDiscover;
		private Point fillCoordinates;

		private Province( bool dummy ) {
			InitProps( ushort.MaxValue, "Filler" );
		}

		public Province() {
			InitProps( 0, "unnamed" );
		}

		public Province( ushort id ) {
			if ( id == 0 )
				InitProps( id, "PTI" );
			else
				InitProps( id, "Province" + id );
		}

		public Province( ushort id, string name ) {
			InitProps( id, name );
		}

		private void InitProps( ushort id, string name ) {
			this.id = id;
			this.name = name;
			piw = PlaceInWorld.Inland;
			religion = Religion.Exotic;
			culture = Culture.None;
			sizeModifier = -1;
			pashas = -1;
			climate = -1;
			ice = -1;
			storm = -1;
			galleys = false;
			manpower = -1;
			income = -1;
			terrain = Terrain.Unknown;
			isTOTSpain = false;
			isTOTPortugal = false;
			isHRE = false;
			mine = false;
			minevalue = -1;
			goods = Goods.Nothing;
			upgradable = false;
			cotHistoricalModifier = -1;
			colonizationDifficulty = -1;
			nativeCombatStrength = -1;
			ferocity = -1;
			nativeCombatEfficiency = -1;
			tradingPostsNegotiation = -1;
			nativesTolerance = -1;
			cityPos = new Point( -1, -1 );
			armyPos = new Point( -1, -1 );
			portPos = new Point( -1, -1 );
			manufactoryPos = new Point( -1, -1 );
			terrainVariants = new TerrainVariant[TerrainVariantCount];
			area = "";
			region = "";
			continent = "";
			defaultCityName = null;
			extraRiverDiscover = new int[ExtraRiverDiscoverCount];
		}

		public bool ReadFromCSV( CSVReader reader ) {
			return ReadFromCSV( reader, GlobalConfig.AllowTOTAndHREInProvinceList );
		}

		public bool ReadFromCSV( CSVReader reader, bool includeTOTAndHRE ) {
			int pid = reader.ReadInt();

			if ( pid < 0 ) return false; // Stop reading
			
			try {
				id = (ushort)pid;
				name = reader.ReadString();
				piw = PlaceInWorld.FromName( reader.ReadString() );
				religion = EU2.Enums.Religion.FromName( reader.ReadString() );
				culture = EU2.Enums.Culture.FromName( reader.ReadString() );
				sizeModifier = reader.ReadInt();
				pashas = reader.ReadInt();
				climate = reader.ReadInt();
				ice = reader.ReadInt();
				storm = reader.ReadInt();
				galleys = reader.ReadBoolean ();
				manpower = reader.ReadInt();
				income = reader.ReadInt();
				terrain = EU2.Enums.Terrain.FromID( reader.ReadInt() );
				if ( includeTOTAndHRE ) {
					isTOTSpain = reader.ReadBoolean();
					isTOTPortugal = reader.ReadBoolean();
					isHRE = reader.ReadBoolean();
				}
				else {
					isTOTSpain = Array.IndexOf( DefaultIDsTOTSpain, id) >= 0;
					isTOTPortugal = Array.IndexOf( DefaultIDsTOTPortugal, id) >= 0;
					isHRE = Array.IndexOf( DefaultIDsHRE, id) >= 0;
				}
				mine = reader.ReadBoolean();
				minevalue = reader.ReadInt();
				goods = EU2.Enums.Goods.FromName( reader.ReadString() );
				upgradable = reader.ReadBoolean();
				cotHistoricalModifier = reader.ReadInt();
				colonizationDifficulty = reader.ReadInt();
				nativeCombatStrength = reader.ReadInt();
				ferocity = reader.ReadFloat();
				nativeCombatEfficiency = reader.ReadFloat();
				tradingPostsNegotiation = reader.ReadInt();
				nativesTolerance = reader.ReadInt();
				cityPos = reader.ReadPoint();
				armyPos = reader.ReadPoint();
				portPos = reader.ReadPoint();
				manufactoryPos = reader.ReadPoint();
				portAdjacency = reader.ReadInt();
				for ( int i=0; i<TerrainVariantCount; ++i ) {
					terrainVariants[i] = new TerrainVariant( reader.ReadPoint(), reader.ReadInt( 0 ) );
				}
				area = reader.ReadString().Trim();
				region = reader.ReadString().Trim();
				continent = reader.ReadString().Trim();
				defaultCityName = reader.ReadString().Trim();
				if ( defaultCityName == "#N/A" || defaultCityName.Length == 0 ) defaultCityName = null;
				for ( int i=0; i<ExtraRiverDiscoverCount; ++i ) {
					extraRiverDiscover[i] = reader.ReadInt( 0 );
				}

				reader.SkipRow();
			}
			catch ( Exception e ) {
				throw new ProvinceFormatException( reader.LineNum+1, reader.ItemNum+1, e );
			}
			return true;
		}

		public void WriteToCSV( CSVWriter writer ) {
			if ( id == ushort.MaxValue ) 
				writer.Write( -1 );
			else
				writer.Write( id );
			writer.Write( name );
			writer.Write( piw.Name );
			writer.Write( religion.Name );
			writer.Write( culture.Name );
			writer.Write( sizeModifier );
			writer.Write( pashas );
			writer.Write( climate );
			writer.Write( ice );
			writer.Write( storm );
			writer.Write( galleys );
			writer.Write( manpower );
			writer.Write( income );
			writer.Write( terrain.ID );
			if ( GlobalConfig.AllowTOTAndHREInProvinceList ) {
				writer.Write( isTOTSpain );
				writer.Write( isTOTPortugal );
				writer.Write( isHRE );
			}
			writer.Write( mine );
			writer.Write( minevalue );
			writer.Write( goods.Name );
			writer.Write( upgradable );
			writer.Write( cotHistoricalModifier );
			writer.Write( colonizationDifficulty );
			writer.Write( nativeCombatStrength );
			writer.Write( ferocity );
			writer.Write( nativeCombatEfficiency );
			writer.Write( tradingPostsNegotiation );
			writer.Write( nativesTolerance );
			writer.Write( cityPos );
			writer.Write( armyPos );
			writer.Write( portPos );
			writer.Write( manufactoryPos.X );
			writer.Write( manufactoryPos.Y );
			writer.Write( portAdjacency );
			for ( int i=0; i<TerrainVariantCount; ++i ) {
				writer.Write( terrainVariants[i].Location );
				writer.Write( terrainVariants[i].Variant  );
			}
			writer.Write( area );
			writer.Write( region );
			writer.Write( continent );
			if ( defaultCityName == null || defaultCityName.Length == 0 ) 
				writer.Write( "#N/A" );
			else
				writer.Write( defaultCityName );
			for ( int i=0; i<ExtraRiverDiscoverCount; ++i ) {
				writer.Write( extraRiverDiscover[i] );
			}
			writer.Write( piw.Value < 0 ? "#N/A" : piw.Value.ToString() );
			writer.Write( fillCoordinates );

			writer.EndRow();
		}

		public bool ReadFrom( XmlNode source ) {
			int pid = -1;
			try {
				pid = int.Parse( source.Attributes.GetNamedItem( "id" ).Value );
			}
			catch {
				return false;
			}
			if ( pid < 0 ) return false; // Stop reading

			name = source.SelectSingleNode( "Name" ).InnerText;
			culture = EU2.Enums.Culture.FromName( source.SelectSingleNode( "Culture" ).InnerText );
			religion = EU2.Enums.Religion.FromName( source.SelectSingleNode( "Religion" ).InnerText );
			sizeModifier = int.Parse( source.SelectSingleNode( "SizeModifier" ).InnerText );
			pashas = int.Parse( source.SelectSingleNode( "Pashas" ).InnerText );

			XmlNode parent = source.SelectSingleNode( "Weather" );
			climate = int.Parse( parent.Attributes.GetNamedItem( "climate" ).Value );
			ice = int.Parse( parent.Attributes.GetNamedItem( "ice" ).Value );
			storm = int.Parse( parent.Attributes.GetNamedItem( "storm" ).Value );

			galleys = source.SelectSingleNode( "Galleys" ).InnerText.ToLower() == "yes";
			manpower = int.Parse( source.SelectSingleNode( "Manpower" ).InnerText );
			income = int.Parse( source.SelectSingleNode( "Income" ).InnerText );
			terrain = EU2.Enums.Terrain.FromID( int.Parse( source.SelectSingleNode( "Terrain" ).InnerText ) );

			parent = source.SelectSingleNode( "Mine" );
			mine = parent.Attributes.GetNamedItem( "available" ).Value.ToLower() == "yes";
			minevalue = int.Parse( parent.Attributes.GetNamedItem( "value" ).Value );

			goods = EU2.Enums.Goods.FromName( source.SelectSingleNode( "Goods" ).InnerText );
			cotHistoricalModifier = int.Parse( source.SelectSingleNode( "COTHistoricalModifier" ).InnerText );
			upgradable = source.SelectSingleNode( "Upgradable" ).InnerText.ToLower() == "yes";

			parent = source.SelectSingleNode( "Colonisation" );
			colonizationDifficulty = int.Parse( parent.SelectSingleNode( "ColonizationDifficulty" ).InnerText );
			nativeCombatStrength = int.Parse( parent.SelectSingleNode( "NativeCombatStrength" ).InnerText );
			ferocity = float.Parse( parent.SelectSingleNode( "Ferocity" ).InnerText );
			nativeCombatEfficiency = float.Parse( parent.SelectSingleNode( "NativeCombatEfficiency" ).InnerText );
			tradingPostsNegotiation = int.Parse( parent.SelectSingleNode( "TradingPostsNegotiation" ).InnerText );
			nativesTolerance = int.Parse( parent.SelectSingleNode( "NativesTolerance" ).InnerText );

			parent = source.SelectSingleNode( "CityPosition" );
			cityPos = new Point( 
				int.Parse( parent.Attributes.GetNamedItem( "x" ).Value ),
				int.Parse( parent.Attributes.GetNamedItem( "y" ).Value ) );

			parent = source.SelectSingleNode( "ArmyPosition" );
			armyPos = new Point( 
				int.Parse( parent.Attributes.GetNamedItem( "x" ).Value ),
				int.Parse( parent.Attributes.GetNamedItem( "y" ).Value ) );
			
			parent = source.SelectSingleNode( "PortPosition" );
			portPos = new Point( 
				int.Parse( parent.Attributes.GetNamedItem( "x" ).Value ),
				int.Parse( parent.Attributes.GetNamedItem( "y" ).Value ) );
			portAdjacency = int.Parse( parent.Attributes.GetNamedItem( "adjacency" ).Value );

			parent = source.SelectSingleNode( "ManufactoryPosition" );
			manufactoryPos = new Point( 
				int.Parse( parent.Attributes.GetNamedItem( "x" ).Value ),
				int.Parse( parent.Attributes.GetNamedItem( "y" ).Value ) );

			int i = 0;
			parent = source.SelectSingleNode( "TerrainVariants" );
			foreach ( XmlNode subparent in parent.SelectNodes( "TerrainVariant" ) ) {
				terrainVariants[i++] = new TerrainVariant( 
					new Point( 
						int.Parse( subparent.Attributes.GetNamedItem( "x" ).Value ),
						int.Parse( subparent.Attributes.GetNamedItem( "y" ).Value ) ),
					int.Parse( subparent.Attributes.GetNamedItem( "variant" ).Value ) );
			}

			parent = source.SelectSingleNode( "Geography" );
			area = parent.SelectSingleNode( "Area" ).InnerText;
			region = parent.SelectSingleNode( "Region" ).InnerText;
			continent = parent.SelectSingleNode( "Continent" ).InnerText;

			if ( source.SelectSingleNode( "DefaultCityName" ) != null ) {
				defaultCityName = source.SelectSingleNode( "DefaultCityName" ).InnerText;
				if ( defaultCityName == "#N/A" || defaultCityName.Length == 0 ) defaultCityName = null;
			}

			i = 0;
			parent = source.SelectSingleNode( "ExtraRiverDiscoveries" );
			foreach ( XmlNode subparent in parent.SelectNodes( "ExtraRiverDiscovery" ) ) {
				extraRiverDiscover[i++] = int.Parse( subparent.InnerText );
			}

			parent = source.SelectSingleNode( "FillCoordinates" );
			if ( parent != null ) {
				fillCoordinates = new Point( 
					int.Parse( parent.Attributes.GetNamedItem( "x" ).Value ),
					int.Parse( parent.Attributes.GetNamedItem( "y" ).Value ) );
			}

			return true;
		}
			
		public bool ReadFrom( XmlReader reader ) {
			// Get id
			if ( !reader.HasAttributes ) return false; // stop reading

			int pid = int.Parse(reader.GetAttribute( "id" ));
			if ( pid < 0 ) return false; // Stop reading
			
			while ( reader.Read() ) {
				// Stop when end element is reached
				if ( reader.NodeType == XmlNodeType.EndElement && reader.Name == "Province" ) break;
				if ( reader.NodeType != XmlNodeType.Element ) continue; // Skip non-elements

				switch ( reader.Name ) {
					case "Name":
						reader.Read(); // read value
						name = reader.Value; 
						reader.Read(); // read end element
						break;

					case "PlaceInWorld":
						reader.Read(); // read value
						piw = PlaceInWorld.FromName( reader.Value );
						reader.Read(); // read end element
						break;

					case "Culture":
						reader.Read(); // read value
						culture = EU2.Enums.Culture.FromName( reader.Value );
						reader.Read(); // read end element
						break;

					case "Religion":
						reader.Read(); // read value
						religion = EU2.Enums.Religion.FromName( reader.Value );
						reader.Read(); // read end element
						break;

					case "SizeModifier":
						reader.Read(); // read value
						religion = EU2.Enums.Religion.FromName( reader.Value );
						reader.Read(); // read end element
						break;
				}
			}
			return true;
		}
			
		public void WriteTo( XmlWriter writer ) {
			writer.WriteStartElement( "Province" );

			if ( id < ushort.MaxValue ) 
				writer.WriteAttributeString( "id", id.ToString() );

			writer.WriteElementString( "Name", name );
			writer.WriteStartElement( "PlaceInWorld" );
			if ( piw.Value >= 0 ) writer.WriteAttributeString( "value", piw.Value.ToString() );
			writer.WriteRaw( piw.Name );
			writer.WriteEndElement();
			writer.WriteElementString( "Culture", culture.Name );
			writer.WriteElementString( "Religion", religion.Name );
			writer.WriteElementString( "SizeModifier", sizeModifier.ToString() );
			writer.WriteElementString( "Pashas", pashas.ToString() );

			writer.WriteStartElement( "Weather" );
			writer.WriteAttributeString( "climate", climate.ToString() );
			writer.WriteAttributeString( "ice", ice.ToString() );
			writer.WriteAttributeString( "storm", storm.ToString() );
			writer.WriteEndElement();

			writer.WriteElementString( "Galleys", galleys ? "yes" : "no" );
			writer.WriteElementString( "Manpower", manpower.ToString() );
			writer.WriteElementString( "Income", income.ToString() );
			writer.WriteElementString( "Terrain", terrain.ID.ToString() );

			writer.WriteStartElement( "Mine" );
			writer.WriteAttributeString( "available", mine ? "yes" : "no" );
			writer.WriteAttributeString( "value", minevalue.ToString() );
			writer.WriteEndElement();

			writer.WriteElementString( "Goods", goods.Name );
			writer.WriteElementString( "COTHistoricalModifier", cotHistoricalModifier.ToString() );
			writer.WriteElementString( "Upgradable", upgradable ? "yes" : "no" );

			writer.WriteStartElement( "Colonisation" );
			writer.WriteElementString( "ColonizationDifficulty", colonizationDifficulty.ToString() );
			writer.WriteElementString( "NativeCombatStrength", nativeCombatStrength.ToString() );
			writer.WriteElementString( "Ferocity", ferocity.ToString() );
			writer.WriteElementString( "NativeCombatEfficiency", nativeCombatEfficiency.ToString() );
			writer.WriteElementString( "TradingPostsNegotiation", tradingPostsNegotiation.ToString() );
			writer.WriteElementString( "NativesTolerance", nativesTolerance.ToString() );
			writer.WriteEndElement();

			writer.WriteStartElement( "CityPosition" );
			writer.WriteAttributeString( "x", cityPos.X.ToString() );
			writer.WriteAttributeString( "y", cityPos.Y.ToString() );
			writer.WriteEndElement();

			writer.WriteStartElement( "ArmyPosition" );
			writer.WriteAttributeString( "x", armyPos.X.ToString() );
			writer.WriteAttributeString( "y", armyPos.Y.ToString() );
			writer.WriteEndElement();

			writer.WriteStartElement( "ManufactoryPosition" );
			writer.WriteAttributeString( "x", manufactoryPos.X.ToString() );
			writer.WriteAttributeString( "y", manufactoryPos.Y.ToString() );
			writer.WriteEndElement();
			
			writer.WriteStartElement( "PortPosition" );
			writer.WriteAttributeString( "x", portPos.X.ToString() );
			writer.WriteAttributeString( "y", portPos.Y.ToString() );
			writer.WriteAttributeString( "adjacency", portAdjacency.ToString() );
			writer.WriteEndElement();
			
			writer.WriteStartElement( "TerrainVariants" );
			for ( int i=0; i<TerrainVariantCount; ++i ) {
				if ( terrainVariants[i].Location.X < 0 || terrainVariants[i].Location.Y < 0 ) continue;
				writer.WriteStartElement( "TerrainVariant" );
				writer.WriteAttributeString( "x", terrainVariants[i].Location.X.ToString() );
				writer.WriteAttributeString( "y", terrainVariants[i].Location.Y.ToString() );
				writer.WriteAttributeString( "variant", terrainVariants[i].Variant.ToString() );
				writer.WriteEndElement();
			}
			writer.WriteEndElement();

			writer.WriteStartElement( "Geography" );
			writer.WriteElementString( "Area", area );
			writer.WriteElementString( "Region", region );
			writer.WriteElementString( "Continent", continent );
			writer.WriteEndElement();

			if ( defaultCityName != null && defaultCityName.Length > 0 ) writer.WriteElementString( "DefaultCityName", defaultCityName );
			writer.WriteStartElement( "ExtraRiverDiscoveries" );
			for ( int i=0; i<ExtraRiverDiscoverCount; ++i ) {
				if ( extraRiverDiscover[i] == 0 ) continue;
				writer.WriteElementString( "ExtraRiverDiscovery", extraRiverDiscover[i].ToString() );
			}
			writer.WriteEndElement();

			writer.WriteStartElement( "FillCoordinates" );
			writer.WriteAttributeString( "x", fillCoordinates.X.ToString() );
			writer.WriteAttributeString( "y", fillCoordinates.Y.ToString() );
			writer.WriteEndElement();
			
			writer.WriteEndElement(); // Province
			writer.WriteWhitespace( "\n" );
		}

		public bool IsRiver() { 
			return terrain == Terrain.River;
		}

		public bool IsOcean() { 
			return terrain == Terrain.Ocean;
		}
		
		public bool IsLand() { 
			return terrain == Terrain.Plains || terrain == Terrain.Forest || terrain == Terrain.Desert || 
				terrain == Terrain.Mountain || terrain == Terrain.Marsh || terrain == Terrain.Unknown;
		}
		
		public bool IsLandNoTI() { 
			return id > TerraIncognitaID && id < Province.Count && IsLand();
		}
		

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
		// 9610;1618;9517;1574;9574;1629;938;
		// 9532;1618;1;9626;1628;1;-100;-100;0;-100;-100;0;
		// Low Countries;Western Europe;Europe;Vlissingen;0;0;
		// 0;1;9555;1595;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1

		public ushort ID {
			get { return id; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public PlaceInWorld PIW {
			get { return piw; }
			set { piw = value; }
		}
		
		public EU2.Enums.Religion Religion {
			get { return religion; }
			set { religion = value; }
		}
		
		public EU2.Enums.Culture Culture {
			get { return culture; }
			set { culture = value; }
		}
		
		public int SizeModifier {
			get { return sizeModifier; }
			set { sizeModifier = value; }
		}
		
		public int Pashas {
			get { return pashas; }
			set { manpower = value; }
		}
		
		public int Climate {
			get { return climate; }
			set { climate = value; }
		}
		
		public int Ice {
			get { return ice; }
			set { ice = value; }
		}

		public int Storm {
			get { return storm; }
			set { storm = value; }
		}
		
		public bool Galleys {
			get { return galleys; }
			set { galleys = value; }
		}

		public int Manpower {
			get { return manpower; }
			set { manpower = value; }
		}

		public int Income {
			get { return income; }
			set { income = value; }
		}

		public EU2.Enums.Terrain Terrain {
			get { return terrain; }
			set { terrain = value; }
		}

		public bool Mine {
			get { return mine; }
			set { mine = value; }
		}

		public int MineValue {
			get { return minevalue; }
			set { minevalue = value; }
		}
		
		public EU2.Enums.Goods Goods {
			get { return goods; }
			set { goods = value; }
		}

		public bool Upgradable {
			get { return upgradable; }
			set { upgradable = value; }
		}

		public int COTHistoricalModifier {
			get { return cotHistoricalModifier; }
			set { cotHistoricalModifier = value; }
		}
		
		public int ColonizationDifficulty {
			get { return colonizationDifficulty; }
			set { colonizationDifficulty = value; }
		}
		
		public int NativeCombatStrength {
			get { return nativeCombatStrength; }
			set { nativeCombatStrength = value; }
		}
		
		public float NativeFerocity {
			get { return ferocity; }
			set { ferocity = value; }
		}
		
		public float NativeCombatEfficiency {
			get { return nativeCombatEfficiency; }
			set { nativeCombatEfficiency = value; }
		}
		
		public int TradingPostsNegotiation {
			get { return tradingPostsNegotiation; }
			set { tradingPostsNegotiation = value; }
		}
		
		public int NativesTolerance {
			get { return nativesTolerance; }
			set { nativesTolerance = value; }
		}
		
		public System.Drawing.Point CityPosition {
			get { return cityPos; }
			set { cityPos = value; }
		}
		
		public System.Drawing.Point ArmyPosition {
			get { return armyPos; }
			set { armyPos = value; }
		}
		
		public System.Drawing.Point PortPosition {
			get { return portPos; }
			set { portPos = value; }
		}
		
		public System.Drawing.Point ManufactoryPosition {
			get { return manufactoryPos; }
			set { manufactoryPos = value; }
		}

		public int PortAdjacency {
			get { return portAdjacency; }
			set { portAdjacency = value; }
		}
		
		public TerrainVariant[] TerrainVariants {
			get { return terrainVariants; }
			set { 
				if ( value == null )
					terrainVariants = new TerrainVariant[TerrainVariantCount];
				else {
					for ( int i=0; i<TerrainVariantCount; ++i ) {
						if ( i<value.Length ) terrainVariants[i] = value[i];
					}
				}
			}
		}

		public string Area {
			get { return area; }
			set { area = value; }
		}
		
		public string Region {
			get { return region; }
			set { region = value; }
		}
		
		public string Continent {
			get { return continent; }
			set { continent = value; }
		}
		
		public string DefaultCityName {
			get { return defaultCityName; }
			set { if ( value != null && value.Trim().Length == 0 ) value = null; defaultCityName = value; }
		}
		
		public int[] ExtraRiverDiscover {
			get { return extraRiverDiscover; }
			set { 
				if ( value == null )
					extraRiverDiscover = new int[ExtraRiverDiscoverCount];
				else {
					for ( int i=0; i<ExtraRiverDiscoverCount; ++i ) {
						extraRiverDiscover[i] = i<value.Length ? value[i] : 0;
					}
				}
			}
		}

		public System.Drawing.Point FillCoordinates {
			get { return fillCoordinates; }
			set { fillCoordinates = value; }
		}
		
		#endregion
	}

	public class ProvinceFormatException : Exception {
		public ProvinceFormatException( ) : base( "Formatting error in province data" ) {
			this.line = -1;
			this.item = -1;
		}

		public ProvinceFormatException( Exception e ) : base( "Formatting error in province data", e ) {
			this.line = -1;
			this.item = -1;
		}

		public ProvinceFormatException( int line ) : base( "Formatting error in province data (line " + line + ")" ) {
			this.line = line;
			this.item = -1;
		}

		public ProvinceFormatException( int line, Exception e ) : base( "Formatting error in province data (line " + line + ")", e ) {
			this.line = line;
			this.item = -1;
		}

		public ProvinceFormatException( int line, int item ) : base( "Formatting error in province data (line " + line + ", item " + item + ")" ) {
			this.line = line;
			this.item = item;
		}

		public ProvinceFormatException( int line, int item, Exception e ) : base( "Formatting error in province data (line " + line + ", item " + item + ")", e ) {
			this.line = line;
			this.item = item;
		}

		public int Line {
			get { return line; }
		}

		public int Item {
			get { return item; }
		}

		private int line, item;
	}
}
