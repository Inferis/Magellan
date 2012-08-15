using System;

namespace EU2.Map.Codec.MapBlockHandling
{
	/// <summary>
	/// Summary description for TreeOptimiser.
	/// </summary>
	public class TreeOptimiser : BreadthFirstTreeWalker {
		public TreeOptimiser( bool lowestonly ) : base( true ) {
			this.optimised = false;
			this.lowestonly = lowestonly;
		}

		public TreeOptimiser() : this( false ) {
		}

		protected override void OnBeforeWalk() {
			optimised = false;
		}

		protected override void OnVisitNode( Node node, int x, int y ) {
			if ( node.IsLeaf() ) return;
			if ( lowestonly && node.Level > 1 ) return;

			// Check if children are leafs
			if ( !node.BottomRightChild.IsLeaf() || !node.BottomLeftChild.IsLeaf() || !node.TopRightChild.IsLeaf() || !node.TopLeftChild.IsLeaf() ) return;

			// Yes, check for same values
			if (  node.BottomRightChild.Data == node.BottomLeftChild.Data && 
				node.BottomRightChild.Data == node.TopRightChild.Data && 
				node.BottomRightChild.Data == node.TopLeftChild.Data ) {
				// Merge
				node.BecomeLeaf( node.BottomRightChild.Data );
				optimised = true;
			}
		}

		public bool Optimised { get { return optimised; } }
			
		private bool optimised;
		private bool lowestonly;
	}
}
