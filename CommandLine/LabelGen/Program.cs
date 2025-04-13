using System.Collections.Generic;
using System.IO;
using SAModel;
using SplitTools;

namespace LabelGen
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, string> output = new Dictionary<int, string>();
            NJS_OBJECT oldobj = new ModelFile(args[0]).Model;
            string[] foundmotions = Directory.GetFiles(Path.Combine(System.Environment.CurrentDirectory, "refs"), "*.saanim", SearchOption.TopDirectoryOnly);
            NJS_MOTION[] srcmot = new NJS_MOTION[foundmotions.Length];
            for (int m = 0; m < srcmot.Length; m++)
                srcmot[m] = NJS_MOTION.Load(foundmotions[m]);
            // Labels from Motions
            if (srcmot.Length > 0)
                GenerateLabels(srcmot, oldobj, Path.GetFileNameWithoutExtension(args[0]).Replace(".nja", ""), output);
            // Labels from filename
            else
                GenerateLabels(oldobj, "object_" + Path.GetFileNameWithoutExtension(args[0]).Replace(".nja", ""), output);
            ModelFile.CreateFile(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[0])), Path.GetFileNameWithoutExtension(args[0]) + "_rec.sa1mdl"), oldobj, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
            IniSerializer.Serialize(output, Path.ChangeExtension(args[0], ".txt"));
        }
    }      
}