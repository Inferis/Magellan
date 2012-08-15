using System;
using System.Collections;

namespace EU2.Map.Codec.MapBlockHandling
{
	/// <summary>
	/// Summary description for Compressor.
	/// </summary>
	public abstract class Compressor : BreadthFirstTreeWalker {
		protected Compressor() : base( true ) {
		}

		protected override void OnBeforeWalk() {
			colors = new byte[1024];
			owners = new byte[1024];
			idtable = new byte[ushort.MaxValue];
			tree = new BitArray( 8192 );
			treeindex = 0;
			leafindex = 0;
			idindex = 0;

			for ( int i=0; i<ushort.MaxValue; ++i ) idtable[i] = byte.MaxValue;
		}

		protected override void OnAfterWalk() {
			tree.Length = treeindex;
		}

		public int IDCount { get { return idindex; } }
		public byte[] IDTable { get { return idtable; } }

		public int LeafCount { get { return leafindex; } }
		public byte[] Owners { get { return owners; } }
		public byte[] Colors { get { return colors; } }
			
		public byte[] Tree {
			get { 
				byte[] result = new byte[(treeindex+7)/8];
				tree.CopyTo( result, 0 );
				return result;
			}
		}
			
		protected byte ConvertID( ushort id ) {
			if ( idtable[id] == byte.MaxValue ) {
				idtable[id] = (byte)idindex;
				idindex++;
			}

			return idtable[id];
		}

		#region Protected Fields
		protected BitArray tree;
		protected byte[] idtable;
		protected byte[] owners;
		protected byte[] colors;
		protected int leafindex;
		protected int idindex;
		protected int treeindex;
		#endregion
	}
}
