using SAModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace splitDecomp
{
    partial class Program
    {
        // Generates dupmodel.dup and dupmotion.dum for landtables
        public static void GenerateDup(LandTable[] lands, string outpath)
        {
            // 'lands' is an array for cases like Ice Cap Act 4
            // It reuses Act 2 models but has some additional dup models, so the counting has to be done for both Acts 2 and 4
            List<Attach> atts = new List<Attach>(); // List of NJS_MODELs that builds progressively
            List<NJS_MOTION> mots = new List<NJS_MOTION>(); // List of NJS_MOTIONs that builds progressively
            List<NJS_OBJECT> dupmodels = new List<NJS_OBJECT>(); // List of all NJS_OBJECTS that reuse any NJS_MODEL
            List<NJS_ACTION> dupactions = new List<NJS_ACTION>();// List of all NJS_ACTIONS that reuse any NJS_MOTION
            List<NJS_OBJECT> dupmodels_result = new List<NJS_OBJECT>(); // List of dup objecs found in the current landtable only
            List<NJS_ACTION> dupactions_result = new List<NJS_ACTION>(); // List of dup mtions found in the current landtable only
            // Make a list of duplicate models and actions
            foreach (LandTable land in lands)
            {
                // Reset current dup lists on every new landtable
                dupmodels_result = new List<NJS_OBJECT>();
                dupactions_result = new List<NJS_ACTION>();
                foreach (COL col in land.COL)
                {
                    //if (CheckDuplicateObject(col.Model) && !dupmodels.Contains(col.Model))
                    if (atts.Contains(col.Model.Attach))
                    {
                        if (!dupmodels.Contains(col.Model))
                        {
                            dupmodels.Add(col.Model);
                            dupmodels_result.Add(col.Model);
                        }
                    }
                    else
                        atts.Add(col.Model.Attach);
                }
                if (land.Anim != null)
                {
                    foreach (GeoAnimData geo in land.Anim)
                    {
                        if (mots.Contains(geo.Animation))
                        {
                            NJS_ACTION act = new NJS_ACTION(geo.Model, geo.Animation);
                            act.Name = geo.Animation.ActionName;
                            if (!dupactions.Contains(act))
                            {
                                dupactions.Add(act);
                                dupactions_result.Add(act);
                            }
                        }
                        else
                            mots.Add(geo.Animation);
                    }
                }
            }
            Directory.CreateDirectory(outpath);
            // Write dupmodel.dup
            if (dupmodels_result.Count > 0)
            {
                TextWriter tw_nja = File.CreateText(Path.Combine(outpath, "dupmodel.dup"));
                foreach (NJS_OBJECT obj in dupmodels_result)
                {
                    ObjectToNJA(obj, tw_nja);
                }
                tw_nja.Flush();
                tw_nja.Close();
            }
            // Write dupmotion.dup
            if (dupactions_result.Count > 0)
            {
                TextWriter twa_nja = File.CreateText(Path.Combine(outpath, "dupmotion.dup"));
                foreach (NJS_ACTION act in dupactions_result)
                {
                    ActionToNJA(act, twa_nja);
                }
                twa_nja.Flush();
                twa_nja.Close();
            }
        }

        // Quick stubs for dupmodel/dupmotion generation
        static void ActionToNJA(NJS_ACTION act, TextWriter writer)
        {
            writer.WriteLine("ACTION {0}[]", act.Name);
            writer.WriteLine("START");
            writer.WriteLine("ObjectHead      {0},", act.Model.Name);
            writer.WriteLine("Motion          " + act.Animation.Name);
            writer.WriteLine("END" + Environment.NewLine);
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
        }

    }
}