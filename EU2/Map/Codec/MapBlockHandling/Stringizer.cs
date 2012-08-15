using System;
using System.Text;

namespace EU2.Map.Codec.MapBlockHandling
{
	/// <summary>
	/// Summary description for Stringizer.
	/// </summary>
	#region Stringizer Class
	public class Stringizer : BreadthFirstTreeWalker {
		public Stringizer() : base( true ) {
			sb = new StringBuilder( 1024 );
		}

		public override string ToString() {
			return sb.ToString();
		}

		protected override void OnVisitNode( Node node, int x, int y ) {
			sb.AppendFormat( "[{0}|{1}|", node.Level, node.IsBranch() ? "B" : "L" );

			if ( !(node.IsBranch() && node.Level > 1) ) {
				if ( node.IsBranch() ) { // 4 leaves on pixel level
					sb.Append( "4" );
					HandleNode( node.BottomRightChild, x+1, y+1 );
					HandleNode( node.BottomLeftChild, x, y+1 );
					HandleNode( node.TopRightChild, x+1, y );
					HandleNode( node.TopLeftChild, x, y );
				}
				else { // Leaf above pixel level
					sb.Append( "1" );
					HandleNode( node, x, y );
				}
			}

			sb.Append( "] " );
		}

		private void HandleNode( Node node, int x, int y ) {
			sb.AppendFormat( "{0:00},{1:00}|{2:00}|{3:0000}|{4}",
				x, y, node.Data.Color, node.Data.ID, node.Data.Border );
		}

		#region Private Fields
		private StringBuilder sb;
		#endregion
	}

	#endregion
}
