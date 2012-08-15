using System;
using EU2.Data;
using System.Diagnostics;

namespace EU2.Map.Codec.MapBlockHandling
{
	/// <summary>
	/// Summary description for TreeClipper.
	/// </summary>
	public class TreeClipperDeep : BreadthFirstTreeWalker {
		public TreeClipperDeep( ) : base( true ) {
		}

		protected override void OnVisitNode( Node node, int x, int y ) {
			if ( node.Level > 2 || node.IsLeaf() ) return;
					
			// Simple mode for now
			int scolor = 0;
			int sborder = 0;
			ushort[] id = new ushort[Province.MaxValue];
			ushort[] riverid = new ushort[Province.MaxValue];
			int samples = 0;
			
			SampleNode( node.BottomRightChild, ref scolor, ref sborder, ref id, ref riverid, ref samples );
			SampleNode( node.BottomLeftChild, ref scolor, ref sborder, ref id, ref riverid, ref samples );
			SampleNode( node.TopRightChild, ref scolor, ref sborder, ref id, ref riverid, ref samples );
			SampleNode( node.TopLeftChild, ref scolor, ref sborder, ref id, ref riverid, ref samples );

			ushort ididx = 0;
			ushort riveridx = 0;
			for ( ushort i=1; i<Province.MaxValue; ++i ) {
				if ( id[i] > id[ididx] ) ididx = i;
				if ( riverid[i] > riverid[riveridx] ) riveridx = i;
			}

			Debug.Assert( (byte)((sborder/samples)>>16) >= 0 && (byte)((sborder/samples)>>16) <= 2, "border assert failed" );

			node.BecomeLeaf( new Pixel( 
				(byte)((scolor/samples)>>16), 
				ididx, 
				riveridx, 
				(byte)((sborder/samples)>>16) ) );
		}

		private void SampleNode( Node node, ref int color, ref int border, ref ushort[] id, ref ushort[] riverid, ref int samples ) {
			if ( node.IsBranch() ) {
				color += 
					((int)node.BottomRightChild.Data.Color << 16) +
					((int)node.BottomLeftChild.Data.Color << 16) + 
					((int)node.TopRightChild.Data.Color << 16) + 
					((int)node.TopLeftChild.Data.Color << 16);

				border += 
					((int)node.BottomRightChild.Data.Border << 16) +
					((int)node.BottomLeftChild.Data.Border << 16) + 
					((int)node.TopRightChild.Data.Border << 16) + 
					((int)node.TopLeftChild.Data.Border << 16);

				++id[node.BottomRightChild.Data.ID];
				++id[node.BottomLeftChild.Data.ID];
				++id[node.TopRightChild.Data.ID];
				++id[node.TopLeftChild.Data.ID];

				++riverid[node.BottomRightChild.Data.RiverID];
				++riverid[node.BottomLeftChild.Data.RiverID];
				++riverid[node.TopRightChild.Data.RiverID];
				++riverid[node.TopLeftChild.Data.RiverID];

				samples += 4;
			}
			else {
				color += node.Data.Color << 16;
				border += node.Data.Border << 16;
				id[node.Data.ID] += 4;
				riverid[node.Data.RiverID] += 4;
				samples++;
			}
		}
	}
}
