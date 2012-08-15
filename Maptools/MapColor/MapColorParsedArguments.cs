using System;
using System.Drawing;
using System.Globalization;
using MapToolsLib;

namespace MapColor
{
	/// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public class MapColorParsedArguments : ParsedArguments
	{
		public MapColorParsedArguments( string[] args ) {
			ProcessOption += new ProcessArgumentHandler(MapColorParsedArguments_ProcessOption);
			Parse( args );
		}

		public int ID {
			get { return id; }
		}

		public int Color {
			get { return color; }
		}

		public string Convertor {
			get { return convertor; }
		}

		public string MakeMap {
			get { return makeMap; }
		}

		private void MapColorParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "i": case "id":
					if ( e.Data.Length > 0 ) {
						try {
							id = int.Parse( e.Data );
						}
						catch {
							id = -1;
						}
					}
					break;

				case "rgb":
					if ( e.Data.Length > 0 ) {
						int idx = e.Data.IndexOfAny( new char[] { ',', '.', ':', '-' } );									
						if ( idx < 0 ) {
							try {
								color = int.Parse( e.Data );
							}
							catch {
								color = -1;
							}
						}
						else {
							NumberStyles ns = NumberStyles.HexNumber;
							try {
								string[] components = e.Data.Split( new char[] { e.Data[idx] }, 3 );
								color = (int.Parse( components[0], ns ) << 16) | (int.Parse( components[1], ns ) << 8) | (int.Parse( components[2], ns ));
							}
							catch {
								color = -1;
							}
						}
					}
					break;

				case "c": 
					if ( e.Data.Length > 0 ) convertor = e.Data;
					break;

				case "map":
					if ( e.Data.Length > 0 ) 
						makeMap = e.Data;
					else 
						makeMap = "colourmap";
					break;
			}
		}

		private int id = -1;
		private int color = -1;
		private string convertor = "";
		private string makeMap = "";
	}
}
