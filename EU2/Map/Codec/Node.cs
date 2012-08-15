using System;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for Node.
	/// </summary>
	public class Node {
		public enum NodeLocation {
			None = -1,
			BottomRight = 0,
			BottomLeft = 1,
			TopRight = 2,
			TopLeft = 3
		}

		public enum ChildMaskLocation {
			None = 0,
			BottomRight = 1,
			BottomLeft = 2,
			TopRight = 4,
			TopLeft = 8
		}

		private Node[] children;
		private Pixel data;
		private int level;
		//private NodeLocation location;

		public Node( bool isLeaf ) {
			//location = (NodeLocation)(-1);
			level = Lightmap.BlockFactor;
			data = Pixel.Empty;
			if ( isLeaf ) 
				BecomeLeaf();
			else
				BecomeBranch();
		}

		public Node( bool isLeaf, int level ) {
			//location = (NodeLocation)(-1);
			this.level = level;
			data = Pixel.Empty;
			if ( isLeaf ) 
				BecomeLeaf();
			else
				BecomeBranch();
		}

		public Node( Node original ) {
			//location = original.location;
			level = original.level;
			data = original.data; 

			if ( original.IsLeaf() ) 
				BecomeLeaf();
			else
				BecomeBranch();
		}

		public Node( Pixel data ) {
			// This creates a leaf by default.
			//location = (NodeLocation)(-1);
			level = Lightmap.BlockFactor;
			BecomeLeaf( data );
		}

		public Node( Pixel data, int level ) {
			// This creates a leaf by default.
			//location = (NodeLocation)(-1);
			this.level = level;
			BecomeLeaf( data );
		}

		public Pixel Data {
			get { return data; }
			set { data = value; }
		}

		public int Level {
			get { return level; }
			set { 
				if ( level != value && children != null ) {
					// Also set all children's levels... :(
					for ( int i=0; i<4; ++i ) {
						if ( children[i] != null ) children[i].Level = value-1;
					}
				}
				level = value; 
			}
		}

		public int ChildCount {
			get {
				if ( IsLeaf() ) return 0;
				int count = 0;

				for ( int i=0; i<4; ++i ) {
					if ( children[i] != null ) count++;
				}

				return count;
			}
		}

		public ChildMaskLocation ChildMask {
			get {
				if ( IsLeaf() ) return ChildMaskLocation.None;
				int mask = (int)ChildMaskLocation.None;

				for ( int i=0; i<4; ++i ) {
					if ( children[i] != null ) mask = mask | (1 << i);
				}

				return (ChildMaskLocation)mask;
			}
		}

		public Node BottomRightChild {
			get { 
				return children[(int)NodeLocation.BottomRight];
			}
			set { this[NodeLocation.BottomRight] = value; }
		}

		public Node BottomLeftChild {
			get { 
				return children[(int)NodeLocation.BottomLeft];
			}
			set { this[NodeLocation.BottomLeft] = value; }
		}

		public Node TopRightChild {
			get { 
				return children[(int)NodeLocation.TopRight];
			}
			set { this[NodeLocation.TopRight] = value; }
		}

		public Node TopLeftChild {
			get { 
				return children[(int)NodeLocation.TopLeft];
			}
			set { this[NodeLocation.TopLeft] = value; }
		}

		public Node this[NodeLocation index] {
			get {
				return children[(int)index];
			}
			set {
				if ( IsLeaf() ) throw new InvalidOperationException();
				value.Level = level-1; // Use property to update levels of children too
				children[(int)index] = value;
			}
		}

		public bool IsLeaf() {
			return children == null;
		}

		public bool IsBranch() {
			return children != null;
		}

		public bool AllChildrenAreLeaves() {
			if ( IsLeaf() ) return false;

			return children[0].IsLeaf() && children[1].IsLeaf() && children[2].IsLeaf() && children[3].IsLeaf();
		}

		public Node Clone() {
			Node result = new Node( IsLeaf(), level );
			result.data = data;
			if ( IsBranch() ) {
				result.children[0] = children[0].Clone();
				result.children[1] = children[1].Clone();
				result.children[2] = children[2].Clone();
				result.children[3] = children[3].Clone();
			}
			
			return result;
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

		public void BecomeLeaf( Pixel data ) {
			children = null;
			this.data = data;
		}

		public void BecomeBranch( ) {
			children = new Node[4];
		}

		public int CalcLeafCount() {
			int leafcount = 0;
			if ( IsLeaf() ) {
				leafcount = 1;
			}
			else {
				leafcount += BottomRightChild.CalcLeafCount();
				leafcount += BottomLeftChild.CalcLeafCount();
				leafcount += TopRightChild.CalcLeafCount();
				leafcount += TopLeftChild.CalcLeafCount();
			}

			return leafcount;
		}
	}

}
