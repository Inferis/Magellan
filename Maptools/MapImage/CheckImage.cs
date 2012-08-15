using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using EU2;
using EU2.Edit;
using EU2.Data;
using EU2.Map;
using EU2.Map.Codec;
using EU2.Map.Drawing;
using MapToolsLib;

namespace MapImage
{
	/// <summary>
	/// Summary description for CheckImage.
	/// </summary>
	public sealed class CheckImage
	{
		public static void Run( string source, PngLevel pngLevel, int checkId, bool tolerant ) {
			PSD.File psd = QueryImage.Run( source );
			if ( !ImportImage.CheckPSD( psd ) ) return; 

			MapInfo info = MapInfo.FromBitmap( psd.Layers[Boot.MapInfoLayerName].Image );

			// Extract the 3 buffers from the PSD
			int[] colorbuffer, idbuffer, borderbuffer;
			ImportImage.GetBuffers( psd, out colorbuffer, out idbuffer, out borderbuffer );

			// Get the convertor 
			IIDConvertor convertor = ImportImage.GetConvertor( info );
			if ( convertor == null ) return;

			IIDConvertor tconvertor = null;
			if ( convertor is Inferis2IDConvertor && tolerant ) tconvertor = new InferisProxIDConvertor(); 

			if ( tconvertor != null ) 
				Console.WriteLine( "Using tolerant ID conversion." );
																   
			// If needed, write out test PNGs of the extracted layers
			if ( pngLevel != PngLevel.None ) ImportImage.WriteTestPNGs( colorbuffer, idbuffer, borderbuffer, pngLevel, info );

			if ( info.LightmapZoom == 0 ) {
				Console.Write( "Checking ID map... " );
				// Use another buffer, since we need to remember the original for the image import later on.
				int[] cidbuffer = new int[idbuffer.Length];

				if ( checkId >= 0 && checkId <= Province.Count ) {
					Point[] tagged = new Point[idbuffer.Length];
					int t = 0;
					for ( int i=0; i<idbuffer.Length; ++i ) {
						cidbuffer[i] = convertor.ConvertRGB( idbuffer[i] );
						if ( cidbuffer[i] == checkId ) {
							tagged[t++] = new Point( i%info.Width, i/info.Width );
						}
					}

					Console.WriteLine( "Done." );
					Console.WriteLine();

					Console.WriteLine( "ID {0} was found at the following locations in the file:", checkId );
					for ( int i=0; i<t; ++i ) {
						Console.WriteLine( "* {0},{1}", tagged[i].X, tagged[i].Y ); 
					}
				}
				else {
					ushort[] tagged = new ushort[Province.InternalCount];
					int invalid = 0;
					for ( int i=0; i<idbuffer.Length; ++i ) {
						cidbuffer[i] = convertor.ConvertRGB( idbuffer[i] );
						if ( cidbuffer[i] < 0 || cidbuffer[i] > Province.Count ) {
							if ( tconvertor != null ) cidbuffer[i] = tconvertor.ConvertRGB( idbuffer[i] );
							if ( cidbuffer[i] < 0 || cidbuffer[i] > Province.Count ) {
								if ( invalid++ == 0 ) Console.WriteLine();
								Console.WriteLine( "Error: Found invalid ID {2} at ({0},{1}). RGB value = {3:X6}", i % info.Width, (int)(i/info.Width), cidbuffer[i], (idbuffer[i] & 0xFFFFFF) );
							}
							else {
								tagged[cidbuffer[i]]++;
							}
						}
						else {
							tagged[cidbuffer[i]]++;
						}

					}

					if ( invalid > 0 ) {
						Console.WriteLine( "Error: Invalid IDs encountered. Cannot import. Please fix." );
						return;
					}

					Console.WriteLine( "Done." );
					Console.WriteLine();

					Console.WriteLine( "The IDs found in this file are:" );
					bool first = true;
					int avgtot = 0, avgcnt = 0;
					for ( int i=0; i<tagged.Length; ++i ) {
						if ( tagged[i] > 0 ) {
							if ( !first ) Console.Write( ", " );
							Console.Write( "{0} ({1})", i, tagged[i] );
							first = false;
							avgtot += tagged[i];
							avgcnt++;
						}
					}
					Console.WriteLine();
					Console.WriteLine();
					avgtot = (avgtot/avgcnt)/10;

					Console.WriteLine( "Possible stray IDs (average = {0}):", avgtot*10 );
					for ( int i=0; i<tagged.Length; ++i ) {
						if ( tagged[i] > 0 && tagged[i] < avgtot ) {
							Console.WriteLine( "* {0} ({1}x)", i, tagged[i] );
						}
					}
					Console.WriteLine();
				}

			}
			else {
				Console.WriteLine( "This is not a lightmap1 exported file: no id check is necessary." );
			}

			Console.WriteLine( "The source file checked out okay. You should probably do a /SI run now before importing." );
		}

	}
}
