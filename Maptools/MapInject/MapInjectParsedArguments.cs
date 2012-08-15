using System;
using MapToolsLib;

namespace MapInject
{
	/// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public class MapInjectParsedArguments : MapToolsLib.ParsedArguments 
	{
		public MapInjectParsedArguments( string[] args ) : base( ) {
			ProcessOption += new ProcessArgumentHandler(MapInjectParsedArguments_ProcessOption);
			ProcessParameter += new ProcessArgumentHandler(MapInjectParsedArguments_ProcessParameter);
			Parse( args );
		}

		public string Source {
			get { return source; }
		}

		public string Moddir {
			get { return moddir; }
		}

		public string DirectoryOverride {
			get { return dir; }
		}

		public int RegenerateLightmaps {
			get { return regenLightmaps; }
		}

		public bool SaveSource {
			get { return saveSource; }
		}

		public bool NoTOT {
			get { return noTOT; }
		}

		private void MapInjectParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "m":
					if ( e.Data.Length > 0 ) moddir = e.Data;
					break;
				case "d":
					if ( e.Data.Length > 0 ) dir = e.Data;
					break;
				case "l":
					regenLightmaps = 1;
					if ( e.Data.ToLower() == "o" ) regenLightmaps = 2; 
					break;
				case "s":
					saveSource = true;
					break;
				case "notot":
					noTOT = true;
					break;
			}
		}

		private void MapInjectParsedArguments_ProcessParameter(ParsedArguments sender, ProcessArgumentEventArgs e) {
			if ( source.Length == 0 ) source = e.Value;
		}
		
		private string source = "";
		private string moddir = "";
		private int regenLightmaps = 0;
		private bool saveSource = false;
		private string dir = "";
		private bool noTOT = false;
	}
}
