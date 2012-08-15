using System;
using System.Drawing;

namespace MapExplorer
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public sealed class Utils {

		public static Bitmap GetImageFromResource( string name ) {
			System.IO.Stream rcStream = null;

			try {
				rcStream = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream( name );
				if ( rcStream == null ) return null;
				return new Bitmap( rcStream );
			}
			catch {
				return null;
			}
			finally {
				if ( rcStream != null ) rcStream.Close();
			}
		}
	}
}
