using System;
using System.Collections.Generic;
using System.Text;
using EU2.Data;

namespace MapExport {
    public class ProvinceQuery : IEnumerable<Province> {
        private ProvinceList source;
        private List<ushort> ocean_rivers;

        public ProvinceQuery(ProvinceList source) {
            this.source = source;
            this.ocean_rivers = new List<ushort>();
        }

        public ProvinceQuery(ProvinceList source, IEnumerable<ushort> ocean_rivers) {
            this.source = source;
            this.ocean_rivers = new List<ushort>(ocean_rivers);
        }

        public bool IsPti(ushort id) {
            return id == 0 || (source[id].IsRiver() && ocean_rivers.Contains(id));
        }

        public bool IsPti(Province prov) {
            return prov.ID == 0 || (prov.IsRiver() && ocean_rivers.Contains(prov.ID));
        }

        public bool IsLand(ushort id) {
            return source[id].IsLand();
        }

        public bool IsLand(Province prov) {
            return prov.IsLand();
        }

        public bool IsRiver(ushort id) {
            return source[id].IsRiver() && !ocean_rivers.Contains(id);
        }

        public bool IsRiver(Province prov) {
            return prov.IsRiver() && !ocean_rivers.Contains(prov.ID);
        }

        public bool IsOcean(ushort id) {
            return source[id].IsOcean();
            //return source[id].IsOcean() || (source[id].IsRiver() && ocean_rivers.Contains(id));
        }

        public bool IsOcean(Province prov) {
            //return prov.IsOcean() || IsOceanRiver(prov);
            //return prov.IsOcean() || (prov.IsRiver() && ocean_rivers.Contains(prov.ID));
            return prov.IsOcean();
        }

        public bool IsOceanRiver(ushort id) {
            return false;
        }

        public bool IsOceanRiver(Province prov) {
            return false;
        }

        public Province this[ushort id] {
            get { return source[id]; }
        }

        public ProvinceList Source {
            get { return source; }
        }


        #region IEnumerable<Province> Members

        public IEnumerator<Province> GetEnumerator() {
            return source.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return source.GetEnumerator();
        }

        #endregion
    }
}
