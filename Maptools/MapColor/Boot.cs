using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using MapToolsLib;

namespace MapColor
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Boot
	{
		private const string ToolDescription = "EU2 Map Colour Conversion Tool";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
Console.WriteLine(MapToolsVersion.Version.GetVersionString(ToolDescription));
			Console.WriteLine( );		

			try {
				MapColorParsedArguments pargs = new MapColorParsedArguments( args );
				if ( pargs.Help ) {
					ShowHelp();
					return;
				}

				if ( pargs.Convertor == "?" ) {
					// Show convertor names
					Console.WriteLine( "Supported ID Convertors:" );
					IEnumerator iter = GetConvertorInstances().GetEnumerator();
					while ( iter.MoveNext() ) {
						Console.WriteLine( "* " + iter.Current.ToString() + (iter.Current is Inferis2IDConvertor ? " (default)" : "") );
					}
					return;
				}
				
				if ( pargs.Color >= 0 || pargs.ID >= 0 || pargs.MakeMap.Length > 0 ) {
					IIDConvertor convertor = new Inferis2IDConvertor();
					if ( pargs.Convertor.Length > 0 ) {
						IEnumerator iter = GetConvertorInstances().GetEnumerator();
						while ( iter.MoveNext() ) {
							if ( iter.Current.ToString().ToLower() == pargs.Convertor.ToLower() ) {
								convertor = (IIDConvertor)iter.Current;
								break;
							}
						}
					}
					Console.WriteLine( "Using convertor \"" + convertor.ToString() + "\"." );
					Console.WriteLine( );					
					
					if ( pargs.MakeMap.Length > 0 ) {
						const int itemWidth = 40;
						const int itemHeight = 12;
						const int rowLen = 32;

						Console.Write( "Creating colour map... " );
						Bitmap map = new Bitmap( itemWidth * rowLen, itemHeight * (int)Math.Ceiling((float)EU2.Data.Province.InternalCount/(float)rowLen) );
						using ( Graphics g = Graphics.FromImage( map ) ) {
							g.Clear( Color.White );
							Font f = new Font( "Small Fonts", 7 );
							g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
							int x = 0, y = 0;
							for ( int i=0; i<EU2.Data.Province.InternalCount; ++i ) {
								using ( Brush b = new SolidBrush( Color.FromArgb( (0xFF << 24) | convertor.ConvertID( (ushort)i ) ) ) ) {
									g.FillRectangle( b, x+1, y+1, itemHeight-2, itemHeight-2 );	
								}
								g.DrawString( i.ToString(), f, Brushes.Black, x+itemHeight, y );

								y += itemHeight;
								if ( y >= map.Height ) {
									y = 0;
									x += itemWidth;
								}
							}
							f.Dispose();
						}

						string file = pargs.MakeMap;
						file = System.IO.Path.GetFullPath( System.IO.Path.GetFileNameWithoutExtension( file ) + ".png" );
						map.Save( file, System.Drawing.Imaging.ImageFormat.Png );
						Console.WriteLine( "Done. Saved to '{0}'.", System.IO.Path.GetFileName( file ) ); 
					}
					else {
						if ( pargs.Color >= 0 ) {
							int id = convertor.ConvertRGB( pargs.Color );
							Color c = Color.FromArgb( pargs.Color );
							convertor.ConvertRGB( pargs.Color );

							Console.WriteLine( "RGB->ID: {0:X6}h ({1:X2}:{2:X2}:{3:X2}) -> {4}", pargs.Color, c.R, c.G, c.B, id );
						}

						if ( pargs.ID >= 0 ) {
							if ( pargs.ID< ushort.MinValue || pargs.ID > ushort.MaxValue ) {
								Console.WriteLine( "ID->RGB: ID value " + pargs.ID + " is out of range." );
							}
							else {
								int rgb = convertor.ConvertID( (ushort)pargs.ID ) & 0xFFFFFF; // SKip alpha 
								Color c = Color.FromArgb( rgb );


								Console.WriteLine( "ID->RGB: {4} -> {0:X6}h ({1:X2}:{2:X2}:{3:X2})", rgb, c.R, c.G, c.B, pargs.ID );
							}
						}
						Console.WriteLine();
						Console.WriteLine( "Remember: Colour values are expressed in hexadecimal formatting." );
					}
				}
				else {
					// Show form
					Console.WriteLine( "Running interactive tool..." );
					Application.EnableVisualStyles();
					Application.Run(new ColorForm());
				}
			}
			catch( Exception e ) {
				MapToolsLib.Utils.LogError( e );
			}

			Console.WriteLine( );
		}

		private static void ShowHelp() {
			Console.WriteLine( "Conversion tool to convert ids to RGB values (and vice versa)." );
			Console.WriteLine( );
			Console.WriteLine( "MCOLOR [/RGB:<colour|red,green,blue>] [/ID:<id>] [/C:<name>]" );
			Console.WriteLine( );
			Console.WriteLine( "  /RGB:<colour>   Specifies a colour value to be converted to an ID, encoded as a 4byte integer value," );
			Console.WriteLine( "                  using RGB order (blue in the lowest 8bits, green in the next 8 bits and red in the top 8 bits. " );
			Console.WriteLine( "                  The upper 8 bits are unused and will not be considered when converting." );
			Console.WriteLine( "  /RGB:<r,g,b>    Specifies a colour value to be convert to and ID. Each colour component is expressed seperately," );
			Console.WriteLine( "                  separated by commas. Each value should be expressed in hexadecimal format, ranging from 00 to FF." );
			Console.WriteLine( "  /ID:<id>        Specifies the ID to convert. This should be in the range 0 to 1615." );
			Console.WriteLine( "  /C:<name>       Specifies what IDConvertor to use. If omitted, it will use \"Inferis2\". " );
			Console.WriteLine( "                  If you specify a \"?\" as the name, the program will show a list of possible convertor names." );
			Console.WriteLine( ); 
			Console.WriteLine( "Starting the program without parameters will show a GUI where you can do multiple conversions." );
		}
		
		private static IIDConvertor[] GetConvertorInstances() {
			Assembly source = Assembly.GetAssembly(typeof(IIDConvertor));
			if ( source == null ) return new IIDConvertor[] { new Inferis2IDConvertor() };

			// Look for IIDConvertor classes
			ArrayList list = new ArrayList();
			IEnumerator iter = source.GetExportedTypes().GetEnumerator();
			while ( iter.MoveNext() ) {
				if ( ((Type)iter.Current).GetInterface( typeof(IIDConvertor).Name ) != null ) {
					list.Add( System.Activator.CreateInstance(((Type)iter.Current)) );
				}
			}

			// Assign result
			IIDConvertor[] result = new IIDConvertor[list.Count];
			list.TrimToSize();
			list.CopyTo( result, 0 );
			return result;
		}
	}
}
