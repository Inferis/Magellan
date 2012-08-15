using System;
using System.Collections;
using System.IO;
using System.Xml;
using EU2.IO;
using EU2.Enums;
using System.Collections.Generic;

namespace EU2.Data
{
	/// <summary>
	/// Summary description for TerrainList.
	/// </summary> 
	public class ProvinceList : IEnumerable<Province>, IStreamWriteable {
		private const int ListSize = Province.InternalCount;
		private Province[] list;

		public ProvinceList() {
			list = new Province[ListSize];
			for ( ushort i=0; i<ListSize; ++i ) list[i] = new Province( i );

			list[Province.BorderID].Terrain = Terrain.Border;
			list[Province.BorderOutlineID].Terrain = Terrain.Border;
		}

		public ProvinceList( CSVReader reader ) {
			list = new Province[ListSize];
			for ( ushort i=0; i<ListSize; ++i ) list[i] = new Province( i );

			ReadFrom( reader );

			list[Province.BorderID].Terrain = Terrain.Border;
			list[Province.BorderOutlineID].Terrain = Terrain.Border;
		}

		public Province this[int id] {
			get { return GetProvince( id ); }
			set { 
				if ( value == null || value.ID != id ) throw new InvalidOperationException();
				list[id] = value; 
			}
		}

		public Province GetProvince( ushort id ) {
			return id >= 0 && id <= list.Length ? list[id] : null;
		}

		public Province GetProvince( int id ) {
			return GetProvince( (ushort)id );
		}

		public bool Contains( ushort id ) {
			return (id >= 0 && id <= list.Length) && list[id] != null;
		}

		public bool Contains( Province prov ) {
			if ( prov == null ) return false;
			return Contains( prov.ID );
		}

		public bool Contains( int id ) {
			return Contains( (ushort)id );
		}

		public bool ReadFrom( CSVReader reader ) {
			// Skip first row
			string header = reader.ReadRow();
			bool newStyle = header.IndexOf( "ToT-SPA;ToT-POR;HRE;" ) >= 0;

			reader.SkipRow();
			for ( int i=0; i<=Province.Count; ++i ) {
				Province item = new Province();
				if ( !item.ReadFromCSV( reader, newStyle ) ) break;
				list[item.ID] = item;
			}

			return true;
		}

		public void WriteTo( CSVWriter writer ) {
			// Write top row
			if ( GlobalConfig.AllowTOTAndHREInProvinceList ) 
				writer.EndRow( "Id;Name;PIW name;Religion;Culture;SizeModifier;Pashas;Climate;Ice?;Storm?;Galleys;Manpower;Income;Terrain;ToT-SPA;ToT-POR;HRE;Mine(1) ?;MineValue;Goods;Uprgadable;CoT Historical Modifier;Difficulty for Colonization;Native Combat Strength;Ferocity;Efficiency of Natives in combat;Negotiation Value for Trading Posts;Natives Tolerance value ;City XPos;City YPos;Army XPos;ArmyYPos;PortXPos;Port YPos;Manufactory XPos; Manufactory YPos;Port/sea Adjacency;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant;Area;Region;Continent;;Extra River Desc Link 1;Extra River Desc Link 2;Extra River Desc Link 3;;Fill coord X;Fill coord Y;;;;;;;;;;;;;;;;" );
			else
				writer.EndRow( "Id;Name;PIW name;Religion;Culture;SizeModifier;Pashas;Climate;Ice?;Storm?;Galleys;Manpower;Income;Terrain;Mine(1) ?;MineValue;Goods;Uprgadable;CoT Historical Modifier;Difficulty for Colonization;Native Combat Strength;Ferocity;Efficiency of Natives in combat;Negotiation Value for Trading Posts;Natives Tolerance value ;City XPos;City YPos;Army XPos;ArmyYPos;PortXPos;Port YPos;Manufactory XPos; Manufactory YPos;Port/sea Adjacency;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant;Area;Region;Continent;;Extra River Desc Link 1;Extra River Desc Link 2;Extra River Desc Link 3;;Fill coord X;Fill coord Y;;;;;;;;;;;;;;;;" );

			for ( ushort i=0; i<=Province.Count; ++i ) {
				if ( list[i] == null ) list[i] = new Province( i, "Dummy" );

				list[i].WriteToCSV( writer );
			}
			Province.Filler.WriteToCSV( writer );
		}

		public bool ReadFrom( XmlReader reader ) {
			reader.Read();
			if ( reader.NodeType != XmlNodeType.XmlDeclaration ) throw new XmlException( "There is no XML declaration." );

			reader.Read();
			if ( reader.NodeType != XmlNodeType.Element || reader.Name != "Provinces" ) throw new XmlException( "Expected <Provinces> node, but got <" + reader.Name + ">." );

			while ( reader.Read() ) {
				if ( reader.NodeType == XmlNodeType.Element ) {
					if ( reader.Name != "Province" ) throw new XmlException( "Expected 'Province' node, but got <" + reader.Name + ">" );

					Province item = new Province();
					if ( !item.ReadFrom( reader ) ) break;
					list[item.ID] = item;
				}
			}

			return true;
		}

		public bool ReadFrom( XmlDocument doc ) {
			foreach ( XmlNode node in doc.GetElementsByTagName( "Province" ) ) {
				Province item = new Province();
				if ( !item.ReadFrom( node ) ) break;
				list[item.ID] = item;
			}

			return true;
		}

		public void WriteTo( XmlWriter writer ) {
			writer.WriteStartDocument();
			writer.WriteStartElement("Provinces");

			for ( ushort i=0; i<=Province.Count; ++i ) {
				if ( list[i] == null ) list[i] = new Province( i, "Dummy" );
				list[i].WriteTo( writer );
			}
			Province.Filler.WriteTo( writer );

			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
		}

		public void WriteTo( Stream stream ) {
			CSVWriter writer = new CSVWriter( stream );
			WriteTo( writer );
			writer.Flush();
		}

		public Province TerraIncognita { 
			get { return list[0]; } 
			set { 
				if ( value.ID != 0 ) throw new InvalidOperationException();
				list[0] = value; 
			}
		}


        #region IEnumerable<Province> Members

        public IEnumerator<Province> GetEnumerator() {
            return new List<Province>(list).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            return list.GetEnumerator();
        }

        #endregion
    }
}
