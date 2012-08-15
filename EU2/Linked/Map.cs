using System;
using EU2.Data.Mapfiles;
using System.Drawing;

namespace EU2.Data
{
	/// <summary>
	/// Summary description for Map.
	/// </summary>
	public class Map : InstallLinkedObject
	{
		public const int Width = 18944;
		public const int Height = 7296;

		private Lightmap[] lightmaps;

		public Map( EU2.Install install ) : base( install ) {
			lightmaps = new Lightmap[5];

			lightmaps[0] = new Lightmap( 0, install.GetMapFile("lightmap1.tbl"), install );
			lightmaps[1] = new Lightmap( 1, install.GetMapFile("lightmap2.tbl"), install );
			lightmaps[2] = new Lightmap( 2, install.GetMapFile("lightmap3.tbl"), install );
			lightmaps[3] = new Lightmap( 4, install.GetMapFile("lightmap4.tbl"), install );
		}

		public Lightmap GetLightmap( int level ) {
			if ( level < 1 || level > 5 ) throw new ArgumentOutOfRangeException();

			return lightmaps[level-1];
		}

		public Lightmap Lightmap1 {
			get {
				return lightmaps[0];
			}
		}

		public Lightmap Lightmap2 {
			get {
				return lightmaps[1];
			}
		}
	
		public Lightmap Lightmap3 {
			get {
				return lightmaps[2];
			}
		}

		public Lightmap Lightmap4 {
			get {
				return lightmaps[3];
			}
		}

		public Size MapSize {
			get { return new Size( Width, Height ); }
		}
	}
}
