using System;
using EU2.IO;

namespace EU2.Bare
{
	/// <summary>
	/// Summary description for PublicEnums.
	/// </summary>
	public class Religion : ICSVReadable
	{
		private string name;
		private int techSpeed;
		private int stabBonus;
		private int productionEfficiency;
		private int tradeEfficiency;
		private int taxIncome;
		private int morale;
		private int annualColonistRatio;
		private int annualDiplomatRatio;
		private int annualMissionaryRatio;

		public Religion( ) {
			name = "";
			techSpeed = -1;
			stabBonus = -1;
			productionEfficiency = -1;
			tradeEfficiency = -1;
			taxIncome = -1;
			morale = -1;
			annualColonistRatio = -1;
			annualDiplomatRatio = -1;
			annualMissionaryRatio = -1;
		}

		public Religion( string name ) {
			this.name = name;
			techSpeed = -1;
			stabBonus = -1;
			productionEfficiency = -1;
			tradeEfficiency = -1;
			taxIncome = -1;
			morale = -1;
			annualColonistRatio = -1;
			annualDiplomatRatio = -1;
			annualMissionaryRatio = -1;
		}

		#region Properties
		public string Name { 
			get { return name; }
			set { name = value; }
		}

		public int TechSpeed { 
			get { return techSpeed; }
			set { techSpeed = value; }
		}

		public int StabilityBonus { 
			get { return stabBonus; }
			set { stabBonus = value; }
		}

		public int ProductionEfficiency { 
			get { return productionEfficiency; }
			set { productionEfficiency = value; }
		}

		public int TradeEfficiency { 
			get { return tradeEfficiency; }
			set { tradeEfficiency = value; }
		}

		public int TaxIncome { 
			get { return taxIncome; }
			set { taxIncome = value; }
		}

		public int Morale { 
			get { return morale; }
			set { morale = Morale; }
		}

		public int AnnualColonistRatio { 
			get { return annualColonistRatio; }
			set { annualColonistRatio = value; }
		}

		public int AnnualDiplomatRatio { 
			get { return annualDiplomatRatio; }
			set { annualDiplomatRatio = value; }
		}

		public int AnnualMissionaryRatio { 
			get { return annualMissionaryRatio; }
			set { annualMissionaryRatio = value; }
		}
		#endregion

		public bool ReadFromCSV( CSVReader reader ) {
			string tmp = reader.ReadString();

			if ( tmp.ToUpper() == "END" ) return false;
			name = tmp;
			techSpeed = reader.ReadInt();
			stabBonus = reader.ReadInt();
			productionEfficiency = reader.ReadInt();
			tradeEfficiency = reader.ReadInt();
			taxIncome = reader.ReadInt();
			morale = reader.ReadInt();
			annualColonistRatio = reader.ReadInt();
			annualDiplomatRatio = reader.ReadInt();
			annualMissionaryRatio = reader.ReadInt();
			reader.SkipRow();

			return true;
		}
	}

	public class UnknownReligionException : Exception {};
}
