using SAModel;
using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VGAudio;

namespace MLTExtract
{
    partial class Program
    {
        static byte[] WaveformRawData;
        static List<GamecubeMidiProgramSample> SampleList;

        public enum GamecubeMPBChunkType
        {
            Waveform, // gcaxMPBW
            Program  // gcaxMPBP
        }

        public class GamecubeMPBChunk
        {
            public GamecubeMPBChunkType Type;
            public byte[] Data;

            public GamecubeMPBChunk(byte[] file, int index, GamecubeMPBChunkType type, int size)
            {
                Type = type;
                Data = new byte[size];
                Array.Copy(file, index, Data, 0, size);
            }
        }

        public class GamecubeMidiProgramBank
        {
            public GamecubeMidiProgramBank(byte[] file, string dir)
            {
                ByteConverter.BigEndian = true;
                int size = ByteConverter.ToInt32(file, 0xC);
                //Console.WriteLine("Data size: {0}", size);
                byte[] data = new byte[size];
                Array.Copy(file, 16, data, 0, size);
                List<GamecubeMPBChunk> chunks = new List<GamecubeMPBChunk>();
                int startbyte = 0;
                Console.WriteLine("Gamecube MPB Info:");
                for (int i = 0; i < 128; i++)
                {
                    startbyte += 0xC;
                    if (startbyte >= data.Length)
                        break;
                    int chunksize = ByteConverter.ToInt32(data, startbyte);
                    byte[] chunk = new byte[chunksize];
                    Array.Copy(data, startbyte + 4, chunk, 0, chunksize);
                    GamecubeMPBChunkType type;
                    string ext = System.Text.Encoding.ASCII.GetString(data, startbyte - 0xC, 8);
                    switch (ext)
                    {
                        case "gcaxMPBW":
                            type = GamecubeMPBChunkType.Waveform;
                            break;
                        case "gcaxMPBP":
                        default:
                            type = GamecubeMPBChunkType.Program;
                            break;
                    }
                    chunks.Add(new GamecubeMPBChunk(chunk, 0, type, chunksize));
                    Console.WriteLine("Chunk type {0} at {1}, size {2}", type.ToString(), (startbyte + 4).ToString("X"), chunksize);
                    File.WriteAllBytes(Path.Combine(dir, Path.GetFileName(dir) + "."+ext.Substring(4)), chunk);
                    startbyte += chunksize + 4;
                }
                foreach (GamecubeMPBChunk chunk in chunks)
                {
                    switch (chunk.Type)
                    {
                        case GamecubeMPBChunkType.Waveform:
                            WaveformRawData = chunk.Data;
                            break;
                        case GamecubeMPBChunkType.Program:
                            ProcessGamecubeMidiPrograms(chunk.Data, dir);
                            break;
                    }
                }
            }
        }

        public class GamecubeMidiProgramSample
        {
            public uint SampleCount; // 0x0
            public uint NibbleCount; // 0x4
            public uint SampleRate; // 0x8
            public bool LoopEnabled; // 0xC, short (1 if true)
            public ushort Format; // 0xE, always 0?
            public uint LoopStart; // 0x10
            public uint LoopEnd; // 0x14
            public uint CurrentAddress; // 0x18
            public short[] Coefficients; // 0x1C, 16 shorts
            public ushort Gain; // 0x3C, always 0?
            public ushort InitialScale; // 0x3E "Initial predictor/scale; always matches first frame header"?
            //0x40 to 0x4B unused?
            public uint WaveformPointer; // 0x4C

            public GamecubeMidiProgramSample(byte[] file, int address)
            {
                SampleCount = ByteConverter.ToUInt32(file, address);
                NibbleCount = ByteConverter.ToUInt32(file, address + 4);
                SampleRate = ByteConverter.ToUInt32(file, address + 8) / 2;
                short loopFlag = ByteConverter.ToInt16(file, address + 0xC);
                if (loopFlag == 1)
                    LoopEnabled = true;
                Format = ByteConverter.ToUInt16(file, address + 0xE);
                LoopStart = ByteConverter.ToUInt32(file, address + 0x10);
                LoopEnd = ByteConverter.ToUInt32(file, address + 0x14);
                CurrentAddress = ByteConverter.ToUInt32(file, address + 0x18);
                List<short> coeffs = new List<short>();
                for (int c = 0; c < 16; c++)
                    coeffs.Add(ByteConverter.ToInt16(file, address + 0x1C + c * 2));
                Coefficients = coeffs.ToArray();
                Gain = ByteConverter.ToUInt16(file, address + 0x3C);
                InitialScale = ByteConverter.ToUInt16(file, address + 0x3E);
                WaveformPointer = ByteConverter.ToUInt32(file, address + 0x4C);
                // ADPCM only
                if (SampleCount != 0)
                {
                    Console.WriteLine("\t\tADPCM Sample Count: {0}, Nibble Count: {1}", SampleCount.ToString(), NibbleCount.ToString());
                    string coeffstring = "";
                    foreach (short co in coeffs)
                    {
                        coeffstring += co.ToString("X4") + " ";
                    }
                    Console.WriteLine("\t\tCoeffs: {0}", coeffstring);
                }
                else
                    Console.WriteLine("\t\tPCM");
                Console.WriteLine("\t\tSample Rate: {0}, Waveform at: {1}", SampleRate.ToString(), WaveformPointer.ToString("X"));
            }
        }

        public class GamecubeMidiProgramSplit
        {
            uint WaveformID; // 0x0
            ushort Unknown; // 0x4 maybe
            byte StartNote; // 0x6
            byte EndNote; // 0x7
            // 0x8-0x9 unknown 0x00 0x7F
            byte DirectLevel; // 0xA maybe
            byte FXLevel; // 0xB maybe
            byte BaseNote; // 0xC maybe
            byte FineTune; // 0xD, multiplied by 2
            ushort Flags; // 0xE-0xF unknown
            byte EnvAttackRate; // 0x10
            byte EnvDecayRate1; // 0x11
            byte EnvDecayLevel; // 0x12
            byte EnvDecayRate2; // 0x13
            byte EnvReleaseRate; // 0x14
            byte EnvKeyRateScaling; // 0x15

            byte[] Waveform;

            public GamecubeMidiProgramSplit(byte[] file, int address, string dir, int prg_id, int layer_id, int split_id)
            {
                WaveformID = ByteConverter.ToUInt32(file, address);
                Console.WriteLine("\t\t\t\t\t Split at {0}, Wave ID: {1}", address.ToString("X"), WaveformID);
                GamecubeMidiProgramSample sample = SampleList[Math.Min((int)WaveformID, SampleList.Count-1)];
                uint endAddr = (WaveformID < SampleList.Count - 1) ? SampleList[(int)WaveformID + 1].WaveformPointer : (uint)WaveformRawData.Length;
                uint length = endAddr - sample.WaveformPointer;
                if (sample.SampleCount != 0) // ADPCM
                    length = sample.NibbleCount / 2;
                Waveform = new byte[length];
                Array.Copy(WaveformRawData, sample.WaveformPointer, Waveform, 0, length);
                string waveFilename = Path.Combine(dir, "P" + prg_id.ToString("D3") + "_L" + layer_id.ToString("") + "_" + split_id.ToString("D3") + ".wav");

                // Save waveform
                byte[] result;
                if (sample.SampleCount != 0) // ADPCM
                {
                    Waveform = DecodeGamecubeADPCM(Waveform, sample.Coefficients);
                    result = AddWavHeader(Waveform, sample.SampleRate, 16);
                }
                else
                    result = AddWavHeader(ReverseWaveFormEndian(Waveform), sample.SampleRate, 16);
                File.WriteAllBytes(waveFilename, result);
            }
        }

        public class GamecubeMidiProgramLayer
        {
            // 16 bytes
            ushort NumSplits; // 0x0
            ushort SplitsPointer; //0x2
            byte BendRangePlus; // 0x4
            byte BendRangeMinus; // 0x5
            byte LayerDelay; // 0x6
            // The rest is unknown/unused
            List<GamecubeMidiProgramSplit> Splits;

            public GamecubeMidiProgramLayer(byte[] file, int index, string dir, int prg_id, int layer_id)
            {
                //Console.WriteLine("Start: {0}", index.ToString("X"));
                NumSplits = file[index];
                Splits = new List<GamecubeMidiProgramSplit>(NumSplits);
                SplitsPointer = ByteConverter.ToUInt16(file, index + 2);
                BendRangePlus = file[index + 4];
                BendRangeMinus = file[index + 5];
                LayerDelay= file[index + 6];
                string unknownbytes = "";
                for (int i = 0; i < 9; i++)
                    unknownbytes += file[index + 7 + i].ToString();
                Console.WriteLine("\t\t\t Layer at {0}, splits at {1}, split count {2}", index.ToString("X"), SplitsPointer.ToString("X"), NumSplits);
                Console.WriteLine("\t\t\t\t BendPlus: {0}, BendMinus: {1}, Delay: {2}, Unknown bytes: {3}", BendRangePlus.ToString(), BendRangeMinus.ToString(), LayerDelay.ToString(), unknownbytes);
                for (int s = 0; s < NumSplits; s++)
                {
                    Splits.Add(new GamecubeMidiProgramSplit(file, SplitsPointer + 48 * s, dir, prg_id, layer_id, s));
                }
            }
        }

        public class GamecubeMidiProgram
        {
            // 4 bytes
            public List<GamecubeMidiProgramLayer> Layers;
            public GamecubeMidiProgram(byte[] file, int index, string dir, int prg_id)
            {
                Layers = new List<GamecubeMidiProgramLayer>();
                int numlayers = file[index];
                int layerpointer = ByteConverter.ToInt16(file, index+2);
                Console.WriteLine("\t\t Layer list at {0}, count {1}", layerpointer.ToString("X"), numlayers);
                for (int l = 0; l < numlayers; l++)
                    Layers.Add(new GamecubeMidiProgramLayer(file, layerpointer + 16 * l, dir, prg_id, l));
            }
        }

        public static void SaveWaveform(GamecubeMidiProgramSample meta, int waveDataLength, string filename)
        {
            byte[] result;
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            byte[] wavedata = new byte[waveDataLength];
            Array.Copy(WaveformRawData, meta.WaveformPointer, wavedata, 0, waveDataLength);
            if (meta.SampleCount != 0) // ADPCM
            {
                wavedata = DecodeGamecubeADPCM(wavedata, meta.Coefficients);
                result = AddWavHeader(wavedata, meta.SampleRate, 16);
            }
            else
                result = AddWavHeader(ReverseWaveFormEndian(wavedata), meta.SampleRate, 16);
            File.WriteAllBytes(filename, result);
            //File.WriteAllBytes(Path.ChangeExtension(filename, null), wavedata);
        }

        public static byte[] ReverseWaveFormEndian(byte[] bigend)
        {
            List<byte> result = new List<byte>();
            for (int v = 0; v < bigend.Length/2; v += 1)
            {
                ushort value = ByteConverter.ToUInt16(bigend, v*2);
                result.AddRange(BitConverter.GetBytes(value));
            }
            return result.ToArray();
        }

        public static void ProcessGamecubeMidiPrograms(byte[] file, string dir)
        {
            SampleList = new List<GamecubeMidiProgramSample>();
            int sampleCount = file[0];
            // 0x1 is unknown
            int samplesPointer = ByteConverter.ToInt16(file, 0x2);
            int numCurves = file[0x4]; // Maybe
            int velocityPointer = ByteConverter.ToInt16(file, 0x6);
            uint programCount = file[0x8];
            int programsPointer = ByteConverter.ToInt16(file, 0xA);
            byte unknown9 = file[0x9];
            uint unknownC = ByteConverter.ToUInt32(file, 0xC);

            Console.WriteLine("Samples at: {0} (count: {1}, Programs at: {2} (count: {3}), Velocity at {4} (count: {5}), Unknown9: {6}, UnknownC: {7}", samplesPointer.ToString("X"), sampleCount, programsPointer.ToString("X"), programCount, velocityPointer.ToString("X"), numCurves, unknown9.ToString(), unknownC.ToString("X"));
            
            // Save velocity curves
            for (int v = 0; v < numCurves; v++)
            {
                BankVelocityCurve curve = new BankVelocityCurve(file, velocityPointer, v);
                IniSerializer.Serialize(curve, Path.Combine(dir, "CURVE" + v.ToString("D2") + ".ini"));
            }

            // Build sample list
            Console.WriteLine("Samples Header:");
            for (int u = 0; u < sampleCount; u++)
            {
                int waveDataAddr = samplesPointer + u * 80;
                Console.WriteLine("\tSample {0} at {1}", u, waveDataAddr.ToString("X"));
                SampleList.Add(new GamecubeMidiProgramSample(file, waveDataAddr));
            }
          
            // Build programs list
            Console.WriteLine("\nPrograms Header:");
            for (int i = 0; i < programCount; i++)
            {
                Console.WriteLine("\tProgram {0} at {1}", i, (programsPointer + 4 * i).ToString("X"));
                GamecubeMidiProgram prg = new GamecubeMidiProgram(file, programsPointer + 4 * i, dir, i);
                //SaveWaveform(SampleList[prg.)
            }
        }

        public static void ProcessGamecubeProgramBank(string filename, string dir)
        {
            byte[] file = File.ReadAllBytes(filename);
            GamecubeMidiProgramBank bank = new GamecubeMidiProgramBank(file, dir);
        }

        public static byte[] DecodeGamecubeADPCM(byte[] data, short[] coeffs)
        {
            short[] decoded = VGAudio.Codecs.GcAdpcm.GcAdpcmDecoder.Decode(data, coeffs);
            List<byte> result = new List<byte>();
            for (int a = 0; a < decoded.Length; a++)
            {
                result.AddRange(BitConverter.GetBytes(decoded[a]));
            }
            return result.ToArray();
        }

    }
}
