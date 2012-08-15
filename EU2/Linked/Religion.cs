using System;

namespace EU2.Data
{
	/// <summary>
	/// Summary description for PublicEnums.
	/// </summary>
	public abstract class Religion : EU2.CSVBasedItem
	{
		private int index;
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

		public Religion( EU2.Install install ) : base( install ) {
			name = "";
		}

		public Religion( EU2.Install install, System.IO.StreamReader reader ) : base( install, reader ) {
		}

		#region Properties
		public string Name { 
			get { return name; }
			set { name = Name; }
		}

		public int TechSpeed { 
			get { return techSpeed; }
			set { techSpeed = TechSpeed; }
		}

		public int StabilityBonus { 
			get { return stabBonus; }
			set { stabBonus = StabilityBonus; }
		}

		public int ProductionEfficiency { 
			get { return productionEfficiency; }
			set { productionEfficiency = ProductionEfficiency; }
		}

		public int TradeEfficiency { 
			get { return tradeEfficiency; }
			set { tradeEfficiency = TradeEfficiency; }
		}

		public int TaxIncome { 
			get { return taxIncome; }
			set { taxIncome = TaxIncome; }
		}

		public int Morale { 
			get { return morale; }
			set { morale = Morale; }
		}

		public int AnnualColonistRatio { 
			get { return annualColonistRatio; }
			set { annualColonistRatio = AnnualColonistRatio; }
		}

		public int AnnualDiplomatRatio { 
			get { return annualDiplomatRatio; }
			set { annualDiplomatRatio = AnnualDiplomatRatio; }
		}

		public int AnnualMissionaryRatio { 
			get { return annualMissionaryRatio; }
			set { annualMissionaryRatio = AnnualMissionaryRatio; }
		}
		#endregion

		#region ICSVBasedItem implementation
		public override bool ReadFrom(System.IO.StreamReader reader) {
			// Get a line from the stream, leave if we're at the end
			string line = reader.ReadLine();
			if ( line == null ) return false;

			// Convert line to csv values
			string[] data = EU2.CSVUtils.CSVValues( line );

			// fill properties
			name = data[0];
			techSpeed = System.Convert.ToInt32( data[1] );
			stabBonus = System.Convert.ToInt32( data[2] );
			productionEfficiency = System.Convert.ToInt32( data[3] );
			tradeEfficiency = System.Convert.ToInt32( data[4] );
			taxIncome = System.Convert.ToInt32( data[5] );
			morale = System.Convert.ToInt32( data[6] );
			annualColonistRatio = System.Convert.ToInt32( data[7] );
			annualDiplomatRatio = System.Convert.ToInt32( data[8] );
			annualMissionaryRatio = System.Convert.ToInt32( data[9] );

			return true;
		}

		public override  void WriteTo(System.IO.StreamWriter writer) {
			writer.WriteLine( "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};x",
				EU2.CSVUtils.CSVString( name ),
				techSpeed, stabBonus, productionEfficiency, tradeEfficiency, 
				taxIncome, morale, annualColonistRatio, annualDiplomatRatio, 
				annualMissionaryRatio );
		}

		public override object CSVBasedKey {
			get { return name; }
		}
		#endregion
	}

	public class UnknownReligionException : Exception {};
}
