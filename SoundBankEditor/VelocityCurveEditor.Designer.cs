
namespace SoundBankEditor
{
    partial class VelocityCurveEditor
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
            this.components = new System.ComponentModel.Container();
            this.trackBarStartPoint = new System.Windows.Forms.TrackBar();
            this.trackBarEndPoint = new System.Windows.Forms.TrackBar();
            this.labelStartPointText = new System.Windows.Forms.Label();
            this.labelEndPointText = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownMultiplier = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownAddX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownAddY = new System.Windows.Forms.NumericUpDown();
            this.labelStartPoint = new System.Windows.Forms.Label();
            this.labelEndPoint = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.comboBoxCurve = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxOverrideStartEnd = new System.Windows.Forms.CheckBox();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonNew = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.contextMenuNewCurve = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.defaultTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextType1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextType2 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextType3 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextType4 = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBoxCurve = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarStartPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEndPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMultiplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAddX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAddY)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.contextMenuNewCurve.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurve)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarStartPoint
            // 
            this.trackBarStartPoint.AutoSize = false;
            this.trackBarStartPoint.Location = new System.Drawing.Point(96, 178);
            this.trackBarStartPoint.Maximum = 127;
            this.trackBarStartPoint.Name = "trackBarStartPoint";
            this.trackBarStartPoint.Size = new System.Drawing.Size(128, 48);
            this.trackBarStartPoint.TabIndex = 1;
            this.trackBarStartPoint.TickFrequency = 0;
            this.trackBarStartPoint.ValueChanged += new System.EventHandler(this.trackBarStartPoint_ValueChanged);
            // 
            // trackBarEndPoint
            // 
            this.trackBarEndPoint.AutoSize = false;
            this.trackBarEndPoint.Location = new System.Drawing.Point(96, 232);
            this.trackBarEndPoint.Maximum = 127;
            this.trackBarEndPoint.Name = "trackBarEndPoint";
            this.trackBarEndPoint.Size = new System.Drawing.Size(128, 48);
            this.trackBarEndPoint.TabIndex = 2;
            this.trackBarEndPoint.TickFrequency = 0;
            this.trackBarEndPoint.ValueChanged += new System.EventHandler(this.trackBarEndPoint_ValueChanged);
            // 
            // labelStartPointText
            // 
            this.labelStartPointText.AutoSize = true;
            this.labelStartPointText.Location = new System.Drawing.Point(46, 184);
            this.labelStartPointText.Name = "labelStartPointText";
            this.labelStartPointText.Size = new System.Drawing.Size(48, 20);
            this.labelStartPointText.TabIndex = 3;
            this.labelStartPointText.Text = "Start:";
            // 
            // labelEndPointText
            // 
            this.labelEndPointText.AutoSize = true;
            this.labelEndPointText.Location = new System.Drawing.Point(48, 237);
            this.labelEndPointText.Name = "labelEndPointText";
            this.labelEndPointText.Size = new System.Drawing.Size(42, 20);
            this.labelEndPointText.TabIndex = 4;
            this.labelEndPointText.Text = "End:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "y=A(x*x)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Add X:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 20);
            this.label5.TabIndex = 7;
            this.label5.Text = "Add Y:";
            // 
            // numericUpDownMultiplier
            // 
            this.numericUpDownMultiplier.Location = new System.Drawing.Point(100, 38);
            this.numericUpDownMultiplier.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownMultiplier.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.numericUpDownMultiplier.Name = "numericUpDownMultiplier";
            this.numericUpDownMultiplier.Size = new System.Drawing.Size(120, 26);
            this.numericUpDownMultiplier.TabIndex = 8;
            // 
            // numericUpDownAddX
            // 
            this.numericUpDownAddX.Location = new System.Drawing.Point(100, 70);
            this.numericUpDownAddX.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownAddX.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.numericUpDownAddX.Name = "numericUpDownAddX";
            this.numericUpDownAddX.Size = new System.Drawing.Size(120, 26);
            this.numericUpDownAddX.TabIndex = 9;
            // 
            // numericUpDownAddY
            // 
            this.numericUpDownAddY.Location = new System.Drawing.Point(100, 101);
            this.numericUpDownAddY.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownAddY.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.numericUpDownAddY.Name = "numericUpDownAddY";
            this.numericUpDownAddY.Size = new System.Drawing.Size(120, 26);
            this.numericUpDownAddY.TabIndex = 10;
            this.numericUpDownAddY.Value = new decimal(new int[] {
            127,
            0,
            0,
            0});
            // 
            // labelStartPoint
            // 
            this.labelStartPoint.AutoSize = true;
            this.labelStartPoint.Location = new System.Drawing.Point(230, 184);
            this.labelStartPoint.Name = "labelStartPoint";
            this.labelStartPoint.Size = new System.Drawing.Size(18, 20);
            this.labelStartPoint.TabIndex = 11;
            this.labelStartPoint.Text = "0";
            this.labelStartPoint.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelEndPoint
            // 
            this.labelEndPoint.AutoSize = true;
            this.labelEndPoint.Location = new System.Drawing.Point(230, 237);
            this.labelEndPoint.Name = "labelEndPoint";
            this.labelEndPoint.Size = new System.Drawing.Size(18, 20);
            this.labelEndPoint.TabIndex = 12;
            this.labelEndPoint.Text = "0";
            this.labelEndPoint.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonSave
            // 
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSave.Location = new System.Drawing.Point(303, 481);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(84, 36);
            this.buttonSave.TabIndex = 13;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(393, 481);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(84, 36);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // comboBoxCurve
            // 
            this.comboBoxCurve.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCurve.FormattingEnabled = true;
            this.comboBoxCurve.Location = new System.Drawing.Point(302, 215);
            this.comboBoxCurve.Name = "comboBoxCurve";
            this.comboBoxCurve.Size = new System.Drawing.Size(176, 28);
            this.comboBoxCurve.TabIndex = 15;
            this.comboBoxCurve.SelectedIndexChanged += new System.EventHandler(this.comboBoxCurve_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(298, 192);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 20);
            this.label6.TabIndex = 16;
            this.label6.Text = "Select Curve:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxOverrideStartEnd);
            this.groupBox1.Controls.Add(this.buttonGenerate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numericUpDownMultiplier);
            this.groupBox1.Controls.Add(this.numericUpDownAddX);
            this.groupBox1.Controls.Add(this.labelEndPoint);
            this.groupBox1.Controls.Add(this.numericUpDownAddY);
            this.groupBox1.Controls.Add(this.labelEndPointText);
            this.groupBox1.Controls.Add(this.labelStartPoint);
            this.groupBox1.Controls.Add(this.trackBarEndPoint);
            this.groupBox1.Controls.Add(this.trackBarStartPoint);
            this.groupBox1.Controls.Add(this.labelStartPointText);
            this.groupBox1.Location = new System.Drawing.Point(12, 183);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 334);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Generate";
            // 
            // checkBoxOverrideStartEnd
            // 
            this.checkBoxOverrideStartEnd.AutoSize = true;
            this.checkBoxOverrideStartEnd.Location = new System.Drawing.Point(10, 148);
            this.checkBoxOverrideStartEnd.Name = "checkBoxOverrideStartEnd";
            this.checkBoxOverrideStartEnd.Size = new System.Drawing.Size(214, 24);
            this.checkBoxOverrideStartEnd.TabIndex = 21;
            this.checkBoxOverrideStartEnd.Text = "Override Start/End Points";
            this.checkBoxOverrideStartEnd.UseVisualStyleBackColor = true;
            this.checkBoxOverrideStartEnd.CheckedChanged += new System.EventHandler(this.checkBoxOverrideStartEnd_CheckedChanged);
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Location = new System.Drawing.Point(80, 286);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(104, 36);
            this.buttonGenerate.TabIndex = 20;
            this.buttonGenerate.Text = "Generate";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(393, 249);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(84, 36);
            this.buttonImport.TabIndex = 18;
            this.buttonImport.Text = "Import...";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(393, 291);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(84, 36);
            this.buttonExport.TabIndex = 19;
            this.buttonExport.Text = "Export...";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonNew
            // 
            this.buttonNew.Enabled = false;
            this.buttonNew.Location = new System.Drawing.Point(302, 249);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(84, 36);
            this.buttonNew.TabIndex = 20;
            this.buttonNew.Text = "New";
            this.buttonNew.UseVisualStyleBackColor = true;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Location = new System.Drawing.Point(302, 291);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(84, 36);
            this.buttonDelete.TabIndex = 21;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            // 
            // contextMenuNewCurve
            // 
            this.contextMenuNewCurve.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuNewCurve.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultTypeToolStripMenuItem,
            this.contextFromFile,
            this.toolStripSeparator1,
            this.contextType1,
            this.contextType2,
            this.contextType3,
            this.contextType4});
            this.contextMenuNewCurve.Name = "contextMenuNewCurve";
            this.contextMenuNewCurve.Size = new System.Drawing.Size(199, 202);
            // 
            // defaultTypeToolStripMenuItem
            // 
            this.defaultTypeToolStripMenuItem.Name = "defaultTypeToolStripMenuItem";
            this.defaultTypeToolStripMenuItem.Size = new System.Drawing.Size(198, 32);
            this.defaultTypeToolStripMenuItem.Text = "Empty";
            this.defaultTypeToolStripMenuItem.Click += new System.EventHandler(this.defaultTypeToolStripMenuItem_Click);
            // 
            // contextFromFile
            // 
            this.contextFromFile.Name = "contextFromFile";
            this.contextFromFile.Size = new System.Drawing.Size(198, 32);
            this.contextFromFile.Text = "From File...";
            this.contextFromFile.Click += new System.EventHandler(this.contextFromFile_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(195, 6);
            // 
            // contextType1
            // 
            this.contextType1.Name = "contextType1";
            this.contextType1.Size = new System.Drawing.Size(198, 32);
            this.contextType1.Text = "Default Type 1";
            this.contextType1.Click += new System.EventHandler(this.contextType1_Click);
            // 
            // contextType2
            // 
            this.contextType2.Name = "contextType2";
            this.contextType2.Size = new System.Drawing.Size(198, 32);
            this.contextType2.Text = "Default Type 2";
            this.contextType2.Click += new System.EventHandler(this.contextType2_Click);
            // 
            // contextType3
            // 
            this.contextType3.Name = "contextType3";
            this.contextType3.Size = new System.Drawing.Size(198, 32);
            this.contextType3.Text = "Default Type 3";
            this.contextType3.Click += new System.EventHandler(this.contextType3_Click);
            // 
            // contextType4
            // 
            this.contextType4.Name = "contextType4";
            this.contextType4.Size = new System.Drawing.Size(198, 32);
            this.contextType4.Text = "Default Type 4";
            this.contextType4.Click += new System.EventHandler(this.contextType4_Click);
            // 
            // pictureBoxCurve
            // 
            this.pictureBoxCurve.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCurve.Location = new System.Drawing.Point(36, 12);
            this.pictureBoxCurve.Name = "pictureBoxCurve";
            this.pictureBoxCurve.Size = new System.Drawing.Size(416, 160);
            this.pictureBoxCurve.TabIndex = 0;
            this.pictureBoxCurve.TabStop = false;
            // 
            // VelocityCurveEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 528);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonNew);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxCurve);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.pictureBoxCurve);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VelocityCurveEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Velocity Curve Editor";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarStartPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarEndPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMultiplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAddX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAddY)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuNewCurve.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurve)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxCurve;
        private System.Windows.Forms.TrackBar trackBarStartPoint;
        private System.Windows.Forms.TrackBar trackBarEndPoint;
        private System.Windows.Forms.Label labelStartPointText;
        private System.Windows.Forms.Label labelEndPointText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownMultiplier;
        private System.Windows.Forms.NumericUpDown numericUpDownAddX;
        private System.Windows.Forms.NumericUpDown numericUpDownAddY;
        private System.Windows.Forms.Label labelStartPoint;
        private System.Windows.Forms.Label labelEndPoint;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxCurve;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.CheckBox checkBoxOverrideStartEnd;
        private System.Windows.Forms.ContextMenuStrip contextMenuNewCurve;
        private System.Windows.Forms.ToolStripMenuItem defaultTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem contextType1;
        private System.Windows.Forms.ToolStripMenuItem contextType2;
        private System.Windows.Forms.ToolStripMenuItem contextType3;
        private System.Windows.Forms.ToolStripMenuItem contextType4;
        private System.Windows.Forms.ToolStripMenuItem contextFromFile;
    }
}