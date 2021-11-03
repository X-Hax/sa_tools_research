using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundBankEditor
{
    public class WaveformTools
    {

        public static byte[] PCM8_signed_to_unsigned(byte[] data)
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

        public static byte[] adpcm2pcm(byte[] input, uint src, uint length)
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

        public static byte[] GeneratePlayableWaveform(byte[] toneData, int bitDepth, float frequency, bool adpcm)
        {
            byte[] result = new byte[toneData.Length];
            Array.Copy(toneData, 0, result, 0, toneData.Length);
            if (adpcm)
            {
                result = adpcm2pcm(result, 0, (uint)result.Length);
                result = AddWavHeader(result, (uint)frequency, 16);
            }
            else
            {
                if (bitDepth == 8) result = WaveformTools.PCM8_signed_to_unsigned(result);
                result = AddWavHeader(result, (uint)frequency, (byte)bitDepth);
                //File.WriteAllBytes(Path.Combine(dir, "P" + id_program.ToString("D3") + "_L" + id_layer.ToString() + "_" + id_split.ToString("D3") + ".wav"), Waveform);
            }
            return result;
        }

        public static void ExportWaveform(byte[] toneData, int bitDepth, int note, bool adpcm, bool raw, string defFilename)
        {
            string extension = raw ? "BIN" : "WAV";
            string filename = defFilename + "." + extension;
            using (SaveFileDialog sfd = new SaveFileDialog { DefaultExt = "wav", Filter = extension + " files|*." + extension, FileName = filename })
            {
                DialogResult res = sfd.ShowDialog();
                if (res == DialogResult.OK)
                    File.WriteAllBytes(sfd.FileName, raw ? toneData : GeneratePlayableWaveform(toneData, bitDepth, Program.baseNotes[note].Frequency, adpcm));
            }
        }

        #endregion

    }
}
