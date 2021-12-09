using System;
using System.Collections.Generic;
using System.IO;
using SAModel;
using System.Linq;

namespace TexIDMatch
{
	class Program
	{
		static List<string> ReturnTexListNames(string filepath, bool isTLS = true)
		{
			List<string> texnames = new List<string>();
			string[] filelines = File.ReadAllLines(filepath);

			switch (isTLS)
			{
				case false:
					for (int i = 0; i < filelines.Length; i++)
					{
						string[] mastername = filelines[i].Split(',');
						texnames.Add(Path.GetFileNameWithoutExtension(mastername[1]));
					}
					break;

				case true:
				default:
					for (int i = 0; i < filelines.Length; i++)
					{
						texnames.Add(Path.GetFileNameWithoutExtension(filelines[i]));
					}
					break;
			}

			return texnames;
		}

		static void UpdateModel(ModelFile mdl, List<string> tls, List<string> pvm, string mdlFilename)
        {
			foreach (Attach att in mdl.Model.GetObjects().Where(a => a.Attach != null).Select(a => a.Attach))
				switch (att)
				{
					case BasicAttach batt:
						if (batt.Material != null)
							foreach (NJS_MATERIAL mat in batt.Material)
                            {
								string name = tls[mat.TextureID];
								mat.TextureID = pvm.IndexOf(name);
                            }
						break;
					case ChunkAttach catt:
						if (catt.Poly != null)
							foreach (PolyChunkTinyTextureID tex in catt.Poly.OfType<PolyChunkTinyTextureID>())
                            {
								string name = tls[tex.TextureID];
								tex.TextureID = (ushort)pvm.IndexOf(name);
							}
						break;
				}

			mdl.SaveToFile(mdlFilename);
		}

		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Texture ID Matching Tool");
				Console.WriteLine("This tool will take a partial texture list, match its IDs to a 'master texlist', and update \n");
				Console.WriteLine("Usage:");
				Console.WriteLine("texIDMatch <input *mdl file> <master texlist> [partial tls file]");
				Console.WriteLine("Input *MDL File - The object to have its texture IDs updated:");
				Console.WriteLine("Expected files: *.sa1mdl || *.sa2mdl || *.sa2bmdl\n");
				Console.WriteLine("Master Texlist - The reference texlist.");
				Console.WriteLine("Expected files: index.txt\n");
				Console.WriteLine("Partial TLS File - Partial texlist file. " +
					"If one is not supplied, program will attempt to locate one with the same filename as the supplied mdl file.");
				Console.WriteLine("Expected files: *.tls");
			}
			else
			{
				if (File.Exists(args[0]))
				{
					if (File.Exists(args[1]) && Path.GetExtension(args[1]) == ".txt")
					{
						ModelFile mdl = new ModelFile(Path.GetFullPath(args[0]));
						NJS_OBJECT obj = mdl.Model;
						List<string> masTexList = new List<string>(ReturnTexListNames(Path.GetFullPath(args[1]), false));
						string miniTLSPath = "";

						if (args.Length > 2 && File.Exists(args[2]))
							miniTLSPath = Path.GetFullPath(args[2]);
						else
							miniTLSPath = Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(args[0])) + ".tls");

						Console.WriteLine(miniTLSPath);

						List<string> minTexList = new List<string>(ReturnTexListNames(miniTLSPath));

						UpdateModel(mdl, minTexList, masTexList, Path.GetFullPath(args[0]));
					}
					else
						Console.WriteLine("Supplied Master Texlist is not a valid file.");
				}
				else
					Console.WriteLine("Supplied object file is not a valid file.");
			}
		}
	}
}
