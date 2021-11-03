using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace SoundBankEditor
{
    public partial class SplitEditor : Form
    {
        byte[] toneDataOriginal;
        SoundPlayer soundPlayer;
        public List<MidiProgramVelocityCurve> velocityCurves;
        int bitDepthOriginal;
        MidiProgramSplit resultSplit;

        public SplitEditor(MidiProgramSplit split, List<MidiProgramVelocityCurve> velocityCurvesOriginal)
        {
            InitializeComponent();
            velocityCurves = velocityCurvesOriginal;
            soundPlayer = new SoundPlayer();
            bitDepthOriginal = split.BitDepth;
            toneDataOriginal = split.Waveform;
            trackBarBaseNote.Value = split.BaseNote;
            checkBoxADPCM.Checked = split.Flags.HasFlag(WaveformFlags.ADPCM);
            checkBoxLoop.Checked = numericUpDownLoopStart.Enabled = labelLoopStart.Enabled = split.Flags.HasFlag(WaveformFlags.Loop);
            trackBarFineTune.Value = split.FineTune;
            trackBarTotalLevel.Value = split.TotalLevel;
            trackBarVelocityLow.Value = split.VelocityLow;
            trackBarVelocityHigh.Value = split.VelocityHigh;
            comboBoxVelocityCurve.Items.Clear();
            for (int u = 0; u<velocityCurvesOriginal.Count; u++)
                comboBoxVelocityCurve.Items.Add("Curve " + u.ToString());
            comboBoxVelocityCurve.SelectedIndex = split.VelocityCurveID;
            checkBoxLFOSync.Checked = split.LFOSyncOn;
            trackBarLFOFrequency.Value = split.LFOFrequency;
            trackBarPitchLFODepth.Value = split.LFOPitchDepth;
            comboBoxPitchLFOWave.SelectedIndex = (int)split.LFOPitchWave;
            trackBarAmpLFODepth.Value = split.LFOAmpDepth;
            checkBoxFilterOn.Checked = !split.FilterDisable;
            trackBarStartFilterLev.Value = split.FilterStartLev;
            trackBarFilterAttackRate.Value = split.FilterAttackRate;
            trackBarAttackFilterLev.Value = split.FilterAttackLev;
            trackBarFilterDecayRate1.Value = split.FilterDecayRate1;
            trackBarDecayFilterLev1.Value = split.FilterDecayFilterLev1;
            trackBarFilterDecayRate2.Value = split.FilterDecayRate2;
            trackBarDecayFilterLev2.Value = split.FilterDecayFilterLev2;
            trackBarFilterReleaseRate.Value = split.FilterReleaseRate;
            trackBarReleaseFilterLev.Value = split.FilterReleaseFilterLev;
            trackBarResonance.Value = split.FilterResonance;
            trackBarEnvelopeAttackRate.Value = split.EnvelopeAttackRate;
            trackBarEnvelopeDecayRate1.Value = split.EnvelopeDecayRate1;
            trackBarEnvelopeDecayLevel.Value = split.EnvelopeDecayLevel; 
            trackBarEnvelopeDecayRate2.Value = split.EnvelopeDecayRate2;
            trackBarEnvelopeReleaseRate.Value = split.EnvelopeReleaseRate;
            trackBarEnvelopeKeyRateScaling.Value = split.KeyRateScaling;
            numericUpDownLoopStart.Value = split.LoopStart;
            switch (split.LFOAmpWave)
            {
                case AmpWaveType.SawAmp:
                    comboBoxAmpLFOWave.SelectedIndex = 0;
                    break;
                case AmpWaveType.SquareAmp:
                    comboBoxAmpLFOWave.SelectedIndex = 1;
                    break;
                case AmpWaveType.TriangleAmp:
                    comboBoxAmpLFOWave.SelectedIndex = 2;
                    break;
                case AmpWaveType.NoiseAmp:
                    comboBoxAmpLFOWave.SelectedIndex = 3;
                    break;
            }
            UpdateAllLabels();
        }

        private void UpdateAllLabels()
        {
            if (toneDataOriginal == null)
                labelWaveformInfo.Text = "No Waveform";
            else
                labelWaveformInfo.Text = toneDataOriginal.Length.ToString() + " frames, " + bitDepthOriginal.ToString() + " bits, " + Program.baseNotes[trackBarBaseNote.Value].Frequency.ToString() + " hz";
            labelBaseNoteInfo.Text = trackBarBaseNote.Value.ToString() + " / " + Program.baseNotes[trackBarBaseNote.Value].Name;
            UpdateLabelFromTrackbar(labelFineTune, trackBarFineTune);
            UpdateLabelFromTrackbar(labelTotalLevel, trackBarTotalLevel);
            UpdateLabelFromTrackbar(labelVelocityLow, trackBarVelocityLow);
            UpdateLabelFromTrackbar(labelVelocityHigh, trackBarVelocityHigh);
            UpdateLabelFromTrackbar(labelLFOFrequency, trackBarLFOFrequency);
            UpdateLabelFromTrackbar(labelPitchLFODepth, trackBarPitchLFODepth);
            UpdateLabelFromTrackbar(labelAmpLFODepth, trackBarAmpLFODepth);
            UpdateLabelFromTrackbar(labelStartFilterLev, trackBarStartFilterLev);
            UpdateLabelFromTrackbar(labelFilterAttackRate, trackBarFilterAttackRate);
            UpdateLabelFromTrackbar(labelAttackFilterLev, trackBarAttackFilterLev);
            UpdateLabelFromTrackbar(labelFilterDecayRate1, trackBarFilterDecayRate1);
            UpdateLabelFromTrackbar(labelDecayFilterLev1, trackBarDecayFilterLev1);
            UpdateLabelFromTrackbar(labelFilterDecayRate2, trackBarFilterDecayRate2);
            UpdateLabelFromTrackbar(labelDecayFilterLev2, trackBarDecayFilterLev2);
            UpdateLabelFromTrackbar(labelFilterReleaseRate, trackBarFilterReleaseRate);
            UpdateLabelFromTrackbar(labelReleaseFilterLev, trackBarReleaseFilterLev);
            UpdateLabelFromTrackbar(labelResonance, trackBarResonance);
            UpdateLabelFromTrackbar(labelEnvelopeAttackRate, trackBarEnvelopeAttackRate);
            UpdateLabelFromTrackbar(labelEnvelopeDecayRate1, trackBarEnvelopeDecayRate1);
            UpdateLabelFromTrackbar(labelEnvelopeDecayLevel, trackBarEnvelopeDecayLevel);
            UpdateLabelFromTrackbar(labelEnvelopeDecayRate2, trackBarEnvelopeDecayRate2);
            UpdateLabelFromTrackbar(labelEnvelopeReleaseRate, trackBarEnvelopeReleaseRate);
            UpdateLabelFromTrackbar(labelEnvelopeKeyRateScaling, trackBarEnvelopeKeyRateScaling);
            DrawEnvelope();
            DrawFilter();
        }

        public void UpdateLabelFromTrackbar(Label text, TrackBar trackbar)
        {
            text.Text = trackbar.Value.ToString();
        }

        private void trackBarBaseNote_ValueChanged(object sender, EventArgs e)
        {
            UpdateAllLabels();
        }

        private void buttonWaveformPlay_Click(object sender, EventArgs e)
        {
            PlayWaveform();
        }


        private void PlayWaveform()
        {
            if (toneDataOriginal != null)
            {
                soundPlayer.Stream = new MemoryStream(WaveformTools.GeneratePlayableWaveform(toneDataOriginal, bitDepthOriginal, Program.baseNotes[trackBarBaseNote.Value].Frequency, checkBoxADPCM.Checked));
                soundPlayer.LoadAsync();
                soundPlayer.Play();
            }
        }

        private void DrawFilter()
        {
            pictureBoxFilter.Image = new Bitmap(310, 116);
            using (Graphics gfx = Graphics.FromImage(pictureBoxFilter.Image))
            {
                Color penColor = checkBoxFilterOn.Checked ? Color.Black : Color.LightGray;
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                Pen drawPen = new Pen(penColor, 2.0f);
                Pen drawPenSoft = new Pen(penColor, 1.0f);
                
                // Graph axis: Left to right
                gfx.DrawLine(drawPen, 0, 108, 302, 108);
                // Graph axis: Top to bottom
                gfx.DrawLine(drawPen, 8, 8, 8, 116);

                // Filter Start Lev
                float originX = 16;
                float originY = 16 + (8184 - trackBarStartFilterLev.Value) / 95.16f;
                DrawRect(gfx, originX, originY, !checkBoxFilterOn.Checked);

                // Filter Attack Rate
                float filterAttackRateBoxX = 16 + scaleRange(0, 31, 0, 67.5f, 31 - trackBarFilterAttackRate.Value);
                float filterAttackRateBoxY = 16 + (8184 - trackBarAttackFilterLev.Value) / 95.16f;
                DrawRect(gfx, filterAttackRateBoxX, filterAttackRateBoxY, !checkBoxFilterOn.Checked);

                // Decay Rate 1 / Filter Lev 1
                float filterDecayRate1X = filterAttackRateBoxX + scaleRange(0, 31, 0, 67.5f, 31 - trackBarFilterDecayRate1.Value);
                float filterDecayRate1Y = 16 + (8184 - trackBarDecayFilterLev1.Value) / 95.16f;
                DrawRect(gfx, filterDecayRate1X, filterDecayRate1Y, !checkBoxFilterOn.Checked);

                // Decay Rate 2 / Filter Lev 2
                float filterDecayRate2X = filterDecayRate1X + scaleRange(0, 31, 0, 67.5f, 31 - trackBarFilterDecayRate2.Value);
                float filterDecayRate2Y = 16 + (8184 - trackBarDecayFilterLev2.Value) / 95.16f;
                DrawRect(gfx, filterDecayRate2X, filterDecayRate2Y, !checkBoxFilterOn.Checked);

                // Release Rate
                float filterReleaseRateX = filterDecayRate2X + scaleRange(0, 31, 0, 67.5f, 31 - trackBarFilterReleaseRate.Value);
                float filterReleaseRateY = 16 + (8184 - trackBarReleaseFilterLev.Value) / 95.16f;
                DrawRect(gfx, filterReleaseRateX, filterReleaseRateY, !checkBoxFilterOn.Checked);

                // Connect lines
                gfx.DrawLine(drawPenSoft, filterAttackRateBoxX, filterAttackRateBoxY, originX, originY);
                gfx.DrawLine(drawPenSoft, filterDecayRate1X, filterDecayRate1Y, filterAttackRateBoxX, filterAttackRateBoxY);
                gfx.DrawLine(drawPenSoft, filterDecayRate2X, filterDecayRate2Y, filterDecayRate1X, filterDecayRate1Y);
                gfx.DrawLine(drawPenSoft, filterReleaseRateX, filterReleaseRateY, filterDecayRate2X, filterDecayRate2Y);
            }
        }

        private void DrawEnvelope()
        {
            pictureBoxEnvelope.Image = new Bitmap(228, 116);
            // Draw graph lines first
            using (Graphics gfx = Graphics.FromImage(pictureBoxEnvelope.Image))
            {

                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                Pen drawPen = new Pen(Color.Black, 2.0f);
                Pen drawPenSoft = new Pen(Color.Black, 1.0f);

                // Graph axis: Left to right
                gfx.DrawLine(drawPen, 0, 108, 228, 108);
                // Graph axis: Top to bottom
                gfx.DrawLine(drawPen, 8, 8, 8, 116);
                float originX = 16;
                float originY = 100;

                // Attack Rate is first 32x4
                float attackRateBoxX = 16 + scaleRange(0, 31, 0, 67.5f, 31 - trackBarEnvelopeAttackRate.Value);
                DrawRect(gfx, attackRateBoxX, 16);

                // Decay Rate 1 is second 32x4, Decay Level is vertically free from 0-31
                float decayRate1MaxX = 16 + 135;
                float decayRate1MinX = attackRateBoxX;
                float decayLevelY = 16 + scaleRange(0, 31, 0, 86, trackBarEnvelopeDecayLevel.Value);
                float decayRate1CurrentX = scaleRange(0, 31, attackRateBoxX, decayRate1MaxX, 31-trackBarEnvelopeDecayRate1.Value);
                DrawRect(gfx, decayRate1CurrentX, decayLevelY);

                // Decay Rate 2 is squished with Decay Rate
                float decayRate2MaxY = 16 + 86;
                float decayRate2CurrentY = scaleRange(0, 31, decayLevelY, decayRate2MaxY, trackBarEnvelopeDecayRate2.Value);
                DrawRect(gfx, decayRate1MaxX, decayRate2CurrentY);

                // Release rate is third 32x4
                float releaseRateX = decayRate1MaxX + scaleRange(0, 31, 0, 67.5f, 31 - trackBarEnvelopeReleaseRate.Value);
                DrawRect(gfx, releaseRateX, 16 + 86);

                // Connect lines
                gfx.DrawLine(drawPenSoft, attackRateBoxX, 16, originX, originY);
                gfx.DrawLine(drawPenSoft, attackRateBoxX, 16, decayRate1CurrentX, decayLevelY);
                gfx.DrawLine(drawPenSoft, decayRate1CurrentX, decayLevelY, decayRate1MaxX, decayRate2CurrentY);
                gfx.DrawLine(drawPenSoft, decayRate1MaxX, decayRate2CurrentY, releaseRateX, 16+86);
            }
        }

        private void DrawRect(Graphics gfx, float x, float y, bool disabled = false)
        {
            Pen drawPen = new Pen(disabled ? Color.LightGray : Color.Black, 2.0f);
            SolidBrush drawBrush = new SolidBrush(disabled ? Color.LightGray : Color.Black);
            Rectangle rectAttackRate = new Rectangle((int)(x - 2), (int)(y-2), 4, 4);
            gfx.FillRectangle(drawBrush, rectAttackRate);
            gfx.DrawRectangle(drawPen, rectAttackRate);
        }

        public float scaleRange(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
        {

            float OldRange = (OldMax - OldMin);
            float NewRange = (NewMax - NewMin);
            float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

            return (NewValue);
        }

        private void checkBoxFilterOn_CheckedChanged(object sender, EventArgs e)
        {
            DrawFilter();
        }

        private void buttonEditVelocityCurve_Click(object sender, EventArgs e)
        {
            using (VelocityCurveEditor velEditor = new VelocityCurveEditor(velocityCurves, comboBoxVelocityCurve.SelectedIndex))
            {
                if (velEditor.ShowDialog() == DialogResult.OK)
                    velocityCurves = velEditor.resultCurves;
            }
        }

        private void buttonWaveformSave_Click(object sender, EventArgs e)
        {
            string filename = "Waveform";
            WaveformTools.ExportWaveform(toneDataOriginal, bitDepthOriginal, trackBarBaseNote.Value, checkBoxADPCM.Checked, false, filename);
        }

        private void buttonWaveformLoad_Click(object sender, EventArgs e)
        {
            
        }

        private void checkBoxLoop_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownLoopStart.Enabled = labelLoopStart.Enabled = checkBoxLoop.Checked;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            resultSplit = new MidiProgramSplit();
        }

        private void buttonWaveformStop_Click(object sender, EventArgs e)
        {
            soundPlayer.Stop();
        }
    }
}
