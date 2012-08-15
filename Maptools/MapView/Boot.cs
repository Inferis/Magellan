using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using MapToolsLib;

namespace MapView
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Boot
	{
        private const string ToolDescription = "EU2 Map View Tool";

        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            Console.WriteLine(MapToolsVersion.Version.GetVersionString(ToolDescription));
            Console.WriteLine();

#if !DEBUG
			try {
#endif
				MapViewParsedArguments pargs = new MapViewParsedArguments( args );
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
					Console.WriteLine( "The specified source file \"{0}\" does not exist.", Path.GetFileName( source ) );
					return;
				}

				// Show form
				Application.EnableVisualStyles();
                MainForm mainForm = new MainForm(source, pargs.UseLightmap);
				Application.Run(mainForm);
                mainForm.Focus();
#if !DEBUG
			}
			catch( Exception e ) {
				MapToolsLib.Utils.LogError( e );
			}
#endif

			Console.WriteLine( );
		}

		private static void ShowHelp() {
			Console.WriteLine( "Tool to visualise an EU2MAP file." );
			Console.WriteLine( );
			Console.WriteLine( "MVIEW <source>" );
			Console.WriteLine( );
			Console.WriteLine( "  <source>        The EU2MAP file to visualise." );
		}
	}
}
