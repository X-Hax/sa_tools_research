using System;
using System.Windows.Forms;
using static SoundBankEditor.ProgramBankEditor;

namespace SoundBankEditor
{
    public partial class ProgramProperties : Form
    {
        public string Result;

        public ProgramProperties(MidiProgram program)
        {
            InitializeComponent();
            textBoxName.Text = program.Name;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Result = textBoxName.Text;
        }
    }
}
