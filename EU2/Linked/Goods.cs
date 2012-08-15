using System;

namespace EU2.Data
{
	/// <summary>
	/// Summary description for Goods.
	/// </summary>
	public class Goods : CSVBasedList
	{
		private const string CSVHEADER = "Goods;Base Resource Value;Mine;TradePost;NumOfSlaves";

		public Goods( EU2.Install install )  : base( install, CSVHEADER, typeof( EU2.Data.Good ) ) {
			ReadFrom( Install.DBPath + "\\goods.csv" );
		}
	}
}
