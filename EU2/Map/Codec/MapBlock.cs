using System;
using System.Text;
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

		private const int IdTableSize = 256;
		private const int Terminator = 128;

		public MapBlock( ) {
			Erase();
		}

		public MapBlock( Node root ) {
			tree = root;
		}

		public MapBlock( CompressedBlock compressed, AdjacencyTable adjacent ) {
			Decompress( compressed, adjacent );
		}

		public static int CalculateCompressedSize( byte[] data ) {
			try {
				int index = 0;

				// Decompress the ID table from the data stream
				Pixel[] owners;
				int idcount = DecompressIdTable( data, ref index, null, out owners );

				// Decompress the tree from the data stream
				Node tree = DecompressTree( data, ref index );
				double leafcount = tree.CalcLeafCount();
				int ownercount = (int)leafcount;

				if ( idcount <= 1 ) 
					ownercount = 0;
				else if ( idcount == 2 )
					ownercount = (int)Math.Ceiling( leafcount / 8 );
				else if ( idcount <= 4 )
					ownercount = (int)Math.Ceiling( leafcount / 4 );
				else if ( idcount <= 16 ) 
					ownercount = (int)Math.Ceiling( leafcount / 2 );

				if ( leafcount % 4 != 0 ) leafcount += 4 - (leafcount % 4);
				leafcount = Math.Ceiling(leafcount * 3 / 4);
				int result = index + ownercount + (int)leafcount;
				//if ( result % 4 != 0 ) result += 4 - (result % 4);

				return result;
			}
			catch {
				return -1;
			}
		}

		public void Decompress( CompressedBlock compressed, AdjacencyTable adjacent ) {
			int index = 0;

			// Decompress the ID table from the data stream
			Pixel[] owners;
			int ownercount = DecompressIdTable( compressed.Data, ref index, adjacent, out owners );

			// Decompress the tree from the data stream
			tree = DecompressTree( compressed.Data, ref index );
			int leafcount = tree.CalcLeafCount();
			// Create the leaf array
			Pixel[] leafs = new Pixel[leafcount + (leafcount % 8 == 0 ? 0 : 8-(leafcount % 8))];

			// Decompress the ownership table next
			DecompressOwnership( compressed.Data, ref index, leafs, leafcount, owners, ownercount );

			// Finally, read the color data, which contains the "color" data for each leaf.
			DecompressColorInfo( compressed.Data, ref index, leafs, leafcount );

			// Assign the leaf data to the nodes
			int leafIndex = 0;
			PopulateNode( tree, leafs, ref leafIndex );
		}

		public CompressedBlock Compress( int xreal, int yreal, int zoom, ProvinceList provinces, AdjacencyTable adjacent, IDMap idmap ) {
			// Walk the tree, storing the info along the way
			MapBlockHandling.Compressor compressor;
			if ( provinces == null || adjacent == null || idmap == null )
				compressor = new MapBlockHandling.LesserCompressor();
			else
				compressor = new MapBlockHandling.DefaultCompressor( xreal, yreal, zoom, provinces, adjacent, idmap );
			WalkTree( compressor );

			// Write away idtable
			int datasize = compressor.IDCount*2 + compressor.Tree.Length + (compressor.LeafCount+3)/4*3;
			if ( compressor.IDCount == 2 ) 
				datasize += (compressor.LeafCount+7)/8;
			else if ( compressor.IDCount == 3 || compressor.IDCount == 4 )  
				datasize += (compressor.LeafCount+3)/4;
			else if ( compressor.IDCount > 4 && compressor.IDCount <= 16 )  
				datasize += (compressor.LeafCount+1)/2;
			else if ( compressor.IDCount > 16 )  
				datasize += compressor.LeafCount;

			byte[] data = new byte[datasize];
			int dataindex = 0;

			// Copy IDTable
			/*
			for ( int i=0; i<compressor.IDCount; ++i ) {
				data[dataindex++] = (byte)(compressor.IDTable[i] & 255);
				data[dataindex++] = (byte)(compressor.IDTable[i] >> 8);
			}
			*/
			byte[] idtable = compressor.IDTable;
			for ( int i=0; i<compressor.IDCount; ++i ) {
				for ( int t=0; t<ushort.MaxValue; ++t ) {
					if ( idtable[t] == i ) {
						data[dataindex++] = (byte)(t & 255);
						data[dataindex++] = (byte)(t >> 8);
						break;
					}
				}
			}
			data[dataindex-1] |= Terminator;

			byte[] tree = compressor.Tree;
			for ( int i=0; i<tree.Length; ++i ) {
				data[dataindex++] = tree[i];
			}

			if ( compressor.IDCount == 2 ) {
				for ( int i=0; i<compressor.LeafCount; i+=8 ) {
					data[dataindex++] = (byte)(
						(compressor.Owners[i]) | 
						(compressor.Owners[i+1] << 1) |
						(compressor.Owners[i+2] << 2) |
						(compressor.Owners[i+3] << 3) |
						(compressor.Owners[i+4] << 4) |
						(compressor.Owners[i+5] << 5) |
						(compressor.Owners[i+6] << 6) |
						(compressor.Owners[i+7] << 7) );
				}
			}
			else if ( compressor.IDCount == 3 || compressor.IDCount == 4 ) {
				for ( int i=0; i<compressor.LeafCount; i+=4 ) {
					data[dataindex++] = (byte)(
						(compressor.Owners[i]) | 
						(compressor.Owners[i+1] << 2) |
						(compressor.Owners[i+2] << 4) |
						(compressor.Owners[i+3] << 6) );
				}
			}
			else if ( compressor.IDCount > 4 && compressor.IDCount <= 16 ) {
				for ( int i=0; i<compressor.LeafCount; i+=2 ) {
					data[dataindex++] = (byte)(
						(compressor.Owners[i]) | 
						(compressor.Owners[i+1] << 4) );
				}
			}
			else if ( compressor.IDCount > 16 ) {
				for ( int i=0; i<compressor.LeafCount; ++i ) {
					data[dataindex++] = (byte)(compressor.Owners[i]);
				}
			}

			// Finally, the 6-bit color values
			for ( int i=0; i<compressor.LeafCount; i+=4) {
				data[dataindex++] = (byte)((compressor.Colors[i] & 63) | ((compressor.Colors[i+1] & 63) << 6));
				data[dataindex++] = (byte)(((compressor.Colors[i+1] & 63) >> 2) | ((compressor.Colors[i+2] & 63) << 4));
				data[dataindex++] = (byte)(((compressor.Colors[i+2] & 63) >> 4) | ((compressor.Colors[i+3] & 63) << 2));
			}

			return new CompressedBlock( data );
		}

		public override bool IsCompressed() { 
			return false; 
		}

		public Node Tree { 
			get { return tree; } 
		}

		public void Erase( ) {
			tree = new Node( false );
			tree.BottomRightChild = new Node( Pixel.Empty );
			tree.BottomLeftChild = new Node( Pixel.Empty );
			tree.TopRightChild = new Node( Pixel.Empty );
			tree.TopLeftChild = new Node( Pixel.Empty );
		}

		public static ushort[] GetRawIDTable( CompressedBlock compressed ) {
			byte[] data = compressed.Data;			

			// -- Get a list of province ids
			int idCount = -1;
			int index = 0;
			ushort[] idTable = new ushort[IdTableSize];
			do {
				++idCount;
				idTable[idCount] = (ushort)(data[index] + ((data[index+1] & 127) << 8));
				index += 2;
			} while ( data[index-1] < Terminator );
			++idCount;

			ushort[] result = new ushort[idCount];
			Array.Copy( idTable, 0, result, 0, idCount );
			return result;
		}


		public virtual bool Equals( MapBlock other ) {
			return this.ToString( true ) == other.ToString( true );
		}

		public virtual string ToString( bool deep ) {
			if ( !deep ) return ToString();

			return WalkTree( new MapBlockHandling.Stringizer() ).ToString();
		}


		#region Leaf Walking stuff
		public ITreeWalker WalkTree( ITreeWalker walker ) {
			walker.WalkTree( this );
			return walker;
		}

		public ITreeWalker WalkTree( ITreeWalker walker, TreeWalkerMode mode ) {
			walker.WalkTree( this, mode );
			return walker;
		}

		public ITreeWalker WalkTree( DelegateWalker.VisitDelegate visitor ) {
			DelegateWalker walker = new DelegateWalker( visitor );
			walker.WalkTree( this );
			return walker;
		}

		public ITreeWalker WalkTree( DelegateWalker.VisitDelegate visitor, int stopAtLevel ) {
			DelegateWalker walker = new DelegateWalker( visitor );
			walker.StopAtLevel = stopAtLevel;
			walker.WalkTree( this );
			return walker;
		}

		public ITreeWalker WalkTree( DelegateWalker.VisitDelegate visitor, TreeWalkerMode mode ) {
			DelegateWalker walker = new DelegateWalker( visitor );
			walker.WalkTree( this, mode );
			return walker;
		}

		public ITreeWalker WalkTree( DelegateWalker.VisitDelegate visitor, bool visitBranches, TreeWalkerMode mode ) {
			DelegateWalker walker = new DelegateWalker( visitor, visitBranches, mode );
			walker.WalkTree( this );
			return walker;
		}

		public ITreeWalker WalkTree( DelegateWalker.VisitDelegate visitor, bool visitBranches ) {
			DelegateWalker walker = new DelegateWalker( visitor, visitBranches );
			walker.WalkTree( this );
			return walker;
		}

		#endregion

		#region Decompression

		/// <summary>
		/// Decompresses the idtable stored in the byte stream. 
		/// </summary>
		/// <param name="data">The byte stream containing the compressed table. </param>
		/// <param name="index"></param>
		private static int DecompressIdTable( byte[] data, ref int index, AdjacencyTable adjacent, out Pixel[] owners ) {

			// -- Get a list of province ids
			int idCount = -1;
			ushort[] idTable = new ushort[IdTableSize];
			do {
				++idCount;
				idTable[idCount] = (ushort)(data[index] + ((data[index+1] & 127) << 8));
				index += 2;
			} while ( data[index-1] < Terminator );
			++idCount;

			// -- Fill the ownertable
			owners = new Pixel[IdTableSize];
			for ( int i=0; i<idCount; ++i ) {
				if ( idTable[i] <= Province.Count ) {
					owners[i] = new Pixel( 0, idTable[i] );
				}
				else {
					// We're dealing with a pixel that's on a border. Decode some more...
					int val = idTable[i];
					int color = val&1;
					ushort id1 = (ushort)idTable[((val>>9)&63)-4];
					if ( id1 >= Province.Count ) id1 = Province.TerraIncognitaID;
					//ushort id2 = adjacent == null ? Province.TerraIncognitaID : adjacent.GetAdjacency( id1, (val>>1)&15 ).ID;
					ushort id2 = Province.TerraIncognitaID;
					byte riveradj = (byte)((val>>5)&15);
					if ( riveradj != Adjacent.Invalid && adjacent != null ) {
						id2 = id1;
						id1 = adjacent.GetAdjacency( id2, riveradj ).ID;
					}

					owners[i] = new Pixel( 0, id1, id2, (byte)(color+1) );
				}
			}

			return idCount;
		}

		private static Node DecompressTree( byte[] data, ref int index ) {
			// Copy the tree into a bit array
			// We know there can be at most 43 bytes of data (1024 pixels or 668 nodes), so we copy those
			// first and resize it to the proper size later.
			int srcsize = index+45 < data.Length ? 45 : data.Length-index;
			byte[] srcdata = new byte[srcsize];
			Array.Copy( data, index, srcdata, 0, srcsize );
			BitArray srctree = new BitArray( srcdata );
			srcdata = null;

			int nodeIndex = 0;
			Node tree = new Node(true);
			ConvertBitTree( 5, srctree, ref nodeIndex, tree );
			index += (int)((nodeIndex+7) / 8); // Set proper data index
			//index += (int)(nodeIndex / 8); // Set proper data index

			return tree;
		}

		private void DecompressOwnership( byte[] data, ref int index, Pixel[] leafs, int leafcount, Pixel[] owners, int ownercount ) {
			// Read the ownership table
			if ( ownercount == 1 ) {
				for ( int i=0; i<leafcount; ++i ) leafs[i] = owners[0];
				return;
			}
			
			if ( ownercount == 2 ) {
				for ( int i=0; i<leafcount; ++index ) {
					leafs[i++] = owners[(data[index] >> 0) & 1];
					leafs[i++] = owners[(data[index] >> 1) & 1];
					leafs[i++] = owners[(data[index] >> 2) & 1];
					leafs[i++] = owners[(data[index] >> 3) & 1];
					leafs[i++] = owners[(data[index] >> 4) & 1];
					leafs[i++] = owners[(data[index] >> 5) & 1];
					leafs[i++] = owners[(data[index] >> 6) & 1];
					leafs[i++] = owners[(data[index] >> 7) & 1];
				}
				return;
			}

			if ( ownercount <= 4 ) {
				for ( int i=0; i<leafcount; ++index ) {
					leafs[i++] = owners[(data[index] >> 0) & 3];
					leafs[i++] = owners[(data[index] >> 2) & 3];
					leafs[i++] = owners[(data[index] >> 4) & 3];
					leafs[i++] = owners[(data[index] >> 6) & 3];
				}
				return;
			}

			if ( ownercount <= 16 ) {
				for ( int i=0; i<leafcount; index++ ) {
					leafs[i++] = owners[(data[index] >> 0) & 15];
					leafs[i++] = owners[(data[index] >> 4) & 15];
				}
				return;
			}

			for ( int i=0; i<leafcount; i++, ++index ) 
				leafs[i] = owners[data[index]];
		}

		private void DecompressColorInfo( byte[] data, ref int index, Pixel[] leafs, int leafcount ) {
			for ( int i=0; i<leafcount; ) {
				leafs[i++].Color = (byte)(data[index] & 63);
				if ( i>=leafcount ) break;
				leafs[i++].Color = (byte)((data[index] >> 6 | data[index+1] << 2) & 63);
				if ( i>=leafcount ) break;
				leafs[i++].Color = (byte)((data[index+1] >> 4 | data[index+2] << 4) & 63);
				if ( i>=leafcount ) break;
				leafs[i++].Color = (byte)(data[index+2] >> 2);
				index += 3;
			}
		}

		private static void ConvertBitTree( int level, BitArray srctree, ref int nodeIndex, Node parent ) {
			// Get flag, and advance node pointer
			bool flag = srctree[nodeIndex++];

			if ( flag && level > 1 ) {
				parent.CreateBranches();

				ConvertBitTree( level - 1, srctree, ref nodeIndex, parent.BottomRightChild );
				ConvertBitTree( level - 1, srctree, ref nodeIndex, parent.BottomLeftChild );
				ConvertBitTree( level - 1, srctree, ref nodeIndex, parent.TopRightChild );
				ConvertBitTree( level - 1, srctree, ref nodeIndex, parent.TopLeftChild );
			}
			else {
				if ( flag ) {
					parent.CreateLeaves();
				}
				else {
					parent.BecomeLeaf();
				}
			}
		}

		private void PopulateNode( Node node, Pixel[] leafs, ref int index ) {
			if ( !node.IsLeaf() ) {
				PopulateNode( node.BottomRightChild, leafs, ref index );
				PopulateNode( node.BottomLeftChild, leafs, ref index );
				PopulateNode( node.TopRightChild, leafs, ref index );
				PopulateNode( node.TopLeftChild, leafs, ref index );
			}
			else {
				node.Data = leafs[index++];
			}
		}
		#endregion

		public static MapBlock Combine( MapBlock bottomright, MapBlock bottomleft, MapBlock topright, MapBlock topleft ) {
			// Merge 4 trees
			Node root = new Node( false, bottomright.Tree.Level+1 );
			root.BottomRightChild = bottomright.Tree.Clone();
			root.BottomLeftChild = bottomleft.Tree.Clone();
			root.TopRightChild = topright.Tree.Clone();
			root.TopLeftChild = topleft.Tree.Clone();
			
			// Use for new block
			MapBlock result = new MapBlock( root );

			// Clip the tree
			result.WalkTree( new MapBlockHandling.TreeClipper() ); 
			result.tree.Level = 5;

			// Optimise the tee
			result.WalkTree( new MapBlockHandling.TreeOptimiser( true ) );

			return result;
		}

		public static MapBlock Combine( MapBlock[,] matrix ) {
			Node root = new Node( false );

			// Create 4 children
			root.BottomRightChild = new Node( false );
			root.BottomLeftChild = new Node( false );
			root.TopRightChild = new Node( false );
			root.TopLeftChild  = new Node( false );

			// Add items to children
			root.BottomRightChild.BottomRightChild = matrix[3,3].Tree.Clone();
			root.BottomRightChild.BottomLeftChild = matrix[2,3].Tree.Clone();
			root.BottomRightChild.TopRightChild = matrix[3,2].Tree.Clone();
			root.BottomRightChild.TopLeftChild = matrix[2,2].Tree.Clone();

			root.BottomLeftChild.BottomRightChild = matrix[1,3].Tree.Clone();
			root.BottomLeftChild.BottomLeftChild = matrix[0,3].Tree.Clone();
			root.BottomLeftChild.TopRightChild = matrix[1,2].Tree.Clone();
			root.BottomLeftChild.TopLeftChild = matrix[0,2].Tree.Clone();

			root.TopRightChild.BottomRightChild = matrix[3,1].Tree.Clone();
			root.TopRightChild.BottomLeftChild = matrix[2,1].Tree.Clone();
			root.TopRightChild.TopRightChild = matrix[3,0].Tree.Clone();
			root.TopRightChild.TopLeftChild = matrix[2,0].Tree.Clone();

			root.TopLeftChild.BottomRightChild = matrix[1,1].Tree.Clone();
			root.TopLeftChild.BottomLeftChild = matrix[0,1].Tree.Clone();
			root.TopLeftChild.TopRightChild = matrix[1,0].Tree.Clone();
			root.TopLeftChild.TopLeftChild = matrix[0,0].Tree.Clone();

			root.Level = matrix[3,3].Tree.Level+2;

			// Use for new block
			MapBlock result = new MapBlock( root );

			// Clip the tree
			result.WalkTree( new MapBlockHandling.TreeClipperDeep( ) ); 
			result.tree.Level = 5;

			// Optimise the tee
			result.WalkTree( new MapBlockHandling.TreeOptimiser( true ) );

			return result;
		}

		#region Private Fields
		private Node tree;
		#endregion
	}
}
