using System;
using EU2.Map.Drawing;
using System.Collections.Generic;

namespace EU2.Enums
{
	/// <summary>
	/// Summary description for Religion.
	/// </summary>
	public class Terrain {
		public Terrain( int id, string name, MapColor color ) {
			this.id = id;
			this.name = name;
			this.color = color;
		}

		public int ID { get { return id; } }
		public string Name { get { return name; } }
		public MapColor Color { get { return color; } }

		public override string ToString() {
			return name;
		}

		#region Static Stuff
		public static Terrain FromID( int id ) {
			if ( id >= 0 && id < terrains.Length ) return terrains[id];
			return null;
		}

		public static Terrain Border			{ get { return terrains[7]; } }
		public static Terrain Desert			{ get { return terrains[3]; } }
		public static Terrain Forest			{ get { return terrains[1]; } }
		public static Terrain Marsh				{ get { return terrains[4]; } }
		public static Terrain Mountain			{ get { return terrains[2]; } }
		public static Terrain Ocean				{ get { return terrains[5]; } }
		public static Terrain Plains			{ get { return terrains[0]; } }
		public static Terrain River				{ get { return terrains[6]; } }
		public static Terrain TerraIncognita	{ get { return terrains[8]; } }
        public static Terrain Unknown { get { return terrains[9]; } }
        public static IEnumerable<Terrain> All { get { return terrains; } }

		private static Terrain[] terrains = new Terrain[10] {
			new Terrain( 0, "Plains", MapColor.Orange ), new Terrain( 1, "Forest", MapColor.Green ), new Terrain( 2, "Mountain", MapColor.DarkOrange ), 
			new Terrain( 3, "Desert", MapColor.Yellow ), new Terrain( 4, "Marsh", MapColor.LightGreen ), new Terrain( 5, "Ocean", MapColor.Water ), 
			new Terrain( 6, "River", MapColor.Water ), new Terrain( 7, "Border", MapColor.Border ), new Terrain( 8, "Terra Incognita", MapColor.Black ), 
			new Terrain( 9, "Unknown", MapColor.Black )
		};

		#endregion

		private int id;
		private MapColor color;
		private string name;
	}
}
