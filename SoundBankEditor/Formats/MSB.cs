using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundBankEditor
{
    public enum MidiSequenceCommand
    {
        NoteOff = 0x80, // 2nd byte: note ID, 3rd byte: velocity
        NoteOn = 0x0, // 0x90 in real MIDI (2nd byte: note ID, 3rd byte: velocity)
        PolyphonicAfterTouch = 0xA, // Note ID, pressure
        ControlChange = 0xB0, // Need a separate enum for these
        ProgramChange = 0xC0,
        ChannelAfterTouch = 0xD0,
        PitchBendChange = 0xE0,
        Meta = 0xF0
    }

    public class MidiSequenceData
    {
        public byte[] DataBytes;
        public MidiSequenceData(byte[] file, uint address)
        {
            List<byte> resultData = new List<byte>();
            for (uint i = address; i < file.Length - 4; i++)
            {
                string readstring = System.Text.Encoding.ASCII.GetString(file, (int)i, 4);
                if (readstring == "ENDD")
                {
                    resultData.Add((byte)'E');
                    resultData.Add((byte)'N');
                    resultData.Add((byte)'D');
                    resultData.Add((byte)'D');
                    break;
                }
                resultData.Add(file[i]);
            }
            DataBytes = resultData.ToArray();
        }
    }

    public class MidiSequenceBank
    {
        string header; //SMSB
        int version;
        uint filesize;
        uint sequencedata;
        public List<MidiSequenceData> Sequences;
        int sequence_count;

        public MidiSequenceBank(byte[] file, int address, string dir)
        {
            header = System.Text.Encoding.ASCII.GetString(file, address, 4);
            version = BitConverter.ToInt32(file, address + 4);
            filesize = BitConverter.ToUInt32(file, address + 8);
            sequence_count = BitConverter.ToInt32(file, address + 0xC);
            Console.WriteLine("{0} v.{1}, size {2}, sequences: {3}", header, version, filesize, sequence_count);
            Sequences = new List<MidiSequenceData>();
            for (int s = 0; s < sequence_count; s++)
            {
                sequencedata = BitConverter.ToUInt32(file, address + 0x10 + 4 * s);
                Sequences.Add(new MidiSequenceData(file, sequencedata));
                Console.WriteLine("Sequence at {0} ({1})", sequencedata.ToString("X"), s);
            }
            // Extract sequences
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            foreach (MidiSequenceData seq in Sequences)
            {
                File.WriteAllBytes(Path.Combine(dir, dir+"_"+Sequences.IndexOf(seq).ToString("D3") + ".MSD"), seq.DataBytes);
            }
        }

        public static MidiSequenceBank LoadMSB(string msbFile)
        {
            return new MidiSequenceBank(File.ReadAllBytes(msbFile), 0, Path.GetFileNameWithoutExtension(msbFile));
        }
    }
}
