using System;

namespace EU2
{
	/// <summary>
	/// Summary description for InvalidZoomFactorException.
	/// </summary>
	public class InvalidZoomFactorException : Exception {
		public InvalidZoomFactorException( int zoom ) : base( "The zoom factor \"" + zoom + "\" specified is invalid." ) {
			this.zoom = zoom;
		}

		public int Zoom {
			get { return zoom; }
		}

		int zoom;
	}
}
