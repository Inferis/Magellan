using System;
using System.Drawing;
using System.Net;
using System.IO;

namespace PSD
{
	/// <summary>
	/// Summary description for Layer.
	/// </summary>
	public class Resource
	{
		public Resource() {
			id = 0;
			name = "";
			unhandled = null;
		}

		public Resource( BinaryReader reader ) : this() {
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			string signature = new string( reader.ReadChars( 4 ) );
			if ( signature != "8BIM" && signature != "MeSa" ) throw new InvalidResourceSignatureException( signature );

			id = IPAddress.NetworkToHostOrder( reader.ReadInt16() );

			name = Utils.ReadPascalString( reader, 2 );

			ProcessData( reader, IPAddress.NetworkToHostOrder( reader.ReadInt32() ) ); 
		}

		public void WriteTo( BinaryWriter writer ) {
		}

		private void ProcessData( BinaryReader reader, int size ) {
			if ( id == 0x03ee ) { // Channel names
				unhandled = reader.ReadBytes( size );
			}
			else {
				unhandled = reader.ReadBytes( size );
			}

			if ( (size & 1) != 0 ) 
				reader.ReadByte(); // Word-alignment on zero-length string
		}

		short id;
		string name;
		byte[] unhandled;
	}

	public class InvalidResourceSignatureException : Exception {
		public InvalidResourceSignatureException() : base( "The resource has an invalid signature." ) {}
		public InvalidResourceSignatureException( string signature ) : base( "The resource has an invalid signature \"" + signature + "\"." ) {}
	}

}
