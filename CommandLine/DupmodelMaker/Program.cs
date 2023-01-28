using SAModel;
using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;

// This program scans a LandTable (.sa1lvl) file and outputs the file dupmodel.dup with declarations for models that reuse data.

namespace DupmodelMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("This program scans a landtable and outputs dupmodel.dup.");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("DupmodelMaker file.sa1lvl");
                Console.WriteLine("\nPress ENTER to exit.");
                Console.ReadLine();
                return;
            }
            LandTable land = LandTable.LoadFromFile(args[0]);
            List<Attach> atts = new List<Attach>();
            List<NJS_OBJECT> dupmodels = new List<NJS_OBJECT>();
            foreach (COL col in land.COL)
            {
                if (CheckDuplicateObject(col.Model) && !dupmodels.Contains(col.Model))
                {
                    dupmodels.Add(col.Model);
                    Console.WriteLine(col.Model.Name);
                }
            }
            // Make a list of duplicate models
            TextWriter tw = File.CreateText("dupmodel_c.dup");
            TextWriter tw_nja = File.CreateText("dupmodel.dup");
            foreach (NJS_OBJECT obj in dupmodels)
            {
                ObjectToC(obj, tw);
                ObjectToNJA(obj, tw_nja);
            }
            tw.Flush();
            tw.Close();
            tw_nja.Flush();
            tw_nja.Close();
            Console.WriteLine("Finished!");
        }

        static bool CheckDuplicateObject(NJS_OBJECT obj)
        {
            // This relies on the label of the OBJECT being different from that of the MODEL.
            // If the MODEL is being reused, usually the OBJECT label has a number at the end.
            // If the OBJECT has the number and the MODEL doesn't, it's most likely a duplicate.
            return obj.Name.Substring(obj.Name.Length - 3, 3) != obj.Attach.Name.Substring(obj.Attach.Name.Length - 3, 3);
        }

        static void ObjectToNJA(NJS_OBJECT obj, TextWriter writer)
        {
            writer.Write("OBJECT      ");
            writer.Write(obj.Name);
            writer.WriteLine("[]");
            writer.WriteLine("START");
            writer.WriteLine("EvalFlags ( 0x" + ((int)obj.GetFlags()).ToString("x8") + " ),");
            writer.WriteLine("Model       " + (obj.Attach != null ? obj.Attach.Name : "NULL") + ",");
            writer.WriteLine("OPosition  {0},", obj.Position.ToNJA());
            writer.WriteLine("OAngle     ( " + ((float)obj.Rotation.X / 182.044f).ToNJA() + ", " + ((float)obj.Rotation.Y / 182.044f).ToNJA() + ", " + ((float)obj.Rotation.Z / 182.044f).ToNJA() + " ),");
            writer.WriteLine("OScale     {0},", obj.Scale.ToNJA());
            writer.WriteLine("Child       " + (obj.Children.Count > 0 ? obj.Children[0].Name : "NULL") + ",");
            writer.WriteLine("Sibling     " + (obj.Sibling != null ? obj.Sibling.Name : "NULL") + ",");
            writer.WriteLine("END" + Environment.NewLine);
            writer.WriteLine("OBJECT_END");
            // Probably not necessary for LandTable
            /* 
            if (obj.Parent == null)
            {
                writer.WriteLine(Environment.NewLine + "DEFAULT_START");
                writer.WriteLine(Environment.NewLine + "#ifndef DEFAULT_OBJECT_NAME");
                writer.WriteLine("#define DEFAULT_OBJECT_NAME " + obj.Name);
                writer.WriteLine("#endif");
                writer.WriteLine(Environment.NewLine + "DEFAULT_END");
            }
            */
        }

        static void ObjectToC(NJS_OBJECT obj, TextWriter writer)
        {
            // ToStruct modified for array-like export
            writer.Write("NJS_OBJECT ");
            writer.Write(obj.Name);
            writer.Write("[1] = { { ");
            writer.Write(((StructEnums.NJD_EVAL)obj.GetFlags()).ToString().Replace(", ", " | "));
            writer.Write(", ");
            writer.Write(obj.Attach != null ? obj.Attach.Name : "NULL");
            foreach (float value in obj.Position.ToArray())
            {
                writer.Write(", ");
                writer.Write(value.ToC());
            }
            foreach (int value in obj.Rotation.ToArray())
            {
                writer.Write(", ");
                writer.Write(value.ToCHex());
            }
            foreach (float value in obj.Scale.ToArray())
            {
                writer.Write(", ");
                writer.Write(value.ToC());
            }
            writer.Write(", ");
            writer.Write(obj.Children.Count > 0 ? "&" + obj.Children[0].Name : "NULL");
            writer.Write(", ");
            writer.Write(obj.Sibling != null ? "&" + obj.Sibling.Name : "NULL");
            writer.Write(" }");
            writer.WriteLine(" };");
        }
    }
}