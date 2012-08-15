using System;
using System.IO;
using System.Drawing;
using EU2.Data;
using System.Globalization;
using System.Collections;

namespace EU2.IO
{
	/// <summary>
	/// Summary description for CSVReader.
	/// </summary>
	public class CSVReader 
	{
		private StreamReader reader;
		private string cache;
		private Queue items;
		private CultureInfo ci;
		private int currentline, currentitem;

		//private char[] buffer;
		//private int bufpos;
		//private int buflen;

		public CSVReader( string path ) {
			reader = new StreamReader( path, System.Text.Encoding.Default, false, 1024 );
			cache = null;
			ci = new CultureInfo( "en-US", false ); 
			currentline = 0;
			currentitem = 0;
			//buffer = new char[1024];
			//bufpos = buflen = 0;
		}

		public CSVReader( System.IO.Stream stream ) {
			reader = new StreamReader( stream, System.Text.Encoding.Default, false, 1024 );
			cache = null;
			ci = new CultureInfo( "en-US", false ); 
			currentline = 0;
			currentitem = 0;
			//buffer = new char[1024];
			//bufpos = buflen = 0;
		}

		public void Close() {
			if ( reader != null ) reader.Close();
			reader = null;
			ci = new CultureInfo( "en-US", false ); 
			//cache = null;
			//buffer = null;
		}

		public int LineNum {
			get { return currentline; }
		}

		public int ItemNum {
			get { return currentitem; }
		}

		public Stream BaseStream {
			get { return reader.BaseStream; }
		}

		public string ReadString() {
			return NextEntry();
		}

		public byte ReadByte() {
			return byte.Parse( NextEntry() );
		}

		public short ReadShort() {
			return short.Parse( NextEntry() );
		}

		public int ReadInt() {
			return int.Parse( NextEntry() );
		}

		public int ReadInt( int defaultValue ) {
			try {
				return int.Parse( NextEntry() );
			}
			catch {
				return defaultValue;
			}
		}

		public float ReadFloat() {
			return float.Parse( NextEntry(), ci );
		}

		public float ReadFloat( float defaultValue ) {
			try {
				return float.Parse( NextEntry(), ci );
			}
			catch {
				return defaultValue;
			}
		}

		public double ReadDouble() {
			return double.Parse( NextEntry(), ci );
		}

		public double ReadDouble( double defaultValue ) {
			try {
				return double.Parse( NextEntry(), ci );
			}
			catch {
				return defaultValue;
			}
		}

		public bool ReadBoolean() {
			return int.Parse( NextEntry() ) != 0;
			/*
			string tmp = NextEntry().Trim();
			bool result;
			try {
				result = bool.Parse( tmp ); 
				Console.WriteLine( "readboolean okay: " + tmp );
			}
			catch ( FormatException e ) {
				try {
					result = int.Parse( tmp ) > 0; 
				}
				catch ( FormatException ) {
					throw e; 
				}
			}

			return result;
			*/
		}

		public bool ReadBoolean( bool defaultValue ) {
			try {
				return int.Parse( NextEntry() ) != 0;
			}
			catch {
				return defaultValue;
			}
		}

		public Point ReadPoint() {
			return new Point( int.Parse( NextEntry() ), int.Parse( NextEntry() ) );
		}

		public Size ReadSize() {
			return new Size( int.Parse( NextEntry() ), int.Parse( NextEntry() ) );
		}

		public Rectangle ReadRectange() {
			return new Rectangle( ReadPoint(), ReadSize() );
		}

		public string ReadRow() {
			if ( reader == null ) throw new IOException();

			string row;
			if ( items == null || items.Count == 0 ) 
				row = reader.ReadLine();
			else {
				string[] tmp = new string[items.Count];
				items.CopyTo( tmp, 0 );
				for ( int i=0; i<tmp.Length; ++i ) {
					if ( tmp[i].IndexOf( " " ) >= 0 ) tmp[i] = "\"" + tmp[i] + "\"";
				}
				row = String.Join( ";", tmp );
			}

			items = null;
			return row;
		}

		public void SkipRow() {
			if ( reader == null ) throw new IOException();

			if ( items == null || items.Count == 0 ) 
				reader.ReadLine();
			items = null;
		}

		public void SkipEntry() {
			NextEntry();
		}

		public void SkipEntries( int count ) {
			for ( int i=0; i<count; ++i ) NextEntry();
		}

		public bool EOF {
			get { return PeekChar() == -1; }
		}

		public string Peek() {
			if ( cache == null ) {
				if ( !EOF ) cache = NextEntry();
			}

			return cache;
		}

		protected string NextEntry() {
			if ( cache != null ) {
				string tmp = cache;
				cache = null;
				return tmp;
			}

			if ( items == null || items.Count == 0 ) {
				items = new Queue( ParseLine(reader.ReadLine()) );
				++currentline;
				currentitem = -1;
			}

			string item = (string)(items.Dequeue());
			++currentitem;
			return item;
		}

#if false
		static private System.Text.RegularExpressions.Regex splitregex = new System.Text.RegularExpressions.Regex( "[;,](?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))" );

		private string[] ParseLine( string line ) {
			// This pattern actually recognizes the correct commas.
			// The Regex.Split() command later gets text between the commas.
			return splitregex.Split( line );
		}
#else
		private string[] ParseLine( string line ) {
			return line.Split( ';' );
		}
#endif
		protected string OLdNextEntry() {
			if ( cache != null ) {
				string tmp = cache;
				cache = null;
				return tmp;
			}

			bool in_string = false;
			string result = "";

			for ( ; !EOF ; ) {
				char c = (char)GetChar();

				switch ( c ) {
					case '"':
						in_string = !in_string;
						break;
					case ';': 
						char nc = (char)reader.Peek();
						if ( (c == '\r' && nc == '\n') || (c == '\n' && nc == '\r') ) GetChar();

						if ( !in_string )  return result;

						goto default;
					case '\n': goto case ';';
					case '\r': goto case ';';
					default:
						result += c;
						break;
				}
			}

			return result;
		}

		#region Buffered implementation
		private int GetChar() {
			if ( reader == null ) throw new IOException();
			return reader.Read();

			//int c = PeekChar();
			//++bufpos;
			//return c;
		}

		private int PeekChar() {
			if ( reader == null ) throw new IOException();
			return reader.Peek();

			/*if ( bufpos >= buflen ) {
				buflen = reader.ReadBlock( buffer, 0, 1024 );
				bufpos = 0;
				
				if ( buflen == 0 ) return -1;
			}

			return buffer[bufpos];
			*/
		}
		#endregion
	}
}
