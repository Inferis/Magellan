using System;

namespace EU2.Map.Codec.MapBlockHandling
{
	/// <summary>
	/// Summary description for TreeClipper.
	/// </summary>
	public class TreeClipper : BreadthFirstTreeWalker {
		public TreeClipper( ) : base( true ) {
		}

		protected override void OnVisitNode( Node node, int x, int y ) {
			if ( node.Level > 1 || node.IsLeaf() ) return;
					
			// Simple mode for now
			byte color = (byte)(
				((float)(node.BottomRightChild.Data.Color) * 0.15) + 
				((float)(node.BottomLeftChild.Data.Color) * 0.25) + 
				((float)(node.TopRightChild.Data.Color) * 0.25) +
				((float)(node.TopLeftChild.Data.Color) * 0.35));
			//byte color = (byte)((((node.BottomRightChild.Data.Color<<16) + (node.BottomLeftChild.Data.Color<<16) + 
			//	(node.TopRightChild.Data.Color<<16) + (node.TopLeftChild.Data.Color<<16)) / 4) >> 16);

			byte border = (byte)((((node.BottomRightChild.Data.Border<<16) + (node.BottomLeftChild.Data.Border<<16) + 
				(node.TopRightChild.Data.Border<<16) + (node.TopLeftChild.Data.Border<<16)) / 4) >> 16);

			//if ( node.BottomRightChild.Data.Border == 1 || node.BottomLeftChild.Data.Border == 1 || node.TopRightChild.Data.Border == 1 || node.TopLeftChild.Data.Border == 1 ) border = 1;
			//if ( node.BottomRightChild.Data.Border == 2 || node.BottomLeftChild.Data.Border == 2 || node.TopRightChild.Data.Border == 2 || node.TopLeftChild.Data.Border == 2 ) border = 2;

			ushort id = node.TopLeftChild.Data.ID;
			ushort riverid = node.TopLeftChild.Data.RiverID;

			node.BecomeLeaf( new Pixel( color, id, riverid, border ) );
		}
	}
}
