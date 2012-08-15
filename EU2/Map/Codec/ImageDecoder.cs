using System;
using System.Drawing;
using EU2.Map.Drawing;
using EU2.Map;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for ImageDecoder.
	/// </summary>
	public class ImageDecoder
	{
		public ImageDecoder( RawImage target ) {
			memory = target.Memory;
			ClearBuffer();
		}

		public void Decode4( int x, int y, MapBlock block, MapBlock right, MapBlock bottom, MapBlock bottomright ) {
			if ( block == null ) {
				// We're not... draw black area.
				DrawBlack( x, y );
				return;
			}

			// Valid block, do more drawing
			if ( bottomright != null && bottom != null ) {
				// Fill Shadowbuffer with bottom and bottom-right info
				PrepareBottomRight( bottomright );
				PrepareBottom( bottom );
			}
			else {
				ClearBuffer();
			}

			// Fill Shadowbuffer with info from the block right to this one
			if ( right != null ) PrepareRight( right );

			// do the actual drawing of this block
			Draw( block, x, y );
		}
		
		public void Decode2( int x, int y, MapBlock block, MapBlock right ) {
			ShiftShadowDown();

			if ( block == null ) {
				// We're not... draw black area.
				DrawBlack( x, y );
				return;
			}

			// Fill Shadowbuffer with info from the block right to this one
			if ( right != null ) PrepareRight( right );

			// do the actual drawing of this block
			Draw( block, x, y );
		}

		public void Decode1( int x, int y, MapBlock block ) {
			if ( block == null ) {
				// We're not... draw black area.
				DrawBlack( x, y );
				return;
			}

			// do the actual drawing of this block
			ClearBuffer();
			Draw( block, x, y );
		}

		#region Helpers to do with clearing
		protected virtual void DrawBlack( int x, int y ) {
			for ( int by=0; by<Lightmap.BlockSize; ++by ) {
				for ( int bx=0; bx<Lightmap.BlockSize; ++bx ) {
					memory[x+bx, y+by].Color = 63;
				}
			}
		}

		#endregion

		#region Shadowbuffer prepare stuff
		private void ShiftShadowDown( ) {
			for ( int y=0; y<Lightmap.BlockSize; ++y ) {
				for ( int x=0; x<Lightmap.BlockSize * 2; ++x ) {
					shadow[x,y+Lightmap.BlockSize] = shadow[x,y];
				}
			}
		}

		private void ClearBuffer( ) {
			const int ShadowSize = Lightmap.BlockSize * 2;
			if ( shadow == null ) shadow = new byte[ShadowSize,ShadowSize];

			for ( int y=0; y<ShadowSize; ++y ) {
				for ( int x=0; x<ShadowSize; ++x ) {
					shadow[x,y] = 6;
				}
			}
		}


		#region BottomRight
		private void PrepareBottomRight( MapBlock block ) {
			sx = 0; sy = 0;
			//block.WalkTree( MapBlock.WalkMode.TopLeftOnly, new MapBlock.Walker( BottomRightWalker ), 0, 0 );
			block.WalkTree( new DelegateWalker.VisitDelegate( BottomRightWalker ), TreeWalkerMode.TopLeftOnly );
		}

		private void BottomRightWalker( Node node, int x, int y ) {
			if ( node.IsLeaf() ) 
				shadow[Lightmap.BlockSize,Lightmap.BlockSize] = node.Data.Color;
			else 
				shadow[Lightmap.BlockSize,Lightmap.BlockSize] = node.TopLeftChild.Data.Color;
		}

		#endregion

		#region Bottom
		protected virtual void PrepareBottom( MapBlock block ) {
			sx = 0; sy = 0;
			//block.WalkTree( MapBlock.WalkMode.TopOnly, new MapBlock.Walker( BottomWalker ), 0, 0 );
			block.WalkTree( new DelegateWalker.VisitDelegate( BottomWalker ), TreeWalkerMode.TopOnly );
		}

		private void BottomWalker( Node node, int x, int y ) {
			y += Lightmap.BlockSize;
			if ( node.IsBranch() ) {
				shadow[x+1,y] = node.TopRightChild.Data.Color;
				shadow[x,y] = node.TopLeftChild.Data.Color;
			}
			else {
				int size = 1 << node.Level;
				int topleft = node.Data.Color << 16;
				int topright = (shadow[x+size, y]) << 16; 

				// Do the interpolation
				int delta = (topright - topleft) >> node.Level;
				int color = topleft;

				for ( int a = 0; a<size; a++ ) { // Loop horizontally
					shadow[x+a, y] = (byte)(color>>16);
					color += delta;
				}
			}
		}

		#endregion

		#region Right
		protected virtual void PrepareRight( MapBlock block ) {
			sx = 0; sy = 0;
			block.WalkTree( new DelegateWalker.VisitDelegate( RightWalker ), TreeWalkerMode.LeftOnly );
		}

		private void RightWalker( Node node, int x, int y ) {
			x += Lightmap.BlockSize;
			if ( node.IsBranch() ) {
				shadow[x,y+1] = node.BottomLeftChild.Data.Color;
				shadow[x,y] = node.TopLeftChild.Data.Color;
			}
			else {
				int size = 1 << node.Level;
				int topleft = node.Data.Color << 16;
				int bottomleft = (shadow[x, y+size]) << 16; 

				// Do the interpolation
				int delta = (bottomleft - topleft) >> node.Level;
				int color = topleft;

				for ( int a = 0; a<size; a++ ) { // Loop horizontally
					shadow[x, y+a] = (byte)(color>>16);
					color += delta;
				}
			}
		}
		#endregion

		#endregion

		#region Block Drawing
		protected virtual void Draw( MapBlock block, int x, int y ) {
			sx = x; sy = y;
			block.WalkTree( new DelegateWalker.VisitDelegate( DrawWalker ) );
		}

		private void DrawWalker( Node node, int x, int y ) {
			int level = node.Level;
			x += sx;
			y += sy;

			if ( node.IsBranch() ) {
				shadow[x-sx+1,y-sy+1] = (memory[(x+1),(y+1)] = node.BottomRightChild.Data).Color; 
				shadow[x-sx,y-sy+1] = (memory[x,(y+1)] = node.BottomLeftChild.Data).Color;
				shadow[x-sx+1,y-sy] = (memory[(x+1),y] = node.TopRightChild.Data).Color; 
				shadow[x-sx,y-sy] = (memory[x,y] = node.TopLeftChild.Data).Color; 
			}
			else {
				int size = 1 << level;
				ushort pid = node.Data.ID;
				ushort rid = node.Data.RiverID;
				byte border = node.Data.Border;

				int topleft = node.Data.Color << 16;
				int bottomright = (shadow[x-sx+size, y-sy+size]) << 16; 
				int topright = (shadow[x-sx+size, y-sy]) << 16; 
				int bottomleft = (shadow[x-sx, y-sy+size]) << 16; 

				int leftstep = (bottomleft - topleft) >> node.Level;
				int rightstep = (bottomright - topright) >> node.Level;
				int left = topleft;
				int right = topright;

				// Do the interpolation
				int xstep = (right - left) >> level;
				int light = left;

				for ( int a = 0; a<size; a++ ) { // Loop horizontally
					shadow[x-sx+a, y-sy] = (byte)(light>>16);
					light += xstep;
				}

				for ( int b = 0; b < size; b++ ) { // Loop vertically
					xstep = (left - right) >> level;
					light = left;

					shadow[x-sx, y-sy+b] = (byte)(left>>16);

					if ( xstep == 0 ) {
						for ( int a = 0; a<size; a++ ) { // Loop horizontally
							memory[(x+a),(y+b)].Color = (byte)((light+0x8000)>>16);
							memory[(x+a),(y+b)].ID = pid;
							memory[(x+a),(y+b)].RiverID = rid;
							memory[(x+a),(y+b)].Border = border;
						}
					}
					else if ( size == 1 ) {
						memory[(x),(y+b)].Color = (byte)((light+0x8000)>>16);				
						memory[(x),(y+b)].ID = pid;
						memory[(x),(y+b)].RiverID = rid;
						memory[(x),(y+b)].Border = border;
					}
					else {
						int prevlight = light;
						int preva = 0;
						for ( int a = 0; a<size; a++ ) { // Loop horizontally
							if ( light != prevlight ) { // Do as little as decoding as possible...
								for ( int a2=preva; a2<a; ++a2 ) {
									memory[(x+a2),(y+b)].Color = (byte)((prevlight+0x8000)>>16);
									memory[(x+a2),(y+b)].ID = pid;
									memory[(x+a2),(y+b)].RiverID = rid;
									memory[(x+a2),(y+b)].Border = border;
								}
								preva = a;
								prevlight = light;
							}
							
							light -= xstep;
						}
						if ( preva<size ) {
							for ( int a2=preva; a2<size; ++a2 ) {
								memory[(x+a2),(y+b)].Color = (byte)((prevlight+0x8000)>>16);
								memory[(x+a2),(y+b)].ID = pid;
								memory[(x+a2),(y+b)].RiverID = rid;
								memory[(x+a2),(y+b)].Border = border;
							}
						}
					}

					left += leftstep;
					right += rightstep;
				}
			}
		}


		#endregion

		#region Private Fields
		private byte[,] shadow;			// for decoding
		private Pixel[,] memory;
		int sx, sy;
		#endregion
	}
}
