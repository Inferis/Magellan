using System;
using System.IO;
using System.Drawing;
using EU2.Data;
using System.Globalization;

namespace EU2.IO
{
	/// <summary>
	/// Summary description for CSVWriter.
	/// </summary>
	public class CSVWriter : IDisposable
	{
		private StreamWriter writer;
		private bool needDelim;
		private CultureInfo ci;

		public CSVWriter( string path ) {
			writer = new StreamWriter( path, false, System.Text.Encoding.Default, 1024 );
			needDelim = false;
			ci = new CultureInfo( "en-US", false ); 
		}

		public CSVWriter( System.IO.Stream stream ) {
			writer = new StreamWriter( stream, System.Text.Encoding.Default, 1024 );
			needDelim = false;
			ci = new CultureInfo( "en-US", false ); 
		}

		public void Flush() {
			if ( writer != null ) writer.Flush();
		}

		public void Close() {
			if ( writer != null ) {
				writer.Close();
			}
			writer = null;
		}

		public Stream BaseStream {
			get { return writer.BaseStream; }
		}

		public void Write() {
			WriteEntry( "" );
		}

		public void Write( string value ) {
			WriteEntry( value );
		}

		public void Write( byte value ) {
			WriteEntry( value.ToString( ci ) );
		}

		public void Write( short value ) {
			WriteEntry( value.ToString( ci ) );
		}

		public void Write( int value ) {
			WriteEntry( value.ToString( ci ) );
		}

		public void Write( float value ) {
			WriteEntry( value.ToString( ci ) );
		}

		public void Write( double value ) {
			WriteEntry( value.ToString( ci ) );
		}

		public void Write( bool value ) {
			WriteEntry( value ? "1" : "0" );
		}

		public void Write( Point value ) {
			Write( value.X );
			Write( value.Y );
		}

		public void Write( Size value ) {
			Write( value.Width );
			Write( value.Height );
		}

		public void Write( Rectangle value ) {
			Write( value.Location );
			Write( value.Size );
		}

		public void EndRow( string contents ) {
			if ( writer == null ) throw new IOException();

			writer.WriteLine( contents );
			needDelim = false;
		}

		public void EndRow() {
			if ( writer == null ) throw new IOException();

			writer.WriteLine();
			needDelim = false;
		}

		protected void WriteDelim() {
			if ( writer == null ) throw new IOException();
			if ( needDelim ) writer.Write( ';' );
			needDelim = false;
		}
			
		protected void WriteEntry( string entry ) {
			if ( writer == null ) throw new IOException();

			WriteDelim();
			if ( entry.Length >= 0 ) {
				if ( entry.IndexOfAny( new char[] { ';', '\n', '\r' } ) >= 0 ) {
					if ( entry.IndexOf( '"' ) >= 0 ) 
						entry = "\"" + entry + "\"";
					else
						entry = "'" + entry + "'";
				}
				writer.Write(entry);
			}
			needDelim = true;
		}

        #region IDisposable Members

        public void Dispose() {
            Close();
        }

        #endregion
    }
}
