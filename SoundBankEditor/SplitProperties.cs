using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static SoundBankEditor.ProgramBankEditor;

namespace SoundBankEditor
{
    public partial class SplitProperties : Form
    {
        public byte resultStartNote;
        public byte resultEndNote;
        public int resultDryPan;
        public byte resultDryLevel;
        public int resultFXLevel;
        public int resultFXChannel;

        public SplitProperties(MidiProgramSplit split)
        {
            InitializeComponent();
            trackBarStartNote.Value = split.StartNote;
            trackBarEndNote.Value = split.EndNote;
            trackBarDryPan.Value = split.DryPan;
            trackBarDryLevel.Value = split.DryLevel;
            trackBarFXLevel.Value = split.FXLevel;
            trackBarFXChannel.Value = split.FXChannel;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            resultStartNote = (byte)trackBarStartNote.Value;
            resultEndNote = (byte)trackBarEndNote.Value;
            resultDryPan = trackBarDryPan.Value;
            resultDryLevel = (byte)trackBarDryLevel.Value;
            resultFXLevel = trackBarFXLevel.Value;
            resultFXChannel = trackBarFXChannel.Value;
        }

        private void RefreshView()
        {
            labelStartNote.Text = trackBarStartNote.Value.ToString();
            labelEndNote.Text = trackBarEndNote.Value.ToString();
            if (trackBarDryPan.Value == 1 || trackBarDryPan.Value == 0)
                labelDryPan.Text = "Center";
            else
                labelDryPan.Text = (trackBarDryPan.Value < 0 ? "L" : "R") + Math.Abs(trackBarDryPan.Value).ToString();
            labelDryLevel.Text = trackBarDryLevel.Value.ToString();
            labelFXLevel.Text = trackBarFXLevel.Value.ToString();
            labelFXChannel.Text = trackBarFXChannel.Value.ToString();
        }

        private void trackBarStartNote_ValueChanged(object sender, EventArgs e)
        {
            RefreshView();
        }
    }
}
