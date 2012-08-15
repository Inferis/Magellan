using System;

namespace EU2.Map.Drawing
{
	/// <summary>
	/// Summary description for LightLevels.
	/// </summary>
	public class LightLevels
	{
		public const int Count = 31;

		public static int Level( int percent ) { return (int)(percent*Count/100); }
		public static int Level( double percent ) { return (int)(percent*Count/100); }

		public static int Level0 { get { return (0*Count/100); } }
		public static int Level10 { get { return (10*Count/100); } }
		public static int Level20 { get { return (20*Count/100); } }
		public static int Level30 { get { return (30*Count/100); } }
		public static int Level40 { get { return (40*Count/100); } }
		public static int Level50 { get { return (50*Count/100); } }
		public static int Level60 { get { return (60*Count/100); } }
		public static int Level70 { get { return (70*Count/100); } }
		public static int Level80 { get { return (80*Count/100); } }
		public static int Level90 { get { return (90*Count/100); } }
		public static int Level100 { get { return (100*Count/100); } }
	}
}
