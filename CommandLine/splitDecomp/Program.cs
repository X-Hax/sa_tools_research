using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using SAModel;
using SplitTools;

namespace splitDecomp
{
    internal class Program
    {
        private static TextWriter log;

        private static void Main(string[] args)
        {
            string startPath = Environment.CurrentDirectory;
            string assetPath = Environment.CurrentDirectory;
            string outputPath = Path.Combine(startPath, "output");
            string outputPathMdl = Path.Combine(startPath, "outputM");
            string[] iniFiles = Directory.GetFiles(Path.Combine(startPath, "ini"), "*.ini", SearchOption.TopDirectoryOnly);
            log = File.CreateText(Path.Combine(startPath, "output.txt"));
            Directory.CreateDirectory(outputPath);
            Console.WriteLine("Log started");
            for (int i = 0; i < iniFiles.Length; i++)
            {
                WriteLogLine("\nProcessing split data: " + Path.GetFileName(iniFiles[i]));
                // Load labels
                IniData iniData = IniSerializer.Deserialize<IniData>(iniFiles[i]);
                Dictionary<int,string> labels = new Dictionary<int,string>();
                string labelName = Path.GetFileNameWithoutExtension(iniData.DataFilename) + "_labels.txt";
                string labelPath = Path.Combine(startPath, "ini", labelName);
                if (File.Exists(labelPath))
                {
                    Console.WriteLine("Using labels from " + labelName);
                    labels = IniSerializer.Deserialize<Dictionary<int, string>>(Path.Combine(startPath, "ini", labelName));
                }
                else
                    labels = new Dictionary<int,string>();
                byte[] datafile = File.ReadAllBytes(Path.Combine(assetPath, iniData.DataFilename));
                if (iniData.ImageBase == null)
                    iniData.ImageBase = 0x400000;
                List<string> labelsExport = new List<string>();
                foreach (var item in iniData.Files)
                {
                    if (!item.Value.Filename.Contains("."))
                        continue;
                    Console.WriteLine($"{item.Value.Filename}");
                    string outputFile = Path.Combine(outputPath, item.Value.Filename[..item.Value.Filename.LastIndexOf('.')]);
                    string outputFileM = Path.Combine(outputPathMdl, item.Value.Filename);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFileM));
                    switch (item.Value.Type)
                    {
                        case "model":
                        case "basicmodel":
                        case "basicdxmodel":
                            NJS_OBJECT obj = new NJS_OBJECT(datafile, item.Value.Address, (uint)iniData.ImageBase, ModelFormat.BasicDX, labels, new Dictionary<int, Attach>());
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                if (!labelsExport.Contains(obj.Name))
                                    labelsExport.Add(obj.Name);
                                Console.WriteLine(outputFile);
                                obj.ToNJA(writer, labelsExport);
                            }
                            ModelFile.CreateFile(outputFileM, obj, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
                            break;
                        case "multidxmodel":
                            string[] modelshex = item.Value.CustomProperties["addresses"].Split(',');
                            List<NJS_OBJECT> models = new List<NJS_OBJECT>();
                            for (int m = 0; m < modelshex.Length; m++)
                            {
                                int maddr = int.Parse(modelshex[m], NumberStyles.HexNumber);
                                NJS_OBJECT objm = new NJS_OBJECT(datafile, maddr, (uint)iniData.ImageBase, ModelFormat.BasicDX, labels, new Dictionary<int, Attach>());
                                NJS_OBJECT rootm = new NJS_OBJECT();
                                rootm.AddChild(objm);
                                rootm.Name = "DO_NOT_EXPORT_" + rootm.Name;
                                models.Add(rootm);
                            }
                            NJS_OBJECT root = new NJS_OBJECT();
                            root.AddChildren(models);
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Console.WriteLine(outputFile);
                                root.Name = "DO_NOT_EXPORT";
                                root.ToNJA(writer, labelsExport);
                            }
                            ModelFile.CreateFile(outputFileM, root, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
                            break;
                    }
                }
            }
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