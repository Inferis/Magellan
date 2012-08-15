using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace MapToolsLib
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public abstract class Utils
	{
		public static void LogError( Exception e ) {
			if ( e == null ) return;

			Console.WriteLine( );

			string msg = e.ToString();

			try {
				msg = "*** Environment Info ***\n\n" +
                    "Magellan Build Version: " + MapToolsVersion.Version.BuildName + " " + MapToolsVersion.Version.BuildVersion + "\n" + 
                    "EU2 Lib Version: " + typeof(EU2.Map.Lightmap).Assembly.GetName().Version + "\n" + 
					"Date: " + DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss" ) + "\n" + 
					"Tool: " + System.Reflection.Assembly.GetCallingAssembly().FullName + " (v" + System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString() + ")\n" +
					"OSVersion: " + Environment.OSVersion.Platform.ToString() + " - " + Environment.OSVersion.Version.ToString() + "\n" + 
					"CurrentDir: " + Environment.CurrentDirectory  + "\n" + 
					"SystemDir: " + Environment.SystemDirectory + "\n" + 
					"CommandLine: " + Environment.CommandLine + "\n" + 
					"CLR: " + Environment.Version.ToString() + "\n" + 
					"EU2: " + (EU2.Install.FromRegistry() == null ? "(no install)" : EU2.Install.FromRegistry().RootPath) + "\n\n*** Error Info ***\n\n" + 
					msg;
			}
			catch {
			}

			string name = String.Format( "error-{0}-{1}.log", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, DateTime.Now.ToString("yyyyMMddHHmmss") ); 

			FileStream stream = null;
			try {
				if ( IsDebugBuild( Assembly.GetCallingAssembly() ) ) throw new Exception(); // to go to the catch statement
				stream = new FileStream( Path.Combine( Environment.CurrentDirectory, name ), FileMode.Create, FileAccess.Write, FileShare.None );
				StreamWriter writer = new StreamWriter( stream, System.Text.Encoding.UTF8 );
				writer.Write( msg );
				writer.Flush();
			}
			catch {
				Console.WriteLine( "An error occured.\nYou can read the error message below." );
				Console.WriteLine( msg );
				Console.WriteLine( "Please send this error info to eu2map@inferis.org for bugfixing." );
				return;
			}
			finally {
				if ( stream != null ) stream.Close();
			}

			Console.WriteLine( "An error occured.\nA log file (\"" + name + "\") has been generated, please send this file (or its contents) to eu2map@inferis.org for bugfixing." );
		}

        public static bool IsDebugBuild(Assembly assembly) {
            foreach (object att in assembly.GetCustomAttributes(false)) {
                if (att is DebuggableAttribute) {
                    return ((DebuggableAttribute)att).IsJITTrackingEnabled;
                }
            }

            return false;
        }

        public static ushort[] ScaleIDBuffer(ushort[,] buffer, int zoom) {
			ushort[] result = new ushort[(buffer.GetLength(0)>>zoom)*(buffer.GetLength(1)>>zoom) ];
			int size = 1<<zoom;

			int r = 0;
			for ( int y=0; y<buffer.GetLength(0); y+=size ) {
				for ( int x=0; x<buffer.GetLength(0); x+=size ) {
					ushort[] tagged = new ushort[EU2.Data.Province.MaxValue];

					// Tag ids in block
					for ( int by=0; by<size; ++by ) {
						for ( int bx=0; bx<size; ++bx ) {
							++tagged[buffer[x+bx,y+by]];
						}
					}
							
					// Find highest occurence
					ushort idx = 0;
					for ( ushort i=0; i<EU2.Data.Province.MaxValue; ++i ) {
						if ( tagged[i] > tagged[idx] ) idx = i;
					}
					
					result[r++] = idx;
				}
			}

			return result;
		}
	}
}
