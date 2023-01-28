using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ArchiveLib;
using VrSharp.Pvr;

// This program scans a folder for PVM archives and PVR files, and outputs Global Index information

namespace GBIXScan
{
    class Program
    {
        static void Main(string[] args)
        {
            bool all = false;
            bool name = false;
            if (args.Length < 1)
            {
                Console.WriteLine("This program scans a folder for PVM archives and PVR files, and outputs Global Index information.");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("GBIXScan [folder] [-all] [-name]");
                Console.WriteLine("\nfolder: Folder to scan");
                Console.WriteLine("\n-all: Output all GBIX, not just conflicts");
                Console.WriteLine("\n-name: Ignore GBIX conflicts with PVR files that share the same name (case insensitive)");
                Console.WriteLine("\nPress ENTER to exit.");
                Console.ReadLine();
                return;
            }
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-all":
                        all = true;
                        break;
                    case "-name":
                        name = true;
                        break;
                    default:
                        break;
                }
            }
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