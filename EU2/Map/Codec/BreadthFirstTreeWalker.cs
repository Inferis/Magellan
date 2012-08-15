using System;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for TreeWalker.
	/// </summary>
	public abstract class BreadthFirstTreeWalker : ITreeWalker
	{
		public BreadthFirstTreeWalker( ) : this( false, TreeWalkerMode.Full ) {
		}

		public BreadthFirstTreeWalker( TreeWalkerMode mode ) : this( false, mode ) {
		}

		public BreadthFirstTreeWalker( bool visitBranches ) : this( visitBranches, TreeWalkerMode.Full ) {
		}

		public BreadthFirstTreeWalker( bool visitBranches, TreeWalkerMode mode ) {
			this.visitBranches = visitBranches;
			this.defaultWalkmode = mode;
		}

		public int StopAtLevel {
			get {
				return stopAtLevel;
			}
			set {
				stopAtLevel = value;
			}
		}

		public void WalkTree( MapBlock block ) {
			WalkTree( block, defaultWalkmode );
		}

		public void WalkTree( MapBlock block, TreeWalkerMode mode ) {
			OnBeforeWalk();
			switch ( mode ) {
				case TreeWalkerMode.Full: 
					WalkTreeFull( block.Tree, 0, 0 ); break;
				case TreeWalkerMode.LeftOnly: 
					WalkTreeLeft( block.Tree, 0, 0 ); break;
				case TreeWalkerMode.TopLeftOnly: 
					WalkTreeTopLeft( block.Tree, 0, 0 ); break;
				case TreeWalkerMode.TopOnly: 
					WalkTreeTop( block.Tree, 0, 0 ); break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			OnAfterWalk();
		}

		protected virtual void OnBeforeWalk() {
		}

		protected abstract void OnVisitNode( Node node, int x, int y );

		protected virtual void OnAfterWalk() {
		}

		#region Specialised Walkers
		protected void WalkTreeFull( Node node, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);
			Node.ChildMaskLocation oldmask = node.ChildMask;

			if ( node.IsBranch() && node.Level > stopAtLevel ) {
				if ( visitBranches ) OnVisitNode( node, x, y );

				if ( node.IsBranch() ) {
					WalkTreeFull( node.BottomRightChild, x+size, y+size );
					WalkTreeFull( node.BottomLeftChild, x, y+size );
					WalkTreeFull( node.TopRightChild, x+size, y );
					WalkTreeFull( node.TopLeftChild, x, y );
				}
			}
			else { 
				OnVisitNode( node, x, y );

				// OnVisitNode might have changed the node (leaf<->branch), so have to recheck
				if ( node.ChildMask != oldmask && node.IsBranch() ) {
					WalkTreeFull( node.BottomRightChild, x+size, y+size );
					WalkTreeFull( node.BottomLeftChild, x, y+size );
					WalkTreeFull( node.TopRightChild, x+size, y );
					WalkTreeFull( node.TopLeftChild, x, y );
				}
			}
		}
	
		protected void WalkTreeLeft( Node node, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);
			Node.ChildMaskLocation oldmask = node.ChildMask;

			if ( node.IsBranch() && node.Level > stopAtLevel ) {
				if ( visitBranches ) OnVisitNode( node, x, y );

				WalkTreeLeft( node.BottomLeftChild, x, y+size );
				WalkTreeLeft( node.TopLeftChild, x, y );
			}
			else { 
				OnVisitNode( node, x, y );

				// OnVisitNode might have changed the node (leaf<->branch), so have to recheck
				if ( node.ChildMask != oldmask && node.IsBranch() ) {
					WalkTreeLeft( node.BottomLeftChild, x, y+size );
					WalkTreeLeft( node.TopLeftChild, x, y );
				}
			}
		}
	
		protected void WalkTreeTop( Node node, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);
			Node.ChildMaskLocation oldmask = node.ChildMask;

			if ( node.IsBranch() && node.Level > stopAtLevel ) {
				if ( visitBranches ) OnVisitNode( node, x, y );

				WalkTreeTop( node.TopRightChild, x+size, y );
				WalkTreeTop( node.TopLeftChild, x, y );
			}
			else { 
				OnVisitNode( node, x, y );

				// OnVisitNode might have changed the node (leaf<->branch), so have to recheck
				if ( node.ChildMask != oldmask && node.IsBranch() ) {
					WalkTreeTop( node.TopRightChild, x+size, y );
					WalkTreeTop( node.TopLeftChild, x, y );
				}
			}
		}
	
		protected void WalkTreeTopLeft( Node node, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);
			Node.ChildMaskLocation oldmask = node.ChildMask;

			if ( node.IsBranch() && node.Level > stopAtLevel ) {
				if ( visitBranches ) OnVisitNode( node, x, y );

				WalkTreeTopLeft( node.TopLeftChild, x, y );
			}
			else { 
				OnVisitNode( node, x, y );

				// OnVisitNode might have changed the node (leaf<->branch), so have to recheck
				if ( node.ChildMask != oldmask && node.IsBranch() ) {
					WalkTreeTopLeft( node.TopLeftChild, x, y );
				}
			}
		}
		#endregion

		private bool visitBranches;
		private TreeWalkerMode defaultWalkmode;
		private int stopAtLevel = 1;
		
	}
}
