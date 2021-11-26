using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundBankEditor
{
    public partial class ProgramBankEditor : Form
    {
        public MidiProgramBank currentBank;
        public int currentProgram;
        public int currentLayer;
        public int currentSplit;
        public string currentOpenFile;
        SoundPlayer soundPlayer;
        string sizeConversion;
        List<MidiProgramVelocityCurve> velocityCurves;

        public ProgramBankEditor()
        {
            sizeConversion = "D";
            InitializeComponent();
            statusTextFilename.Text = statusTextNumPrograms.Text = statusTextVersion.Text = "";
            soundPlayer = new SoundPlayer();
            if (Program.args.Length > 0)
                LoadMidiProgramBank(Program.args[0]);
        }

        private void showSizeAsHexadecimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sizeConversion = showSizeAsHexadecimalToolStripMenuItem.Checked ? "X" : "D";
            RefreshProgramList();
            RefreshLayerList();
            RefreshSplitList();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Title = "Open MPB File", Filter = "MPB Files|*.MPB|All Files|*.*" })
                if (ofd.ShowDialog() == DialogResult.OK)
                    LoadMidiProgramBank(ofd.FileName);
        }

        private void LoadMidiProgramBank(string file)
        {
            currentBank = new MidiProgramBank(File.ReadAllBytes(file), 0);
            currentOpenFile = file;
            this.Text = "Program Bank Editor - " + Path.GetFullPath(file);
            statusTextFilename.Text = Path.GetFileName(file);
            statusTextVersion.Text = "MIDI " + currentBank.BankMode.ToString() + " Bank Version " + currentBank.Version.ToString();
            statusTextNumPrograms.Text = currentBank.Programs.Count.ToString() + " programs";
            // Load metadata
            string metapath = Path.Combine(Path.GetDirectoryName(currentOpenFile), Path.GetFileNameWithoutExtension(currentOpenFile) + ".txt");
            if (File.Exists(metapath))
            {
                string[] names = File.ReadAllLines(metapath);
                for (int i = 0; i < currentBank.Programs.Count; i++)
                    currentBank.Programs[i].Name = names[i];
            }
            velocityCurves = currentBank.Curves;
            RefreshProgramList();
        }

        private void RefreshProgramList()
        {
            listViewPrograms.Items.Clear();
            foreach (MidiProgram prg in currentBank.Programs)
                listViewPrograms.Items.Add(new ListViewItem(new[] { currentBank.Programs.IndexOf(prg).ToString(), prg.Name, prg.CountSplits().ToString(), prg.GetSize().ToString(sizeConversion) }));
            listViewPrograms.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewPrograms.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewPrograms.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewPrograms.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void RefreshLayerList()
        {
            listViewLayers.Items.Clear();
            foreach (MidiProgramLayer layer in currentBank.Programs[currentProgram].Layers)
                listViewLayers.Items.Add(new ListViewItem(new[] { currentBank.Programs[currentProgram].Layers.IndexOf(layer).ToString(), layer.BendRangeMinus.ToString(), layer.BendRangePlus.ToString(), layer.LayerDelay.ToString(), layer.CountSplits().ToString(), layer.GetSize().ToString(sizeConversion) }));
        }

        private void RefreshSplitList()
        {
            listViewSplits.Items.Clear();
            if (currentBank.Programs[currentProgram].Layers.Count < 1 || currentBank.Programs[currentProgram].Layers[currentLayer].Splits.Count < 1)
                return;
            foreach (MidiProgramSplit split in currentBank.Programs[currentProgram].Layers[currentLayer].Splits)
                listViewSplits.Items.Add(new ListViewItem(new[] { currentBank.Programs[currentProgram].Layers[currentLayer].Splits.IndexOf(split).ToString(), split.StartNote.ToString(), split.EndNote.ToString(), split.DryLevel.ToString(), split.GetPanLevel(), split.FXLevel.ToString(), split.FXChannel.ToString(), split.GetSize().ToString(sizeConversion) }));
        }

        private void listViewPrograms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPrograms.SelectedItems.Count > 0)
            {
                currentProgram = listViewPrograms.SelectedIndices[0];
                currentLayer = 0;
                currentSplit = 0;
                RefreshLayerList();
                RefreshSplitList();
            }
        }

        private void listViewLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewLayers.SelectedItems.Count > 0)
            {
                currentLayer = listViewLayers.SelectedIndices[0];
                currentSplit = 0;
                RefreshSplitList();
            }
        }

        private void listViewPrograms_DoubleClick(object sender, EventArgs e)
        {
            if (listViewPrograms.SelectedItems.Count > 0)
            {
                using (ProgramProperties prt = new ProgramProperties(currentBank.Programs[listViewPrograms.SelectedIndices[0]]))
                    if (prt.ShowDialog() == DialogResult.OK)
                    {
                        currentBank.Programs[listViewPrograms.SelectedIndices[0]].Name = prt.Result;
                        RefreshProgramList();
                    }
            }
        }

        private void listViewLayers_DoubleClick(object sender, EventArgs e)
        {
            if (listViewLayers.SelectedItems.Count > 0)
            {
                currentLayer = listViewLayers.SelectedIndices[0];
                MidiProgramLayer lr = currentBank.Programs[currentProgram].Layers[currentLayer];
                using (LayerProperties prt = new LayerProperties(lr))
                    if (prt.ShowDialog() == DialogResult.OK)
                    {
                        lr.LayerDelay = prt.resultDelay;
                        lr.BendRangeMinus = (byte)prt.resultBendRangeMinus;
                        lr.BendRangePlus = (byte)prt.resultBendRangePlus;
                        currentBank.Programs[currentProgram].Layers[currentLayer] = lr;
                        RefreshLayerList();
                    }
            }
        }

        private void listViewSplits_DoubleClick(object sender, EventArgs e)
        {
            editToolStripMenuItem_Click(sender, e);
        }

        private void buttonSplitProperties_Click(object sender, EventArgs e)
        {
            if (listViewSplits.SelectedItems.Count > 0)
            {
                currentSplit = listViewSplits.SelectedIndices[0];
                MidiProgramSplit sp = currentBank.Programs[currentProgram].Layers[currentLayer].Splits[currentSplit];
                using (SplitProperties prt = new SplitProperties(sp))
                    if (prt.ShowDialog() == DialogResult.OK)
                    {
                        sp.StartNote = prt.resultStartNote;
                        sp.EndNote = prt.resultEndNote;
                        sp.DryLevel = prt.resultDryLevel;
                        sp.DryPan = (sbyte)prt.resultDryPan;
                        sp.FXLevel = prt.resultFXLevel;
                        sp.FXChannel = prt.resultFXChannel;
                        currentBank.Programs[currentProgram].Layers[currentLayer].Splits[currentSplit] = sp;
                        RefreshSplitList();
                    }
            }
        }

        private void buttonSplitPlay_Click(object sender, EventArgs e)
        {
            PlayWaveform();
        }

        private void toolStripMenuItemPlay_Click(object sender, EventArgs e)
        {
            PlayWaveform();
        }

        private void PlayWaveform()
        {
            MidiProgramSplit split = currentBank.Programs[currentProgram].Layers[currentLayer].Splits[currentSplit];
                if (split.Waveform != null)
                {
                    soundPlayer.Stream = new MemoryStream(WaveformTools.GeneratePlayableWaveform(split.Waveform, split.BitDepth, Program.baseNotes[split.BaseNote].Frequency, split.Flags.HasFlag(WaveformFlags.ADPCM)));
                    soundPlayer.LoadAsync();
                    soundPlayer.Play();
                }
        }

        private void toolStripMenuItemExportWaveform_Click(object sender, EventArgs e)
        {
            MidiProgramSplit split = currentBank.Programs[currentProgram].Layers[currentLayer].Splits[currentSplit];
            string filename = currentBank.Programs[currentProgram].Name + "_" + currentLayer.ToString("D3") + "_" + currentSplit.ToString("D3");
            WaveformTools.ExportWaveform(split.Waveform, split.BitDepth, split.BaseNote, split.Flags.HasFlag(WaveformFlags.ADPCM), false, filename);
        }

        private void listViewSplits_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuSplit.Show((Control)sender, e.Location);
        }

        private void exportRawWaveformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MidiProgramSplit split = currentBank.Programs[currentProgram].Layers[currentLayer].Splits[currentSplit];
            string filename = currentBank.Programs[currentProgram].Name + "_" + currentLayer.ToString("D3") + "_" + currentSplit.ToString("D3");
            WaveformTools.ExportWaveform(split.Waveform, split.BitDepth, split.BaseNote, split.Flags.HasFlag(WaveformFlags.ADPCM), true, filename);
        }

        private void toolStripMenuItemProperties_Click(object sender, EventArgs e)
        {
            buttonSplitProperties_Click(sender, e);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MidiProgramSplit sp = currentBank.Programs[currentProgram].Layers[currentLayer].Splits[currentSplit];
            using (SplitEditor prt = new SplitEditor(sp, velocityCurves))
                if (prt.ShowDialog() == DialogResult.OK)
                {
                    currentBank.Programs[currentProgram].Layers[currentLayer].Splits[currentSplit] = sp;
                    RefreshSplitList();
                }
        }

        private void listViewSplits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSplits.SelectedIndices.Count > 0)
                currentSplit = listViewSplits.SelectedIndices[0];
        }

        private void buttonSplitStop_Click(object sender, EventArgs e)
        {
            soundPlayer.Stop();
        }

        private void CalculateMPBChecksum()
        {
            byte[] file = File.ReadAllBytes("D:\\cock3");
            int result = 0;
            for (int i = 0; i < 277; i++)
            {
                if (System.Text.Encoding.ASCII.GetString(file, i, 4) == "ENDB")
                    break;
                result += file[i];
            }
            result -= 51;
            MessageBox.Show(result.ToString(""));
        }

        private void buttonProgramProperties_Click(object sender, EventArgs e)
        {
            CalculateMPBChecksum();
        }
    }
}
