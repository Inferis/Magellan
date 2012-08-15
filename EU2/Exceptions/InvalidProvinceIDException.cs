using System;

namespace EU2
{
	/// <summary>
	/// Summary description for InvalidProvinceIDException.
	/// </summary>
	public class InvalidProvinceIDException : Exception
	{
		public InvalidProvinceIDException( int id ) : base( "The province ID " + id + " is invalid." ) {
			this.id = (ushort)id;
		}

		public int ID {
			get { return id; }
		}

		private int id = 0;
	}
}
