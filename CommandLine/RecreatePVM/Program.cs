﻿using ArchiveLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace RecreatePVM
{
    // This tool builds a PVM from a list of texture names by recursively searching for each texture in the "textures" folder.
    // Textures that couldn't be found use a question mark image but retain their names.

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("This tool builds a PVM from a list of texture names by recursively searching for each texture in the 'textures' folder.");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("RecreatePVM <texturelist.tls> [-q]");
                Console.WriteLine("\nSwitches:");
                Console.WriteLine("-q: When multiple unique textures with the same name are found, pick the first one by default (no prompt)");
                Console.WriteLine("\nPress ENTER to exit.");
                Console.ReadLine();
                return;
            }
            bool quick = false;
            if (args.Length > 1 && args[1] == "-q")
                quick = true;
            int count = 0;
            string[] files = File.ReadAllLines(args[0]);
            PuyoFile pvm = new PuyoFile();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Contains(".pvr"))
                {
                    string[] found = Directory.GetFiles("textures", files[i], SearchOption.AllDirectories);
                    if (found.Length == 1)
                    {
                        pvm.Entries.Add(new PVMEntry(File.ReadAllBytes(found[0]), Path.GetFileNameWithoutExtension(files[i])));
                        Console.WriteLine("{0}: Found {1}", i.ToString(), files[i]);
                    }
                    else if (found.Length > 1)
                    {
                        if (quick)
                        {
                            pvm.Entries.Add(new PVMEntry(File.ReadAllBytes(found[0]), Path.GetFileNameWithoutExtension(files[i])));
                            Console.WriteLine("{0}: Found {1}", i.ToString(), files[i]);
                        }
                        else
                        {
                            List<string> uniqueTexturesFound = new List<string>();
                            uniqueTexturesFound.Add(found[0]);
                            for (int l = 1; l < found.Length; l++)
                            {
                                if (!isTextureInList(found[l], uniqueTexturesFound))
                                    uniqueTexturesFound.Add(found[l]);
                            }
                            int res = 0;
                            if (uniqueTexturesFound.Count > 1)
                            {
                                Console.WriteLine();
                                Console.WriteLine("Multiple unique textures found for {0}", files[i]);
                                for (int z = 0; z < uniqueTexturesFound.Count; z++)
                                    Console.WriteLine("Texture {0}:{1}", z, uniqueTexturesFound[z]);
                                Console.WriteLine("Select which texture to use (type 1 or 2 or 3 etc.)");
                                string result = Console.ReadLine();
                                bool q = int.TryParse(result, out res);
                                if (!q)
                                    Console.WriteLine("Error parsing ID, using the first texture");
                            }
                            Console.WriteLine();
                            pvm.Entries.Add(new PVMEntry(File.ReadAllBytes(uniqueTexturesFound[res]), Path.GetFileNameWithoutExtension(files[i])));
                            Console.WriteLine("{0}: Found {1}", i.ToString(), uniqueTexturesFound[res]);
                        }
                    }
                    else
                    {
                        pvm.Entries.Add(new PVMEntry(Properties.Resource1.unknown, Path.GetFileNameWithoutExtension(files[i])));
                        Console.WriteLine("{0}: Missing {1}", i.ToString(), files[i]);
                        count++;
                    }
                }
            }
            pvm.Save(Path.ChangeExtension(args[0], ".pvm"));
            Console.WriteLine("Missing {0} textures of {1}", count, pvm.Entries.Count);
        }

        static bool IsTextureIdentical(string file1, string file2)
        {
            byte[] file1b = File.ReadAllBytes(file1);
            byte[] file2b = File.ReadAllBytes(file2);
            if (file1b.Length != file2b.Length)
                return false;
            for (int k = 32; k < file1b.Length; k++)
            {
                if (file1b[k] != file2b[k])
                    return false;
            }
            return true;
        }

        static bool isTextureInList(string file, List<string> list)
        {
            foreach (string str in list)
            {
                if (IsTextureIdentical(str, file))
                    return true;
            }
            return false;
        }
    }
}