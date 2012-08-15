using System;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace MapToolsVersion {
#if FX1_1
    public abstract class Version {
#else
    public static class Version {
#endif
        public const string BuildName = "Lapu";
        public const string BuildVersion = "2.3.0.*";

        // Version history
        // 2.0.0 - Virgenes - 16 jul 2006
        // 2.1.0 - Elcana - 19 jul 2006
        // 2.2.0 - Celebes - 5 sep 2006
        // 2.2.1 - Sulawesi - 30 oct 2006 (Sulawesi=Celebes)
        // 2.3.0 - Lapu - 30 jan 2007

        public static string GetVersionString(string desc) {
            return string.Format("{0} {1}\n{2}", 
                GetToolName(), 
                GetToolVersion(), 
                desc);
        }

        public static string GetToolName() {
            string name = Assembly.GetEntryAssembly().GetName().Name;
            return name;
        }

        public static string GetToolVersion() {
            return BuildName + " (v" + Assembly.GetEntryAssembly().GetName().Version.ToString() + ")";
        }
    }
}
