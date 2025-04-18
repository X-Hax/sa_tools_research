﻿using System;
using System.Collections.Generic;
using System.IO;
using SAModel;

namespace CompareTool
{
    public static partial class Program
    {
        static bool saveDiffFile = false; // Output code file if true
        static bool overwriteDiffFile = true; // Diff list file is overwritten if true
        static int maxDifferences = 100; // Maximum number of differences to log before the model is treated as too different
        static Dictionary<int, List<ModelDiffData>> modelDiffList = new(); // Dictionary of model differences
        static string outputDiffFilename = "result.c"; // Output code filename
        static string dllHandle = ""; // Name of DLL handle to use for addresses in code file

        static void Main(string[] args)
        {
            ProgramMode programMode = ProgramMode.Help; // Program mode set from command line

            if (args.Length > 0)
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "-f":
                    case "-folder":
                        programMode = ProgramMode.CompareFolder;
                        break;
                    case "-s":
                    case "-sort":
                        programMode = ProgramMode.SortModel;
                        break;
                    case "-c":
                    case "-comp":
                    case "-compare":
                        programMode = ProgramMode.CompareFile;
                        break;
                }
                for (int a = 0; a < args.Length; a++)
                {
                    if (args[a] == "-d" || args[a] == "-diff")
                        saveDiffFile = true;
                    if (args[a] == "-o" || args[a] == "-overwrite")
                        overwriteDiffFile = false;
                    if (args[a] == "-n" || args[a] == "-name")
                        outputDiffFilename = Path.GetFullPath(args[a + 1]);
                    if (args[a] == "-m" || args[a] == "-max")
                        maxDifferences = int.Parse(args[a + 1]);
                    if (args[a] == "-h" || args[a] == "-handle")
                        dllHandle = args[a + 1];
                }
            }
            switch (programMode)
            {
                case ProgramMode.Help:
                    ShowHelp();
                    return;
                case ProgramMode.SortModel:
                    string[] filenames = File.ReadAllLines(args[1]);
                    for (int f = 0; f < filenames.Length; f++)
                    {
                        Console.WriteLine("Sorting file {0}", filenames[f]);
                        ModelFile model = new ModelFile(filenames[f]);
                        SortModel(model.Model, true);
                        model.SaveToFile(filenames[f]);
                    }
                    return;
                case ProgramMode.CompareFolder:
                    Console.WriteLine("Folder mode");
                    CompareFolders(args);
                    return;
                case ProgramMode.CompareFile:
                    CompareFiles(args);
                    return;
            }
        }

        static void CompareFiles(string[] args)
        {
            string filename_src = Path.GetFullPath(args[1]); // File for comparison 1
            string filename_dst = Path.GetFullPath(args[2]); // File for comparison 2

            if (!File.Exists(filename_src) || !File.Exists(filename_dst))
            {
                Console.WriteLine("File {0} or {1} doesn't exist.", filename_src, filename_dst);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Source file: {0}", filename_src);
            Console.WriteLine("Destination file: {0}", filename_dst);
            if (saveDiffFile)
            {
                Console.Write("Output file: {0}, ", outputDiffFilename);
                Console.Write("mode: " + (overwriteDiffFile ? "Overwrite" : "Append") + "\n");
            }

            string ext = Path.GetExtension(filename_src).ToLowerInvariant();

            switch (ext)
            {
                case ".sa1lvl":
                    LandTable land_src = LandTable.LoadFromFile(filename_src);
                    LandTable land_dst = LandTable.LoadFromFile(filename_dst);
                    COL[] arr_src = land_src.COL.ToArray();
                    COL[] arr_dst = land_dst.COL.ToArray();
                    int addr = int.Parse(land_src.COLName.Replace("collist_", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
                    // Assume the same COL item order, otherwise it's fucked
                    if (arr_dst.Length != arr_src.Length)
                        Console.WriteLine("COL count different: {0} vs {1}", arr_src.Length, arr_dst.Length);
                    // Compare COL items
                    List<ModelDiffData> col = new();
                    maxDifferences = 90000;
                    for (int c = 0; c < arr_dst.Length; c++)
                    {
                        int c2 = FindCompareCOL(arr_dst[c], arr_src);
                        if (c2 != -1)
                            CompareCOL(arr_src[c2], arr_dst[c], c2, col);
                        else
                            Console.WriteLine("\tCould not match COL item {0} in the destination landtable", c);
                    }
                    AddDiffItems(addr, col);
                    // Compare models
                    for (int c = 0; c < Math.Min(arr_dst.Length, arr_src.Length); c++)
                    {
                        if (arr_src[c].Model.Attach != null)
                        {
                            int c2 = FindCompareCOL(arr_dst[c], arr_src);
                            if (c2 != -1)
                            {
                                bool result = CompareModel(arr_src[c2].Model, arr_dst[c].Model);
                                if (result == false)
                                    Console.WriteLine("COL item {0} model is identical", c.ToString());
                                else
                                    Console.WriteLine("\tCOL item {0} model is different", c.ToString());
                            }
                            else
                                Console.WriteLine("\tCould not match COL item {0} in the destination landtable", c);
                        }
                    }
                    // Save differences
                    if (saveDiffFile)
                        SerializeDiffList(outputDiffFilename);
                    /*
                    // Compare using different order
                    else
                    {
                       
                        Dictionary<int, int> matches = new Dictionary<int, int>();
                        for (int c1 = 0; c1 < arr_dst.Length; c1++)
                        {
                            bool found = false;
                            for (int c2 = 0; c2 < arr_dst.Length; c2++)
                                if (arr_src[c1].Model.Attach != null && CompareCOL(arr_src[c1], arr_dst[c2], c1, addr, 0))
                                    if (!matches.ContainsKey(c2) && !matches.ContainsValue(c1))
                                    {
                                        matches.Add(c2, c1);
                                        found = true;
                                        Console.WriteLine("COL item {0} matched with {1}", c1, c2);
                                        CompareAttach((BasicAttach)arr_src[c1].Model.Attach, (BasicAttach)arr_dst[c2].Model.Attach);
                                    }
                            // Try again but less strict
                            if (!found)
                            {
                                for (int c2 = 0; c2 < arr_dst.Length; c2++)
                                    if (arr_src[c1].Model.Attach != null && CompareCOL(arr_src[c1], arr_dst[c2], c1, addr, 1))
                                        if (!matches.ContainsKey(c2) && !matches.ContainsValue(c1))
                                        {
                                            matches.Add(c2, c1);
                                            Console.WriteLine("COL item {0} partially matched with {1}", c1, c2);
                                            CompareAttach((BasicAttach)arr_src[c1].Model.Attach, (BasicAttach)arr_dst[c2].Model.Attach);
                                        }
                            }
                        }
                    Console.WriteLine("Total COL items in landtables: {0} vs {1}, matches: {2}", arr_src.Length, arr_dst.Length, matches.Count);
                    */
                    break;
                case ".sa1mdl":
                    NJS_OBJECT mdl_src = new ModelFile(filename_src).Model;
                    NJS_OBJECT mdl_dst = new ModelFile(filename_dst).Model;
                    CompareModel(mdl_src, mdl_dst);
                    if (saveDiffFile)
                        SerializeDiffList(outputDiffFilename);
                    break;
                default:
                    break;
            }
        }

        static void SortModel(NJS_OBJECT mdl, bool withchildren)
        {
            if (mdl.Attach != null)
            {
                if (!(mdl.Attach is BasicAttach))
                {
                    Console.WriteLine("This operation is only supported for basic models.");
                    return;
                }
                BasicAttach basicatt = (BasicAttach)mdl.Attach;
                List<NJS_MESHSET> mesh_opaque = new List<NJS_MESHSET>();
                List<NJS_MESHSET> mesh_trans = new List<NJS_MESHSET>();
                List<NJS_MATERIAL> mats_opaque = new List<NJS_MATERIAL>();
                List<NJS_MATERIAL> mats_trans = new List<NJS_MATERIAL>();
                ushort matid_current = 0;
                List<ushort> matids = new();
                // Opaque meshes
                foreach (NJS_MESHSET m in basicatt.Mesh)
                {
                    if (!basicatt.Material[m.MaterialID].UseAlpha)
                    {
                        mesh_opaque.Add(m);
                        if (!mats_opaque.Contains(basicatt.Material[m.MaterialID]))
                        {
                            mats_opaque.Add(basicatt.Material[m.MaterialID]);
                            matids.Add(matid_current);
                            matid_current++;
                        }
                        else
                            matids.Add((ushort)mats_opaque.IndexOf(basicatt.Material[m.MaterialID]));
                    }
                }
                // Transparent meshes
                foreach (NJS_MESHSET m in basicatt.Mesh)
                {
                    if (basicatt.Material[m.MaterialID].UseAlpha)
                    {
                        mesh_trans.Add(m);
                        if (!mats_trans.Contains(basicatt.Material[m.MaterialID]))
                        {
                            mats_trans.Add(basicatt.Material[m.MaterialID]);
                            matids.Add(matid_current);
                            matid_current++;
                        }
                        else
                            matids.Add((ushort)(mats_opaque.Count + mats_trans.IndexOf(basicatt.Material[m.MaterialID])));
                    }
                }
                mesh_opaque.AddRange(mesh_trans);
                mats_opaque.AddRange(mats_trans);
                ushort matid_new = 0;
                foreach (NJS_MESHSET m in mesh_opaque)
                {
                    m.MaterialID = matids[matid_new];
                    matid_new++;
                }
                BasicAttach basicatt_new = new BasicAttach(basicatt.Vertex, basicatt.Normal, mesh_opaque, mats_opaque);
                basicatt_new.Bounds.Center = basicatt.Bounds.Center;
                basicatt_new.Bounds.Radius = basicatt.Bounds.Radius;
                basicatt_new.MaterialName = basicatt.MaterialName;
                basicatt_new.MeshName = basicatt.MeshName;
                basicatt_new.NormalName = basicatt.NormalName;
                basicatt_new.VertexName = basicatt.VertexName;
                basicatt_new.Name = basicatt.Name;
                mdl.Attach = basicatt_new;
            }
            if (withchildren)
                foreach (NJS_OBJECT child in mdl.Children)
                    SortModel(child, true);
        }

        static bool CompareFloat(float f1, float f2)
        {
            if (f1 == f2) 
                return false;
            else if (f1 > f2) 
                return Math.Abs(f1 - f2) > 0.05f;
            else 
                return Math.Abs(f2 - f1) > 0.05f;
        }

        static bool CompareVector(Vertex v1, Vertex v2)
        {
            if (CompareFloat(v1.X, v2.X))
                return true;
            if (CompareFloat(v1.Y, v2.Y))
                return true;
            if (CompareFloat(v1.Z, v2.Z))
                return true;
            return false;
        }

        static bool CompareInt(int i1, int i2)
        {
            if (i1 == i2)
                return false;
            else if (i1 > i2)
                return Math.Abs(i1 - i2) > 32;
            else
                return Math.Abs(i2 - i1) > 32;
        }

        static bool CompareRotation(Rotation r1, Rotation r2)
        {
            if (CompareInt(r1.X, r2.X))
                return true;
            if (CompareInt(r1.Y, r2.Y))
                return true;
            if (CompareInt(r1.Z, r2.Z))
                return true;
            return false;
        }

        static public bool CompareMotion(NJS_MOTION mot_src, NJS_MOTION mot_dest, bool verbose = true)
        {
            bool result = false;
            // Model parts
            if (mot_src.ModelParts != mot_dest.ModelParts)
            {
                if (verbose) Console.WriteLine("Model part mismatch: {0} vs {1}", mot_src.ModelParts, mot_dest.ModelParts);
                return true;
            }
            if (mot_src.Frames != mot_dest.Frames)
            {
                if (verbose) Console.WriteLine("Frame number mismatch: {0} vs {1}", mot_src.Frames, mot_dest.Frames);
                return true;
            }
            if (mot_src.InterpolationMode != mot_dest.InterpolationMode)
            {
                if (verbose) Console.WriteLine("Interpolation mode different: {0} vs {1}", mot_src.InterpolationMode.ToString(), mot_dest.InterpolationMode.ToString());
                return true;
            }
            if (mot_src.Models.Count != mot_dest.Models.Count)
            {
                if (verbose) Console.WriteLine("Count mismatch: {0} vs {1}", mot_src.Models.Count, mot_dest.Models.Count);
                return true;
            }
            foreach (var anim in mot_src.Models)
            {
                if (!mot_dest.Models.ContainsKey(anim.Key))
                {
                    if (verbose) Console.WriteLine("Dst doesn't have key");
                    return true;
                }
                if (mot_dest.Models[anim.Key] == null && anim.Value != null)
                {
                    if (verbose) Console.WriteLine("Key mismatch: dest is null");
                    return true;
                }
                if (mot_dest.Models[anim.Key] != null && anim.Value == null)
                {
                    if (verbose) Console.WriteLine("Key mismatch: src is null");
                    return true;
                }
                if (anim.Value.Position != null && anim.Value.Position.Count > 0)
                {
                    if (mot_dest.Models[anim.Key].Position == null || mot_dest.Models[anim.Key].Position.Count != anim.Value.Position.Count)
                        return true;
                    foreach (var item in anim.Value.Position)
                    {
                        if (!mot_dest.Models[anim.Key].Position.ContainsKey(item.Key))
                            return true;
                        if (CompareVector(mot_dest.Models[anim.Key].Position[item.Key], item.Value))
                            return true;
                    }
                }
                if (anim.Value.Rotation != null && anim.Value.Rotation.Count > 0)
                {
                    if (mot_dest.Models[anim.Key].Rotation == null || mot_dest.Models[anim.Key].Rotation.Count != anim.Value.Rotation.Count)
                        return true;
                    foreach (var item in anim.Value.Rotation)
                    {
                        if (!mot_dest.Models[anim.Key].Rotation.ContainsKey(item.Key))
                            return true;
                        if (CompareRotation(mot_dest.Models[anim.Key].Rotation[item.Key], item.Value))
                            return true;
                    }
                }
                if (anim.Value.Scale != null && anim.Value.Scale.Count > 0)
                {
                    if (mot_dest.Models[anim.Key].Scale == null || mot_dest.Models[anim.Key].Scale.Count != anim.Value.Scale.Count)
                        return true;
                    foreach (var item in anim.Value.Scale)
                    {
                        if (!mot_dest.Models[anim.Key].Scale.ContainsKey(item.Key))
                            return true;
                        if (CompareVector(mot_dest.Models[anim.Key].Scale[item.Key], item.Value))
                            return true;
                    }
                }
                if (anim.Value.Vertex != null && anim.Value.Vertex.Count > 0)
                {
                    if (mot_dest.Models[anim.Key].Vertex == null || mot_dest.Models[anim.Key].Vertex.Count != anim.Value.Vertex.Count)
                        return true;
                    foreach (var item in anim.Value.Vertex)
                    {
                        if (!mot_dest.Models[anim.Key].Vertex.ContainsKey(item.Key))
                            return true;
                        for (int v = 0; v < anim.Value.Vertex.Count; v++)
                        {
                            if (CompareVector(mot_dest.Models[anim.Key].Vertex[item.Key][v], item.Value[v]))
                                return true;
                        }
                    }
                }
                if (anim.Value.Normal != null && anim.Value.Normal.Count > 0)
                {
                    if (mot_dest.Models[anim.Key].Normal == null || mot_dest.Models[anim.Key].Normal.Count != anim.Value.Normal.Count)
                        return true;
                    foreach (var item in anim.Value.Normal)
                    {
                        if (!mot_dest.Models[anim.Key].Normal.ContainsKey(item.Key))
                            return true;
                        for (int v = 0; v < anim.Value.Normal.Count; v++)
                        {
                            if (CompareVector(mot_dest.Models[anim.Key].Normal[item.Key][v], item.Value[v]))
                                return true;
                        }
                    }
                }
            }
            return result;
        }

        public static bool CompareModel(NJS_OBJECT mdl_src, NJS_OBJECT mdl_dst, bool notroot = false, bool verbose = true)
        {
            //Console.WriteLine("Comparing {0} with {1}", mdl_src.Name, mdl_dst.Name);
            bool result = false;
            try
            {
                if ((mdl_src.Attach != null && mdl_dst.Attach == null) || (mdl_src.Attach == null && mdl_dst.Attach != null))
                {
                    if (verbose) Console.WriteLine("Model data is missing in one object but present in another");
                    return true;
                }
                if (mdl_src.GetObjects().Length > 1 || mdl_dst.GetObjects().Length > 1)
                {
                    if (mdl_src.GetObjects().Length != mdl_dst.GetObjects().Length)
                    {
                        if (verbose) Console.WriteLine("Model hierarchy is different");
                        return true;
                    }
                }
                int flags_src = (int)mdl_src.GetFlags();
                int flags_dst = (int)mdl_dst.GetFlags();
                if (flags_src != flags_dst)
                {
                    if (verbose) Console.WriteLine("Evalflags are different: {0} vs {1}", flags_src.ToString("X8"), flags_dst.ToString("X8"));
                    if (verbose) Console.WriteLine("Evalflags are different: {0} vs {1}", mdl_src.GetFlags().ToString(), mdl_dst.GetFlags().ToString());
                    result = true;
                }
                if (mdl_src.Attach != null)
                    if (CompareAttach((BasicAttach)mdl_src.Attach, (BasicAttach)mdl_dst.Attach, verbose))
                        result = true;
                if (!notroot)
                {
                    NJS_OBJECT[] objs_src = mdl_src.GetObjects();
                    NJS_OBJECT[] objs_dst = mdl_dst.GetObjects();
                    for (int id = 0; id < objs_src.Length; id++)
                    {
                        if (objs_src[id].Attach != null)
                            if (CompareModel(objs_src[id], objs_dst[id], true, verbose))
                                result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (verbose) Console.WriteLine(ex.ToString());
                result = true;
            }
            return result;
        }

        static void CompareFolders(string[] args)
        {
            bool handleWritten = false;
            if (File.Exists(outputDiffFilename))
                File.Delete(outputDiffFilename);
            List<string> results = new List<string>();
            string folder1 = args[1];
            string folder2 = args[2];
            string[] files1 = System.IO.Directory.GetFiles(folder1, "*.sa1mdl", SearchOption.AllDirectories);
            string[] files2 = System.IO.Directory.GetFiles(folder2, "*.sa1mdl", SearchOption.AllDirectories);
            overwriteDiffFile = false;
            for (int u = 0; u < files1.Length; u++)
            {
                bool found = false;
                for (int i = 0; i < files2.Length; i++)
                {
                    if (Path.GetFileNameWithoutExtension(files1[u]) == Path.GetFileNameWithoutExtension(files2[i]))
                    {
                        found = true;
                        modelDiffList = new();
                        if (CompareModel(new ModelFile(files1[u]).Model, new ModelFile(files2[i]).Model, verbose: true))
                        {
                            Console.WriteLine("\tModel is different: {0}\n", files1[u]);
                            results.Add(files1[u]);
                            TextWriter outz = File.AppendText(outputDiffFilename);
                            if (dllHandle != "" && !handleWritten)
                            {
                                outz.WriteLine("HMODULE handle{0} = GetModuleHandle(L\"{0}\");\n", dllHandle.ToUpperInvariant());
                                handleWritten = true;
                            }
                            outz.WriteLine("// {0}", files1[u]);
                            if (saveDiffFile)
                                WriteSingleItemDiffs(outz);
                            outz.Flush();
                            outz.Close();
                        }
                        else
                            Console.WriteLine("Model is identical: {0}", files1[u]);
                        break;
                    }
                }
                if (!found)
                    Console.WriteLine("\tModel not found: {0}", files1[u]);
            }
            TextWriter tw = File.CreateText("result.txt");
            foreach (var item in results)
            {
                tw.WriteLine(item);
            }
            tw.Flush();
            tw.Close();
        }

        public static bool CompareAttach(BasicAttach att_src, BasicAttach att_dst, bool verbose = true)
        {
            bool dontadd = false; // Don't write out differences (e.g. if array counts are different)
            int addr = 0; // Address of an item to be replaced
            List<ModelDiffData> items; // List of differences for materials, UVs 
            bool result = false;

            //Console.WriteLine("Comparing attach {0} with {1}", att_src.Name, att_dst.Name);
            // Compare materials
            if (att_src.Material.Count != att_dst.Material.Count)
            {
                if (verbose) Console.WriteLine("Material count different: {0} vs {1}", att_src.Material.Count, att_dst.Material.Count);
                result = true;
                dontadd = true;
            }
            else if (att_src.Material.Count > 0)
            {
                items = new();
                addr = int.Parse(att_src.MaterialName.Replace("matlist_", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
                for (int m = 0; m < att_src.Material.Count; m++)
                {
                    NJS_MATERIAL[] mat_src = att_src.Material.ToArray();
                    NJS_MATERIAL[] mat_dst = att_dst.Material.ToArray();
                    if (m >= mat_dst.Length) break;
                    if (mat_src[m].TextureID != mat_dst[m].TextureID)
                    {
                        if (verbose) Console.WriteLine("Different texture IDs in material {0}: {1} vs {2}", m, mat_src[m].TextureID, mat_dst[m].TextureID, mat_src[m].Flags.ToString("X8"), mat_dst[m].Flags.ToString("X8"));
                        items.Add(new MaterialTextureDiffData { ArrayID = m, TexID = mat_dst[m].TextureID });
                        result = true;
                    }
                    if (mat_src[m].Flags != mat_dst[m].Flags)
                    {
                        if (verbose) Console.WriteLine("Different flags in material {0}: {1} vs {2}", m, mat_src[m].Flags.ToString("X8"), mat_dst[m].Flags.ToString("X8"));
                        items.Add(new MaterialFlagsDiffData { ArrayID = m, Flags = mat_dst[m].Flags });
                        result = true;
                    }
                    if (mat_src[m].Exponent != mat_dst[m].Exponent)
                    {
                        if (verbose) Console.WriteLine("Different exponent in material {0}: {1} vs {2}", m, mat_src[m].Exponent, mat_dst[m].Exponent);
                        items.Add(new MaterialExponentDiffData { ArrayID = m, Exponent = mat_dst[m].Exponent });
                        result = true;
                    }
                    if (mat_src[m].DiffuseColor != mat_dst[m].DiffuseColor)
                    {
                        if (verbose) Console.WriteLine("Different diffuse color for material {0}: {1} vs {2}", m, mat_src[m].DiffuseColor.ToArgb().ToString("X"), mat_dst[m].DiffuseColor.ToArgb().ToString("X"));
                        items.Add(new DiffuseColorDiffData { ArrayID = m, A = mat_dst[m].DiffuseColor.A, R = mat_dst[m].DiffuseColor.R, G = mat_dst[m].DiffuseColor.G, B = mat_dst[m].DiffuseColor.B });
                        result = true;
                    }
                    if (mat_src[m].SpecularColor != mat_dst[m].SpecularColor)
                    {
                        if (verbose) Console.WriteLine("Different specular color for material {0}: {1} vs {2}", m, mat_src[m].SpecularColor.ToArgb().ToString("X"), mat_dst[m].SpecularColor.ToArgb().ToString("X"));
                        items.Add(new SpecularColorDiffData { ArrayID = m, A = mat_dst[m].SpecularColor.A, R = mat_dst[m].SpecularColor.R, G = mat_dst[m].SpecularColor.G, B = mat_dst[m].SpecularColor.B });
                        result = true;
                    }
                }
                if (!dontadd)
                    AddDiffItems(addr, items);
            }

            // Compare vertices
            if (att_src.Vertex.Length != att_dst.Vertex.Length)
            {
                if (verbose) Console.WriteLine("Vertex count different: {0} vs {1}", att_src.Vertex.Length, att_dst.Vertex.Length);
                dontadd = true;
                result = true;
            }
            else
            {
                items = new();
                addr = int.Parse(att_src.VertexName.Replace("vertex_", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
                for (int m = 0; m < att_src.Vertex.Length; m++)
                {
                    if (m >= att_dst.Vertex.Length) break;
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    if (Math.Abs(att_src.Vertex[m].X - att_dst.Vertex[m].X) > 0.01f)
                        x = true;
                    if (Math.Abs(att_src.Vertex[m].Y - att_dst.Vertex[m].Y) > 0.01f)
                        y = true;
                    if (Math.Abs(att_src.Vertex[m].Z - att_dst.Vertex[m].Z) > 0.01f)
                        z = true;
                    if (x || y || z)
                    {
                        if (verbose) Console.WriteLine("Different vertex {0}: {1} vs {2}", m, att_src.Vertex[m], att_dst.Vertex[m]);
                        if (!dontadd)
                            items.Add(new VertexNormalDiffData { ArrayID = m, X = att_dst.Vertex[m].X, Y = att_dst.Vertex[m].Y, Z = att_dst.Vertex[m].Z });
                        result = true;
                    }
                }
                if (!dontadd)
                    AddDiffItems(addr, items);
            }

            // Compare normals
            if (att_src.Normal.Length != att_dst.Normal.Length)
            {
                dontadd = true;
                if (verbose) Console.WriteLine("Normal count different: {0} vs {1}", att_src.Normal.Length, att_dst.Normal.Length);
                result = true;
            }
            else
            {
                items = new();
                addr = int.Parse(att_src.NormalName.Replace("normal_", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
                for (int m = 0; m < att_src.Normal.Length; m++)
                {
                    if (m >= att_dst.Normal.Length) break;
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    if (Math.Abs(att_src.Normal[m].X - att_dst.Normal[m].X) > 0.01f)
                        x = true;
                    if (Math.Abs(att_src.Normal[m].Y - att_dst.Normal[m].Y) > 0.01f)
                        y = true;
                    if (Math.Abs(att_src.Normal[m].Z - att_dst.Normal[m].Z) > 0.01f)
                        z = true;
                    if (x || y || z)
                    {
                        if (verbose) Console.WriteLine("Different normal {0}: {1} vs {2}", m, att_src.Normal[m], att_dst.Normal[m]);
                        if (!dontadd)
                            items.Add(new VertexNormalDiffData { ArrayID = m, X = att_dst.Normal[m].X, Y = att_dst.Normal[m].Y, Z = att_dst.Normal[m].Z });
                        result = true;
                    }
                }
                if (!dontadd)
                    AddDiffItems(addr, items);
            }

            // Compare meshsets
            if (att_src.Mesh.Count != att_dst.Mesh.Count)
            {
                if (verbose) Console.WriteLine("Mesh count different: {0} vs {1}", att_src.Mesh.Count, att_dst.Mesh.Count);
                dontadd = true;
                result = true;
            }
            else
            {
                bool stop = false;
                for (int u = 0; u < att_src.Mesh.Count; u++)
                {
                    // Compare attributes
                    if (att_src.Mesh[u].PAttr != att_dst.Mesh[u].PAttr)
                    {
                        if (verbose) Console.WriteLine("Attributes different for mesh {0}: {1} vs {2}", att_src.Mesh[u].PAttr.ToString("X"), att_dst.Mesh[u].PAttr.ToString("X"));
                        result = true;
                        dontadd = true;
                        stop = true;
                    }

                    // Check vertex colors
                    if ((att_src.Mesh[u].VColor == null && att_dst.Mesh[u].VColor != null) || (att_dst.Mesh[u].VColor == null && att_src.Mesh[u].VColor != null))
                    {
                        if (verbose) Console.WriteLine("Vertex color mismatch detected.");
                        result = true;
                        dontadd = true;
                        stop = true;
                    }

                    // Check UVs
                    if ((att_src.Mesh[u].UV == null && att_dst.Mesh[u].UV != null) || (att_dst.Mesh[u].UV == null && att_src.Mesh[u].UV != null))
                    {
                        if (verbose) Console.WriteLine("UV mismatch detected.");
                        result = true;
                        dontadd = true;
                        stop = true;
                    }

                    if (!stop)
                    {
                        // Compare polys
                        if (att_src.Mesh[u].Poly != null)
                        {
                            if (att_src.Mesh[u].Poly.Count != att_dst.Mesh[u].Poly.Count)
                            {
                                if (verbose) Console.WriteLine("Poly count different for mesh {0}: {1} vs {2}", u, att_src.Mesh[u].Poly.Count, att_dst.Mesh[u].Poly.Count);
                                result = true;
                                dontadd = true;
                            }
                            else
                            {
                                for (int v = 0; v < att_src.Mesh[u].Poly.Count; v++)
                                {
                                    if (v >= att_dst.Mesh[u].Poly.Count) break;
                                    if (att_src.Mesh[u].Poly[v].Indexes.Length != att_dst.Mesh[u].Poly[v].Indexes.Length)
                                    {
                                        if (verbose) Console.WriteLine("Poly index count different for mesh {0}: {1} vs {2}", u, att_src.Mesh[u].Poly[v].Indexes.Length, att_dst.Mesh[u].Poly[v].Indexes.Length);
                                        dontadd = true;
                                        result = true;
                                    }
                                    for (int i = 0; i < att_src.Mesh[u].Poly[v].Indexes.Length; i++)
                                    {
                                        if (i >= att_dst.Mesh[u].Poly[v].Indexes.Length) break;
                                        if (att_src.Mesh[u].Poly[v].Indexes[i] != att_dst.Mesh[u].Poly[v].Indexes[i])
                                        {
                                            if (verbose) Console.WriteLine("Mesh {0} poly {1} index {2} different: {3} vs {4}", u, v, i, att_src.Mesh[u].Poly[v].Indexes[i], att_dst.Mesh[u].Poly[v].Indexes[i]);
                                            result = true;
                                            dontadd = true;
                                        }
                                    }
                                }
                            }
                        }

                        // Compare vcolors
                        if (att_src.Mesh[u].VColor != null)
                        {
                            addr = int.Parse(att_src.Mesh[u].VColorName.Replace("vcolor_", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
                            if (att_src.Mesh[u].VColor.Length != att_dst.Mesh[u].VColor.Length)
                            {
                                if (verbose) Console.WriteLine("VColor count different for mesh {0}: {1} vs {2}", u, att_src.Mesh[u].VColor.Length, att_dst.Mesh[u].VColor.Length);
                                dontadd = true;
                                result = true;
                            }
                            else
                            {
                                items = new();
                                for (int v = 0; v < att_src.Mesh[u].VColor.Length; v++)
                                {
                                    if (v >= att_dst.Mesh[u].VColor.Length) break;
                                    if (att_src.Mesh[u].VColor[v].A != att_dst.Mesh[u].VColor[v].A || att_src.Mesh[u].VColor[v].R != att_dst.Mesh[u].VColor[v].R || att_src.Mesh[u].VColor[v].G != att_dst.Mesh[u].VColor[v].G || att_src.Mesh[u].VColor[v].B != att_dst.Mesh[u].VColor[v].B)
                                    {
                                        if (verbose) Console.WriteLine("VColor {0} different for mesh {1}: {2} vs {3}", v, u, att_src.Mesh[u].VColor[v], att_dst.Mesh[u].VColor[v]);
                                        if (!dontadd)
                                            items.Add(new ColorDiffData { ArrayID = v, A = att_dst.Mesh[u].VColor[v].A, R = att_dst.Mesh[u].VColor[v].R, G = att_dst.Mesh[u].VColor[v].G, B = att_dst.Mesh[u].VColor[v].B });
                                        result = true;
                                    }
                                }
                                if (!dontadd)
                                    AddDiffItems(addr, items);
                            }
                        }

                        // Compare UVs
                        if (att_src.Mesh[u].UV != null)
                        {
                            bool name = false;
                            addr = int.Parse(att_src.Mesh[u].UVName.Replace("uv_", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
                            if (att_src.Mesh[u].UV.Length != att_dst.Mesh[u].UV.Length)
                            {
                                if (verbose) Console.WriteLine("UV count different for mesh {0}: {1} vs {2}", u, att_src.Mesh[u].UV.Length, att_dst.Mesh[u].UV.Length);
                                dontadd = true;
                                result = true;
                            }
                            else
                            {
                                items = new();
                                for (int uvindex = 0; uvindex < att_src.Mesh[u].UV.Length; uvindex++)
                                {
                                    short src_U = (short)(att_src.Mesh[u].UV[uvindex].U * 255f);
                                    short src_V = (short)(att_src.Mesh[u].UV[uvindex].V * 255f);
                                    short dst_U = (short)(att_dst.Mesh[u].UV[uvindex].U * 255f);
                                    short dst_V = (short)(att_dst.Mesh[u].UV[uvindex].V * 255f);
                                    if (src_U != dst_U || src_V != dst_V)
                                    {
                                        if (!name)
                                        {
                                            name = true;
                                            if (verbose) Console.WriteLine("UV array {0} is different: Source U{1} V{2}, Destination U{3} V{4}", att_src.Mesh[u].UVName, src_U, src_V, dst_U, dst_V);
                                            result = true;
                                        }
                                        if (!dontadd)
                                            items.Add(new UVDiffData { ArrayID = uvindex, U = dst_U, V = dst_V });
                                        //Console.WriteLine("{0} : {1}, {2} is {3}, {4}", uvindex, src_U, src_V, dst_U, dst_V);
                                    }
                                }
                                if (!dontadd)
                                    AddDiffItems(addr, items);
                            }
                        }
                    }
                }
            }
            return result;
        }

        static void AddDiffItems(int addr, List<ModelDiffData> items)
        {
            if (modelDiffList.ContainsKey(addr))
            {
                //Console.WriteLine("Reused array at {0}", addr.ToString("X"));
            }
            else if (maxDifferences != 0 && items.Count >= maxDifferences)
            {
                //Console.WriteLine("Over {0} differences detected", maxDifferences);
            }
            else if (items.Count > 0)
            {
                //Console.WriteLine("Adding {0} items for address {1}", items.Count, addr.ToString("X"));
                modelDiffList.Add(addr, items);
            }
        }

       public static int FindCompareCOL(COL item, COL[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (item.Bounds.Radius != list[i].Bounds.Radius)
                    continue;
                if (item.Bounds.Center.X != list[i].Bounds.Center.X)
                    continue;
                if (item.Bounds.Center.Y != list[i].Bounds.Center.Y)
                    continue;
                if (item.Bounds.Center.Z != list[i].Bounds.Center.Z)
                    continue;
                if (item.Model.Attach.Bounds.Center.X != list[i].Model.Attach.Bounds.Center.X)
                    continue;
                if (item.Model.Attach.Bounds.Center.Y != list[i].Model.Attach.Bounds.Center.Y)
                    continue;
                if (item.Model.Attach.Bounds.Center.Z != list[i].Model.Attach.Bounds.Center.Z)
                    continue;
                if (item.Model.Attach.Bounds.Radius != list[i].Model.Attach.Bounds.Radius)
                    continue;
                BasicAttach att_src = (BasicAttach)list[i].Model.Attach;
                BasicAttach att_dst = (BasicAttach)item.Model.Attach;
                if (att_src.Vertex.Length != att_dst.Vertex.Length)
                    continue;
                if (att_src.Normal.Length != att_dst.Normal.Length)
                    continue;
                if (att_src.Mesh.Count != att_dst.Mesh.Count)
                    continue;
                //Console.WriteLine(i.ToString());
                return i;
            }
            return -1;
        }

        public static bool CompareCOL(COL item1, COL item2, int arrayID, List<ModelDiffData> items)
        {
            bool same = true;
            if (item1.Bounds.Center.X != item2.Bounds.Center.X ||
                item1.Bounds.Center.Y != item2.Bounds.Center.Y ||
                item1.Bounds.Center.Z != item2.Bounds.Center.Z)
            {
                same = false;
                Console.WriteLine("Bounds different for COL item {0} : {1} / {2} / {3} vs {4} / {5} / {6}", arrayID, item1.Bounds.Center.X, item1.Bounds.Center.Y, item1.Bounds.Center.Z, item2.Bounds.Center.X, item2.Bounds.Center.Y, item2.Bounds.Center.Z);
                //items.Add(new VertexNormalDiffData { ArrayID = arrayID, X = item2.Bounds.Center.X, Y = item2.Bounds.Center.Y, Z = item2.Bounds.Center.Z });
            }
            if (item1.Bounds.Radius != item2.Bounds.Radius ||
                item1.WidthY != item2.WidthY ||
                item1.WidthZ != item2.WidthZ)
            {
                same = false;
                Console.WriteLine("Radius different for COL item {0} : {1} / {2} / {3} vs {4} / {5} / {6}", arrayID, item1.Bounds.Center.X, item1.WidthY, item1.WidthZ, item2.Bounds.Radius, item2.WidthY, item2.WidthZ);
                //items.Add(new VertexNormalDiffData { ArrayID = arrayID, X = item2.Bounds.Radius, Y = item2.WidthY, Z = item2.WidthZ });
            }
            if (item1.Flags != item2.Flags)
            {
                same = false;
                Console.WriteLine("Flags different for COL item {0} : / {1} vs {2}", arrayID, item1.Flags.ToString("X"), item2.Flags.ToString("X"));
                items.Add(new ColFlagsDiffData { ArrayID = arrayID, flags = (uint)item2.Flags });
            }
            if (!same)
                Console.WriteLine("\tCOL item {0} is different", arrayID.ToString());
            return same;
        }

        static void WriteSingleItemDiffs(TextWriter tw)
        {
            if (modelDiffList.Count == 0 || modelDiffList.Count >= maxDifferences)
            {
                tw.WriteLine("\t//Model too different (num diff: {0})\n", modelDiffList.Count);
                return;
            }
            foreach (var item in modelDiffList)
            {
                foreach (var item2 in item.Value)
                {
                    // Write texids
                    if (item2 is MaterialTextureDiffData tex)
                        tw.WriteLine("((NJS_MATERIAL*){0})->attr_texId = {1};",
                            dllHandle == "" ? "0x" + (0x400000 + item.Key + NJS_MATERIAL.Size * tex.ArrayID).ToString("X8") :
                            "((size_t)handle" + dllHandle.ToUpperInvariant() + " + " + "0x" + (item.Key + NJS_MATERIAL.Size * tex.ArrayID).ToString("X8") + ")",
                            tex.TexID);

                    // Write material colors
                    else if (item2 is DiffuseColorDiffData clr)
                        tw.WriteLine("((NJS_MATERIAL*){0})->diffuse.color = 0x{1}{2}{3}{4};",
                             dllHandle == "" ? "0x" + (0x400000 + item.Key + NJS_MATERIAL.Size * clr.ArrayID).ToString("X8") :
                            "((size_t)handle" + dllHandle.ToUpperInvariant() + " + " + "0x" + (item.Key + 4 * clr.ArrayID).ToString("X8") + ")",
                            clr.A.ToString("X2"), clr.R.ToString("X2"), clr.G.ToString("X2"), clr.B.ToString("X2"));

                    // Write specular colors
                    else if (item2 is SpecularColorDiffData sclr)
                        tw.WriteLine("((NJS_MATERIAL*){0})->specular.color = 0x{1}{2}{3}{4};",
                             dllHandle == "" ? "0x" + (0x400000 + item.Key + NJS_MATERIAL.Size * sclr.ArrayID).ToString("X8") :
                            "((size_t)handle" + dllHandle.ToUpperInvariant() + " + " + "0x" + (item.Key + 4 * sclr.ArrayID).ToString("X8") + ")",
                            sclr.A.ToString("X2"), sclr.R.ToString("X2"), sclr.G.ToString("X2"), sclr.B.ToString("X2"));

                    // Write exponents
                    else if (item2 is MaterialExponentDiffData exp)
                        tw.WriteLine("((NJS_MATERIAL*){0})->exponent = {1};",
                             dllHandle == "" ? "0x" + (0x400000 + item.Key + NJS_MATERIAL.Size * exp.ArrayID).ToString("X8") :
                            "((size_t)handle" + dllHandle.ToUpperInvariant() + " + " + "0x" + (item.Key + NJS_MATERIAL.Size * exp.ArrayID).ToString("X8") + ")",
                            exp.Exponent);

                    // Write material flags
                    else if (item2 is MaterialFlagsDiffData flg)
                        tw.WriteLine("((NJS_MATERIAL*){0})->attrflags = 0x{1};",
                            dllHandle == "" ? "0x" + (0x400000 + item.Key + NJS_MATERIAL.Size * flg.ArrayID).ToString("X8") :
                            "((size_t)handle" + dllHandle.ToUpperInvariant() + " + " + "0x" + (item.Key + NJS_MATERIAL.Size * flg.ArrayID).ToString("X8")+")",
                            flg.Flags.ToString("X8"));

                    // Write UVs
                    else if (item2 is UVDiffData uvd)
                    {
                        tw.WriteLine("*(NJS_TEX*){0} = {{ {1}, {2} }};",
                            dllHandle == "" ? "0x" + (0x400000 + item.Key + UV.Size * uvd.ArrayID).ToString("X8") :
                            "((size_t)handle" + dllHandle.ToUpperInvariant() + " + " + "0x" + (item.Key + UV.Size * uvd.ArrayID).ToString("X8") + ")",
                            uvd.U, uvd.V);
                    }

                    // Write vertices/normals/COL position
                    else if (item2 is VertexNormalDiffData vtx)
                        tw.WriteLine("*(NJS_VECTOR*){0} = {{ {1}, {2}, {3} }};",
                             dllHandle == "" ? "0x" + (0x400000 + item.Key + Vertex.Size * vtx.ArrayID).ToString("X8") :
                            "((size_t)handle" + dllHandle.ToUpperInvariant() + " + " + "0x" + (item.Key + Vertex.Size * vtx.ArrayID).ToString("X8") + ")",
                            vtx.X.ToC(), vtx.Y.ToC(), vtx.Z.ToC());

                    // Write COL flags
                    else if (item2 is ColFlagsDiffData cfl)
                        tw.WriteLine("((_OBJ_LANDENTRY*){0})[{1}].slAttribute = {2};",
                 dllHandle == "" ? "0x" + (0x400000 + item.Key).ToString("X8") :
                "((size_t)handle" + dllHandle.ToUpperInvariant() + " + " + "0x" + (item.Key).ToString("X8") + ")", cfl.ArrayID,
                "0x" + cfl.flags.ToString("X8"));
                    /* Old one that writes to the COL item directly
                    tw.WriteLine("((_OBJ_LANDENTRY*){0}->slAttribute = {1};",
                         dllHandle == "" ? "0x" + (0x400000 + item.Key + 36 * cfl.ArrayID).ToString("X8") :
                        "((size_t)handle" + dllHandle.ToUpperInvariant() + " + " + "0x" + (item.Key + 36 * cfl.ArrayID).ToString("X8") + ")",
                        "0x"+cfl.flags.ToString("X8"));
                    */
                }
            }
            tw.WriteLine();
        }

        static void SerializeDiffList(string filename)
        {
            TextWriter tw = overwriteDiffFile ? File.CreateText(filename) : File.AppendText(filename);
            WriteSingleItemDiffs(tw);
            tw.Flush();
            tw.Close();
        }

        static void ShowHelp()
        {
            Console.WriteLine("This tool compares levels or models and outputs C code to patch the differences between them when possible.\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("Comparing two .sa1lvl or .sa1mdl files:");
            Console.WriteLine("\tCompareTool -c <file1> <file2> [-diff] [-overwrite] [-name <outFilename>] [-max <number>] [-h <handleName>]");
            Console.WriteLine("Comparing two folders with .sa1mdl files:");
            Console.WriteLine("\tCompareTool -f <folder1> <folder2> [-diff] [-name <outFilename>] [-max <number>] [-h <handleName>]");
            Console.WriteLine("Sorting transparent meshes in models (overwrites original models):");
            Console.WriteLine("\tCompareTool -s <listFilename>");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("\tfile1: Original level or model");
            Console.WriteLine("\tfile2: Level or model to compare against");
            Console.WriteLine("\tlistFilename: A text file containing a list of model paths");
            Console.WriteLine("\t-diff (-d): Output C code");
            Console.WriteLine("\t-overwrite (-o): Overwrite the C code file instead of appending to it (single file mode only)");
            Console.WriteLine("\t-name (-n): Output filename (default is `result.c`)");
            Console.WriteLine("\t-max (-m): Maximum number of differences for producing code (default is 100, 0 is unlimited)");
            Console.WriteLine("\t-handle (-h): DLL handle name for addresses in code");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("To compare `Level_PC.sa1lvl` to `Level_Gamecube.sa1lvl` and output a code patch to `resultLevel.c`:");
            Console.WriteLine("\tCompareTool -c Level_PC.sa1lvl Level_Gamecube.sa1lvl -diff -name resultLevel.c");
            Console.WriteLine("To compare assets in `sadx` and `sa1` folders and output a code patch for models in ADV00MODELS.DLL with < 50 changes:");
            Console.WriteLine("\tCompareTool -f sadx sa1 -diff -max 50 -h ADV00MODELS");
            Console.WriteLine();
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
            return;
        }

        private enum ProgramMode
        {
            Help,
            SortModel,
            CompareFile,
            CompareFolder
        }
    }
}