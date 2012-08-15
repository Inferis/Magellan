using System;
using System.IO;
using System.Reflection;
using EU2.Map;
using EU2.Edit;
using MapToolsLib;

namespace MapInject
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Boot
	{
		private const string ToolDescription = "EU2 Map Injection Tool";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
            Console.WriteLine(MapToolsVersion.Version.GetVersionString(ToolDescription));
			Console.WriteLine( );

#if !DEBUG
			try {
#endif
				MapInjectParsedArguments pargs = new MapInjectParsedArguments( args );

				if ( pargs.Help ) {
					ShowHelp();
					return;
				}

				if ( pargs.Source == "" ) {
					Console.WriteLine( "No source file specified!" );
					return;
				}

				string source = pargs.Source;
				if ( Path.GetExtension( source ) == "" ) source = Path.ChangeExtension( source, "eu2map" );
				source = Path.GetFullPath( source ); 

				// Check if source exists
				if ( !System.IO.File.Exists( source ) ) {
					Console.WriteLine( "The specified source file \"{0}\" could not be found.", Path.GetFileName( source ) );
					return;
				}

				EU2.Install install;
				if ( pargs.DirectoryOverride.Length > 0 ) {
					string dir = Path.GetFullPath( pargs.DirectoryOverride ); 
					if ( !Directory.Exists( dir ) ) {
						Console.WriteLine( "The specified directory override \"{0}\" does not exist.", dir );
						return;
					}
					install = EU2.Install.FromPath( dir );
				}
				else 
					install = EU2.Install.FromRegistry();

				if ( install == null ) {
					Console.WriteLine( "Error: Can't get EU2 base directory (if using /D, check your path)." );
					return;
				}

				Console.WriteLine( "Opening source file \"{0}\"...", Path.GetFileName( source ) );

				EU2.Edit.File file = new EU2.Edit.File();
				file.ReadFrom( source );

				Console.WriteLine( "Regenerating boundboxes..." );
				file.BoundBoxes = new BoundBoxes( file.IDMap.CalculateBoundBoxes() );

				if ( pargs.RegenerateLightmaps > 0 ) {
					if ( file.Lightmap2 != null && file.Lightmap3 != null && pargs.RegenerateLightmaps < 2 ) {
						Console.WriteLine( "Lightmaps exists, not regenerating done." );
					}
					else {
						if ( pargs.RegenerateLightmaps >= 2 || file.Lightmap2 == null ) {
							Console.WriteLine( "Regenerating Lightmap2 and Lightmap3 from Lightmap1..." );
							Lightmap map2, map3;
							file.Lightmap1.Shrink( out map2, out map3 );
							file.Lightmap2 = map2;
							file.Lightmap3 = map3;
						}
						else {
							Console.WriteLine( "Regenerating Lightmap3 from Lightmap2..." );
							file.Lightmap3 = file.Lightmap2.Shrink();
						}
					}
				}

				Console.WriteLine( "Regenerating IDGrid..." );
				try {
					file.IDGrid = IDGrid.Build( file.IDMap );
				}
				catch ( EU2.RegionSizeOverflowException e ) {
					Console.WriteLine( "The map you are trying to inject is too complicated (More than 256 provinces in the area {0},{1}-{2},{3}). Please reduce complexity before proceeding.", e.Region.Left, e.Region.Top, e.Region.Right, e.Region.Bottom );
					return;
				}


				Console.WriteLine( "Regenerating Incognita Grid..." );
				file.IncognitaGrid = IncognitaGrid.Build( file.IDMap, file.Provinces, file.IDGrid );

				file.FileProcessing += new EU2.Edit.FileProcessingHandler( EU2File_WriteFileItemInject ); 
				file.Inject( install, new InjectParams( pargs.NoTOT ) );

				if ( pargs.SaveSource ) {
					file.FileProcessing -= new EU2.Edit.FileProcessingHandler( EU2File_WriteFileItemInject ); 
					file.FileProcessing += new EU2.Edit.FileProcessingHandler( EU2File_WriteFileItemSave ); 
					file.WriteTo( source );
				}

				Console.Write( "Injection successful." );
#if !DEBUG
			}
			catch( Exception e ) {
				MapToolsLib.Utils.LogError( e );
			}
#endif

			Console.WriteLine( );
		}

		private static void ShowHelp() {
			Console.WriteLine( "Injects new map data into the EU2 map (to the game files, from the intermediate format)." );
			Console.WriteLine( );
			Console.WriteLine( "MINJECT <source[.eu2map]> [/D:<directory>] [/L]" );
			Console.WriteLine( );
			Console.WriteLine( "  <destination>   Specifies the file to export to. The extension is automatically added if it is omitted." );
			Console.WriteLine( "  /D:<dir>        Specifies the base directory to put the result files. This should be the root EU2 directory." );
			Console.WriteLine( "  /L              Indicates to regenerate lightmap2 and lightmap3 from lightmap1 (downsizing)." );
			Console.WriteLine( "  /S              Indicates to save the source file after injecting the data (including all generated info)." );
			Console.WriteLine( "  /NOTOT          Don't include the ToT and HRE fields in province.csv. Use this for compatibility with older EU2 versions." );
			Console.WriteLine( );
			Console.WriteLine( "The injector will lookup the EU2 path through the registry." );
		}
	
		private static void EU2File_WriteFileItemInject( IFile file, EU2.Edit.FileProcessingEventArgs e ) {
			switch ( e.Action ) {
				case FileProcessingAction.Write:
					Console.WriteLine( "Injecting {0}...", e.Item );
					break;
				case FileProcessingAction.Generate:
					Console.WriteLine( "Generating {0}...", e.Item );
					break;
				case FileProcessingAction.Regenerate:
					Console.WriteLine( "Regenerating {0}...", e.Item );
					break;
				default:
					Console.WriteLine( "{1} {0}...", e.Item, e.Action.ToString() );
					break;
			}
		}

		private static void EU2File_WriteFileItemSave( IFile file, EU2.Edit.FileProcessingEventArgs e ) {
			switch ( e.Action ) {
				case FileProcessingAction.Write:
					Console.WriteLine( "Saving {0}...", e.Item );
					break;
				case FileProcessingAction.Generate:
					Console.WriteLine( "Generating {0}...", e.Item );
					break;
				case FileProcessingAction.Regenerate:
					Console.WriteLine( "Regenerating {0}...", e.Item );
					break;
				default:
					Console.WriteLine( "{1} {0}...", e.Item, e.Action.ToString() );
					break;
			}
		}
	}
}
