using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using SAModel;
using SplitTools;

namespace splitDecomp
{
    partial class Program
    {
        // Gets a label for address
        private static string GetLabelForAddress(int address, Dictionary<int, string> labels)
        {
            if (labels.ContainsKey(address))
                return labels[address];
            return null;
        }

        // Gets a label for address based on the label such as "object_00000000"
        private static string GetLabelForAddressFromString(string name, Dictionary<int, string> labels)
        {
            int address = int.Parse(name.Substring(name.Length - 8, 8), NumberStyles.HexNumber);
            if (labels.ContainsKey(address))
                return labels[address];
            return null;
        }

        // Creates an object name starting with "object_" from the filename
        static string ObjNameFromFilename(string filename)
        {
            // Remove SAModel extensions
            if (filename.Contains(".sa1mdl"))
                filename = filename.Replace(".sa1mdl", "");
            if (filename.Contains(".sa2mdl"))
                filename = filename.Replace(".sa2mdl", "");
            // If there's still an extension, remove it
            if (filename.Contains('.'))
                filename = Path.GetFileNameWithoutExtension(filename);
            StringBuilder sb = new StringBuilder();
            // Remove garbage
            if (filename.StartsWith("object___"))
            {
                filename = filename.Replace("object___", "");
            }
            if (filename.StartsWith("object_"))
            {
                filename = filename.Replace("object_", "");
            }
            // Split prefix and suffixes
            string[] nameparts = filename.Split('_');
            // Add the starting label
            sb.Append("object_");
            //Console.WriteLine(nameparts.Length.ToString());
            // Add label parts
            if (nameparts.Length > 0)
            {
                // Add prefix
                sb.Append(nameparts[0]);
                if (nameparts.Length > 1)
                    sb.Append("_");
                // Add other parts
                for (int i = 1; i < nameparts.Length; i++)
                {
                    sb.Append(nameparts[i]);
                    if (nameparts.Length > 1 && i < nameparts.Length - 1)
                        sb.Append("_");
                }
                // Add a separator before adding parts again
                if (nameparts.Length > 1)
                    sb.Append("_");
                // Add other parts again without the last "_"
                for (int i = 1; i < nameparts.Length; i++)
                {
                    // Ignore certain keywords that don't appear twice
                    //if (nameparts[i].Contains("kihon"))
                    //continue;
                    sb.Append(nameparts[i]);
                    if (nameparts.Length > 1 && i < nameparts.Length - 1)
                        sb.Append("_");
                }
            }
            return sb.ToString();
        }

        // Generates labels for an NJS_TEXLIST from filename
        static void TexLabelsFromFilename(NJS_TEXLIST tx, string filename)
        {
            // Remove SAModel extensions
            if (filename.Contains(".satex"))
                filename = filename.Replace(".satex", "");
            // If there's still an extension, remove it
            if (filename.Contains('.'))
                filename = Path.GetFileNameWithoutExtension(filename);
            if (filename.StartsWith("texlist_"))
            {
                tx.Name = filename;
                tx.TexnameArrayName = ReplaceFirst(filename, "texlist_", "texture_");
            }
            else
            {
                tx.Name = "texlist_" + filename;
                tx.TexnameArrayName = "texture_" + filename;
            }
        }

        // Creates an NJS_MOTION name starting with "motion_" or "shape_" from the filename
        static string MotNameFromFilename(string filename, bool shape)
        {
            string prefix = shape ? "shape_" : "motion_";
            // Remove SAModel extensions
            if (filename.Contains(".saanim"))
                filename = filename.Replace(".saanim", "");
            // If there's still an extension, remove it
            if (filename.Contains('.'))
                filename = Path.GetFileNameWithoutExtension(filename);
            if (filename.StartsWith("action_"))
                return ReplaceFirst(filename, "action_", prefix);
            if (filename.StartsWith(prefix))
                return filename;
            return prefix + filename;
        }

        // Creates an NJS_ACTION name starting with "action_" from the filename
        static string ActNameFromFilename(string filename)
        {
            // Remove SAModel extensions
            if (filename.Contains(".saanim"))
                filename = filename.Replace(".saanim", "");
            // If there's still an extension, remove it
            if (filename.Contains('.'))
                filename = Path.GetFileNameWithoutExtension(filename);
            if (filename.StartsWith("action_"))
                return filename;
            if (filename.StartsWith("motion_"))
                return ReplaceFirst(filename, "motion_", "action_");
            return "action_" + filename;
        }

        // Creates labels for NJS_MODEL based on object name
        static void AttachLabelsFromName(BasicAttach batt, string label, Dictionary<int, string> output)
        {
            batt.Name = AddLabel(batt.Name, label, "model", output);
            if (LabelIsNumerical(batt.NormalName))
                batt.NormalName = AddLabel(batt.NormalName, label, "normal", output);
            if (LabelIsNumerical(batt.VertexName))
                batt.VertexName = AddLabel(batt.VertexName, label, "point", output);
            if (LabelIsNumerical(batt.MeshName))
                batt.MeshName = AddLabel(batt.MeshName, label, "meshset", output);
            if (LabelIsNumerical(batt.MaterialName))
                batt.MaterialName = AddLabel(batt.MaterialName, label, "material", output);
            if (batt.Mesh != null && batt.Mesh.Count > 0)
            {
                foreach (NJS_MESHSET mesh in batt.Mesh)
                {
                    //Console.WriteLine("Mesh {0}", batt.Mesh.IndexOf(mesh));
                    if (mesh.Poly != null && LabelIsNumerical(mesh.PolyName))
                    {
                        mesh.PolyName = AddLabelWithID(mesh.PolyName, label, "pgS", batt.Mesh.IndexOf(mesh), output);
                    }
                    if (mesh.UV != null && LabelIsNumerical(mesh.UVName))
                    {
                        mesh.UVName = AddLabelWithID(mesh.UVName, label, "vuvS", batt.Mesh.IndexOf(mesh), output);
                    }
                    if (mesh.VColor != null && LabelIsNumerical(mesh.VColorName))
                    {
                        mesh.VColorName = AddLabelWithID(mesh.VColorName, label, "vcS", batt.Mesh.IndexOf(mesh), output);
                    }
                    if (mesh.PolyNormal != null && LabelIsNumerical(mesh.PolyNormalName))
                    {
                        mesh.PolyNormalName = AddLabelWithID(mesh.PolyNormalName, label, "pnS", batt.Mesh.IndexOf(mesh), output);
                    }
                }
            }
        }

        // Creates labels for NJS_OBJECT based on object name
        static void ObjLabelsFromName(NJS_OBJECT obj, string label, Dictionary<int, string> output, bool root = true)
        {
            //Log.WriteLine("Object: {0}", label);
            int addr = GetAddressFromLabel(obj.Name);
            if (addr != 0 && !output.ContainsKey(addr))
                output.Add(addr, label);
            obj.Name = label;
            if (obj.Attach != null && obj.Attach is BasicAttach && LabelIsNumerical(obj.Attach.Name))
            {
                AttachLabelsFromName((BasicAttach)obj.Attach, label, output);
            }
            if (root)
            {
                NJS_OBJECT[] oos = obj.GetObjects();
                for (int ch = 0; ch < oos.Length - 1; ch++)
                {
                    ObjLabelsFromName(oos[ch + 1], label + "_c" + ch.ToString("D2"), output, false);
                }
            }
        }

        // Creates labels for NJS_OBJECT based on filename
        static void ObjLabelsFromFilename(NJS_OBJECT obj, string filename, Dictionary<int, string> output)
        {
            ObjLabelsFromName(obj, ObjNameFromFilename(filename), output);
        }

        // Generate labels for NJS_ACTION based on labels for NJS_OBJECT and action name
        static void ActLabelsFromActName(NJS_ACTION act, string actname, Dictionary<int, string> output)
        {
            if (!output.ContainsValue(act.Animation.Name))
                MotLabelsFromMotObjName(act.Animation, act.Model, actname, output);
            act.Name = AddLabel(act.Name, act.Model.Name, "action", output);
        }
        
        // Generate labels for NJS_MOTION based on labels for NJS_OBJECT and motion name
        static void MotLabelsFromMotObjName(NJS_MOTION mot, NJS_OBJECT obj, string motname, Dictionary<int, string> output)
        {
            //Log.WriteLine("Motion for object: {0}", obj.Name);
            if (motname.StartsWith("motion_"))
                motname = motname.Replace("motion_", "");
            if (motname.StartsWith("action_"))
                motname = motname.Replace("action_", "");
            string objName = obj.Name;
            mot.Name = AddLabel(mot.Name, objName, "motion_" + motname, output);
            mot.MdataName = AddLabel(mot.MdataName, objName, "mdata_" + motname, output);
            //Log.WriteLine("Motion: {0}", mot.Name);
            //Log.WriteLine("Mdata: {0}", mot.MdataName);
            int nodeid = 0;
            foreach (KeyValuePair<int, AnimModelData> model in mot.Models)
            {
                string nodename = obj.GetObjectsAnimated()[model.Key].Name;
                if (model.Value.Position.Count > 0)
                {
                    model.Value.PositionName = AddLabel(model.Value.PositionName, nodename, "pos_" + motname, output);
                    //Log.WriteLine("Pos: {0}", model.Value.PositionName);
                }
                if (model.Value.Rotation.Count > 0)
                {
                    model.Value.RotationName = AddLabel(model.Value.RotationName, nodename, "rot_" + motname, output);
                    //Log.WriteLine("Rot: {0}", model.Value.RotationName);
                }
                if (model.Value.Scale.Count > 0)
                {
                    model.Value.ScaleName = AddLabel(model.Value.ScaleName, nodename, "scl_" + motname, output);
                    //Log.WriteLine("Scl: {0}", model.Value.ScaleName);
                }
                if (model.Value.Vector.Count > 0)
                {
                    model.Value.VectorName = AddLabel(model.Value.VectorName, nodename, "vct_" + motname, output);
                    //Log.WriteLine("Vect: {0}", model.Value.VectorName);
                }
                // TODO
                /*
                if (model.Value.Vertex.Count > 0)
                {
                    model.Value.VertexName = MakeLabel(nodename, "vlist_" + motname);
                    for (int frame = 0; frame < model.Value.Vertex.Count; frame++)
                    {
                        int[] keyframes = model.Value.Vertex.Keys.ToArray();
                        model.Value.VertexItemName[frame] = MakeLabel(nodename, "vert" + keyframes[frame].ToString() + "_" + motname);
                    }
                }
                if (model.Value.Normal.Count > 0)
                {
                    model.Value.ScaleName = MakeLabel(nodename, "vert" + model.Key.ToString() + "_" + motname);
                }
                */
                //Log.WriteLine();
            }

        }

        // Gets the address from a label, e.g. "01020304" out of "object_01020304" as an integer
        static int GetAddressFromLabel(string label)
        {
            if (label == null || label.Length < 11) //Smallest possible autogenerated name like "uv_..."
                return 0;
            int addr = 0;
            bool succ = int.TryParse(label.Substring(label.Length - 8, 8), NumberStyles.HexNumber, null, out addr);
            if (succ == false)
                Log.WriteLine("Name {0} couldn't be parsed", label);
            return addr;
        }

        // Returns a generated label based on the object name, a new prefix and an array ID (for meshset items etc.)
        public static string AddLabelWithID(string name_addr, string label_object, string newprefix, int index, Dictionary<int, string> output)
        {
            // name_addr is an address-based label such as "attach_00000000"
            // label_object is an NJS_OBJECT label such as "object_myobject"
            // newprefix is the new prefix for the label such as "model"
            // output is a labels list
            // return value is the complete label such as "attach_myobject"
            string result = ReplaceFirst(label_object, "object", newprefix + "_" + index.ToString());
            int addr = GetAddressFromLabel(name_addr);
            if (addr != 0 && !output.ContainsKey(addr))
                output.Add(addr, result);
            //Log.WriteLine("{0}: {1}", name_addr, result);
            return result;
        }

        // Returns a generated label based on the object name and a new prefix
        static string AddLabel(string name_addr, string label_object, string newprefix, Dictionary<int, string> output)
        {
            // name_addr is an address-based label such as "attach_00000000"
            // label_object is an NJS_OBJECT label such as "object_myobject"
            // newprefix is the new prefix for the label such as "model"
            // output is a labels list
            // return value is the complete label such as "attach_myobject"
            string result = ReplaceFirst(label_object, "object", newprefix);
            int addr = GetAddressFromLabel(name_addr);
            if (addr != 0 && !output.ContainsKey(addr))
                output.Add(addr, result);
            //Log.WriteLine("{0}: {1}", name_addr, result);
            return result;
        }

        // From https://stackoverflow.com/a/8809437
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        // Generates labels for a Landtable
        public static void GenerateLandtableLabels(LandTable lt, Dictionary<int, string> labels)
        {
            Dictionary<string, int> attachCounts = new Dictionary<string, int>();
            Dictionary<string, int> childCounts = new Dictionary<string, int>();
            Dictionary<string, int> actionCounts = new Dictionary<string, int>();
            Dictionary<string,string> originalActions = new Dictionary<string, string>();
            List<NJS_OBJECT> objs = new List<NJS_OBJECT>();
            foreach (var col in lt.COL)
            {
                if (!objs.Contains(col.Model))
                    objs.Add(col.Model);
            }
            if (lt.Anim != null && lt.Anim.Count > 0)
            {
                foreach (var an in lt.Anim)
                {
                    if (!objs.Contains(an.Model))
                        objs.Add(an.Model);
                    if (!LabelIsNumerical(an.Animation.ActionName))
                        originalActions.Add(an.Animation.Name, an.Animation.ActionName);
                }
            }
            foreach (NJS_OBJECT obj in objs)
            {
                if (LabelIsNumerical(obj.Name))
                {
                    if (obj.Attach != null && labels.ContainsValue(obj.Attach.Name))
                    {
                        int maddr = GetAddressFromLabel(obj.Name);
                        if (!attachCounts.ContainsKey(obj.Attach.Name))
                            attachCounts.Add(obj.Attach.Name, 0);
                        obj.Name = ReplaceFirst(obj.Attach.Name, "model_", "object_") + "_d" + attachCounts[obj.Attach.Name].ToString("D2");
                        labels.Add(maddr, obj.Name);
                        attachCounts[obj.Attach.Name]++;
                        Log.WriteLine("Land object {0} at {1}", obj.Name, maddr.ToString("X"));
                    }
                    else if (obj.Children != null && obj.Children.Count > 0 && labels.ContainsValue(obj.Children[0].Name))
                    {
                        int maddr = GetAddressFromLabel(obj.Name);
                        if (!childCounts.ContainsKey(obj.Children[0].Name))
                            childCounts.Add(obj.Children[0].Name, 0);
                        obj.Name = obj.Children[0].Name + "_d" + childCounts[obj.Children[0].Name].ToString("D2");
                        labels.Add(maddr, obj.Name);
                        childCounts[obj.Children[0].Name]++;
                        Log.WriteLine("Landanim object {0} at {1}", obj.Name, maddr.ToString("X"));
                    }
                }
            }
            if (lt.Anim != null && lt.Anim.Count > 0)
            {
                foreach (var geo in lt.Anim)
                {
                    if (LabelIsNumerical(geo.Animation.ActionName))
                    {
                        if (originalActions.ContainsKey(geo.Animation.Name))
                        {
                            int aaddr = GetAddressFromLabel(geo.Animation.ActionName);
                            if (!actionCounts.ContainsKey(geo.Animation.Name))
                                actionCounts.Add(geo.Animation.Name, 0);
                            geo.Animation.ActionName = originalActions[geo.Animation.Name] + "_d" + actionCounts[geo.Animation.Name].ToString("D2");
                            labels.Add(aaddr, geo.Animation.ActionName);
                            actionCounts[geo.Animation.Name]++;
                            Log.WriteLine("Landanim action {0} at {1}", geo.Animation.ActionName, aaddr.ToString("X"));
                        }
                    }
                }
            }
        }
    }
}