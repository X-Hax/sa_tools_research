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
            string outputPathM = Path.Combine(startPath, "outputM");
            string iniPath = Path.Combine(startPath, "ini");
            string[] iniFiles = Directory.GetFiles(iniPath, "*.ini", SearchOption.TopDirectoryOnly);
            Directory.CreateDirectory(outputPath);
            Console.WriteLine("Log started");
            for (int i = 0; i < iniFiles.Length; i++)
            {
                Console.WriteLine("\nProcessing split data: " + Path.GetFileName(iniFiles[i]));
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
                    string outputFileM = Path.Combine(outputPathM, item.Value.Filename);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFileM));
                    switch (item.Value.Type)
                    {
                        case "deathzone":
                            List<string> hashes = new List<string>();
                            int num = 0;
                            int address = item.Value.Address;
                            while (ByteConverter.ToUInt32(datafile, address + 4) != 0)
                            {
                                string njaFolderLocation = Path.Combine(outputPath, Path.GetDirectoryName(item.Value.Filename));
                                string modelsFolderLocation = Path.Combine(outputPathM, Path.GetDirectoryName(item.Value.Filename));
                                Directory.CreateDirectory(modelsFolderLocation);
                                string file_tosave;
                                if (item.Value.CustomProperties.ContainsKey("filename" + num.ToString()))
                                    file_tosave = item.Value.CustomProperties["filename" + num++.ToString()];
                                else
                                    file_tosave = num++.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl";
                                ModelFormat modelfmt_death = ModelFormat.BasicDX; // Death zones in all games except SADXPC use Basic non-DX models
                                NJS_OBJECT deathObj = new NJS_OBJECT(datafile, datafile.GetPointer(address + 4, (uint)iniData.ImageBase), (uint)iniData.ImageBase, modelfmt_death, labels, new Dictionary<int, Attach>());
                                ModelFile.CreateFile(Path.Combine(modelsFolderLocation, file_tosave), deathObj, null, null, null, null, modelfmt_death);
                                using (TextWriter writer = File.CreateText(Path.Combine(njaFolderLocation, file_tosave[..file_tosave.LastIndexOf('.')])))
                                {
                                    if (!labelsExport.Contains(deathObj.Name))
                                        labelsExport.Add(deathObj.Name);
                                    Console.WriteLine(outputFile);
                                    deathObj.ToNJA(writer, labelsExport, exportDefaults: false);
                                }
                                address += 8;
                            }
                            break;
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
                            Console.WriteLine(outputFile);
                            texlist.Save(outputFileM);
                            break;
                        case "basicdxattach":
                            BasicAttach batt = new BasicAttach(datafile, item.Value.Address, (uint)iniData.ImageBase, true, labels);
                            NJS_OBJECT bobj = new NJS_OBJECT();
                            bobj.Attach = batt;
                            if (batt.Name.StartsWith("attach_"))
                                bobj.Name = ReplaceLabel(batt.Name, "attach", "object");
                            else if (batt.Name.StartsWith("model_"))
                                bobj.Name = ReplaceLabel(batt.Name, "model", "object");
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                if (!labelsExport.Contains(bobj.Name))
                                    labelsExport.Add(bobj.Name);
                                Console.WriteLine(outputFile);
                                bobj.ToNJA(writer, labelsExport, exportDefaults: false);
                            }
                            ModelFile.CreateFile(outputFileM, bobj, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
                            break;
                        case "multidxattach":
                            string[] attsshex = item.Value.CustomProperties["addresses"].Split(',');
                            List<NJS_OBJECT> modelsa = new List<NJS_OBJECT>();
                            for (int m = 0; m < attsshex.Length; m++)
                            {
                                int maddr = int.Parse(attsshex[m], NumberStyles.HexNumber);
                                BasicAttach attm = new BasicAttach(datafile, maddr, (uint)iniData.ImageBase, true, labels);
                                NJS_OBJECT att_head = new NJS_OBJECT();
                                att_head.Attach = attm;
                                if (attm.Name.StartsWith("attach_"))
                                    att_head.Name = ReplaceLabel(attm.Name, "attach", "object");
                                else if (attm.Name.StartsWith("model_"))
                                    att_head.Name = ReplaceLabel(attm.Name, "model", "object");
                                NJS_OBJECT rootm = new NJS_OBJECT();
                                rootm.AddChild(att_head);
                                rootm.Name = "DO_NOT_EXPORT_" + rootm.Name;
                                modelsa.Add(rootm);
                            }
                            NJS_OBJECT roota = new NJS_OBJECT();
                            roota.AddChildren(modelsa);
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Console.WriteLine(outputFile);
                                roota.Name = "DO_NOT_EXPORT";
                                roota.ToNJA(writer, labelsExport, exportDefaults: false);
                            }
                            ModelFile.CreateFile(outputFileM, roota, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
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
                                if (item.Value.CustomProperties.ContainsKey("texlist"))
                                {
                                    NJS_TEXLIST tx = new NJS_TEXLIST(datafile, int.Parse(item.Value.CustomProperties["texlist"], NumberStyles.HexNumber), (uint)iniData.ImageBase, labels);
                                    tx.ToNJA(writer, labelsExport);
                            }
                                obj.ToNJA(writer, labelsExport, exportDefaults: false);
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
                                root.ToNJA(writer, labelsExport, exportDefaults: false);
                            }
                            ModelFile.CreateFile(outputFileM, root, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
                            break;
                        case "motion":
                        case "animation":
                            List<int> numverts_list = new List<int>();
                            string objName = null;
                            int[] numverts = new int[0];
                            int numparts = 0;
                            if (item.Value.CustomProperties.ContainsKey("refaddr"))
                            {
                                NJS_OBJECT objm = new NJS_OBJECT(datafile, int.Parse(item.Value.CustomProperties["refaddr"], NumberStyles.HexNumber), (uint)iniData.ImageBase, ModelFormat.BasicDX, labels, new Dictionary<int, Attach>());
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
                            NJS_MOTION mot = new NJS_MOTION(datafile, item.Value.Address, (uint)iniData.ImageBase, numparts, labels, item.Value.CustomProperties.ContainsKey("shortrot"), numverts);
                            if (mot.Name.StartsWith("motion_"))
                                mot.ActionName = ReplaceLabel(mot.Name, "motion", "action");
                            else if (mot.Name.StartsWith("animation_"))
                                mot.ActionName = ReplaceLabel(mot.Name, "animation", "action");
                            if (string.IsNullOrEmpty(mot.ObjectName))
                                mot.ObjectName = objName;
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Console.WriteLine(outputFile);
                                mot.ToNJA(writer, labelsExport, exportDefaults: false);
                            }
                            mot.Save(outputFileM);
                            break;
                        case "action":
                            NJS_ACTION action = new NJS_ACTION(datafile, item.Value.Address, (uint)iniData.ImageBase, ModelFormat.BasicDX, labels, new Dictionary<int, Attach>());
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Console.WriteLine(outputFile);
                                action.Animation.ToNJA(writer, labelsExport, exportDefaults: false);
                            }
                            action.Animation.Save(outputFileM);
                            break;
                    }
                }
            }
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