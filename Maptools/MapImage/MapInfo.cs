using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection;
using MapToolsLib;

namespace MapImage
{
	/// <summary>
	/// Summary description for MapInfo.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 4),]
	public struct MapInfo
	{
		private ushort signature;
		private ushort version;
		private int zoom;
		private int x;
		private int y;
		private int width;
		private int height;
		[MarshalAs(UnmanagedType.ByValArray,ArraySubType=UnmanagedType.U1,SizeConst=44)]
		private byte[] buffer;
		[MarshalAs(UnmanagedType.ByValArray,ArraySubType=UnmanagedType.U2,SizeConst=512)]
		private char[] originfile;
		[MarshalAs(UnmanagedType.ByValArray,ArraySubType=UnmanagedType.U2,SizeConst=512)]
		private char[] exportfile;
		[MarshalAs(UnmanagedType.ByValArray,ArraySubType=UnmanagedType.U2,SizeConst=128)]
		private char[] idconvertor;

		public MapInfo( int zoom, int x, int y, int width, int height, string origin, string export, IIDConvertor convertor ) {
			this.signature = 0xE2E1;
			this.version = 1;
			this.zoom = zoom;

			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;

			// reserved buffer
			buffer = new byte[44];

			// strings
			originfile = new char[512];
			exportfile = new char[512];
			idconvertor = new char[128];

			SetString( ref originfile, origin );
			SetString( ref exportfile, export );
			IDConvertor = convertor.GetType();
		}

		public MapInfo( int zoom, Point location, Size size, string origin, string export, IIDConvertor convertor ) 
			: this( zoom, location.X, location.Y, size.Width, size.Height, origin, export, convertor ) {
		}

		public bool CheckSignature() {
			return signature == 0xE2E1;
		}

		public int Version {
			get { return version; }
		}

		public int LightmapZoom {
			get { return zoom; }
			set { zoom = value; }
		}

		public Point Location {
			get { return new Point( x, y ); }
			set { x = value.X; y = value.Y; }
		}

		public int X {
			get { return x; }
			set { x = value; }
		}

		public int Y {
			get { return y; }
			set { y = value; }
		}

		public Size Size {
			get { return new Size( width, height ); }
			set { width = value.Width; height = value.Height; }
		}

		public Rectangle Bounds {
			get { return new Rectangle( x, y, width, height ); }
		}

		public int Width {
			get { return width; }
			set { width = value; }
		}

		public int Height {
			get { return height; }
			set { height = value; }
		}

		public string Origin {
			get { return GetString( originfile ); }
			set { SetString( ref originfile, value ); }
		}

		public string Export {
			get { return GetString( exportfile ); }
			set { SetString( ref exportfile, value ); }
		}

		public Type IDConvertor {
			get { 
				string name = GetString( idconvertor );
				if ( name == null || name.Length == 0 ) return null;
				name = name.Split( ',' )[0];
				return Assembly.GetAssembly(typeof(IIDConvertor)).GetType( name, false );
				//return Type.GetType( name, false );
			}
			set { SetString( ref idconvertor, value.AssemblyQualifiedName ); }
		}

		private string GetString( char[] buffer ) {
			return new string( buffer, 1, ((int)buffer[0]) ); 
		}

		private void SetString( ref char[] buffer, string src ) {
			buffer[0] = (char)src.Length;
			Array.Clear( buffer, 1, buffer.Length-1 );
			src.CopyTo( 0, buffer, 1, src.Length );
		}

		public Bitmap AsBitmap() {
			Bitmap result = new Bitmap( 32, 32 );

			BitmapData locked = result.LockBits( new Rectangle( 0, 0, result.Width, result.Height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
			Marshal.StructureToPtr( this, locked.Scan0, false );
			result.UnlockBits( locked );
			
			return result;
		}

		public static MapInfo FromBitmap( Bitmap source ) {
			BitmapData locked = source.LockBits( new Rectangle( 0, 0, source.Width, source.Height ), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );
			MapInfo result = (MapInfo)Marshal.PtrToStructure( locked.Scan0, typeof(MapInfo) );
			source.UnlockBits( locked );

			return result;
		}
	}
}
