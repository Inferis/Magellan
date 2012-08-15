using System;
using System.IO;
using System.Drawing;
using System.Collections;
using EU2.Data;
using EU2.Map.Drawing;
using EU2.Map.Codec;

namespace EU2.Map
{
	public enum GetIDOptions {
		None = 0,
		OnlyInclusiveProvinces = 1,
		SkipTI = 2,
		SkipRivers = 4
	}

	/// <summary>
	/// Summary description for ID.
	/// </summary>
	public class IDMap : ScanlineCodedMap
	{
		private static int MaxDistance = (int)Math.Sqrt(int.MaxValue);

		public IDMap() : base() {
		}

		public IDMap( BinaryReader reader ) : base( reader ) {
		}

		public IDMap( string path ) : base( path ) {
		}

		public override void SetZeroID() {
			ZeroID = Province.TerraIncognitaID;
		}

		public override bool CheckValue( int value ) {
			return value >= Province.MinValue && value <= Province.MaxValue;
		}

		public bool IsOldMap() {
			int[] indexes = new int[] { 0, 344, 1513, 2101, 3011, 4390, 5104, 6119 };

			for ( int i=0; i<indexes.Length; ++i ) {
				IDZone[] zones = lines[i].GetZones();

				int previd = zones[0].ID;
				for ( int z=1; z<zones.Length; ++z ) {
					if ( previd == 0 && zones[z].ID == 0 || previd > 0 && zones[z].ID > 0 ) 
						return false; // Nope!
					previd = zones[z].ID;
				}
			}

			return true;
		}

		public ushort FindClosestLand( int x, int y, ProvinceList provinces ) {
			return FindClosestLand( x, y, this[x,y], Lightmap.BlockSize, provinces, null );
		}

		public ushort FindClosestLand( int x, int y, ProvinceList provinces, int range ) {
			return FindClosestLand( x, y, this[x,y], range, provinces, null );
		}

		public ushort FindClosestLand( int x, int y, ushort skipid, ProvinceList provinces ) {
			return FindClosestLand( x, y, skipid, Lightmap.BlockSize, provinces, null );
		}

		public ushort FindClosestLand( int x, int y, ushort skipid, ProvinceList provinces, AdjacencyTable adjacent ) {
			return FindClosestLand( x, y, skipid, Lightmap.BlockSize, provinces, adjacent);
		}

		public ushort FindClosestLand( int x, int y, ushort skipid, int range, ProvinceList provinces, AdjacencyTable adjacent ) {
			int halfwidth = (range >> 1);
			int halfheight = (range >> 1);

			ushort closest_id = skipid;

			// -- Build a bitmap around this point
			x = Lightmap.BaseNormalizeX( x );

			int max_distance = MaxDistance;
			ushort[,] map = ExportBitmapGrid( x-halfwidth, y-halfheight, range, range );
			for ( int my=0; my<range; ++my ) {
				for ( int mx=0; mx<range; ++mx ) {
					// -- Skip for TI and pixels belonging to this province
					if ( map[mx,my] == Province.TerraIncognitaID || map[mx,my] == skipid ) continue;
					if ( !provinces[map[mx,my]].IsLandNoTI() ) continue; // Only land
						
					// Also check if adjacency exists
					if ( adjacent != null && skipid != 0 ) {
						int adj = adjacent.GetAdjacencyIndex( skipid, map[mx,my] );
						if ( adj < 0 || adj >= 16 ) continue;
					}
							
					// -- This pixel qualifies: check the distance.
					// Normally, we should need a sqrt() too, but it doesn't really matter here, 
					// as we just want "the smallest" and not how small it actually is.
					int distance = ((mx-halfwidth)*(mx-halfwidth)) + ((my-halfheight)*(my-halfheight)); 
					if ( distance < max_distance ) {
						max_distance = distance;
						closest_id = map[mx,my];
					}
				}
			}

			return closest_id;
		}

		public ushort[] GetIDs( int x, int y, int width, int height, GetIDOptions options, ProvinceList provinces ) {
			// Check for valid coordinates and adjust if needed
			bool includeTI = (options & GetIDOptions.SkipTI) == 0;
			bool inclusive = (options & GetIDOptions.OnlyInclusiveProvinces) > 0;
			bool skipRivers = (options & GetIDOptions.SkipRivers) > 0 && (provinces != null);

			x = Lightmap.BaseNormalizeX( x );
			width += x;
			if ( y < 0 ) {
				height += y;
				y = 0;
			}
			height += y;
			if ( height > Lightmap.BaseHeight ) height = Lightmap.BaseHeight;

			int[] tagged = new int[Province.InternalCount];
			// Reset all items to 0
			for ( int i=0; i<Province.Count; ++i ) tagged[i] = 0;

			// Walk over the zones for each line, and tag each time we encounter a province id
			for ( ; y<height; ++y ) {
				lines[y].TagIDs( tagged, x, width, inclusive );
			}

			// Count items in list
			int count = 0;
			ushort start = (ushort )(includeTI ? 0 : 1);
			for ( int i=start; i<Province.InternalCount; ++i ) {
				if ( tagged[i] > 0 && (!skipRivers || (skipRivers && !provinces[i].IsRiver()) ) ) count++;
			}

			ushort[] list = new ushort[count];
			count = 0;
			for ( ushort i=start; i<Province.InternalCount; ++i ) {
				if ( tagged[i] > 0 && (!skipRivers || !provinces[i].IsRiver()) ) list[count++] = i;
			}

			return list;
		}

		public ushort[] GetIDs( Point location, Size size, bool inclusive ) {
			return GetIDs( location.X, location.Y, size.Width, size.Height, inclusive ? GetIDOptions.OnlyInclusiveProvinces : 0, null );
		}

		public ushort[] GetIDs( Rectangle bounds, bool inclusive ) {
			return GetIDs( bounds.X, bounds.Y, bounds.Width, bounds.Height, inclusive ? GetIDOptions.OnlyInclusiveProvinces : 0, null );
		}

		private ushort[,] GetPerimeterMap( int x, int y, int perimeter ) {
			perimeter = (perimeter/2);
			return ExportBitmapGrid( x-perimeter, y-perimeter, perimeter*2, perimeter*2 );
			//return ExportBitmapGrid( x-(perimeter/2)-1, y-(perimeter/2)-1, perimeter, perimeter );
		}

		public double CalcDistance( int x, int y, int perimeter, ushort id ) {
			return CalcDistance( GetPerimeterMap( x, y, perimeter ), id );
		}
	
		public double CalcDistance( ushort[,] map, ushort id ) {
			if ( map.GetLength(0) != map.GetLength(1) ) throw new ArgumentOutOfRangeException( "map", "The map should be a square rectangle" );
			int perimeter = (int)(map.GetLength(0)/2);

			// Walk over the map and look at each pixel
			double min_distance = MaxDistance;
			for ( int y=0; y<map.GetLength(1); y++ ) {
				for ( int x=0; x<map.GetLength(0); x++ ) {
					if ( map[x,y] == id ) {
						double distance = (x-perimeter)*(x-perimeter) + (y-perimeter)*(y-perimeter);
						if ( distance < min_distance ) min_distance = distance;
					}
				}
			}

			if ( min_distance < MaxDistance ) min_distance = Math.Sqrt(min_distance);
			return min_distance;
		}

		public void FindNearest( int x, int y, int perimeter, int count, ProvinceList provinces, out ushort[] ids, out double[] distances ) {
			int halfperimeter = (perimeter/2);
			// Get all ids around this point (in the specfied perimeter)
			ushort[] idlist = GetIDs( x-halfperimeter, y-halfperimeter, perimeter, perimeter, GetIDOptions.SkipTI | GetIDOptions.SkipRivers, provinces );
			ushort[,] area = GetPerimeterMap( x, y, perimeter );

			// Calculate distances for the found ids
			double[] distlist = new double[idlist.Length];
			for ( int i=0; i<idlist.Length; ++i ) {
				if ( idlist[i] != Province.TerraIncognitaID )
					distlist[i] = CalcDistance( area, idlist[i] );
				else
					distlist[i] = MaxDistance;
			}

			ids = new ushort[count];
			distances = new double[count];
			for ( int i=0; i<count; ++i ) {
				ids[i] = Province.TerraIncognitaID;
				distances[i] = MaxDistance;
			}

			Array.Sort( distlist, idlist );

			// Copy to result set
			Array.Copy( idlist, 0, ids, 0, idlist.Length < count ? idlist.Length : count );
			Array.Copy( distlist, 0, distances, 0, distlist.Length < count ? distlist.Length : count );
		}
		
		public void FindNearest( Point location, int perimeter, int count, ProvinceList provinces, out ushort[] ids, out double[] distances ) {
			FindNearest( location.X, location.Y, perimeter, count, provinces, out ids, out distances );
		}
		
		public void FindNearest( int x, int y, int perimeter, int count, out ushort[] ids, out double[] distances ) {
			FindNearest( x, y, perimeter, count, null, out ids, out distances );
		}

		public void FindNearest( Point location, int perimeter, int count, out ushort[] ids, out double[] distances ) {
			FindNearest( location.X, location.Y, perimeter, count, null, out ids, out distances );
		}
		
		public void FindNearest( int x, int y, int perimeter, ProvinceList provinces, out ushort id, out double distance ) {
			ushort[] ids;
			double[] distances;

			FindNearest( x, y, perimeter, 1, provinces, out ids, out distances );
			
			id = ids[0];
			distance = distances[0];
		}

		public void FindNearest( int x, int y, int perimeter, out ushort id, out double distance ) {
			FindNearest( x, y, perimeter, null, out id, out distance );
		}

		public void FindNearest( Point location, int perimeter, out ushort id, out double distance ) {
			FindNearest( location.X, location.Y,  perimeter, null, out id, out distance );
		}

		public void FindNearest( Point location, int perimeter, ProvinceList provinces, out ushort id, out double distance ) {
			FindNearest( location.X, location.Y, perimeter, provinces, out id, out distance );
		}

		public AdjacencyTable BuildAdjacencyTable( Rectangle region, ProvinceList provinces ) {
			const int Offset = 2; // 1 is too small, it seems...
			AdjacencyTable table = new AdjacencyTable();

			// Build a matrix of provinces by walking over the map and marking each adjacency in the matrix.
			int[,] matrix = new int[Province.InternalCount,Province.InternalCount];
			int[,] rivers = new int[Province.InternalCount,Province.InternalCount];

			// Fix region
			if ( region.Right > Lightmap.BaseWidth ) region.Width =  Lightmap.BaseWidth - region.X;
			if ( region.Bottom > Lightmap.BaseHeight ) region.Height =  Lightmap.BaseHeight - region.Y;

			ushort[] histogram = null;

			// Enlarge region (only if it's not encompassing the whole map).
			// This is necessary to detect changed on the border of the region specified...
			if ( region.X != 0 || region.Y != 0 || region.Width < Lightmap.BaseWidth || region.Width < Lightmap.BaseHeight ) {
				// Need to enlarge the region so that it *completely* encompasses all provinces in the original region

				// First, Get a map of all the IDs in the original region
				histogram = GetHistogram( region );

				// Get all ids in the region
				ushort[] ids = GetIDs( region, false );
				ProvinceBoundBox[] boxes = CalculateBoundBoxes();
				for ( int i=0; i<ids.Length; ++i ) {
					region = Rectangle.Union( region, boxes[ids[i]].Box );
				}

				// Fix region, again.
				if ( region.Right > Lightmap.BaseWidth ) region.Width =  Lightmap.BaseWidth - region.X;
				if ( region.Bottom > Lightmap.BaseHeight ) region.Height =  Lightmap.BaseHeight - region.Y;
			}

			int id1, id2;
			int[] places = new int[] { Offset*2, 0, 0, -Offset*2, 
									   Offset*4, -Offset*2, Offset*2, -Offset*4, 
									   Offset*6, -Offset*4, Offset*4, -Offset*6, 
									   Offset*8, -Offset*6, Offset*6, -Offset*8 };
			for ( int y=region.Top; y<region.Bottom; ++y ) {
				// Walk over zones on this row
				int zleft = lines[y].LookupZoneIndex( region.Left );
				int zright = lines[y].LookupZoneIndex( region.Right );

				for ( int z=zleft; z<=zright; ++z ) {
					// Skip if we're TI
					if ( lines[y].GetZone( z ).ID == Province.TerraIncognitaID ) continue;

					// Process this zone.
					int left = lines[y].GetZone( z ).X;
					int right = left + lines[y].GetZoneLength( z );
					for ( int x=left; x<right; ++x ) {
						#region Left/Right check
						// -- Left/right check first
						id1 = this[x-Offset,y]; 
						id2 = this[x+Offset,y];

						if ( id1 != Province.TerraIncognitaID && id2 != Province.TerraIncognitaID ) {
							if ( provinces[id2].IsRiver() ) {
								// The right one is a river... 
								++matrix[id1,id2];
								++matrix[id2,id1];

								// Look for land...
								for ( int i=0; i<places.Length; i+=2 ) {
									id2 = this[x+places[i],y+places[i+1]];
									if ( provinces[id2].IsLand() ) {
										++matrix[id1,id2];
										++matrix[id2,id1];
										// Also mark river adjacency
										++rivers[id1,id2];
										++rivers[id2,id1];
										break;
									}
								}

								// If not found, don't mark a thing
							}
							else {
								++matrix[id1,id2];
								++matrix[id2,id1];
							}
						}
						#endregion

						#region Up/down check
						// -- Up/down check next
						id1 = this[x,y-Offset]; 
						id2 = this[x,y+Offset];

						if ( id1 != 0 && id2 != 0 ) {
							if ( provinces[id2].IsRiver() ) {
								// The bottom one is a river... 
								++matrix[id1,id2];
								++matrix[id2,id1];

								// Look further down for land...
								for ( int i=0; i<places.Length; i+=2 ) {
									id2 = this[x+(-places[i+1]),y+places[i]];
									if ( provinces[id2].IsLand() ) {
										++matrix[id1,id2];
										++matrix[id2,id1];
										// Also mark river adjacency
										++rivers[id1,id2];
										++rivers[id2,id1];
										break;
									}
								}

								// If not found, don't mark a thing
							}
							else {
								++matrix[id1,id2];
								++matrix[id2,id1];
							}
						}
						#endregion
					}
				}
			}

			// Walk over the matrix, and build an actual list of adjacencies for each province.
			const int Threshold = 8; // we need a certain amount of matches to have adjacency...

			// Start iterating at 1, no need to incorporate TI...
			for ( ushort ph=1; ph<Province.Count; ++ph ) {
				// Skip ids not present in the original region
				if ( histogram != null && histogram[ph] <= 0 ) continue;

				// Do "normal" adjacencies first
				for ( ushort pv=1; pv<Province.Count; ++pv ) { 
					if ( ph == pv || provinces[pv].IsRiver() ) continue;
					// Check for the threshold
					if ( matrix[ph,pv] <= Threshold ) continue;

					table.Add( ph, new Adjacent( pv, rivers[ph,pv] == matrix[ph,pv] ? AdjacencyType.River : AdjacencyType.Normal ) );
				}

				// Do "river" adjacencies next
				for ( ushort pv=1; pv<Province.Count; ++pv ) { 
					if ( ph == pv || !provinces[pv].IsRiver() ) continue;
					// Check for the threshold
					if ( matrix[ph,pv] <= Threshold ) continue;

					table.Add( ph, new Adjacent( pv, AdjacencyType.Normal ) );
				}
			}

			return table;
		}
			
		public AdjacencyTable BuildAdjacencyTable( ProvinceList provinces ) {
			return BuildAdjacencyTable( new Rectangle( 0, 0, Lightmap.BaseWidth, Lightmap.BaseHeight ), provinces );
		}

		public ushort[] BuildAdjacencyTable( AdjacencyTable mergeWith, Rectangle region, ProvinceList provinces ) {
			return mergeWith.Merge( BuildAdjacencyTable( region, provinces ) );
		}

		public ushort[] GetHistogram( Rectangle area ) {
			ushort[] bitmap = ExportBitmapBuffer( area );
			ushort[] result = new ushort[Province.InternalCount];

			// Clear result
			for ( int i=0; i<result.Length; ++i ) result[i] = 0;

			// Count each pixel
			for ( int i=0; i<bitmap.Length; ++i ) result[bitmap[i]]++;

			return result;
		}

		public ProvinceBoundBox[] CalculateBoundBoxes() {
			ProvinceBoundBox[] boxes = new ProvinceBoundBox[Province.InternalCount];

			// Initialize array
			for ( ushort i=0; i<Province.InternalCount; ++i ) {
				boxes[i] = new ProvinceBoundBox( Lightmap.BaseWidth, Lightmap.BaseHeight, 0, 0, i );
			}

			// Walk over zones and shrink each box as needed
			IDZone firstZone, secondZone, lastZone;
			for ( int y=0; y<Lightmap.BaseHeight; ++y ) {
				Scanline line = lines[y];

				int bufcount = line.Count-2; // Don't need to include the stopper, and the last zone (see later)
				firstZone = line.GetZone( 0 );
				secondZone = line.GetZone( 1 );
				lastZone = line.GetZone( line.Count-2 );

				for ( int bufindex=0; bufindex<bufcount; ++bufindex ) {
					ushort id = line.GetZone( bufindex ).ID;

					if ( id == Province.TerraIncognitaID || id > Province.Count ) continue;

					// Check Y values first (this is simple)
					if ( boxes[id].Top > y ) boxes[id].Top = y;
					if ( boxes[id].Bottom < y ) boxes[id].Bottom = y;

					// Now for the X values
					// We need to take special care for the zones at the dateline: the box is calculated from 2 zones...
					if ( bufindex > 0 ) {
						int left = line.GetZone( bufindex ).X; 
						int right = line.GetZone( bufindex+1 ).X; 

						// Full zone (this is simple)
						if ( boxes[id].Right > Lightmap.BaseWidth ) {
							// Don't have to check left, as it will never be able to match
							if ( Lightmap.BaseNormalizeX( boxes[id].Right ) < right ) boxes[id].Right = right + Lightmap.BaseWidth;
						}
						else {
							if ( boxes[id].Left > left ) boxes[id].Left = left;
							if ( boxes[id].Right < right ) boxes[id].Right = right;
						}
					}
					else {
						// First zone, is it a split zone?
						if ( firstZone.ID == lastZone.ID ) {
							// Wrapping province, treat as 1
							if ( boxes[firstZone.ID].Left > lastZone.X ) boxes[firstZone.ID].Left = lastZone.X;
							if ( boxes[firstZone.ID].Right < secondZone.X+Lightmap.BaseWidth ) boxes[firstZone.ID].Right = secondZone.X+Lightmap.BaseWidth;
						}
						else {
							// 2 seperate provinces with edge on dateline
							boxes[firstZone.ID].Left = 0;
							boxes[firstZone.ID].Right = secondZone.X;

							boxes[lastZone.ID].Left = lastZone.X;
							boxes[lastZone.ID].Right = Lightmap.BaseWidth;
						}
					}
				}
			}

			return boxes;
		}

		private const int BuildBlockSize = Lightmap.BlockSize*64; // 2048

		public static IDMap Build( Lightmap map ) {
			// Should be a zero zoom lightmap
			if ( map.Zoom != 0 ) throw new InvalidZoomFactorException( map.Zoom );

			// We build the map in 2048/2048 blocks
			IDMap result = new IDMap();
			IShader32 shader = new ImportIDShader();
			bool vd = map.VolatileDecompression;
			try {
				map.VolatileDecompression = true;
				for ( int y=0; y<Lightmap.BaseHeight; y+=BuildBlockSize ) {
					int height = (y+BuildBlockSize > Lightmap.BaseHeight) ? (Lightmap.BaseHeight-y) : BuildBlockSize;

					for ( int x=0; x<Lightmap.BaseWidth; x+=BuildBlockSize ) {
						int width = (x+BuildBlockSize > Lightmap.BaseWidth) ? (Lightmap.BaseWidth-x) : BuildBlockSize;

						// Get the bitmap from the lightmap
						// And import it into ourselves
						result.ImportBitmapBuffer( x, y, width, height, map.DecodeImage( new Rectangle( x, y, width, height ), shader ) );
					}
				}
			} 
			finally {
				map.VolatileDecompression = vd;
			}

			return result;
		}

		internal class ImportIDShader : IShader32 {

			public int[] Shade32( RawImage image ) {
				int[] buffer = new int[image.PixelCount];
				int bufidx = 0;
				Pixel[,] memory = image.Memory;
				int height = image.Size.Height;
				int width = image.Size.Width;

				for ( int y=0; y<height; ++y ) {
					for ( int x=0; x<width; ++x ) {
						int id = memory[x,y].ID;
						buffer[bufidx++] = id == Province.TerraIncognitaID ? Province.MaxValue : id;
					}
				}

				return buffer;
			}

		}

	}
}
