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
	/// Summary description for Class1.
	/// </summary>
	sealed class Boot {
		public const string MapInfoLayerName = "MapInfo ** DON'T EDIT!";
		public const string ShadingLayerName = "Shading";
		public const string IDLayerName = "IDs";
		public const string BorderLayerName = "Borders";

		private const string ToolDescription = "EU2 Map Image IO Tool";

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
			MapImageParsedArguments pargs = new MapImageParsedArguments( args );
			if ( pargs.Help ) {
				ShowHelp();
				return;
			}

			if ( pargs.Action == Action.None ) {
				Console.WriteLine( "No action specified. Please use the /I, /SI, /C, /Q or /E directive. Use /? for more help." );
				return;
			}
			if ( pargs.Source == "" ) {
				Console.WriteLine( "No source file specified!" );
				return;
			}
			string source = pargs.Source; 

			if ( Path.GetExtension( source ) == "" ) {
				if ( pargs.Action == Action.ExportImage )
					source = Path.ChangeExtension( source, "eu2map" );
				else if ( pargs.Action == Action.ImportImage || pargs.Action == Action.QueryImage || pargs.Action == Action.CheckImage )
					source = Path.ChangeExtension( source, "psd" );
			}
			source = Path.GetFullPath( source ); 

			// Check if source exists
			if ( !System.IO.File.Exists( source ) ) {
				Console.WriteLine( "The specified source file \"{0}\" does not exist.", Path.GetFileName( source ) );
				return;
			}

			string target = pargs.Target;
			if ( target == "" ) {
				if ( pargs.Action == Action.ExportImage ) {
					target = String.Format( "eu2img{4}{0}{1}{2}{3}.{5}", pargs.Region.X, pargs.Region.Y, pargs.Region.Width, pargs.Region.Height, pargs.UseLightmap, pargs.PngLevel != PngLevel.None ? "png" : "psd" );
					Console.WriteLine( "No target file specified. Using default name: {0}", target );
				}
				else if ( pargs.Action == Action.ImportImage ) {
					Console.WriteLine( "No target file specified!" );
					return;
				}
			}

			if (  pargs.Action != Action.QueryImage && pargs.Action != Action.CheckImage ) {
				if ( Path.GetExtension( target ) == "" ) {
					if ( pargs.Action == Action.ExportImage )
                        target = Path.ChangeExtension(target, pargs.PngLevel != PngLevel.None ? "png" : "psd");
					else if ( pargs.Action == Action.ImportImage )
						target = Path.ChangeExtension( target, "eu2map" );
				}
				target = Path.GetFullPath( target ); 
			}

			// Check if target exists
			if ( pargs.Action == Action.ExportImage && System.IO.File.Exists( target ) && !pargs.Overwrite ) {
				Console.WriteLine( "Target file \"{0}\" exists. Specify the /O option to overwrite it.", Path.GetFileName( target ) );
				return;
			}
			else if ( pargs.Action == Action.ImportImage && !System.IO.File.Exists( target ) ) {
				Console.WriteLine( "The specified target file \"{0}\" does not exist.", Path.GetFileName( target ) );
				return;
			}

			switch ( pargs.Action ) {
				case Action.ImportImage:
                    ImportImage.Run(source, target, pargs.Regenerate, pargs.Relocate, pargs.Simulate, pargs.PngLevel, pargs.Tolerant, pargs.CheckIDGrid);
					break;
				case Action.ExportImage:
					ExportImage.Run( source, target, pargs.Region, pargs.RegionMode, pargs.UseLightmap, pargs.PngLevel, pargs.Direct );
					break;
				case Action.QueryImage:
					QueryImage.Run( source );
					break;
				case Action.CheckImage:
					CheckImage.Run( source, pargs.PngLevel, pargs.CheckID, pargs.Tolerant );
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

		public static IIDConvertor DefaultConvertor {
			get { return defaultConvertor; }
		}
		
		private static void ShowHelp() {
            Console.WriteLine("Imports, exports or queries an EU2 map image.");
            Console.WriteLine();
            Console.WriteLine("MIMAGE /I <source[.psd]> <destination[.eu2map]> [/1|/2|/3] [/G:<level>] [/P] [/PEDANTIC]");
            Console.WriteLine("MIMAGE /SI <source[.psd]> <destination[.eu2map]> [/1|/2|/3] [/G:<level>] [/P] [/PEDANTIC]");
            Console.WriteLine("MIMAGE /C <source[.psd]> [/P] [/PEDANTIC]");
            Console.WriteLine("MIMAGE /E <source[.eu2map]> [<destination[.psd]>] [/R:<x,y,width,height>] [/1|/2|/3] [/O] [/P]");
            Console.WriteLine("MIMAGE /Q <source[.psd]> <destination[.eu2map]>");
            Console.WriteLine();
            Console.WriteLine("  /I              Indicates that you're importing an image into a map file.");
            Console.WriteLine("  /E              Indicates that you're exporting an image from a map file.");
            Console.WriteLine("  /Q              Indicates that you're querying an image file.");
            Console.WriteLine("  /SI             Indicates that you're simulating an import. This is identical to /I, but ");
            Console.WriteLine("                  the target file will not be modified, even if something goes wrong.");
            Console.WriteLine("  /C              Indicates that you're checking a source image. This checks if the psd is correctly");
            Console.WriteLine("                  formatted, and if the idmap contains correct values. ");
            Console.WriteLine("  <source>        Specifies the source file. This should be a PSD file for the /I and /Q actions,");
            Console.WriteLine("                  and a EU2MAP file for the /E action.");
            Console.WriteLine("  <destination>   Specifies the destination file. This should be a EU2MAP file for the /I action,");
            Console.WriteLine("                  and a PSD file for the /E action.");
            Console.WriteLine("                  If you omit the destination while exporting, one will be generated for you.");
            Console.WriteLine("  /1              Uses lightmap1 as source/target (depending on the action).");
            Console.WriteLine("  /2              Uses lightmap2 as source/target (depending on the action).");
            Console.WriteLine("  /3              Uses lightmap3 as source/target (depending on the action).");
            Console.WriteLine("  /R:<region>     Specifies the region to be exported. x,y denotes the topleft corner of the rectangle,");
            Console.WriteLine("                  width,height are the width and height of the rectangle. This will be adjusted by the program");
            Console.WriteLine("                  to the nearest 32x32 boundaries.");
            Console.WriteLine("  /O              Allows overwriting an existing file when exporting.");
            Console.WriteLine("  /P              Exports 3 PNG files instead of 1 PSD file. The PSD extension is automatically ");
            Console.WriteLine("                  changed into PNG, and each file is appended with the layer name which would normally");
            Console.WriteLine("                  appear in the PSD file. The MapInfo layer is omitted.");
            Console.WriteLine("  /G:<level>      This option indicates how to generate adjacent.tbl when importing.");
            Console.WriteLine("                  0 -> No regenerating is done. ");
            Console.WriteLine("                  1 -> Indicates to do a quick regenerate. This only happens when the ID map was modified. (default)");
            Console.WriteLine("                  2 -> Indicates to do a quick regenerate, even if the ID map wasn't modified.");
            Console.WriteLine("                  3 -> Indicates to do a complete regenerate. (Long!)");
            Console.WriteLine("  /PEDANTIC       Uses pedantic ID matching when importing an image. In this mode, the colour values must be exact.");
            Console.WriteLine("  /CHECKIDGRID    Checks the complexity of the ID grid regions of the imported area.");
            Console.WriteLine("                  Works only in simulation mode. (This feature is EXPERIMENTAL!)");
            Console.WriteLine();
            Console.WriteLine("Not specifying a /G:x option defaults to /G:1 being used.");
            Console.WriteLine("If no lightmap is specified, lightmap1 is used by default.");
        }

		private static IIDConvertor defaultConvertor = new Inferis2IDConvertor();
	}
}
