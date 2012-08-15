using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using LayerPainter;
using EU2.Data;
using EU2.Edit;

namespace MapView {
    class ProvinceLayerPainter : LayerPainterBase {
        private int highlight = -1;
        private File source = null;
        private Image cityBitmap;

        public ProvinceLayerPainter() {
            using( System.IO.Stream stream = this.GetType().Assembly.GetManifestResourceStream( "MapView.images.city.png" )){
                cityBitmap = Bitmap.FromStream(stream);
            }
        }

        public override string Name {
            get { return "Province"; }
        }

        public File Source {
            get {
                return source;
            }
            set {
                source = value;
            }
        }

        public int HighlightId {
            get {
                return highlight;
            }
            set {
                highlight = value;
            }
        }

        public override void QuickPaint(System.Drawing.Graphics g, EU2.Map.ILightmapDimensions m, System.Drawing.Rectangle area) {
            DoPaint(g, m, area, true);
        }

        public override void Paint(System.Drawing.Graphics g, EU2.Map.ILightmapDimensions m, System.Drawing.Rectangle area) {
            DoPaint(g, m, area, false);
        }

        private void DoPaint(System.Drawing.Graphics g, EU2.Map.ILightmapDimensions m, System.Drawing.Rectangle area, bool fast) {
            if (source == null) return;
            //if (highlight <= 0) return;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            Rectangle actualarea = m.CoordMap.BlocksToActual(area);
            foreach (EU2.Map.ProvinceBoundBox currentBox in source.BoundBoxes.GetAllIntersectingWith(actualarea)) {
                int current = currentBox.ProvinceID;
                //if (!area.IntersectsWith(source.BoundBoxes[current].Box)) continue;

                Province prov = source.Provinces[current];
                if (!prov.IsLand()) return;

                string name = prov.DefaultCityName;
                if (string.IsNullOrEmpty(name)) name = prov.Name;
                using (Font f = new Font("Georgia", 10, FontStyle.Bold)) {
                    foreach (Point pt in new Point[] { prov.CityPosition }) {
                        pt.Offset(-actualarea.X, -actualarea.Y);
                        Point drawPt = pt;
                        drawPt.Offset(-cityBitmap.Width / 2, -cityBitmap.Height / 2);
                        g.DrawImage(cityBitmap, drawPt);

                        if (!string.IsNullOrEmpty(name)) {
                            drawPt = pt;
                            drawPt.Offset((int)(-g.MeasureString(prov.Name, f).Width / 2), cityBitmap.Height / 2 + 3);
                            using (Brush b = new SolidBrush(Color.FromArgb(200, Color.Black))) {
                                g.DrawString(name, f, b, drawPt);
                            }
                            drawPt.Offset(-1, -1);
                            g.DrawString(name, f, Brushes.White, drawPt);
                        }
                    }
                }
            }
        }
    }
}
