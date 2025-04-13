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
        // It also converts SADX death zones to "basicdxmodel" items.
        // If multiple actions are found for a motion, its split type is changed to "actionM" to indicate the requirement of a manual fix.
        private static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string iniPath = Path.Combine(Environment.CurrentDirectory, "ini");
            string assetPath = Environment.CurrentDirectory;
            string[] iniFiles = Directory.GetFiles(iniPath, "*.ini", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < iniFiles.Length; i++)
            {
                Console.WriteLine("\nProcessing split data: " + Path.GetFileName(iniFiles[i]));
                // Load INI file
                IniData iniData = IniSerializer.Deserialize<IniData>(iniFiles[i]);
                IniData newdata = new IniData { DataFilename = iniData.DataFilename, Game = Game.SADX, ImageBase = iniData.ImageBase, Files = new() };
                // Load binary file
                byte[] datafile = File.ReadAllBytes(Path.Combine(assetPath, iniData.DataFilename));
                // Set default key
                if (iniData.ImageBase == null)
                    iniData.ImageBase = 0x400000;
                foreach (var item in iniData.Files)
                {
                    switch (item.Value.Type)
                    {
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
                            List<int> numverts_list = new List<int>();
                            string objName = null;
                            int[] numverts = new int[0];
                            int numparts = 0;
                            if (item.Value.CustomProperties.ContainsKey("refaddr"))
                            {
                                NJS_OBJECT objm = new NJS_OBJECT(datafile, int.Parse(item.Value.CustomProperties["refaddr"], NumberStyles.HexNumber), (uint)iniData.ImageBase, ModelFormat.BasicDX, new Dictionary<int, string>(), new Dictionary<int, Attach>());
                                objName = objm.Name;
                                numverts = objm.GetVertexCounts();
                                if (item.Value.Filename.Contains("shape") || (item.Value.Filename.Contains(".nas.saanim")))
                                    numparts = objm.CountMorph();
                                else
                                    numparts = objm.CountAnimated();
                            }
                            else
                            {
                                if (item.Value.CustomProperties.ContainsKey("numparts"))
                                    numparts = int.Parse(item.Value.CustomProperties["numparts"]);
                                if (item.Value.CustomProperties.ContainsKey("numverts"))
                                {
                                    string[] vertlist = item.Value.CustomProperties["numverts"].Split(',');
                                    for (int v = 0; v < vertlist.Length; v++)
                                    {
                                        numverts_list.Add(int.Parse(vertlist[v]));
                                        numverts = numverts_list.ToArray();
                                    }
                                }
                            }
                            NJS_MOTION mot = new NJS_MOTION(datafile, item.Value.Address, (uint)iniData.ImageBase, numparts, new Dictionary<int, string>(), item.Value.CustomProperties.ContainsKey("shortrot"), numverts);
                            List<int> actionaddr = FindActions(datafile, (uint)iniData.ImageBase, item.Value);
                            if (actionaddr.Count == 1)
                            {
                                Console.WriteLine(item.Value.Filename + ": " + actionaddr[0].ToString("X"));
                                newdata.Files.Add(item.Key, new SplitTools.FileInfo { Type = "action", Address = actionaddr[0], Filename = item.Value.Filename });
                            }
                            else if (actionaddr.Count > 1)
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach (int act in actionaddr)
                                {
                                    sb.Append(act.ToString("X") + ",");
                                }
                                newdata.Files.Add(item.Key, new SplitTools.FileInfo { Type = "actionM", Address = actionaddr[0], Filename = item.Value.Filename, CustomProperties = new Dictionary<string, string> { { "addresses", sb.ToString() } } });
                                Console.WriteLine("MULTIPLE: " + item.Value.Filename + ": " + sb.ToString());
                            }
                            else
                            {
                                Console.WriteLine("NOT FOUND: " + item.Value.Filename);
                            }
                            break;
                    }
                }
                IniSerializer.Serialize(newdata, Path.GetFileName(iniFiles[i]) + "_b.ini");
            }
        }
    }
}