using System;
using System.Drawing;
using System.Drawing.Imaging;
using EU2.Map.Codec;
using EU2.Map.Drawing;
using EU2.Data;
using System.Collections;

namespace MapToolsLib
{
	/// <summary>
	/// Summary description for BlockVisualiser.
	/// </summary>
	public class BlockVisualiser : DepthFirstTreeWalker
	{
		private const int VisPadding = 0;
		private const int VisSize = 30;

		public BlockVisualiser( ColorScales scales, ProvinceList provinces ) {
			this.shades = scales.Shades;
			this.provinces = provinces;

			this.rangecheck = new byte[512];
			for( int r=0; r<512; r++ ) {
				if ( r >= 256 ) rangecheck[r] = 127;
				else if ( r >= 132 ) rangecheck[r] = (byte)(r-128);
				else rangecheck[r] = 4;
			}		

			imagestack = new Stack();
		}

		public Bitmap Result { get { return result; } }

		protected override void OnAfterWalk() {
			result = (Bitmap)imagestack.Pop();
		}

		protected override void OnBeforeWalk() {
			visitBranches = true;
		}

		protected override void OnVisitNode( Node node, int x, int y ) {
			Bitmap vis = null;
			if ( node.IsBranch() && node.Level > 1 ) {
				// combine 4 images into a larger one
				Bitmap tl = (Bitmap)imagestack.Pop();
				Bitmap tr = (Bitmap)imagestack.Pop();
				Bitmap bl = (Bitmap)imagestack.Pop();
				Bitmap br = (Bitmap)imagestack.Pop();

				int width = bl.Width + br.Width;
				if ( tl.Width + tr.Width > width ) width = tl.Width + tr.Width;

				int height = tl.Height + bl.Height;
				if ( tr.Height + br.Height > height ) height = tr.Height + br.Height;
				
				vis = new Bitmap( width + VisPadding, height + VisPadding );
				using ( Graphics g = Graphics.FromImage( vis ) ) {
					g.DrawImageUnscaled( tl, 0, 0 );
					g.DrawImageUnscaled( tr, vis.Width-tr.Width, 0 );
					g.DrawImageUnscaled( bl, 0, vis.Height-bl.Height );
					g.DrawImageUnscaled( br, vis.Width-br.Width, vis.Height-br.Height );
				}
			}
			else if ( node.IsBranch() ) { 
				// Display 4 leaves on pixel level
				vis = new Bitmap( VisSize*2 + VisPadding, VisSize*2 + VisPadding, PixelFormat.Format32bppArgb );
				using ( Graphics g = Graphics.FromImage( vis ) ) {
					g.DrawLine( Pens.Black, VisSize, VisSize, vis.Width-VisSize, vis.Height-VisSize );
					g.DrawLine( Pens.Black, vis.Width-VisSize, VisSize, VisSize, vis.Height-VisSize );
					DrawNode( node.BottomRightChild, g,  vis.Width-VisSize, vis.Height-VisSize );
					DrawNode( node.BottomLeftChild, g, 0, vis.Height-VisSize );
					DrawNode( node.TopRightChild, g,  vis.Width-VisSize, 0 );
					DrawNode( node.TopLeftChild, g, 0, 0 );
				}
			}
			else { // Display a leaf above pixel level
				vis = new Bitmap( VisSize << node.Level, VisSize << node.Level, PixelFormat.Format32bppArgb );
				using ( Graphics g = Graphics.FromImage( vis ) ) {
					DrawNode( node, g, 0, 0 );
				}
			}

			imagestack.Push( vis );
		}
		
		private void DrawNode( Node node, Graphics g, int x, int y ) {
			int color = shades[((rangecheck[(node.Data.Color-0+128+32)]<<6) | (int)(provinces[node.Data.ID].Terrain.Color) )];

			unchecked {
				using ( Brush b = new SolidBrush( Color.FromArgb( (int)(0xFF000000) | color ) ) ) {
					g.FillRectangle( b, x, y, VisSize << node.Level, VisSize << node.Level );
				}
			}
			Brush fb = node.Data.IsBorder() ? Brushes.Red : Brushes.Black;

			g.FillRectangle( Brushes.White, x+2, y+2, VisSize-4, 8 );
			g.FillRectangle( Brushes.White, x+2, y+11, VisSize-4, 8 );
			g.FillRectangle( Brushes.White, x+2, y+20, VisSize-4, 8 );

			using ( StringFormat sf = new StringFormat( StringFormatFlags.NoWrap ) ) {
				sf.Alignment = StringAlignment.Center;
				using ( Font f = new Font( "Tahoma", 5.0F ) ) {
					g.DrawString( node.Data.ID.ToString(), f, fb, new RectangleF( x+2, y+2, VisSize-4, 8 ), sf );
					g.DrawString( node.Data.RiverID.ToString(), f, fb, new RectangleF( x+2, y+11, VisSize-4, 8 ), sf );
					g.DrawString( node.Data.Color.ToString(), f, fb, new RectangleF( x+2, y+20, VisSize-4, 8 ), sf );
				}
			}
		}

		private byte[] rangecheck;
		private int[] shades;
		private ProvinceList provinces;
		private Stack imagestack;
		private Bitmap result;
	}
}
