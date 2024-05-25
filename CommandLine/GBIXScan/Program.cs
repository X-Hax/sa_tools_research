using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ArchiveLib;
using VrSharp.Pvr;

// This program:
// 1) Scans a folder for PVM archives and PVR files, and outputs Global Index information
// 2) Transfers GBIX from one PVM to another

namespace GBIXTool
{
    class Program
    {
        static void Main(string[] args)
        {
            bool all = false;
            bool name = false;
            bool transfer = false;
            bool ignorecase = false;
            if (args.Length < 1)
            {
                Console.WriteLine("This program works with Global Indices in PVM archives and PVR files.");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("- Scan a folder for PVM archives and PVR files, and output GBIX information:");
                Console.WriteLine("GBIXTool <folder> [-all] [-name]");
                Console.WriteLine("\tfolder: Folder to scan");
                Console.WriteLine("\t-all: Output all GBIX, not just conflicts");
                Console.WriteLine("\t-name: Ignore GBIX conflicts with PVR files that share the same name (case insensitive)");
                Console.WriteLine("\n- Transfer GBIX from one PVM to another for textures with the same names:");
                Console.WriteLine("GBIXTool -t <file_src> <file_dst> [-i]");
                Console.WriteLine("\tfile_src: File with GBIX to be transferred");
                Console.WriteLine("\tfile_dst: File which will have the new GBIX");
                Console.WriteLine("\t-i: Treat upper and lower case as identical when comparing filenames.");
                Console.WriteLine("\nPress ENTER to exit.");
                Console.ReadLine();
                return;
            }
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-all":
                        all = true;
                        break;
                    case "-name":
                        name = true;
                        break;
                    case "-transfer":
                        transfer = true;
                        break;
                    case "-i":
                        ignorecase = true;
                        break;
                    default:
                        break;
                }
            }

            // Transfer
            if (transfer)
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Wrong argument count, exiting.");
                    return;
                }
                PuyoFile src = new PuyoFile(File.ReadAllBytes(args[1]));
                PuyoFile dst = new PuyoFile(File.ReadAllBytes(args[2]));
                foreach (var srcentry in src.Entries)
                {
                    uint gbx_src = 0;
                    for (int u = 0; u < srcentry.Data.Length - 4; u++)
                    {
                        if (BitConverter.ToUInt32(srcentry.Data, u) == 0x58494247) // GBIX
                            gbx_src = BitConverter.ToUInt32(srcentry.Data, u + 8);
                    }
                    if (gbx_src == 0)
                    {
                        Console.WriteLine("GBIX entry not found in source texture " + srcentry.Name);
                        continue;
                    }
                    else
                    {
                        byte[] gbix_bytes = BitConverter.GetBytes(gbx_src);
                        bool found_dstfile = false;
                        foreach (var dstentry in dst.Entries)
                        {
                            // Find destination entry
                            if (dstentry.Name == srcentry.Name || (ignorecase == true && dstentry.Name.ToLowerInvariant() == srcentry.Name.ToLowerInvariant()))
                            {
                                found_dstfile = true;
                                // Find GBIX data
                                bool found_dstgbix = false;
                                for (int u = 0; u < dstentry.Data.Length - 4; u++)
                                {
                                    if (BitConverter.ToUInt32(dstentry.Data, u) == 0x58494247) // GBIX
                                    {
                                        found_dstgbix = true;
                                        Console.WriteLine("Setting GBIX in destination texture {0} to {1} (0x{2})", dstentry.Name, gbx_src.ToString(), gbx_src.ToString("X"));
                                        dstentry.Data[u + 8] = gbix_bytes[0];
                                        dstentry.Data[u + 9] = gbix_bytes[1];
                                        dstentry.Data[u + 10] = gbix_bytes[2];
                                        dstentry.Data[u + 11] = gbix_bytes[3];
                                    }
                                }
                                // If GBIX data can't be found, add it at start
                                if (!found_dstgbix)
                                {
                                    Console.WriteLine("GBIX data not found in destination texture {0}, adding it to start of file.", dstentry.Name);
                                    byte[] newarray = new byte[dstentry.Data.Length + 16];
                                    newarray[0] = 0x47;
                                    newarray[1] = 0x42;
                                    newarray[2] = 0x49;
                                    newarray[3] = 0x58;
                                    newarray[4] = 0x08;
                                    newarray[5] = 0;
                                    newarray[6] = 0;
                                    newarray[7] = 0;
                                    newarray[8] = gbix_bytes[0];
                                    newarray[9] = gbix_bytes[1];
                                    newarray[10] = gbix_bytes[2];
                                    newarray[11] = 0;
                                    newarray[12] = 0;
                                    newarray[13] = 0;
                                    newarray[14] = 0;
                                    newarray[15] = 0;
                                    Array.Copy(dstentry.Data, 0, newarray, 16, dstentry.Data.Length);
                                    dstentry.Data = newarray;
                                }
                            }
                        }
                        // Entry not found
                        if (!found_dstfile)
                            Console.WriteLine("Destination texture {0} not found.", srcentry.Name);
                    }
                    File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(args[2]), Path.GetFileNameWithoutExtension(args[2]) + "_new" + Path.GetExtension(args[2])), dst.GetBytes());
                }
                return;
            }

            // Scan
            List<GBIXInfo> list = new List<GBIXInfo>();
            string folder = args[0];
            string[] files_prs = System.IO.Directory.GetFiles(folder, "*.prs", SearchOption.AllDirectories);
            string[] files_pvm = System.IO.Directory.GetFiles(folder, "*.pvm", SearchOption.AllDirectories);
            string[] files_pvr = System.IO.Directory.GetFiles(folder, "*.pvr", SearchOption.AllDirectories);
            for (int i = 0; i < files_pvr.Length; i++)
            {
                list.Add(GetGBIXInfoFromPVR(files_pvr[i]));
                Console.WriteLine(files_pvr[i]);
            }
            for (int i = 0; i < files_pvm.Length; i++)
            {
                list.AddRange(GetGBIXInfoFromPVM(files_pvm[i]));
                Console.WriteLine(files_pvm[i]);
            }
            list.Sort((f1, f2) => StringComparer.OrdinalIgnoreCase.Compare(f1.gbix, f2.gbix));
            TextWriter tw = File.CreateText("gbix.txt");
            List<uint> checkedf = new List<uint>();
            for (int info1 = 0; info1 < list.Count; info1++)
            {
                if (checkedf.Contains(list[info1].gbix))
                    continue;
                StringBuilder sb = new StringBuilder();
                bool conflict = false;
                for (int info2 = 0; info2 < list.Count; info2++)
                {
                    if (info1 != info2 && list[info2].gbix == list[info1].gbix && (!name || name && list[info1].pvrName.ToLowerInvariant() != list[info2].pvrName.ToLowerInvariant()))
                    {
                        sb.Append(list[info2].GetName() + ",");
                        conflict = true;
                    }
                    if (all)
                    {
                        conflict = true;
                    }
                }
                if (conflict)
                {
                    if (sb.Length > 0)
                        sb.Remove(sb.Length - 1, 1); // Remove last comma
                    tw.Write(list[info1].gbix.ToString() + ":" + list[info1].GetName());
                    if (sb.Length > 0)
                        tw.Write("," + sb.ToString());
                    tw.Write('\n');
                }
                checkedf.Add(list[info1].gbix);
            }
            tw.Close();
        }

        private class GBIXInfo
        {
            public uint gbix;
            public string pvrName;
            public string pvmName;

            public string GetName()
            {
                return pvmName != "" ? pvmName + ":" + pvrName : pvrName;
            }
        };

        static List<GBIXInfo> GetGBIXInfoFromPVM(string file)
        {
            List<GBIXInfo> infos = new List<GBIXInfo>();
            byte[] data = (Path.GetExtension(file).ToLowerInvariant() == ".prs") ? FraGag.Compression.Prs.Decompress(file) : File.ReadAllBytes(file);
            if (PuyoFile.Identify(data) == PuyoArchiveType.Unknown)
                return new List<GBIXInfo>();
            PuyoFile f = new PuyoFile(data);
            for (int z = 0; z < f.Entries.Count; z++)
            {
                PvrTexture pvr = new PvrTexture(f.Entries[z].Data);
                infos.Add(new GBIXInfo { gbix = pvr.GlobalIndex, pvmName = Path.GetFileName(file), pvrName = f.Entries[z].Name });
            }
            return infos;
        }

        static GBIXInfo GetGBIXInfoFromPVR(string file)
        {
            PvrTexture pvr = new PvrTexture(File.ReadAllBytes(file));
            return new GBIXInfo { gbix = pvr.GlobalIndex, pvmName = "", pvrName = Path.GetFileName(file) };
        }

    }
}