using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Exports .sa*mdl and .saanim files as if the were compiled into the binary. Useful for hacking data in DC versions of SA1/SA2.
namespace ExportRawBinary
{
	class Program
	{
		static void Main(string[] args)
		{
			string srcfilename;
            uint startaddr = 0;
			uint endaddr = 0;

			if (args.Length > 1)
			{
				// Parse arguments
				srcfilename = Path.GetFullPath(args[0]);
				string dstfilename = Path.ChangeExtension(srcfilename, ".bin");
				uint key = uint.Parse(args[1], System.Globalization.NumberStyles.HexNumber);
				if (args.Length > 2)
					startaddr = uint.Parse(args[2], System.Globalization.NumberStyles.HexNumber);
				if (args.Length > 3)
					endaddr = uint.Parse(args[3], System.Globalization.NumberStyles.HexNumber);
				if (args.Length > 4)
					dstfilename = Path.GetFullPath(args[4]);
				// Export data
				switch (Path.GetExtension(srcfilename).ToLowerInvariant())
				{
					// OBJECT
					case ".sa1mdl":
					case ".sa2mdl":
					case ".sa2bmdl":
						ModelFile modelFile = new ModelFile(srcfilename);
						NJS_OBJECT nObject = modelFile.Model;
						byte[] objBytes = nObject.GetBytes(key + startaddr, false, out uint addr);
						if (endaddr != 0 && endaddr != addr)
						{
							if (endaddr < addr)
							{
								Console.WriteLine("Not enough space for the root node, earliest possible position is {0}.", addr.ToString("X"));
								return;
							}
							Console.WriteLine("Free bytes: {0} from {1} to {2}", endaddr - addr - startaddr, addr.ToCHex(), (endaddr-startaddr).ToCHex());
							List<byte> tempbytes = objBytes.ToList();
							tempbytes.InsertRange((int)addr, new byte[endaddr - addr - startaddr]);
							objBytes = tempbytes.ToArray();
						}
                        if (args.Length <= 4)
							File.WriteAllBytes(dstfilename, objBytes);
                        else
                        {
                            byte[] outBytes = File.ReadAllBytes(dstfilename);
                            Array.Copy(objBytes, 0, outBytes, startaddr, objBytes.Length);
                            File.WriteAllBytes(dstfilename, outBytes);
                        }
                        break;
					// MOTION
                    case ".saanim":
						NJS_MOTION motion = NJS_MOTION.Load(srcfilename);
						byte[] motBytes = motion.GetBytes(key + startaddr, new Dictionary<string, uint>(), out uint addrm);
                        if (endaddr != 0 && endaddr != addrm)
                        {
                            if (endaddr < addrm)
                            {
                                Console.WriteLine("Not enough space for the motion, earliest possible position is {0}.", addrm.ToString("X"));
                                return;
                            }
                            Console.WriteLine("Free bytes: {0} from {1} to {2}", endaddr - addrm - startaddr, addrm.ToCHex(), (endaddr - startaddr).ToCHex());
                            List<byte> tempbytes = motBytes.ToList();
                            tempbytes.InsertRange((int)addrm, new byte[endaddr - addrm - startaddr]);
                            motBytes = tempbytes.ToArray();
                        }
						if (args.Length <= 4)
							File.WriteAllBytes(dstfilename, motBytes);
						else
						{
                            byte[] outBytes = File.ReadAllBytes(dstfilename);
                            Array.Copy(motBytes, 0, outBytes, startaddr, motBytes.Length);
							File.WriteAllBytes(dstfilename, outBytes);
                        }
                        break;
					// Error
                    case "default":
						Console.WriteLine("Unsupported extension: {0}", Path.GetExtension(srcfilename));
						return;
                }
            }
			else
			{
				Console.WriteLine("Export .sa*mdl and .saanim files as if the were compiled into the binary.\n\nUsage:");
				Console.WriteLine("ExportRawBinary <source.sa1mdl or source.saanim> <key> [startaddr] [lastaddr] [destfile]\n");
				Console.WriteLine("\tkey: Binary key, such as C900000 or 8C010000.");
				Console.WriteLine("\tstartaddr: Address from where the data (e.g. first material or first MKEY) starts.");
				Console.WriteLine("\tlastaddr: Address of MOTION or the root OBJECT.");
				Console.WriteLine("\tdestfile: Output filename (write at the start address).\n");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
		}
	}
}