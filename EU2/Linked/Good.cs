using System;
using System.IO;

namespace EU2.Data
{
	/// <summary>
	/// Summary description for Good.
	/// </summary>
	public class Good : EU2.CSVBasedItem, IComparable
	{
		string name;
		int baseResourceValue;
		bool mine, tradePost;
		int numOfSlaves;

		public Good( EU2.Install install, string name, int baseResourceValue ) : base( install )  {
			this.name = name;
			this.baseResourceValue = baseResourceValue;
			this.mine = false;
			this.tradePost = false;
			this.numOfSlaves = 0;
		}

		public Good( EU2.Install install, string name, int baseResourceValue, bool mine, bool tradePost, int numOfSlaves ) : base( install ) {
			this.name = name;
			this.baseResourceValue = baseResourceValue;
			this.mine = mine;
			this.tradePost = tradePost;
			this.numOfSlaves = numOfSlaves;
		}

		public Good( EU2.Install install, System.IO.StreamReader reader ) : base( install, reader ) {
		}

		#region CSVBasedItem implementation
		public override bool ReadFrom( System.IO.StreamReader reader ) {
			// Get a line from the stream, leave if we're at the end
			string line = reader.ReadLine();
			if ( line == null ) return false;

			// Convert line to csv values
			string[] data = EU2.CSVUtils.CSVValues( line );

			// fill properties
			name = data[0];
			baseResourceValue = System.Convert.ToInt32( data[1] );
			mine = System.Convert.ToBoolean( data[2] );
			tradePost = System.Convert.ToBoolean( data[3] );
			numOfSlaves = System.Convert.ToInt32( data[4] );

			return true;
		}

		public override void WriteTo( System.IO.StreamWriter writer ) {
			writer.WriteLine( "{0};{1};{2};{3};{4}",
				EU2.CSVUtils.CSVString( name ),
				baseResourceValue, mine ? 1 : 0, tradePost ? 1 : 0, numOfSlaves );
		}

		public override object CSVBasedKey {
			get { return name; }
		}
		#endregion
		
		#region Properties
		public string Name {
			get { return name; }
			set { name = Name; }
		}

		public int BaseResourceValue {
			get { return baseResourceValue; }
			set { baseResourceValue = BaseResourceValue; }
		}
	
		public bool Mine {
			get { return mine; }
			set { mine = Mine; }
		}
	
		public bool TradePost {
			get { return tradePost; }
			set { tradePost = TradePost; }
		}
	
		public int NumOfSlaves {
			get { return numOfSlaves; }
			set { numOfSlaves = NumOfSlaves; }
		}
		#endregion

		public int CompareTo(object obj) {
			if ( !obj.GetType().IsInstanceOfType( this ) ) throw new ArgumentException();

			Good good = (Good)obj;
			int r;
			if ( (r=name.CompareTo( good.name )) == 0 ) {
				r = baseResourceValue.CompareTo( good.baseResourceValue );
			}

			return r;
		}
	}
}
