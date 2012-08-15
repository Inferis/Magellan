using System;
using System.Collections;
using System.IO;
using EU2.IO;

namespace EU2.Bare
{
	/// <summary>
	/// Summary description for ReligionList.
	/// </summary>
	public class ReligionList : IEnumerable, ICSVReadable
	{
		private Religion[] list;

		public ReligionList() {
			list = null;
		}

		public Religion Lookup( string name ) {
			int index = LookupIndex( name );
			return index < 0 ? null : list[index];
		}

		public Religion this[string name] {
			get {
				return Lookup( name );
			}
			set {
				int index = LookupIndex( name );
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
			for ( ; ; ) {
				Religion item = new Religion();
				if ( !item.ReadFromCSV( reader ) ) break;

				items.Add( item );
			}

			list = new Religion[items.Count];
			items.CopyTo( list );

			return true;
		}

		private int LookupIndex( string name ) {
			if ( list == null ) return -1;

			name = name.ToLower();
			for ( int i=0; i<list.Length; ++i ) {
				if ( list[i].Name.ToLower() == name ) return i;
			}

			return -1;
		}

		private void Add( Religion item ) {
			Religion[] oldlist = list;

			list = new Religion[list.Length+1];
			oldlist.CopyTo( list, 0 );
			list[oldlist.Length] = item;
		}	
		
	}
}
