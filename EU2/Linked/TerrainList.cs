using System;
using System.Collections;
using System.IO;
using EU2.IO;

namespace EU2.Linked {
	/// <summary>
	/// Summary description for TerrainTypes.
	/// </summary>
	public class TerrainList : EU2.Bare.TerrainList, IInstallLinked {
		private EU2.Install install;

		public TerrainList( EU2.Install install ) {
			CSVReader reader = new CSVReader( install.GetMapFile( "terrain types.csv" ) );
			try {
				ReadFromCSV( reader );
			}
			finally {
				reader.Close();
			}
		}

		public EU2.Install Install { get { return install; } }
	}
}
