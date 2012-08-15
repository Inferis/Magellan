using System;
using EU2.IO;

namespace EU2.Bare
{
	/// <summary>
	/// Summary description for Good.
	/// </summary>
	public class Good : ICSVReadable
	{
		string name;
		int baseResourceValue;
		bool mine, tradePost;
		int numOfSlaves;

		public Good( )  {
			this.name = "";
			this.baseResourceValue = -1;
			this.mine = false;
			this.tradePost = false;
			this.numOfSlaves = -1;
		}

		public Good( string name )  {
			if ( !IsValidName( name ) ) throw new ArgumentOutOfRangeException();

			this.name = name.ToLower();
			this.baseResourceValue = -1;
			this.mine = false;
			this.tradePost = false;
			this.numOfSlaves = -1;
		}

		public bool ReadFromCSV( CSVReader reader ) {
			name = reader.ReadString();
			baseResourceValue = reader.ReadInt();
			mine = reader.ReadBoolean();
			tradePost = reader.ReadBoolean();
			numOfSlaves = reader.ReadInt();

			return true;
		}

		#region Properties
		public string Name {
			get { return name; }
			set { name = value; }
		}

		public int BaseResourceValue {
			get { return baseResourceValue; }
			set { baseResourceValue = value; }
		}
	
		public bool Mine {
			get { return mine; }
			set { mine = value; }
		}
	
		public bool TradePost {
			get { return tradePost; }
			set { tradePost = value; }
		}
	
		public int NumOfSlaves {
			get { return numOfSlaves; }
			set { numOfSlaves = value; }
		}
		#endregion

		public static bool IsValidName( string name ) {
			name = name.ToLower();
			return ( name == "nothing" || name == "coffee" || name == "cot" || name == "grai" || name == "gold" ||  
                     name == "fish" || name == "furs" || name == "ivor" || name == "metal" || name == "mineral" || 
                     name == "navs" || name == "orient" || name == "slav" || name == "salt" || name == "spic" || 
					 name == "sug" || name == "tea" || name == "tob" || name == "wine" || name == "wool" );
		}
	}
}
