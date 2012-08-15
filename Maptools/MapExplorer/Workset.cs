using System;
using EU2.Edit;
using EU2.Map;
using EU2.Data;
using EU2.Map.Drawing;

namespace MapExplorer
{
	/// <summary>
	/// Summary description for Workset.
	/// </summary>
	public sealed class Workset
	{
		public static Workset Current = new Workset();

		public Workset() {
			install = EU2.Install.FromRegistry();
			file = null;
			filepath = "";
			lastopendirectory = "";
		}

		public Workset( string installPath ) {
			install = EU2.Install.FromPath( installPath );
			file = null;
		}

		public EU2.Install Install {
			get { return install; }
		}

		public File File {
			get { return file; }
		}

		public string FilePath {
			get { return filepath; }
		}

		public string LastOpenDirectory {
			get { return lastopendirectory; }
			set { lastopendirectory = value; }
		}

		public File OpenFile( string path ) {
			File newfile = new EU2.Edit.File();
			newfile.ReadFrom( path );

			filepath = path;
			file = newfile;
			return file;
		}

		public File CreateFile( ) {
			File newfile = new File();
			newfile.Lightmap1 = Lightmap.CreateEmpty( 0 );
			newfile.Provinces = new ProvinceList();
			newfile.AdjacencyTable = new AdjacencyTable();
			newfile.IDMap = new IDMap();

			// Get colorscales from resource
			System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MapExplorer.Resources.colorscales.csv");
			try {
				newfile.ColorScales = new ColorScales( new EU2.IO.CSVReader( stream ) );
			}
			finally {
				if ( stream != null ) stream.Close();
			}

			filepath = "";
			file = newfile;
			return file;
		}

		public bool SaveFile() {
			if ( filepath.Length == 0 ) return false;
			file.WriteTo( filepath );
			filemodified = false;

			return true;
		}

		public bool SaveFileAs( string path ) {
			filepath = path;
			return SaveFile();
		}

		public void CloseFile() {
			file = null;
			filepath = "";
		}

		public bool FileModified {
			get {
				return filemodified;
			}
			set {
				if ( value ) filemodified = true;
			}
		}

		private EU2.Edit.File file;
		private string filepath;
		private bool filemodified;
		private EU2.Install install;
		private string lastopendirectory;
	}
}
