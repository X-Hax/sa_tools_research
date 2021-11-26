using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SoundBankEditor.ProgramBankEditor;

namespace SoundBankEditor
{
    public partial class LayerProperties : Form
    {
        public uint resultDelay;
        public int resultBendRangeMinus;
        public int resultBendRangePlus;

        public LayerProperties(MidiProgramLayer layer)
        {
            InitializeComponent();
            trackBarBendRangeMinus.Value = layer.BendRangeMinus;
            trackBarBendRangePlus.Value = layer.BendRangePlus;
            trackBarDelay.Value = (int)layer.LayerDelay / 4;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            resultDelay = (uint)trackBarDelay.Value * 4;
            resultBendRangeMinus = trackBarBendRangeMinus.Value;
            resultBendRangePlus = trackBarBendRangePlus.Value;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void trackBarBendRangePlus_ValueChanged(object sender, EventArgs e)
        {
            labelBendRangePlus.Text = trackBarBendRangePlus.Value.ToString();
        }


        private void trackBarBendRangeMinus_ValueChanged(object sender, EventArgs e)
        {
            labelBendRangeMinus.Text = trackBarBendRangeMinus.Value.ToString();
        }

        private void trackBarDelay_ValueChanged(object sender, EventArgs e)
        {
            labelDelay.Text = (4 * trackBarDelay.Value).ToString();
        }
    }
}
