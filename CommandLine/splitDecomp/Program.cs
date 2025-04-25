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
            // Location of the EXE file's parent folder for default INI data
            string startPath = new DirectoryInfo(AppContext.BaseDirectory).Parent.FullName;
            // Variables
            string iniPath = Path.Combine(startPath, "DecompData"); // Location of INI files and labels
            string assetPath = Environment.CurrentDirectory; // Location of binary files
            string outputPath = Path.Combine(Environment.CurrentDirectory, "output"); // Output path for source NJA files
            bool samodel = true; // Whether to output sa1mdl, saanim etc. or not
            string outputPathM = Path.Combine(Environment.CurrentDirectory, "outputM"); // Output path for sa1mdl, saanim etc. files
            // Process arguments
            if (args.Length > 0)
            {
                for (int arg = 0; arg < args.Length; arg++)
                {
                    switch (args[arg].ToLowerInvariant())
                    {
                        case "-?":
                        case "/?":
                            ShowHelp();
                            return;
                        case "-ini":
                            iniPath = Path.GetFullPath(args[arg + 1]);
                            break;
                        case "-out":
                            outputPath = Path.GetFullPath(args[arg + 1]);
                            break;
                        case "-outmdl":
                            outputPathM = Path.GetFullPath(args[arg + 1]);
                            break;
                        case "-nosamdl":
                            samodel = false;
                            break;
                        case "-game":
                            assetPath = Path.GetFullPath(args[arg + 1]);
                            break;
                    }
                }
            }
            // Initialize logger
            Log.Init(Path.Combine(Environment.CurrentDirectory, "splitDecomp.log"));
            // Clear screen
            Console.Clear();
            Console.Write("\x1b[3J");
            // Set text encoder for 932 (required for NJA export)
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // Print out parameters
            Log.WriteLine("Using INI data from folder: {0}", iniPath);
            Log.WriteLine("Using assets in folder: {0}", assetPath);
            Log.WriteLine("NJA output path: {0}", outputPath);
            if (samodel)
                Log.WriteLine("SAModel output path: {0}", outputPathM);
            // Load the dupmodel paths list if it exists
            string duplistpath = Path.Combine(iniPath, "duppath.txt");
            Dictionary<string, string> duplist;
            if (File.Exists(duplistpath))
            {
                duplist = IniSerializer.Deserialize<Dictionary<string, string>>(duplistpath);
                Log.WriteLine("Using duplist path: " + duplistpath);
                string[] oldDupFiles = Directory.GetFiles(outputPath, "dupmo*.dup", SearchOption.AllDirectories);
                if (oldDupFiles != null && oldDupFiles.Length > 0)
                {
                    Log.WriteLine("Deleting old dup files...");
                    foreach (string old in oldDupFiles)
                    {
                        Log.WriteLine("Deleting {0}", old);
                        File.Delete(old);
                    }
                }
            }
            else
            {
                duplist = new Dictionary<string, string>();
                Log.WriteLine("Duplist path not found");
            }
            // Initialize the list of actions that have objects in DLLs
            InitDLLActionList();
            // Create output folder
            Directory.CreateDirectory(outputPath);
            // Start scanning for split files
            string[] iniFiles = Directory.GetFiles(iniPath, "*.ini", SearchOption.TopDirectoryOnly);
            if (iniFiles.Length == 0)
            {
                Log.WriteLine("Error: No INI files found. See help below.");
                ShowHelp();
                return;
            }
            for (int i = 0; i < iniFiles.Length; i++)
            {
                Log.WriteLine("\nProcessing split data: " + Path.GetFileName(iniFiles[i]));
                // Create a list to keep track of exported labels
                List<string> labelsExport = new List<string>();
                // Load INI file
                IniData iniData = IniSerializer.Deserialize<IniData>(iniFiles[i]);
                // Create a list of LandTable items that will be checked for dupmodels. This will be done after all other items are exported.
                List<SplitTools.FileInfo> landtableInfo = new List<SplitTools.FileInfo>();
                // Create a list of NJS_OBJECT and NJS_MOTION names. This is used in dupmodel generation.
                List<string> objLabels = new List<string>();
                List<string> motLabels = new List<string>();
                // Load labels
                Dictionary<int, string> labels = new Dictionary<int, string>();
                string labelName = Path.GetFileNameWithoutExtension(iniData.DataFilename) + "_labels.txt";
                string labelPath = Path.Combine(iniPath, labelName);
                if (File.Exists(labelPath))
                {
                    Log.WriteLine("Using labels from: " + labelPath);
                    labels = IniSerializer.Deserialize<Dictionary<int, string>>(labelPath);
                }
                else
                {
                    Log.WriteLine("Labels file not found for {0}", iniData.DataFilename);
                    labels = new Dictionary<int, string>();
                }
                // Load binary file from the 'system' folder
                string binaryFilePath = Path.Combine(assetPath, "system", iniData.DataFilename);
                // If that didn't work, load the binary file from the parent folder
                if (!File.Exists(binaryFilePath))
                    binaryFilePath = Path.Combine(assetPath, iniData.DataFilename);
                // If that didn't work either, assume an error
                if (!File.Exists(binaryFilePath))
                {
                    Log.WriteLine("Error: Binary file {0} not found. See help below.", binaryFilePath);
                    ShowHelp();
                    return;
                }
                byte[] datafile = File.ReadAllBytes(binaryFilePath);
                // Set default key
                if (iniData.ImageBase == null)
                    iniData.ImageBase = 0x400000;
                // Scan through split entries
                foreach (var item in iniData.Files)
                {
                    //Log.WriteLine("Item: {0}", item.Value.Filename);
                    // Ignore items that have no extension
                    if (!item.Value.Filename.Contains("."))
                        continue;
                    // Set output paths
                    string outputFile = Path.Combine(outputPath, item.Value.Filename[..item.Value.Filename.LastIndexOf('.')]);
                    string outputFileM = Path.Combine(outputPathM, item.Value.Filename);
                    // Create output folders
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                    if (samodel)
                        Directory.CreateDirectory(Path.GetDirectoryName(outputFileM));
                    // Parse items
                    switch (item.Value.Type)
                    {
                        case "landtable":
                            landtableInfo.Add(item.Value);
                            break;
                        case "multitexlist":
                            string[] addrtexs = item.Value.CustomProperties["addresses"].Split(',');
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                for (int m = 0; m < addrtexs.Length; m++)
                                {
                                    int taddr = int.Parse(addrtexs[m], NumberStyles.HexNumber);
                                    NJS_TEXLIST txl = new NJS_TEXLIST(datafile, taddr, (uint)iniData.ImageBase, labels);
                                    txl.ToNJA(writer, labelsExport);
                                    if (samodel)
                                    {
                                        string outputFileM2 = Path.Combine(Path.GetDirectoryName(outputFileM), Path.GetFileNameWithoutExtension(outputFileM) + "_" + m.ToString() + ".tls.satex");
                                        txl.Save(outputFileM2);
                                    }
                                }
                                Log.WriteLine(outputFile);
                            }
                            break;
                        case "texlist":
                        case "texnamearray":
                            NJS_TEXLIST texlist = new NJS_TEXLIST(datafile, item.Value.Address, (uint)iniData.ImageBase, labels);
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                texlist.ToNJA(writer, labelsExport);
                            }
                            Log.WriteLine(outputFile);
                            if (samodel)
                                texlist.Save(outputFileM);
                            break;
                        case "basicdxattach":
                            BasicAttach batt = new BasicAttach(datafile, item.Value.Address, (uint)iniData.ImageBase, true, labels);
                            NJS_OBJECT bobj = new NJS_OBJECT();
                            bobj.Attach = batt;
                            // Generate the NJS_OBJECT label from the NJS_MODEL label.
                            // Ideally the object itself should be ripped, but there are cases when the object doesn't exist.
                            if (batt.Name.StartsWith("attach_"))
                                bobj.Name = ReplaceLabel(batt.Name, "attach", "object");
                            else if (batt.Name.StartsWith("model_"))
                                bobj.Name = ReplaceLabel(batt.Name, "model", "object");
                            Log.WriteLine("Warning: Using generated object name for '{0}'", batt.Name);
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                if (!labelsExport.Contains(bobj.Name))
                                    labelsExport.Add(bobj.Name);
                                Log.WriteLine(outputFile);
                                bobj.ToNJA(writer, labelsExport, exportDefaults: false);
                            }
                            if (samodel)
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
                                if (!item.Value.CustomProperties.ContainsKey("object"))
                                {
                                    Log.WriteLine("Warning: Using generated object name for '{0}'", attm.Name);
                                }
                                else
                                {
                                    att_head.Name = "DO_NOT_EXPORT_" + att_head.Name;
                                }
                                NJS_OBJECT rootm = new NJS_OBJECT();
                                rootm.AddChild(att_head);
                                rootm.Name = "DO_NOT_EXPORT_" + rootm.Name;
                                modelsa.Add(rootm);
                            }
                            NJS_OBJECT roota = new NJS_OBJECT();
                            roota.AddChildren(modelsa);
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Log.WriteLine(outputFile);
                                roota.Name = "DO_NOT_EXPORT";
                                roota.ToNJA(writer, labelsExport, exportDefaults: false);
                                if (item.Value.CustomProperties.ContainsKey("object"))
                                {
                                    NJS_OBJECT njso = new NJS_OBJECT(datafile, int.Parse(item.Value.CustomProperties["object"], NumberStyles.HexNumber), (uint)iniData.ImageBase, ModelFormat.BasicDX, labels, new Dictionary<int, Attach>());
                                        njso.ToNJA(writer, labelsExport, exportDefaults: false);
                                }
                            }
                            if (samodel)
                                ModelFile.CreateFile(outputFileM, roota, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
                            break;
                        case "model":
                        case "basicmodel":
                        case "basicdxmodel":
                        case "chunkmodel":
                            bool chunk = item.Value.Type == "chunkmodel";
                            NJS_OBJECT obj = new NJS_OBJECT(datafile, item.Value.Address, (uint)iniData.ImageBase, chunk ? ModelFormat.Chunk : ModelFormat.BasicDX, labels, new Dictionary<int, Attach>());
                            objLabels.Add(obj.Name);
                            if (obj.Attach != null)
                                objLabels.Add(obj.Attach.Name); // HACK
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                if (!labelsExport.Contains(obj.Name))
                                    labelsExport.Add(obj.Name);
                                Log.WriteLine(outputFile);
                                if (item.Value.CustomProperties.ContainsKey("texlist"))
                                {
                                    NJS_TEXLIST tx = new NJS_TEXLIST(datafile, int.Parse(item.Value.CustomProperties["texlist"], NumberStyles.HexNumber), (uint)iniData.ImageBase, labels);
                                    tx.ToNJA(writer, labelsExport);
                                }
                                obj.ToNJA(writer, labelsExport, exportDefaults: false);
                            }
                            if (samodel)
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
                                Log.WriteLine(outputFile);
                                root.Name = "DO_NOT_EXPORT";
                                root.ToNJA(writer, labelsExport, exportDefaults: false);
                            }
                            if (samodel)
                                ModelFile.CreateFile(outputFileM, root, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
                            break;
                        case "camera":
                            NinjaCamera cam = new NinjaCamera(datafile, item.Value.Address, labels);
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Log.WriteLine(outputFile);
                                cam.ToNJA(writer, labelsExport);
                            }
                            break;
                        case "caction":
                            NinjaCameraAction camAction = new NinjaCameraAction(datafile, item.Value.Address, (uint)iniData.ImageBase, labels);
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Log.WriteLine(outputFile);
                                camAction.ToNJA(writer, labelsExport);
                            }
                            break;
                        case "motion":
                        case "animation":
                            List<int> numverts_list = new List<int>();
                            string objName = null;
                            int[] numverts = new int[0];
                            int numparts = 0;
                            if (item.Value.CustomProperties.ContainsKey("refaddr"))
                            {
                                ModelFormat fmt = ModelFormat.BasicDX;
                                if (item.Value.CustomProperties.ContainsKey("format"))
                                {
                                    switch (item.Value.CustomProperties["format"].ToLowerInvariant())
                                    {
                                        case "chunk":
                                            fmt = ModelFormat.Chunk;
                                            break;
                                        case "gc":
                                            fmt = ModelFormat.GC;
                                            break;
                                        case "xj":
                                            fmt = ModelFormat.XJ;
                                            break;
                                        case "basic":
                                            fmt = ModelFormat.Basic;
                                            break;
                                        case "basicdx":
                                        default:
                                            fmt = ModelFormat.BasicDX;
                                            break;
                                    }
                                }
                                NJS_OBJECT objm = new NJS_OBJECT(datafile, int.Parse(item.Value.CustomProperties["refaddr"], NumberStyles.HexNumber), (uint)iniData.ImageBase, fmt, labels, new Dictionary<int, Attach>());
                                objName = objm.Name;
                                numverts = objm.GetVertexCounts();
                                if (item.Value.Filename.Contains("shape") || item.Value.Filename.Contains(".nas.saanim") || item.Value.Filename.Contains(".cut.saanim"))
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
                            else if (mot.Name.StartsWith("_motion_"))
                                mot.ActionName = ReplaceLabel(mot.Name, "_motion", "_action");
                            else if (mot.Name.StartsWith("animation_"))
                                mot.ActionName = ReplaceLabel(mot.Name, "animation", "action");
                            if (string.IsNullOrEmpty(mot.ObjectName))
                                mot.ObjectName = objName;
                            Log.WriteLine("Warning: Using generated action name for motion {0}", mot.Name);
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Log.WriteLine(outputFile);
                                mot.ToNJA(writer, labelsExport, exportDefaults: false);
                                // If the motion is in the "actions with object in DLL" list, add an action
                                if (mot.ActionName != null && ActionsWithObjectsInDLLs.ContainsKey(mot.ActionName))
                                    ActionToNJAfromText(mot.ActionName, ActionsWithObjectsInDLLs[mot.ActionName], mot.Name, writer);
                            }
                            if (samodel)
                                mot.Save(outputFileM);
                            motLabels.Add(mot.ActionName);
                            motLabels.Add(mot.Name);
                            break;
                        case "action":
                            int np = 0; // For cases when the size of the MDATA array is wrong
                            ushort mdatas = 0; // For cases when the MDATA type is wrong (MDATA2 instead of MDATA3 etc.)
                            if (item.Value.CustomProperties.ContainsKey("numparts"))
                                np = int.Parse(item.Value.CustomProperties["numparts"]);
                            if (item.Value.CustomProperties.ContainsKey("mdata"))
                                mdatas = ushort.Parse(item.Value.CustomProperties["mdata"]);
                            NJS_ACTION action = new NJS_ACTION(datafile, item.Value.Address, (uint)iniData.ImageBase, ModelFormat.BasicDX, labels, new Dictionary<int, Attach>(), np);
                            if (!labels.ContainsKey(item.Value.Address) && !LabelIsNumerical(action.Animation.Name))
                            {
                                if (action.Animation.Name.StartsWith("motion_"))
                                    action.Name = ReplaceLabel(action.Animation.Name, "motion", "action");
                                else if (action.Animation.Name.StartsWith("_motion_"))
                                    action.Name = ReplaceLabel(action.Animation.Name, "_motion", "_action");
                                else if (action.Animation.Name.StartsWith("animation_"))
                                    action.Name = ReplaceLabel(action.Animation.Name, "animation", "action");
                                action.Animation.ActionName = action.Name;
                                Log.WriteLine(string.Format("Warning: label for action at {0} ({1}) missing, using '{2}'", item.Value.Address.ToString("X"), ((uint)iniData.ImageBase + item.Value.Address).ToString("X"), action.Name));
                            }
                            using (TextWriter writer = File.CreateText(outputFile))
                            {
                                Log.WriteLine(outputFile);
                                action.Animation.ToNJA(writer, labelsExport, exportDefaults: false, mdatatype: mdatas);
                            }
                            if (samodel)
                                action.Animation.Save(outputFileM);
                            motLabels.Add(action.Animation.Name);
                            motLabels.Add(action.Name);
                            break;
                            // This is left over in case it's needed again in the future
                            /*
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
                                    if (samodel)
                                        ModelFile.CreateFile(Path.Combine(modelsFolderLocation, file_tosave), deathObj, null, null, null, null, modelfmt_death);
                                    using (TextWriter writer = File.CreateText(Path.Combine(njaFolderLocation, file_tosave[..file_tosave.LastIndexOf('.')])))
                                    {
                                        if (!labelsExport.Contains(deathObj.Name))
                                            labelsExport.Add(deathObj.Name);
                                        Log.WriteLine(outputFile);
                                        deathObj.ToNJA(writer, labelsExport, exportDefaults: false);
                                    }
                                    address += 8;
                                }
                                break;
                            */
                    }
                }
                // Generate dupmodel for landtables
                if (landtableInfo.Count > 0)
                {
                    // Create an empty landtable list for dupmodel checking
                    List<LandTable> landTables = new List<LandTable>();
                    foreach (var item in landtableInfo)
                    {
                        // Set output paths
                        string outputFile = Path.Combine(outputPath, item.Filename[..item.Filename.LastIndexOf('.')]);
                        string outputFileM = Path.Combine(outputPathM, item.Filename);
                        // Create output folders
                        Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                        if (samodel)
                            Directory.CreateDirectory(Path.GetDirectoryName(outputFileM));
                        // Load LandTable
                        LandTable landTable = new LandTable(datafile, item.Address, (uint)iniData.ImageBase, LandTableFormat.SADX, labels);
                        landTables.Add(landTable);
                        // Save LandTable
                        if (samodel)
                            landTable.SaveToFile(outputFileM, LandTableFormat.SADX);
                        // Set names
                        string landFilename = Path.GetFileName(item.Filename);
                        string landLocation = Path.GetDirectoryName(item.Filename);
                        if (duplist.ContainsKey(landFilename))
                        {
                            Log.WriteLine("\nUsing duplist for " + landFilename);
                            string dupShortLocation = duplist[landFilename];
                            GenerateDup(landTables.ToArray(), Path.Combine(outputPath, landLocation, dupShortLocation), objLabels, motLabels);
                        }
                    }
                }
            }
            // Close log
            Log.Finish();
        }

        private static void ShowHelp()
        {
            Log.WriteLine("This tool outputs NJA assets for SADX decomp.");
            Log.WriteLine("Usage: splitDecomp -ini \"path_ini\" -game \"path_game\" -out \"path_output\" -outmdl \"path_outputmdl\" [-nosamdl]");
            Log.WriteLine("\npath_to_ini: Location of decomp INI files and labels");
            Log.WriteLine("path_game: Game folder(location of sonic.exe)");
            Log.WriteLine("path_output: Output folder for NJA files (e.g. \"D:\\sadx-decomp\\SonicAdventure\\sonic\")");
            Log.WriteLine("path_outputmdl: Output folder for sa1mdl, saanim and sa1lvl files");
            Log.WriteLine("\n-nosamdl: do not output sa1mdl, saanim and sa1lvl files");
            Log.Write("\nPress any key to exit.");
            Console.ReadLine();
        }

        // Replaces the first part of a label with a different part (e.g. turn "model_00000000" into "object_00000000"
        private static string ReplaceLabel(string label, string src, string dst)
        {
            return dst + label.Substring(src.Length);
        }

        // Checks whether a label ends with a hex number (e.g. "object_00000000")
        private static bool LabelIsNumerical(string label)        
        {
            if (label.Length < 8)
                return false;
            string number = label.Substring(label.Length-8, 8);
            bool res = int.TryParse(number, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int test);
            return res;
        }
    }
}