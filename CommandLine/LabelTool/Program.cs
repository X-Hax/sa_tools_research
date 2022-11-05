using System;
using System.Collections.Generic;
using System.IO;
using SAModel;
using SplitTools;

// This tool was made to facilitate label transfer between the X360 and 2004 PC versions of SADX. Code is very rough and not meant for regular use.

namespace LabelTool
{
    partial class Program
    {
        static void Main(string[] args)
        {
            switch (args[args.Length - 1])
            {
                // Export labels from SA*MDL, SA*LVL and SAANIM files
                case "-save":
                    LabelSave(args);
                    return;
                // Load labels and apply them to SA*MDL, SA*LVL and SAANIM files
                case "-load":
                    LabelLoad(args);
                    return;
                // Scan a folder and output all NJS_OBJECT labels found in level files
                case "-scanlabels":
                    ScanLabels(args);
                    return;
                // Load two lists and remove all lines in the first list containing string in the second list
                case "-remlabels":
                    CompareRemoveLabels(args);
                    return;
                // Generate script for IDA and DataPointers
                case "-ida":
                    LabelGen_IdaAndPointers(args);
                    return;
                // Generate labels
                case "-g":
                    LabelGen_Main(args);
                    return;
                // Create match list
                case "-cm":
                    LabelMatchList(args);
                    return;
                // Create an address range list
                case "-ad":
                    LabelAddressList(args);
                    return;
                // Find duplicate addresses in split INI files
                case "-di":
                    LabelDuplicateINI(args);
                    return;
                // Strip labels from an SAModel file
                case "-st":
                    LabelStrip(args);
                    return;
            }
        }

        static List<string> addresses;
        static List<string> names;

        static void WriteDataPointerItem(TextWriter output, string type, string name, string address, bool array = false, string count = "")
        {
            if (array)
                output.WriteLine("DataArray(" + type + ", " + name + ", 0x" + address + ", " + count + ");");
            else
                output.WriteLine("DataPointer(" + type + ", " + name + ", 0x" + address + ");");
        }

        static void WriteIdaScriptItem(TextWriter output, string type, string name, string address, bool array = false, string count = "")
        {
            if (array)
            {
                output.WriteLine("del_items(0x" + address + ");");
                output.WriteLine("apply_type(0x" + address + ",\"" + type + " " + name + "[" + count + "]" + ";\", 1);");
                output.WriteLine("set_name(0x" + address + ",\"" + name + "\");");
            }
            else
            {
                output.WriteLine("del_items(0x" + address + ");");
                output.WriteLine("apply_type(0x" + address + ",\"" + type + " " + name + ";\", 1);");
                output.WriteLine("set_name(0x" + address + ",\"" + name + "\");");
            }
        }

        // Strips metadata from .sa1lvl, .sa1mdl and .saanim files
        static void LabelStrip(string[] args)
        {
            string outpath = Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(args[0]) + "_n" + Path.GetExtension(args[0]));
            Console.WriteLine("Output path: {0}", outpath);
            switch (Path.GetExtension(args[0]).ToLowerInvariant())
            {
                case ".sa1lvl":
                case ".sa2lvl":
                    LandTable lt = LandTable.LoadFromFile(args[0]);
                    lt.SaveToFile(outpath, lt.Format, true);
                    break;
                case ".sa1mdl":
                case ".sa2mdl":
                case ".sa2bmdl":
                    ModelFile mf = new ModelFile(args[0]);
                    NJS_OBJECT ob = mf.Model;
                    ModelFile.CreateFile(outpath, ob, null, null, null, new Dictionary<uint, byte[]>(), ob.GetModelFormat(), true);
                    break;
                case ".saanim":
                    NJS_MOTION mot = NJS_MOTION.Load(args[0]);
                    mot.Save(outpath, true);
                    break;
            }
        }

        // Loads transfer_ida.txt and outputs a header with DataArrays and DataPointers
        static void LabelGen_IdaAndPointers(string[] args)
        {
            bool check = false; // Set to true to check for duplicate names/addresses
            if (check)
            {
                addresses = new List<string>();
                names = new List<string>();
            }
            string[] source = File.ReadAllLines(args[0]);
            TextWriter output_ida = File.CreateText("transfer_ida.idc");
            TextWriter output_dp = File.CreateText("transfer_dp.h");
            output_ida.WriteLine("static main()");
            output_ida.WriteLine("{");
            for (int i = 0; i < source.Length; i++)
            {
                //Console.WriteLine("Line: {0}", source[i]);
                string[] div = source[i].Split(',');
                // Array
                if (div.Length == 4)
                {
                    if (check && (addresses.Contains(div[3]) || names.Contains(div[2])))
                    {
                        Console.WriteLine("Duplicate entry: {0} at {1}", div[2], div[3]);
                        continue;
                    }
                    if (check)
                    {
                        addresses.Add(div[3]);
                        names.Add(div[2]);
                    }
                    WriteIdaScriptItem(output_ida, div[1], div[2], div[3], true, div[0]);
                    WriteDataPointerItem(output_dp, div[1], div[2], div[3], true, div[0]);
                }
                // Regular
                else if (div.Length == 3)
                {
                    if (check && (addresses.Contains(div[2]) || names.Contains(div[1])))
                    {
                        Console.WriteLine("Duplicate entry: {0} at {1}", div[1], div[2]);
                        continue;
                    }
                    if (check)
                    {
                        addresses.Add(div[2]);
                        names.Add(div[1]);
                    }
                    WriteIdaScriptItem(output_ida, div[0], div[1], div[2]);
                    WriteDataPointerItem(output_dp, div[0], div[1], div[2]);
                }
                else
                    Console.WriteLine("Unknown line: {0}", source[i]);
            }
            output_ida.WriteLine("}");
            output_ida.Flush();
            output_ida.Close();
            output_dp.Flush();
            output_dp.Close();
        }

        // Builds a label database by comparing two folders containing labelled and unlabelled assets
        static void LabelMatchList(string[] args)
        {
            string folder1 = args[0];
            string folder2 = args[1];
            string[] files = Directory.GetFiles(Path.GetFullPath(folder1), "*.sa*", SearchOption.AllDirectories);
            TextWriter writer_split = File.CreateText("transfer_split.txt");
            TextWriter writer_ida = File.CreateText("transfer_ida.txt");
            Dictionary<string, int> labelindex = IniSerializer.Deserialize<Dictionary<string, int>>(Path.Combine(Environment.CurrentDirectory, "labels", "index.txt"));
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = MakeRelativePath(Path.GetFullPath(folder1), files[i]);
                Console.WriteLine("Processing file {0} of {1}: {2}", i + 1, files.Length, filePath);
                string fileMatch = Path.Combine(Path.GetFullPath(folder2), filePath);
                if (File.Exists(fileMatch))
                    TransferLabels(files[i], fileMatch, writer_split, writer_ida, labelindex);
                else
                    Console.WriteLine("File not found: {0}", fileMatch);
                Console.WriteLine();
            }
            writer_split.Flush();
            writer_split.Close();
            writer_ida.Flush();
            writer_ida.Close();
        }

        // Scans a folder for split INI files and prints duplicate addresses
        static void LabelDuplicateINI(string[] args)
        {
            Dictionary<int, string> addresses = new Dictionary<int, string>();
            string[] files = Directory.GetFiles(Path.GetFullPath(args[0]), "*.ini", SearchOption.TopDirectoryOnly);
            for (int u = 0; u < files.Length; u++)
            {
                if (files[u].Contains("sadxlvl") || files[u].Contains("DLL") || files[u].Contains("CHRMODELS") || files[u].Contains("objdefs") || files[u].Contains("garden"))
                    continue;
                //Console.WriteLine("Checking file {0}", files[u]);
                IniData inifile = IniSerializer.Deserialize<IniData>(files[u]);
                foreach (var split in inifile.Files)
                {
                    if (addresses.ContainsKey(split.Value.Address))
                    {
                        if (!split.Value.Filename.Contains(".dum"))
                            Console.WriteLine("{0}: {1} in {2} already exists as {3}", split.Value.Address.ToString("X"), split.Value.Filename, files[u], addresses[split.Value.Address]);
                    }
                    else
                    {
                        if (!split.Value.Filename.Contains(".dum"))
                            addresses.Add(split.Value.Address, split.Value.Filename);
                    }
                }
            }
        }

        // Scans a folder with unlabelled (address-based) assets and outputs a list of address ranges occupied by each asset
        static void LabelAddressList(string[] args)
        {
            string folder = args[0];
            string[] files = Directory.GetFiles(Path.GetFullPath(folder), "*.sa*", SearchOption.AllDirectories);
            TextWriter writer_list = File.CreateText("transfer_address.txt");
            for (int i = 0; i < files.Length; i++)
            {
                string filePath_rel = MakeRelativePath(Path.GetFullPath(folder), files[i]);
                Console.WriteLine("Processing file {0} of {1}: {2}", i + 1, files.Length, filePath_rel);
                AddressesFromLabels(files[i], filePath_rel, writer_list);
            }
            writer_list.Flush();
            writer_list.Close();
        }

        // Scans a folder with labelled assets and outputs all labels to a text file
        static void ScanLabels(string[] args)
        {
            string folder = args[0];
            string[] files = Directory.GetFiles(Path.GetFullPath(folder), "*.sa*", SearchOption.AllDirectories);
            TextWriter writer_list = File.CreateText("foundlabels.txt");
            for (int i = 0; i < files.Length; i++)
            {
                string filePath_rel = MakeRelativePath(Path.GetFullPath(folder), files[i]);
                Console.WriteLine("Processing file {0} of {1}: {2}", i + 1, files.Length, filePath_rel);
                LabelsFromFile(files[i], writer_list);
            }
            writer_list.Flush();
        }

        // Loads two lists and removes any lines from the first list containing strings in the second list
        static void CompareRemoveLabels(string[] args)
        {
            List<int> linesToRemove = new List<int>();
            string[] source = File.ReadAllLines(args[0]);
            string[] toremove = File.ReadAllLines(args[1]);
            for (int i = 0; i < source.Length; i++)
            {
                for (int u = 0; u < toremove.Length; u++)
                    if (source[i].Contains(toremove[u]))
                        linesToRemove.Add(i);
            }
            TextWriter result = File.CreateText("result.txt");
            for (int i = 0; i < source.Length; i++)
                if (!linesToRemove.Contains(i))
                    result.WriteLine(source[i]);
            result.Flush();
            result.Close();
        }
    }
}