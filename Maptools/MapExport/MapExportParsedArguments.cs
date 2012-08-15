using System;
using MapToolsLib;

namespace MapExport
{
	/// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public class MapExportParsedArguments : MapToolsLib.ParsedArguments 
	{
		public MapExportParsedArguments( string[] args ) : base( ) {
			ProcessOption += new ProcessArgumentHandler(MapExportParsedArguments_ProcessOption);
			ProcessParameter += new ProcessArgumentHandler(MapExportParsedArguments_ProcessParameter);
			Parse( args );
		}

		public string Source {
			get { return source; }
		}

		public string DirectoryOverride {
			get { return dir; }
		}

		private void MapExportParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "d":
					if ( e.Data.Length > 0 ) dir = e.Data;
					break;
			}
		}

		private void MapExportParsedArguments_ProcessParameter(ParsedArguments sender, ProcessArgumentEventArgs e) {
			if ( source.Length == 0 ) source = e.Value;
		}
		
		private string source = "";
		private string dir = "";
	}
}
