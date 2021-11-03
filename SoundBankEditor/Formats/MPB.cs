using SplitTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundBankEditor
{
    public enum WaveformFlags
    {
        ADPCM = 0x01,
        Loop = 0x02,
    }

    public enum LFOFlags
    {
        Square= 0x01,
        Triangle = 0x02,
        LFOSync=0x80
    }

    public enum AmpWaveType
    {
        SawAmp = 0,
        SquareAmp = 8,
        TriangleAmp = 16,
        NoiseAmp=32,
    }

    public enum PitchWaveType
    {
        SawPitch = 0,
        SquarePitch = 1,
        TrianglePitch = 2,
        NoisePitch = 3
    }

    public class MidiProgramSplit // 48 bytes X number of splits
    {
        public int BitDepth;
        [IniIgnore]
        int jump; //0x00, | 0x80 to use 8-bit signed PCM, the rest is number of jumps by 65536 bytes in addition to Waveform data offset
        public WaveformFlags Flags; //0x01
        [IniIgnore]
        uint Waveformpointer; //0x02
        public ushort LoopStart; //0x04
        public bool DrumMode;
        [IniIgnore]
        public ushort numsamples; //0x06
        public int EnvelopeAttackRate;
        public int EnvelopeDecayRate1;
        public int EnvelopeDecayRate2;
        [IniIgnore]
        public ushort releasedecay; //0x0A, release rate 0-31 + decay level (0-31) * 32 + key rate scaling (0-15) * 1024
        public int EnvelopeReleaseRate;
        public int EnvelopeDecayLevel;
        public int KeyRateScaling;
        public byte unused2; //0x0C
        public byte unused3; //0x0D
        [IniIgnore]
        //LFO shit
        byte lfopitchvalue; //0x0E, 0 to 7 miltiplied by 32 + amp lf0 depth (0 to 7) + amp lfo type (amp lfo wave: saw 0, square + 8, triangle + 16, noise + 24)
        public bool LFOSyncOn;
        public int LFOFrequency;
        public int LFOPitchDepth;
        public PitchWaveType LFOPitchWave;
        public int LFOAmpDepth;
        public AmpWaveType LFOAmpWave;
        [IniIgnore]
        byte lfo_pitchstuff; //0x0F, 0 to 31 multiplied by 4 + pitch lfo type (pitch lfo wave: saw 0, square + 1, triangle + 2, noise + 3)
        [IniIgnore]
        public byte fxlevelchannel; //0x10, & 0xF to get FX channel, the rest is FX level
        [IniAlwaysInclude]
        public int FXLevel;
        [IniAlwaysInclude]
        public int FXChannel;
        public byte eleven; //0x11
        public int DryPan; //0x12, L15 through C to R15 (actual byte calculated differently in MPB/MDB v.1 and v.2)
        public byte DryLevel; //0x13, 0 to 15
        public byte TotalLevel; //0x15, 0 to 127
        public int FilterResonance; //0x14, 0 to 31, add | 0x20 to disable filtering
        public bool FilterDisable;
        public ushort FilterStartLev; //0x16, 0 to 8184
        public ushort FilterAttackLev; //0x18, 0 to 8184
        public ushort FilterDecayFilterLev1; //0x1A, 0 to 8184
        public ushort FilterDecayFilterLev2; //0x1C, 0 to 8184
        public ushort FilterReleaseFilterLev; //0x1E, 0 to 8184
        public byte FilterDecayRate1; //0x20, 0 to 31
        public byte FilterAttackRate; //0x21, 0 to 31
        public byte FilterReleaseRate; //0x22, 0 to 31
        public byte FilterDecayRate2; //0x23, 0 to 31
        [IniAlwaysInclude]
        public byte StartNote; //0x24, 0 to 127
        [IniAlwaysInclude]
        public byte EndNote; //0x25, 0 to 127
        [IniAlwaysInclude]
        public byte BaseNote; //0x26, 0 to 127
        public int FineTune; //0x27, -64 to 63 multiplied by 2
        public byte VelocityCurveID; //0x2A
        public byte VelocityLow; //0x2B, 0 to 127
        public byte VelocityHigh; //0x2C, 0 to 127
        [IniName("Unknown1")]
        public byte unknown1;
        [IniName("Unknown2")]
        public byte unknown2;
        public byte unused4; //0x2D
        public byte unused5; //0x2E
        public byte unused6; //0x2F
        [IniIgnore]
        public byte[] Waveform;

        public MidiProgramSplit(byte[] file, int address, int id_program, int id_layer, int id_split, int MPBVersion = 1)
        {
            Console.WriteLine("\t\tSplit at {0} ({1})", address.ToString("X"), id_split);
            jump = file[address];
            Flags = (WaveformFlags)file[address + 1];
            Waveformpointer = BitConverter.ToUInt16(file, address + 0x02) + 65536 * ((uint)jump & 0xF);
            LoopStart = BitConverter.ToUInt16(file, address + 0x04);
            numsamples = BitConverter.ToUInt16(file, address + 0x06);
            // Envelope Decay Rate 1/2, Attack Rate
            ushort attackdecay = BitConverter.ToUInt16(file, address + 0x08); //0x08, attack rate 0-31 + decay1 rate (0-31) * 64 + decay2 rate (0-31) * 2048
            EnvelopeDecayRate2 = attackdecay / 2048;
            EnvelopeDecayRate1 = (attackdecay % 2048) / 64;
            EnvelopeAttackRate = attackdecay - EnvelopeDecayRate2 * 2048 - EnvelopeDecayRate1 * 64;

            // Envelope Decay Level, Release Rate, Key Scaling
            ushort releasedecay = BitConverter.ToUInt16(file, address + 0x0A); //0x0A, release rate 0-31 + decay level (0-31) * 32 + key rate scaling (0-15) * 1024
            KeyRateScaling = releasedecay / 1024;
            EnvelopeDecayLevel = (releasedecay % 1024) / 32;
            EnvelopeReleaseRate = releasedecay - KeyRateScaling * 1024 - EnvelopeDecayLevel * 32;

            Console.WriteLine("\t\t\tSamples: {0}, Waveform at: {1}, jump: {2}, loop: {3}", numsamples, Waveformpointer.ToString("X"), jump.ToString("X"), LoopStart.ToString("X"));
            unused2 = file[address + 0x0C];
            unused3 = file[address + 0x0D];
            // LFO Frequency, Amp
            lfopitchvalue = file[address + 0x0E]; //0x0E, 0 to 7 miltiplied by 32 + amp lf0 depth (0 to 7) + amp lfo type (amp lfo wave: saw 0, square + 8, triangle + 16, noise + 24)
            LFOPitchDepth = lfopitchvalue / 32;
            LFOAmpDepth = (lfopitchvalue - LFOPitchDepth * 32) % 8; // ALFOS
            LFOAmpWave = (AmpWaveType)(lfopitchvalue - LFOPitchDepth * 32 - LFOAmpDepth); // ALFOWS
            // LFO
            lfo_pitchstuff = file[address + 0x0F]; //0x0F, 0 to 31 multiplied by 4 + pitch lf0 type (pitch lfo wave: saw 0, square + 1, triangle + 2, noise + 3) + 0x80 if LFO Sync is on
            LFOSyncOn = ((LFOFlags)lfo_pitchstuff).HasFlag(LFOFlags.LFOSync);
            if (LFOSyncOn)
                lfo_pitchstuff -= 0x80;
            LFOFrequency = lfo_pitchstuff / 4;
            LFOPitchWave = (PitchWaveType)(lfo_pitchstuff - LFOFrequency * 4); // PLFOWS
            fxlevelchannel = file[address + 0x10]; // & 0xF to get FX channel, the rest is FX level
            FXLevel = fxlevelchannel >> 4;
            FXChannel = fxlevelchannel & 0xF;
            eleven = file[address + 0x11];
            // Calculate Pan level depending on version
            uint drypan_raw = file[address + 0x12];
            switch (MPBVersion)
            {
                case 2:
                    if (drypan_raw >= 32)
                        DryPan = (int)drypan_raw - 31;
                    else
                        DryPan = 16 - (int)drypan_raw;
                    break;
                case 1:
                    if (drypan_raw >= 16)
                        DryPan = 16 - (int)drypan_raw;
                    else
                        DryPan = (int)drypan_raw;
                    break;
                default:
                    break;
            }
            DryLevel = file[address + 0x13];// 0 to 15
            FilterResonance = file[address + 0x14];// 0 to 31, add | 0x20 to disable filtering
            if ((FilterResonance & 0x20) > 0) FilterDisable = true;
            FilterResonance = FilterResonance % 0x20;
            TotalLevel = file[address + 0x15];// 0 to 127
            FilterStartLev = file[address + 0x16];// 0 to 8184
            FilterAttackLev = file[address + 0x18];// 0 to 8184
            FilterDecayFilterLev1 = file[address + 0x1A];// 0 to 8184
            FilterDecayFilterLev2 = file[address + 0x1C];// 0 to 8184
            FilterReleaseFilterLev = file[address + 0x1E];// 0 to 8184
            FilterDecayRate1 = file[address + 0x20];// 0 to 31
            FilterAttackRate = file[address + 0x21];// 0 to 31
            FilterReleaseRate = file[address + 0x22];// 0 to 31
            FilterDecayRate2 = file[address + 0x23];// 0 to 31
            StartNote = file[address + 0x24];// 0 to 127
            EndNote = file[address + 0x25];// 0 to 127
            BaseNote = file[address + 0x26];// 0 to 127
            FineTune = (sbyte)(file[address + 0x27]) / 2;// -64 to 63 multiplied by 2
            double ass = ((100.0 * (BaseNote - 69)) + FineTune) / 1200.0;
            double frq = 440.0 * Math.Pow(2.0f, ass);
            //Console.WriteLine("Freq: {0}", frq);
            unknown1 = file[address + 0x28];
            unknown2 = file[address + 0x29];
            VelocityCurveID = file[address + 0x2A];// 
            VelocityLow = file[address + 0x2B];// 0 to 127
            VelocityHigh = file[address + 0x2C];// 0 to 127
            DrumMode = file[address + 0x2D] == 255;
            unused5 = file[address + 0x2E];
            unused6 = file[address + 0x2F];
            Console.WriteLine("\t\t\tStart/End: {0}/{1}, Direct: {2}, Pan: 0x{3}, FX: {4}, FXCh: {5}", StartNote, EndNote, DryLevel, drypan_raw.ToString("X"), FXLevel, FXChannel);
            Console.WriteLine("\t\t\tUnknown values: {0}, {1}, {2}, {3}, {4}, {5}, {6}", unknown1, unknown2, unused2, unused3, DrumMode, unused5, unused6);
            if (Flags.HasFlag(WaveformFlags.ADPCM)) BitDepth = 4;
            else if ((jump >> 4) == 8) BitDepth = 8;
            else BitDepth = 16;
            int pcmbytes = (numsamples / 8) * BitDepth;
            if (numsamples != 65535 && pcmbytes > 0 && Waveformpointer + pcmbytes < file.Length)
            {
                Waveform = new byte[pcmbytes];
              
                Array.Copy(file, Waveformpointer, Waveform, 0, pcmbytes);
               
                Console.WriteLine("\t\t\tWaveform data: {0} bytes, flags: {1}", pcmbytes, Flags.ToString());
            }
            else
            {
                Console.WriteLine("\t\t\tMissing/invalid Waveform data, flags: {0}, numsamples: {1}", Flags.ToString(), numsamples.ToString());
            }
        }

        public string GetPanLevel()
        {
            if (DryPan == 1 || DryPan == 0)
                return "Center " + DryPan.ToString();
            else
                return (DryPan < 0 ? "L" : "R") + Math.Abs(DryPan).ToString();
        }

        public MidiProgramSplit()
        { }

        public int GetSize()
        {
            if (Waveform != null)
                return 48 + Waveform.Length;
            else return 48;
        }
    }

    public struct MidiProgramLayer // 16 bytes X 4
    {
        [IniIgnore]
        uint splitcount;
        [IniIgnore]
        uint splitpointer;
        public uint LayerDelay; // 0 to 256 (compressed range from 0 to 1024)
        public byte BendRangePlus; // 0 to 24
        public byte BendRangeMinus;  // 0 to 24
        [IniName("Unknown")]
        public ushort unk;
        [IniIgnore]
        public List<MidiProgramSplit> Splits;

        public MidiProgramLayer(byte[] file, int address, int id_program, int id_layer, int MPBVersion = 1)
        {
            Console.WriteLine("\tLayer at {0} ({1})", address.ToString("X"), id_layer);
            splitcount = BitConverter.ToUInt32(file, address);
            splitpointer = BitConverter.ToUInt32(file, address + 4);
            LayerDelay = BitConverter.ToUInt32(file, address + 8) * 4;
            BendRangePlus = file[address + 0xC];
            BendRangeMinus = file[address + 0xD];
            unk = BitConverter.ToUInt16(file, address + 0xE);
            Splits = new List<MidiProgramSplit>();
            for (int s = 0; s < splitcount; s++)
            {
                MidiProgramSplit spl = new MidiProgramSplit(file, (int)splitpointer + 48 * s, id_program, id_layer, s, MPBVersion);
                Splits.Add(spl);
                //IniSerializer.Serialize(spl, Path.Combine(dir, "P" + id_program.ToString("D3") + "_L" + id_layer.ToString("") + "_" + s.ToString("D3") + ".ini"));
            }
        }

        public int CountSplits()
        {
            return Splits.Count;
        }

        public int GetSize()
        {
            int result = 16;
            foreach (MidiProgramSplit split in Splits)
                result += split.GetSize();
            return result;
        }
    }

    public class MidiProgram // 4 bytes X number of sounds
    {
        public string Name;
        public List<MidiProgramLayer> Layers;
        public MidiProgram(byte[] file, int address, int id_program, int MPBVersion=1)
        {
            Name = "Program " + id_program.ToString();
            Console.WriteLine("\n--- Program at {0} ({1})---", address.ToString("X"), id_program);
            uint layersmainpointer = BitConverter.ToUInt32(file, address);
            Layers = new List<MidiProgramLayer>();
            for (int i = 0; i < 4; i++)
            {
                uint layerpointer = BitConverter.ToUInt32(file, (int)layersmainpointer + 4 * i);
                if (layerpointer == 0) continue;
                MidiProgramLayer lr = new MidiProgramLayer(file, (int)layerpointer, id_program, i, MPBVersion);
                Layers.Add(lr);
                //IniSerializer.Serialize(lr, Path.Combine(dir, "P" + id_program.ToString("D3") + "_L" + i.ToString() + ".ini"));
            }
            Console.WriteLine("--- End of Program ---", address.ToString("X"));
        }

        public int CountSplits()
        {
            int result = 0;
            foreach (MidiProgramLayer lr in Layers)
                result += lr.CountSplits();
            return result;
        }

        public int GetSize()
        {
            int result = 4;
            foreach (MidiProgramLayer layer in Layers)
                result += layer.GetSize();
            return result;
        }
    }

    public class MidiProgramVelocityCurve
    {
        [IniName("Point")]
        [IniCollection(IniCollectionMode.NoSquareBrackets)]
        public byte[] curvedata;

        public MidiProgramVelocityCurve(byte[] file, int address, int id_curve)
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

        public MidiProgramVelocityCurve()
        {
            curvedata = new byte[128];
        }

        public MidiProgramVelocityCurve(byte[] data)
        {
            curvedata = new byte[128];
            Array.Copy(data, curvedata, 128);
        }
    }
    public enum ProgramBankMode
    {
        Program,
        Drum
    }

    public class MidiProgramBank
    {
        string header; //SMPB
        public ProgramBankMode BankMode;
        public int Version;
        uint filesize;
        public List<MidiProgram> Programs;
        int program_count;
        public List<MidiProgramVelocityCurve> Curves;
        int curve_count;
        int ptrs1;
        int ptrs1_count;
        int ptrs2;
        int ptrs2_count;

        public MidiProgramBank(byte[] file, int address)
        {
            header = System.Text.Encoding.ASCII.GetString(file, address, 4);
            BankMode = header == "SMDB" ? ProgramBankMode.Drum : ProgramBankMode.Program;
            Version = BitConverter.ToInt32(file, address + 4);
            filesize = BitConverter.ToUInt32(file, address + 8);
            int programs_ptr = BitConverter.ToInt32(file, address + 0x10);
            program_count = BitConverter.ToInt32(file, address + 0x14);
            int curves_ptr = BitConverter.ToInt32(file, address + 0x18);
            curve_count = BitConverter.ToInt32(file, address + 0x1C);
            ptrs1 = BitConverter.ToInt32(file, address + 0x20);
            ptrs1_count = BitConverter.ToInt32(file, address + 0x24);
            ptrs2 = BitConverter.ToInt32(file, address + 0x28);
            ptrs2_count = BitConverter.ToInt32(file, address + 0x2C);
            Console.WriteLine("{0} v.{1}, size {2}, prg at {3}, prgs: {4}, cur at {5}, curs: {6}, p1 at {7}, p1s: {8}, p2 at {9}, p2s: {10}", header, Version, filesize, programs_ptr.ToString("X"), program_count, curves_ptr.ToString("X"), curve_count, ptrs1.ToString("X"), ptrs1_count, ptrs2.ToString("X"), ptrs2_count);
            Curves = new List<MidiProgramVelocityCurve>();
            for (int c = 0; c < curve_count; c++)
            {
                MidiProgramVelocityCurve curve = new MidiProgramVelocityCurve(file, curves_ptr + 128 * c, c);
                Curves.Add(curve);
                //IniSerializer.Serialize(curve, Path.Combine(dir, "CURVE" + c.ToString("D2") + ".ini"));
            }
            Programs = new List<MidiProgram>();
            for (int i = 0; i < program_count; i++)
            {
                Programs.Add(new MidiProgram(file, programs_ptr + 4 * i, i, Version));
            }
        }
    }
}
