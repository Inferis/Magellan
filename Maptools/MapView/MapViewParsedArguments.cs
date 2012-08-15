using System;
using System.Drawing;
using System.Globalization;
using MapToolsLib;

namespace MapView
{
	/// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public class MapViewParsedArguments : ParsedArguments
	{
		public MapViewParsedArguments( string[] args ) {
			ProcessParameter += new ProcessArgumentHandler(MapViewParsedArguments_ProcessParameter);
			ProcessOption += new ProcessArgumentHandler(MapViewParsedArguments_ProcessOption);
			Parse( args );
		}

		public int UseLightmap {
			get { return useLightmap; }
		}

		public string Source {
			get { return source; }
		}

		private void MapViewParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "1": case "2": case "3":
					useLightmap = int.Parse( e.Value );
					break;
			}
		}

		private void MapViewParsedArguments_ProcessParameter(ParsedArguments sender, ProcessArgumentEventArgs e) {
			if ( source.Length == 0 ) source = e.Value;
		}

		private int useLightmap = 1;
		private string source = "";
	}
}
