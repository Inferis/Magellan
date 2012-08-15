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
	/// Summary description for ExportImage.
	/// </summary>
	public sealed class ExportImage
	{
		public static void Run( string source, string target, Rectangle region, RegionMode mode, int zoom, PngLevel pngLevel, bool direct ) {
			if ( (mode == RegionMode.Normal && region.IsEmpty) || (mode == RegionMode.All && region.Width <= 0 ) ) {
				Console.WriteLine( "No region was specified. Use \"/r:<x,y,width,height>\", \"/r:full\" or \"/r:<size>\" to specify the region to export." );
				return;
			}

			Console.WriteLine( "Opening source file \"{0}\"...", Path.GetFileName( source ) );

			region.Width -= 1;
			region.Height -= 1;

			IFile file;
			if ( direct ) {
				file = new EU2.Edit.EU2Proxy();
				file.ReadFrom( source ); // source is EU2 dir
			}
			else {
				file = new EU2.Edit.File();
				file.ReadFrom( source );
			}

			Lightmap map = null;
			switch ( zoom ) {
				case 1: 
					map = file.Lightmap1; break;
				case 2: 
					map = file.Lightmap2; break;
				case 3: 
					map = file.Lightmap3; break;
			}

			if ( map == null ) {
				Console.WriteLine( "The source file does not contain lightmap{0}.", zoom );
				return;
			}

			map.VolatileDecompression = true;

			if ( mode == RegionMode.All ) {
				int size = ((region.Width+Lightmap.BlockSize-1) >> Lightmap.BlockFactor) << Lightmap.BlockFactor;
				Console.WriteLine( "Exporting full map as separate files of size {0}x{0}.", size );
				for ( int y=0; y<map.Size.Height; y+=size ) {
					for ( int x=0; x<map.Size.Width; x+=size ) {
						region = new Rectangle( x, y, size, size );
						string parttarget = Path.Combine( Path.GetDirectoryName( target ), 
							Path.GetFileNameWithoutExtension( target ) + 
							String.Format( "-[{0},{1}-{2},{3}]", region.Left, region.Top, region.Right, region.Bottom ) +
							Path.GetExtension( target ) );
						DoExport( map, region, pngLevel, file.IDMap, Path.GetFileName( source ), parttarget ); 
					}
				}
			}
			else if ( mode == RegionMode.Full ) {
				Console.WriteLine( "Exporting full map as one file." );
				region = new Rectangle( new Point( 0, 0 ), map.Size );
				DoExport( map, region, pngLevel, file.IDMap, Path.GetFileName( source ), target ); 
			}
			else {
				Console.WriteLine( "Exporting map region: ({0},{1})-({2},{3}) (width={4}, height={5}).", region.Left, region.Top, region.Right, region.Bottom, region.Width, region.Height );
                DoExport(map, region, pngLevel, file.IDMap, Path.GetFileName(source), target); 
			}

			Console.Write( "Export successful." );
		}

		private static void DoExport( Lightmap map, Rectangle region, PngLevel pngLevel, IDMap idmap, string sourcefile, string target ) {
			Console.WriteLine( "Decoding image..." );
			RawImage rawimg = map.DecodeImage( region );
			int[] shadebuffer, idbuffer, borderbuffer;
			new IOShader( Boot.DefaultConvertor ).MultiShade32( rawimg, out shadebuffer, out idbuffer, out borderbuffer );

			// We don't use the lightmap version as a source: this can lead to errors. 
			// Use the idmap as source. For lightmap2 and lightmap3, this needs to be scaled down.
			// -- NOTE: if this is changed, don't forget to turn on the id converting in the IOShader.Multishade above!
			ushort[] tmpbuffer = null;
				
			if ( map.Zoom == 0 ) {
				tmpbuffer = idmap.ExportBitmapBuffer( rawimg.Bounds );
			}
			else {
				Console.WriteLine( "Scaling idmap..." );
				tmpbuffer = MapToolsLib.Utils.ScaleIDBuffer( idmap.ExportBitmapGrid( map.CoordMap.ZoomedToActual( rawimg.Bounds ) ), map.Zoom );
			}

			// Convert it to the map format
			for ( int i=0; i<idbuffer.Length; ++i ) {
				if ( tmpbuffer[i] >= Province.Count ) tmpbuffer[i] = Province.TerraIncognitaID;
				idbuffer[i] = Boot.DefaultConvertor.ConvertID( tmpbuffer[i] );
			}

			if ( pngLevel != PngLevel.None ) {
				string basetarget = Path.ChangeExtension( Path.GetFileNameWithoutExtension( target ) + "-###", Path.GetExtension( target ) );

                if ((pngLevel & PngLevel.Shading) > 0) {
                    target = basetarget.Replace("###", Boot.ShadingLayerName);
                    Console.WriteLine("Exporting to \"{0}\"...", Path.GetFileName(target));
                    Visualiser.CreateImage32(shadebuffer, rawimg.Size).Save(target, ImageFormat.Png);
                }

                if ((pngLevel & PngLevel.IDs) > 0) {
                    target = basetarget.Replace("###", Boot.IDLayerName);
                    Console.WriteLine("Exporting to \"{0}\"...", Path.GetFileName(target));
                    Visualiser.CreateImage32(idbuffer, rawimg.Size).Save(target, ImageFormat.Png);
                }

                if ((pngLevel & PngLevel.Borders) > 0) {
                    target = basetarget.Replace("###", Boot.BorderLayerName);
                    Console.WriteLine("Exporting to \"{0}\"...", Path.GetFileName(target));
                    Visualiser.CreateImage32(borderbuffer, rawimg.Size).Save(target, ImageFormat.Png);
                }
			}
			else {
				Console.WriteLine( "Exporting to \"{0}\"...", Path.GetFileName( target ) );

				PSD.File psd = new PSD.File( rawimg.Size );

				Bitmap img = new MapInfo( map.Zoom, rawimg.Location, rawimg.Size, sourcefile, Path.GetFileName( target ), Boot.DefaultConvertor ).AsBitmap();
				PSD.Layer l = psd.Layers.Add( new PSD.Layer( Boot.MapInfoLayerName, 0, 0, img ) );
				l.Opacity = 255;
				l.Visible = false;
				l.ProtectTransparancy = true;

				img = Visualiser.CreateImage32( shadebuffer, rawimg.Size );
				l = psd.Layers.Add( new PSD.Layer( Boot.ShadingLayerName, 0, 0, img ) );
				l.Opacity = 254;
				l.Visible = true;
				l.ProtectTransparancy = false;

				img = Visualiser.CreateImage32( idbuffer, rawimg.Size );

				l = psd.Layers.Add( new PSD.Layer( Boot.IDLayerName, 0, 0, img ) );
				l.Opacity = 254;
				l.Mode = PSD.LayerMode.Overlay;
				l.Visible = true;
				l.ProtectTransparancy = false;

				img = Visualiser.CreateImage32( borderbuffer, rawimg.Size, true );
				l = psd.Layers.Add( new PSD.Layer( Boot.BorderLayerName, 0, 0, img ) );
				l.Opacity = 254;
				l.Visible = true;
				l.ProtectTransparancy = false;

				// -- Write the psd
				FileStream stream = null;
				try {
					stream = new FileStream( target, FileMode.Create, FileAccess.Write, FileShare.None );
					psd.WriteTo( new BinaryWriter( stream ) );
				}
				finally {
					if ( stream != null ) stream.Close();
				}
			}
		}


	}
}
