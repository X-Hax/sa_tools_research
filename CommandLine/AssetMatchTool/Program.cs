using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using SAModel;
using SplitTools;
using static CompareTool.Program;

namespace AssetMatchTool
{
    partial class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowHelp();
                return;
            }
            string iniName = args[0];
            string binaryName = "Sonic Adventure DX.exe";
            if (args.Length > 1)
                binaryName = args[1];
            uint binaryKey;
            Directory.CreateDirectory("output");
            Console.WriteLine("Binary name: " + binaryName);
            if (args.Length < 3 || !uint.TryParse(args[2], System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out binaryKey))
            {
                Console.WriteLine("Using the default key of 0x401A00 (SADX Steam EXE)");
                binaryKey = 0x401A00;
            }
            else
                Console.WriteLine("Binary key: " + binaryKey.ToString("X"));
            byte[] datafile = File.ReadAllBytes(Path.GetFullPath(binaryName));
            IniData iniData = IniSerializer.Deserialize<IniData>(iniName);
            byte[] datafile_orig = File.ReadAllBytes(Path.GetFullPath(iniData.DataFilename));
            ByteConverter.BigEndian = iniData.BigEndian;
            uint binaryKeyOrig = 0x400000;
            if (iniData.ImageBase != null)
                binaryKeyOrig= (uint)iniData.ImageBase;
            if (iniData.ImageBase != null)
             binaryKeyOrig = (uint)iniData.ImageBase;
            if (iniData.StartOffset != 0)
            {
                byte[] datafile_new = new byte[iniData.StartOffset + datafile_orig.Length];
                datafile_orig.CopyTo(datafile_new, iniData.StartOffset);
                datafile_orig = datafile_new;
            }
            Console.WriteLine("\n---RESULTS---");
            foreach (var info in iniData.Files)
            {
                bool needcheck = false;
                List<uint> res = new List<uint>();
                switch (info.Value.Type)
                {
                    case "landtable":
                        needcheck = true;
                        Console.Write(info.Value.Filename + "...");
                        LandTable lt = new LandTable(datafile_orig, info.Value.Address, binaryKeyOrig, LandTableFormat.SADX, new Dictionary<int, string>(), iniData.StartOffset);
                        ByteConverter.BigEndian = false;
                        res = FindLandtable(lt, datafile, binaryKey);
                        break;
                    case "model":
                    case "basicmodel":
                    case "basicdxmodel":
                        needcheck = true;
                        Console.Write(info.Value.Filename + "...");
                        ByteConverter.BigEndian = iniData.BigEndian;
                        NJS_OBJECT mdl = new NJS_OBJECT(datafile_orig, info.Value.Address, binaryKeyOrig, ModelFormat.BasicDX, new Dictionary<int, string>(), new Dictionary<int, Attach>());
                        ByteConverter.BigEndian = false;
                        res = FindModel(mdl, datafile, binaryKey);
                        break;
                    case "motion":
                    case "animation":
                        needcheck = true;
                        Console.Write(info.Value.Filename + "...");
                        ByteConverter.BigEndian = iniData.BigEndian;
                        int numparts = 1;
                        if (info.Value.CustomProperties.ContainsKey("numparts"))
                            numparts = int.Parse(info.Value.CustomProperties["numparts"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
                        NJS_MOTION mot = new NJS_MOTION(datafile_orig, info.Value.Address, binaryKeyOrig, numparts, new Dictionary<int, string>(), false);
                        ByteConverter.BigEndian = false;
                        res = FindMotion(mot, numparts, datafile, binaryKey);
                        break;
                    case "action":
                        needcheck = true;
                        Console.Write(info.Value.Filename + "...");
                        ByteConverter.BigEndian = iniData.BigEndian;
                        NJS_ACTION ani = new NJS_ACTION(datafile_orig, info.Value.Address, binaryKeyOrig, ModelFormat.BasicDX, new Dictionary<int, string>(), new Dictionary<int, Attach>());
                        int a_numparts = ani.Animation.ModelParts;
                        ByteConverter.BigEndian = false;
                        res = FindMotion(ani.Animation, a_numparts, datafile, binaryKey);
                        break;
                    default:
                        break;

                }
                if (needcheck)
                {
                    iniData.DataFilename = binaryName;
                    iniData.ImageBase = binaryKey;
                    iniData.Files[info.Key].Address = -1;
                    // Not matched
                    if (res.Count == 0)
                    {
                        Console.Write("\r" + info.Value.Filename + ": NOT FOUND");
                        Console.Write(System.Environment.NewLine);
                    }
                    // Unique match
                    else if (res.Count == 1)
                    {
                        Console.Write("\r" + info.Value.Filename + ": FOUND " + res[0].ToString("X"));
                        Console.Write(System.Environment.NewLine);
                        iniData.Files[info.Key].Address = (int)res[0];
                    }
                    // Multiple matches
                    else
                    {
                        string resstr = "";
                        for (int v = 0; v < res.Count; v++)
                        {
                            resstr += res[v].ToString("X");
                            if (v < res.Count - 1)
                                resstr += ", ";
                        }
                        Console.Write("\r" + info.Value.Filename + ": MULTIPLE found " + resstr);
                        iniData.Files[info.Key].Address = 0;
                        iniData.Files[info.Key].CustomProperties.Add("Multiple", resstr);
                        Console.Write(System.Environment.NewLine);
                    }
                    IniSerializer.Serialize(iniData, Path.Combine("output", Path.GetFileName(iniName)));
                }
            }
        }
        static List<uint> FindMotion(NJS_MOTION src_motion, int numparts, byte[] dataFile, uint imageBase)
        {
            List<uint> motion_list = new List<uint>();
            int step = dataFile.Length / 32;
            List<Task> tasks = new List<Task>();
            for (int s = 0; s < 32; s++)
            {
                int start = Math.Max(0, step * (s - 1));
                int end = Math.Min(s * step, dataFile.Length);
                Task t = new Task(() =>
                {
                    for (int i = start; i < end; i += 4)
                    {
                        //if (i % 4000 == 0)
                        //Console.Write("\r{0} ", i.ToString("X8"));
                        if (!Check.CheckMotion(dataFile, (uint)i, numparts, imageBase, src_motion))
                            continue;
                        else
                        {
                            try
                            {
                                NJS_MOTION dst_motion = new NJS_MOTION(dataFile, (int)i, imageBase, numparts, new Dictionary<int, string>());
                                if (CompareMotion(src_motion, dst_motion, verbose: false))
                                    continue;
                                motion_list.Add((uint)i);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                });
                tasks.Add(t);
                t.Start();
            }
            Task.WaitAll(tasks.ToArray());
            Console.Write("\r");
            return motion_list;
        }

        static List<uint> FindModel(NJS_OBJECT src_model, byte[] dataFile, uint imageBase)
        {
            List<uint> model_list = new List<uint>();
            int step = dataFile.Length / 32;
            List<Task> tasks = new List<Task>();
            for (int s = 0; s < 32; s++)
            {
                int start = Math.Max(0, step * (s - 1));
                int end = Math.Min(s*step, dataFile.Length);
                //Console.WriteLine("Start: {0}, End: {1}",start.ToString("X"), end.ToString("X"));
                Task t = new Task(() => 
                {
                    for (int i = start; i < end; i+=4)
                    {
                        //if (i % 4000 == 0)
                            //Console.Write("\r{0} ", i.ToString("X8"));
                        if (!Check.CheckModel(dataFile, (uint)i, src_model.GetObjects().Length, ModelFormat.BasicDX, imageBase, false, false, src_model))
                            continue;
                        else
                        {
                            try
                            {
                                //Console.WriteLine(i.ToString("X"));
                                NJS_OBJECT dst_model = new NJS_OBJECT(dataFile, (int)i, imageBase, ModelFormat.BasicDX, new Dictionary<int, Attach>());
                                if (CompareModel(src_model, dst_model, verbose: false))
                                    continue;
                                model_list.Add((uint)i);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                });
                tasks.Add(t);
                t.Start();
            }
            Task.WaitAll(tasks.ToArray());
            Console.Write("\r");
            return model_list;
        }

        static List<uint> FindLandtable(LandTable land_src, byte[] dataFile, uint imageBase)
        {
            List<uint> land_lands = new List<uint>();
            COL[] arr_src = land_src.COL.ToArray();
            for (uint i = 0; i < dataFile.Length; i++)
            {
                if (i % 4000 == 0)
                    Console.Write("\r{0} ", i.ToString("X8"));
                if (!Check.CheckLandTable(dataFile, i, LandTableFormat.SADX, imageBase))
                    continue;
                else
                {
                    try
                    {
                        LandTable land_dst = new LandTable(dataFile, (int)i, imageBase, LandTableFormat.SADX);
                        if (land_dst.TextureFileName != land_src.TextureFileName)
                            continue;
                        if (land_dst.Flags != land_src.Flags)
                            continue;
                        if ((short)land_dst.Attributes != (short)land_src.Attributes)
                            continue;
                        if (land_dst.FarClipping != land_src.FarClipping)
                            continue;

                        COL[] arr_dst = land_dst.COL.ToArray();

                        //Console.WriteLine("\nTrying lantable at " + i.ToString("X"));
                        // Assume the same COL item order, otherwise it's fucked
                        if (arr_dst.Length != arr_src.Length)
                        {
                            continue;
                        }
                        // Compare COL items
                        List<ModelDiffData> col = new();
                        bool same = true;
                        for (int c = 0; c < arr_dst.Length; c++)
                        {
                            if (!CompareCOL(arr_src[c], arr_dst[c], c, col))
                            {
                                same = false;
                                break;
                            }
                        }
                        if (same)
                        {
                            land_lands.Add(i);
                        }
                        else
                            continue;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return land_lands;
        }

        static void ShowHelp()
        {
            Console.WriteLine("This program scans the specified binary and attempts to find the data specified in a split INI file.");
            Console.WriteLine("The end result is saved to an INI file that can be used with the split tool.");
            Console.WriteLine("Usage: AssetMatchTool <inifile> <binary> [key]");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}