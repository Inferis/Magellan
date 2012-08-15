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
	/// Summary description for ImportImage.
	/// </summary>
	public sealed class ImportImage
	{
		public static void Run( string source, string target, int regen, Point relocate, bool nowrite, PngLevel pngLevel, bool tolerant, bool checkIdgrid ) {
            if (pngLevel != PngLevel.All && pngLevel != PngLevel.None) {
                Console.WriteLine("Can't use PNG specifier with import. Use /p instead of /p:<spec>.");
                return;
            }

			if ( nowrite ) Console.WriteLine( "*** Simulation ***\n" );

			// Open the PSD, and check if it's valid.
			PSD.File psd = QueryImage.Run( source );
			if ( !CheckPSD( psd ) ) return;
			Console.WriteLine( );

			// Get the mapinfo from the PSD
			MapInfo info = MapInfo.FromBitmap( psd.Layers[Boot.MapInfoLayerName].Image );
			if ( !relocate.IsEmpty ) {
				relocate = new Point( Lightmap.FitToGrid( relocate.X ), Lightmap.FitToGrid( relocate.Y ) );
				Console.WriteLine( "Image will be relocated from {0},{1} to {2},{3}...", info.X, info.Y, relocate.X, relocate.Y );
				info.Location = relocate;
			}

			// Extract the 3 buffers from the PSD
			int[] colorbuffer, idbuffer, borderbuffer;
			ImportImage.GetBuffers( psd, out colorbuffer, out idbuffer, out borderbuffer );

			// Remove ref to PSD so it can get garbagecollected
			psd = null;

			// Get the convertor 
			IIDConvertor convertor = GetConvertor( info );
			if ( convertor == null ) return;

			IIDConvertor tconvertor = null;
			if ( convertor is Inferis2IDConvertor && tolerant ) tconvertor = new InferisProxIDConvertor(); 

			if ( tconvertor != null ) 
				Console.WriteLine( "Using tolerant ID conversion." );

			// Now, open the map file
			Console.WriteLine( "Opening target file \"{0}\"...", Path.GetFileName( target ) );
			EU2.Edit.File file = new EU2.Edit.File();
			file.ReadFrom( target );

			int[] cidbuffer = null;
			if ( info.LightmapZoom == 0 ) {
				Console.Write( "Checking ID map... " );
				// Use another buffer, since we need to remember the original for the image import later on.
				cidbuffer = new int[idbuffer.Length];
				int invalid = 0;
				for ( int i=0; i<idbuffer.Length; ++i ) {
					cidbuffer[i] = convertor.ConvertRGB( idbuffer[i] );
					if ( cidbuffer[i] < 0 || cidbuffer[i] > Province.Count ) {
						if ( tconvertor != null ) cidbuffer[i] = tconvertor.ConvertRGB( idbuffer[i] );
						if ( cidbuffer[i] < 0 || cidbuffer[i] > Province.Count ) {
							Console.WriteLine( "Error: Found invalid ID {2} at ({0},{1}). RGB value = {3:X6}", i % info.Width, (int)(i/info.Width), cidbuffer[i], (idbuffer[i] & 0xFFFFFF) );
							if ( invalid++ > 10 ) {
								Console.WriteLine( "Error: Too many invalid IDs found... Stopping." );
								break;
							}
						}
					}
					
					if ( cidbuffer[i] == Province.TerraIncognitaID ) cidbuffer[i] = Province.MaxValue;
				}

				if ( invalid > 0 ) {
					Console.WriteLine( "Error: Invalid IDs encountered. Cannot import. Please fix." );
					return;
				}
			}

			// If needed, write out test PNGs of the extracted layers
            if (pngLevel != PngLevel.None)
                WriteTestPNGs(colorbuffer, idbuffer, borderbuffer, pngLevel, info);

			// and get the correct Lightmap
			Lightmap map = null;
			switch ( info.LightmapZoom ) {
				case 0: 
					map = file.Lightmap1; break;
				case 1: 
					map = file.Lightmap2; break;
				case 2: 
					map = file.Lightmap3; break;
			}

			if ( map == null ) {
				Console.WriteLine( "Error: The source image was exported from lightmap{0}, but it no longer exists in the target file." );
				return;
			}

			if ( map.Zoom == 0 ) {
				// Then check if the bitmap differs...
				ushort[] checkbuffer = file.IDMap.ExportBitmapBuffer( info.Bounds );
				bool mismatch = false;
				System.Diagnostics.Debug.Assert( checkbuffer.Length == cidbuffer.Length, "ID buffers have non-matching size" );
				for ( int i=0; !mismatch && i<cidbuffer.Length; ++i ) {
					if ( cidbuffer[i] != checkbuffer[i] ) mismatch = true;
				}

				if ( mismatch ) {
					if ( map.Zoom > 0 ) {
						Console.WriteLine( "Changes to ID map only supported for lightmap 1." );
						return;
					}
					Console.WriteLine( "Mismatch found, updating the ID map." );
					file.IDMap.ImportBitmapBuffer( info.Bounds, cidbuffer );
				}
				else 
					Console.WriteLine( " No update needed to the ID map." );

				ushort[] affectedIDs = null;
				bool doBoundboxes = false;
				if ( regen > 0 ) {
					// regenerate certain files first...
					if ( regen > 2 ) {
						Console.WriteLine( "Regenerating adjacenties (complete)..." );
						file.AdjacencyTable = file.IDMap.BuildAdjacencyTable( file.Provinces );

						// This marks all ids as affected.
						affectedIDs = new ushort[Province.Count];
						for ( ushort i=0; i<Province.Count; ++i ) affectedIDs[i] = i;

						// Force regen of boundboxes
						doBoundboxes = true;
					}
					else if ( mismatch || regen > 1 ) {
						Console.Write( "Regenerating adjacenties (quick) " );
						if ( regen > 1 ) Console.Write( "(forced)" );
						Console.WriteLine( "... " );

						// Rebuild part and note the affected Ids.
						affectedIDs = file.IDMap.BuildAdjacencyTable( file.AdjacencyTable, info.Bounds, file.Provinces );

						// Force regen of boundboxes
						doBoundboxes = true;
					}
				}

				// reattach
				map.Attach( file.Provinces, file.AdjacencyTable, file.IDMap );

				if ( doBoundboxes ) {
					Console.WriteLine( "Regenerating boundboxes..." );
					file.BoundBoxes = new BoundBoxes( file.IDMap.CalculateBoundBoxes() );
				}

				if ( affectedIDs != null && affectedIDs.Length > 0 ) {
					Rectangle[] boxes = file.BoundBoxes.GetBoxes( affectedIDs );
					int[] indexes = map.GetBlockIndexes( boxes );
					int[] indexes2 = map.GetBlockIndexes( info.Bounds );
					
					int total = indexes.Length;
					for ( int i=0; i<indexes.Length; ++i ) {
						for ( int e=0; e<indexes2.Length; ++e ) {
							if ( indexes[i] == indexes2[e] ) {
								indexes[i] = -1;
								total--;
								break;
							}
						}
					}

					Console.WriteLine( "Decompressing blocks of affected provinces outside of modified area." );
					Console.Write( "{0} provinces affected, {1} blocks to recompress... ", affectedIDs.Length, total );
					if ( total > 0 ) {
						indexes2 = new int[total];
						total = 0;
						for ( int i=0; i<indexes.Length; ++i ) {
							if ( indexes[i] > 0 ) indexes2[total++] = indexes[i];
						}
						map.Recompress( indexes2 );
					}

					if ( nowrite && checkIdgrid ) {
						Console.Write( "Checking for IDGrid complexity..." );
						Rectangle errorArea;
						if ( IDGrid.CheckAreaForOverflow( file.IDMap, info.Bounds, out errorArea ) ) {
							Console.WriteLine(" ok!" );
						}
						else {
							Console.WriteLine( " too complex (error in region {0},{1}-{2},{3}).", errorArea.Left, errorArea.Top, errorArea.Right, errorArea.Bottom);
						}
					}
					Console.WriteLine( "Done." );
				}
			}
			else {
				Console.Write( "Scaling down ID map (file changes will not be applied at this zoom level)..." );
				ushort[] tmpbuffer = MapToolsLib.Utils.ScaleIDBuffer( file.IDMap.ExportBitmapGrid( map.CoordMap.ZoomedToActual( info.Bounds ) ), map.Zoom );
				idbuffer = new int[tmpbuffer.Length];
				for ( int i=0; i<idbuffer.Length; ++i ) {
					if ( tmpbuffer[i] >= Province.Count ) tmpbuffer[i] = Province.TerraIncognitaID;
					idbuffer[i] = (tconvertor == null ? convertor : tconvertor).ConvertID( tmpbuffer[i] );
				}
				Console.WriteLine( " Done." );
			}

			Console.WriteLine( "Importing image..." );
			RawImage rawimg = new IOShader( tconvertor == null ? convertor : tconvertor ).Unshade32( map.Zoom, info.Bounds, colorbuffer, idbuffer, borderbuffer );
			map.EncodeImage( rawimg );

			if ( !nowrite ) {
				file.WriteTo( target );
				Console.Write( "Import successful." );
			}
			else {
				try {
					file.WriteTo( System.IO.FileStream.Null );	
				}
				catch ( Exception e ) {
					Console.Write( "There's an error when saving the target file." );
					throw e;
				}
				Console.Write( "The import process was tested successfully. You can safely do a real import with the same source and target." );
			}
		}

		public static bool CheckPSD( PSD.File psd ) {
			if ( psd == null ) return false;

			if ( !psd.Layers.Contains( Boot.ShadingLayerName ) ) {
				Console.WriteLine( "Error: The shading layer is missing." );
				return false;
			}
			if ( !psd.Layers.Contains( Boot.IDLayerName ) ) {
				Console.WriteLine( "Error: The id layer is missing." );
				return false;
			}
			if ( !psd.Layers.Contains( Boot.BorderLayerName ) ) {
				Console.WriteLine( "Error: The borders layer is missing." );
				return false;
			}

			return true;
		}

		public static void GetBuffers( PSD.File psd, out int[] colorbuffer, out int[] idbuffer, out int[] borderbuffer ) {
			psd.Layers.HideAll();
			psd.Layers[Boot.ShadingLayerName].Visible = true;
			psd.Layers[Boot.ShadingLayerName].OpacityF = 1;
			psd.Layers[Boot.ShadingLayerName].Mode = PSD.LayerMode.Normal;
			colorbuffer = Visualiser.CreateBuffer32( psd.Layers.CreateFlattenedImage() );

			psd.Layers[Boot.ShadingLayerName].Visible = false;
			psd.Layers[Boot.IDLayerName].Visible = true;
			psd.Layers[Boot.IDLayerName].OpacityF = 1;
			psd.Layers[Boot.IDLayerName].Mode = PSD.LayerMode.Normal;
			idbuffer = Visualiser.CreateBuffer32( psd.Layers.CreateFlattenedImage() );

			psd.Layers[Boot.IDLayerName].Visible = false;
			psd.Layers[Boot.BorderLayerName].Visible = true;
			psd.Layers[Boot.BorderLayerName].OpacityF = 1;
			psd.Layers[Boot.BorderLayerName].Mode = PSD.LayerMode.Normal;
			borderbuffer = Visualiser.CreateBuffer32( psd.Layers.CreateFlattenedImage() );
		}

		public static IIDConvertor GetConvertor( MapInfo info ) {
			IIDConvertor convertor = null;
			if ( info.IDConvertor == null ) {
				Console.WriteLine( "Error: The source contains an invalid idconvertor reference." );
				return null;
			}
			else {
				try {
					convertor = (IIDConvertor)info.IDConvertor.GetConstructor( Type.EmptyTypes ).Invoke( null );
				} 
				catch {
					Console.WriteLine( "Warning: The source contains an valid idconvertor reference, but it could not be instantiated." );
					return null;
				}
				Console.WriteLine( "Using convertor \"{0}\".", convertor.ToString() );
			}

			return convertor;
		}

		public static void WriteTestPNGs( int[] colorbuffer, int[] idbuffer, int[] borderbuffer, PngLevel pngLevel, MapInfo info ) {
			Console.Write( "Writing buffer check PNGs...");
            if ((pngLevel & PngLevel.Shading)>0) {
                Console.Write("Colorbuffer... ");
                Visualiser.CreateImage32(colorbuffer, info.Size).Save("test-Shading.png", ImageFormat.Png);
            }
            if ((pngLevel & PngLevel.IDs)>0) {
                Console.Write("IDbuffer... ");
                Visualiser.CreateImage32(idbuffer, info.Size).Save("test-IDs.png", ImageFormat.Png);
            }
            if ((pngLevel & PngLevel.Borders)>0) {
                Console.Write("Borderbuffer... ");
                Visualiser.CreateImage32(borderbuffer, info.Size).Save("test-Borders.png", ImageFormat.Png);
            }
			Console.WriteLine( "Done!" );
		}
	}
}
