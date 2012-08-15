using System;

namespace MapCreate
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Boot
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			Console.WriteLine( "mextract - EU2 Map Extraction Tool - v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString() );
			Console.WriteLine( );

#if !DEBUG
			try {
#endif
				MapCreateParsedArguments pargs = new MapCreateParsedArguments( args );

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
				if ( File.Exists( target ) && !pargs.Overwrite ) {
					Console.WriteLine( "The specified target file \"{0}\" exists. Specify the /O option to overwrite it.", Path.GetFileName( target ) );
					return;
				}

				EU2.Install install;
				if ( pargs.DirectoryOverride.Length > 0 ) 
					install = EU2.Install.FromPath( pargs.DirectoryOverride );
				else 
					install = EU2.Install.FromRegistry();
			
				if ( install == null ) {
					Console.WriteLine( "Error: Can't get EU2 base directory (if using /D, check your path)." );
					return;
				}

				Console.WriteLine( "Extracting file from EU2 files to \"{0}\"...\nRemember: This can take a while.", Path.GetFileName( target ) );
				EU2.Edit.FileExtractOptions options = new EU2.Edit.FileExtractOptions( pargs.IncludeLightmaps, pargs.ForceGenerate, true );
				EU2.Edit.File.Extract( install, target, options, new EU2.Edit.File.WriteFileItemHandler( EU2File_WriteFileItem ) );
				Console.WriteLine( "Extract successful." );
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
			Console.WriteLine( "MEXTRACT <destination[.eu2map]> [/D:directory] [/O] [/1|/2|/3]" );
			Console.WriteLine( );
			Console.WriteLine( "  <destination>   Specifies the file to export to. The extension is automatically added if it is omitted." );
			Console.WriteLine( "  /O              Allows overwriting an existing file when extracting." );
			Console.WriteLine( "  /D:<dir>        Specifies the base directory to look for the files. This should be the root EU2 directory." );
			Console.WriteLine( "  /1              Indicates to only extract lightmap1." );
			Console.WriteLine( "  /2              Indicates to only extract lightmap1 and lightmap2." );
			Console.WriteLine( "  /3              Indicates to extract all lightmaps." );
			Console.WriteLine( "  /G              Forces ID tbl to be generated from lightmap1, even if the program thinks it shouldn't be." );
			Console.WriteLine( );
			Console.WriteLine( "The extractor will lookup the EU2 path through the registry." );
			Console.WriteLine( "By default, the extractor will extract all 3 lightmaps." );
		}
	}
}
