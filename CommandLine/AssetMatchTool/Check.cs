using SAModel;
using System;

namespace AssetMatchTool
{
    public static class Check
    {
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

        public static bool CheckMotion(byte[] datafile, uint address, int numhierarchy, uint ImageBase, NJS_MOTION src_motion)
        {
            if (address > (uint)datafile.Length - 20) return false;
            int numparts_l = NJS_MOTION.CalculateModelParts(datafile, (int)address, ImageBase);
            if (numparts_l == 0) return false;
            else if (numparts_l < numhierarchy) return false;
                uint mdatap = ByteConverter.ToUInt32(datafile, (int)address);
            if (mdatap == 0 || mdatap <= ImageBase || mdatap >= datafile.Length)
                return false;
            //Console.WriteLine("Mdata: " + mdatap.ToString("X"));
            int mdata = (int)(mdatap - ImageBase);
            if (mdata <= 0) return false;
            int frames = ByteConverter.ToInt32(datafile, (int)address + 4);
            if (frames <= 0 || src_motion.Frames != frames)
                return false;
            //Console.WriteLine("Frames: " + frames.ToString("X"));
            AnimFlags animtype = (AnimFlags)ByteConverter.ToUInt16(datafile, (int)address + 8);
            // Types that don't exist in SADX
            if (animtype.HasFlag(AnimFlags.Vector))
                return false;
            if (animtype.HasFlag(AnimFlags.Spot))
                return false;
            if (animtype.HasFlag(AnimFlags.Quaternion))
                return false;
            if (animtype.HasFlag(AnimFlags.Point))
                return false;
            if (animtype.HasFlag(AnimFlags.Intensity))
                return false;
            if (animtype.HasFlag(AnimFlags.Color))
                return false;
            if (animtype.HasFlag(AnimFlags.Angle))
                return false;
            if (animtype.HasFlag(AnimFlags.Roll))
                return false;
            if (animtype.HasFlag(AnimFlags.Target))
                return false;
            if (!animtype.HasFlag(AnimFlags.Position) && 
                !animtype.HasFlag(AnimFlags.Rotation) && 
                !animtype.HasFlag(AnimFlags.Scale) && 
                !animtype.HasFlag(AnimFlags.Vertex) && 
                !animtype.HasFlag(AnimFlags.Normal))
                return false;
            return true;
        }

        public static bool CheckModel(byte[] datafile, uint address, int numhierarchy, ModelFormat modelfmt, uint ImageBase, bool landtable = false, bool verbose = false, NJS_OBJECT obj = null, bool relax = false)
        {
            //Console.WriteLine("Check: {0}", address.ToString("X"));
            if (address > (uint)datafile.Length - 20) return false;
            float radius = 0;
            uint vertices = 0;
            uint normals = 0;
            uint vert_count = 0;
            uint meshlists = 0;
            short mesh_count = 0;
            short mat_count = 0;
            float center_x = 0;
            float center_y = 0;
            float center_z = 0;
            int flags = ByteConverter.ToInt32(datafile, (int)address);
            ObjectFlags objeflags = (ObjectFlags)flags;
            if (flags > 0x3FFF || flags < 0) return false;
            uint attach = ByteConverter.ToUInt32(datafile, (int)address + 4);
            if (attach != 0 && attach < ImageBase) 
                return false;
            Vertex pos = new Vertex(datafile, (int)address + 8);
            Rotation rot = new Rotation(datafile, (int)address + 0x14);
            Vertex scl = new Vertex(datafile, (int)address + 0x20);
            uint child = ByteConverter.ToUInt32(datafile, (int)address + 0x2C);
            uint sibling = ByteConverter.ToUInt32(datafile, (int)address + 0x30);
            if (obj != null)
            {
                int objflags = (int)obj.GetFlags();
                if (objflags != 0 && objflags != flags) return false;
                if (obj.Children != null && obj.Children.Count > 0 && child == 0) return false;
                if (child != 0 && objeflags.HasFlag(ObjectFlags.NoChildren)) return false;
                if (obj.Sibling != null && sibling == 0) return false;
                if (CompareVector(obj.Scale, scl))
                    return false;
                if (CompareVector(obj.Position, pos))
                    return false;
                if (CompareRotation(obj.Rotation, rot))
                    return false;
            }
            if (landtable && (child != 0 || sibling != 0)) return false;
            if (child > address + ImageBase) return false;
            if (sibling > address + ImageBase) return false;
            if (child != 0 && child < ImageBase) return false;
            if (child > datafile.Length - 52 + ImageBase) return false;
            if (sibling > datafile.Length - 52 + ImageBase) return false;
            if (sibling != 0 && sibling < ImageBase) return false;
            if (attach != 0)
            {
                if (attach > datafile.Length - 51 + ImageBase) return false;
                vertices = ByteConverter.ToUInt32(datafile, ((int)(attach - ImageBase)));
                if (vertices < ImageBase) return false;
                if (vertices > datafile.Length - 51 + ImageBase) return false;
                normals = ByteConverter.ToUInt32(datafile, ((int)(attach - ImageBase) + 4));
                if (normals != 0 && normals < ImageBase) return false;
                if (normals > datafile.Length - 51 + ImageBase) return false;
                vert_count = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase) + 8);
                if (vert_count > 2048 || vert_count == 0) return false;
                meshlists = ByteConverter.ToUInt32(datafile, (int)(attach - ImageBase) + 0xC);
                if (meshlists != 0 && meshlists < ImageBase) return false;
                if (meshlists > datafile.Length - 51 + ImageBase) return false;
                mesh_count = ByteConverter.ToInt16(datafile, (int)(attach - ImageBase) + 0x14);
                if (mesh_count > 2048 || mesh_count < 0) return false;
                mat_count = ByteConverter.ToInt16(datafile, (int)(attach - ImageBase) + 0x16);
                if (mat_count > 2048 || mat_count < 0) return false;
                center_x = ByteConverter.ToSingle(datafile, (int)(attach - ImageBase) + 0x18);
                center_y = ByteConverter.ToSingle(datafile, (int)(attach - ImageBase) + 0x1C);
                center_z = ByteConverter.ToSingle(datafile, (int)(attach - ImageBase) + 0x20);
                radius = ByteConverter.ToSingle(datafile, (int)(attach - ImageBase) + 0x24);
                if (center_x < -100000.0f || center_x > 100000.0f) return false;
                if (center_y < -100000.0f || center_y > 100000.0f) return false;
                if (center_z < -100000.0f || center_z > 100000.0f) return false;
                if (radius < 0.0f || radius > 100000.0f) return false;
            }
            if (pos.X < -100000 || pos.X > 100000) return false;
            if (pos.Y < -100000 || pos.Y > 100000) return false;
            if (pos.Z < -100000 || pos.Z > 100000) return false;
            if (scl.X <= 0 || scl.X > 10000) return false;
            if (scl.Y <= 0 || scl.Y > 10000) return false;
            if (scl.Z <= 0 || scl.Z > 10000) return false;
            if (child == address + ImageBase || (attach != 0 && child == attach)) return false;
            if (sibling == address + ImageBase || (attach != 0 && sibling == attach)) return false;
            if (child != 0 && child == sibling) return false;
            if (!relax)
            {
                if (numhierarchy != -1 && child != 0)
                {
                    if (numhierarchy < 3)
                    {
                        numhierarchy++;
                        return CheckModel(datafile, child - ImageBase, numhierarchy, modelfmt, ImageBase, false, false, null);
                    }
                    else
                        return CheckModel(datafile, child - ImageBase, -1, modelfmt, ImageBase, false, false, null);
                }
                if (numhierarchy != -1 && sibling != 0)
                {
                    if (numhierarchy < 3)
                    {
                        numhierarchy++;
                        return CheckModel(datafile, sibling - ImageBase, numhierarchy, modelfmt, ImageBase, false, false, null);
                    }
                    else
                        return CheckModel(datafile, sibling - ImageBase, -1, modelfmt, ImageBase, false, false, null);
                }
                if (attach == 0 && flags == 0) return false;
            }
            //Console.WriteLine("Attach pointer {0}, Vertices count {1}, Mesh count {2}, Center {3} {4} {5}, Radius {6} at {7}", attach.ToString("X"), vert_count, mesh_count, center_x, center_y, center_z, radius, address.ToString("X"));
            if (numhierarchy != -1)
                if (verbose)
                    Console.WriteLine("Trying {0} model at {1}", modelfmt.ToString(), address.ToString("X"));
            return true;
        }

        public static bool CheckLandTable(byte[] datafile, uint address, LandTableFormat landfmt, uint ImageBase)
        {
            if (address > (uint)datafile.Length - 52)
                return false;
            short COLCount;
            short AnimCount;
            short ChunkCount;
            ushort Unknown1;
            uint COLAddress;
            uint AnimPointer;
            uint Texlist;
            uint Buffer;
            int ObjAddrPointer;
            uint ObjAddr;
            ModelFormat modelfmt = ModelFormat.Basic;
            switch (landfmt)
            {
                case LandTableFormat.SA1:
                    modelfmt = ModelFormat.Basic;
                    break;
                case LandTableFormat.SADX:
                    modelfmt = ModelFormat.BasicDX;
                    break;
                case LandTableFormat.SA2:
                    modelfmt = ModelFormat.Chunk;
                    break;
                case LandTableFormat.SA2B:
                    modelfmt = ModelFormat.GC;
                    break;
            }
            switch (landfmt)
            {
                case LandTableFormat.SA1:
                case LandTableFormat.SADX:
                    COLCount = ByteConverter.ToInt16(datafile, (int)address);
                    if (COLCount <= 0 || COLCount > 2048) return false;
                    AnimCount = ByteConverter.ToInt16(datafile, (int)address + 2);
                    if (AnimCount < 0 || AnimCount > 2048) return false;
                    COLAddress = ByteConverter.ToUInt32(datafile, (int)address + 0xC);
                    if (COLAddress < ImageBase || COLAddress == 0) return false;
                    if (COLAddress > datafile.Length - 32 + ImageBase) return false;
                    AnimPointer = ByteConverter.ToUInt32(datafile, (int)address + 0x10);
                    if (AnimPointer != 0 && AnimPointer < ImageBase) return false;
                    if (AnimPointer > datafile.Length - 32 + ImageBase) return false;
                    Texlist = ByteConverter.ToUInt32(datafile, (int)address + 0x18);
                    if (Texlist != 0 && Texlist < ImageBase) return false;
                    if (Texlist > datafile.Length - 32 + ImageBase) return false;
                    ObjAddrPointer = (int)(COLAddress - ImageBase) + 0x18;
                    ObjAddr = ByteConverter.ToUInt32(datafile, ObjAddrPointer);
                    if (ObjAddr < ImageBase) return false;
                    if (!CheckModel(datafile, ObjAddr - ImageBase, -1, modelfmt, ImageBase, true, false, null, false)) return false;
                    break;
                case LandTableFormat.SA2:
                case LandTableFormat.SA2B:
                    COLCount = ByteConverter.ToInt16(datafile, (int)address);
                    if (COLCount < 0) return false;
                    ChunkCount = ByteConverter.ToInt16(datafile, (int)address + 2);
                    if (ChunkCount < -1) return false;
                    Unknown1 = ByteConverter.ToUInt16(datafile, (int)address + 4);
                    if (Unknown1 != 65535) return false;
                    COLAddress = ByteConverter.ToUInt32(datafile, (int)address + 0x10);
                    if (COLAddress < ImageBase) return false;
                    if (COLAddress > datafile.Length - 32 + ImageBase) return false;
                    Buffer = ByteConverter.ToUInt32(datafile, (int)address + 0x14);
                    if (Buffer != 0) return false;
                    AnimPointer = ByteConverter.ToUInt32(datafile, (int)address + 0x18);
                    if (AnimPointer != 0 && AnimPointer < ImageBase) return false;
                    if (AnimPointer > datafile.Length - 32 + ImageBase) return false;
                    Texlist = ByteConverter.ToUInt32(datafile, (int)address + 0x1C);
                    if (Texlist > datafile.Length - 32 + ImageBase) return false;
                    if (Texlist == 0 || Texlist < ImageBase) return false;
                    break;
            }
            return true;
        }
    }
}