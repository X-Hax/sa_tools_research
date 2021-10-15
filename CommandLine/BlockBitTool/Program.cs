using SAModel;
using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;

// This program converts BlockBits in SA1 Autodemo landtables to texture IDs matched with a recreated texture list.

namespace BlockBitTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("This tool rips LandTable files and their texture lists from Autodemo binaries with proper texture IDs.");
                Console.WriteLine("\nUsage:");
                Console.WriteLine("BlockBitTool <binaryFile> <key> <address>");
                Console.WriteLine("\nExample:");
                Console.WriteLine("BlockBitTool ADV02.PRS C900000 B2780");
                Console.WriteLine("The above will produce a landtable file and a texlist file from which a PVM can be reconstructed.");
                Console.WriteLine("\nPress ENTER to exit.");
                Console.ReadLine();
                return;
            }
            // Binary file
            byte[] binary = File.ReadAllBytes(args[0]);
            if (Path.GetExtension(args[0].ToLowerInvariant()) == ".prs")
                binary = FraGag.Compression.Prs.Decompress(binary);
            uint key = uint.Parse(args[1], System.Globalization.NumberStyles.HexNumber);
            // Landtable
            int address = int.Parse(args[2], System.Globalization.NumberStyles.HexNumber);
            LandTable original = new LandTable(binary, address, key, LandTableFormat.SA1);
            // First go through the COL list and create a texlist with all textures that it uses
            TexnameArray texlist_full = CreateLandtableTexlist(original, binary, key);
            texlist_full.Save(original.Name + ".tls");
            // Now go through the COL list again and match up texture names with the list that was created earlier
            LandTable result = ConvertLandTableBlockBits(original, binary, key, texlist_full);
            result.SaveToFile(original.Name + ".sa1lvl", LandTableFormat.SA1);
        }

        static TexnameArray CreateLandtableTexlist(LandTable landTable, byte[] binary, uint key)
        {
            List<string> texlistRecreated = new List<string>();
            if (landTable.TextureList != 0)
            {
                Console.WriteLine("This landtable has a single texlist pointer.");
                return new TexnameArray(binary, (int)(landTable.TextureList - key), key);
            }
            foreach (COL col in landTable.COL)
            {
                if (col.BlockBits > key)
                {
                    TexnameArray texlist = new TexnameArray(binary, (int)((uint)col.BlockBits - key), key);
                    BasicAttach batt = (BasicAttach)col.Model.Attach;
                    foreach (NJS_MATERIAL mat in batt.Material)
                    {
                        string texname = texlist.TextureNames[mat.TextureID];
                        if (!texlistRecreated.Contains(texname))
                            texlistRecreated.Add(texname);
                    }
                }
            }
            return new TexnameArray(texlistRecreated.ToArray());
        }

        static LandTable ConvertLandTableBlockBits(LandTable landTable, byte[] binary, uint key, TexnameArray texlistFull)
        {
            Dictionary<string, int> textureIDs = new Dictionary<string, int>();
            List<NJS_MATERIAL> fucked = new List<NJS_MATERIAL>();
            foreach (COL col in landTable.COL)
            {
                if (col.BlockBits != 0)
                {
                    TexnameArray texlist = new TexnameArray(binary, (int)((uint)col.BlockBits - key), key);
                    BasicAttach batt = (BasicAttach)col.Model.Attach;
                    foreach (NJS_MATERIAL mat in batt.Material)
                    {
                        //Console.WriteLine(batt.MaterialName +":"+ mat.TextureID + ":" + col.Model.Name);
                        if (fucked.Contains(mat))
                            continue;
                        string texname = texlist.TextureNames[mat.TextureID];
                        for (int t = 0; t < texlistFull.TextureNames.Length; t++)
                        {
                            if (texlistFull.TextureNames[t] == texname)
                            {
                                mat.TextureID = t;
                                fucked.Add(mat);
                                Console.WriteLine("Texture ID {0} for texture {1}", t.ToString(), texname);
                                break;
                            }
                        }

                    }
                    col.BlockBits = 0;
                }
            }
            return landTable;
        }
    }
}