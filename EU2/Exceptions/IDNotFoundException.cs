using System;

namespace EU2
{
	/// <summary>
	/// Summary description for IDNotFoundException.
	/// </summary>
	public class IDNotFoundException : Exception
	{
		public IDNotFoundException( ushort id ) : base( "The ID was not found." ) {
			this.id = id;
		}

		public ushort ID {
			get { return id; }
		}
		
		private ushort id;
	}
}
