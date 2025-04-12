using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using SAModel;
using SplitTools;

namespace splitFromFolder
{
    internal class Program
    {
        private static TextWriter log;

        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowHelp();
                return;
            }
            Dictionary<string, SplitTools.FileInfo> splitData = new Dictionary<string, SplitTools.FileInfo>();
            string basePath = args[0];
            string[] files = Directory.GetFiles(args[0], "*.*", SearchOption.AllDirectories);
            log = File.CreateText(basePath + "_output.txt");
            for (int i = 0; i < files.Length; i++)
            {
                string filename = Path.GetRelativePath(basePath, files[i]);
                string ext = Path.GetExtension(filename).ToLowerInvariant();
                switch (ext)
                {
                    case ".satex":
                        {
                            NJS_TEXLIST texlist = NJS_TEXLIST.Load(files[i]);
                            int addr = int.Parse(texlist.Name.Substring(texlist.Name.Length - 8, 8), NumberStyles.HexNumber);
                            splitData.Add(addr.ToString("X8"), new SplitTools.FileInfo
                            {
                                Address = addr,
                                Filename = filename,
                                Type = "texlist"
                            });
                            WriteLogLine("TEXLIST " + addr.ToString("X8") + ":" + filename);
                            break;
                        }
                    case ".sa1lvl":
                        {
                            LandTable landTable = LandTable.LoadFromFile(files[i]);
                            int addr = int.Parse(landTable.Name.Substring(landTable.Name.Length - 8, 8), NumberStyles.HexNumber);
                            splitData.Add(addr.ToString("X8"), new SplitTools.FileInfo
                            {
                                Address = addr,
                                Filename = filename,
                                Type = "landtable"
                            });
                            WriteLogLine("LANDTABLE " + addr.ToString("X8") + ":" + filename);
                            break;
                        }
                    case ".sa1mdl":
                    case ".sa2mdl":
                        {
                            ModelFile modelFile = new ModelFile(files[i]);
                            if (modelFile.Model.Name.Length == 15)
                            {
                                int addr = int.Parse(modelFile.Model.Name.Substring(modelFile.Model.Name.Length - 8, 8), NumberStyles.HexNumber);
                                splitData.Add(addr.ToString("X8"), new SplitTools.FileInfo
                                {
                                    Address = addr,
                                    Filename = filename,
                                    Type = ((ext == ".sa1mdl") ? "basicdxmodel" : "chunkmodel")
                                });
                                WriteLogLine("OBJECT " + addr.ToString("X8") + ":" + filename);
                            }
                            else
                            {
                                int addr = int.Parse(modelFile.Model.Attach.Name.Substring(modelFile.Model.Attach.Name.Length - 8, 8), NumberStyles.HexNumber);
                                splitData.Add(addr.ToString("X8"), new SplitTools.FileInfo
                                {
                                    Address = addr,
                                    Filename = filename,
                                    Type = ((ext == ".sa1mdl") ? "basicdxattach" : "chunkattach")
                                });
                                WriteLogLine("ATTACH " + addr.ToString("X8") + ":" + filename);
                            }
                            break;
                        }
                    case ".saanim":
                        {
                            NJS_MOTION motion = NJS_MOTION.Load(files[i]);
                            if (!string.IsNullOrEmpty(motion.ActionName))
                            {
                                int addr = int.Parse(motion.ActionName.Substring(motion.ActionName.Length - 8, 8), NumberStyles.HexNumber);
                                splitData.Add(addr.ToString("X8"), new SplitTools.FileInfo
                                {
                                    Address = addr,
                                    Filename = filename,
                                    Type = "action"
                                });
                                WriteLogLine("ACTION " + addr.ToString("X8") + ":" + filename);
                            }
                            else
                            {
                                int addr = int.Parse(motion.Name.Substring(motion.Name.Length - 8, 8), NumberStyles.HexNumber);
                                Dictionary<string, string> props = new Dictionary<string, string>();
                                props.Add("numparts", motion.ModelParts.ToString());                                
                                splitData.Add(addr.ToString("X8"), new SplitTools.FileInfo
                                {
                                    Address = addr,
                                    Filename = filename,
                                    Type = "motion",
                                    CustomProperties = props
                                });
                                WriteLogLine("MOTION " + addr.ToString("X8") + ":" + filename);
                            }
                            break;
                        }
                }
            }
            IniData iniData = new IniData();
            iniData.Files = splitData;
            iniData.DataFilename = basePath + ".dll";
            iniData.Game = Game.SADX;
            iniData.ImageBase = 0x10000000;
            IniSerializer.Serialize(iniData, basePath + "_b.ini");
            log.Close();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("This program scans a folder and outputs an INI file in a SplitBinary compatible format.");
            Console.WriteLine("The last 8 characters in the labels for levels, models (basic/chunk) and animations are parsed as binary addresses.");
            Console.WriteLine("The purpose of the tool is to provide a quick way to reconstruct data output by splitDLL as a SplitBinary INI file.");
            Console.WriteLine("Usage: splitFromFolder <relative_path>");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        private static void WriteLogSingle(string msg)
        {
            log.Write(msg);
            Console.Write(msg);
            log.Flush();
        }

        private static void WriteLogLine(string msg)
        {
            log.WriteLine(msg);
            Console.WriteLine(msg);
            log.Flush();
        }
    }
}