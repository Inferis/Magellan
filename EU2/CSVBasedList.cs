using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace EU2
{
	/// <summary>
	/// Summary description for CSVBasedItem.
	/// </summary>
	public abstract class CSVBasedItem : InstallLinkedObject {
		public CSVBasedItem( EU2.Install install, System.IO.StreamReader reader ) : base( install ) {
			if ( !ReadFrom( reader ) ) throw new AtEndOfStreamException();
		}

		public CSVBasedItem( EU2.Install install ) : base( install ) {
		}
		
		public abstract bool ReadFrom( System.IO.StreamReader reader );
		public abstract void WriteTo( System.IO.StreamWriter reader );
		public abstract object CSVBasedKey { get; }
	}

	/// <summary>
	/// Summary description for CSVBasedList.
	/// </summary>
	public abstract class CSVBasedList : InstallLinkedObject
	{
		protected System.Collections.Specialized.HybridDictionary list;
		private string csvHeaderLine;
		private System.Type itemType;

		public CSVBasedList( EU2.Install install, string csvHeaderLine, System.Type itemType ) : base( install ) {
			this.list = new System.Collections.Specialized.HybridDictionary();
			this.csvHeaderLine = csvHeaderLine;
			this.itemType = itemType;
		}

		public CSVBasedList( EU2.Install install, string csvHeaderLine, System.Type itemType, System.Collections.Specialized.HybridDictionary list )  : base( install ) {
			this.list = new System.Collections.Specialized.HybridDictionary();
			this.csvHeaderLine = csvHeaderLine;
			this.itemType = itemType;
		}

		public int Count {
			get { return list.Count; }
		}

		public bool ReadFrom( string filename ) {
			FileStream stream = new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read );
			StreamReader reader = new StreamReader( stream );
			try {
				return ReadFrom( reader );
			}
			finally {
				reader.Close();
				stream.Close();
			}
		}

		public bool ReadFrom( System.IO.StreamReader reader ) {
			list.Clear();

			// Read the first line, as it will be the header line
			string line = reader.ReadLine();
			if ( line != csvHeaderLine ) throw new NonMatchingHeaderLineException();

			while ( true ) {
				CSVBasedItem item;
				try {
					item = (CSVBasedItem)Activator.CreateInstance(this.itemType, new object[2] { Install, reader } );
					list.Add( item.CSVBasedKey, item );
				}
				catch ( System.Reflection.TargetInvocationException e ) {
					try {
						throw e.InnerException;
					} 
					catch ( IgnoreItemExpection ) {
						// Just continue
					}
					catch ( AtEndOfStreamException ) {
						break;
					}
				}
			}

			return true;
		}

		public void WriteTo( string filename ) {
			FileStream stream = new FileStream( filename, FileMode.Create, FileAccess.Write, FileShare.None );
			StreamWriter writer = new StreamWriter( stream );
			try {
				WriteTo( writer );
			}
			finally {
				writer.Close();
				stream.Close();
			}
		}

		public void WriteTo( System.IO.StreamWriter writer ) {
			IEnumerator enm = list.GetEnumerator();

			for ( enm.Reset(); enm.MoveNext(); ) {
				((CSVBasedItem)enm.Current).WriteTo( writer );
			}
		}

		protected string CSVHeaderLine {
			get { return csvHeaderLine; }
		}

	}

	public class AtEndOfStreamException : Exception {}
	public class NonMatchingHeaderLineException : Exception {}
	public class IgnoreItemExpection : Exception {}

	public abstract class CSVUtils {
		private static Regex CSVREGEX = new Regex( "\"([^\"\\\\]*(\\.[^\"\\\\]*)*)\";?|([^;]+);?|;", RegexOptions.Compiled );

		public static string CSVString( string value ) {
			if ( value.IndexOfAny( new char[] { ',', ';' } ) >= 0 ) {
				return "\"" + value + "\"";
			}
			else {
				return value;
			}
		}

		public static string[] CSVValues( string csvLine ) {
			System.Collections.ArrayList data = new System.Collections.ArrayList();

			Match match = CSVREGEX.Match( csvLine );
			while ( match.Success ) {
				string item;
				if ( match.Groups[1].Captures.Count > 0 ) 
					item = match.Groups[1].Captures[0].Value;
				else if ( match.Groups[3].Captures.Count > 0 ) 
					item = match.Groups[3].Captures[0].Value;
				else
					item = "";

				// Add the item to the data
				data.Add( item );
				match = match.NextMatch();
			}
			
			// Return a string array
			string[] result = new string[data.Count];
			data.CopyTo( result );
			return result;
		}
	}
}
