using System;
using System.IO;
using System.Reflection;
using EU2;
using EU2.Edit;
using EU2.Data;
using MapToolsLib;

namespace MapProvince
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Boot
	{
		private const string ToolDescription = "EU2 Map Province IO Tool";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
Console.WriteLine(MapToolsVersion.Version.GetVersionString(ToolDescription));
			Console.WriteLine( );

#if !DEBUG
			try {
#endif
				MapProvinceParsedArguments pargs = new MapProvinceParsedArguments( args );
				if ( pargs.Help ) {
					ShowHelp();
					return;
				}

				if ( pargs.Action == Action.None ) {
					Console.WriteLine( "No action specified. Please use the /I or /E directive. Use /? for more help." );
					return;
				}
				if ( pargs.Source == "" ) {
					Console.WriteLine( "No source file specified!" );
					return;
				}
				string source = pargs.Source; 

				if ( Path.GetExtension( source ) == "" ) {
					if ( pargs.Action == Action.ExportProvince )
						source = Path.ChangeExtension( source, "eu2map" );
					else if ( pargs.Action == Action.ImportProvince )
						source = Path.ChangeExtension( source, pargs.Mode == ExportMode.XML ? "xml" : "csv" );
				}
				source = Path.GetFullPath( source ); 

				// Check if source exists
				if ( !System.IO.File.Exists( source ) ) {
					Console.WriteLine( "The specified source file \"{0}\" does not exist.", Path.GetFileName( source ) );
					return;
				}

				string target = pargs.Target;
				if ( target == "" ) {
					if ( pargs.Action == Action.ExportProvince ) {
						target = String.Format( "{0}-provinces.{1}", Path.GetFileNameWithoutExtension( source ), pargs.Mode == ExportMode.XML ? "xml" : "csv" );
						Console.WriteLine( "No target file specified. Using default name: {0}", target );
					}
					else if ( pargs.Action == Action.ImportProvince ) {
						Console.WriteLine( "No target file specified!" );
						return;
					}
				}

				if ( Path.GetExtension( target ) == "" ) {
					if ( pargs.Action == Action.ExportProvince )
						target = Path.ChangeExtension( target, pargs.Mode == ExportMode.XML ? "xml" : "csv" );
					else if ( pargs.Action == Action.ImportProvince )
						target = Path.ChangeExtension( target, "eu2map" );
				}
				target = Path.GetFullPath( target ); 

				// Check if target exists
				if ( pargs.Action == Action.ExportProvince && System.IO.File.Exists( target ) && !pargs.Overwrite ) {
					Console.WriteLine( "Target file \"{0}\" exists. Specify the /O option to overwrite it.", Path.GetFileName( target ) );
					return;
				}
				else if ( pargs.Action == Action.ImportProvince && !System.IO.File.Exists( target ) ) {
					Console.WriteLine( "The specified target file \"{0}\" does not exist.", Path.GetFileName( target ) );
					return;
				}

				switch ( pargs.Action ) {
					case Action.ImportProvince:
						ImportProvince( source, target, pargs.Mode );
						break;
					case Action.ExportProvince:
						ExportProvince( source, target, pargs.Mode, !pargs.NoTOT );
						break;
				}
#if !DEBUG
			}
			catch( Exception e ) {
				MapToolsLib.Utils.LogError( e );
			}
#endif

			Console.WriteLine( );
		}

		public static void ImportProvince( string source, string target, ExportMode mode ) {
			Console.WriteLine( "Opening source file \"{0}\"...", Path.GetFileName( source ) );
			ProvinceList provinces = new ProvinceList();
			FileStream stream = null;
			try {
				stream = new FileStream( source, FileMode.Open, FileAccess.Read, FileShare.Read );
				if ( mode == ExportMode.XML ) {
					System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
					doc.Load( stream );
					provinces.ReadFrom( doc );
				}
				else {
					provinces.ReadFrom( new EU2.IO.CSVReader( stream ) );
				}
			}
			finally {
				if ( stream != null ) stream.Close();
			}

			Console.WriteLine( "Opening target file \"{0}\"...", Path.GetFileName( target ) );
			EU2.Edit.File file = new EU2.Edit.File();
			try {
				file.ReadFrom( target );
			}
			catch ( EU2.Edit.InvalidFileFormatException ) {
				Console.WriteLine( "The specified target file is not an EU2Map file, or is corrupt." );
				return;
			}

			file.Provinces = provinces;

			file.WriteTo( target );
			Console.WriteLine( "Import successful." );		
		}

		public static void ExportProvince( string source, string target, ExportMode mode, bool allowTOT ) {
			Console.WriteLine( "Opening source file \"{0}\"...", Path.GetFileName( source ) );

			EU2.Edit.File file = new EU2.Edit.File();
			try {
				file.ReadFrom( source );
			}
			catch ( EU2.Edit.InvalidFileFormatException ) {
				Console.WriteLine( "The specified source file is not an EU2Map file, or is corrupt." );
				return;
			}

			if ( file.Provinces == null ) {
				Console.WriteLine( "Error: The file does not contain province info." );
				return;
			}

			// -- Write the file
			Console.WriteLine( "Exporting to \"{0}\"...", Path.GetFileName( target ) );
			FileStream stream = null;
			try {
				stream = new FileStream( target, FileMode.Create, FileAccess.Write, FileShare.None );
				using ( GlobalConfigChange gcc = new GlobalConfigChange() ) {
					gcc.AllowTOTAndHREInProvinceList = allowTOT;
					if ( mode == ExportMode.XML ) {
						file.Provinces.WriteTo( new System.Xml.XmlTextWriter( stream, System.Text.Encoding.UTF8 ) );
					}
					else {
						file.Provinces.WriteTo( stream );
					}
				}
			}
			finally {
				if ( stream != null ) stream.Close();
			}

			Console.WriteLine( "Export successful." );
		}

		private static void ShowHelp() {
			Console.WriteLine( "Imports or exports province data from/to an EU2 map file." );
			Console.WriteLine( );
			Console.WriteLine( "{0} /I <source[.csv|.xml]> <destination[.eu2map]> [/XML|/CSV]", MapToolsVersion.Version.GetToolName().ToUpper() );
			Console.WriteLine( "{0} /E <source[.eu2map]> [<destination[.csv|.xml]>] [/O] [/XML|/CSV]", MapToolsVersion.Version.GetToolName().ToUpper() );
			Console.WriteLine( );
			Console.WriteLine( "  /I              Indicates that you're importing an image into a map file." );
			Console.WriteLine( "  /E              Indicates that you're exporting an image from a map file." );
			Console.WriteLine( "  <source>        Specifies the source file. This should be a PSD file for the /I and /Q actions," );
			Console.WriteLine( "                  and a EU2MAP file for the /E action." );
			Console.WriteLine( "  <destination>   Specifies the destination file. This should be a EU2MAP file for the /I action," );
			Console.WriteLine( "                  and a PSD file for the /E action." );
			Console.WriteLine( "                  If you omit the destination while exporting, one will be generated for you." );
			Console.WriteLine( "  /O              Allows overwriting an existing file when exporting." );
			Console.WriteLine( "  /XML            Specifies that the province file should be in XML format (regardless of import/export)." );
			Console.WriteLine( "  /CSV            Specifies that the province file should be in CSV format (regardless of import/export)." );
			Console.WriteLine( );
			Console.WriteLine( "If you omit the /XML or /CSV option, the program will choose XML by default when exporting. When importing, " );
			Console.WriteLine( "it will to figure out the file format itself." );
		}
	}
}
