using SAModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace splitDecomp
{
    partial class Program
    {
        // Generates dupmodel.dup and dupmotion.dup for landtables
        public static void GenerateDup(LandTable[] lands, string outpath, List<string> objLabels, List<string> motLabels)
        {
            // 'lands' is an array for cases like Ice Cap Act 4
            // It reuses Act 2 models but has some additional dup models, so the counting has to be done for both Acts 2 and 4
            // 'objLabels' and 'motLabels' are lists of object/attach and action/motion labels that were split before the dup check
            List<string> atts = new List<string>(); // List of NJS_MODELs that builds progressively
            List<string> child = new List<string>(); // List of child NJS_OBJECTS that builds progressively
            List<string> mots = new List<string>(); // List of NJS_MOTIONs that builds progressively (by name)
            List<NJS_OBJECT> dupmodels = new List<NJS_OBJECT>(); // List of all NJS_OBJECTS that reuse any NJS_MODEL
            List<NJS_ACTION> dupactions = new List<NJS_ACTION>();// List of all NJS_ACTIONS that reuse any NJS_MOTION
            List<NJS_OBJECT> dupmodels_result = new List<NJS_OBJECT>(); // List of dup objects found in the current landtable only
            List<NJS_ACTION> dupactions_result = new List<NJS_ACTION>(); // List of dup motions found in the current landtable only
            // Make a list of duplicate models and actions
            foreach (LandTable land in lands)
            {
                // Reset current dup lists on every new landtable
                dupmodels_result = new List<NJS_OBJECT>();
                dupactions_result = new List<NJS_ACTION>();
                // Make a list of objects to process - from both COL and Anim list
                List<NJS_OBJECT> landObjects = new List<NJS_OBJECT>();
                foreach (COL col in land.COL)
                    if (!landObjects.Contains(col.Model))
                        landObjects.Add(col.Model);
                if (land.Anim != null)
                    foreach (GeoAnimData geo in land.Anim)
                        if (!landObjects.Contains(geo.Model))
                            landObjects.Add(geo.Model);
                // Process the object list
                foreach (NJS_OBJECT obj in landObjects)
                {
                    Log.Write("OBJECT {0}: ", obj.Name);
                    // If the object was split before, treat it as original
                    if (objLabels.Contains(obj.Name))
                    {
                        Log.Write("original\n");
                        // If the object has a model, add it to the list
                        if (obj.Attach != null)
                            atts.Add(obj.Attach.Name);
                        // If the object has a child, add it to the list
                        if (obj.Children != null && obj.Children.Count > 0)
                            child.Add(obj.Children[0].Name);
                        continue;
                    }
                    // If the object's attach is already added to the current attach list or was split before, treat it as duplicate
                    if (obj.Attach != null && (atts.Contains(obj.Attach.Name) || objLabels.Contains(obj.Attach.Name)))
                    {
                        if (!dupmodels.Contains(obj))
                        {
                            Log.Write("reusing attach {0}\n", obj.Attach.Name);
                            dupmodels.Add(obj);
                            dupmodels_result.Add(obj);
                        }
                        else
                            Log.Write("is already in dupmodels\n");
                    }
                    // If the object's child is already added to the current child list, treat it as duplicate
                    else if (obj.Children != null && obj.Children.Count > 0 && child.Contains(obj.Children[0].Name))
                    {
                        if (!dupmodels.Contains(obj))
                        {
                            Log.Write("reusing child {0}\n", obj.Children[0].Name);
                            dupmodels.Add(obj);
                            dupmodels_result.Add(obj);
                        }
                        else
                            Log.Write("is already in dupmodels\n");
                    }
                    // This should not happen...
                    else
                    {
                        Log.Write("was not split before\n");
                        atts.Add(obj.Attach.Name);
                    }
                }
                // Process the GeoAnim list
                if (land.Anim != null)
                {
                    foreach (GeoAnimData geo in land.Anim)
                    {
                        Log.Write("ACTION {0}: ", geo.Animation.ActionName);
                        // If the action was already split, treat the motion as original
                        if (motLabels.Contains(geo.Animation.ActionName))
                        {
                            Log.Write("original\n");
                            mots.Add(geo.Animation.Name);
                            continue;
                        }
                        // If the motion is already on the current motions list or was split as part of another action, treat it as a duplicate
                        if (mots.Contains(geo.Animation.Name) || motLabels.Contains(geo.Animation.Name))
                        {
                            NJS_ACTION act = new NJS_ACTION(geo.Model, geo.Animation);
                            act.Name = geo.Animation.ActionName;
                            if (!dupactions.Contains(act))
                            {
                                Log.Write("reusing {0}\n", act.Animation.Name);
                                dupactions.Add(act);
                                dupactions_result.Add(act);
                            }
                            else
                                Log.Write("already in dupactions\n");
                        }
                        else
                        {
                            // This should not happen...
                            Log.Write("was not split before\n");
                            mots.Add(geo.Animation.Name);
                        }
                    }
                }
            }
            Directory.CreateDirectory(outpath);
            // Write dupmodel.dup
            if (dupmodels_result.Count > 0)
            {
                StreamWriter tw_nja = new StreamWriter(Path.Combine(outpath, "dupmodel.dup"));
                foreach (NJS_OBJECT obj in dupmodels_result)
                {
                    if (dupmodels_result.IndexOf(obj) > 0)
                        tw_nja.WriteLine();
                    ObjectToNJA(obj, tw_nja);
                }
                tw_nja.Flush();
                tw_nja.Close();
            }
            // Write dupmotion.dup
            if (dupactions_result.Count > 0)
            {
                StreamWriter twa_nja = new StreamWriter(Path.Combine(outpath, "dupmotion.dup"));
                foreach (NJS_ACTION act in dupactions_result)
                {
                    //Log.WriteLine("Export {0}", act.Name);
                    if (dupactions_result.IndexOf(act) > 0)
                        twa_nja.WriteLine();
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
            writer.WriteLine("END");
        }

    }
}