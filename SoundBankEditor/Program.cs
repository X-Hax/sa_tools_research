using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SoundBankEditor
{
	static partial class Program
    {
        public static BaseNote[] baseNotes;
        internal static string[] args;
        public static Form primaryForm;

        [STAThread]
        static void Main(string[] args)
        {
            Program.args = args;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            baseNotes = CreateBaseNotes();
            if (args.Length > 0)
            switch (Path.GetExtension(args[0].ToLowerInvariant()))
            {
                case ".msb":
                    primaryForm = new SequenceBankEditor();
                    break;
                case ".mpb":
                default:
                    primaryForm = new ProgramBankEditor();
                    break;
            }
            else
                primaryForm = new ProgramBankEditor();
            Application.Run(primaryForm);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (primaryForm != null)
            {
                Exception ex = (Exception)e.ExceptionObject;
                string errDesc = "SoundBank Editor has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
                    "If you wish to report a bug, please include the following in your report:";
				SAModel.SAEditorCommon.ErrorDialog report = new SAModel.SAEditorCommon.ErrorDialog("SoundBank Editor", errDesc, ex.ToString());
                DialogResult dgresult = report.ShowDialog(primaryForm);
                switch (dgresult)
                {
                    case DialogResult.Abort:
                    case DialogResult.OK:
                        Application.Exit();
                        break;
                }
            }
            else
            {
                string logPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SoundBankEditor.log";
                System.IO.File.WriteAllText(logPath, e.ExceptionObject.ToString());
                MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath + ".", "SoundBank Editor Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class BaseNote
        {
            public byte ID;
            public string Name;
            public float Frequency;

            public BaseNote(byte id, string name, float frequency)
            {
                ID = id;
                Name = name;
                Frequency = frequency;
            }
        }

        static BaseNote[] CreateBaseNotes()
        {
            List<BaseNote> result = new List<BaseNote>();
            // -2
            result.Add(new BaseNote(0, "C-2", 1411200.0f));
            result.Add(new BaseNote(1, "C#-2", 1411200.0f));
            result.Add(new BaseNote(2, "D-2", 1411200.0f));
            result.Add(new BaseNote(3, "D#-2", 1411200.0f));
            result.Add(new BaseNote(4, "E-2", 1411200.0f));
            result.Add(new BaseNote(5, "F-2", 1411200.0f));
            result.Add(new BaseNote(6, "F#-2", 1411200.0f));
            result.Add(new BaseNote(7, "G-2", 1411200.0f));
            result.Add(new BaseNote(8, "G#-2", 1411200.0f));
            result.Add(new BaseNote(9, "A-2", 1411200.0f));
            result.Add(new BaseNote(10, "A#-2", 1411200.0f));
            result.Add(new BaseNote(11, "B-2", 1411200.0f));
            // -1
            result.Add(new BaseNote(12, "C-1", 705600.0f));
            result.Add(new BaseNote(13, "C#-1", 705600.0f));
            result.Add(new BaseNote(14, "D-1", 705600.0f));
            result.Add(new BaseNote(15, "D#-1", 705600.0f));
            result.Add(new BaseNote(16, "E-1", 705600.0f));
            result.Add(new BaseNote(17, "F-1", 705600.0f));
            result.Add(new BaseNote(18, "F#-1", 705600.0f));
            result.Add(new BaseNote(19, "G-1", 705600.0f));
            result.Add(new BaseNote(20, "G#-1", 705600.0f));
            result.Add(new BaseNote(21, "A-1", 705600.0f));
            result.Add(new BaseNote(22, "A#-1", 705600.0f));
            result.Add(new BaseNote(23, "B-1", 705600.0f));
            // 0
            result.Add(new BaseNote(24, "C0", 352800.0f));
            result.Add(new BaseNote(25, "C#0", 352800.0f));
            result.Add(new BaseNote(26, "D0", 352800.0f));
            result.Add(new BaseNote(27, "D#0", 352800.0f));
            result.Add(new BaseNote(28, "E0", 352800.0f));
            result.Add(new BaseNote(29, "F0", 352800.0f));
            result.Add(new BaseNote(30, "F#0", 352800.0f));
            result.Add(new BaseNote(31, "G0", 352800.0f));
            result.Add(new BaseNote(32, "G#0", 352800.0f));
            result.Add(new BaseNote(33, "A0", 352800.0f));
            result.Add(new BaseNote(34, "A#0", 352800.0f));
            result.Add(new BaseNote(35, "B0", 352800.0f));
            // 1
            result.Add(new BaseNote(36, "C1", 176400.0f));
            result.Add(new BaseNote(37, "C#1", 176400.0f));
            result.Add(new BaseNote(38, "D1", 176400.0f));
            result.Add(new BaseNote(39, "D#1", 176400.0f));
            result.Add(new BaseNote(40, "E1", 176400.0f));
            result.Add(new BaseNote(41, "F1", 176400.0f));
            result.Add(new BaseNote(42, "F#1", 176400.0f));
            result.Add(new BaseNote(43, "G1", 176400.0f));
            result.Add(new BaseNote(44, "G#1", 176400.0f));
            result.Add(new BaseNote(45, "A1", 176400.0f));
            result.Add(new BaseNote(46, "A#1", 176400.0f));
            result.Add(new BaseNote(47, "B1", 176400.0f));
            // 2
            result.Add(new BaseNote(48, "C2", 88200.0f));
            result.Add(new BaseNote(49, "C#2", 88200.0f));
            result.Add(new BaseNote(50, "D2", 88200.0f));
            result.Add(new BaseNote(51, "D#2", 88200.0f));
            result.Add(new BaseNote(52, "E2", 88200.0f));
            result.Add(new BaseNote(53, "F2", 88200.0f));
            result.Add(new BaseNote(54, "F#2", 88200.0f));
            result.Add(new BaseNote(55, "G2", 88200.0f));
            result.Add(new BaseNote(56, "G#2", 88200.0f));
            result.Add(new BaseNote(57, "A2", 88200.0f));
            result.Add(new BaseNote(58, "A#2", 88200.0f));
            result.Add(new BaseNote(59, "B2", 88200.0f));
            // 3
            result.Add(new BaseNote(60, "C3", 44100.0f));
            result.Add(new BaseNote(61, "C#3", 44100.0f));
            result.Add(new BaseNote(62, "D3", 44100.0f));
            result.Add(new BaseNote(63, "D#3", 44100.0f));
            result.Add(new BaseNote(64, "E3", 44100.0f));
            result.Add(new BaseNote(65, "F3", 44100.0f));
            result.Add(new BaseNote(66, "F#3", 44100.0f));
            result.Add(new BaseNote(67, "G3", 44100.0f));
            result.Add(new BaseNote(68, "G#3", 44100.0f));
            result.Add(new BaseNote(69, "A3", 44100.0f));
            result.Add(new BaseNote(70, "A#3", 44100.0f));
            result.Add(new BaseNote(71, "B3", 44100.0f));
            // 4
            result.Add(new BaseNote(72, "C4", 22050.0f));
            result.Add(new BaseNote(73, "C#4", 22050.0f));
            result.Add(new BaseNote(74, "D4", 22050.0f));
            result.Add(new BaseNote(75, "D#4", 22050.0f));
            result.Add(new BaseNote(76, "E4", 22050.0f));
            result.Add(new BaseNote(77, "F4", 22050.0f));
            result.Add(new BaseNote(78, "F#4", 22050.0f));
            result.Add(new BaseNote(79, "G4", 22050.0f));
            result.Add(new BaseNote(80, "G#4", 22050.0f));
            result.Add(new BaseNote(81, "A4", 22050.0f));
            result.Add(new BaseNote(82, "A#4", 22050.0f));
            result.Add(new BaseNote(83, "B4", 22050.0f));
            // 5
            result.Add(new BaseNote(84, "C5", 11025.0f));
            result.Add(new BaseNote(85, "C#5", 11025.0f));
            result.Add(new BaseNote(86, "D5", 11025.0f));
            result.Add(new BaseNote(87, "D#5", 11025.0f));
            result.Add(new BaseNote(88, "E5", 11025.0f));
            result.Add(new BaseNote(89, "F5", 11025.0f));
            result.Add(new BaseNote(90, "F#5", 11025.0f));
            result.Add(new BaseNote(91, "G5", 11025.0f));
            result.Add(new BaseNote(92, "G#5", 11025.0f));
            result.Add(new BaseNote(93, "A5", 11025.0f));
            result.Add(new BaseNote(94, "A#5", 11025.0f));
            result.Add(new BaseNote(95, "B5", 11025.0f));
            // 6
            result.Add(new BaseNote(96, "C6", 5512.5f));
            result.Add(new BaseNote(97, "C#6", 5512.5f));
            result.Add(new BaseNote(98, "D6", 5512.5f));
            result.Add(new BaseNote(99, "D#6", 5512.5f));
            result.Add(new BaseNote(100, "E6", 5512.5f));
            result.Add(new BaseNote(101, "F6", 5512.5f));
            result.Add(new BaseNote(102, "F#6", 5512.5f));
            result.Add(new BaseNote(103, "G6", 5512.5f));
            result.Add(new BaseNote(104, "G#6", 5512.5f));
            result.Add(new BaseNote(105, "A6", 5512.5f));
            result.Add(new BaseNote(106, "A#6", 5512.5f));
            result.Add(new BaseNote(107, "B6", 5512.5f));
            // 7
            result.Add(new BaseNote(108, "C7", 2756.25f));
            result.Add(new BaseNote(109, "C#7", 2756.25f));
            result.Add(new BaseNote(110, "D7", 2756.25f));
            result.Add(new BaseNote(111, "D#7", 2756.25f));
            result.Add(new BaseNote(112, "E7", 2756.25f));
            result.Add(new BaseNote(113, "F7", 2756.25f));
            result.Add(new BaseNote(114, "F#7", 2756.25f));
            result.Add(new BaseNote(115, "G7", 2756.25f));
            result.Add(new BaseNote(116, "G#7", 2756.25f));
            result.Add(new BaseNote(117, "A7", 2756.25f));
            result.Add(new BaseNote(118, "A#7", 2756.25f));
            result.Add(new BaseNote(119, "B7", 2756.25f));
            // 8
            result.Add(new BaseNote(120, "C8", 1378.125f));
            result.Add(new BaseNote(121, "C#8", 1378.125f));
            result.Add(new BaseNote(122, "D8", 1378.125f));
            result.Add(new BaseNote(123, "D#8", 1378.125f));
            result.Add(new BaseNote(124, "E8", 1378.125f));
            result.Add(new BaseNote(125, "F8", 1378.125f));
            result.Add(new BaseNote(126, "F#8", 1378.125f));
            result.Add(new BaseNote(127, "G8", 1378.125f));

            return result.ToArray();
        }
    }
}