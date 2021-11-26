using System;
using System.Collections.Generic;
using System.IO;
using SAModel.SAEditorCommon.ProjectManagement;
using SplitTools;
using SplitTools.SplitDLL;

// This tool is meant to transfer SAMDL project mode metadata between split INI files.

namespace MetaTransfer
{
	partial class Program
	{
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("SAMDL Metadata Transfer Tool");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("Creating a meta database:");
                Console.WriteLine("MetaTransfer <templateFile> <dataBaseFile>");
                Console.WriteLine("\nImporting a meta database:");
                Console.WriteLine("MetaTransfer <dataBaseFile> <outputTemplateFile>");
                Console.WriteLine("\nUse quotes for any filename that constains spaces.");
                Console.WriteLine("\nPress ENTER to exit.");
                Console.ReadLine();
                return;
            }
            if (Path.GetExtension(args[0].ToLowerInvariant()) == ".xml")
                CreateMetadata(args);
            else
                InsertMetadata(args);
        }

        static void CreateMetadata(string[] args)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Console.WriteLine("Loading template: {0}", args[0]);
            Templates.SplitTemplate template = ProjectFunctions.openTemplateFile(Path.GetFullPath(args[0]), true);
            string iniFolder = Path.Combine(Environment.CurrentDirectory, template.GameInfo.DataFolder);
            // Model extension crap
            string modelext = ".sa1mdl";
            if (template.GameInfo.GameName.Contains("SA2"))
            {
                if (template.GameInfo.GameName.Contains("PC") || template.GameInfo.GameName.Contains("SA2B"))
                    modelext = ".sa2bmdl";
                else
                    modelext = ".sa2mdl";
            }
            foreach (Templates.SplitEntry splitData in template.SplitEntries)
            {
                Console.WriteLine("Split entry: {0}", splitData.IniFile);
                string inipath = Path.Combine(iniFolder, (splitData.IniFile.ToLower() + ".ini"));
                switch (Path.GetExtension(splitData.SourceFile).ToLowerInvariant())
                {
                    // SplitBinary
                    case ".exe":
                    case ".bin":
                    case ".prs":
                    case ".rel":
                        IniData inifile = IniSerializer.Deserialize<IniData>(inipath);
                        foreach (KeyValuePair<string, SplitTools.FileInfo> item in new List<KeyValuePair<string, SplitTools.FileInfo>>(inifile.Files))
                        {
                            switch (item.Value.Type)
                            {
                                case "model":
                                case "basicmodel":
                                case "basicdxmodel":
                                case "chunkmodel":
                                case "gcmodel":
                                    if (!result.ContainsKey(item.Value.Filename))
                                    {
                                        List<string> meta = new List<string>();
                                        // Name
                                        meta.Add(item.Key);
                                        // PVM
                                        if (item.Value.CustomProperties.ContainsKey("texture"))
                                            meta.Add(item.Value.CustomProperties["texture"]);
                                        // TexIDs
                                        if (item.Value.CustomProperties.ContainsKey("texids"))
                                            meta.Add(item.Value.CustomProperties["texids"]);
                                        // or texlist INI
                                        else if (item.Value.CustomProperties.ContainsKey("texnames"))
                                            meta.Add(item.Value.CustomProperties["texnames"]);
                                        result.Add(NormalizePath(item.Value.Filename), string.Join("|", meta));
                                        Console.WriteLine(item.Value.Filename + ": {0}", item.Key);
                                    }
                                    else
                                        Console.WriteLine("Duplicate entry for {0} found in {1}", item.Value.Filename, splitData.IniFile);
                                    break;
                            }
                        }
                        break;
                    // SplitDLL
                    case ".dll":
                        IniDataSplitDLL inidll = IniSerializer.Deserialize<IniDataSplitDLL>(inipath);
                        foreach (KeyValuePair<string, SplitTools.SplitDLL.FileInfo> item in inidll.Files)
                        {
                            switch (item.Value.Type)
                            {
                                // Single model
                                case "model":
                                case "object":
                                case "basicmodel":
                                case "basicdxmodel":
                                case "chunkmodel":
                                case "gcmodel":
                                case "morph":
                                case "attach":
                                case "basicattach":
                                case "basicdxattach":
                                case "chunkattach":
                                case "gcattach":
                                    if (item.Value.CustomProperties.ContainsKey("meta"))
                                    {
                                        if (!result.ContainsKey(item.Value.Filename))
                                        {
                                            result.Add(NormalizePath(item.Value.Filename), item.Value.CustomProperties["meta"]);
                                            Console.WriteLine(item.Value.Filename + ": {0}", item.Value.CustomProperties["meta"]);
                                        }
                                        else
                                            Console.WriteLine("Duplicate entry for {0} found in {1}", item.Value.Filename, splitData.IniFile);
                                    }
                                    break;
                                // Model array
                                case "modelarray":
                                case "modelsarray":
                                case "attacharray":
                                case "basicattacharray":
                                case "basicdxattacharray":
                                case "chunkattacharray":
                                case "gcattacharray":
                                    for (int i = 0; i < item.Value.Length; i++)
                                    {
                                        if (item.Value.CustomProperties.ContainsKey("meta" + i.ToString()))
                                        {
                                            string modelpath;
                                            if (!item.Value.CustomProperties.ContainsKey("filename" + i.ToString()))
                                            {
                                                Console.WriteLine("Warning: Filename for {0}:{1} not specified", item.Key, i.ToString());
                                                modelpath = Path.Combine(item.Value.Filename, i.ToString("D3") + modelext);
                                            }
                                            else
                                                modelpath = Path.Combine(item.Value.Filename, item.Value.CustomProperties["filename" + i.ToString()] + modelext);
                                            if (!result.ContainsKey(modelpath))
                                            {
                                                result.Add(NormalizePath(modelpath), item.Value.CustomProperties["meta" + i.ToString()]);
                                                Console.WriteLine(modelpath + ": {0}", item.Value.CustomProperties["meta" + i.ToString()]);
                                            }
                                            else
                                                Console.WriteLine("Duplicate entry for {0} found in {1}", item.Value.Filename, splitData.IniFile);
                                        }
                                    }
                                    break;
                                // Action array
                                case "actionarray":
                                    for (int i = 0; i < item.Value.Length; i++)
                                    {
                                        if (item.Value.CustomProperties.ContainsKey("meta" + i.ToString() + "_m"))
                                        {
                                            string modelpath = Path.Combine(item.Value.Filename, item.Value.CustomProperties["filename" + i.ToString() +"_m"] + modelext);
                                            if (!result.ContainsKey(modelpath))
                                            {
                                                result.Add(NormalizePath(modelpath), item.Value.CustomProperties["meta" + i.ToString() + "_m"]);
                                                Console.WriteLine(modelpath + ": {0}", item.Value.CustomProperties["meta" + i.ToString() + "_m"]);
                                            }
                                            else
                                                Console.WriteLine("Duplicate entry for {0} found in {1}", item.Value.Filename, splitData.IniFile);
                                        }
                                    }
                                        break;
                                default:
                                    break;
                            }
                        }
                        break;
                    // SplitNB
                    case ".nb":
                        Dictionary<int, string> nbFilenames = IniSerializer.Deserialize<Dictionary<int, string>>(inipath);
                        foreach (var nbitem in nbFilenames)
                        {
                            List<string> resultNBMeta = new List<string>();
                            string[] nbMeta = nbitem.Value.Split('|');
                            // 0 is path without extension, 1 is name, 2 is texture
                            if (nbMeta.Length > 1)
                            {
                                for (int i = 1; i < nbMeta.Length; i++)
                                    resultNBMeta.Add(nbMeta[i]);
                                Console.WriteLine("{0}:{1}", nbMeta[0] + modelext, resultNBMeta[0]);
                                result.Add(NormalizePath(nbMeta[0] + modelext), string.Join("|", resultNBMeta));
                            }
                            else
                                Console.WriteLine("No metadata for item {0}:{1}", nbitem.Key.ToString(), nbMeta[0] + modelext);
                        }
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("Output file: {0}", Path.GetFullPath(args[1]));
            IniSerializer.Serialize(result, args[1]);
        }

        static void InsertMetadata(string[] args)
        {
            Console.WriteLine("Loading metadata: {0}", args[0]);
            Dictionary<string, string> metadata = IniSerializer.Deserialize<Dictionary<string, string>>(Path.GetFullPath(args[0]));
            Console.WriteLine("Loading template: {0}", args[1]);
            Templates.SplitTemplate template = ProjectFunctions.openTemplateFile(Path.GetFullPath(args[1]), true);
            string iniFolder = Path.Combine(Environment.CurrentDirectory, template.GameInfo.DataFolder);
            // Model extension crap
            string modelext = ".sa1mdl";
            if (template.GameInfo.GameName.Contains("SA2"))
            {
                if (template.GameInfo.GameName.Contains("PC") || template.GameInfo.GameName.Contains("SA2B"))
                    modelext = ".sa2bmdl";
                else
                    modelext = ".sa2bmdl";
            }
            foreach (Templates.SplitEntry splitData in template.SplitEntries)
            {
                Console.WriteLine("Split entry: {0}", splitData.IniFile);
                string inipath = Path.Combine(iniFolder, (splitData.IniFile.ToLower() + ".ini"));
                switch (Path.GetExtension(splitData.SourceFile).ToLowerInvariant())
                {
                    // SplitBinary
                    case ".exe":
                    case ".bin":
                    case ".prs":
                    case ".rel":
                        IniData inifile = IniSerializer.Deserialize<IniData>(inipath);
                        Dictionary<string, SplitTools.FileInfo> result = new Dictionary<string, SplitTools.FileInfo>();
                        foreach (KeyValuePair<string, SplitTools.FileInfo> item in new List<KeyValuePair<string, SplitTools.FileInfo>>(inifile.Files))
                        {
                            switch (item.Value.Type)
                            {
                                case "model":
                                case "basicmodel":
                                case "basicdxmodel":
                                case "chunkmodel":
                                case "gcmodel":
                                    if (metadata.ContainsKey(NormalizePath(item.Value.Filename)))
                                    {
                                        string[] meta = metadata[NormalizePath(item.Value.Filename)].Split('|');
                                        if (!result.ContainsKey(meta[0]))
                                        {
                                            // Texture
                                            if (meta.Length > 1 && meta[1] != "")
                                            {
                                                // PVM/Texlist INI name
                                                item.Value.CustomProperties["texture"] = meta[1];
                                                // TexIDs and texture names
                                                if (meta.Length > 2)
                                                {
                                                    // If texture is a texlist INI
                                                    if (Path.GetExtension(meta[1].ToLowerInvariant()) == ".ini")
                                                        item.Value.CustomProperties["texnames"] = meta[2];
                                                    else
                                                        item.Value.CustomProperties["texids"] = meta[2];
                                                }
                                            }
                                            result.Add(meta[0], item.Value);
                                            Console.WriteLine(item.Value.Filename + ": {0}", meta[0]);
                                        }
                                        else
                                            Console.WriteLine("Duplicate entry: {0}", meta[0]);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Item not found: {0} in {1}", item.Value.Filename, splitData.IniFile);
                                        result.Add(item.Key, item.Value);
                                    }
                                    break;
                                default:
                                    result.Add(item.Key, item.Value);
                                    break;
                            }
                        }
                        inifile.Files = result;
                        IniSerializer.Serialize(inifile, inipath);
                        break;
                    // SplitDLL
                    case ".dll":
                        IniDataSplitDLL inidll = IniSerializer.Deserialize<IniDataSplitDLL>(inipath);
                        Dictionary<string, SplitTools.SplitDLL.FileInfo> resultdll = new Dictionary<string, SplitTools.SplitDLL.FileInfo>();
                        foreach (KeyValuePair<string, SplitTools.SplitDLL.FileInfo> item in inidll.Files)
                        {
                            switch (item.Value.Type)
                            {
                                // Single model
                                case "model":
                                case "object":
                                case "basicmodel":
                                case "basicdxmodel":
                                case "chunkmodel":
                                case "gcmodel":
                                case "morph":
                                case "attach":
                                case "basicattach":
                                case "basicdxattach":
                                case "chunkattach":
                                case "gcattach":
                                    if (metadata.ContainsKey(NormalizePath(item.Value.Filename)))
                                    {
                                        string[] meta = metadata[NormalizePath(item.Value.Filename)].Split('|');
                                        if (!resultdll.ContainsKey(meta[0]))
                                        {
                                            resultdll.Add(meta[0], item.Value);
                                            Console.WriteLine(item.Value.Filename + ": {0}", meta[0]);
                                        }
                                        else
                                            Console.WriteLine("Duplicate entry for {0} found in {1}", meta[0], splitData.IniFile);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Item not found: {0}", item.Value.Filename);
                                        resultdll.Add(item.Key, item.Value);
                                    }
                                    break;
                                // Model array
                                case "modelarray":
                                case "modelsarray":
                                case "attacharray":
                                case "basicattacharray":
                                case "basicdxattacharray":
                                case "chunkattacharray":
                                case "gcattacharray":
                                    for (int i = 0; i < item.Value.Length; i++)
                                    {
                                        if (!item.Value.CustomProperties.ContainsKey("filename" + i.ToString()))
                                            continue;
                                        string modelpath = Path.Combine(item.Value.Filename, item.Value.CustomProperties["filename" + i.ToString()]);
                                        if (metadata.ContainsKey(NormalizePath(modelpath)))
                                            item.Value.CustomProperties["meta" + i.ToString()] = metadata[NormalizePath(modelpath)];
                                    }
                                    resultdll.Add(item.Key, item.Value);
                                    break;
                                // Action array
                                case "actionarray":
                                    for (int i = 0; i < item.Value.Length; i++)
                                    {
                                        if (!item.Value.CustomProperties.ContainsKey("filename" + i.ToString() + "_m"))
                                            continue;
                                        string modelpath = Path.Combine(item.Value.Filename, item.Value.CustomProperties["filename" + i.ToString() + "_m"] + modelext);
                                        if (metadata.ContainsKey(NormalizePath(modelpath)))
                                        {
                                            item.Value.CustomProperties["meta" + i.ToString() + "_m"] = metadata[NormalizePath(modelpath)];
                                            Console.WriteLine(modelpath + ": {0}", item.Value.CustomProperties["meta" + i.ToString() + "_m"]);
                                        }
                                    }
                                    resultdll.Add(item.Key, item.Value);
                                    break;
                                default:
                                    resultdll.Add(item.Key, item.Value);
                                    break;
                            }
                        }
                        inidll.Files = resultdll;
                        IniSerializer.Serialize(inidll, inipath);
                        break;
                    // SplitNB
                    case ".nb":
                        Dictionary<int, string> nbFilenames = IniSerializer.Deserialize<Dictionary<int, string>>(inipath);
                        Dictionary<int, string> resultNB = new Dictionary<int, string>();
                        foreach (var nbitem in nbFilenames)
                        {
                            string[] nbMeta = nbitem.Value.Split('|');
                            // 0 is path without extension, 1 is name, 2 is texture
                            if (metadata.ContainsKey(NormalizePath(nbMeta[0] + modelext)))
                            {
                                List<string> resultNBWithName = new List<string>();
                                resultNBWithName.Add(NormalizePath(nbMeta[0]));
                                resultNBWithName.Add(metadata[NormalizePath(nbMeta[0] + modelext)]);
                                resultNB.Add(nbitem.Key, string.Join("|", resultNBWithName));
                                Console.WriteLine("{0}:{1}", nbMeta[0] + modelext, resultNBWithName[1]);
                            }
                            else
                            {
                                Console.WriteLine("Item not found: {0}", nbMeta[0] + modelext);
                                resultNB.Add(nbitem.Key, NormalizePath(nbMeta[0]));
                            }
                        }
                        IniSerializer.Serialize(resultNB, inipath);
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("Output file: {0}", Path.GetFullPath(args[1]));
            
        }

        static string NormalizePath(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToLowerInvariant();
        }
    }
}