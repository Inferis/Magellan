using System;

namespace PSD
{
	/// <summary>
	/// Summary description for HSL.
	/// </summary>
	public class HSL
	{
		public HSL() {
			h = 0;
			s = 0;
			l = 0;
		}

		public HSL( double h, double s, double l ) {
			this.h = h;
			this.s = s;
			this.l = l;
		}

		public double H {
			get {
				return h;
			}
			set {
				h = value > 1 ? 1 : (value < 0 ? 0 : value);
			}
		}

		public double S {
			get {
				return s;
			}
			set {
				s = value > 1 ? 1 : (value < 0 ? 0 : value);
			}
		}

		public double L {
			get {
				return l;
			}
			set {
				l = value > 1 ? 1 : (value < 0 ? 0 : value);
			}
		}

		private double h, s, l;
	}
}
