using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using EU2;
using EU2.Edit;
using EU2.Data;
using EU2.Map;
using EU2.Map.Codec;
using EU2.Map.Drawing;
using MapToolsLib;

namespace MapImage
{
	/// <summary>
	/// Summary description for QueryImage.
	/// </summary>
	public sealed class QueryImage
	{
		public static PSD.File Run( string source ) {
			Console.WriteLine( "Opening source file \"{0}\"...", Path.GetFileName( source ) );

			PSD.File psd = new PSD.File( source );
			if ( psd.Layers.Length < 4 ) {
				Console.WriteLine( "Error: The source file must contain at least 4 layers." );
				return null;
			}

			if ( !psd.Layers.Contains( Boot.MapInfoLayerName ) ) {
				Console.WriteLine( "Error: There is no \"MapInfo\" layer in the source file." );
				return null;
			}

			MapInfo info = MapInfo.FromBitmap( psd.Layers[Boot.MapInfoLayerName].Image );
			if ( !info.CheckSignature() ) {
				Console.WriteLine( "Error: The MapInfo layer is incorrect (signature error)." );
				return null;
			}

			Console.WriteLine( "Source file info:" );
			Console.WriteLine( "* Zoom: lightmap{0}", (info.LightmapZoom+1) );
			Console.WriteLine( "* Image Origin: {0}, {1}", info.X, info.Y );
			Console.WriteLine( "* Image Size: {0}, {1}", info.Width, info.Height );
			Console.WriteLine( "* Extracted from file: {0}", info.Origin );
			Console.WriteLine( "* Original source filename: {0}", info.Export );
			Console.Write( "* IDConvertor Type: " );
			if ( info.IDConvertor == null ) 
				Console.WriteLine( "(none or invalid reference)" );
			else {
				try {
					Console.WriteLine( info.IDConvertor.GetConstructor( Type.EmptyTypes ).Invoke( null ).ToString() );
				} 
				catch {
					Console.WriteLine( "(invalid reference)" );
				}
			}
			Console.WriteLine( "* MapInfo version: {0}", (info.Version) );

			return psd;
		}		
	}
}
