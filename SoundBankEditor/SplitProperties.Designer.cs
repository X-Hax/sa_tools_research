
namespace SoundBankEditor
{
    partial class SplitProperties
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.trackBarDryLevel = new System.Windows.Forms.TrackBar();
            this.trackBarEndNote = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarStartNote = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.trackBarDryPan = new System.Windows.Forms.TrackBar();
            this.trackBarFXLevel = new System.Windows.Forms.TrackBar();
            this.trackBarFXChannel = new System.Windows.Forms.TrackBar();
            this.labelStartNote = new System.Windows.Forms.Label();
            this.labelEndNote = new System.Windows.Forms.Label();
            this.labelDryLevel = new System.Windows.Forms.Label();
            this.labelDryPan = new System.Windows.Forms.Label();
            this.labelFXLevel = new System.Windows.Forms.Label();
            this.labelFXChannel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDryLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEndNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarStartNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDryPan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFXLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFXChannel)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(376, 341);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(72, 32);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSave.Location = new System.Drawing.Point(298, 341);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(72, 32);
            this.buttonSave.TabIndex = 20;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // trackBarDryLevel
            // 
            this.trackBarDryLevel.AutoSize = false;
            this.trackBarDryLevel.Location = new System.Drawing.Point(123, 125);
            this.trackBarDryLevel.Maximum = 15;
            this.trackBarDryLevel.Name = "trackBarDryLevel";
            this.trackBarDryLevel.Size = new System.Drawing.Size(256, 48);
            this.trackBarDryLevel.TabIndex = 19;
            this.trackBarDryLevel.TickFrequency = 0;
            this.trackBarDryLevel.ValueChanged += new System.EventHandler(this.trackBarStartNote_ValueChanged);
            // 
            // trackBarEndNote
            // 
            this.trackBarEndNote.AutoSize = false;
            this.trackBarEndNote.Location = new System.Drawing.Point(123, 71);
            this.trackBarEndNote.Maximum = 127;
            this.trackBarEndNote.Name = "trackBarEndNote";
            this.trackBarEndNote.Size = new System.Drawing.Size(256, 48);
            this.trackBarEndNote.TabIndex = 18;
            this.trackBarEndNote.TickFrequency = 0;
            this.trackBarEndNote.ValueChanged += new System.EventHandler(this.trackBarStartNote_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Dry Level:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "End Note:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 20);
            this.label2.TabIndex = 15;
            this.label2.Text = "Start Note:";
            // 
            // trackBarStartNote
            // 
            this.trackBarStartNote.AutoSize = false;
            this.trackBarStartNote.Location = new System.Drawing.Point(123, 17);
            this.trackBarStartNote.Maximum = 127;
            this.trackBarStartNote.Name = "trackBarStartNote";
            this.trackBarStartNote.Size = new System.Drawing.Size(256, 48);
            this.trackBarStartNote.TabIndex = 13;
            this.trackBarStartNote.TickFrequency = 0;
            this.trackBarStartNote.ValueChanged += new System.EventHandler(this.trackBarStartNote_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(44, 184);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 20);
            this.label5.TabIndex = 22;
            this.label5.Text = "Dry Pan:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 20);
            this.label6.TabIndex = 23;
            this.label6.Text = "FX Level:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 292);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 20);
            this.label7.TabIndex = 24;
            this.label7.Text = "FX Channel:";
            // 
            // trackBarDryPan
            // 
            this.trackBarDryPan.AutoSize = false;
            this.trackBarDryPan.Location = new System.Drawing.Point(123, 179);
            this.trackBarDryPan.Maximum = 16;
            this.trackBarDryPan.Minimum = -15;
            this.trackBarDryPan.Name = "trackBarDryPan";
            this.trackBarDryPan.Size = new System.Drawing.Size(256, 48);
            this.trackBarDryPan.TabIndex = 25;
            this.trackBarDryPan.TickFrequency = 0;
            this.trackBarDryPan.Value = 1;
            this.trackBarDryPan.ValueChanged += new System.EventHandler(this.trackBarStartNote_ValueChanged);
            // 
            // trackBarFXLevel
            // 
            this.trackBarFXLevel.AutoSize = false;
            this.trackBarFXLevel.Location = new System.Drawing.Point(123, 233);
            this.trackBarFXLevel.Maximum = 15;
            this.trackBarFXLevel.Name = "trackBarFXLevel";
            this.trackBarFXLevel.Size = new System.Drawing.Size(256, 48);
            this.trackBarFXLevel.TabIndex = 26;
            this.trackBarFXLevel.TickFrequency = 0;
            this.trackBarFXLevel.ValueChanged += new System.EventHandler(this.trackBarStartNote_ValueChanged);
            // 
            // trackBarFXChannel
            // 
            this.trackBarFXChannel.AutoSize = false;
            this.trackBarFXChannel.Location = new System.Drawing.Point(123, 287);
            this.trackBarFXChannel.Maximum = 15;
            this.trackBarFXChannel.Name = "trackBarFXChannel";
            this.trackBarFXChannel.Size = new System.Drawing.Size(256, 48);
            this.trackBarFXChannel.TabIndex = 27;
            this.trackBarFXChannel.TickFrequency = 0;
            this.trackBarFXChannel.ValueChanged += new System.EventHandler(this.trackBarStartNote_ValueChanged);
            // 
            // labelStartNote
            // 
            this.labelStartNote.AutoSize = true;
            this.labelStartNote.Location = new System.Drawing.Point(385, 22);
            this.labelStartNote.Name = "labelStartNote";
            this.labelStartNote.Size = new System.Drawing.Size(51, 20);
            this.labelStartNote.TabIndex = 28;
            this.labelStartNote.Text = "0: C-2";
            // 
            // labelEndNote
            // 
            this.labelEndNote.AutoSize = true;
            this.labelEndNote.Location = new System.Drawing.Point(385, 76);
            this.labelEndNote.Name = "labelEndNote";
            this.labelEndNote.Size = new System.Drawing.Size(51, 20);
            this.labelEndNote.TabIndex = 29;
            this.labelEndNote.Text = "0: C-2";
            // 
            // labelDryLevel
            // 
            this.labelDryLevel.AutoSize = true;
            this.labelDryLevel.Location = new System.Drawing.Point(385, 132);
            this.labelDryLevel.Name = "labelDryLevel";
            this.labelDryLevel.Size = new System.Drawing.Size(18, 20);
            this.labelDryLevel.TabIndex = 30;
            this.labelDryLevel.Text = "0";
            // 
            // labelDryPan
            // 
            this.labelDryPan.AutoSize = true;
            this.labelDryPan.Location = new System.Drawing.Point(385, 184);
            this.labelDryPan.Name = "labelDryPan";
            this.labelDryPan.Size = new System.Drawing.Size(18, 20);
            this.labelDryPan.TabIndex = 31;
            this.labelDryPan.Text = "0";
            // 
            // labelFXLevel
            // 
            this.labelFXLevel.AutoSize = true;
            this.labelFXLevel.Location = new System.Drawing.Point(385, 236);
            this.labelFXLevel.Name = "labelFXLevel";
            this.labelFXLevel.Size = new System.Drawing.Size(18, 20);
            this.labelFXLevel.TabIndex = 32;
            this.labelFXLevel.Text = "0";
            // 
            // labelFXChannel
            // 
            this.labelFXChannel.AutoSize = true;
            this.labelFXChannel.Location = new System.Drawing.Point(385, 292);
            this.labelFXChannel.Name = "labelFXChannel";
            this.labelFXChannel.Size = new System.Drawing.Size(18, 20);
            this.labelFXChannel.TabIndex = 33;
            this.labelFXChannel.Text = "0";
            // 
            // SplitProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 392);
            this.Controls.Add(this.labelFXChannel);
            this.Controls.Add(this.labelFXLevel);
            this.Controls.Add(this.labelDryPan);
            this.Controls.Add(this.labelDryLevel);
            this.Controls.Add(this.labelEndNote);
            this.Controls.Add(this.labelStartNote);
            this.Controls.Add(this.trackBarFXChannel);
            this.Controls.Add(this.trackBarFXLevel);
            this.Controls.Add(this.trackBarDryPan);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.trackBarDryLevel);
            this.Controls.Add(this.trackBarEndNote);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackBarStartNote);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplitProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Split Properties";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDryLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEndNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarStartNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDryPan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFXLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFXChannel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TrackBar trackBarDryLevel;
        private System.Windows.Forms.TrackBar trackBarEndNote;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarStartNote;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar trackBarDryPan;
        private System.Windows.Forms.TrackBar trackBarFXLevel;
        private System.Windows.Forms.TrackBar trackBarFXChannel;
        private System.Windows.Forms.Label labelStartNote;
        private System.Windows.Forms.Label labelEndNote;
        private System.Windows.Forms.Label labelDryLevel;
        private System.Windows.Forms.Label labelDryPan;
        private System.Windows.Forms.Label labelFXLevel;
        private System.Windows.Forms.Label labelFXChannel;
    }
}