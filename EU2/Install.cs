using System;
using Microsoft.Win32;
//using EU2.Linked;

namespace EU2
{
	/// <summary>
	/// 
	/// </summary>
	public class Install 
	{
		string rootpath;
		//EU2.Data.Map map;
		//EU2.Linked.ProvinceList provinces;
		//EU2.Linked.ReligionList religions;
		//EU2.Linked.TerrainList terraintypes;

		private Install()
		{
			rootpath = "";
		}

		private Install( string path ) {
			rootpath = path;
		}

		/*
		public EU2.Data.Map Map {
			get {
				if ( map == null ) map = new EU2.Data.Map( this );
				return map;
			}
		}

		public EU2.Data.ProvinceList Provinces {
			get {
				if ( provinces == null ) {
					provinces = new ProvinceList( this );
				}
				return provinces;
			}
		}

		public EU2.Data.ReligionList Religions {
			get {
				if ( religions == null ) {
					religions = new ReligionList( this );
				}
				return religions;
			}
		}

		public EU2.Linked.TerrainList Terrains {
			get {
				if ( terraintypes == null ) {
					terraintypes = new TerrainList( this );
				}
				return terraintypes;
			}
		}
		*/

		#region Path Properties
		public string RootPath {
			get {
				return rootpath;
			}
		}

		public string DBPath {
			get {
				return rootpath + "\\db";
			}
		}

		public string GetDBFile(string file) {
			return DBPath + "\\" + file;
		}

		public string MapPath {
			get {
				return rootpath + "\\map";
			}
		}

		public string GetMapFile(string file) {
			return MapPath + "\\" + file;
		}

		public string AIPath {
			get {
				return rootpath + "\\ai";
			}
		}

		public string GetAIFile(string file) {
			return AIPath + "\\" + file;
		}

		public string EventPath {
			get {
				return rootpath + "\\db\\Events";
			}
		}

		public string GetEventFile(string file) {
			return EventPath + "\\" + file;
		}

		public string LeadersPath {
			get {
				return rootpath + "\\db\\Leaders";
			}
		}

		public string MonarchsPath {
			get {
				return rootpath + "\\db\\Monarchs";
			}
		}
		#endregion
		
		public static Install FromPath( string path )  {
			return new Install( path );
		}

		public static Install FromRegistry(){
			Install install;

			try {
				RegistryKey key = Registry.LocalMachine.OpenSubKey( "SOFTWARE\\Paradox\\Europa Universalis 2", false );

				install = new Install(key.GetValue( "Install Path" ).ToString());
				key.Close();
			}
			catch {
				install = null;
			}

			return install; 
		}


	}
}
