using System;
using EU2.Data;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for OwnerInfo.
	/// </summary>
	public struct OwnerInfo
	{
		public static OwnerInfo None = new OwnerInfo( Province.TerraIncognitaID );

		public OwnerInfo( ushort provinceId ) {
			this.ProvinceId = provinceId;
			this.Border = false;
		}

		public OwnerInfo( ushort provinceId, bool border ) {
			this.ProvinceId = provinceId;
			this.Border = border;
		}

		public ushort ProvinceId;
		public bool Border;
	}
}
