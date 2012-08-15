using System;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for TreeWalker.
	/// </summary>
	public abstract class DepthFirstTreeWalker : ITreeWalker
	{
		public DepthFirstTreeWalker( ) : this( false, TreeWalkerMode.Full ) {
		}

		public DepthFirstTreeWalker( TreeWalkerMode mode ) : this( false, mode ) {
		}

		public DepthFirstTreeWalker( bool visitBranches ) : this( visitBranches, TreeWalkerMode.Full ) {
		}

		public DepthFirstTreeWalker( bool visitBranches, TreeWalkerMode mode ) {
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

			if ( node.IsBranch() && node.Level > stopAtLevel ) {
				if ( node.IsBranch() ) {
					WalkTreeFull( node.BottomRightChild, x+size, y+size );
					WalkTreeFull( node.BottomLeftChild, x, y+size );
					WalkTreeFull( node.TopRightChild, x+size, y );
					WalkTreeFull( node.TopLeftChild, x, y );
				}

				if ( visitBranches ) OnVisitNode( node, x, y );
			}
			else { 
				OnVisitNode( node, x, y );
			}
		}
	
		protected void WalkTreeLeft( Node node, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);

			if ( node.IsBranch() && node.Level > stopAtLevel ) {
				WalkTreeLeft( node.BottomLeftChild, x, y+size );
				WalkTreeLeft( node.TopLeftChild, x, y );

				if ( visitBranches ) OnVisitNode( node, x, y );
			}
			else { 
				OnVisitNode( node, x, y );
			}
		}
	
		protected void WalkTreeTop( Node node, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);

			if ( node.IsBranch() && node.Level > stopAtLevel ) {
				WalkTreeTop( node.TopRightChild, x+size, y );
				WalkTreeTop( node.TopLeftChild, x, y );

				if ( visitBranches ) OnVisitNode( node, x, y );
			}
			else { 
				OnVisitNode( node, x, y );
			}
		}
	
		protected void WalkTreeTopLeft( Node node, int x, int y ) {
			if ( node == null ) return;
			int size = ((1 << node.Level) >> 1);

			if ( node.IsBranch() && node.Level > stopAtLevel ) {
				WalkTreeTopLeft( node.TopLeftChild, x, y );

				if ( visitBranches ) OnVisitNode( node, x, y );
			}
			else { 
				OnVisitNode( node, x, y );
			}
		}
		#endregion

		protected bool visitBranches;
		protected TreeWalkerMode defaultWalkmode;
		protected int stopAtLevel = 1;
	}
}
