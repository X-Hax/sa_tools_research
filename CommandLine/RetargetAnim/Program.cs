using SAModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace RetargetAnim
{
	class Program
	{
		static void Main(string[] args)
		{
			string srcfilename;
			string dstfilename;
			string dictfilename;
			if (args.Length > 1)
			{
				srcfilename = Path.GetFullPath(args[0]);
                dictfilename = Path.GetFullPath(args[1]);
				if (args.Length > 2)
					dstfilename = Path.GetFullPath(args[2]);
				else
					dstfilename = Path.Combine(Path.GetDirectoryName(srcfilename), Path.GetFileNameWithoutExtension(srcfilename) + "_r.saanim");
                Console.WriteLine("Input file: {0}", srcfilename);
                Console.WriteLine("Node list: {0}", dictfilename);
				NJS_MOTION motion = NJS_MOTION.Load(srcfilename);
				string[] nodelist = File.ReadAllLines(dictfilename);
				Dictionary<int, AnimModelData> newanim = new Dictionary<int, AnimModelData>();
				foreach (KeyValuePair<int, AnimModelData> anim in motion.Models)
				{
					int newnodeid = int.Parse(nodelist[anim.Key]);
					Console.WriteLine("Node {0}->{1}", anim.Key, newnodeid);
                    newanim.Add(newnodeid, anim.Value);
                }
				motion.Models = newanim;
                Console.WriteLine("Output file: {0}", dstfilename);
				motion.Save(dstfilename);
            }
			else
			{
				Console.WriteLine("Retarget an animation using a node list.\n\nUsage:");
				Console.WriteLine("RetargetAnim <source.saanim> <nodelist.txt> [destination.saanim]\n");
				Console.WriteLine("The node list is a text file containing target node IDs (the whole hierarchy) separated by a newline character.\n");
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
		}
	}
}