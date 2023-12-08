using Newtonsoft.Json;
using SAModel;
using SplitTools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

// The label structs have changed and this doesn't work properly anymore.
// Methods to retrieve and export labels
namespace LabelTool
{
    partial class Program
    {
        static void LabelSave(string[] args)
        {
            JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
            string datafilename = args[0];
            string extension = Path.GetExtension(datafilename).ToLowerInvariant();
            string outfilename = Path.ChangeExtension(datafilename, ".salabel");
            Console.WriteLine("Exporting labels from: {0}", datafilename);
            Console.WriteLine("Output filename: {0}", outfilename);
            switch (extension)
            {
                case ".sa1mdl":
                case ".sa2mdl":
                case ".sa2bmdl":
                    ModelFile mf = new ModelFile(datafilename);
                    NJS_OBJECT ob = mf.Model;
                    List<LabelOBJECT> result = new List<LabelOBJECT>();
                    NJS_OBJECT[] objs = ob.GetObjects();
                    for (int i = 0; i < objs.Length; i++)
                        result.Add(new(objs[i]));
                    using (TextWriter tw = File.CreateText(outfilename))
                        using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
                            js.Serialize(jtw, result);
                    break;
                case ".saanim":
                    NJS_MOTION mot = NJS_MOTION.Load(datafilename);
                    LabelMOTION resultl = new(mot);
                    using (TextWriter tw = File.CreateText(outfilename))
                    using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
                        js.Serialize(jtw, resultl);
                    break;
                case ".sa1lvl":
                case ".sa2lvl":
                    LandTable lnd = LandTable.LoadFromFile(datafilename);
                    LabelLANDTABLE resultlt = new(lnd);
                    using (TextWriter tw = File.CreateText(outfilename))
                    using (JsonTextWriter jtw = new JsonTextWriter(tw) { Formatting = Formatting.Indented })
                        js.Serialize(jtw, resultlt);
                    break;
            }
        }

        static void LabelLoad(string[] args)
        {
            JsonSerializer js = new JsonSerializer() { Culture = System.Globalization.CultureInfo.InvariantCulture };
            string datafilename = args[0];
            string extension = Path.GetExtension(datafilename).ToLowerInvariant();
            string labelfilename = Path.Combine(Path.GetDirectoryName(datafilename), Path.GetFileNameWithoutExtension(datafilename) + ".salabel");
            string outfilename = Path.Combine(Path.GetDirectoryName(datafilename), Path.GetFileNameWithoutExtension(datafilename) + "_l" + extension);
            Console.WriteLine("Loading labels for: {0}", datafilename);
            Console.WriteLine("Label file for: {0}", labelfilename);
            Console.WriteLine("Output filename: {0}", outfilename);
            TextReader tr = File.OpenText(labelfilename);
            JsonTextReader jtr = new JsonTextReader(tr);
            switch (extension)
            {
                case ".sa1mdl":
                case ".sa2mdl":
                case ".sa2bmdl":
                    ModelFile mf = new ModelFile(datafilename);
                    NJS_OBJECT ob = mf.Model;
                    List<LabelOBJECT> labels_m = js.Deserialize<List<LabelOBJECT>>(jtr);
                    NJS_OBJECT[] objs = ob.GetObjects();
                    for (int i = 0; i < objs.Length; i++)
                        labels_m[i].Apply(objs[i]);
                    ModelFile.CreateFile(outfilename, ob, null, mf.Author, mf.Description, mf.Metadata, mf.Format);
                    break;
                case ".saanim":
                    NJS_MOTION mot = NJS_MOTION.Load(datafilename);
                    LabelMOTION resultm = js.Deserialize<LabelMOTION>(jtr);
                    resultm.Apply(mot);
                    mot.Save(outfilename);
                    break;
                case ".sa1lvl":
                case ".sa2lvl":
                    LandTable lnd = LandTable.LoadFromFile(datafilename);
                    LabelLANDTABLE resultlt = js.Deserialize<LabelLANDTABLE>(jtr);
                    resultlt.Apply(lnd);
                    lnd.SaveToFile(outfilename, lnd.Format);
                    break;
            }
        }

        static void LabelTexlist(string[] args)
        {
            Dictionary <int,string> applabels= new Dictionary<int,string>();
            IniData ini = IniSerializer.Deserialize<IniData>(args[0]);
            applabels = IniSerializer.Deserialize<Dictionary<int, string>>(args[1]);
            Dictionary<string, SplitTools.FileInfo> newlist = new();
            foreach (var file in ini.Files)
            {
                if (file.Value.Type == "texlist")
                {
                    bool found = false;
                    string name = Path.GetFileName(file.Value.Filename).Replace(".tls.satex", "");
                    //Console.WriteLine(name);
                    foreach (var dict in applabels)
                    {
                        if (dict.Value.Contains("texlist") && dict.Value.Equals("texlist_" + name))
                        {
                            //Console.WriteLine(name + " / " + dict.Value);
                            newlist.Add(file.Key, new SplitTools.FileInfo { Address = dict.Key, Type = "texlist", Filename = file.Value.Filename });
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        foreach (var dict in applabels)
                        {
                            if (dict.Value.ToLowerInvariant().Contains("texlist") && dict.Value.ToLowerInvariant().Contains(name))
                            {
                                Console.WriteLine("Found second: " + name + " / " + dict.Value);
                                newlist.Add(file.Key, new SplitTools.FileInfo { Address = dict.Key, Type = "texlist", Filename = file.Value.Filename });
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        Console.WriteLine("Not found: {0}", file.Key);
                        newlist.Add(file.Key, new SplitTools.FileInfo { Address = 0x9999, Type = "texlist", Filename = file.Value.Filename });
                    }
                }
            }
            IniData newini = new IniData { Files = newlist };
            IniSerializer.Serialize(newini, "out.ini");
        }
    }
}