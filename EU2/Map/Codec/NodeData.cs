using System;
using System.Runtime.InteropServices;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for NodeData.
	/// </summary>
	public struct NodeData
	{
		public NodeData( OwnerInfo owner, byte color ) {
			this.owner = owner;
			this.color = color;
		}

		public NodeData( Pixel pixel ) {
			this.owner = new OwnerInfo( pixel.id );
			this.color = pixel.color;
		}

		public OwnerInfo Owner {
			get { return owner; }
			set { owner = value; }
		}

		public byte Color {
			get { return color; }
			set { color = value; }
		}

		private OwnerInfo owner;
		private byte color;
	}
}
