using System;
using System.Collections;

namespace EU2 {
	/// <summary>
	/// Summary description for GlobalConfig.
	/// </summary>
	public sealed class GlobalConfigChange : IDisposable {
		public GlobalConfigChange() {
			GlobalConfig.Apply( this );
		}

		public bool AllowTOTAndHREInProvinceList {
			get {
				return allowTOTAndHRE;
			}
			set {
				allowTOTAndHRE_changed = true;
				allowTOTAndHRE = value;
			}
		}

		internal bool AllowTOTAndHREInProvinceList_Changed {
			get {
				return allowTOTAndHRE_changed;
			}
		}

		#region IDisposable Members

		public void Dispose() {
			// Signal global config to undo this change
			GlobalConfig.Undo( this );
		}

		#endregion

		private static bool allowTOTAndHRE;
		private static bool allowTOTAndHRE_changed;
	}

	public sealed class GlobalConfig {
		public static bool AllowTOTAndHREInProvinceList {
			get {
				IEnumerator iter = changes.ToArray().GetEnumerator();
				while ( iter.MoveNext() ) {
					GlobalConfigChange change = (GlobalConfigChange)iter.Current;
					if ( change.AllowTOTAndHREInProvinceList_Changed ) {
						// found
						return change.AllowTOTAndHREInProvinceList;
					}
				}

				return true;
			}
		}

		internal static void Apply( GlobalConfigChange change ) {
			changes.Push( change );
		}

		internal static void Undo( GlobalConfigChange change ) {
			if ( change != changes.Peek() ) throw new GlobalConfigIntegrityException();
			changes.Pop();
		}

		private static Stack changes = new Stack();

	}

	public class GlobalConfigIntegrityException : Exception {
		public GlobalConfigIntegrityException() : base( "GlobalConfig integrity error: The most recent applied change does not match the requested undo change" ) {
		}
	}
}
