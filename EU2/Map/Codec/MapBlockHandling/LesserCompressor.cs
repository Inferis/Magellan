using System;

namespace EU2.Map.Codec.MapBlockHandling
{
	/// <summary>
	/// Summary description for Compressor.
	/// </summary>
	public class LesserCompressor : Compressor {
		public LesserCompressor( ) {
		}

		protected override void OnVisitNode( Node node, int x, int y ) {
			tree[treeindex++] = node.IsBranch();

			if ( node.IsBranch() && node.Level > 1 ) return;

			if ( node.IsBranch() ) { // 4 leaves on pixel level
				owners[leafindex] = ConvertID( node.BottomRightChild.Data.ID ); // Store the province
				colors[leafindex++] = node.BottomRightChild.Data.Color;

				owners[leafindex] = ConvertID( node.BottomLeftChild.Data.ID ); // Store the province
				colors[leafindex++] = node.BottomLeftChild.Data.Color;

				owners[leafindex] = ConvertID( node.TopRightChild.Data.ID ); // Store the province
				colors[leafindex++] = node.TopRightChild.Data.Color;

				owners[leafindex] = ConvertID( node.TopLeftChild.Data.ID ); // Store the province
				colors[leafindex++] = node.TopLeftChild.Data.Color;
			}
			else { // Leaf above pixel level
				owners[leafindex] = ConvertID( node.Data.ID );
				colors[leafindex++] = node.Data.Color;
			}
		}
	}
}
