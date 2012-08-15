using System;
using System.IO;
using System.Drawing;
using System.Collections;
using EU2.Data;
using EU2.IO;

namespace EU2.Map {
	/// <summary>
	/// Summary description for IDGrid.
	/// </summary>
	public class IDGrid : IStreamWriteable {
		public const int RegionWidth = 2048;
		public const int RegionHeight = 2048;
		public const int RegionSize = 256;

		private ushort[] grid;
		private int width, height;

		public IDGrid() {
			width = (int)((Lightmap.BaseWidth+RegionWidth) / RegionWidth);
			height = (int)((Lightmap.BaseHeight+RegionHeight) / RegionHeight);

			int regioncount = width * height;
			grid = new ushort[regioncount*RegionSize];
		}

		public IDGrid( BinaryReader reader ) {
			width = (int)((Lightmap.BaseWidth+RegionWidth) / RegionWidth);
			height = (int)((Lightmap.BaseHeight+RegionHeight) / RegionHeight);

			int regioncount = width * height;
			grid = new ushort[regioncount*RegionSize];

			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			for ( int i=0; i<grid.Length; ++i ) {
				grid[i] = (ushort)(reader.ReadInt32( ));
			}
		}

		public void WriteTo( BinaryWriter writer ) {
			for ( int i=0; i<grid.Length; ++i ) {
				writer.Write( (int)grid[i] );
			}
		}

		public void WriteTo( Stream stream ) {
			WriteTo( new BinaryWriter( stream ) );
		}

		public int Width {
			get { return width; }
		}

		public int Height {
			get { return height; }
		}

		public Size Size {
			get { return new Size(width, height); }
		}

		public byte MapID( int x, int y, ushort realID ) {
			x /= RegionWidth;
			y /= RegionHeight;

			ushort[] idlist = GetIDs( x, y );
			for ( int i=0; i<RegionSize; ++i ) {
				if ( idlist[i] == realID ) return (byte)i;
			}

			// Shouldn't be possible... but anyway.
			throw new IDNotFoundException( realID );
		}

		public ushort RealID( int x, int y, byte gridID ) {
			x /= RegionWidth;
			y /= RegionHeight;

			return GetIDs( x, y )[gridID];
		}

		public void BuildRegion( Point region, IDMap idmap ) {
			BuildRegion( region.X, region.Y, idmap );
		}

		public void BuildRegion( int regionx, int regiony, IDMap idmap ) {
			BuildRegion(regionx, regiony, idmap, false);
		}

		public void BuildRegion( int regionx, int regiony, IDMap idmap, bool dontThrow ) {
			Rectangle regionRect = new Rectangle( (regionx*RegionWidth)-256, (regiony*RegionHeight)-256, RegionWidth+512, RegionHeight+512 );
			ushort[] idlist = idmap.GetIDs( regionRect.X, regionRect.Y, regionRect.Width, regionRect.Height, GetIDOptions.SkipTI, null );
			//ushort[] idlist = idmap.GetIDs( regionx*RegionWidth, regiony*RegionHeight, RegionWidth, RegionHeight, false, false );
			if ( idlist.Length > RegionSize ) {
				if ( dontThrow ) return;
				throw new RegionSizeOverflowException( regionRect ); 
			}

			// Reset ids for this region
			int offset = GetOffsetFromRegion( regionx, regiony );
			for ( int i=0; i<RegionSize; ++i ) grid[offset+i] = Province.TerraIncognitaID;

			// Don't forget to sort...
			if ( idlist.Length > 0 ) {
				Array.Sort( idlist, new IDComparer() );
				if ( idlist[idlist.Length-1] == Province.MaxValue ) {
					ushort[] idlist2 = new ushort[idlist.Length-1];
					Array.Copy( idlist, 0, idlist2, 0, idlist2.Length );
					idlist = idlist2;
				}
			}

			// Check again
			if ( idlist.Length == 0 )
				idlist = new ushort[] { Province.TerraIncognitaID };

			// copy the list... 
			if ( idlist[0] == Province.TerraIncognitaID ) 
				idlist.CopyTo( grid, offset );
			else {
				//Start copying at index "1" to force a TI id in the beginning.
				if ( idlist.Length > RegionSize-1 ) throw new RegionSizeOverflowException( regionRect );  // Extra check
				idlist.CopyTo( grid, offset+1 );
			}
		}

		internal class IDComparer : IComparer {
			#region IComparer Members

			public int Compare(object x, object y) {
				if ( (ushort)x < (ushort)y ) return -1;
				if ( (ushort)x > (ushort)y ) return 1;
				return 0;
			}

			#endregion
		}


		public static Rectangle GetRegionRect(int regionx, int regiony) {
			return new Rectangle(regionx*RegionWidth, regiony*RegionHeight, RegionWidth, RegionHeight);
		}

		public static bool CheckAreaForOverflow( IDMap idmap, Rectangle area, out Rectangle errorArea ) {
			int l, r, t, b;

			l = (int)Math.Floor((double)(area.Left / RegionWidth));
			t = (int)Math.Floor((double)(area.Top / RegionWidth));
			r = (int)Math.Ceiling((double)(area.Right / RegionWidth));
			b = (int)Math.Ceiling((double)(area.Bottom / RegionWidth));
			IDGrid result = new IDGrid();
			try {
				for ( int y=t; y<b; ++y ) {
					for ( int x=l; x<r; ++x ) {
						result.BuildRegion( x, y, idmap );
					}
				}
				errorArea = Rectangle.Empty;
				return true;
			}
			catch ( RegionSizeOverflowException e ) {
				errorArea = e.Region;
				return false;
			}
		}

		public static IDGrid Build( IDMap idmap ) {
			IDGrid result = new IDGrid();
			for ( int y=0; y<result.height; ++y ) {
				for ( int x=0; x<result.width; ++x ) {
					result.BuildRegion( x, y, idmap );
				}
			}

			return result;
		}

		public static IDGrid ForcedBuild( IDMap idmap ) {
			IDGrid result = new IDGrid();
			for ( int y=0; y<result.height; ++y ) {
				for ( int x=0; x<result.width; ++x ) {
					result.BuildRegion( x, y, idmap, true );
				}
			}

			return result;
		}

		public bool IsInRegion( ushort provinceID, int regionx, int regiony ) {
			int offset = GetOffsetFromRegion( regionx, regiony );
			for ( int i=0; i<RegionSize; ++i ) {
				if ( grid[offset+i] == provinceID ) return true;
			}

			return false;
		}

		public bool IsInRegion( ushort provinceID, Point region ) {
			return IsInRegion( provinceID, region.X, region.Y );
		}

		public Point[] GetRegionList( ushort provinceID ) {
			ArrayList list = new ArrayList();
			for ( int i=0; i<grid.Length; ++i ) {
				if ( grid[i] == provinceID ) list.Add( GetRegionByOffset( i ) );
			}

			Point[] result = new Point[list.Count];
			list.CopyTo( result, 0 );
			return result;
		}

		private int GetOffsetFromRegion( int regionx, int regiony ) {
			return (regionx*height+regiony)*RegionSize;
		}

		private Point GetRegionByOffset( int offset ) {
			return new Point( (offset/RegionSize)/height, (offset/RegionSize)%height );
		}

		public ushort[] GetIDs( int regionx, int regiony ) {
			ushort[] result = new ushort[RegionSize];
			Array.Copy( grid, GetOffsetFromRegion( regionx, regiony ), result, 0, RegionSize );

			return result;
		}

		public bool[] GetMarkedProvinces( int regionx, int regiony ) {
			bool[] result = new bool[Province.InternalCount];
			ushort[] ids = GetIDs( regionx, regiony );
					
			for ( int i=0; i<RegionSize; ++i ) result[ids[i]] = true;
	
			return result;
		}
	}

}
