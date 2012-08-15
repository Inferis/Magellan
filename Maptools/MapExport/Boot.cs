using System;
using System.IO;
using System.Reflection;
using EU2.Map;
using EU2.Edit;
using MapToolsLib;
using System.Drawing;
using EU2.Map.Codec;
using System.Collections.Generic;
using EU2.Data;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using EU2.Enums;
using EU2.IO;
using System.Resources;

namespace MapExport
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Boot
	{
		private const string ToolDescription = "EU2 Map Export Tool";
        private static Rectangle MapRestrict {
            get { return new Rectangle(0, 0, 30000, 20000); }
        }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
            Console.WriteLine(MapToolsVersion.Version.GetVersionString(ToolDescription));
			Console.WriteLine( );

#if !DEBUG
			try {
#endif
				MapExportParsedArguments pargs = new MapExportParsedArguments( args );

				if ( pargs.Help ) {
					ShowHelp();
					return;
				}

				if ( pargs.Source == "" ) {
					Console.WriteLine( "No source file specified!" );
					return;
				}

				string source = pargs.Source;
				if ( Path.GetExtension( source ) == "" ) source = Path.ChangeExtension( source, "eu2map" );
				source = Path.GetFullPath( source ); 

				// Check if source exists
				if ( !System.IO.File.Exists( source ) ) {
					Console.WriteLine( "The specified source file \"{0}\" could not be found.", Path.GetFileName( source ) );
					return;
				}

				string dir;
                if (pargs.DirectoryOverride.Length > 0) {
                    dir = Path.GetFullPath(pargs.DirectoryOverride);
                    if (!Directory.Exists(dir)) {
                        Console.WriteLine("The specified directory override \"{0}\" does not exist.", dir);
                        return;
                    }
                }
                else
                    dir = Directory.GetCurrentDirectory();

				Console.WriteLine( "Opening source file \"{0}\"...", Path.GetFileName( source ) );

				EU2.Edit.File file = new EU2.Edit.File();
				file.ReadFrom( source );

                string pretarget = Path.Combine(dir, Path.GetFileNameWithoutExtension(source));
                string target = pretarget;
                int i = 1;
                while (Directory.Exists(target)) {
                    target = string.Format("{0} ({1})", pretarget, i++);
                }
                Directory.CreateDirectory(target);

                Console.WriteLine("Writing files to \"{0}\"...", target);
                if (!DoExport(file, target))
                    return;

                Console.Write( "Export successful." );
#if !DEBUG
			}
			catch( Exception e ) {
				MapToolsLib.Utils.LogError( e );
			}
#endif

			Console.WriteLine( );
		}

        private static bool DoExport(EU2.Edit.File file, string target) {
            // Generate a mapping of EU2 ids to EU3 ids first
            Dictionary<ushort, ushort> provmap, reverseprovmap;
            ushort seaStartsAt;

            List<ushort> allowedProvinces = CheckAllowedProvinces(file);


            ProvinceQuery provquery = new ProvinceQuery(file.Provinces, new ushort[] { 1326, 1327, 1361, 1442, 1458, 1453, 1454, 1459, 1460, 1461, 1585, 1593 });
            GenerateIDMapping(allowedProvinces, provquery, out provmap, out reverseprovmap, out seaStartsAt);

            using (StreamWriter writer = new StreamWriter(Path.Combine(target, "default.map"))) {
                writer.WriteLine("max_provinces = {0}", provmap.Count);
                writer.WriteLine("sea_starts = {0}", seaStartsAt);
                writer.WriteLine("definitions = \"definition.csv\"");
                writer.WriteLine("provinces = \"provinces.bmp\"");
                writer.WriteLine("positions = \"positions.txt\"");
                writer.WriteLine("terrain = \"terrain.bmp\"");
                writer.WriteLine("rivers = \"rivers.bmp\"");
                writer.WriteLine("terrain_definition = \"terrain.txt\"");
                writer.WriteLine("terrain_topology = \"topology.bmp\"");
                writer.WriteLine("tree_definition = \"trees.txt\"");
                writer.WriteLine("continent = \"continent.txt\"");
                writer.WriteLine("adjacencies = \"adjacencies.csv\"");
                writer.WriteLine("climate = \"climate.txt\"");
            }

            if (!ExportDefinition(file, target, reverseprovmap, new InferisEU3CompatIDConvertor(seaStartsAt)))
                return false;

            if (!ExportTerrain(target)) 
                return false;

            if (!ExportContinents(file, target, provmap))
                return false;

            if (!ExportClimate(file, target, provmap))
                return false;

            if (!ExportProvincesAndPositions(file, target, provmap))
                return false;

            DumpResource("terrain.txt", Path.Combine(target, "terrain.txt"));
            DumpResource("trees.txt", Path.Combine(target, "trees.txt"));

            if (!ExportBitmaps(file, target, provquery, provmap, reverseprovmap, seaStartsAt))
                return false;

            return true;
        }

        private static List<ushort> CheckAllowedProvinces(EU2.Edit.File file) {
            List<ushort> result = new List<ushort>();

            Console.WriteLine("Generating boundboxes...");
            file.BoundBoxes = new BoundBoxes(file.IDMap.CalculateBoundBoxes());

            foreach(ProvinceBoundBox box in file.BoundBoxes.GetAllIntersectingWith(MapRestrict)) {
                if ((box.Box.Width > 0 && box.Box.Height > 0) || box.ProvinceID == Province.TerraIncognitaID)
                    result.Add(box.ProvinceID);
            }

            return result;
        }

        private static void DumpResource(string id, string targetpath) {
            using (Stream stream = typeof(Boot).Assembly.GetManifestResourceStream(string.Format("MapExport.{0}", id))) {
                using (StreamReader rdr = new StreamReader(stream)) {
                    using (StreamWriter writer = new StreamWriter(targetpath)) {
                        writer.Write(rdr.ReadToEnd());
                    }
                }
            }
        }

        private static bool ExportProvincesAndPositions(EU2.Edit.File file, string target, Dictionary<ushort, ushort> provmap) {
            Console.WriteLine("Exporting provinces (province & position data)...");
            
            string path = Path.Combine(target, "provinces");
            Directory.CreateDirectory(path);
            using (StreamWriter posWriter = new StreamWriter(Path.Combine(target, "positions.txt"))) {
                foreach (Province prov in file.Provinces) {
                    if (!provmap.ContainsKey(prov.ID))
                        continue;

                    if (!prov.IsLand() || prov.ID == Province.TerraIncognitaID) 
                        continue;

                    using (StreamWriter writer = new StreamWriter(Path.Combine(path, string.Format("{0} - {1}.txt", provmap[prov.ID], prov.Name)))) {
                        writer.WriteLine("#{0} (was {1}, is {2})", prov.Name, prov.ID, provmap[prov.ID]);
                        writer.WriteLine();
                    }

                    posWriter.WriteLine("#{0} (was {1}, is {2})", prov.Name, prov.ID, provmap[prov.ID]);
                    posWriter.WriteLine("{0} = {{", provmap[prov.ID]);
                    posWriter.WriteLine("}");
                    posWriter.WriteLine();
                }
            }

            return true;
        }

        private static bool ExportTerrain(string target) {
            Console.WriteLine("Exporting terrains...");

            using (StreamWriter writer = new StreamWriter(Path.Combine(target, "terrain.txt"))) {
                writer.WriteLine("### Generated by mexport ({0})", DateTime.Now);

                foreach (Terrain terr in Terrain.All) {
                    writer.WriteLine("{0} = {{", terr.Name.ToLower().Replace(" ", "_"));
                    writer.WriteLine("\tcolor = {{ {0} }}", terr.ID);
                    writer.WriteLine("\tmovement_cost = {0}", 1);
                    if (terr == Terrain.Ocean) {
                        writer.WriteLine("\tis_water = yes");
                        writer.WriteLine("\thas_texture = no");
                    }
                    if (terr == Terrain.TerraIncognita) {
                        writer.WriteLine("\thas_texture = no");
                    }
                    writer.WriteLine("}");
                    writer.WriteLine();
                }
            }
            
            return true;
        }

        private static bool ExportContinents(EU2.Edit.File file, string target, Dictionary<ushort, ushort> provmap) {
            Console.WriteLine("Exporting continents...");

            Dictionary<string,List<ushort>> continents = new Dictionary<string,List<ushort>>();
            foreach (Province prov in file.Provinces) {
                if (!provmap.ContainsKey(prov.ID))
                    continue;
                string cont = prov.Continent.Trim().ToLower();
                if (string.IsNullOrEmpty(cont) || cont == "-")
                    continue;

                if (continents.ContainsKey(cont))
                    continents[cont].Add(provmap[prov.ID]);
                else
                    continents[cont] = new List<ushort>(new ushort[] { provmap[prov.ID] });
            }

            using (StreamWriter writer = new StreamWriter(Path.Combine(target, "continent.txt"))) {
                writer.WriteLine("### Generated by mexport ({0})", DateTime.Now);
                
                foreach (string cont in continents.Keys) {
                    writer.WriteLine("{0} = {{", cont);
                    writer.Write("\t");
                    continents[cont].Sort();
                    foreach (ushort id in continents[cont]) {
                        writer.Write(id);
                        writer.Write(" ");
                    }
                    writer.WriteLine();
                    writer.WriteLine("}");
                }
            }

            return true;
        }

        private static bool ExportClimate(EU2.Edit.File file, string target, Dictionary<ushort, ushort> provmap) {
            Console.WriteLine("Exporting climate...");

            Dictionary<string, List<ushort>> climates = new Dictionary<string, List<ushort>>();
            foreach (Province prov in file.Provinces) {
                if (!provmap.ContainsKey(prov.ID))
                    continue;

                string clim = "";
                switch (prov.Climate) {
                    case 0:
                        clim = "severe_winter";
                        break;
                    case 7:
                        clim = "severe_winter";
                        break;
                    case 5:
                        clim = "normal_winter";
                        break;
                    case 2:
                        clim = "mild_winter";
                        break;
                    case 1:
                        clim = "tropical";
                        break;
                    case 3:
                        clim = "tropical";
                        break;
                    case 8:
                        clim = "normal_winter";
                        break;
                    case 4: case 6:
                        clim = "tropical";
                        break;
                    default:
                        continue;
                }

                if (climates.ContainsKey(clim))
                    climates[clim].Add(provmap[prov.ID]);
                else
                    climates[clim] = new List<ushort>(new ushort[] { provmap[prov.ID] });
            }

            using (StreamWriter writer = new StreamWriter(Path.Combine(target, "climate.txt"))) {
                writer.WriteLine("### Generated by mexport ({0})", DateTime.Now); 
                foreach (string clim in climates.Keys) {
                    writer.WriteLine("{0} = {{", clim);
                    writer.Write("\t");
                    climates[clim].Sort();
                    foreach (ushort id in climates[clim]) {
                        writer.Write(id);
                        writer.Write(" ");
                    }
                    writer.WriteLine();
                    writer.WriteLine("}");
                }
            }

            return true;
        }

        private static bool ExportDefinition(EU2.Edit.File file, string target, Dictionary<ushort, ushort> reverseprovmap, IIDConvertor convertor) {
            Console.WriteLine("Exporting province color definitions...");

            using (CSVWriter writer = new CSVWriter(Path.Combine(target, "definition.csv"))) {
                writer.EndRow("province;red;green;blue;");
                foreach (ushort id in reverseprovmap.Keys) {
                    if (id <= 0)
                        continue;
                    writer.Write(id);
                    int rgb = convertor.ConvertID(id);
                    writer.Write((rgb >> 16) & 0xFF);
                    writer.Write((rgb >> 8) & 0xFF);
                    writer.Write(rgb & 0xFF);
                    writer.Write(file.Provinces[reverseprovmap[id]].Name);
                    writer.EndRow();
                }
            }

            return true;
        }

        private static void GenerateIDMapping(IList<ushort> allowed, ProvinceQuery provinces, out Dictionary<ushort, ushort> map, out Dictionary<ushort, ushort> reversemap, out ushort seaStartsAt) {
            List<ushort> landprovs = new List<ushort>();
            List<ushort> seaprovs = new List<ushort>();

            foreach (Province prov in provinces) {
                if (!allowed.Contains(prov.ID))
                    continue;

                if (provinces.IsLand(prov))
                    landprovs.Add(prov.ID);
                else if (provinces.IsOcean(prov))
                    seaprovs.Add(prov.ID);            
            }

            map = new Dictionary<ushort, ushort>();
            reversemap = new Dictionary<ushort, ushort>();
            for (int i = 0; i < landprovs.Count; ++i) {
                map.Add(landprovs[i], (ushort)(i+1));
                reversemap.Add((ushort)(i + 1), landprovs[i]);
            }
            seaStartsAt = (ushort)landprovs.Count;
            for (int i = 0; i < seaprovs.Count; ++i) {
                map.Add(seaprovs[i], (ushort)(seaStartsAt + i + 1));
                reversemap.Add((ushort)(seaStartsAt + i + 1), seaprovs[i]);
            }

            map.Add(0, 0);
            reversemap.Add(0, 0);
        }

        private static bool ExportBitmaps(EU2.Edit.File file, string target, ProvinceQuery provquery, Dictionary<ushort, ushort> provmap, Dictionary<ushort, ushort> reverseprovmap, ushort seaStartsAt) {
            if (file.Lightmap1 == null) {
                Console.WriteLine("The source file has no lightmap1!");
                return false;
            }

            int factor = 10; // 3 = /8, 4 = /16
            // /8 --> 18944 = 2368
            // /16 --> 18944 = 1184
            Lightmap map = file.Lightmap1;

            int mw = Math.Min(MapRestrict.Width, map.Size.Width);
            int mh = Math.Min(MapRestrict.Height, map.Size.Height);
            int blocksize = Lightmap.BlockSize * factor;
            Console.Write("Exporting bitmaps... ");

            Size small = new Size((int)Math.Ceiling((double)mw / factor), (int)Math.Ceiling((double)mh / factor));

            Bitmap provinces = new Bitmap(small.Width, small.Height, PixelFormat.Format24bppRgb);
            Bitmap terrain = new Bitmap(small.Width, small.Height, PixelFormat.Format8bppIndexed);
            Bitmap rivers = new Bitmap(small.Width, small.Height, PixelFormat.Format8bppIndexed);
            Bitmap topology = new Bitmap(small.Width, small.Height, PixelFormat.Format8bppIndexed);
            IIDConvertor convertor = new InferisEU3CompatIDConvertor(seaStartsAt);

            AssignPaletteFromResource("xterrain.bmp", terrain);
            AssignPaletteFromResource("xrivers.bmp", rivers);
            AssignPaletteFromResource("xtopology.bmp", topology);

            Dictionary<ushort, bool> presentProvinces = new Dictionary<ushort, bool>();

            map.VolatileDecompression = true;
            for (int y = 0; y < mh; y += blocksize) {
                Console.Write("{0}%... ", Math.Ceiling((double)y/mh*100.0));
                //if (Math.Ceiling((double)y / map.Size.Height * 100.0) > 14)
                //    break;
                for (int x = 0; x < mw; x+=blocksize) {
                    int blockwidth = x + blocksize > mw ? mw - x : blocksize;
                    int blockheight = y + blocksize > mh ? mh - y : blocksize;

                    ushort[] idbuf;
                    byte[] riverbuf;
                    ExtractIDs(map.DecodeImage(new Rectangle(x, y, blockwidth, blockheight)), provquery, file.IDMap, factor, out idbuf, out riverbuf);

                    idbuf = Shrink(idbuf, factor, blockwidth, blockheight);
                    riverbuf = ShrinkRivers(riverbuf, factor, blockwidth, blockheight);
                    for (int i = 0; i < idbuf.Length; ++i) {
                        idbuf[i] = provmap.ContainsKey(idbuf[i]) ? provmap[idbuf[i]] : (ushort)0;
                    }

                    int sx = (int)Math.Floor((double)x / factor);
                    int sy = (int)Math.Floor((double)y / factor);
                    int sw = (int)Math.Ceiling((double)blockwidth / factor);
                    int sh = (int)Math.Ceiling((double)blockheight / factor);
                    Rectangle lockRect = new Rectangle(sx, sy, sw, sh);

                    byte[] rgb = IdToRgb(idbuf, convertor);
                    BitmapData bmd = null;
                    try {
                        bmd = provinces.LockBits(lockRect, ImageLockMode.WriteOnly, provinces.PixelFormat);
                        for (int by = 0; by < bmd.Height; ++by) {
                            Marshal.Copy(rgb, by * bmd.Width * 3, new IntPtr(bmd.Scan0.ToInt32() + bmd.Stride * by), bmd.Width * 3); 
                        }
                    }
                    finally {
                        if (bmd != null) provinces.UnlockBits(bmd);
                    }

                    // Terrain
                    rgb = IdToTerrain(idbuf, reverseprovmap, provquery);
                    bmd = null;
                    try {
                        bmd = terrain.LockBits(lockRect, ImageLockMode.WriteOnly, terrain.PixelFormat);
                        for (int by = 0; by < bmd.Height; ++by) {
                            Marshal.Copy(rgb, by * bmd.Width, new IntPtr(bmd.Scan0.ToInt32() + bmd.Stride * by), bmd.Width);
                        }
                    }
                    finally {
                        if (bmd != null)
                            terrain.UnlockBits(bmd);
                    }

                    // Topology
                    rgb = TerrainToTopology(rgb);
                    bmd = null;
                    try {
                        bmd = topology.LockBits(lockRect, ImageLockMode.WriteOnly, topology.PixelFormat);
                        for (int by = 0; by < bmd.Height; ++by) {
                            Marshal.Copy(rgb, by * bmd.Width, new IntPtr(bmd.Scan0.ToInt32() + bmd.Stride * by), bmd.Width);
                        }
                    }
                    finally {
                        if (bmd != null)
                            topology.UnlockBits(bmd);
                    }

                    // rivers
                    bmd = null;
                    try {
                        bmd = rivers.LockBits(lockRect, ImageLockMode.WriteOnly, rivers.PixelFormat);
                        for (int by = 0; by < bmd.Height; ++by) {
                            Marshal.Copy(riverbuf, by * bmd.Width, new IntPtr(bmd.Scan0.ToInt32() + bmd.Stride * by), bmd.Width);
                        }
                    }
                    finally {
                        if (bmd != null)
                            rivers.UnlockBits(bmd);
                    }
                }
            }

            // Assign terrain palette
            //ColorPalette pal = terrain.Palette;
            //unchecked {
            //    foreach (Terrain terr in Terrain.All) {
            //        pal.Entries[terr.ID] = Color.FromArgb((int)0xFF000000 | file.ColorScales.Scales[(int)terr.Color].Average());
            //    }
            //}
            //terrain.Palette = pal;

            // Assign topology palette
            //ColorPalette pal = topology.Palette;
            //for (int i = 1; i < 255; ++i)
            //    pal.Entries[i] = Color.FromArgb(i, i, i);
            //pal.Entries[0] = Color.Red;
            //topology.Palette = pal;

            // Assign rivers palette
            //pal = rivers.Palette;
            //pal.Entries[254] = Color.Pink;
            //for (int i=0; i<254; ++i)
            //    pal.Entries[i] = Color.FromArgb(255-i, 255-i, 255);
            //pal.Entries[255] = Color.White;
            //rivers.Palette = pal;

            Console.WriteLine(" done!");

            Console.Write("Saving bitmaps...");

            provinces.RotateFlip(RotateFlipType.RotateNoneFlipY);
            provinces.Save(Path.Combine(target, "provinces.bmp"), ImageFormat.Bmp);
            terrain.RotateFlip(RotateFlipType.RotateNoneFlipY);
            terrain.Save(Path.Combine(target, "terrain.bmp"), ImageFormat.Bmp);
            rivers.RotateFlip(RotateFlipType.RotateNoneFlipY);
            rivers.Save(Path.Combine(target, "rivers.bmp"), ImageFormat.Bmp);
            topology.RotateFlip(RotateFlipType.RotateNoneFlipY);
            topology.Save(Path.Combine(target, "topology.bmp"), ImageFormat.Bmp);

            Console.WriteLine(" done!");

            return true;
        }

        private static void AssignPaletteFromResource(string id, Bitmap target) {
            using (Stream stream = typeof(Boot).Assembly.GetManifestResourceStream(string.Format("MapExport.{0}", id))) {
                using (Bitmap src = new Bitmap(stream)) {
                    target.Palette = src.Palette;
                    //for (int i = 0; i < target.Palette.Entries.Length; ++i) {
                    //    target.Palette.Entries = Color.FromArgb(src.Palette.Entries[i].ToArgb());
                    //}
                }
            }
        }

        private static ushort[] Shrink(ushort[] idbuf, int factor, int blockwidth, int blockheight) {
            int step = factor;
            int smallblockwidth = (int)Math.Ceiling((double)blockwidth / factor);
            int smallblockheight = (int)Math.Ceiling((double)blockheight / factor);
            ushort[] result = new ushort[smallblockwidth * smallblockheight];

            int sy = 0;
            for (int y = 0; y < blockheight; y += step, sy++) {
                int sx = 0;
                for (int x = 0; x < blockwidth; x += step, sx++) {
                    Dictionary<ushort, int> counter = new Dictionary<ushort, int>();

                    int bw = x + step > blockwidth ? blockwidth - x : step;
                    int bh = y + step > blockheight ? blockheight - y : step;
                    for (int by = 0; by < bh; ++by) {
                        for (int bx = 0; bx < bw; ++bx) {
                            int index = (bx + x) + (by + y) * blockwidth;
                            if (counter.ContainsKey(idbuf[index]))
                                counter[idbuf[index]]++;
                            else
                                counter[idbuf[index]] = 1;
                        }
                    }

                    int max = -1;
                    ushort max_id = 0;
                    foreach (ushort id in counter.Keys) {
                        if (counter[id] > max) {
                            max = counter[id];
                            max_id = id;
                        }
                    }

                    result[sx + sy * smallblockwidth] = max_id;
                }
            }

            return result;
        }

        private static byte[] ShrinkRivers(byte[] riverbuf, int factor, int blockwidth, int blockheight) {
            int step = factor;
            int smallblockwidth = (int)Math.Ceiling((double)blockwidth / factor);
            int smallblockheight = (int)Math.Ceiling((double)blockheight / factor);
            byte[] result = new byte[smallblockwidth * smallblockheight];

            int sy = 0;
            for (int y = 0; y < blockheight; y += step, sy++) {
                int sx = 0;
                for (int x = 0; x < blockwidth; x += step, sx++) {
                    Dictionary<ushort, int> counter = new Dictionary<ushort, int>();

                    int bw = x + step > blockwidth ? blockwidth - x : step;
                    int bh = y + step > blockheight ? blockheight - y : step;
                    counter[0] = 0;
                    counter[254] = 0;
                    counter[255] = 0;
                    for (int by = 0; by < bh; ++by) {
                        for (int bx = 0; bx < bw; ++bx) {
                            counter[riverbuf[(bx + x) + (by + y) * blockwidth]]++;
                        }
                    }

                    int sidx = sx + sy * smallblockwidth;
                    if (counter[254] > counter[0] + counter[255]) {
                        // more ocean then river + land
                        result[sidx] = 254;
                    }
                    else if (counter[0] > 31) {
                        // river
                        result[sidx] = (byte)(((double)counter[0] / (bw * bh) * 253));
                    }
                    else {
                        // land only
                        result[sidx] = 255;
                    }
                }
            }

            return result;
        }

        private static void ExtractIDs(RawImage image, ProvinceQuery provinces, IDMap idmap, int factor, out ushort[] idbuffer, out byte[] riverbuffer) {
            idbuffer = new ushort[image.PixelCount];
            riverbuffer = new byte[image.PixelCount];

            int height = image.Size.Height;
            int width = image.Size.Width;
            Pixel[,] memory = image.Memory;

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    ushort provid = memory[x, y].ID;
                    int idx = x + y * width;
                    if (provinces.IsPti(provid)) {
                        idbuffer[idx] = Province.TerraIncognitaID;
                        riverbuffer[idx] = 255;
                    }
                    else if (provinces.IsOcean(provid)) {
                        riverbuffer[idx] = 254;
                        idbuffer[idx] = provid;
                    }
                    else if (y + image.Y < 8 * factor || y + image.Y >= Lightmap.BaseHeight - 8 * factor) {
                        idbuffer[idx] = Province.TerraIncognitaID;
                        riverbuffer[idx] = 255;
                    }
                    else {
                        if (provinces.IsRiver(provid)) { // Is River, convert province id to nearest land
                            provid = idmap.FindClosestLand(image.X + x, image.Y + y, provid, provinces.Source);
                            // riverbuffer[idx] = 0;
                            riverbuffer[idx] = 255;
                        }
                        else {
                            riverbuffer[idx] = 255;
                        }
                        idbuffer[idx] = provid;
                    }
                }
            }
        }

        private static byte[] IdToRgb(ushort[] idbuffer, IIDConvertor convertor) {
            byte[] result = new byte[idbuffer.Length*3];

            for (int i = 0; i < idbuffer.Length; ++i) {
                int rgb = convertor.ConvertID(idbuffer[i]);

                // R and B switched due to GDI issue
                result[i * 3 + 2] = (byte)((rgb >> 16) & 0xFF);
                result[i * 3 + 1] = (byte)((rgb >> 8) & 0xFF);
                result[i * 3] = (byte)(rgb & 0xFF);
            }

            return result;
        }

        private static byte[] IdToTerrain(ushort[] idbuffer, Dictionary<ushort, ushort> reverseprovmap, ProvinceQuery provinces) {
            byte[] result = new byte[idbuffer.Length];

            for (int i = 0; i < idbuffer.Length; ++i) {
                Province prov = provinces[reverseprovmap[idbuffer[i]]];

                if (provinces.IsPti(prov)) {
                    result[i] = (byte)EU3Terrains.Pti;
                }
                else if (provinces.IsOceanRiver(prov))
                    result[i] = (byte)EU3Terrains.InlandOcean;
                else if (provinces.IsOcean(prov))
                    result[i] = (byte)EU3Terrains.Ocean;
                else if (provinces.IsLand(prov)) {
                    if (prov.Terrain == Terrain.Marsh) {
                        result[i] = (byte)EU3Terrains.Marsh;
                    }
                    else if (prov.Terrain == Terrain.Forest) {
                        result[i] = (byte)EU3Terrains.ConiferousForest;
                    }
                    else if (prov.Terrain == Terrain.Desert) {
                        result[i] = (byte)EU3Terrains.Desert;
                    }
                    else if (prov.Terrain == Terrain.Plains) {
                        result[i] = (byte)EU3Terrains.Plains;
                    }
                    else if (prov.Terrain == Terrain.Mountain) {
                        result[i] = (byte)EU3Terrains.Mountains;
                    }
                    else {
                        result[i] = (byte)EU3Terrains.Farmland;
                    }
                }
                else
                    result[i] = (byte)EU3Terrains.Pti;
            }

            return result;
        }

        enum EU3Terrains {
            Ocean = 0,
            Farmland = 1,
            Plains = 8,
            Steppe = 16,
            ConiferousForest = 24,
            DecidousForest = 32,
            Jungle = 40,
            Marsh = 48,
            Desert = 56,
            Hills = 64,
            Mountains = 72,
            ImpassableMountains = 80,
            InlandOcean = 81,
            Pti = 255
        }

        private static byte[] TerrainToTopology(byte[] terrain) {
            byte[] result = new byte[terrain.Length];

            Random rnd = new Random();
            for (int i = 0; i < terrain.Length; ++i) {
                result[i] = 0;
                if (terrain[i] == (byte)EU3Terrains.Pti)
                    result[i] = 0;
                else if (terrain[i] == (byte)EU3Terrains.Ocean || terrain[i] == (byte)EU3Terrains.InlandOcean) 
                    result[i] = 0;
                else if (terrain[i] == (byte)EU3Terrains.Mountains)
                    result[i] = (byte)rnd.Next(37, 47);
                else if (terrain[i] == (byte)EU3Terrains.ConiferousForest || terrain[i] == (byte)EU3Terrains.DecidousForest)
                    result[i] = (byte)rnd.Next(26, 36);
                else
                    result[i] = (byte)rnd.Next(19, 25);
            }

            return result;
        }

        private static void ShowHelp() {
			Console.WriteLine( "Exports the EU2MAP data to a files readable by EU3" );
			Console.WriteLine( );
			Console.WriteLine( "MEXPORT <source[.eu2map]> [/D:<directory>]" );
			Console.WriteLine( );
			Console.WriteLine( "  <source>        Specifies the file to create an export from. The extension is automatically added if it is omitted." );
			Console.WriteLine( "  /D:<dir>        Specifies the base directory to put the result files." );
		}

    }
}
