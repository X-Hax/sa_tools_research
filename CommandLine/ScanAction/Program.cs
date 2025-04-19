using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using SAModel;
using SplitTools;
using System.Text;

namespace ScanAction
{
    partial class Program
    {
        // This program opens all splitBinary INI files in the folder and attempts to locate actions for all motions.
        // If multiple actions are found for a motion, its split type is changed to "actionM" to indicate the requirement of a manual fix.
        // The output INI file will only contain landtables, models, actions, motions (if an action wasn't found for them), shape motions and texlists.
        private static void Main(string[] args)
        {
            // Set encoder
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // Set paths
            string iniPath = Path.Combine(Environment.CurrentDirectory, "ini"); // Location of INI files to parse
            string assetPath = Environment.CurrentDirectory; // Location of data files
            // Temporary variables
            int numparts = 0; // Number of model parts in the motion
            bool chunk = false; // Whether the reference model is chunk or not
            // Load INI files           
            string[] iniFiles = Directory.GetFiles(iniPath, "*.ini", SearchOption.TopDirectoryOnly);
            // Clear screen
            Console.Clear();
            Console.Write("\x1b[3J");
            for (int i = 0; i < iniFiles.Length; i++)
            {
                Console.WriteLine("Processing split data: " + Path.GetFileName(iniFiles[i]));
                // Load INI file
                IniData iniData = IniSerializer.Deserialize<IniData>(iniFiles[i]);
                // Set default key
                if (iniData.ImageBase == null)
                    iniData.ImageBase = 0x400000;
                // Create clone INI data for actions
                IniData newdata = new IniData { DataFilename = iniData.DataFilename, Game = Game.SADX, ImageBase = iniData.ImageBase, Files = new() };
                // Load binary file
                byte[] datafile = File.ReadAllBytes(Path.Combine(assetPath, iniData.DataFilename));
                foreach (var item in iniData.Files)
                {
                    switch (item.Value.Type)
                    {
                        /*
                        // This doesn't fit the overall purpose of the tool but was useful for SADX decomp purposes. 
                        // It converts SADX death zone items to "basicdxmodel" items.
                        case "deathzone":
                            List<string> hashes = new List<string>();
                            int num = 0;
                            int address = item.Value.Address;
                            while (ByteConverter.ToUInt32(datafile, address + 4) != 0)
                            {
                                string file_tosave;
                                if (item.Value.CustomProperties.ContainsKey("filename" + num.ToString()))
                                    file_tosave = item.Value.CustomProperties["filename" + num++.ToString()];
                                else
                                    file_tosave = num++.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl";
                                newdata.Files.Add((address + 4).ToString("X"), new SplitTools.FileInfo { Address = datafile.GetPointer(address + 4, (uint)iniData.ImageBase), Type = "basicdxmodel", Filename = file_tosave });
                                address += 8;
                            }
                            break;
                        */
                        case "landtable":
                        case "texlist":
                        case "texnamearray":
                        case "basicdxattach":
                        case "multidxattach":
                        case "model":
                        case "basicmodel":
                        case "basicdxmodel":
                        case "chunkmodel":
                        case "multidxmodel":
                        case "action":
                            newdata.Files.Add(item.Key, item.Value);
                            break;
                        case "motion":
                        case "animation":
                            // Pass shape motions through as-is
                            if (item.Value.Filename.Contains(".nas"))
                            {
                                Console.WriteLine("SHAPE: " + item.Value.Filename);
                                newdata.Files.Add(item.Key, item.Value);
                                break;
                            }
                            // If there's a "refaddr" field, use the reference model to calculate model parts
                            if (item.Value.CustomProperties.ContainsKey("refaddr"))
                            {
                                NJS_OBJECT objm = new NJS_OBJECT(datafile, int.Parse(item.Value.CustomProperties["refaddr"], NumberStyles.HexNumber), (uint)iniData.ImageBase, ModelFormat.BasicDX, new Dictionary<int, string>(), new Dictionary<int, Attach>());
                                numparts = objm.CountAnimated();
                            }
                            // Scan for motions: Pass 1
                            NJS_MOTION mot = new NJS_MOTION(datafile, item.Value.Address, (uint)iniData.ImageBase, numparts, new Dictionary<int, string>(), item.Value.CustomProperties.ContainsKey("shortrot"));
                            List<int> actionaddr = FindActions(datafile, (uint)iniData.ImageBase, item.Value, chunk);
                            if (actionaddr.Count == 1)
                            {
                                try
                                {
                                    //Console.WriteLine("Try action at {0} ({1})", actionaddr[0].ToString("X"), ((uint)iniData.ImageBase + actionaddr[0]).ToString("X"));
                                    NJS_ACTION act = new NJS_ACTION(datafile, actionaddr[0], (uint)iniData.ImageBase, ModelFormat.BasicDX, new Dictionary<int, Attach>());
                                    Console.WriteLine(item.Value.Filename + ": " + string.Format("{0} ({1})", actionaddr[0].ToString("X"), ((uint)iniData.ImageBase + actionaddr[0]).ToString("X")));
                                    newdata.Files.Add(item.Key, new SplitTools.FileInfo { Type = "action", Address = actionaddr[0], Filename = item.Value.Filename });
                                }
                                catch
                                {
                                    Console.WriteLine("ERROR: " + item.Value.Filename);
                                    newdata.Files.Add(item.Key, item.Value);
                                }
                            }
                            else if (actionaddr.Count > 1)
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach (int act in actionaddr)
                                {
                                    sb.Append(string.Format("{0} ({1})", act.ToString("X"), ((uint)iniData.ImageBase + act).ToString("X")));
                                }
                                newdata.Files.Add(item.Key, new SplitTools.FileInfo { Type = "actionM", Address = actionaddr[0], Filename = item.Value.Filename, CustomProperties = new Dictionary<string, string> { { "addresses", sb.ToString() } } });
                                Console.WriteLine("MULTIPLE: " + item.Value.Filename + ": " + sb.ToString());
                            }
                            // Scan for motions: Pass 2
                            else
                            {
                                // Try looking through split items to find a reference model
                                if (!item.Value.CustomProperties.ContainsKey("refaddr"))
                                {
                                    bool found = false;
                                    foreach (var data in iniData.Files)
                                    {
                                        if (found)
                                            break;
                                        switch (data.Value.Type)
                                        {
                                            case "model":
                                            case "basicmodel":
                                            case "basicdxmodel":
                                                if (data.Value.CustomProperties.ContainsKey("animations") && data.Value.CustomProperties["animations"].Contains(Path.GetFileName(item.Value.Filename)))
                                                {
                                                    Console.WriteLine("Found model: " + data.Value.Address.ToString("X") + " (" + ((uint)iniData.ImageBase + data.Value.Address).ToString("X") + ")");
                                                    chunk = false;
                                                    found = true;
                                                    item.Value.CustomProperties.Add("refaddr", data.Value.Address.ToString("X"));
                                                }
                                                break;
                                            case "chunkmodel":
                                                if (data.Value.CustomProperties.ContainsKey("animations") && data.Value.CustomProperties["animations"].Contains(Path.GetFileName(item.Value.Filename)))
                                                {
                                                    chunk = true;
                                                    found = true;
                                                    item.Value.CustomProperties.Add("refaddr", data.Value.Address.ToString("X"));
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                actionaddr = FindActions(datafile, (uint)iniData.ImageBase, item.Value, chunk);
                                if (actionaddr.Count == 1)
                                {
                                    try
                                    {
                                        NJS_ACTION act = new NJS_ACTION(datafile, actionaddr[0], (uint)iniData.ImageBase, ModelFormat.BasicDX, new Dictionary<int, Attach>());
                                        Console.WriteLine(item.Value.Filename + ": " + string.Format("{0} ({1})", actionaddr[0].ToString("X"), ((uint)iniData.ImageBase + actionaddr[0]).ToString("X")));
                                        newdata.Files.Add(item.Key, new SplitTools.FileInfo { Type = "action", Address = actionaddr[0], Filename = item.Value.Filename });
                                    }
                                    catch
                                    {
                                        Console.WriteLine("ERROR: " + item.Value.Filename);
                                        newdata.Files.Add(item.Key, item.Value);
                                    }
                                }
                                else if (actionaddr.Count > 1)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    foreach (int act in actionaddr)
                                    {
                                        sb.Append(string.Format("UNCONFIRMED MULTIPLE: {0} ({1})", act.ToString("X"), ((uint)iniData.ImageBase + act).ToString("X")));
                                    }
                                    newdata.Files.Add(item.Key, new SplitTools.FileInfo { Type = "actionM", Address = actionaddr[0], Filename = item.Value.Filename, CustomProperties = new Dictionary<string, string> { { "addresses", sb.ToString() } } });
                                    Console.WriteLine("MULTIPLE: " + item.Value.Filename + ": " + sb.ToString());
                                }
                                else
                                {
                                    Console.WriteLine("NOT FOUND: " + item.Value.Filename + " (" + ((uint)iniData.ImageBase + item.Value.Address).ToString("X") + ")");
                                    newdata.Files.Add(item.Key, item.Value);
                                }
                            }
                            break;
                    }
                }
                IniSerializer.Serialize(newdata, Path.GetFileName(iniFiles[i]) + "_b.ini");
            }
        }
    }
}