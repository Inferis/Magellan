using System;
using System.Drawing;
using EU2.IO;

namespace EU2.Bare
{
	/// <summary>
	/// Summary description for Terrain.
	/// </summary>
	public class Terrain : ICSVReadable
	{
		int id;
		string name;
		float movement;

		public Terrain() {
			this.id = -1;
			this.name = "";
			this.movement = -1;
		}

		public Terrain( int id ) {
			this.id = id;
			this.name = "";
			this.movement = -1;
		}

		public Terrain( int id, string name, float movement ) {
			this.id = id;
			this.name = name;
			this.movement = movement;
		}

		public bool ReadFromCSV( CSVReader reader ) {
			id = reader.ReadInt();
			name = reader.ReadString();
			movement = reader.ReadFloat();

			return true;
		}

		#region Properties
		public int ID {
			get { return id; }
			set { id = ID; }
		}

		public string Name {
			get { return name; }
			set { name = Name; }
		}

		public float Movement {
			get { return movement; }
			set { movement = Movement; }
		}

		public Color Color {
			get { return LookupColor( this ); }
		}
		#endregion

		public static Color LookupColor( Terrain terrain ) {
			return LookupColor( terrain, System.Drawing.Color.Black );
		}

		public static Color LookupColor( Terrain terrain, Color notFoundColor ) {
			switch ( terrain.ID ) {
				case 0:		return Color.DarkKhaki;
				case 1:		return Color.ForestGreen;
				case 2:		return Color.Brown;
				case 3:		return Color.Gold;
				case 4:		return Color.OliveDrab;
				case 5:		return Color.RoyalBlue;
				case 6:		return Color.SkyBlue;
				case 7:		return Color.Red;
				case 8:		return Color.AntiqueWhite;
				default:	return notFoundColor;
			}
		}
	}
}
