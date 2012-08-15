using System;
using MapToolsLib;

namespace MapProvince
{
	public enum Action {
		None,
		ImportProvince,
		ExportProvince
	}

	public enum ExportMode {
		DontCare,
		Plain,
		XML
	}

	/// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public class MapProvinceParsedArguments : ParsedArguments
	{
		public MapProvinceParsedArguments( string[] args ) : base( ) {
			ProcessOption += new ProcessArgumentHandler(MapProvinceParsedArguments_ProcessOption);
			ProcessParameter += new ProcessArgumentHandler(MapProvinceParsedArguments_ProcessParameter);
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

		public bool NoTOT {
			get { return noTOT; }
		}

		private void MapProvinceParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "i": case "import":
					action = Action.ImportProvince;
					break;
				case "e": case "export":
					action = Action.ExportProvince;
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
				case "notot":
					noTOT = true;
					break;
			}
		}

		private void MapProvinceParsedArguments_ProcessParameter(ParsedArguments sender, ProcessArgumentEventArgs e) {
			if ( source.Length == 0 ) source = e.Value;
			else if ( target.Length == 0 ) target = e.Value;
		}

		private string source = "";
		private string target = "";
		private bool overwrite = false;
		private Action action = Action.None;
		private ExportMode mode = ExportMode.DontCare;
		private bool noTOT = false;

	}
}
