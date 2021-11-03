using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundBankEditor
{
    public partial class SequenceBankEditor : Form
    {
        public SequenceBankEditor()
        {
            InitializeComponent();
            MidiSequenceBank bank = MidiSequenceBank.LoadMSB(Program.args[0]);
            MessageBox.Show(bank.Sequences.Count.ToString() + " sequences.");
        }
    }
}
