using System;
using System.Collections;
using System.IO;

namespace EU2.Linked
{
	/// <summary>
	/// Summary description for ReligionList.
	/// </summary>
	public class ReligionList : EU2.Bare.ReligionList, IInstallLinked {
		private EU2.Install install;

		public ReligionList( EU2.Install install ) {
			CSVReader reader = new CSVReader( install.GetDBFile( "religion.csv" ) );
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
