using System;
using EU2.Data;

namespace EU2.Map.Codec.MapBlockHandling
{
	/// <summary>
	/// Summary description for Compressor.
	/// </summary>
	public class DefaultCompressor : Compressor {
		public DefaultCompressor( int rx, int ry, int zoom, ProvinceList provinces, AdjacencyTable adjacent, IDMap idmap ) {
			this.rx = rx;
			this.ry = ry;
			this.zoom = zoom;
			this.provinces = provinces;
			this.adjacent = adjacent;
			this.idmap = idmap;

		}

		protected override void OnVisitNode( Node node, int x, int y ) {
			tree[treeindex++] = node.IsBranch();

			if ( node.IsBranch() && node.Level > 1 ) return;

			if ( node.IsBranch() ) { // 4 leaves on pixel level
				HandleNode( node.BottomRightChild, x+1, y+1 );
				HandleNode( node.BottomLeftChild, x, y+1 );
				HandleNode( node.TopRightChild, x+1, y );
				HandleNode( node.TopLeftChild, x, y );
			}
			else { // Leaf above pixel level
				owners[leafindex] = ConvertID( node.Data.ID );
				colors[leafindex++] = node.Data.Color;
			}
		}

		private void HandleNode( Node node, int x, int y ) {
			ushort neighbourid = 0;	
			ushort provid = node.Data.ID;

            try {
                if (!provinces.Contains(provid))
                    throw new InvalidProvinceIDException(provid);
                if (node.Data.Border > 0 && provinces != null && (provinces[provid].IsLand() || provinces[provid].IsRiver())) {
                    byte color = (byte)(node.Data.Border - 1);
                    int adjx = rx + (x << zoom);
                    int adjy = ry + (y << zoom);

                    ushort riverid = Province.MaxValue;
                    if (provinces[provid].IsRiver()) { // Is River, convert province id to nearest land
                        riverid = provid;
                        provid = idmap.FindClosestLand(adjx, adjy, riverid, provinces);
                    }
                    neighbourid = idmap.FindClosestLand(adjx, adjy, provid, provinces, adjacent); // Get nearest neighbour

                    if (provid != neighbourid && provid != riverid && provinces[neighbourid].IsLand()) {
                        // We have 3 things now: 2 provinces and a possible river.
                        int neighadj = adjacent.GetAdjacencyIndex(provid, neighbourid);
                        if (neighadj >= 0 || neighadj < 16) {
                            int riveradj = Adjacent.Invalid;
                            if (riverid < Province.Count) { // Convert river to adjacency
                                riveradj = adjacent.GetAdjacencyIndex(provid, riverid);
                            }

                            if (riveradj >= 0) {
                                if (riveradj >= 16)
                                    throw new AdjacencyIndexOutOfRangeException(provid, riverid, riveradj);

                                int providx = ConvertID(provid); // Convert the province id to an index in the id table
                                if (providx < 0 || providx + 4 >= 64)
                                    throw new ProvinceIndexOutOfRangeException(provid, providx);

                                // Store the river and the two surrounding provinces (river and land)
                                owners[leafindex] = ConvertID((ushort)(color + (neighadj << 1) + (riveradj << 5) + ((providx + 4) << 9)));
                            }
                            else {
                                owners[leafindex] = ConvertID(riverid); // Store only the river
                            }
                        }
                        else {
                            owners[leafindex] = ConvertID(provid); // Store the province
                        }
                    }
                    else {
                        owners[leafindex] = ConvertID(provid); // Store the province
                    }
                }
                else {
                    owners[leafindex] = ConvertID(provid); // Store the province
                }
                colors[leafindex++] = node.Data.Color;
            }
            catch (Exception ex) {
                throw new NodeCompressionException(node, x+rx, y+ry, ex);
            }
		}

		#region Private Fields
		private int zoom;
		private ProvinceList provinces;
		private AdjacencyTable adjacent;
		private IDMap idmap;
		private int rx; 
		private int ry;
		#endregion
	}

    public class NodeCompressionException : Exception {
        Node node;
        int x, y;

        public NodeCompressionException() {
            node = null;
            x = 0;
            y = 0;
        }

        public NodeCompressionException(Node node, int x, int y, Exception innerException)
            : base(string.Format("Node compression failed at ({0},{1}): {2}", x, y, innerException.Message), innerException) {
            this.node = node;
            this.x = x;
            this.y = y;
        }

        public NodeCompressionException(string message)
            : base(message) {
            node = null;
            x = 0;
            y = 0;
        }

        public int X {
            get { return x; }
        }

        public int Y {
            get { return y; }
        }
    }

    public class ProvinceIndexOutOfRangeException : Exception {
        private int index;
        private int provinceId;

        public ProvinceIndexOutOfRangeException()
            : base("Converting a province id to a block based index failed: the index is out of range.") {
        }

        public ProvinceIndexOutOfRangeException(int provinceId, int index)
            : base(string.Format("Converting a province id (={0}) to a block based index (={1}) failed: the index is out of range.", provinceId, index)) {
            this.index = index;
            this.provinceId = provinceId;
        }

        public ProvinceIndexOutOfRangeException(string message)
            : base(message) {
        }

        public int Index {
            get { return index; }
        }

        public int ProvinceId {
            get { return provinceId; }
        }
    }

    public class AdjacencyIndexOutOfRangeException : Exception {
        private int riveradj;
        private int provinceId;
        private int riverId;

        public AdjacencyIndexOutOfRangeException()
            : base("River adjacency index between province and river produced is out of range") {
        }

        public AdjacencyIndexOutOfRangeException(int provid, int riverid, int riveradj)
            : base(string.Format("River adjacency index (={2}) lookup between province (={0}) and river (={1}) is out of range.", provid, riverid, riveradj)) {
            this.provinceId = provid;
            this.riverId = riverid;
            this.riveradj = riveradj;
        }

        public AdjacencyIndexOutOfRangeException(string message)
            : base(message) {
        }

        public int ProvinceId {
            get { return provinceId; }
        }

        public int RiverId {
            get { return riverId; }
        }

        public int RiverAdj {
            get { return riveradj; }
        }

    }

}
