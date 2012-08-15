using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using EU2;
using EU2.Edit;
using EU2.Data;
using EU2.Map;
using MapToolsLib;

namespace MapStats
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Boot
	{
		private const string ToolDescription = "EU2 Map Stats Tool";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            Console.WriteLine(MapToolsVersion.Version.GetVersionString(ToolDescription));
			Console.WriteLine( );

#if !DEBUG
			try {
#endif
				MapStatsParsedArguments pargs = new MapStatsParsedArguments( args );
				if ( pargs.Help ) {
					ShowHelp();
					return;
				}

				if ( pargs.Action == Action.None ) {
					Console.WriteLine( "No action specified. Please use the /BB, /ADJ or /R directive. Use /? for more help." );
					return;
				}
				if ( pargs.Source == "" ) {
					Console.WriteLine( "No source file specified!" );
					return;
				}
				string source = pargs.Source; 

				if ( Path.GetExtension( source ) == "" ) {
					source = Path.ChangeExtension( source, "eu2map" );
				}
				source = Path.GetFullPath( source ); 

				// Check if source exists
				if ( !System.IO.File.Exists( source ) ) {
					Console.WriteLine( "The specified source file \"{0}\" does not exist.", Path.GetFileName( source ) );
					return;
				}

				string target = pargs.Target;
				if ( target == "" ) {
					if ( pargs.Action == Action.BoundBoxes ) {
						target = String.Format( "{0}-boundbox.{1}", Path.GetFileNameWithoutExtension( source ), pargs.Mode == ExportMode.XML ? "xml" : "csv" );
					}
					else if ( pargs.Action == Action.Adjacencies ) {
						target = String.Format( "{0}-adjacencies.{1}", Path.GetFileNameWithoutExtension( source ), pargs.Mode == ExportMode.XML ? "xml" : "csv" );
					}
					else if ( pargs.Action == Action.Regions ) {
						target = String.Format( "{0}-regions.{1}", Path.GetFileNameWithoutExtension( source ), pargs.Mode == ExportMode.XML ? "xml" : "csv" );
					}
					else {
						Console.WriteLine( "No target file specified!" );
						return;
					}
					Console.WriteLine( "No target file specified. Using default name: {0}", target );
				}

				if ( Path.GetExtension( target ) == "" ) {
					target = Path.ChangeExtension( target, pargs.Mode == ExportMode.XML ? "xml" : "csv" );
				}
				target = Path.GetFullPath( target ); 

				// Check if target exists
				if ( System.IO.File.Exists( target ) && !pargs.Overwrite ) {
					Console.WriteLine( "Target file \"{0}\" exists. Specify the /O option to overwrite it.", Path.GetFileName( target ) );
					return;
				}

				switch ( pargs.Action ) {
					case Action.BoundBoxes:
						ExportBoundBoxes(source, target, pargs.Mode);
						break;
					case Action.Adjacencies:
						ExportAdjacencies(source, target, pargs.Mode);
						break;
					case Action.Regions:
						ExportRegions(source, target, pargs.Mode);
						break;
				}
#if !DEBUG
			}
			catch( Exception e ) {
				MapToolsLib.Utils.LogError( e );
			}
#endif

			Console.WriteLine( );
		}

		public static void ExportBoundBoxes(string source, string target, ExportMode mode) {
			Console.WriteLine( "Opening source file \"{0}\"...", Path.GetFileName( source ) );

			EU2.Edit.File file = new EU2.Edit.File();
			try {
				file.ReadFrom( source );
			}
			catch ( EU2.Edit.InvalidFileFormatException ) {
				Console.WriteLine( "The specified source file is not an EU2Map file, or is corrupt." );
				return;
			}

			EU2.Map.ProvinceBoundBox[] boxes = null;
			if ( file.BoundBoxes == null ) {
				if (file.IDMap == null) {
					Console.WriteLine( "Error: The file does not contain boundbox info, and no IDMap info. Can't calculate boxes." );
					return;
				}
				Console.WriteLine( "The file does not contain boundbox info. Will calculate them now from the IDMap." );
				Console.Write( "Calculating... " );
				boxes = file.IDMap.CalculateBoundBoxes();
				Console.WriteLine( "done!" );
			}
			else {
				boxes = file.BoundBoxes.Boxes;
			}

			// -- Write the file
			Console.WriteLine( "Exporting to \"{0}\"...", Path.GetFileName( target ) );
			FileStream stream = null;
			try {
				stream = new FileStream( target, FileMode.Create, FileAccess.Write, FileShare.None );
				if ( mode == ExportMode.XML ) {
					WriteBoxes(boxes, new System.Xml.XmlTextWriter(stream, System.Text.Encoding.UTF8));
				}
				else {
					WriteBoxes(boxes, new EU2.IO.CSVWriter(stream));
				}
			}
			finally {
				if ( stream != null ) stream.Close();
			}

			Console.WriteLine( "Export successful." );
		}

		private static void WriteBoxes(EU2.Map.ProvinceBoundBox[] boxes, System.Xml.XmlTextWriter writer) {
			writer.WriteStartDocument();
			writer.WriteStartElement("BoundBoxes");

			foreach(EU2.Map.ProvinceBoundBox box in boxes) {
				writer.WriteStartElement("Box");
				writer.WriteAttributeString("provinceId", box.ProvinceID.ToString());
				writer.WriteAttributeString("left", box.Left.ToString());
				writer.WriteAttributeString("top", box.Top.ToString());
				writer.WriteAttributeString("right", box.Right.ToString());
				writer.WriteAttributeString("bottom", box.Bottom.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
		}

		private static void WriteBoxes(EU2.Map.ProvinceBoundBox[] boxes, EU2.IO.CSVWriter writer) {
			writer.EndRow( "ProvinceId;Left;Top;Right;Bottom;" );

			foreach(EU2.Map.ProvinceBoundBox box in boxes) {
				writer.Write(box.ProvinceID);
				writer.Write(box.Left);
				writer.Write(box.Top);
				writer.Write(box.Right);
				writer.Write(box.Bottom);
				writer.EndRow();
			}
			writer.Flush();
		}

		public static void ExportAdjacencies(string source, string target, ExportMode mode) {
			Console.WriteLine( "Opening source file \"{0}\"...", Path.GetFileName( source ) );

			EU2.Edit.File file = new EU2.Edit.File();
			try {
				file.ReadFrom( source );
			}
			catch ( EU2.Edit.InvalidFileFormatException ) {
				Console.WriteLine( "The specified source file is not an EU2Map file, or is corrupt." );
				return;
			}

			EU2.Map.AdjacencyTable adj = null;
			if ( file.AdjacencyTable == null ) {
				if (file.IDMap == null) {
					Console.WriteLine( "Error: The file does not contain adjacency info, and no IDMap info. Can't calculate adjacencies." );
					return;
				}
				if (file.Provinces == null) {
					Console.WriteLine( "Error: The file does not contain adjacency info, and no Province info. Can't calculate adjacencies." );
					return;
				}
				Console.WriteLine( "The file does not contain boundbox info. Will calculate them now from the IDMap." );
				Console.Write( "Calculating... " );
				adj = file.IDMap.BuildAdjacencyTable(file.Provinces);
				Console.WriteLine( "done!" );
			}
			else {
				adj = file.AdjacencyTable;
			}

			// -- Write the file
			Console.WriteLine( "Exporting to \"{0}\"...", Path.GetFileName( target ) );
			FileStream stream = null;
			try {
				stream = new FileStream( target, FileMode.Create, FileAccess.Write, FileShare.None );
				if ( mode == ExportMode.XML ) {
					WriteAdjacencies(adj, file.Provinces, new System.Xml.XmlTextWriter(stream, System.Text.Encoding.UTF8));
				}
				else {
					WriteAdjacencies(adj, file.Provinces, new EU2.IO.CSVWriter(stream));
				}
			}
			finally {
				if ( stream != null ) stream.Close();
			}

			Console.WriteLine( "Export successful." );
		}

		private static void WriteAdjacencies(EU2.Map.AdjacencyTable adjtable, EU2.Data.ProvinceList provinces, System.Xml.XmlTextWriter writer) {
			writer.WriteStartDocument();
			writer.WriteStartElement("Adjacencies");

			for(int i=Province.MinID; i<=Province.MaxID; ++i) {
				EU2.Map.Adjacent[] adj = adjtable[i];
				writer.WriteStartElement("From");
				writer.WriteAttributeString("id", i.ToString());
				writer.WriteAttributeString("name", provinces[i].Name);
				for(int t=0; t<adj.Length; ++t) {
					writer.WriteStartElement("To");
					writer.WriteAttributeString("id", adj[t].ID.ToString());
					writer.WriteAttributeString("name", provinces[adj[t].ID].Name);
					writer.WriteAttributeString("type", adj[t].Type.ToString().ToLower());
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
		}

		private static void WriteAdjacencies(EU2.Map.AdjacencyTable adjtable, EU2.Data.ProvinceList provinces, EU2.IO.CSVWriter writer) {
			writer.EndRow( "fromProvinceId;fromProvinceName;toProvinceId;toProvinceName;type" );

			for(int i=Province.MinID; i<=Province.MaxID; ++i) {
				EU2.Map.Adjacent[] adj = adjtable[i];
				for(int t=0; t<adj.Length; ++t) {
					writer.Write(i);
					writer.Write(provinces[i].Name);
					writer.Write(adj[t].ID);
					writer.Write(provinces[adj[t].ID].Name);
					writer.Write(adj[t].Type.ToString().ToLower());
					writer.EndRow();
				}
			}
			writer.Flush();
		}

		public static void ExportRegions(string source, string target, ExportMode mode) {
			Console.WriteLine( "Opening source file \"{0}\"...", Path.GetFileName( source ) );

			EU2.Edit.File file = new EU2.Edit.File();
			try {
				file.ReadFrom( source );
			}
			catch ( EU2.Edit.InvalidFileFormatException ) {
				Console.WriteLine( "The specified source file is not an EU2Map file, or is corrupt." );
				return;
			}

			EU2.Map.IDGrid idgrid = null;
			if ( file.IDGrid == null ) {
				if (file.IDMap == null) {
					Console.WriteLine( "Error: The file does not contain region (idgrid) info, and no IDMap info. Can't calculate region info." );
					return;
				}
				Console.WriteLine( "The file does not contain idgrid info. Will calculate them now from the IDMap." );
				Console.Write( "Calculating... " );
				idgrid = EU2.Map.IDGrid.ForcedBuild(file.IDMap);
				Console.WriteLine( "done!" );
			}
			else {
				idgrid = file.IDGrid;
			}

			// -- Write the file
			Console.WriteLine( "Exporting to \"{0}\"...", Path.GetFileName( target ) );
			FileStream stream = null;
			try {
				stream = new FileStream( target, FileMode.Create, FileAccess.Write, FileShare.None );
				if ( mode == ExportMode.XML ) {
					WriteRegions(idgrid, file.Provinces, file.IDMap, new System.Xml.XmlTextWriter(stream, System.Text.Encoding.UTF8));
				}
				else {
					WriteRegions(idgrid, file.Provinces, file.IDMap, new EU2.IO.CSVWriter(stream));
				}
			}
			finally {
				if ( stream != null ) stream.Close();
			}

			Console.WriteLine( "Export successful." );
		}

		private static void WriteRegions(EU2.Map.IDGrid idgrid, EU2.Data.ProvinceList provinces, IDMap idmap, System.Xml.XmlTextWriter writer) {
			writer.WriteStartDocument();
			writer.WriteStartElement("IDGrid");

			for(int y=0; y<idgrid.Height; ++y ) {
				for(int x=0; x<idgrid.Width; ++x ) {
					writer.WriteStartElement("Region");
					writer.WriteAttributeString("index-x", x.ToString());
					writer.WriteAttributeString("index-y", y.ToString());
					writer.WriteAttributeString("offset-x", (x*EU2.Map.IDGrid.RegionWidth).ToString());
					writer.WriteAttributeString("offset-y", (y*EU2.Map.IDGrid.RegionHeight).ToString());

					ushort[] list = idgrid.GetIDs(x,y);
					int cnt=0;
					for(; cnt<list.Length; ++cnt) {
						if ( list[cnt] == 0 && cnt>0 ) break;
					}

					if ( cnt <= 1 ) {
						writer.WriteAttributeString("provinceCount", "0");
						Rectangle rect = Rectangle.Empty;
						if ( !EU2.Map.IDGrid.CheckAreaForOverflow(idmap, EU2.Map.IDGrid.GetRegionRect(x,y), out rect) ) {
							writer.WriteStartElement("Overflow");
							writer.WriteAttributeString("x", rect.X.ToString());
							writer.WriteAttributeString("y", rect.Y.ToString());
							writer.WriteAttributeString("width", rect.Width.ToString());
							writer.WriteAttributeString("height", rect.Height.ToString());
							writer.WriteEndElement();
						}
					}
					else {
						writer.WriteAttributeString("provinceCount", cnt.ToString());
						for(int i=0; i<list.Length; ++i) {
							if ( list[i] == 0 && i>0 ) break;
							writer.WriteStartElement("Province");
							writer.WriteAttributeString("index", i.ToString());
							writer.WriteAttributeString("id", list[i].ToString());
							writer.WriteAttributeString("name", provinces[list[i]].Name);
							writer.WriteEndElement();
						}
					}
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
		}


		private static void WriteRegions(EU2.Map.IDGrid idgrid, EU2.Data.ProvinceList provinces, IDMap idmap, EU2.IO.CSVWriter writer) {
			writer.EndRow( "region-index-x;region-index-y;region-offset-x;region-offset-x;region-provinceCount;index;provinceId;name" );

			for(int y=0; y<idgrid.Height; ++y ) {
				for(int x=0; x<idgrid.Width; ++x ) {
					ushort[] list = idgrid.GetIDs(x,y);
					if ( list.Length == 0 ) {
						writer.Write(x);
						writer.Write(y);
						writer.Write(x*EU2.Map.IDGrid.RegionWidth);
						writer.Write(y*EU2.Map.IDGrid.RegionHeight);
						writer.Write(0);

						Rectangle rect = Rectangle.Empty;
						if ( !EU2.Map.IDGrid.CheckAreaForOverflow(idmap, EU2.Map.IDGrid.GetRegionRect(x,y), out rect) ) {
							writer.Write("Overflow");
							writer.Write(rect.X);
							writer.Write(rect.Y);
							writer.Write(rect.Width);
							writer.Write(rect.Height);
						}
						writer.EndRow();
					}
					else {
						int i=0;
						for(; i<list.Length; ++i) {
							if ( list[i] == 0 && i>0 ) break;
						}
						int cnt = i;

						for(i=0; i<list.Length; ++i) {
							if ( list[i] == 0 && i>0 ) break;
							writer.Write(x);
							writer.Write(y);
							writer.Write(x*EU2.Map.IDGrid.RegionWidth);
							writer.Write(y*EU2.Map.IDGrid.RegionHeight);
							writer.Write(cnt);
							writer.Write(i);
							writer.Write(list[i]);
							writer.Write(provinces[list[i]].Name);
							writer.EndRow();
						}
					}
				}
			}
			writer.Flush();
		}


		private static void ShowHelp() {
			Console.WriteLine( "Exports statistics (or something) from an EU2Map file." );
			Console.WriteLine( );
			Console.WriteLine( "{0} /BB  <source[.eu2map]> [<destination[.csv|.xml]>] [/O] [/XML|/CSV]", MapToolsVersion.Version.GetToolName().ToUpper() );
			Console.WriteLine( "{0} /ADJ <source[.eu2map]> [<destination[.csv|.xml]>] [/O] [/XML|/CSV]", MapToolsVersion.Version.GetToolName().ToUpper() );
			Console.WriteLine( "{0} /R   <source[.eu2map]> [<destination[.csv|.xml]>] [/O] [/XML|/CSV]", MapToolsVersion.Version.GetToolName().ToUpper() );
			Console.WriteLine( );
			Console.WriteLine( "  /BB             Indicates that you want to export Boundbox info from the file." );
			Console.WriteLine( "  /ADJ            Indicates that you want to export Adjacency info from the file." );
			Console.WriteLine( "  /R              Indicates that you want to export Region info from the file." );
			Console.WriteLine( "  <source>        Specifies the source file (a EU2MAP file)." );
			Console.WriteLine( "  <destination>   Specifies the destination file." );
			Console.WriteLine( "                  If you omit the destination while exporting, one will be generated for you." );
			Console.WriteLine( "  /O              Allows overwriting an existing file when exporting." );
			Console.WriteLine( "  /XML            Specifies that exported data should be in XML format." );
			Console.WriteLine( "  /CSV            Specifies that exported data should be in CSV format." );
			Console.WriteLine( );
			Console.WriteLine( "If you omit the /XML or /CSV option, the program will choose CSV by default." );
		}
	}
}
