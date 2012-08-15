using System;
using System.Collections;
using System.IO;
using EU2.IO;

namespace EU2.Bare
{
	/// <summary>
	/// Summary description for TerrainList.
	/// This is forced to be a 10 item list, since the EU2 engine has 10 "hardcoded" item.
	/// </summary>
	public class TerrainList : IEnumerable, ICSVReadable
	{
		private const int ListSize = 10;
		private Terrain[] list;

		public TerrainList() {
			list = new Terrain[ListSize];
		}

		public Terrain LookupByName( string name ) {
			int index = LookupIndexByName( name );
			return index < 0 ? null : list[index];
		}

		public Terrain LookupByID( int id ) {
			int index = LookupIndexByID( id );
			return index < 0 ? null : list[index];
		}

		public Terrain this[string name] {
			get {
				return LookupByName( name );
			}
			set {
				int index = LookupIndexByName( name );
				if ( index <= 0 ) 
					Add( value );
				else
					list[index] = value; 
			}
		}

		public Terrain this[int id] {
			get {
				return LookupByID( id );
			}
			set {
				int index = LookupIndexByID( id );
				if ( index <= 0 ) 
					Add( value );
				else
					list[index] = value; 
			}
		}

		public IEnumerator GetEnumerator() {
			return list.GetEnumerator();
		}

		public bool ReadFromCSV( CSVReader reader ) {
			ArrayList items = new ArrayList();

			// Skip first row
			reader.SkipRow();
			for ( ; !reader.EOF ; ) {
				Terrain item = new Terrain( );
				if ( !item.ReadFromCSV( reader ) ) break;

				Add( item );
			}

			return true;
		}

		private int LookupIndexByName( string name ) {
			if ( list == null ) return -1;

			name = name.ToLower();
			for ( int i=0; i<list.Length; ++i ) {
				if ( list[i].Name.ToLower() == name ) return i;
			}

			return -1;
		}

		private int LookupIndexByID( int id ) {
			if ( list == null ) return -1;

			if ( id >=0 && id < list.Length && list[id] != null && list[id].ID == id ) return id;

			return -1;
		}

		private void Add( Terrain item ) {
			if ( item.ID >= 0 && item.ID < list.Length ) {
				// Assign specific item
				list[item.ID] = item;
			}
			else {
				throw new InvalidTerrainIDException( item.ID );
			}
		}	
		
	}

	public class InvalidTerrainIDException : Exception {
		int id;

		public InvalidTerrainIDException( int id ) {
			this.id = id;
		}

		public int ID {
			get { return id; }
		}
	}
}
