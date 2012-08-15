using System;
using MapToolsLib;

namespace MapExtract
{
	/// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public class MapExtractParsedArguments : ParsedArguments 
	{
		public MapExtractParsedArguments( string[] args ) : base( ) {
			ProcessOption += new ProcessArgumentHandler(MapExtractParsedArguments_ProcessOption);
			ProcessParameter += new ProcessArgumentHandler(MapExtractParsedArguments_ProcessParameter);
			Parse( args );
		}

		public string Target {
			get { return target; }
		}

		public string Moddir {
			get { return moddir; }
		}

		public bool Overwrite {
			get { return overwrite; }
		}

		public int IncludeLightmaps {
			get { return includeLightmaps; }
		}

		public string DirectoryOverride {
			get { return dir; }
		}

		public bool Create {
			get { return create; }
		}

		public bool ForceGenerate {
			get { return forceGenerate; }
		}

		private void MapExtractParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "m":
					if ( e.Data.Length > 0 ) moddir = e.Data;
					break;
				case "d":
					if ( e.Data.Length > 0 ) dir = e.Data;
					break;
				case "o":
					overwrite = true;
					break;
				case "c":
					create = true;
					break;
				case "g":
					forceGenerate = true;
					break;
				case "1":
				case "2":
				case "3":
					includeLightmaps = int.Parse( e.Value );
					break;
			}
		}

		private void MapExtractParsedArguments_ProcessParameter(ParsedArguments sender, ProcessArgumentEventArgs e) {
			if ( target.Length == 0 ) target = e.Value;
		}

		private string target = "";
		private string moddir = "";
		private bool overwrite = false;
		private bool create = false;
		private int includeLightmaps = 3;
		private bool forceGenerate = false;
		private string dir = "";
	}
}
