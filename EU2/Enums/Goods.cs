using System;
using System.Collections;

namespace EU2.Enums
{
	/// <summary>
	/// Summary description for Religion.
	/// </summary>
	public class Goods {
		private string name;

		public Goods( string name ) {
			this.name = name;
		}

		public string Name { get { return name; } }

		#region Static Stuff
		public static Goods FromName( string name ) {
			name = name.ToLower();
			if ( goods.Contains( name ) ) return (Goods)goods[name];
			return (Goods)goods["nothing"];
		}

		public static Goods Nothing			{ get { return (Goods)goods[NothingTag]; } }
		public static Goods Coffee			{ get { return (Goods)goods[CoffeeTag]; } }
		public static Goods Cotton			{ get { return (Goods)goods[ClothTag]; } }
		public static Goods Cloth			{ get { return (Goods)goods[CottonTag]; } }
		public static Goods Grain			{ get { return (Goods)goods[GrainTag]; } }
		public static Goods Gold			{ get { return (Goods)goods[GoldTag]; } }
		public static Goods Fish			{ get { return (Goods)goods[FishTag]; } }
		public static Goods Furs			{ get { return (Goods)goods[FursTag]; } }
		public static Goods Ivory			{ get { return (Goods)goods[IvoryTag]; } }
		public static Goods Metal			{ get { return (Goods)goods[MetalTag]; } }
		public static Goods Minerals		{ get { return (Goods)goods[MineralsTag]; } }
		public static Goods NavalSupplies	{ get { return (Goods)goods[NavalSuppliesTag]; } }
		public static Goods Oriental		{ get { return (Goods)goods[OrientalTag]; } }
		public static Goods Slaves			{ get { return (Goods)goods[SlavesTag]; } }
		public static Goods Salt			{ get { return (Goods)goods[SaltTag]; } }
		public static Goods Spices			{ get { return (Goods)goods[SpicesTag]; } }
		public static Goods Sugar			{ get { return (Goods)goods[SugarTag]; } }
		public static Goods Tea				{ get { return (Goods)goods[TeaTag]; } }
		public static Goods Tobacoo			{ get { return (Goods)goods[TobaccoTag]; } }
		public static Goods Wine			{ get { return (Goods)goods[WineTag]; } }
		public static Goods Wool			{ get { return (Goods)goods[WoolTag]; } }

		static Goods( ) {
			goods = new Hashtable();
			Add( new Goods( NothingTag ) );
			Add( new Goods( CoffeeTag ) );
			Add( new Goods( ClothTag ) );
			Add( new Goods( CottonTag ) );
			Add( new Goods( GrainTag ) );
			Add( new Goods( GoldTag ) );
			Add( new Goods( FishTag ) );
			Add( new Goods( FursTag ) );
			Add( new Goods( IvoryTag ) );
			Add( new Goods( MetalTag ) );
			Add( new Goods( MineralsTag ) );
			Add( new Goods( NavalSuppliesTag ) );
			Add( new Goods( OrientalTag ) );
			Add( new Goods( SlavesTag ) );
			Add( new Goods( SaltTag ) );
			Add( new Goods( SpicesTag ) );
			Add( new Goods( SugarTag ) );
			Add( new Goods( TeaTag ) );
			Add( new Goods( TobaccoTag ) );
			Add( new Goods( WineTag ) );
			Add( new Goods( WoolTag ) );
		}

		internal static void Add( Goods good ) {
			goods.Add( good.Name, good );
		}

		internal const string NothingTag = "nothing";
		internal const string CoffeeTag = "coffee";
		internal const string ClothTag = "clo";
		internal const string CottonTag = "cot";
		internal const string GrainTag = "grai";
		internal const string GoldTag = "gold";
		internal const string FishTag = "fish";
		internal const string FursTag = "furs";
		internal const string IvoryTag = "ivor";
		internal const string MetalTag = "metal";
		internal const string MineralsTag = "mineral";
		internal const string NavalSuppliesTag = "navs";
		internal const string OrientalTag = "orient";
		internal const string SlavesTag = "slav";
		internal const string SaltTag = "salt";
		internal const string SpicesTag = "spic";
		internal const string SugarTag = "sug";
		internal const string TeaTag = "tea";
		internal const string TobaccoTag = "tob";
		internal const string WineTag = "wine";
		internal const string WoolTag = "wool";

		private static Hashtable goods;
		#endregion
	}
}
