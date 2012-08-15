using System;
using System.Collections;
using System.IO;
using System.Drawing;

namespace EU2.Data
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Provinces : EU2.CSVBasedList, IDisposable, IEnumerable {
		private const string CSVHEADER = "Id;Name;PIW name;Religion;Culture;SizeModifier;Pashas;Climate;Ice?;Storm?;Galleys;Manpower;Income;Terrain;Mine(1) ?;MineValue;Goods;Uprgadable;CoT Historical Modifier;Difficulty for Colonization;Native Combat Strength;Ferocity;Efficiency of Natives in combat;Negotiation Value for Trading Posts;Natives Tolerance value ;City XPos;City YPos;Army XPos;ArmyYPos;PortXPos;Port YPos;Manufactory XPos; Manufactory YPos;Port/sea Adjacency;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant;Terrain x;Terrain Y;Terrain variant;Area;Region;Continent;;Extra River Desc Link 1;Extra River Desc Link 2;Extra River Desc Link 3;;Fill coord X;Fill coord Y;;;;;;;;;;;;;;;;";
		private bool loaded;
		private EU2.Data.Mapfiles.BoundBox boundbox;
		private EU2.Data.Mapfiles.AdjacencyTable adjacencies;
		private EU2.Data.Mapfiles.IDTable idtable;
		private Province ti;

		public Provinces( EU2.Install install ) : base( install, CSVHEADER, typeof( Province ) ) {
			loaded = false;
			boundbox = null;
			idtable = null;
			adjacencies = null;
		}

		public Province this[int id] {
			get {
				return FindByID( (ushort)id );
			}
			set {
				Load();
				if ( list.Contains( id ) ) {
					list[id] = value;
				}
				else {
					list.Add( id, value );
				}
			}
		}

		public Province this[string name] {
			get {
				return FindByName( name );
			}
		}

		private void Load() {
			if ( loaded ) return;

			loaded = ReadFrom( Install.DBPath + "\\province.csv" );
			ti = FindByID( 0 );
		}

		public void Dispose() {
		}

		public System.Collections.IEnumerator GetEnumerator() {
			Load();
			return new System.Collections.SortedList( list ).Values.GetEnumerator();
			//return list.Values.GetEnumerator();
		}

		public EU2.Data.Mapfiles.BoundBox BoundBoxes {
			get {
				if ( boundbox == null ) boundbox = new EU2.Data.Mapfiles.BoundBox( Install );
				return boundbox;
			}
		}

		public EU2.Data.Mapfiles.AdjacencyTable Adjacencies {
			get {
				if ( adjacencies == null ) adjacencies = new EU2.Data.Mapfiles.AdjacencyTable( Install );
				return adjacencies;
			}
		}

		public Province FromLocation( int x, int y ) {
			if ( idtable == null ) idtable = new EU2.Data.Mapfiles.IDTable( Install );
			
			return this[idtable.HitTest( x, y )];
		}

		public Province FromLocation( Point location ) {
			return FromLocation( location.X, location.Y );
		}

		public Province FindByID( ushort id ) {
			Load();
			return list.Contains( id ) ? (Province)list[id] : null;
		}

		public Province FindByName( string name ) {
			Load();
			IEnumerator pe = list.GetEnumerator();
			
			for ( pe.Reset(); pe.MoveNext(); ) {
				Province prov = (Province)((DictionaryEntry)pe.Current).Value;
				if ( prov.Name.ToLower() == name.ToLower() ) return prov;
			}

			return null;
		}

		public Province TerraIncognita {
			get {
				Load();
				return ti;
			}
		}
	}
}
