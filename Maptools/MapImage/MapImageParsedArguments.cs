using System;
using System.Drawing;
using MapToolsLib;

namespace MapImage {
	public enum Action {
		None,
		ImportImage,
		ExportImage,
		QueryImage,
		CheckImage
	}

	public enum RegionMode {
		Normal,
		All,
		Full
	}

    public enum PngLevel {
        None = 0,
        All = PngLevel.Shading | PngLevel.IDs | PngLevel.Borders,
        Shading = 1,
        IDs = 2,
        Borders = 4
    }
    
    /// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public class MapImageParsedArguments : ParsedArguments {
		public MapImageParsedArguments( string[] args ) : base( ) {
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

		public string Target {
			get { return target; }
		}

		public bool Simulate {
			get { return simulate; }
		}

		public bool Overwrite {
			get { return overwrite; }
		}

		public bool Direct {
			get { return direct; }
		}

		public int UseLightmap {
			get { return useLightmap; }
		}

        public PngLevel PngLevel {
            get { return pngLevel; }
        }
        
        public int Regenerate {
			get { return regen; }
		}

		public Rectangle Region {
			get { return region; }
		}

		public RegionMode RegionMode {
			get { return mode; }
		}

		public int CheckID {
			get { return id; }
		}

		public Point Relocate {
			get { return reloc; }
		}

		public bool Tolerant {
			get { return tolerant; }
		}

		public bool CheckIDGrid {
			get { return checkidgrid; }
		}

		private void MapImageParsedArguments_ProcessOption(ParsedArguments sender, ProcessArgumentEventArgs e) {
			switch ( e.Value.ToLower() ) {
				case "i": case "import":
					action = Action.ImportImage;
					simulate = false;
					break;
				case "e": case "export":
					action = Action.ExportImage;
					break;
				case "q": case "query":
					action = Action.QueryImage;
					break;
				case "si": case "simulateimport":
					action = Action.ImportImage;
					simulate = true;
					break;
				case "c": case "check":
					action = Action.CheckImage;
					break;
				case "o":
					overwrite = true;
					break;
				case "1": case "2": case "3":
					useLightmap = int.Parse( e.Value );
					break;
				case "p": case "png":
                    if (e.Data.Length > 0) {
                        foreach(string s in e.Data.Split(new char[] { ',' })) {
                            switch (s.ToLower()) {
                                case "shading":
                                case "s":
                                case "shades":
                                case "shade":
                                    pngLevel |= PngLevel.Shading;
                                    break;

                                case "ids":
                                case "i":
                                case "id":
                                case "idmap":
                                    pngLevel |= PngLevel.IDs;
                                    break;

                                case "borders":
                                case "b":
                                case "border":
                                    pngLevel |= PngLevel.Borders;
                                    break;
                            }
                        }
                    }
                    else {
                        pngLevel = PngLevel.All;
                    }
					break;
				case "d": case "direct":
					direct = true;
					break;
				case "g":
					if ( e.Data.Length > 0 ) {
						try {
							regen = int.Parse( e.Data );
						}
						catch ( FormatException ) {
							// Do nothing
						}
					}
					else {
						regen = 1;
					}
					break;
				case "r":
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
				case "relocate":
					if ( e.Data.Length > 0 ) {
						string[] spec= e.Data.Split( new char[] {','}, 2 );
					
						if ( spec.Length >= 2 ) {
							Point newReloc = reloc;
							try {
								newReloc = new Point( int.Parse( spec[0] ), int.Parse( spec[1] ) );
							}
							catch ( FormatException ) {
								// Do nothing
							}
							reloc = newReloc;
						}
					}
					break;
				case "id":
					try {
						id = int.Parse( e.Data );
					}
					catch ( FormatException ) {
						id = -1;
					}
					break;
				case "pedantic":
					tolerant = false;
					break;
				case "checkidgrid":
					checkidgrid = true;
					break;
			}
		}

		private void MapImageParsedArguments_ProcessParameter(ParsedArguments sender, ProcessArgumentEventArgs e) {
			if ( source.Length == 0 ) source = e.Value;
			else if ( target.Length == 0 ) target = e.Value;
		}

		private string source = "";
		private string target = "";
		private bool overwrite = false;
		private int useLightmap = 1;
		private Rectangle region = Rectangle.Empty;
		private RegionMode mode = RegionMode.Normal;
		private Action action = Action.None;
        private PngLevel pngLevel = PngLevel.None;
		private bool simulate = false;
		private bool direct = false;
		private int regen = 1;
		private int id = -1;
		private Point reloc = Point.Empty;
		private bool tolerant = true;
		private bool checkidgrid = false;
	}
}
