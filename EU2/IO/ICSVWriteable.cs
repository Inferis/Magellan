using System;
using System.IO;

namespace EU2.IO
{
	/// <summary>
	/// </summary>
	public interface IStreamWriteable {
		
		void WriteTo( Stream stream );

	}

}
