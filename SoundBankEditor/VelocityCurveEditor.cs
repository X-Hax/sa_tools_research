using SplitTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SoundBankEditor
{
    public partial class VelocityCurveEditor : Form
    {
        public List<MidiProgramVelocityCurve> resultCurves;

        public VelocityCurveEditor(List<MidiProgramVelocityCurve> curves, int index)
        {
            InitializeComponent();
            resultCurves = new List<MidiProgramVelocityCurve>();
            foreach (MidiProgramVelocityCurve curveOriginal in curves)
                resultCurves.Add(new MidiProgramVelocityCurve(curveOriginal.curvedata));
            comboBoxCurve.Items.Clear();
            for (int i = 0; i < curves.Count; i++)
                comboBoxCurve.Items.Add("Curve " + i.ToString());
            comboBoxCurve.SelectedIndex = index;
            // Curve Y values are normally from 0 to 127, but some files such as E_0001 have them out of range
            trackBarStartPoint.Value = (int)Math.Min((uint)127, (uint)resultCurves[comboBoxCurve.SelectedIndex].curvedata[0]);
            trackBarEndPoint.Value = (int)Math.Min((uint)127, (uint)resultCurves[comboBoxCurve.SelectedIndex].curvedata[127]);
            DrawCurve();
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            labelStartPoint.Text = trackBarStartPoint.Value.ToString();
            labelEndPoint.Text = trackBarEndPoint.Value.ToString();
        }

        private void DrawCurve()
        {
            Bitmap result = new Bitmap(416, 160);
            using (Graphics gfx = Graphics.FromImage(result))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                Pen drawPen = new Pen(Color.Black, 2.0f);
                Pen drawPenGreen = new Pen(Color.Green, 2.0f);
                Pen drawPenRed = new Pen(Color.Red, 2.0f);
                Pen drawPenSoft = new Pen(Color.Black, 1.0f);

                // Draw axis: left to right
                gfx.DrawLine(drawPen, 0, 128+16, 384+16, 128+16);
                // Draw axis: top to bottom
                gfx.DrawLine(drawPen, 16, 0, 16, 160);

                for (int l = 0; l < 127; l++)
                {
                    Point p1 = new Point(16+l*3, 128 - resultCurves[comboBoxCurve.SelectedIndex].curvedata[l] + 16);
                    Point p2 = new Point(16 + l *3 + 3, 128 - resultCurves[comboBoxCurve.SelectedIndex].curvedata[l + 1] + 16);
                    gfx.DrawLine(drawPenSoft, p1, p2);
                }
                pictureBoxCurve.Image = result;
            }
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            List<byte> curvePoints = new List<byte>();
            // Divide by zero
            if (numericUpDownMultiplier.Value == 0)
            {
                for (int x = 0; x < 128; x++)
                    curvePoints.Add(0);
                resultCurves[comboBoxCurve.SelectedIndex].curvedata = curvePoints.ToArray();
                DrawCurve();
                return;
            }
            for (int x = 0; x < 128; x++)
            {
                int cx = x - (int)numericUpDownAddX.Value;
                int y = -1* (cx * cx) / (int)numericUpDownMultiplier.Value + (int)numericUpDownAddY.Value;
                if (y > 0 && y < 128)
                    curvePoints.Add((byte)y);
                else if ((int)numericUpDownMultiplier.Value > 0)
                    curvePoints.Add(0);
                else
                    curvePoints.Add(127);
            }
            if (checkBoxOverrideStartEnd.Checked)
            {
                curvePoints[0] = (byte)trackBarStartPoint.Value;
                curvePoints[127] = (byte)trackBarEndPoint.Value;
            }
            resultCurves[comboBoxCurve.SelectedIndex].curvedata = curvePoints.ToArray();
            DrawCurve();
        }

        private void checkBoxOverrideStartEnd_CheckedChanged(object sender, EventArgs e)
        {
            trackBarStartPoint.Enabled = labelStartPointText.Enabled = labelStartPoint.Enabled = trackBarEndPoint.Enabled = labelEndPointText.Enabled = labelEndPoint.Enabled = checkBoxOverrideStartEnd.Checked;
        }

        private void trackBarStartPoint_ValueChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }

        private void trackBarEndPoint_ValueChanged(object sender, EventArgs e)
        {
            UpdateLabels();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            contextMenuNewCurve.Show((Control)sender, 0, 0);
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog { FileName = "CURVE" + comboBoxCurve.SelectedIndex.ToString("D3"), Filter = "INI Files|*.ini|Raw Data|*.bin" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(sfd.FileName).ToLowerInvariant() == ".ini")
                        IniSerializer.Serialize(resultCurves[comboBoxCurve.SelectedIndex], sfd.FileName);
                    else
                        File.WriteAllBytes(sfd.FileName, resultCurves[comboBoxCurve.SelectedIndex].curvedata);
                }
            }
        }

        private void contextFromFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { FileName = "CURVE" + comboBoxCurve.SelectedIndex.ToString("D3"), Filter = "INI Files|*.ini|Raw Data|*.bin" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(ofd.FileName).ToLowerInvariant() == ".ini")
                        resultCurves[comboBoxCurve.SelectedIndex] = IniSerializer.Deserialize<MidiProgramVelocityCurve>(ofd.FileName);
                    else
                        resultCurves[comboBoxCurve.SelectedIndex].curvedata = File.ReadAllBytes(ofd.FileName);
                    DrawCurve();
                }
            }
        }

        private void contextType1_Click(object sender, EventArgs e)
        {
            Array.Copy(Properties.Resources.DefaultCurve1, resultCurves[comboBoxCurve.SelectedIndex].curvedata, 128);
            DrawCurve();
        }

        private void contextType2_Click(object sender, EventArgs e)
        {
            Array.Copy(Properties.Resources.DefaultCurve2, resultCurves[comboBoxCurve.SelectedIndex].curvedata, 128);
            DrawCurve();
        }

        private void contextType3_Click(object sender, EventArgs e)
        {
            Array.Copy(Properties.Resources.DefaultCurve3, resultCurves[comboBoxCurve.SelectedIndex].curvedata, 128);
            DrawCurve();
        }

        private void contextType4_Click(object sender, EventArgs e)
        {
            Array.Copy(Properties.Resources.DefaultCurve4, resultCurves[comboBoxCurve.SelectedIndex].curvedata, 128);
            DrawCurve();
        }

        private void defaultTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultCurves[comboBoxCurve.SelectedIndex].curvedata = new byte[128];
            DrawCurve();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxCurve_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawCurve();
        }

        /*
private Bitmap ScaleGraph(Bitmap image, float scaleX, float scaleY)
{
    int newwidth = (int)((float)image.Width * scaleX);
    int newheight = (int)((float)image.Height * scaleY);
    Bitmap bmp = new Bitmap(newwidth, newheight);
    using (Graphics gfx = Graphics.FromImage(bmp))
    {
        gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        gfx.DrawImage(image, 0, 0, newwidth, newheight);
    }
    return bmp;
}
*/
    }
}
