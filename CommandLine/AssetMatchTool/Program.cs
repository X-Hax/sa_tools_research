using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SAModel;
using SplitTools;

namespace AssetMatchTool
{
    enum AssetType
    {
        Level,
        Model,
        Motion,
        Action
    }

    partial class Program
    {
        static TextWriter log;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowHelp();
                return;
            }
            log = File.CreateText("output.txt");
            List<string> missingItems = new List<string>();
            Dictionary<string, int> numPartsList = new Dictionary<string, int>();
            List<Tuple<int, AssetType, string>> originalAssetList = new List<Tuple<int, AssetType, string>>();
            List<Tuple<List<int>, AssetType, string>> newAssetList = new List<Tuple<List<int>, AssetType, string>>();
            string iniName = args[0];
            string binaryName = "Sonic Adventure DX.exe";
            if (args.Length > 1)
                binaryName = args[1];
            uint binaryKey;
            Directory.CreateDirectory("output");
            WriteLogLine("Binary name: " + binaryName);
            if (args.Length < 3 || !uint.TryParse(args[2], System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out binaryKey))
            {
                WriteLogLine("Using the default key of 0x401A00 (SADX Steam EXE)");
                binaryKey = 0x401A00;
            }
            else
                WriteLogLine("Binary key: " + binaryKey.ToString("X"));
            byte[] datafile = File.ReadAllBytes(Path.GetFullPath(binaryName));
            IniData iniData = IniSerializer.Deserialize<IniData>(iniName);
            byte[] datafile_orig = File.ReadAllBytes(Path.GetFullPath(iniData.DataFilename));
            ByteConverter.BigEndian = iniData.BigEndian;
            uint binaryKeyOrig = 0x400000;
            if (iniData.ImageBase != null)
                binaryKeyOrig = (uint)iniData.ImageBase;
            if (iniData.StartOffset != 0)
            {
                byte[] datafile_new = new byte[iniData.StartOffset + datafile_orig.Length];
                datafile_orig.CopyTo(datafile_new, iniData.StartOffset);
                datafile_orig = datafile_new;
            }
            // Make a list of original assets
            Dictionary<string, SplitTools.FileInfo> newFiles = new Dictionary<string, SplitTools.FileInfo>();
            foreach (var info in iniData.Files)
            {
                switch (info.Value.Type)
                {
                    case "landtable":
                        originalAssetList.Add(new Tuple<int, AssetType, string>(info.Value.Address, AssetType.Level, info.Value.Filename));
                        newFiles.Add(info.Key, info.Value);
                        break;
                    case "model":
                    case "basicmodel":
                    case "basicdxmodel":
                        originalAssetList.Add(new Tuple<int, AssetType, string>(info.Value.Address, AssetType.Model, info.Value.Filename));
                        newFiles.Add(info.Key, info.Value);
                        break;
                    case "motion":
                    case "animation":
                        originalAssetList.Add(new Tuple<int, AssetType, string>(info.Value.Address, AssetType.Motion, info.Value.Filename));
                        if (info.Value.CustomProperties.ContainsKey("numparts"))
                            numPartsList.Add(info.Value.Filename, int.Parse(info.Value.CustomProperties["numparts"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo));
                        newFiles.Add(info.Key, info.Value);
                        break;
                    case "action":
                        originalAssetList.Add(new Tuple<int, AssetType, string>(info.Value.Address, AssetType.Action, info.Value.Filename));
                        newFiles.Add(info.Key, info.Value);
                        break;
                    default:
                        break;
                }
            }
            iniData.Files = newFiles;
            // Sort list by address
            originalAssetList.Sort((y, x) => y.Item1.CompareTo(x.Item1));
            int dataRange = originalAssetList[originalAssetList.Count - 1].Item1 - originalAssetList[0].Item1;
            WriteLogLine("Original data range: " + originalAssetList[0].Item1.ToString("X") + "/" + originalAssetList[originalAssetList.Count - 1].Item1.ToString("X") + " (size " + dataRange.ToString("X") + ")");
            // Pass 1
            WriteLogLine("\n---RESULTS PASS 1---");
            int currentPosition = 0;
            for (int sc = 0; sc < originalAssetList.Count; sc++)
            {
                var scanData = originalAssetList[sc];
                bool updatePos = false;
                List<int> res = new List<int>();
                ByteConverter.BigEndian = iniData.BigEndian;
                WriteLogSingle(scanData.Item3);
                switch (scanData.Item2)
                {
                    case AssetType.Level:
                        LandTable lt = new LandTable(datafile_orig, scanData.Item1, binaryKeyOrig, LandTableFormat.SADX, new Dictionary<int, string>(), iniData.StartOffset);
                        ByteConverter.BigEndian = false;
                        res = FindLandtable(lt, datafile, binaryKey);
                        updatePos = false;
                        break;
                    case AssetType.Model:
                        NJS_OBJECT mdl = new NJS_OBJECT(datafile_orig, scanData.Item1, binaryKeyOrig, ModelFormat.BasicDX, new Dictionary<int, string>(), new Dictionary<int, Attach>());
                        ByteConverter.BigEndian = false;
                        int cnt = mdl.GetObjects().Length;
                        res = FindModel(mdl, datafile, cnt, binaryKey, currentPosition, datafile.Length, false);
                        if (res.Count == 0)
                            res = FindModel(mdl, datafile, cnt, binaryKey, 0, datafile.Length, true);
                        updatePos = true;
                        break;
                    case AssetType.Motion:
                        int numparts = 1;
                        if (numPartsList.ContainsKey(scanData.Item3))
                            numparts = numPartsList[scanData.Item3];
                        NJS_MOTION mot = new NJS_MOTION(datafile_orig, scanData.Item1, binaryKeyOrig, numparts, new Dictionary<int, string>(), false);
                        ByteConverter.BigEndian = false;
                        res = FindMotion(mot, numparts, datafile, binaryKey, currentPosition, datafile.Length, false);
                        if (res.Count == 0)
                            res = FindMotion(mot, numparts, datafile, binaryKey, 0, datafile.Length, true);
                        updatePos = true;
                        break;
                    case AssetType.Action:
                        ByteConverter.BigEndian = iniData.BigEndian;
                        NJS_ACTION ani = new NJS_ACTION(datafile_orig, scanData.Item1, binaryKeyOrig, ModelFormat.BasicDX, new Dictionary<int, string>(), new Dictionary<int, Attach>());
                        int a_numparts = ani.Animation.ModelParts;
                        if (!numPartsList.ContainsKey(scanData.Item3))
                            numPartsList.Add(scanData.Item3, a_numparts);
                        ByteConverter.BigEndian = false;
                        res = FindMotion(ani.Animation, a_numparts, datafile, binaryKey, currentPosition, datafile.Length, false);
                        if (res.Count == 0)
                            res = FindMotion(ani.Animation, a_numparts, datafile, binaryKey, 0, datafile.Length, true);
                        updatePos = true;
                        break;
                }
                // Not matched
                if (res.Count == 0)
                {
                    missingItems.Add(scanData.Item3);
                    WriteLogLine(": NOT FOUND");
                }
                // Unique match
                else if (res.Count == 1)
                {
                    if (updatePos)
                        currentPosition = res[0];
                    WriteLogLine(": FOUND " + (res[0] + binaryKey).ToString("X"));
                    newAssetList.Add(new Tuple<List<int>, AssetType, string>(res, scanData.Item2, scanData.Item3));
                }
                // Multiple matches
                else
                {
                    string resstr = "";
                    for (int v = 0; v < res.Count; v++)
                    {
                        resstr += (res[v] + binaryKey).ToString("X");
                        if (v < res.Count - 1)
                            resstr += ", ";
                    }
                    res.Sort();
                    WriteLogLine(": MULTIPLE found " + resstr);
                    newAssetList.Add(new Tuple<List<int>, AssetType, string>(res, scanData.Item2, scanData.Item3));
                }
            }
            WriteLogLine("\n---RESULTS PASS 2---");
            // Print final list
            iniData.DataFilename = binaryName;
            iniData.ImageBase = binaryKey;
            iniData.StartOffset = 0;
            iniData.BigEndian = false;
            foreach (var ini in iniData.Files)
            {
                ini.Value.Address = -1;
            }
            for (int item = 0; item < newAssetList.Count; item++)
            {
                foreach (var ini in iniData.Files)
                {
                    if (ini.Value.Filename == newAssetList[item].Item3)
                    {
                        // If the source asset was an action, set it to motion
                        if (newAssetList[item].Item2 == AssetType.Action)
                            ini.Value.Type = "motion";
                        // Add model parts for motion
                        if (!ini.Value.CustomProperties.ContainsKey("numparts") && numPartsList.ContainsKey(newAssetList[item].Item3))
                        {
                            ini.Value.CustomProperties.Add("numparts", numPartsList[newAssetList[item].Item3].ToString());
                        }
                        // Not found
                        if (newAssetList[item].Item1.Count == 0)
                        {
                            ini.Value.Address = -1;
                            WriteLogLine("NOT FOUND: " + newAssetList[item].Item3);
                        }
                        // Unique match
                        else if (newAssetList[item].Item1.Count == 1)
                        {
                            ini.Value.Address = newAssetList[item].Item1[0];
                            WriteLogLine((binaryKey + newAssetList[item].Item1[0]).ToString("X") + ": " + newAssetList[item].Item3);

                        }
                        // Multiple matches
                        else
                        {
                            ini.Value.Address = newAssetList[item].Item1[0];
                            string resstr = "";
                            for (int v = 0; v < newAssetList[item].Item1.Count; v++)
                            {
                                resstr += newAssetList[item].Item1[v].ToString("X") + "(" + (binaryKey + newAssetList[item].Item1[v]).ToString("X") + ")";
                                if (v < newAssetList[item].Item1.Count - 1)
                                    resstr += ", ";
                            }
                            WriteLogLine("MULTIPLE: " + newAssetList[item].Item3 + ": " + resstr);
                            ini.Value.CustomProperties.Add("Multiple", resstr);
                        }
                    }
                }
            }
            IniSerializer.Serialize(iniData, Path.Combine("output", Path.GetFileName(iniName)));
            if (missingItems.Count > 0)
            {
                WriteLogLine("\n---MISSING---");
                foreach (var item in missingItems)
                {
                    WriteLogLine(item);
                }
            }
            log.Close();
        }

        static void ShowHelp()
        {
            Console.WriteLine("This program scans the specified binary and attempts to find the data specified in a split INI file.");
            Console.WriteLine("The end result is saved to an INI file that can be used with the split tool.");
            Console.WriteLine("Usage: AssetMatchTool <inifile> <binary> [key]");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        static void WriteLogSingle(string msg)
        {
            log.Write(msg);
            Console.Write(msg);
            log.Flush();
        }

        static void WriteLogLine(string msg)
        {
            log.WriteLine(msg);
            Console.WriteLine(msg);
            log.Flush();
        }
    }
}