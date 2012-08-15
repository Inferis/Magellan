using System;
using System.Drawing;
using MapToolsLib;

namespace MapTest {
	public enum Action {
		None,
		LightmapDownSizing
	}

	public enum RegionMode {
		Normal,
		All,
		Full
	}

    /// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
    public class MapTestParsedArguments : ParsedArguments {
        public MapTestParsedArguments(string[] args) : base() {
			ProcessOption += new ProcessArgumentHandler(MapImageParsedArguments_ProcessOption);
			ProcessParameter += new ProcessArgumentHandler(MapImageParsedArguments_ProcessParameter);
			Parse( args );
		}

		public Action Action {
			get { return action; }
		}

		public string Source {
			get { return source; }
		}

		public Rectangle Region {
			get { return region; }
		}

		public RegionMode RegionMode {
			get { return mode; }
		}

		private void MapImageParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "lds":
                    action = Action.LightmapDownSizing;
					if ( e.Data.Length > 0 ) {
						if ( e.Data.ToLower() == "full" ) {
							region = new Rectangle( -1, -1, -1, -1 );
							mode = RegionMode.Full;
						}
						else {
							int size = -1;
							try {
								size = int.Parse( e.Data );
							}
							catch ( FormatException ) {
								size = -1;
							}

							if ( size <= 0 ) {
								string[] spec= e.Data.Split( new char[] {','}, 4 );
							
								if ( spec.Length >= 4 ) {
									Rectangle newRegion = region;
									try {
										newRegion = new Rectangle( int.Parse( spec[0] ), int.Parse( spec[1] ), int.Parse( spec[2] ), int.Parse( spec[3] ) );
									}
									catch ( FormatException ) {
										// Do nothing
									}
									region = newRegion;
								}
								mode = RegionMode.Normal;
							}
							else {
								region = new Rectangle( 0, 0, size, size );
								mode = RegionMode.All;
							}
						}
					}
					break;
			}
		}

		private void MapImageParsedArguments_ProcessParameter(ParsedArguments sender, ProcessArgumentEventArgs e) {
			if ( source.Length == 0 ) source = e.Value;
		}

		private string source = "";
		private Rectangle region = Rectangle.Empty;
		private RegionMode mode = RegionMode.Normal;
		private Action action = Action.None;
	}
}
