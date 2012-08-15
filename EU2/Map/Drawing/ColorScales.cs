using System;
using EU2.IO;
using System.IO;
using System.Drawing;

namespace EU2.Map.Drawing
{
	/// <summary>
	/// Summary description for ColorScales.
	/// </summary>
	public class ColorScales : IStreamWriteable
	{
		public const int Count = 26;
		public const int MinValue = 0;
		public const int MaxValue = 25;

		private ColorScale[] scales;
		private int[] shades;
		private bool recalc;

		public ColorScales() {
			scales = new ColorScale[Count];
			for ( int i=0; i<Count; ++i ) {
				scales[i] = new ColorScale( "" );
			}
			shades = new int[8192];
			recalc = true;
		}

		public ColorScales( CSVReader reader ) {
			scales = new ColorScale[Count];
			for ( int i=0; i<Count; ++i ) {
				scales[i] = new ColorScale( "" );
			}
			shades = new int[8192];

			ReadFrom( reader );
		}

		public void ReadFrom( CSVReader reader ) {
			for ( int i=0; i<Count; ++i ) {
				scales[i].ReadFrom( reader );
			}
			recalc = true;
		}

		public void WriteTo( CSVWriter writer ) {
			for ( int i=0; i<Count; ++i ) {
				scales[i].WriteTo( writer );
			}
			writer.Flush();
		}

		public virtual void WriteTo( Stream stream ) {
			CSVWriter writer = new CSVWriter( stream );
			WriteTo( writer );
		}

		public void CalculateShades() {
			// for each color
			for ( int c=0; c<Count; ++c ) {
				int[] normalshades = scales[c].Interpolate();
				int[] darkshades = scales[c].Interpolate( 0.5 );

				for ( int i=0; i<normalshades.Length; ++i ) {
					shades[(((i+32)<<6) | c)] = normalshades[i];
					shades[(((i+32)<<6) | c)+32] = darkshades[i];
				}
			}

				/*
			double dr, dg, db, dr2, dg2, db2;
			double r, g, b, r2, g2, b2;

			for ( int c=0; c<Count; ++c ) {
				for ( int i=0; i<3; ++i ) {
					dr = (scales[c][i+1].Red-scales[c][i].Red)/(scales[c][i+1].Index-scales[c][i].Index);
					dg = (scales[c][i+1].Green-scales[c][i].Green)/(scales[c][i+1].Index-scales[c][i].Index);
					db = (scales[c][i+1].Blue-scales[c][i].Blue)/(scales[c][i+1].Index-scales[c][i].Index);

					r = scales[c][i].Red;
					g = scales[c][i].Green;
					b = scales[c][i].Blue;
					for ( int di=scales[c][i].Index; di<scales[c][i+1].Index; ++di ) {
						shades[(((di+32)<<6) | c)] = MakeShade( r, g, b );
						shades[(((di+32)<<6) | c)+32] = MakeShade( r/2, g/2, b/2 );

						r += dr;
						g += dg;
						b += db;
					}
				}
				*/

				/*
				// Don't know what this lot is for...
				double cr1 = 255, cg1 = 245, cb1 = 220;
				double cr2 = 226, cg2 = 220, cb2 = 180;
				dr = (cr1-cr2)/4;
				dg = (cg1-cg2)/4;
				db = (cb1-cb2)/4;

				r = cr1;
				g = cg1;
				b = cb1;
				for ( int di=0; di<4; ++di ) {
					shades[(((di)<<6) | c)] = MakeShade( r, g, b );
					shades[(((di)<<6) | c)+32] = MakeShade( r, g, b );

					r += dr;
					g += dg;
					b += db;
				}

				dr = (scales[c][0].Red-cr1) / 28;
				dg = (scales[c][0].Green-cg1) / 28;
				db = (scales[c][0].Blue-cb1) / 28;
				r = cr1;
				g = cg1;
				b = cb1;
				dr2 = ((scales[c][0].Red/2)-cr1) / 28;
				dg2 = ((scales[c][0].Green/2)-cg1) / 28;
				db2 = ((scales[c][0].Blue/2)-cb1) / 28;
				r2 = cr1;
				g2 = cb1;
				b2 = cg1;

				for ( int di=4; di<32; ++di ) {
					shades[(((di)<<6) | c)] = MakeShade( r, g, b );
					shades[(((di)<<6) | c)+32] = MakeShade( r2, g2, b2 );

					r += dr;
					g += dg;
					b += db;
					r2 += dr2;
					g2 += dg2;
					b2 += db2;
				}

				cr1 = 50; cg1 = 50; cb1 = 50;
				dr = (scales[c][3].Red-cr1) / 32;
				dg = (scales[c][3].Green-cg1) / 32;
				db = (scales[c][3].Blue-cb1) / 32;
				r = scales[c][3].Red;
				g = scales[c][3].Green;
				b = scales[c][3].Blue;

				for ( int di=96; di<128; ++di ) {
					shades[(((di)<<6) | c)] = MakeShade( r, g, b );
					shades[(((di)<<6) | c)+32] = MakeShade( r/2, g/2, b/2 );

					r += dr;
					g += dg;
					b += db;
				}
			}
			*/
		}

        public ColorScale[] Scales {
            get {
                return scales;
            }
        }

		public int[] Shades {
			get { 
				if ( recalc ) {
					CalculateShades(); 
					recalc = false; 
				}
				return shades; 
			}
		}
	}

	public enum MapColor {
		Black = 0,
		Green = 1,
		Orange = 2,
		LightBlue = 3,
		LightGray = 4,
		LightBrown = 5,
		DarkYellow = 6,
		Yellow = 7,
		DarkGray = 8,
		LightYellow = 9,
		Gray = 10,
		DarkGreen = 11,
		Blue = 12,
		LightRed = 13,
		White = 14,
		DarkBrown = 15,
		Red = 16,
		LightGreen = 17,
		LightOrange = 18,
		Brown = 19,
		DarkBlue = 20,
		DarkRed = 21,
		DarkOrange = 22,
		Water = 23,
		Border = 24,
		BorderOutline = 25
	};


	public struct ColorScale {
		private string name;
		private RGBI[] data;

		public ColorScale( string name ) {
			this.name = name;
			this.data = new RGBI[4];
			for ( int i=0; i<4 && i<data.Length; ++i ) this.data[i] = new RGBI(0,0,0,0);
		}

		public ColorScale( string name, RGBI[] data ) {
			this.name = name;
			this.data = new RGBI[4];
			for ( int i=0; i<4 && i<data.Length; ++i ) this.data[i] = data[i];
		}

		public ColorScale( string name, RGBI data0, RGBI data1, RGBI data2, RGBI data3 ) {
			this.name = name;
			this.data = new RGBI[4];
			this.data[0] = data0;
			this.data[1] = data1;
			this.data[2] = data2;
			this.data[3] = data3;
		}

		public void ReadFrom( CSVReader reader ) {
			name = reader.ReadString();
			reader.SkipRow();
			reader.SkipRow();
			for ( int i=0; i<4; ++i ) {
				data[i].ReadFrom( reader );
			}
		}

		public void WriteTo( CSVWriter writer ) {
			writer.Write( name );
			writer.Write();
			writer.Write();
			writer.Write();
			writer.EndRow();

			writer.Write( "red" );
			writer.Write( "green" );
			writer.Write( "blue" );
			writer.Write( "index" );
			writer.EndRow();

			for ( int i=0; i<4; ++i ) {
				data[i].WriteTo( writer );
			}
		}

        public int Average() {
            int[] colors = Interpolate();
            return colors[colors.Length/2];
        }

        public int[] Interpolate() {
			return Interpolate( 1, 1, 1 );
		}

		public int[] Interpolate( double factor ) {
			return Interpolate( factor, factor, factor );
		}

		public int[] Interpolate( double redfactor, double greenfactor, double bluefactor ) {
			int[] shades = new int[64];
			double dr, dg, db, r, g, b;
			int startindex, endindex;

			for ( int i=-1; i<4; ++i ) {
				if ( i < 0 ) {
					dr = dg = db = 0;

					r = data[0].Red;
					g = data[0].Green;
					b = data[0].Blue;

					startindex = 0;
					endindex = data[0].Index;
				}
				else if ( i<3 ) {
					int di = (data[i+1].Index-data[i].Index);
					if ( di == 0 ) {
						dr = dg = db = 0;
					}
					else {
						dr = (data[i+1].Red-data[i].Red)/(data[i+1].Index-data[i].Index);
						dg = (data[i+1].Green-data[i].Green)/(data[i+1].Index-data[i].Index);
						db = (data[i+1].Blue-data[i].Blue)/(data[i+1].Index-data[i].Index);
					}

					r = data[i].Red;
					g = data[i].Green;
					b = data[i].Blue;

					startindex = Math.Min(Math.Max((int)data[i].Index, 0), 64);
                    endindex = Math.Min(Math.Max((int)data[i + 1].Index, 0), 64);
				}
				else {
					dr = 0; dg = 0; db = 0;

					r = data[3].Red;
					g = data[3].Green;
					b = data[3].Blue;

					startindex = data[3].Index;
					endindex = 64;
				}

				for ( int di=startindex; di<endindex; ++di ) {
					shades[di] = Shader( r*redfactor, g*greenfactor, b*bluefactor );

					r += dr;
					g += dg;
					b += db;
				}
			}

			return shades;
		}

		private int Shader( double r, double g, double b ) {
			return (((int)r) << 16) | (((int)g) << 8) | ((int)b);
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public RGBI[] Data {
			get { return data; }
			set { data = value; }
		}

        public RGBI this[int index] {
			get { return data[index]; }
			set { data[index] = value; }
		}

    }

	public struct RGBI {
		private byte index;
		private byte red;
		private byte green;
		private byte blue;

		public RGBI( int index, int red, int green, int blue ) {
			this.index = (byte)(index);
			this.red = (byte)(red);
			this.green = (byte)(green);
			this.blue = (byte)(blue);
		}

		public void ReadFrom( CSVReader reader ) {
			red = reader.ReadByte();
			green = reader.ReadByte();
			blue = reader.ReadByte();
			index = reader.ReadByte();
		}

		public void WriteTo( CSVWriter writer ) {
			writer.Write( red );
			writer.Write( green );
			writer.Write( blue );
			writer.Write( index );
			writer.EndRow();
		}

		public byte Red {
			get { double v = (((double)red)/256*0.6+0.5)*red*0.9; if ( v>255 ) v = 255; return (byte)v; }
		}

		public byte R {
			get { return red; }
			set { red = value; }
		}

		public byte Green {
			get { double v = (((double)green)/256*0.6+0.5)*green*0.9; if ( v>255 ) v = 255; return (byte)v; }
		}
	
		public byte G {
			get { return green; }
			set { green = value; }
		}
	
		public byte Blue {
			get { double v = (((double)blue)/256*0.6+0.5)*blue*0.9; if ( v>255 ) v = 255; return (byte)v; }
		}

		public byte B {
			get { return blue; }
			set { blue = value; }
		}

		public byte Index {
			get { return index; }
			set { index = value; }
		}

		public byte I {
			get { return index; }
			set { index = value; }
		}

        public Color Color {
            get { return Color.FromArgb(red, green, blue); }
        }
	}
}
