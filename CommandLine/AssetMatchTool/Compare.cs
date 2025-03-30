using System;
using SAModel;

namespace AssetMatchTool
{
    public partial class Program
    {
        // Returns true if floats don't match
        static bool CompareFloat(float f1, float f2)
        {
            if (f1 == f2)
                return false;
            else if (f1 > f2)
                return Math.Abs(f1 - f2) > 0.05f;
            else
                return Math.Abs(f2 - f1) > 0.05f;
        }

        // Returns true if doubles don't match
        static bool CompareDouble(double f1, double f2)
        {
            if (f1 == f2)
                return false;
            else if (f1 > f2)
                return Math.Abs(f1 - f2) > 0.05f;
            else
                return Math.Abs(f2 - f1) > 0.05f;
        }

        // Returns true if vertices/vectors don't match
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

        // Returns true if ints don't match
        static bool CompareInt(int i1, int i2)
        {
            if (i1 == i2)
                return false;
            else if (i1 > i2)
                return Math.Abs(i1 - i2) > 32;
            else
                return Math.Abs(i2 - i1) > 32;
        }

        // Returns true if rotations don't match
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

        // Returns true if motions don't match
        static public bool CompareMotion(NJS_MOTION mot_src, NJS_MOTION mot_dest)
        {
            // Model parts
            if (mot_src.ModelParts != mot_dest.ModelParts)
            {
                return true;
            }
            if (mot_src.Frames != mot_dest.Frames)
            {
                return true;
            }
            if (mot_src.InterpolationMode != mot_dest.InterpolationMode)
            {
                return true;
            }
            if (mot_src.Models.Count != mot_dest.Models.Count)
            {
                return true;
            }
            foreach (var anim in mot_src.Models)
            {
                if (!mot_dest.Models.ContainsKey(anim.Key))
                {
                    return true;
                }
                if (mot_dest.Models[anim.Key] == null && anim.Value != null)
                {
                    return true;
                }
                if (mot_dest.Models[anim.Key] != null && anim.Value == null)
                {
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
            return false;
        }

        // Returns true if attaches don't match
        public static bool CompareAttach(BasicAttach att_src, BasicAttach att_dst)
        {
            // Compare materials
            if (att_src.Material.Count != att_dst.Material.Count)
            {
                return true;
            }
            else if (att_src.Material.Count > 0)
            {
                for (int m = 0; m < att_src.Material.Count; m++)
                {
                    NJS_MATERIAL[] mat_src = att_src.Material.ToArray();
                    NJS_MATERIAL[] mat_dst = att_dst.Material.ToArray();
                    if (m >= mat_dst.Length) break;
                    if (mat_src[m].TextureID != mat_dst[m].TextureID)
                    {
                        return true;
                    }
                    if (mat_src[m].Flags != mat_dst[m].Flags)
                    {
                        return true;
                    }
                    if (mat_src[m].Exponent != mat_dst[m].Exponent)
                    {
                        return true;
                    }
                    if (mat_src[m].DiffuseColor != mat_dst[m].DiffuseColor)
                    {
                        return true;
                    }
                    if (mat_src[m].SpecularColor != mat_dst[m].SpecularColor)
                    {
                        return true;
                    }
                }
            }

            // Compare vertices
            if (att_src.Vertex.Length != att_dst.Vertex.Length)
            {
                return true;
            }
            else
            {
                for (int m = 0; m < att_src.Vertex.Length; m++)
                {
                    if (m >= att_dst.Vertex.Length) break;
                    if (CompareVector(att_src.Vertex[m], att_dst.Vertex[m]))
                        return true;
                }
            }

            // Compare normals
            if (att_src.Normal.Length != att_dst.Normal.Length)
            {
                return true;
            }
            else
            {
                for (int m = 0; m < att_src.Normal.Length; m++)
                {
                    if (m >= att_dst.Normal.Length) break;
                    if (CompareVector(att_src.Normal[m], att_dst.Normal[m]))
                        return true;
                }
            }

            // Compare meshsets
            if (att_src.Mesh.Count != att_dst.Mesh.Count)
            {
                return true;
            }
            else
            {
                for (int u = 0; u < att_src.Mesh.Count; u++)
                {
                    // Compare attributes
                    if (att_src.Mesh[u].PAttr != att_dst.Mesh[u].PAttr)
                    {
                        return true;
                    }

                    // Check vertex colors
                    if ((att_src.Mesh[u].VColor == null && att_dst.Mesh[u].VColor != null) || (att_dst.Mesh[u].VColor == null && att_src.Mesh[u].VColor != null))
                    {
                        return true;
                    }

                    // Check UVs
                    if ((att_src.Mesh[u].UV == null && att_dst.Mesh[u].UV != null) || (att_dst.Mesh[u].UV == null && att_src.Mesh[u].UV != null))
                    {
                        return true;
                    }

                    // Compare polys
                    if (att_src.Mesh[u].Poly != null)
                    {
                        if (att_src.Mesh[u].Poly.Count != att_dst.Mesh[u].Poly.Count)
                        {
                            return true;
                        }
                        else
                        {
                            for (int v = 0; v < att_src.Mesh[u].Poly.Count; v++)
                            {
                                if (v >= att_dst.Mesh[u].Poly.Count) break;
                                if (att_src.Mesh[u].Poly[v].Indexes.Length != att_dst.Mesh[u].Poly[v].Indexes.Length)
                                {
                                    return true;
                                }
                                for (int i = 0; i < att_src.Mesh[u].Poly[v].Indexes.Length; i++)
                                {
                                    if (i >= att_dst.Mesh[u].Poly[v].Indexes.Length) break;
                                    if (att_src.Mesh[u].Poly[v].Indexes[i] != att_dst.Mesh[u].Poly[v].Indexes[i])
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }

                    // Compare vcolors
                    if (att_src.Mesh[u].VColor != null)
                    {
                        if (att_src.Mesh[u].VColor.Length != att_dst.Mesh[u].VColor.Length)
                        {
                            return true;
                        }
                        else
                        {
                            for (int v = 0; v < att_src.Mesh[u].VColor.Length; v++)
                            {
                                if (v >= att_dst.Mesh[u].VColor.Length) break;
                                if (att_src.Mesh[u].VColor[v].A != att_dst.Mesh[u].VColor[v].A || att_src.Mesh[u].VColor[v].R != att_dst.Mesh[u].VColor[v].R || att_src.Mesh[u].VColor[v].G != att_dst.Mesh[u].VColor[v].G || att_src.Mesh[u].VColor[v].B != att_dst.Mesh[u].VColor[v].B)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    // Compare UVs
                    if (att_src.Mesh[u].UV != null)
                    {
                        if (att_src.Mesh[u].UV.Length != att_dst.Mesh[u].UV.Length)
                        {
                            return true;
                        }
                        else
                        {
                            for (int uvindex = 0; uvindex < att_src.Mesh[u].UV.Length; uvindex++)
                            {
                                if (CompareDouble(att_src.Mesh[u].UV[uvindex].U, att_dst.Mesh[u].UV[uvindex].U))
                                    return true;
                                if (CompareDouble(att_src.Mesh[u].UV[uvindex].U, att_dst.Mesh[u].UV[uvindex].U))
                                    return true;

                            }
                        }
                    }
                }
            }
            return false;
        }

        // Returns true if objects don't match
        public static bool CompareModel(NJS_OBJECT mdl_src, NJS_OBJECT mdl_dst, bool notroot = false)
        {
            try
            {
                if ((mdl_src.Attach != null && mdl_dst.Attach == null) || (mdl_src.Attach == null && mdl_dst.Attach != null))
                {
                    return true;
                }
                if (mdl_src.GetObjects().Length > 1 || mdl_dst.GetObjects().Length > 1)
                {
                    if (mdl_src.GetObjects().Length != mdl_dst.GetObjects().Length)
                    {
                        return true;
                    }
                }
                int flags_src = (int)mdl_src.GetFlags();
                int flags_dst = (int)mdl_dst.GetFlags();
                if (flags_src != flags_dst)
                {
                    return true;
                }
                if (mdl_src.Attach != null)
                    if (CompareAttach((BasicAttach)mdl_src.Attach, (BasicAttach)mdl_dst.Attach))
                        return true;
                if (!notroot)
                {
                    NJS_OBJECT[] objs_src = mdl_src.GetObjects();
                    NJS_OBJECT[] objs_dst = mdl_dst.GetObjects();
                    for (int id = 0; id < objs_src.Length; id++)
                    {
                        if (objs_src[id].Attach != null)
                            if (CompareModel(objs_src[id], objs_dst[id], true))
                                return true;
                    }
                }
            }
            catch
            {
                return true;
            }
            return false;
        }

        // Returns true if landtable items don't match
        public static bool CompareCOL(COL item1, COL item2)
        {
            if (CompareVector(item1.Bounds.Center, item2.Bounds.Center))
                return true;
            if (CompareFloat(item1.Bounds.Radius, item2.Bounds.Radius))
                return true;
            if (item1.Flags != item2.Flags)
                return true;
            if (CompareAttach((BasicAttach)item1.Model.Attach, (BasicAttach)item2.Model.Attach))
                return true;
            return false;
        }
    }
}