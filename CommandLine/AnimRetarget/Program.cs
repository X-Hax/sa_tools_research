using System;
using System.Collections.Generic;
using System.IO;
using SAModel;

// This tool retargets an animation for a different hierarchy.
// To use, load three motions:
// 1) Motion with original hierarchy
// 2) Motion identical to 1 but with desired hierarchy
// 3) Motion to be rearranged from original to desired hierarchy
// At the moment position and rotation animations are supported.

namespace AnimRetarget
{
    partial class Program
    {
        static void Main(string[] args)
        {
            NJS_MOTION motion_orig = NJS_MOTION.Load(args[0]);
            NJS_MOTION motion_new = NJS_MOTION.Load(args[1]);
            NJS_MOTION motion_dest = NJS_MOTION.Load(args[2]);
            Dictionary<int, int> MatchList = new Dictionary<int, int>();
            Console.WriteLine("Getting hierarchy data...");
            foreach (var model1 in motion_orig.Models)
            {
                // Compare by rotation
                if (model1.Value.Rotation.Count > 0 && model1.Value.Position.Count == 0)
                {
                    foreach (var model2 in motion_new.Models)
                    {
                        if (model2.Value.Rotation.Count > 0)
                            if (CompareMKEY_Rot(model1.Value.Rotation, model2.Value.Rotation))
                            {
                                Console.WriteLine("ROT MDATA {0} seems to match {1}", model1.Key, model2.Key);
                                MatchList.Add(model1.Key, model2.Key);
                                break;
                            }
                    }
                }
                // Compare by position
                if (model1.Value.Rotation.Count == 0 && model1.Value.Position.Count > 0)
                {
                    foreach (var model2 in motion_new.Models)
                    {
                        if (model2.Value.Position.Count > 0)
                            if (CompareMKEY_Pos(model1.Value.Position, model2.Value.Position))
                            {
                                Console.WriteLine("POS MDATA {0} seems to match {1}", model1.Key, model2.Key);
                                MatchList.Add(model1.Key, model2.Key);
                                break;
                            }
                    }
                }
                // Compare by both
                if (model1.Value.Rotation.Count > 0 && model1.Value.Position.Count > 0)
                {
                    foreach (var model2 in motion_new.Models)
                    {
                        if (model2.Value.Position.Count > 0 && model2.Value.Rotation.Count > 0)
                            if (CompareMKEY_Pos(model1.Value.Position, model2.Value.Position) && CompareMKEY_Rot(model1.Value.Rotation, model2.Value.Rotation))
                            {
                                Console.WriteLine("P&R MDATA {0} seems to match {1}", model1.Key, model2.Key);
                                MatchList.Add(model1.Key, model2.Key);
                                break;
                            }
                    }
                }
            }
            // Set MDATA indices in the destination motion
            Console.WriteLine("\nSetting hierarchy data in the destination model...");
            Dictionary<int, AnimModelData> newmodeldata = new Dictionary<int, AnimModelData>();
            foreach (KeyValuePair<int, int> match in MatchList)
            {
                foreach (var model in motion_dest.Models)
                {
                    if (model.Key == match.Key)
                    {
                        newmodeldata.Add(match.Value, model.Value);
                        Console.WriteLine("{0}->{1}", model.Key, match.Value);
                    }
                }
            }
            motion_dest.Models = newmodeldata;
            string outputname = Path.GetFileNameWithoutExtension(args[2]) + "_fix.saanim";
            Console.WriteLine("Output filename: {0}", outputname);
            motion_dest.Save(outputname);
        }

        static bool CompareMKEY_Rot(Dictionary<int, Rotation> dict1, Dictionary<int, Rotation> dict2)
        {
            foreach (KeyValuePair<int, Rotation> pair1 in dict1)
            {
                foreach (KeyValuePair<int, Rotation> pair2 in dict2)
                {
                    if (pair1.Key == pair2.Key)
                    {
                        if (pair1.Value.X != pair2.Value.X)
                        {
                            //Console.WriteLine("Different rotation X: {0} vs {1}", pair1.Value.X, pair2.Value.X);
                            return false;
                        }
                        if (pair1.Value.Y != pair2.Value.Y)
                        {
                            //Console.WriteLine("Different rotation Y: {0} vs {1}", pair1.Value.Y, pair2.Value.Y);
                            return false;
                        }
                        if (pair1.Value.Z != pair2.Value.Z)
                        {
                            //Console.WriteLine("Different rotation Z: {0} vs {1}", pair1.Value.Z, pair2.Value.Z);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        static bool CompareMKEY_Pos(Dictionary<int, Vertex> dict1, Dictionary<int, Vertex> dict2)
        {
            foreach (KeyValuePair<int, Vertex> pair1 in dict1)
            {
                foreach (KeyValuePair<int, Vertex> pair2 in dict2)
                {
                    if (pair1.Key == pair2.Key)
                    {
                        if (Math.Round(pair1.Value.X, 3) != Math.Round(pair2.Value.X, 3))
                        {
                            //Console.WriteLine("Different position X: {0} vs {1}", pair1.Value.X, pair2.Value.X);
                            return false;
                        }
                        if (Math.Round(pair1.Value.Y, 3) != Math.Round(pair2.Value.Y, 3))
                        {
                            //Console.WriteLine("Different position Y: {0} vs {1}", pair1.Value.Y, pair2.Value.Y);
                            return false;
                        }
                        if (Math.Round(pair1.Value.Z, 3) != Math.Round(pair2.Value.Z, 3))
                        {
                            //Console.WriteLine("Different position Z: {0} vs {1}", pair1.Value.Z, pair2.Value.Z);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}