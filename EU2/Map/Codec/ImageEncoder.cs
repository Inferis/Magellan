using System;
using EU2.Map;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for ImageEncoder.
	/// </summary>
	public class ImageEncoder
	{
		protected static int[] thresholds = new int[6] { 8, 6, 4, 3, 2, 1 };

		private RawImage source;

		public ImageEncoder( RawImage source ) {
			this.source = source;
		}

		public MapBlock Encode( int x, int y ) {
			MapBlock result = new MapBlock( BuildTree( x, y, Lightmap.BlockFactor ) );

			return result;
		}

		#region Encode Helpers
		private Node BuildTree( int x, int y, int factor ) {
			// factor 0 = 1 pixel, so don't do advanced searching as it's useless.
			int size = 1 << factor;
			int topleft = source.Memory[x,y].Color << 16;
			int topright = source.Memory[x+size,y].Color << 16;
			int bottomleft = source.Memory[x,y+size].Color << 16;
			int bottomright = source.Memory[x+size,y+size].Color << 16;

			int leftystep = (bottomleft-topleft)/size;
			int rightystep = (bottomright-topright)/size;
			int blockowner = source[x,y].ID;

			// Walk over the blocks contents, and see if it is "uniform" or not. 
			// if it isn't, divide it into 4 subblocks and do the subprocessing of each block.
			for ( int cy=0, leftcolor=topleft, rightcolor=topright; 
				cy<size; 
				++cy, leftcolor+=leftystep, rightcolor+=rightystep ) {
				int xstep = (rightcolor-leftcolor) / size;

				for ( int cx=0, color = leftcolor; cx<size; ++cx, color+=xstep ) {
					Pixel mem = source[x+cx,y+cy];
					if ( mem.ID != blockowner || mem.IsBorder() || Math.Abs( mem.Color - ((color + 0x008000)>>16) ) >= thresholds[factor] ) {
						// Need to create a branch
						Node node = new Node( false ); 

						if ( --factor == 0 ) {
							node.BottomRightChild = new Node( source[x+1,y+1] );
							node.BottomLeftChild = new Node( source[x,y+1] );
							node.TopRightChild = new Node( source[x+1,y] );
							node.TopLeftChild = new Node( source[x,y] );
						}
						else {
							size >>= 1; // Half the size
							node.BottomRightChild = BuildTree( x+size, y+size, factor );
							node.BottomLeftChild = BuildTree( x, y+size, factor );
							node.TopRightChild = BuildTree( x+size, y, factor );
							node.TopLeftChild = BuildTree( x, y, factor );
						}

						return node;
					} 
				}
			}

			// This block is "uniform" so create a leaf...
			return new Node( source[x,y] );
		}

		#endregion
	}
}
