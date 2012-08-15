using System;
using System.Collections.Generic;
using System.IO;

namespace MapTest {
    class Boot {
        private const string ToolDescription = "EU2 Map Test Tool";

        static void Main(string[] args) {
            Console.WriteLine(MapToolsVersion.Version.GetVersionString(ToolDescription));
            Console.WriteLine();

#if !DEBUG
			try {
#endif
            MapTestParsedArguments pargs = new MapTestParsedArguments(args);

            if (pargs.Help)
            {
                ShowHelp();
                return;
            }

            if (pargs.Source == "")
            {
                Console.WriteLine("No source file specified!");
                return;
            }

            string source = pargs.Source;
            if (Path.GetExtension(source) == "")
                source = Path.ChangeExtension(source, "eu2map");
            source = Path.GetFullPath(source);

            // Check if source exists
            if (!System.IO.File.Exists(source))
            {
                Console.WriteLine("The specified source file \"{0}\" could not be found.", Path.GetFileName(source));
                return;
            }

#if !DEBUG
			}
			catch( Exception e ) {
				MapToolsLib.Utils.LogError( e );
			}
#endif

            Console.WriteLine();
        }

        private static void ShowHelp() {
            Console.WriteLine("Tests various stuff on the eu2map data.");
            Console.WriteLine();
            Console.WriteLine("MTEST <source[.eu2map]> [/LS:<x,y,width,height>]");
            Console.WriteLine();
            Console.WriteLine("  <source>        Specifies the file to test. The extension is automatically added if it is omitted.");
            Console.WriteLine("  /LS:<region>    Specifies a region to tested for downsizing. x,y denotes the topleft corner of the rectangle,");
            Console.WriteLine("                  width,height are the width and height of the rectangle. This will be adjusted by the program");
            Console.WriteLine("                  to the nearest 32x32 boundaries. These are specified as lightmap1 coordinates; 3 PNG files");
            Console.WriteLine("                  will be generated for each downsized lightmap.");
        }
    }
}
