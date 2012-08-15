using System;
using EU2.Map;
using System.Drawing;

namespace LayerPainter
{
	/// <summary>
	/// Summary description for LayerPainterBase.
	/// </summary>
	public abstract class LayerPainterBase : ILayerPainter {
		public delegate void PropertyChangedEventHandler( object sender, EventArgs e );
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChange( EventArgs e ) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if ( handler != null ) handler( this, e );
		}

		#region ILayerPainter Members

        private bool enabled = true;
        public bool Enabled {
            get {
                return enabled;
            }
            set {
                if (value != enabled) {
                    enabled = value;
                    OnPropertyChange(EventArgs.Empty);
                }
            }
        }
        public abstract string Name { get; }
		public abstract void QuickPaint(Graphics g, ILightmapDimensions m, Rectangle area);
		public abstract void Paint(Graphics g, ILightmapDimensions m, Rectangle area);

		#endregion
    }
}
