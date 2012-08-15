using System;
using MapToolsLib;

namespace MapStats {

	public enum Action {
		None,
		BoundBoxes,
		Adjacencies,
		Regions
	}

	public enum ExportMode {
		DontCare,
		Plain,
		XML
	}

	/// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public class MapStatsParsedArguments : ParsedArguments
	{
		public MapStatsParsedArguments( string[] args ) : base( ) {
			ProcessOption += new ProcessArgumentHandler(MapStatsParsedArguments_ProcessOption);
			ProcessParameter += new ProcessArgumentHandler(MapStatsParsedArguments_ProcessParameter);
			Parse( args );
		}

		public Action Action {
			get { return action; }
		}

		public string Source {
			get { return source; }
		}

		public string Target {
			get { return target; }
		}

		public bool Overwrite {
			get { return overwrite; }
		}

		public ExportMode Mode  {
			get { return mode; }
		}

		private void MapStatsParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "bb": case "boundbox":
					action = Action.BoundBoxes;
					break;
				case "adj": case "adjacent": case "adjacency": case "adjacencies":
					action = Action.Adjacencies;
					break;
				case "r": case "regions":
					action = Action.Regions;
					break;
				case "x": case "xml":
					mode = ExportMode.XML;
					break;
				case "c": case "csv":
					mode = ExportMode.Plain;
					break;
				case "o":
					overwrite = true;
					break;
			}
		}

		private void MapStatsParsedArguments_ProcessParameter(ParsedArguments sender, ProcessArgumentEventArgs e) {
			if ( source.Length == 0 ) source = e.Value;
			else if ( target.Length == 0 ) target = e.Value;
		}

		private string source = "";
		private string target = "";
		private bool overwrite = false;
		private Action action = Action.None;
		private ExportMode mode = ExportMode.DontCare;

	}
}
