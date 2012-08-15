using System;
using System.IO;
using System.Net;
using System.Collections;

namespace PSD
{
	/// <summary>
	/// Summary description for Resources.
	/// </summary>
	public class Resources
	{
		public Resources() {
			items = new Resource[0];
		}

		public Resources( BinaryReader reader ) : this() {
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			int datalen = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			if ( datalen  == 0 ) {
				items = new Resource[0];
				return;
			}

			datalen += (int)reader.BaseStream.Position;
			ArrayList tmpitems = new ArrayList( 16 );
			while ( reader.BaseStream.Position < datalen ) {
				tmpitems.Add( new Resource( reader ) );
			}
			items = new Resource[tmpitems.Count];
			tmpitems.CopyTo( 0, items, 0, items.Length );
		}

		public void WriteTo( BinaryWriter writer ) {
			writer.Write( IPAddress.HostToNetworkOrder( (int)0 ) );
		}

		public IEnumerator GetEnumerator() {
			return items.GetEnumerator();
		}

		public Resource this[int index] {
			get { return items[index]; }
			set { items[index] = value; }
		}

		private Resource[] items;
	}
}
