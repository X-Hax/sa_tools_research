using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SAModel;

namespace AssetMatchTool
{
    public partial class Program
    {
        static List<int> FindMotion(NJS_MOTION src_motion, int numparts, byte[] dataFile, uint imageBase, int startAddr, int endAddr)
        {
            if (endAddr == 0)
                endAddr = dataFile.Length;
            List<int> motion_list = new List<int>();
            int step = (endAddr - startAddr) / 32;
            List<Task> tasks = new List<Task>();
            for (int s = 0; s < 32; s++)
            {
                int start = Math.Max(startAddr, startAddr + step * (s - 1));
                int end = Math.Min(startAddr + s * step, endAddr);
                Task t = new Task(() =>
                {
                    for (int i = start; i < end; i += 4)
                    {
                        if (!Check.CheckMotion(dataFile, (uint)i, numparts, imageBase, src_motion))
                            continue;
                        else
                        {
                            try
                            {
                                NJS_MOTION dst_motion = new NJS_MOTION(dataFile, i, imageBase, numparts, new Dictionary<int, string>());
                                if (CompareMotion(src_motion, dst_motion))
                                    continue;
                                motion_list.Add(i);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                });
                tasks.Add(t);
                t.Start();
            }
            Task.WaitAll(tasks.ToArray());
            return motion_list;
        }

        static List<int> FindModel(NJS_OBJECT src_model, byte[] dataFile, uint imageBase, int startAddr, int endAddr, bool relax)
        {
            if (endAddr == 0)
                endAddr = dataFile.Length;
            List<int> model_list = new List<int>();
            int step = (endAddr - startAddr) / 32;
            List<Task> tasks = new List<Task>();
            for (int s = 0; s < 32; s++)
            {
                int start = Math.Max(startAddr, startAddr + step * (s - 1));
                int end = Math.Min(startAddr + s * step, endAddr);
                Task t = new Task(() =>
                {
                    for (int i = start; i < end; i += 4)
                    {
                        if (!Check.CheckModel(dataFile, (uint)i, src_model.GetObjects().Length, ModelFormat.BasicDX, imageBase, false, false, src_model, relax))
                            continue;
                        else
                        {
                            try
                            {
                                NJS_OBJECT dst_model = new NJS_OBJECT(dataFile, i, imageBase, ModelFormat.BasicDX, new Dictionary<int, Attach>());
                                if (CompareModel(src_model, dst_model))
                                    continue;
                                model_list.Add(i);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                });
                tasks.Add(t);
                t.Start();
            }
            Task.WaitAll(tasks.ToArray());
            return model_list;
        }

        static List<int> FindLandtable(LandTable land_src, byte[] dataFile, uint imageBase)
        {
            List<int> land_lands = new List<int>();
            COL[] arr_src = land_src.COL.ToArray();
            for (int i = 0; i < dataFile.Length; i+=4)
            {
                if (!Check.CheckLandTable(dataFile, (uint)i, LandTableFormat.SADX, imageBase))
                    continue;
                else
                {
                    try
                    {
                        LandTable land_dst = new LandTable(dataFile, i, imageBase, LandTableFormat.SADX);
                        if (land_dst.TextureFileName != land_src.TextureFileName)
                            continue;
                        if (land_dst.Flags != land_src.Flags)
                            continue;
                        if ((short)land_dst.Attributes != (short)land_src.Attributes)
                            continue;
                        if (land_dst.FarClipping != land_src.FarClipping)
                            continue;

                        COL[] arr_dst = land_dst.COL.ToArray();

                        if (arr_dst.Length != arr_src.Length)
                        {
                            continue;
                        }
                        // Compare COL items
                        bool same = true;
                        for (int c = 0; c < arr_dst.Length; c++)
                        {
                            if (CompareCOL(arr_src[c], arr_dst[c]))
                            {
                                same = false;
                                break;
                            }
                        }
                        if (same)
                        {
                            land_lands.Add(i);
                        }
                        else
                            continue;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return land_lands;
        }
    }
}