using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using SAModel;
using SplitTools;

namespace splitDecomp
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string startPath = Environment.CurrentDirectory;
            string assetPath = Environment.CurrentDirectory;
            string outputPath = Path.Combine(startPath, "output");
            string outputPathMdl = Path.Combine(startPath, "outputM");
            string iniPath = Path.Combine(startPath, "ini");
            string[] iniFiles = Directory.GetFiles(iniPath, "*.ini", SearchOption.TopDirectoryOnly);
            log = File.CreateText(Path.Combine(startPath, "output.txt"));
            Directory.CreateDirectory(outputPath);
            Console.WriteLine("Log started");
            for (int i = 0; i < iniFiles.Length; i++)
            {
                WriteLogLine("\nProcessing split data: " + Path.GetFileName(iniFiles[i]));
                // Create a list to keep track of exported labels
                List<string> labelsExport = new List<string>();
                // Load INI file
                IniData iniData = IniSerializer.Deserialize<IniData>(iniFiles[i]);
                // Create an empty landtable list for dupmodel checking
                List<LandTable> landTables = new List<LandTable>();
                // Load the dupmodel paths list if it exists
                string duplistpath = Path.Combine(iniPath, "duppath.txt");
                Dictionary<string, string> duplist;
                if (File.Exists(duplistpath))
                {
                    duplist = IniSerializer.Deserialize<Dictionary<string, string>>(duplistpath);
                    Console.WriteLine("Using duplist path: " + duplistpath);
                }
                else
                {
                    duplist = new Dictionary<string, string>();
                    Console.WriteLine("Duplist path not found");
                }
                // Load labels
                Dictionary<int, string> labels = new Dictionary<int, string>();
                string labelName = Path.GetFileNameWithoutExtension(iniData.DataFilename) + "_labels.txt";
                string labelPath = Path.Combine(startPath, "ini", labelName);
                if (File.Exists(labelPath))
                {
                    Console.WriteLine("Using labels from " + labelName);
                    labels = IniSerializer.Deserialize<Dictionary<int, string>>(Path.Combine(startPath, "ini", labelName));
                }
                else
                {
                    Console.WriteLine("Labels file not found");
                    labels = new Dictionary<int, string>();
                }
                // Load binary file
                byte[] datafile = File.ReadAllBytes(Path.Combine(assetPath, iniData.DataFilename));
                // Set default key
                if (iniData.ImageBase == null)
                    iniData.ImageBase = 0x400000;
                foreach (var item in iniData.Files)
                {
                    if (!item.Value.Filename.Contains("."))
                        continue;
                    string outputFile = Path.Combine(outputPath, item.Value.Filename[..item.Value.Filename.LastIndexOf('.')]);
                    string outputFileM = Path.Combine(outputPathMdl, item.Value.Filename);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFileM));
                    switch (item.Value.Type)
                    {
                        case "landtable":
                            LandTable landTable = new LandTable(datafile, item.Value.Address, (uint)iniData.ImageBase, LandTableFormat.SADX);
                            landTables.Add(landTable);
                            // Duplist
                            string landFilename = Path.GetFileName(item.Value.Filename);
                            string landLocation = Path.GetDirectoryName(item.Value.Filename);
                            Console.WriteLine("Generating duplist for " + landFilename);
                            if (duplist.ContainsKey(landFilename))
                            {
                                string dupShortLocation = duplist[landFilename];
                                GenerateDup(landTables.ToArray(), Path.Combine(outputPath, landLocation, dupShortLocation));
                            }
                            break;
                        case "texlist":
                        case "texnamearray":
                            NJS_TEXLIST texlist = new NJS_TEXLIST(datafile, item.Value.Address, (uint)iniData.ImageBase, labels);
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                texlist.ToNJA(writer, labelsExport);
                            }
                            texlist.Save(outputFileM);
                            break;
                        case "basicdxattach":
                            BasicAttach batt = new BasicAttach(datafile, item.Value.Address, (uint)iniData.ImageBase, true, labels);
                            NJS_OBJECT bobj = new NJS_OBJECT();
                            bobj.Attach = batt;
                            if (batt.Name.StartsWith("attach_"))
                                bobj.Name = ReplaceLabel(batt.Name, "attach", "object");
                            if (batt.Name.StartsWith("model_"))
                                bobj.Name = ReplaceLabel(batt.Name, "model", "object");
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                if (!labelsExport.Contains(bobj.Name))
                                    labelsExport.Add(bobj.Name);
                                Console.WriteLine(outputFile);
                                bobj.ToNJA(writer, labelsExport);
                            }
                            ModelFile.CreateFile(outputFileM, bobj, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
                            break;
                        case "model":
                        case "basicmodel":
                        case "basicdxmodel":
                        case "chunkmodel":
                            bool chunk = item.Value.Type == "chunkmodel";
                            NJS_OBJECT obj = new NJS_OBJECT(datafile, item.Value.Address, (uint)iniData.ImageBase, chunk ? ModelFormat.Chunk : ModelFormat.BasicDX, labels, new Dictionary<int, Attach>());
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                if (!labelsExport.Contains(obj.Name))
                                    labelsExport.Add(obj.Name);
                                Console.WriteLine(outputFile);
                                obj.ToNJA(writer, labelsExport);
                            }
                            ModelFile.CreateFile(outputFileM, obj, null, null, null, new Dictionary<uint, byte[]>(), chunk ? ModelFormat.Chunk : ModelFormat.BasicDX);
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
                        case "motion":
                        case "animation":
                            List<int> numverts_list = new List<int>();
                            int[] numverts = new int[0];
                            string objName = null;
                            if (item.Value.CustomProperties.ContainsKey("numverts"))
                            {
                                string[] vertlist = item.Value.CustomProperties["numverts"].Split(',');
                                for (int v = 0; v < vertlist.Length; v++)
                                {
                                    numverts_list.Add(int.Parse(vertlist[v]));
                                    numverts = numverts_list.ToArray();
                                }
                            }
                            else if (item.Value.CustomProperties.ContainsKey("refaddr"))
                            {
                                NJS_OBJECT objm = new NJS_OBJECT(datafile, int.Parse(item.Value.CustomProperties["refaddr"], NumberStyles.HexNumber), (uint)iniData.ImageBase, ModelFormat.BasicDX, labels, new Dictionary<int, Attach>());
                                numverts = objm.GetVertexCounts();
                                objName = objm.Name;
                            }
                            NJS_MOTION mot = new NJS_MOTION(datafile, item.Value.Address, (uint)iniData.ImageBase, int.Parse(item.Value.CustomProperties["numparts"]), labels, item.Value.CustomProperties.ContainsKey("shortrot"), numverts);
                            if (mot.Name.StartsWith("motion_"))
                                mot.ActionName = ReplaceLabel(mot.Name, "motion", "action");
                            else if (mot.Name.StartsWith("animation_"))
                                mot.ActionName = ReplaceLabel(mot.Name, "animation", "action");
                            if (string.IsNullOrEmpty(mot.ObjectName))
                                mot.ObjectName = objName;
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Console.WriteLine(outputFile);
                                mot.ToNJA(writer, labelsExport);
                            }
                            mot.Save(outputFileM);
                            break;
                        case "action":
                            NJS_ACTION action = new NJS_ACTION(datafile, item.Value.Address, (uint)iniData.ImageBase, ModelFormat.BasicDX, new Dictionary<int, Attach>());
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Console.WriteLine(outputFile);
                                action.Animation.ToNJA(writer, labelsExport);
                            }
                            action.Animation.Save(outputFileM);
                            break;
                    }
                }
            }
            log.Close();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        // Replaces the first part of a label with a different part (e.g. turn "model_00000000" into "object_00000000"
        private static string ReplaceLabel(string label, string src, string dst)
        {
            return dst + label.Substring(src.Length);
        }

    }
}