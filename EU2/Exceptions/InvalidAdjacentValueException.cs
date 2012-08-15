using System;

namespace EU2
{
	/// <summary>
	/// Summary description for InvalidAdjacentValueException.
	/// </summary>
	public class InvalidAdjacentValueException : Exception
	{
		public InvalidAdjacentValueException( int adjacentValue ) : base( "An invalid adjacent value was encountered." ) {
			adj = adjacentValue;
		}

		public InvalidAdjacentValueException( int adjacentValue, string message ) : base( message ) {
			adj = adjacentValue;
		}

		public int AdjacentValue {
			get { return adj; }
		}

		private int adj;
	}
}
