using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTExtract
{
    partial class Program
    {
        [Flags]
        public enum WaveformFlags
        {
            FLAG_ADPCM = BIT_0,
            FLAG_LOOP = BIT_1,
        }

        public enum AmpWaveType
        {
            TYPE_SAW = 0,
            TYPE_SQUARE = 8,
            TYPE_TRIANGLE = 16,
        }

        public enum PitchWaveType
        {
            TYPE_SAW = 0,
            TYPE_SQUARE = 1,
            TYPE_TRIANGLE = 2,
        }

        public class BankSplit //48 bytes X number of splits
        {
            [IniName("BitDepth")]
            public int bitdepth;
            [IniIgnore]
            public int jump; //0x00, | 0x80 to use 8-bit signed PCM, the rest is number of jumps by 65536 bytes in addition to waveform data offset
            [IniName("Flags")]
            public WaveformFlags flags; //0x01
            [IniIgnore]
            public uint waveformpointer; //0x02
            [IniName("LoopStart")]
            public ushort loopstart; //0x04
            [IniIgnore]
            public ushort numsamples; //0x06
            [IniName("AttackRate")]
            public int attackrate;
            [IniName("DecayRate1")]
            public int decayrate1;
            [IniName("DecayRate2")]
            public int decayrate2;
            [IniIgnore]
            public ushort attackdecay; //0x08, attack rate 0-31 + decay1 rate (0-31) * 64 + decay2 rate (0-31) * 2048
            [IniIgnore]
            public ushort releasedecay; //0x0A, release rate 0-31 + decay level (0-31) * 32 + key rate scaling (0-15) * 1024
            [IniName("ReleaseRate")]
            public int releaserate;
            [IniName("DecayLevel")]
            public int decaylevel;
            [IniName("KeyRateScaling")]
            public int keyratescaling;
            public byte unused2; //0x0C
            public byte unused3; //0x0D
            [IniIgnore]
            public byte lf0_pitch; //0x0E, 0 to 7 miltiplied by 32 + amp lf0 depth (0 to 7) + amp lf0 type (amp lf0 wave: saw 0, square + 8, triangle + 16)
            [IniName("LF0_Frequency")]
            public int lf0frequency;
            [IniName("LF0_PitchDepth")]
            public int pitchdepth;
            [IniName("LF0_PitchLF0Wave")]
            PitchWaveType pitchtype;
            [IniName("LF0_AmpDepth")]
            public int ampdepth;
            [IniName("LF0_AmpLF0Wave")]
            AmpWaveType amptype;
            [IniIgnore]
            public byte lf0_freq; //0x0F, 0 to 31 multiplied by 4 + pitch lf0 type (pitch lf0 wave: saw 0, square + 1, triangle + 2)
            [IniIgnore]
            public byte fxlevelchannel; //0x10, & 0xF to get FX channel, the rest is FX level
            [IniAlwaysInclude]
            [IniName("FX_Level")]
            public int fxlevel;
            [IniAlwaysInclude]
            [IniName("FX_Channel")]
            public int fxchannel;
            public byte eleven; //0x11
            [IniName("DryPan")]
            public int drypan; //0x12, L15 through C to R15 (-15 to 15)
            [IniName("DryLevel")]
            public byte drylevel; //0x13, 0 to 15
            [IniName("TotalLevel")]
            public byte totallevel; //0x15, 0 to 127
            [IniName("Filter_Resonance")]
            public int resonance; //0x14, 0 to 31, add | 0x20 to disable filtering
            [IniName("Filter_Disable")]
            public bool nofilter;
            [IniName("Filter_StartLev")]
            public ushort startfilter; //0x16, 0 to 8184
            [IniName("Filter_FilterLev")]
            public ushort attackfilter; //0x18, 0 to 8184
            [IniName("Filter_DecayFilterLev1")]
            public ushort decay1filter; //0x1A, 0 to 8184
            [IniName("Filter_DecayFilterLev2")]
            public ushort decay2filter; //0x1C, 0 to 8184
            [IniName("Filter_ReleaseFilterLev")]
            public ushort releasefilter; //0x1E, 0 to 8184
            [IniName("Filter_DecayRate1")]
            public byte decay1; //0x20, 0 to 31
            [IniName("Filter_AttackRate")]
            public byte attack; //0x21, 0 to 31
            [IniName("Filter_ReleaseRate")]
            public byte release; //0x22, 0 to 31
            [IniName("Filter_DecayRate2")]
            public byte decay2; //0x23, 0 to 31
            [IniAlwaysInclude]
            [IniName("Start")]
            public byte start; //0x24, 0 to 127
            [IniAlwaysInclude]
            [IniName("End")]
            public byte end; //0x25, 0 to 127
            [IniAlwaysInclude]
            [IniName("BaseNote")]
            public byte basenote; //0x26, 0 to 127
            [IniName("FineTune")]
            public int finetune; //0x27, -64 to 63 multiplied by 2
            [IniName("VelocityCurveID")]
            public byte velocitycurve; //0x2A
            [IniName("VelocityLow")]
            public byte velocitylow; //0x2B, 0 to 127
            [IniName("VelocityHigh")]
            public byte velocityhigh; //0x2C, 0 to 127
            [IniName("Unknown1")]
            public byte unknown1;
            [IniName("Unknown2")]
            public byte unknown2;
            public byte unused4; //0x2D
            public byte unused5; //0x2E
            public byte unused6; //0x2F
            [IniIgnore]
            byte[] waveform;

            public BankSplit(byte[] file, int address, int id_program, int id_layer, int id_split, string dir)
            {
                Console.WriteLine("\t\tSplit at {0} ({1})", address.ToString("X"), id_split);
                jump = file[address];
                flags = (WaveformFlags)file[address + 1];
                waveformpointer = BitConverter.ToUInt16(file, address + 0x02) + 65536 * ((uint)jump & 0xF);
                loopstart = BitConverter.ToUInt16(file, address + 0x04);
                numsamples = BitConverter.ToUInt16(file, address + 0x06);
                attackdecay = BitConverter.ToUInt16(file, address + 0x08);
                decayrate2 = attackdecay / 2048;
                decayrate1 = (attackdecay % 2048) / 64;
                attackrate = attackdecay - decayrate2 * 2048 - decayrate1 * 64;
                //0x08, attack rate 0-31 + decay1 rate (0-31) * 64 + decay2 rate (0-31) * 2048
                releasedecay = BitConverter.ToUInt16(file, address + 0x0A);
                //0x0A, release rate 0-31 + decay level (0-31) * 32 + key rate scaling (0-15) * 1024
                keyratescaling = releasedecay / 1024;
                decaylevel = (releasedecay % 1024) / 32;
                releaserate = releasedecay - keyratescaling * 1024 - decaylevel * 32;
                Console.WriteLine("\t\t\tSamples: {0}, waveform at: {1}, jump: {2}, loop: {3}", numsamples, waveformpointer.ToString("X"), jump.ToString("X"), loopstart.ToString("X"));
                unused2 = file[address + 0x0C];
                unused3 = file[address + 0x0D];
                lf0_pitch = file[address + 0x0E]; //0x0E, 0 to 7 miltiplied by 32 + amp lf0 depth (0 to 7) + amp lf0 type (amp lf0 wave: saw 0, square + 8, triangle + 16)
                lf0frequency = lf0_pitch / 32;
                ampdepth = (lf0_pitch - lf0frequency * 32) % 8;
                amptype = (AmpWaveType)(lf0_pitch - lf0frequency * 32 - ampdepth);
                lf0_freq = file[address + 0x0F]; //0x0F, 0 to 31 multiplied by 4 + pitch lf0 type (pitch lf0 wave: saw 0, square + 1, triangle + 2)
                pitchdepth = lf0_freq / 4;
                pitchtype = (PitchWaveType)(pitchdepth - pitchdepth);
                fxlevelchannel = file[address + 0x10]; // & 0xF to get FX channel, the rest is FX level
                fxlevel = fxlevelchannel >> 4;
                fxchannel = fxlevelchannel & 0xF;
                eleven = file[address + 0x11];
                drypan = file[address + 0x12];// L15 through C to R15 (-15 to 15)
                drylevel = file[address + 0x13];// 0 to 15
                resonance = file[address + 0x14];// 0 to 31, add | 0x20 to disable filtering
                if ((resonance & 0x20) > 0) nofilter = true;
                resonance = resonance % 0x20;
                totallevel = file[address + 0x15];// 0 to 127
                startfilter = file[address + 0x16];// 0 to 8184
                attackfilter = file[address + 0x18];// 0 to 8184
                decay1filter = file[address + 0x1A];// 0 to 8184
                decay2filter = file[address + 0x1C];// 0 to 8184
                releasefilter = file[address + 0x1E];// 0 to 8184
                decay1 = file[address + 0x20];// 0 to 31
                attack = file[address + 0x21];// 0 to 31
                release = file[address + 0x22];// 0 to 31
                decay2 = file[address + 0x23];// 0 to 31
                start = file[address + 0x24];// 0 to 127
                end = file[address + 0x25];// 0 to 127
                basenote = file[address + 0x26];// 0 to 127
                finetune = (sbyte)(file[address + 0x27]) / 2;// -64 to 63 multiplied by 2
                double ass = ((100.0 * (basenote - 69)) + finetune) / 1200.0;
                double frq = 440.0 * Math.Pow(2.0f, ass);
                Console.WriteLine("Freq: {0}", frq);
                unknown1 = file[address + 0x28];
                unknown2 = file[address + 0x29];
                velocitycurve = file[address + 0x2A];// 
                velocitylow = file[address + 0x2B];// 0 to 127
                velocityhigh = file[address + 0x2C];// 0 to 127
                unused4 = file[address + 0x2D];
                unused5 = file[address + 0x2E];
                unused6 = file[address + 0x2F];
                if (flags.HasFlag(WaveformFlags.FLAG_ADPCM)) bitdepth = 4;
                else if ((jump >> 4) == 8) bitdepth = 8;
                else bitdepth = 16;
                int pcmbytes = (numsamples / 8) * bitdepth;
                if (pcmbytes > 0 && waveformpointer + pcmbytes < file.Length)
                {
                    waveform = new byte[pcmbytes];
                    Array.Copy(file, waveformpointer, waveform, 0, pcmbytes);
                    Console.WriteLine("\t\t\tWaveform data: {0} bytes, flags: {1}", pcmbytes, flags.ToString());
                    if (flags.HasFlag(WaveformFlags.FLAG_ADPCM))
                    {
                        waveform = adpcm2pcm(waveform, 0, (uint)pcmbytes);
                        waveform = AddWavHeader(waveform, 22050, 16);
                    }
                    else
                    {
                        if (bitdepth == 8) waveform = PCM8_signed_to_unsigned(waveform);
                        waveform = AddWavHeader(waveform, 22050, (byte)bitdepth);
                    }
                    File.WriteAllBytes(Path.Combine(dir, "P" + id_program.ToString("D3") + "_L" + id_layer.ToString() + "_" + id_split.ToString("D3") + ".wav"), waveform);
                }
                else
                {
                    Console.WriteLine("\t\t\tMissing/invalid waveform data, flags: {0}", flags.ToString());
                }
            }
        }

        public struct BankLayer //16 bytes X 4
        {
            [IniIgnore]
            public uint splitcount;
            [IniIgnore]
            public uint splitpointer;
            [IniName("LayerDelay")]
            public uint layerdelay; //0 to 256 (compressed range from 0 to 1024)
            [IniName("BendRangePlus")]
            public byte bendrangeplus; //0 to 24
            [IniName("BendRangeMinus")]
            public byte bendrangeminus;  //0 to 24
            [IniName("Unknown")]
            public ushort unk;
            [IniIgnore]
            List<BankSplit> splits;

            public BankLayer(byte[] file, int address, int id_program, int id_layer, string dir)
            {
                Console.WriteLine("\tLayer at {0} ({1})", address.ToString("X"), id_layer);
                splitcount = BitConverter.ToUInt32(file, address);
                splitpointer = BitConverter.ToUInt32(file, address + 4);
                layerdelay = BitConverter.ToUInt32(file, address + 8) * 4;
                bendrangeplus = file[address + 0xC];
                bendrangeminus = file[address + 0xD];
                unk = BitConverter.ToUInt16(file, address + 0xE);
                splits = new List<BankSplit>();
                for (int s = 0; s < splitcount; s++)
                {
                    BankSplit spl = new BankSplit(file, (int)splitpointer + 48 * s, id_program, id_layer, s, dir);
                    splits.Add(spl);
                    IniSerializer.Serialize(spl, Path.Combine(dir, "P" + id_program.ToString("D3") + "_L" + id_layer.ToString("") + "_" + s.ToString("D3") + ".ini"));
                }
            }
        }

        public class BankProgram //4 bytes X number of sounds
        {
            List<BankLayer> layers;
            public BankProgram(byte[] file, int address, int id_program, string dir)
            {
                Console.WriteLine("\n--- Program at {0} ({1})---", address.ToString("X"), id_program);
                uint layersmainpointer = BitConverter.ToUInt32(file, address);
                layers = new List<BankLayer>();
                for (int i = 0; i < 4; i++)
                {
                    uint layerpointer = BitConverter.ToUInt32(file, (int)layersmainpointer + 4 * i);
                    if (layerpointer == 0) continue;
                    BankLayer lr = new BankLayer(file, (int)layerpointer, id_program, i, dir);
                    layers.Add(lr);
                    IniSerializer.Serialize(lr, Path.Combine(dir, "P" + id_program.ToString("D3") + "_L" + i.ToString() + ".ini"));
                }
                Console.WriteLine("--- End of Program ---", address.ToString("X"));
            }
        }

        public class BankVelocityCurve
        {
            [IniName("Point")]
            [IniCollection(IniCollectionMode.NoSquareBrackets)]
            public byte[] curvedata;
            public BankVelocityCurve(byte[] file, int address, int id_curve)
            {
                Console.WriteLine("\n--- Velocity Curve at {0} ({1}) ---", address.ToString("X"), id_curve);
                Console.Write("Data: ");
                curvedata = new byte[128];
                for (int i = 0; i < 128; i++)
                {
                    curvedata[i] = file[address + i];
                    Console.Write(curvedata[i] + " ");
                }
                Console.Write(System.Environment.NewLine);
                Console.WriteLine("--- End of Velocity Curve ---", address.ToString("X"), id_curve);
            }
        }

        public class BankBank
        {
            string header; //SMPB
            int version;
            uint filesize;
            List<BankProgram> programs;
            int program_count;
            List<BankVelocityCurve> curves;
            int curve_count;
            int ptrs1;
            int ptrs1_count;
            int ptrs2;
            int ptrs2_count;
            public BankBank(byte[] file, int address, string dir)
            {
                header = System.Text.Encoding.ASCII.GetString(file, address, 4);
                version = BitConverter.ToInt32(file, address + 4);
                filesize = BitConverter.ToUInt32(file, address + 8);
                int programs_ptr = BitConverter.ToInt32(file, address + 0x10);
                program_count = BitConverter.ToInt32(file, address + 0x14);
                int curves_ptr = BitConverter.ToInt32(file, address + 0x18);
                curve_count = BitConverter.ToInt32(file, address + 0x1C);
                ptrs1 = BitConverter.ToInt32(file, address + 0x20);
                ptrs1_count = BitConverter.ToInt32(file, address + 0x24);
                ptrs2 = BitConverter.ToInt32(file, address + 0x28);
                ptrs2_count = BitConverter.ToInt32(file, address + 0x2C);
                Console.WriteLine("{0} v.{1}, size {2}, prg at {3}, prgs: {4}, cur at {5}, curs: {6}, p1 at {7}, p1s: {8}, p2 at {9}, p2s: {10}", header, version, filesize, programs_ptr.ToString("X"), program_count, curves_ptr.ToString("X"), curve_count, ptrs1.ToString("X"), ptrs1_count, ptrs2.ToString("X"), ptrs2_count);
                curves = new List<BankVelocityCurve>();
                for (int c = 0; c < curve_count; c++)
                {
                    BankVelocityCurve curve = new BankVelocityCurve(file, curves_ptr + 128 * c, c);
                    curves.Add(curve);
                    IniSerializer.Serialize(curve, Path.Combine(dir, "CURVE" + c.ToString("D2") + ".ini"));
                }
                programs = new List<BankProgram>();
                for (int i = 0; i < program_count; i++)
                {
                    programs.Add(new BankProgram(file, programs_ptr + 4 * i, i, dir));
                }
            }
        }
        public class BankSequence
        {
            string header; //SMSB
            int version;
            uint filesize;
            uint sequencedata;
            //List<BankSequenceData> sequences;
            int sequence_count;
            public BankSequence(byte[] file, int address, string dir)
            {
                header = System.Text.Encoding.ASCII.GetString(file, address, 4);
                version = BitConverter.ToInt32(file, address + 4);
                filesize = BitConverter.ToUInt32(file, address + 8);
                sequence_count = BitConverter.ToInt32(file, address + 0xC);
                Console.WriteLine("{0} v.{1}, size {2}, sequences: {3}", header, version, filesize, sequence_count);
                for (int s = 0; s < sequence_count; s++)
                {
                    sequencedata = BitConverter.ToUInt32(file, address + 0x10 + 4 * s);
                    Console.WriteLine("Sequence at {0} ({1})", sequencedata.ToString("X"), s);
                }
            }
        }

        static void ProcessSequenceBank(string filename, string dir)
        {
            byte[] file = File.ReadAllBytes(filename);
            BankSequence bnk = new BankSequence(file, 0, dir);
        }

        static void ProcessProgramBank(string filename, string dir)
        {
            byte[] file = File.ReadAllBytes(filename);
            BankBank bnk = new BankBank(file, 0, dir);
        }
    }
}
