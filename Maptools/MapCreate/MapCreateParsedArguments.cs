using System;
using MapToolsLib;

namespace MapExtract
{
	/// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public class MapCreateParsedArguments : ParsedArguments 
	{
		public MapCreateParsedArguments( string[] args ) : base( ) {
			ProcessOption += new ProcessArgumentHandler(MapCreateParsedArguments_ProcessOption);
			ProcessParameter += new ProcessArgumentHandler(MapCreateParsedArguments_ProcessParameter);
			Parse( args );
		}

		public string Target {
			get { return target; }
		}

		public bool Overwrite {
			get { return overwrite; }
		}

		private void MapCreateParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "p":
					if ( e.Data.Length > 0 ) provincefile = e.Data;
					break;
				case "o":
					overwrite = true;
					break;
			}
		}

		private void MapCreateParsedArguments_ProcessParameter(ParsedArguments sender, ProcessArgumentEventArgs e) {
			if ( target.Length == 0 ) target = e.Value;
		}

		private string provincefile = "";
		private bool overwrite = false;
	}
}
