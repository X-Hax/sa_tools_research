using System;
using System.Collections.Generic;
using System.IO;
using SplitTools;
using System.Text;
using ArchiveLib;

namespace MLTExtract
{
    partial class Program
	{
		const int BIT_0 = (1 << 0);
		const int BIT_1 = (1 << 1);
		const int BIT_2 = (1 << 2);
		const int BIT_3 = (1 << 3);
		const int BIT_4 = (1 << 4);
		const int BIT_5 = (1 << 5);
		const int BIT_6 = (1 << 6);
		const int BIT_7 = (1 << 7);
		const int BIT_8 = (1 << 8);
		const int BIT_9 = (1 << 9);
		const int BIT_10 = (1 << 10);
		const int BIT_11 = (1 << 11);
		const int BIT_12 = (1 << 12);
		const int BIT_13 = (1 << 13);
		const int BIT_14 = (1 << 14);
		const int BIT_15 = (1 << 15);

        static void Main(string[] args)
        {
            List<string> bankfiles = new List<string>();
            if (args.Length == 0)
            {
                Console.WriteLine("This program extracts waveforms and metadata from Dreamcast MLT archives and MPB soundbanks.\n");
                Console.WriteLine("Usage: MLTExtract <file>\n");
                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();
                return;
            }
            string filename = args[0];
            string fname = Path.GetFileNameWithoutExtension(filename);
            string dir = Path.Combine(Environment.CurrentDirectory, fname);
            switch (Path.GetExtension(filename).ToLowerInvariant())
            {
                case ".mpb":
                case ".msb":
                case ".gcaxmpb":
                    ProcessBankFile(filename, "");
                    return;
                default:
                    break;
            }
            if (!File.Exists(filename))
            {
                Console.WriteLine("Error: file {0} does not exist", filename);
                Console.WriteLine("Press ENTER to exit.");
                Console.ReadLine();
                return;
            }
            byte[] file = File.ReadAllBytes(filename);
            if (Path.GetExtension(filename).ToLowerInvariant() == ".prs") file = FraGag.Compression.Prs.Decompress(file);
            Console.WriteLine("Extracting MLT file: {0}", filename);
            string hdr = System.Text.Encoding.ASCII.GetString(file, 0, 4);
            Console.WriteLine("Output folder: {0}", dir);
            Directory.CreateDirectory(dir);
            // Gamecube header
            if (hdr == "gcax")
            {
                Console.WriteLine("Gamecube MLT detected.");
                gcaxMLTFile gcaxMLT = new gcaxMLTFile(file, Path.GetFileNameWithoutExtension(filename));
                foreach (gcaxMLTFile.gcaxMLTEntry entry in gcaxMLT.Entries)
                {
                    File.WriteAllBytes(Path.Combine(dir, entry.Name), entry.Data);
                    gcaxMLT.CreateIndexFile(dir);
                    bankfiles.Add(entry.Name);
                }
            }
            else
            {
                Console.WriteLine("Assuming Dreamcast MLT.");
                MLTFile dcmlt = new MLTFile(file, Path.GetFileNameWithoutExtension(filename));
                foreach (MLTFile.MLTEntry entry in dcmlt.Entries)
                {
                    if (entry.Data == null)
                        continue;
                    File.WriteAllBytes(Path.Combine(dir, entry.Name), entry.Data);
                    dcmlt.CreateIndexFile(dir);
                    bankfiles.Add(entry.Name);
                }
            }
            foreach (string bfile in bankfiles)
            {
                ProcessBankFile(Path.Combine(dir, bfile), dir);
            }
        }

        static void ProcessBankFile(string filename, string dir)
        {
            string dir2 = Path.Combine(dir, Path.GetFileNameWithoutExtension(filename));
            switch (Path.GetExtension(filename).ToLowerInvariant())
            {
                // Program
                case ".mpb":
                    Console.WriteLine("\nProcessing Dreamcast program bank: {0}", filename);
                    Directory.CreateDirectory(dir2);
                    ProcessProgramBank(filename, dir2);
                    break;
                case ".gcaxmpb":
                    Console.WriteLine("\nProcessing Gamecube program bank: {0}", Path.GetFileNameWithoutExtension(filename));
                    Directory.CreateDirectory(dir2);
                    ProcessGamecubeProgramBank(filename, dir2);
                    break;
                // Sequence
                case ".msb":
                    Console.WriteLine("\nProcessing Dreamcast sequence bank: {0}", Path.GetFileNameWithoutExtension(filename));
                    Directory.CreateDirectory(dir2);
                    ProcessSequenceBank(filename, dir2);
                    break;
                default:
                    break;
            }
        }

        static byte[] PCM8_signed_to_unsigned(byte[] data)
		{
			List<byte> result = new List<byte>();
			for (int u = 0; u < data.Length; u++)
			{
				int r = (int)data[u] + 128;
				result.Add((byte)r);
			}
			return result.ToArray();
		}

		//All code below is taken from AicaADPCM2WAV by Sappharad https://github.com/Sappharad/AicaADPCM2WAV/
		#region AICA ADPCM decoding
		static readonly int[] diff_lookup = {
		1,3,5,7,9,11,13,15,
		-1,-3,-5,-7,-9,-11,-13,-15,
	};

		static int[] index_scale = {
		0x0e6, 0x0e6, 0x0e6, 0x0e6, 0x133, 0x199, 0x200, 0x266
	};

		private static byte[] adpcm2pcm(byte[] input, uint src, uint length)
		{
			byte[] dst = new byte[length * 4];
			int dstLoc = 0;
			int cur_quant = 0x7f;
			int cur_sample = 0;
			bool highNybble = false;

			while (dstLoc < dst.Length)
			{
				int shift1 = highNybble ? 4 : 0;
				int delta = (input[src] >> shift1) & 0xf;

				int x = cur_quant * diff_lookup[delta & 15];
				x = cur_sample + ((int)(x + ((uint)x >> 29)) >> 3);
				cur_sample = (x < -32768) ? -32768 : ((x > 32767) ? 32767 : x);
				cur_quant = (cur_quant * index_scale[delta & 7]) >> 8;
				cur_quant = (cur_quant < 0x7f) ? 0x7f : ((cur_quant > 0x6000) ? 0x6000 : cur_quant);

				dst[dstLoc++] = (byte)(cur_sample & 0xFF);
				dst[dstLoc++] = (byte)((cur_sample >> 8) & 0xFF);

				cur_sample = cur_sample * 254 / 256;

				highNybble = !highNybble;
				if (!highNybble)
				{
					src++;
				}
			}
			return dst;
		}
		#endregion

		#region WAV stuff
		public static byte[] AddWavHeader(byte[] input, uint frequency, byte bitDepth = 16)
		{
			byte[] output = new byte[input.Length + 44];
			Array.Copy(Encoding.ASCII.GetBytes("RIFF"), 0, output, 0, 4);
			WriteUint(4, (uint)output.Length - 8, output);
			Array.Copy(Encoding.ASCII.GetBytes("WAVE"), 0, output, 8, 4);
			Array.Copy(Encoding.ASCII.GetBytes("fmt "), 0, output, 12, 4);
			WriteUint(16, 16, output); //Header size
			output[20] = 1; //PCM
			output[22] = 1; //1 channel
			WriteUint(24, frequency, output); //Sample Rate
			WriteUint(28, (uint)(frequency * (bitDepth / 8)), output); //Bytes per second
			output[32] = (byte)(bitDepth >> 3); //Bytes per sample
			output[34] = bitDepth; //Bits per sample
			Array.Copy(Encoding.ASCII.GetBytes("data"), 0, output, 36, 4);
			WriteUint(40, (uint)output.Length, output); //Date size
			Array.Copy(input, 0, output, 44, input.Length);

			return output;
		}

		public static byte[] ChangeBitDepth16to32(byte[] input)
		{
			byte[] output = new byte[input.Length * 2];
			//Expand by repeating. 0x9876 becomes 0x98769876 which should be equivalent to the original amplitude.

			for (int i = 0; i < input.Length; i += 2)
			{
				output[(i * 2) + 0] = input[i];
				output[(i * 2) + 1] = input[i + 1];
				output[(i * 2) + 2] = input[i];
				output[(i * 2) + 3] = input[i + 1];
			}

			return output;
		}

		private static void WriteUint(uint offset, uint value, byte[] destination)
		{
			for (int i = 0; i < 4; i++)
			{
				destination[offset + i] = (byte)(value & 0xFF);
				value >>= 8;
			}
		}
		#endregion
	}
}