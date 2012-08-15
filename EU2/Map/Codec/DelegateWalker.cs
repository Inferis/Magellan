using System;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for DelegateWalker.
	/// </summary>
	public class DelegateWalker : BreadthFirstTreeWalker {
		public DelegateWalker( VisitDelegate visitor ) : base() {
			this.visitor = visitor;
		}

		public DelegateWalker( VisitDelegate visitor, TreeWalkerMode mode ) : base( false, mode ) {
			this.visitor = visitor;
		}

		public DelegateWalker( VisitDelegate visitor, bool visitBranches ) : base( visitBranches, TreeWalkerMode.Full ) {
			this.visitor = visitor;
		}

		public DelegateWalker( VisitDelegate visitor, bool visitBranches, TreeWalkerMode mode ) : base( visitBranches, mode ) {
			this.visitor = visitor;
		}

		protected override void OnVisitNode( Node node, int x, int y ) {
			visitor( node, x, y );
		}

		public delegate void VisitDelegate( Node node, int x, int y ); 

		private VisitDelegate visitor;
	}

}
