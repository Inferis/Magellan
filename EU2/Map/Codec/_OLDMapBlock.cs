using System;
using System.Collections;
using EU2.Data;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for MapBlock.
	/// </summary>
	public class MapBlock : GenericMapBlock {
		#region Public Enumerations
		public enum WalkMode {
			Full,
			LeftOnly,
			TopLeftOnly,
			TopOnly
		}
		#endregion

		public MapBlock( ) {
		}

		public MapBlock( CompressedBlock compressed, AdjacencyTable adjacent ) {
			//owners = null;
			//tree = null;
			//leafs = null;
			Decompress( compressed, adjacent );
		}

		public void Decompress( CompressedBlock compressed, AdjacencyTable adjacent ) {
			int index = 0;

			// Decompress the ID table from the data stream
			DecompressIdTable( compressed.Data, ref index, adjacent );

			// Decompress the tree from the data stream
			int leafcount = DecompressTree( compressed.Data, ref index );
			// Create the leaf array
			this.leafs = new NodeData[leafcount + (leafcount % 8 == 0 ? 0 : 8-(leafcount % 8))];

			// Decompress the ownership table next
			DecompressOwnership( compressed.Data, ref index );

			// Finally, read the color data, which contains the "color" data for each leaf.
			DecompressColorInfo( compressed.Data, ref index );
		}

		public CompressedBlock Compress() {
			return null;
		}

		public override bool IsCompressed() { 
			return false; 
		}


		#region Decompression Helpers

		/// <summary>
		/// Decompresses the idtable stored in the byte stream. 
		/// </summary>
		/// <param name="data">The byte stream containing the compressed table. </param>
		/// <param name="index"></param>
		private void DecompressIdTable( byte[] data, ref int index, AdjacencyTable adjacent ) {
			const int IdTableSize = 256;
			const int Terminator = 128;

			// -- Get a list of province ids
			int idCount = -1;
			ushort[] idTable = new ushort[IdTableSize];
			do {
				++idCount;
				idTable[idCount] = (ushort)(data[index] + ((data[index+1]&127) << 8));
				index += 2;
			} while ( data[index-1] < Terminator );
			++idCount;

			// -- Fill the ownertable
			owners = new OwnerInfo[idCount];
			for ( int i=0; i<idCount; ++i ) {
				if ( idTable[i] < Province.Count ) {
					owners[i] = new OwnerInfo( idTable[i] );
				}
				else {
					// We're dealing with a pixel that's on a border. Decode some more...
					int val = idTable[i];
					int color = val&1;
					ushort id1 = (ushort)idTable[((val>>9)&63)-4];
					if ( id1 >= Province.Count ) id1 = Province.TerraIncognitaID;
					ushort id2 = adjacent == null ? Province.TerraIncognitaID : adjacent.GetAdjacency( id1, (val>>1)&15 ).ID;

					owners[i] = new OwnerInfo( id1, id2, (byte)((val>>5)&15) );
				}
			}
		}

		private int DecompressTree( byte[] data, ref int index ) {
			// Copy the tree into a bit array
			// We know there can be at most 43 bytes of data (1024 pixels or 668 nodes), so we copy those
			// first and resize it to the proper size later.
			int treesize = index+45 < data.Length ? index+45 : data.Length-index;
			byte[] treedata = new byte[treesize];
			Array.Copy( data, index, treedata, 0, treesize );
			tree = new BitArray( treedata );
			treedata = null;

			int nodeIndex = 0;
			int leafcount = WalkTreeSimple( 5, tree, ref nodeIndex );

			tree.Length = nodeIndex; // Set actual tree length
			index += (int)((nodeIndex+7) / 8); // Set proper data index

			return leafcount;
		}

		private void DecompressOwnership( byte[] data, ref int index ) {
			// Read the ownership table
			if ( owners.Length == 1 ) {
				for ( int i=0; i<leafs.Length; ++i ) leafs[i].OwnerIndex = 0;
			}
			else if ( owners.Length  == 2 ) {
				for ( int i=0; i<leafs.Length; ++index ) {
					leafs[i++].OwnerIndex = (byte)((data[index] >> 0) & 1);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 1) & 1);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 2) & 1);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 3) & 1);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 4) & 1);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 5) & 1);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 6) & 1);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 7) & 1);
				}
			}
			else if ( owners.Length  <= 4 ) {
				for ( int i=0; i<leafs.Length; ++index ) {
					leafs[i++].OwnerIndex = (byte)((data[index] >> 0) & 3);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 2) & 3);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 4) & 3);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 6) & 3);
				}
			}
			else if ( owners.Length  <= 16 ) {
				for ( int i=0; i<leafs.Length; index++ ) {
					leafs[i++].OwnerIndex = (byte)((data[index] >> 0) & 15);
					leafs[i++].OwnerIndex = (byte)((data[index] >> 4) & 15);
				}
			}
			else if ( owners.Length  > 16 ) {
				for ( int i=0; i<leafs.Length; i++, ++index ) 
					leafs[i].OwnerIndex = data[index];
			}
		}

		private void DecompressColorInfo( byte[] data, ref int index ) {
			for ( int i=0; i<leafs.Length; ) {
				leafs[i++].Color = (byte)(data[index] & 63);
				leafs[i++].Color = (byte)((data[index] >> 6 | data[index+1] << 2) & 63);
				leafs[i++].Color = (byte)((data[index+1] >> 4 | data[index+2] << 4) & 63);
				leafs[i++].Color = (byte)(data[index+2] >> 2);
				index += 3;
			}
		}
		#endregion

		/*
		#region Old Leaf Walking stuff
		public delegate void Walker( NodeData node, int x, int y );

		public void WalkTree( Walker walker, int x, int y ) {
			WalkTree( WalkMode.Full, root, walker, x, y );
		}
	
		public  void WalkTree( WalkMode mode, Walker walker, int x, int y ) {
			int nodeIndex = 0;
			int leafIndex = 0;

			switch ( mode ) {
				case WalkMode.Full: 
					WalkTreeFull( 5, ref nodeIndex, ref leafIndex, walker, x, y ); break;
				case WalkMode.LeftOnly: 
					WalkTreeLeft( 5, ref nodeIndex, ref leafIndex, walker, x, y ); break;
				case WalkMode.TopLeftOnly: 
					WalkTreeTopLeft( 5, ref nodeIndex, ref leafIndex, walker, x, y ); break;
				case WalkMode.TopOnly: 
					WalkTreeTop( 5, ref nodeIndex, ref leafIndex, walker, x, y ); break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected void WalkTreeFull( int level, ref int nodeIndex, ref int leafIndex, Walker walker, int x, int y ) {
			int size = ((1 << level) >> 1);
			bool flag = tree[nodeIndex++];

			if ( flag && level >= 1 ) {
				--level;
				WalkTreeFull( level, ref nodeIndex, ref leafIndex, walker, x+size, y+size ); // bottomright
				WalkTreeFull( level, ref nodeIndex, ref leafIndex, walker, x, y+size ); // bottomleft
				WalkTreeFull( level, ref nodeIndex, ref leafIndex, walker, x+size, y ); // topright
				WalkTreeFull( level, ref nodeIndex, ref leafIndex, walker, x, y ); // topleft
			}
			else { 
				if ( flag ) {
					walker( leafs[leafIndex++], x+1, y+1 );
					walker( leafs[leafIndex++], x, y+1 );
					walker( leafs[leafIndex++], x+1, y );
				}
				walker( leafs[leafIndex++], x, y );
			}
		}
	
		protected void WalkTreeLeft( int level, ref int nodeIndex, ref int leafIndex,  Walker walker, int x, int y ) {
			int size = ((1 << level) >> 1);
			bool flag = tree[nodeIndex++];

			if ( flag && level >= 1 ) {
				--level;
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // bottomright
				WalkTreeLeft( level, ref nodeIndex, ref leafIndex, walker, x, y+size ); // bottomleft
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // topright
				WalkTreeLeft( level, ref nodeIndex, ref leafIndex, walker, x, y ); // topleft
			}
			else { 
				if ( flag ) {
					leafIndex++;
					walker( leafs[leafIndex++], x, y+1 );
					leafIndex++;
				}
				walker( leafs[leafIndex++], x, y );
			}
		}
	
		protected void WalkTreeTop( int level, ref int nodeIndex, ref int leafIndex, Walker walker, int x, int y ) {
			int size = ((1 << level) >> 1);
			bool flag = tree[nodeIndex++];

			if ( flag && level >= 1 ) {
				--level;
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // bottomright
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // bottomleft
				WalkTreeFull( level, ref nodeIndex, ref leafIndex, walker, x+size, y ); // topright
				WalkTreeFull( level, ref nodeIndex, ref leafIndex, walker, x, y ); // topleft
			}
			else { 
				if ( flag ) {
					leafIndex += 2;
					walker( leafs[leafIndex++], x+1, y );
				}
				walker( leafs[leafIndex++], x, y );
			}
		}
	
		protected void WalkTreeTopLeft( int level, ref int nodeIndex, ref int leafIndex, Walker walker, int x, int y ) {
			int size = ((1 << level) >> 1);
			bool flag = tree[nodeIndex++];

			if ( flag && level >= 1 ) {
				--level;
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // bottomright
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // bottomleft
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // topright
				WalkTreeTopLeft( level, ref nodeIndex, ref leafIndex, walker, x, y ); // topleft
			}
			else { 
				if ( flag ) leafIndex += 3;
				walker( leafs[leafIndex++], x, y );
			}
		}
	
		protected void SkipBranch( int level, ref int nodeIndex, ref int leafIndex ) {
			bool flag = tree[nodeIndex++];
			if ( flag && level >= 1 ) {
				--level;
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // bottomright
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // bottomleft
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // topright
				SkipBranch( level, ref nodeIndex, ref leafIndex ); // topleft
			}
			else {
				leafIndex += flag ? 4 : 1;
			}
		}
	
		#endregion
		*/

		private int WalkTreeSimple( int level, BitArray tree, ref int nodeIndex ) {
			// Get flag, and advance node pointer
			bool flag = tree[nodeIndex++];
			int leafcount = 0;

			if ( tree[nodeIndex] && level > 1 ) {
				leafcount += WalkTreeSimple( level - 1, tree, ref nodeIndex );
				leafcount += WalkTreeSimple( level - 1, tree, ref nodeIndex );
				leafcount += WalkTreeSimple( level - 1, tree, ref nodeIndex );
				leafcount += WalkTreeSimple( level - 1, tree, ref nodeIndex );
			}
			else {
				leafcount = flag ? 4 : 1;
			}

			return leafcount;
		}

		private OwnerInfo[] owners;
		private System.Collections.BitArray tree;
		private NodeData[] leafs;
	}
}
