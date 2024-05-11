using System;
using System.Collections.Generic;
using System.IO;

// This program compares two binaries and outputs a list of differences in a format that can be read by SA1-DC-HD image builder's pattern patcher.
// At the moment pattern patching is used for pointers, and the program scans the binaries in 4-byte chunks.

namespace PatternPatchMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("This program compares two binaries and outputs a list of differences in a format that can be read by SA1-DC-HD image builder's pattern patcher.");
                Console.WriteLine("PRS files are unpacked and repacked automatically.");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("PatternPatchMaker <srcfile> <dstfile> -i");
                Console.WriteLine("\nsrcfile: Original file name, e.g. ADV00.PRS, for comparison");
                Console.WriteLine("\ndstfile: Patched file name, e.g. ADV00_patched.PRS, for finding the differences");
                Console.WriteLine("\n-i: Output an invididual file patch instead of a pattern patch.");
                Console.WriteLine("\nPattern patches are 4 bytes, individual patches can be as small as 1 byte.");
                Console.WriteLine("\nPress ENTER to exit.");
                Console.ReadLine();
                return;
            }
            string filen_src = args[0];
            string filen_dst = args[1];
            bool individual = (args.Length > 2 && args[2] == "-i");
            if (!File.Exists(filen_src) || !File.Exists(filen_dst))
            {
                Console.WriteLine("Source or destination file doesn't exist.");
                Console.ReadLine();
                return;
            }
            byte[] file_src = File.ReadAllBytes(filen_src);
            byte[] file_dst = File.ReadAllBytes(filen_dst);
            if (Path.GetExtension(filen_src).ToLowerInvariant() == ".prs")
                file_src = FraGag.Compression.Prs.Decompress(file_src);
            if (Path.GetExtension(filen_dst).ToLowerInvariant() == ".prs")
                file_dst = FraGag.Compression.Prs.Decompress(file_dst);
            if (file_src.Length != file_dst.Length)
            {
                Console.WriteLine("Source and destination file lengths don't match, resizing source array.");
                Array.Resize(ref file_src, file_dst.Length);
            }
            List<string> patches = new List<string>();
            int patchcount = 0;
            if (individual)
            {
                patches.Add("["+filen_src.ToUpperInvariant()+"]");
                int prevadr = 0;
                for (int index = 0; index < file_src.Length; index ++)
                {
                    byte data_src = file_src[index];
                    byte data_dst = file_dst[index];
                    if (data_src != data_dst)
                    {
                        if (index - prevadr == 1)
                        {
                            patches[patchcount] += " " + data_dst.ToString("X");
                            prevadr = index;
                        }
                        else
                        {
                            patches.Add(index.ToString("X") + "=" + data_dst.ToString("X"));
                            patchcount++;
                            prevadr = index;
                        }
                    }
                }
            }
            else
            {
                for (int index = 0; index < file_src.Length - 4; index += 4)
                {
                    uint data_src = BitConverter.ToUInt32(file_src, index);
                    uint data_dst = BitConverter.ToUInt32(file_dst, index);
                    if (data_src != data_dst)
                        patches.Add("Patch_" + index.ToString("X") + "," + data_src.ToString("X") + "," + data_dst.ToString("X"));
                }
            }
            File.WriteAllLines(Path.ChangeExtension(filen_src, ".ini"), patches.ToArray());
        }
    }
}