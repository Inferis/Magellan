using System;
using System.IO;
using System.Reflection;
using EU2;
using EU2.Map;
using EU2.Data;
using EU2.Edit;
using EU2.Map.Drawing;
using MapToolsLib;

namespace MapExtract
{
	/// <summary>
	/// Summary description for Boot.
	/// </summary>
	class Boot
	{
		private const string ToolDescription = "EU2 Map Extraction Tool";

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
				MapExtractParsedArguments pargs = new MapExtractParsedArguments( args );

				if ( pargs.Help ) {
					ShowHelp();
					return;
				}

				if ( pargs.Target == "" ) {
					Console.WriteLine( "No target file specified!" );
					return;
				}

				string target = pargs.Target;
				if ( Path.GetExtension( target ) == "" ) target = Path.ChangeExtension( target, "eu2map" );
				target = Path.GetFullPath( target ); 

				// Check if target exists
				if ( System.IO.File.Exists( target ) && !pargs.Overwrite ) {
					Console.WriteLine( "The specified target file \"{0}\" exists. Specify the /O option to overwrite it.", Path.GetFileName( target ) );
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
			
				if ( pargs.Create ) {
					Console.WriteLine( "Creating empty map file \"{0}\"...", Path.GetFileName( target ) );

					EU2.Edit.File file = new EU2.Edit.File();
					file.Lightmap1 = Lightmap.CreateEmpty( 0 );
					file.Provinces = new ProvinceList();
					file.AdjacencyTable = new AdjacencyTable();
					file.IDMap = new IDMap();

					FileStream stream = null;
					if ( install != null && System.IO.File.Exists( install.GetMapFile( "colorscales.csv" ) ) ) {
						stream = new FileStream( install.GetMapFile( "colorscales.csv" ), FileMode.Open, FileAccess.Read, FileShare.Read );
						try {
							file.ColorScales = new ColorScales( new EU2.IO.CSVReader( stream ) );
						}
						finally {
							if ( stream != null ) stream.Close();
						}
					}

					file.FileProcessing += new FileProcessingHandler( EU2File_WriteFileItem );
					file.WriteTo( target );
					Console.WriteLine( "Extract successful." );
				}
				else {
					if ( install == null ) {
						Console.WriteLine( "Error: Can't get EU2 base directory (if using /D, check your path)." );
						return;
					}

					Console.WriteLine( "Extracting file from EU2 files to \"{0}\"...\nRemember: This can take a while.", Path.GetFileName( target ) );
					EU2.Edit.FileExtractOptions options = new EU2.Edit.FileExtractOptions( pargs.IncludeLightmaps, pargs.ForceGenerate, true );
					EU2.Edit.File.Extract( install, target, options, new EU2.Edit.FileProcessingHandler( EU2File_WriteFileItem ) );
					Console.WriteLine( "Extract successful." );
				}
#if !DEBUG
			}
			catch( Exception e ) {
				MapToolsLib.Utils.LogError( e );
			}
#endif
			Console.WriteLine( );
		}

		private static void ShowHelp() {
			Console.WriteLine( "Extracts the EU2 map from the game files into the intermediate format." );
			Console.WriteLine( );
			Console.WriteLine( "MEXTRACT <destination[.eu2map]> [/D:directory] [/C] [/O] [/1|/2|/3]" );
			Console.WriteLine( );
			Console.WriteLine( "  <destination>   Specifies the file to export to. The extension is automatically added if it is omitted." );
			Console.WriteLine( "  /O              Allows overwriting an existing file when extracting." );
			Console.WriteLine( "  /D:<dir>        Specifies the base directory to look for the files. This should be the root EU2 directory." );
			Console.WriteLine( "  /1              Indicates to only extract lightmap1." );
			Console.WriteLine( "  /2              Indicates to only extract lightmap1 and lightmap2." );
			Console.WriteLine( "  /3              Indicates to extract all lightmaps." );
			Console.WriteLine( "  /G              Forces ID tbl to be generated from lightmap1, even if the program thinks it shouldn't be." );
			Console.WriteLine( "  /C              Create an empty eu2map file (no actual extraction is done)." );
			Console.WriteLine( );
			Console.WriteLine( "The extractor will lookup the EU2 path through the registry." );
			Console.WriteLine( "By default, the extractor will extract all 3 lightmaps." );
		}
		
		private static void EU2File_WriteFileItem( IFile file, EU2.Edit.FileProcessingEventArgs e ) {
			switch ( e.Action ) {
				case FileProcessingAction.Write:
					Console.WriteLine( "Writing {0}...", e.Item );
					break;
				case FileProcessingAction.Generate:
					Console.WriteLine( "Generating {0}...", e.Item );
					break;
				case FileProcessingAction.Regenerate:
					Console.WriteLine( "Regenerating {0}...", e.Item );
					break;
				case FileProcessingAction.Fix:
					Console.WriteLine( "Fixing {0}...", e.Item );
					break;
				default:
					Console.WriteLine( "{1} {0}...", e.Item, e.Action.ToString() );
					break;
			}
		}
	}
}
