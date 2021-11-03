
namespace SoundBankEditor
{
    partial class LayerProperties
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.trackBarBendRangePlus = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.trackBarBendRangeMinus = new System.Windows.Forms.TrackBar();
            this.trackBarDelay = new System.Windows.Forms.TrackBar();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelBendRangePlus = new System.Windows.Forms.Label();
            this.labelBendRangeMinus = new System.Windows.Forms.Label();
            this.labelDelay = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBendRangePlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBendRangeMinus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarBendRangePlus
            // 
            this.trackBarBendRangePlus.AutoSize = false;
            this.trackBarBendRangePlus.Location = new System.Drawing.Point(138, 20);
            this.trackBarBendRangePlus.Maximum = 24;
            this.trackBarBendRangePlus.Name = "trackBarBendRangePlus";
            this.trackBarBendRangePlus.Size = new System.Drawing.Size(256, 48);
            this.trackBarBendRangePlus.TabIndex = 1;
            this.trackBarBendRangePlus.TickFrequency = 0;
            this.trackBarBendRangePlus.ValueChanged += new System.EventHandler(this.trackBarBendRangePlus_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Bend Range +:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Bend Range -:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Layer Delay:";
            // 
            // trackBarBendRangeMinus
            // 
            this.trackBarBendRangeMinus.AutoSize = false;
            this.trackBarBendRangeMinus.Location = new System.Drawing.Point(138, 74);
            this.trackBarBendRangeMinus.Maximum = 24;
            this.trackBarBendRangeMinus.Name = "trackBarBendRangeMinus";
            this.trackBarBendRangeMinus.Size = new System.Drawing.Size(256, 48);
            this.trackBarBendRangeMinus.TabIndex = 8;
            this.trackBarBendRangeMinus.TickFrequency = 0;
            this.trackBarBendRangeMinus.ValueChanged += new System.EventHandler(this.trackBarBendRangeMinus_ValueChanged);
            // 
            // trackBarDelay
            // 
            this.trackBarDelay.AutoSize = false;
            this.trackBarDelay.LargeChange = 16;
            this.trackBarDelay.Location = new System.Drawing.Point(138, 128);
            this.trackBarDelay.Maximum = 256;
            this.trackBarDelay.Name = "trackBarDelay";
            this.trackBarDelay.Size = new System.Drawing.Size(256, 48);
            this.trackBarDelay.SmallChange = 4;
            this.trackBarDelay.TabIndex = 9;
            this.trackBarDelay.TickFrequency = 0;
            this.trackBarDelay.ValueChanged += new System.EventHandler(this.trackBarDelay_ValueChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSave.Location = new System.Drawing.Point(296, 182);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(72, 32);
            this.buttonSave.TabIndex = 10;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(374, 182);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 32);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelBendRangePlus
            // 
            this.labelBendRangePlus.AutoSize = true;
            this.labelBendRangePlus.Location = new System.Drawing.Point(400, 25);
            this.labelBendRangePlus.Name = "labelBendRangePlus";
            this.labelBendRangePlus.Size = new System.Drawing.Size(18, 20);
            this.labelBendRangePlus.TabIndex = 31;
            this.labelBendRangePlus.Text = "0";
            // 
            // labelBendRangeMinus
            // 
            this.labelBendRangeMinus.AutoSize = true;
            this.labelBendRangeMinus.Location = new System.Drawing.Point(400, 79);
            this.labelBendRangeMinus.Name = "labelBendRangeMinus";
            this.labelBendRangeMinus.Size = new System.Drawing.Size(18, 20);
            this.labelBendRangeMinus.TabIndex = 32;
            this.labelBendRangeMinus.Text = "0";
            // 
            // labelDelay
            // 
            this.labelDelay.AutoSize = true;
            this.labelDelay.Location = new System.Drawing.Point(400, 133);
            this.labelDelay.Name = "labelDelay";
            this.labelDelay.Size = new System.Drawing.Size(18, 20);
            this.labelDelay.TabIndex = 33;
            this.labelDelay.Text = "0";
            // 
            // LayerProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 231);
            this.Controls.Add(this.trackBarDelay);
            this.Controls.Add(this.labelDelay);
            this.Controls.Add(this.labelBendRangeMinus);
            this.Controls.Add(this.labelBendRangePlus);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.trackBarBendRangeMinus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackBarBendRangePlus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LayerProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Layer Properties";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBendRangePlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBendRangeMinus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TrackBar trackBarBendRangePlus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackBarBendRangeMinus;
        private System.Windows.Forms.TrackBar trackBarDelay;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelBendRangePlus;
        private System.Windows.Forms.Label labelBendRangeMinus;
        private System.Windows.Forms.Label labelDelay;
    }
}