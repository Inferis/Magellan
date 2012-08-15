using System;

namespace EU2.Map.Codec
{
	/// <summary>
	/// The GenericMapBlock is a  
	/// </summary>
	public abstract class GenericMapBlock
	{
		protected GenericMapBlock() {
		}

		public abstract bool IsCompressed();
	}
}
