using System;

namespace EU2.Data.Mapfiles
{
	public enum WalkMode {
		Full,
		LeftOnly,
		TopLeftOnly,
		TopOnly
	}

	/// <summary>
	/// Summary description for DecompressedBlock.
	/// </summary>
	public class DecompressedBlock
	{
		private ushort[] idTable;
		private int idCount;
		private Node root;

		private const int IDTABLE_SIZE = 256;

		public DecompressedBlock() {
			root = new Node( false );
			idTable = new ushort[IDTABLE_SIZE];
			idCount = 0;
		}

		public DecompressedBlock( CompressedBlock block ) {
			root = new Node( false );
			idTable = new ushort[IDTABLE_SIZE];
			idCount = 0;

			DecodeFrom( block );
		}

		public void DecodeFrom( CompressedBlock block ) {
			const int INVALID_ADJ = 15;
			const int TERMINATOR = 128;

			Install install = block.Parent.Install;

			byte[] data = block.Data;
			for ( int i=0; i<IDTABLE_SIZE; ++i ) idTable[i] = ushort.MaxValue;
			int index = 0;
			idCount = -1;

			// Get a list of province ids
			do {
				++idCount;
				idTable[idCount] = (ushort)(data[idCount<<1] + ((data[(idCount<<1)+1]&127) << 8));
				index += 2;
			} while ( data[(idCount<<1)+1] < TERMINATOR );
			++idCount;

			// Normalize the IDs... Basically, we're stripping some of that data away which we don't need (mainly to do with borders).
			for ( int i=0; i<idCount; ++i ) {
				if ( idTable[i] <= Province.MaxID ) continue;

				ushort id = idTable[((idTable[i]>>9) & 63)-4];
				if( id > Province.MaxID ) 
					id = install.Provinces.TerraIncognita.ID;
				else {
					int river = (idTable[i]>>5) & 15;
					if ( river != INVALID_ADJ ) id = install.Provinces[id].GetNeighbor( river ).ID;

					idTable[i] = id;
				}
			}

			// Build the tree.
			DecompressorState decompstate = new DecompressorState( index, ref data );
			BuildTree( root, decompstate, 5 );
			index = decompstate.GetFinalNodeIndex();
			// Define ownership and colors tables
			int numLeaves = decompstate.NumOfLeaves;
			int sizeLeaves = decompstate.SizeOfLeaves;
			byte[] colors = new byte[decompstate.SizeOfLeaves + (4-(decompstate.SizeOfLeaves % 4))]; 
			ushort[] ownership = null;
			decompstate = null; 

			if ( idCount == 1 ) {
				ownership = new ushort[numLeaves];
				for ( int i=0; i<numLeaves; ++i ) 
					ownership[i] = idTable[0];
			}
			else if ( idCount == 2 ) {
				ownership = new ushort[numLeaves + (8-(numLeaves % 8))];
				for ( int i=0; i<numLeaves; ++index ) {
					ownership[i++] = idTable[(data[index] >> 0) & 1];
					ownership[i++] = idTable[(data[index] >> 1) & 1];
					ownership[i++] = idTable[(data[index] >> 2) & 1];
					ownership[i++] = idTable[(data[index] >> 3) & 1];
					ownership[i++] = idTable[(data[index] >> 4) & 1];
					ownership[i++] = idTable[(data[index] >> 5) & 1];
					ownership[i++] = idTable[(data[index] >> 6) & 1];
					ownership[i++] = idTable[(data[index] >> 7) & 1];
				}
			}
			else if ( idCount <= 4 ) {
				ownership = new ushort[numLeaves + (4-(numLeaves % 4))];
				for ( int i=0; i<numLeaves; ++index ) {
					ownership[i++] = idTable[(data[index] >> 0) & 3];
					ownership[i++] = idTable[(data[index] >> 2) & 3];
					ownership[i++] = idTable[(data[index] >> 4) & 3];
					ownership[i++] = idTable[(data[index] >> 6) & 3];
				}
			}
			else if ( idCount <= 16 ) {
				ownership = new ushort[numLeaves + (2-(numLeaves % 2))];
				for ( int i=0; i<numLeaves; index++ ) {
					ownership[i++] = idTable[(data[index] >> 0) & 15];
					ownership[i++] = idTable[(data[index] >> 4) & 15];
				}
			}
			else if ( idCount > 16 ) {
				ownership = new ushort[numLeaves];
				for ( int i=0; i<ownership.Length; ++i, ++index ) 
					ownership[i] = idTable[data[index]];
			}

			// Read in color table
			// Finally, read the leaf data. This part contains the "color" data for each leaf.
			for ( int i=0; i<sizeLeaves; ) {
				colors[i++] = (byte)(data[index] & 63);
				colors[i++] = (byte)((data[index] >> 6 | data[index+1] << 2) & 63);
				colors[i++] = (byte)((data[index+1] >> 4 | data[index+2] << 4) & 63);
				colors[i++] = (byte)(data[index+2] >> 2);
				index += 3;
			}

			// Fill them into the tree
			PopulatorState popstate = new PopulatorState( ref ownership, ref colors );
			PopulateTree( root, popstate );
		}

		public void EncodeTo( CompressedBlock block ) {
		}

		public delegate void Walker( Node node, int x, int y );

		public void WalkTree( Walker walker, int x, int y ) {
			WalkTree( WalkMode.Full, root, walker, x, y );
		}
	
		public  void WalkTree( WalkMode mode, Walker walker, int x, int y ) {
			switch ( mode ) {
				case WalkMode.Full: 
					WalkTreeFull( root, walker, x, y ); break;
				case WalkMode.LeftOnly: 
					WalkTreeLeft( root, walker, x, y ); break;
				case WalkMode.TopLeftOnly: 
					WalkTreeTopLeft( root, walker, x, y ); break;
				case WalkMode.TopOnly: 
					WalkTreeTop( root, walker, x, y ); break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void WalkTreeFull( Node node, Walker walker, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);

			if ( node.Flag && node.Level >= 1 ) {
				WalkTreeFull( node.BottomRightChild, walker, x+size, y+size );
				WalkTreeFull( node.BottomLeftChild, walker, x, y+size );
				WalkTreeFull( node.TopRightChild, walker, x+size, y );
				WalkTreeFull( node.TopLeftChild, walker, x, y );
			}
			else { 
				walker( node, x, y );
			}
		}
	
		private void WalkTreeLeft( Node node, Walker walker, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);

			if ( node.Flag && node.Level >= 1 ) {
				WalkTreeLeft( node.BottomLeftChild, walker, x, y+size );
				WalkTreeLeft( node.TopLeftChild, walker, x, y );
			}
			else { 
				walker( node, x, y );
			}
		}
	
		private void WalkTreeTop( Node node, Walker walker, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);

			if ( node.Flag && node.Level >= 1 ) {
				WalkTreeTop( node.TopRightChild, walker, x+size, y );
				WalkTreeTop( node.TopLeftChild, walker, x, y );
			}
			else { 
				walker( node, x, y );
			}
		}
	
		private void WalkTreeTopLeft( Node node, Walker walker, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);

			if ( node.Flag && node.Level >= 1 ) {
				WalkTreeTopLeft( node.TopLeftChild, walker, x, y );
			}
			else { 
				walker( node, x, y );
			}
		}
	
		private void WalkTree( WalkMode mode, Node node, Walker walker, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);

			if ( node.Flag && node.Level >= 1 ) {
				WalkTree( mode, node.BottomRightChild, walker, x+size, y+size );
				WalkTree( mode, node.BottomLeftChild, walker, x, y+size );
				WalkTree( mode, node.TopRightChild, walker, x+size, y );
				WalkTree( mode, node.TopLeftChild, walker, x, y );
			}
			else { 
				walker( node, x, y );
			}
		}

		private void BuildTree( Node currentNode, DecompressorState state, int level ) {
			bool flag = state.Flag;
			state.NextNode();
	
			currentNode.Level = level;
			currentNode.Flag = flag;
			if ( flag && level > 1 ) {
				currentNode.CreateBranches( );

				BuildTree( currentNode.BottomRightChild, state, level-1 ); 
				BuildTree( currentNode.BottomLeftChild, state, level-1 ); 
				BuildTree( currentNode.TopRightChild, state, level-1 ); 
				BuildTree( currentNode.TopLeftChild, state, level-1 );
			}
			else {
				if ( flag ) {
					currentNode.CreateLeaves();
					state.AddLeaves( );
				}
				else {
					currentNode.BecomeLeaf();
					state.AddLeaf( );
				}
			}
		}

		private void PopulateTree( Node currentNode, PopulatorState state ) {
			if ( currentNode == null ) return;

			if ( !currentNode.IsLeaf() ) {
				// Walk over the tree in a breath-first manner
				PopulateTree( currentNode.BottomRightChild, state );
				PopulateTree( currentNode.BottomLeftChild, state );
				PopulateTree( currentNode.TopRightChild, state );
				PopulateTree( currentNode.TopLeftChild, state );
			}
			else {
				currentNode.Owner = state.CurrentOwner;
				currentNode.Color = state.CurrentColor;
				state.NextLeaf();
			}
		}

	}

	internal class PopulatorState {
		private int leafIndex;
		private ushort[] owners;
		private byte[] colors;

		public PopulatorState( ref ushort[] owners, ref byte[] colors ) {
			leafIndex = 0;
			this.owners = owners;
			this.colors = colors;
		}

		public void Reset() {
			leafIndex = 0;
		}

		public void NextLeaf() {
			leafIndex++;
		}

		public int LI {
			get { return leafIndex; }
		}

		public ushort CurrentOwner {
			get { return owners[leafIndex]; }
		}

		public byte CurrentColor {
			get { return colors[leafIndex]; }
		}
	}
		
	internal class DecompressorState {
		private int nodeIndex;
		private byte nodeMask;
		private int numOfLeaves;
		private int sizeOfLeaves;
		private byte[] data;

		public DecompressorState( int startIndex, ref byte[] data ) {
			nodeIndex = startIndex;
			nodeMask = 1;
			numOfLeaves = 0;
			sizeOfLeaves = 0;
			this.data = data;
		}

		public void NextNode() {
			nodeMask <<= 1;
			if ( nodeMask == 0 ) { nodeMask = 1; nodeIndex++; }
		}

		public bool Flag {
			get { return (data[nodeIndex] & nodeMask) > 0; }
		}

		public int GetFinalNodeIndex() {
			if ( nodeMask > 1 ) ++nodeIndex;
			nodeMask = 1;

			return nodeIndex;
		}

		public void AddLeaf() {
			++numOfLeaves;
			++sizeOfLeaves;
		}

		public void AddLeaves() {
			numOfLeaves += 4;
			sizeOfLeaves += 4;
		}

		public int NumOfLeaves {
			get { return numOfLeaves; }
		}

		public int SizeOfLeaves {
			get { return sizeOfLeaves; }
		}
	}


	public class Node {
		public enum NodeLocation {
			BottomRight = 0,
			BottomLeft = 1,
			TopRight = 2,
			TopLeft = 3,
			Unspecified = -1
		}

		private Node[] children;
		private ushort owner;
		private byte color;
		private int level;
		private bool flag;
		private NodeLocation location;

		public Node( bool isLeaf ) {
			location = NodeLocation.Unspecified;
			level = -1;
			flag = false;
			if ( isLeaf ) 
				BecomeLeaf();
			else
				BecomeBranch();
		}

		public Node( bool isLeaf, int level ) {
			location = NodeLocation.Unspecified;
			this.level = level;
			flag = false;
			if ( isLeaf ) 
				BecomeLeaf();
			else
				BecomeBranch();
		}

		public Node( ushort owner, byte color ) {
			// This creates a leaf by default.
			location = NodeLocation.Unspecified;
			level = -1;
			flag = false;
			BecomeLeaf( owner, color );
		}

		public Node( ushort owner, byte color, int level ) {
			// This creates a leaf by default.
			location = NodeLocation.Unspecified;
			this.level = level;
			flag = false;
			BecomeLeaf( owner, color );
		}

		public ushort Owner {
			get { return owner; }
			set { owner = value; }
		}

		public byte Color {
			get { return color; }
			set { color = value; }
		}

		public int Level {
			get { return level; }
			set { level = value; }
		}

		public bool Flag {
			get { return flag; }
			set { flag = value; }
		}

		public NodeLocation Location {
			get { return location; }
			set { location = value; }
		}

		public Node BottomRightChild {
			get { return this[NodeLocation.BottomRight] ; }
			set { this[NodeLocation.BottomRight] = value; }
		}

		public Node BottomLeftChild {
			get { return this[NodeLocation.BottomLeft] ; }
			set { this[NodeLocation.BottomLeft] = value; }
		}

		public Node TopRightChild {
			get { return this[NodeLocation.TopRight] ; }
			set { this[NodeLocation.TopRight] = value; }
		}

		public Node TopLeftChild {
			get { return this[NodeLocation.TopLeft] ; }
			set { this[NodeLocation.TopLeft] = value; }
		}

		public Node this[NodeLocation index] {
			get {
				if ( IsLeaf() ) throw new InvalidOperationException();
				if ( index == NodeLocation.Unspecified ) throw new ArgumentOutOfRangeException();
				return children[(int)index];

			}
			set {
				if ( IsLeaf() ) throw new InvalidOperationException();
				if ( index == NodeLocation.Unspecified ) throw new ArgumentOutOfRangeException();
				children[(int)index] = value;
			}
		}

		public bool IsLeaf() {
			return children == null;
		}

		public bool AllChildrenAreLeaves() {
			if ( IsLeaf() ) return false;

			return children[0].IsLeaf() && children[1].IsLeaf() && children[2].IsLeaf() && children[3].IsLeaf();
		}

		public void CreateBranches() {
			BecomeBranch();

			children[0] = new Node( false, level-1 );
			children[1] = new Node( false, level-1 );
			children[2] = new Node( false, level-1 );
			children[3] = new Node( false, level-1 );
		}

		public void CreateLeaves() {
			BecomeBranch();

			children[0] = new Node( true, level-1 );
			children[1] = new Node( true, level-1 );
			children[2] = new Node( true, level-1 );
			children[3] = new Node( true, level-1 );
		}

		public void BecomeLeaf() {
			children = null;
		}

		public void BecomeLeaf( ushort owner, byte color ) {
			children = null;
			this.owner = owner;
			this.color = color;
		}

		public void BecomeBranch( ) {
			children = new Node[4];
			this.owner = 0;
			this.color = 0;
		}
	}
}
