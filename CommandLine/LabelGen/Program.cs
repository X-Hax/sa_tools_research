using System;
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
            string[] foundmotions;
            string[] foundobjects;
            if (args.Length > 1)
            {
                foundmotions = Directory.GetFiles(Path.Combine(System.Environment.CurrentDirectory, args[1]), "*.saanim", SearchOption.TopDirectoryOnly);
                foundobjects = Directory.GetFiles(Path.Combine(System.Environment.CurrentDirectory, args[1]), "*.sa*mdl", SearchOption.TopDirectoryOnly);
            }
            else
            {
                foundmotions = Directory.GetFiles(Path.Combine(System.Environment.CurrentDirectory, UpTo(Path.GetFileName(args[0]), ".")), "*.saanim", SearchOption.TopDirectoryOnly);
                foundobjects = Directory.GetFiles(Path.Combine(System.Environment.CurrentDirectory, UpTo(Path.GetFileName(args[0]), ".")), "*.sa*mdl", SearchOption.TopDirectoryOnly);
            }
            NJS_MOTION[] srcmot = new NJS_MOTION[foundmotions.Length];
            NJS_OBJECT[] srcobj = new NJS_OBJECT[foundobjects.Length];
            for (int m = 0; m < srcmot.Length; m++)
                srcmot[m] = NJS_MOTION.Load(foundmotions[m]);
            for (int m = 0; m < srcobj.Length; m++)
                srcobj[m] = new ModelFile(foundobjects[m]).Model;
            // Labels from Objects
            if (srcobj.Length > 0)
                GenerateLabels(srcobj, oldobj, output);
            // Labels from Motions
            else if (srcmot.Length > 0)
                GenerateLabels(srcmot, oldobj, Path.GetFileNameWithoutExtension(args[0]).Replace(".nja", ""), output);
            // Labels from filename
            else
                GenerateLabels(oldobj, "object_" + Path.GetFileNameWithoutExtension(args[0]).Replace(".nja", ""), output);
            ModelFile.CreateFile(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[0])), Path.GetFileNameWithoutExtension(args[0]) + "_rec.sa1mdl"), oldobj, null, null, null, new Dictionary<uint, byte[]>(), ModelFormat.BasicDX);
            IniSerializer.Serialize(output, Path.ChangeExtension(args[0], ".txt"));
        }

        static string UpTo(string s, string stopper) => s.Substring(0, Math.Max(0, s.IndexOf(stopper)));
    }      
}