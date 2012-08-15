using System;
using System.Drawing;

namespace PSD
{
	/// <summary>
	/// Summary description for LayerMode.
	/// </summary>
	public sealed class LayerMode {
		public static LayerMode Normal { get { return norm; } }
		public static LayerMode Darken { get { return dark; } }
		public static LayerMode Lighten { get { return lite; } }
		public static LayerMode Hue { get { return hue; } }
		public static LayerMode Saturation { get { return sat; } }
		public static LayerMode Colour { get { return colr; } }
		public static LayerMode Multiply { get { return mul; } }
		public static LayerMode Screen { get { return scrn; } }
		public static LayerMode Dissolve { get { return diss; } }
		public static LayerMode Difference { get { return diff; } }
		public static LayerMode Luminance { get { return lum; } }
		public static LayerMode HardLight { get { return hLit; } }
		public static LayerMode SoftLight { get { return sLit; } }
		public static LayerMode Overlay { get { return over; } }

		internal delegate void Blender( int[] basedata, int[] blenddata );

		internal LayerMode( string key, Blender blender ) {
			this.key = key;
			this.blender = blender;
		}

		public string Key {
			get { return key; }
		}

		public void Blend( int[] basedata, int[] blenddata, float opacity ) {
			blender( basedata, blenddata );
			
			for ( int i=0; i<blenddata.Length; ++i ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );

				int cr = MULTALPHA( c2.R, c1.R, (int)(c2.A*opacity) );
				int cg = MULTALPHA( c2.G, c1.G, (int)(c2.A*opacity) );
				int cb = MULTALPHA( c2.B, c1.B, (int)(c2.A*opacity) );

				basedata[i] = Color.FromArgb( 255, cr, cg, cb ).ToArgb();
			}
		}

		public static LayerMode FromKey( string key ) {
			key = key.ToLower();
			if ( key == norm.Key ) return norm;
			if ( key == dark.Key ) return dark;
			if ( key == lite.Key ) return lite;
			if ( key == hue.Key ) return hue;
			if ( key == sat.Key ) return sat;
			if ( key == colr.Key ) return colr;
			if ( key == mul.Key ) return mul;
			if ( key == scrn.Key ) return scrn;
			if ( key == diss.Key ) return diss;
			if ( key == diff.Key ) return diff;
			if ( key == lum.Key ) return lum;
			if ( key == hLit.Key ) return hLit;
			if ( key == sLit.Key ) return sLit;
			if ( key == over.Key ) return over;

			throw new ArgumentException( "No blend mode '" + key + "' found" );
		}

		internal static LayerMode norm = new LayerMode( "norm", new Blender( NormalBlender ) );
		internal static LayerMode dark = new LayerMode( "dark", new Blender( DarkenBlender ) );
		internal static LayerMode lite = new LayerMode( "lite", new Blender( LightenBlender ) );
		internal static LayerMode hue = new LayerMode( "hue ", new Blender( HueBlender ) );
		internal static LayerMode sat = new LayerMode( "sat ", new Blender( SaturationBlender ) );
		internal static LayerMode colr = new LayerMode( "colr", new Blender( ColourBlender ) );
		internal static LayerMode mul = new LayerMode( "mul ", new Blender( MultiplyBlender ) );
		internal static LayerMode scrn = new LayerMode( "scrn", new Blender( ScreenBlender ) );
		internal static LayerMode diss = new LayerMode( "diss", new Blender( DissolveBlender ) );
		internal static LayerMode diff = new LayerMode( "diff", new Blender( DifferenceBlender ) );
		internal static LayerMode lum = new LayerMode( "lum ", new Blender( LuminosityBlender ) );
		internal static LayerMode hLit = new LayerMode( "hLit", new Blender( HardLightBlender ) );
		internal static LayerMode sLit = new LayerMode( "sLit", new Blender( SoftLightBlender ) );
		internal static LayerMode over = new LayerMode( "over", new Blender( OverlayBlender ) );

		private string key;
		private Blender blender;

		private static void NormalBlender( int[] basedata, int[] blenddata ) {
			//Array.Copy( blenddata, 0, basedata, 0, blenddata.Length );
		}

		private static void DarkenBlender( int[] basedata, int[] blenddata  ) {
			throw new NotImplementedException();
		}
	
		private static void LightenBlender( int[] basedata, int[] blenddata ) {
			throw new NotImplementedException();
		}
	
		private static void HueBlender( int[] basedata, int[] blenddata ) {
			for ( int i=0; i<blenddata.Length; i++ ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );
				HSL h1 = Utils.RGB2HSL( c1 );
				HSL h2 = Utils.RGB2HSL( c2 );

				h1.H = h2.H; 
				int ca = (int)(c1.A < c2.A ? c1.A : c2.A);

				c1 = Utils.HSL2RGB( h1 );
				blenddata[i] = Color.FromArgb( ca, c1.R, c1.G, c1.B ).ToArgb();
			}
		}
	
		private static void SaturationBlender( int[] basedata, int[] blenddata ) {
			for ( int i=0; i<blenddata.Length; i++ ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );
				HSL h1 = Utils.RGB2HSL( c1 );
				HSL h2 = Utils.RGB2HSL( c2 );

				h1.S = h2.S; 
				int ca = (int)(c1.A < c2.A ? c1.A : c2.A);

				c1 = Utils.HSL2RGB( h1 );
				blenddata[i] = Color.FromArgb( ca, c1.R, c1.G, c1.B ).ToArgb();
			}
		}
	
		private static void ColourBlender( int[] basedata, int[] blenddata ) {
			for ( int i=0; i<blenddata.Length; i++ ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );
				HSL h1 = Utils.RGB2HSL( c1 );
				HSL h2 = Utils.RGB2HSL( c2 );

				h1.H = h2.H; 
				h1.S = h2.S; 
				int ca = (int)(c1.A < c2.A ? c1.A : c2.A);

				c1 = Utils.HSL2RGB( h1 );
				blenddata[i] = Color.FromArgb( ca, c1.R, c1.G, c1.B ).ToArgb();
			}
		}
	
		private static void MultiplyBlender( int[] basedata, int[] blenddata ) {
			for ( int i=0; i<blenddata.Length; i++ ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );

				int cr = MULT( c1.R, c2.R );
				int cg = MULT( c1.G, c2.G );
				int cb = MULT( c1.B, c2.B );
				int ca = (int)(c1.A < c2.A ? c1.A : c2.A);

				blenddata[i] = Color.FromArgb( ca, cr, cg, cb ).ToArgb();
			}
		}
	
		private static void ScreenBlender( int[] basedata, int[] blenddata ) {
			for ( int i=0; i<blenddata.Length; i++ ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );

				int cr = 255 - MULT( 255 - c1.R, 255 - c2.R );
				int cg = 255 - MULT( 255 - c1.G, 255 - c2.G );
				int cb = 255 - MULT( 255 - c1.B, 255 - c2.B );
				int ca = (int)(c1.A < c2.A ? c1.A : c2.A);

				blenddata[i] = Color.FromArgb( ca, cr, cg, cb ).ToArgb();
			}
		}
	
		private static void DissolveBlender( int[] basedata, int[] blenddata ) {
			throw new NotImplementedException();
		}
	
		private static void DifferenceBlender( int[] basedata, int[] blenddata ) {
			for ( int i=0; i<blenddata.Length; i++ ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );

				int cr = c1.R - c2.R; if ( cr < 0 ) cr = -cr;
				int cg = c1.G - c2.G; if ( cg < 0 ) cg = -cg;
				int cb = c1.B - c2.B; if ( cb < 0 ) cb = -cb;
				int ca = (int)(c1.A < c2.A ? c1.A : c2.A);

				blenddata[i] = Color.FromArgb( ca, cr, cg, cb ).ToArgb();
			}
		}

		private static void LuminosityBlender( int[] basedata, int[] blenddata ) {
			for ( int i=0; i<blenddata.Length; i++ ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );
				HSL h1 = Utils.RGB2HSL( c1 );
				HSL h2 = Utils.RGB2HSL( c2 );

				h1.L = h2.L; 
				int ca = (int)(c1.A < c2.A ? c1.A : c2.A);

				c1 = Utils.HSL2RGB( h1 );
				blenddata[i] = Color.FromArgb( ca, c1.R, c1.G, c1.B ).ToArgb();
			}
		}
	
		private static void HardLightBlender( int[] basedata, int[] blenddata ) {
			throw new NotImplementedException();
		}

		private static void SoftLightBlender( int[] basedata, int[] blenddata ) {
			/*
				 tmpM = INT_MULT (src1[b], src2[b], tmpM);
				tmpS = 255 - INT_MULT((255 - src1[b]), (255 - src2[b]), tmp1);
				dest[b] = INT_MULT ((255 - src1[b]), tmpM, tmp2) + INT_MULT (src1[b], tmpS, tmp3);
			*/
			for ( int i=0; i<blenddata.Length; i++ ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );

				int cr = MULT( (255-c1.R), MULT( c1.R, c2.R ) ) + MULT( c1.R, 255 - MULT( 255-c1.R, 255-c2.R ) );
				int cg = MULT( (255-c1.G), MULT( c1.R, c2.G ) ) + MULT( c1.G, 255 - MULT( 255-c1.G, 255-c2.G ) );
				int cb = MULT( (255-c1.B), MULT( c1.R, c2.B ) ) + MULT( c1.B, 255 - MULT( 255-c1.B, 255-c2.B ) );
				int ca = (int)(c1.A < c2.A ? c1.A : c2.A);

				blenddata[i] = Color.FromArgb( ca, cr, cg, cb ).ToArgb();
			}
		}
	
		private static void OverlayBlender( int[] basedata, int[] blenddata ) {
			for ( int i=0; i<blenddata.Length; i++ ) {
				Color c1 = Color.FromArgb( basedata[i] );
				Color c2 = Color.FromArgb( blenddata[i] );

				int cr = MULT( c1.R, c1.R + MULT( 2*c2.R, 255-c1.R ) );
				int cg = MULT( c1.G, c1.G + MULT( 2*c2.G, 255-c1.G ) );
				int cb = MULT( c1.B, c1.B + MULT( 2*c2.B, 255-c1.B ) );
				int ca = (int)(c1.A < c2.A ? c1.A : c2.A);

				blenddata[i] = Color.FromArgb( ca, cr, cg, cb ).ToArgb();
			}
		}

		private static int MULT( int a, int b ) {
			int t = (a * b + 0x80);
			return ((t >> 8) + t) >> 8;
		}

		private static int MULTALPHA( int a, int b, int alpha ) {
			return MULT( a - b, (int)(alpha) ) + b;
		}
	}
}
