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
	public class Goods : IEnumerable, ICSVReadable {
		private const int ListSize = 21;
		private Good[] list;

		public Goods() {
			list = new Good[ListSize] {   new Good( "coffee" ),
										  new Good( "cot" ),
										  new Good( "clo" ),
										  new Good( "grai" ),
										  new Good( "gold" ),
										  new Good( "fish" ),
										  new Good( "furs" ),
										  new Good( "ivor" ),
										  new Good( "metal" ),
										  new Good( "mineral" ),
										  new Good( "navs" ),
										  new Good( "orient" ),
										  new Good( "salt" ),
										  new Good( "slav" ),
										  new Good( "spic" ),
										  new Good( "sug" ),
										  new Good( "tea" ),
										  new Good( "tob" ),
										  new Good( "wine" ),
										  new Good( "wool" ),
										  new Good( "nothing" ) };
		}

		public Good LookupByName( string name ) {
			int index = LookupIndexByName( name );
			return index < 0 ? null : list[index];
		}

		public Good this[string name] {
			get {
				return LookupByName( name );
			}
			set {
				int index = LookupIndexByName( name );
				if ( list[index].Name.ToLower() != value.Name.ToLower() ) throw new InvalidOperationException();
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
			for ( int i=0; i<ListSize && !reader.EOF; ++i ) {
				Good item = new Good();
				if ( !item.ReadFromCSV( reader ) ) break;

				list[LookupIndexByName( item.Name )] = item;
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

	}
}
