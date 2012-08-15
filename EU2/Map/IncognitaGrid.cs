using System;
using System.IO;
using EU2.Data;
using EU2.IO;

namespace EU2.Map
{
	/// <summary>
	/// Summary description for Incognita.
	/// </summary>
	public class IncognitaGrid : IStreamWriteable {
		private IncognitaGridItem[,] grid;

		public IncognitaGrid() {
			grid = new IncognitaGridItem[Lightmap.BaseWidth >> Lightmap.BlockFactor,(Lightmap.BaseHeight >> Lightmap.BlockFactor)+1];

			for ( int y=0; y<grid.GetLength(1); y++ ) {
				for ( int x=0; x<grid.GetLength(0); x++ ) {
					grid[x,y] = new IncognitaGridItem( true );
				}
			}
		}

		public IncognitaGrid( BinaryReader reader ) {
			grid = new IncognitaGridItem[Lightmap.BaseWidth >> Lightmap.BlockFactor,(Lightmap.BaseHeight >> Lightmap.BlockFactor)+1];
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			for ( int y=0; y<grid.GetLength(1); y++ ) {
				for ( int x=0; x<grid.GetLength(0); x++ ) {
					grid[x,y] = new IncognitaGridItem( reader );
				}
			}
		}

		public void WriteTo( BinaryWriter writer ) {
			for ( int y=0; y<grid.GetLength(1); y++ ) {
				for ( int x=0; x<grid.GetLength(0); x++ ) {
					grid[x,y].WriteTo( writer );
				}
			}
		}

		public void WriteTo( Stream stream ) {
			WriteTo( new BinaryWriter( stream ) );
		}

		public IncognitaGridItem this[int x, int y] {
			get {
				return grid[x,y];
			}
			set {
				grid[x,y] = value;
			}
		}

		public static IncognitaGrid Build( IDMap idmap, ProvinceList provinces, IDGrid idgrid ) {
			IncognitaGrid result = new IncognitaGrid();

			double scansize = (double)(Lightmap.BlockSize << 1);
			double scanfactor = 256.0 / Math.Sqrt( (scansize/2.0) * (scansize/2.0) );

			Random rnd = new Random();
			int halfscan = (int)(scansize/2);
			for( int y=0; y<Lightmap.BaseHeight; y+=Lightmap.BlockSize ) {
				for( int x=0; x<Lightmap.BaseWidth; x+=Lightmap.BlockSize ) {
					ushort[] list = idmap.GetIDs( x-halfscan, y-halfscan, (int)scansize, (int)scansize, GetIDOptions.SkipTI | GetIDOptions.SkipRivers, provinces );

					if ( list.Length > 1 ) {
						double[] distances;
						ushort[] idlist;
						int weight;

						idmap.FindNearest( x, y, (int)scansize, 4, provinces, out idlist, out distances );
						for ( int i=0; i<4; ++i ) {
							if ( idlist[i] > Province.MaxID ) idlist[i] = Province.TerraIncognitaID;
							weight = (int)(distances[i]*scanfactor);
							weight = weight - 15 + (rnd.Next(0x7fff) & 31);
							if ( weight < 0 ) weight = 0;
							else if ( weight > 255 ) weight = 255;

							result.grid[x >> Lightmap.BlockFactor,y >> Lightmap.BlockFactor].data[i].id = idgrid.MapID( x, y, idlist[i] );
							result.grid[x >> Lightmap.BlockFactor,y >> Lightmap.BlockFactor].data[i].weight = (byte)weight;
						}
					}
					else {
						ushort id =  Province.TerraIncognitaID;
						if ( list.Length > 0 ) {
							id = list[0];
							if ( id > Province.MaxID ) id = Province.TerraIncognitaID;
						}

						result.grid[x >> Lightmap.BlockFactor, y >> Lightmap.BlockFactor].data[0].id = idgrid.MapID( x, y, id );
						result.grid[x >> Lightmap.BlockFactor, y >> Lightmap.BlockFactor].data[0].weight = 0;

						result.grid[x >> Lightmap.BlockFactor, y >> Lightmap.BlockFactor].data[1].id = 0;
						result.grid[x >> Lightmap.BlockFactor, y >> Lightmap.BlockFactor].data[1].weight = 255;

						result.grid[x >> Lightmap.BlockFactor, y >> Lightmap.BlockFactor].data[2].id = 0;
						result.grid[x >> Lightmap.BlockFactor, y >> Lightmap.BlockFactor].data[2].weight = 255;
					
						result.grid[x >> Lightmap.BlockFactor, y >> Lightmap.BlockFactor].data[3].id = 0;
						result.grid[x >> Lightmap.BlockFactor, y >> Lightmap.BlockFactor].data[3].weight = 255;
					}
				}
			}

			return result;
		}

		public static IncognitaGrid Build( IDMap idmap, ProvinceList provinces ) {
			return Build( idmap, provinces, IDGrid.Build( idmap ) );
		}
	}

	public struct IncognitaGridItem {
		internal IDWeight[] data;

		public IncognitaGridItem( bool dummy ) {
			this.data = new IDWeight[4] { new IDWeight( 0, 255 ), new IDWeight( 0, 255 ), new IDWeight( 0, 255 ), new IDWeight( 0, 255 ) };
		}

		public IncognitaGridItem( IDWeight[] data ) {
			this.data = new IDWeight[4];
			data.CopyTo( this.data, 0 );
		}

		public IncognitaGridItem( BinaryReader reader ) {
			this.data = new IDWeight[4];
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			byte[] buf = new byte[8];
			reader.Read( buf, 0, 8 );

			for ( int i=0; i<data.Length; ++i ) {
				data[i].ID = buf[i];
				data[i].Weight = buf[4+i];
			}
		}

		public void WriteTo( BinaryWriter writer ) {
			byte[] buf = new byte[8];

			for ( int i=0; i<data.Length; ++i ) {
				buf[i] = data[i].ID;
				buf[4+i] = data[i].Weight;
			}
			writer.Write( buf, 0, 8 );
		}

		public IDWeight this[int index] {
			get { return data[index]; }
			set { data[index] = value; }
		}

	}

	public struct IDWeight {
		internal byte id;
		internal byte weight;

		internal IDWeight( byte id, byte weight ) {
			this.id = id;
			this.weight = weight;
		}

		public byte ID {
			get { return id; }
			set { id = value; }
		}

		public byte Weight {
			get { return weight; }
			set { weight = value; }
		}
	}

}
