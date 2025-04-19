namespace SADXSourceView
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            treeView = new System.Windows.Forms.TreeView();
            buttonSetLocation = new System.Windows.Forms.Button();
            buttonRefresh = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            checkBoxAssets = new System.Windows.Forms.CheckBox();
            textBox1 = new System.Windows.Forms.TextBox();
            buttonCopy = new System.Windows.Forms.Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // treeView
            // 
            treeView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            treeView.Location = new System.Drawing.Point(0, 43);
            treeView.Margin = new System.Windows.Forms.Padding(2);
            treeView.Name = "treeView";
            treeView.Size = new System.Drawing.Size(734, 413);
            treeView.TabIndex = 0;
            // 
            // buttonSetLocation
            // 
            buttonSetLocation.Location = new System.Drawing.Point(374, 7);
            buttonSetLocation.Margin = new System.Windows.Forms.Padding(2);
            buttonSetLocation.Name = "buttonSetLocation";
            buttonSetLocation.Size = new System.Drawing.Size(90, 23);
            buttonSetLocation.TabIndex = 2;
            buttonSetLocation.Text = "Set Location...";
            buttonSetLocation.UseVisualStyleBackColor = true;
            buttonSetLocation.Click += buttonSetLocation_Click;
            // 
            // buttonRefresh
            // 
            buttonRefresh.Location = new System.Drawing.Point(468, 7);
            buttonRefresh.Margin = new System.Windows.Forms.Padding(2);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Size = new System.Drawing.Size(61, 23);
            buttonRefresh.TabIndex = 3;
            buttonRefresh.Text = "Refresh";
            buttonRefresh.UseVisualStyleBackColor = true;
            buttonRefresh.Click += buttonRefresh_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonCopy);
            panel1.Controls.Add(checkBoxAssets);
            panel1.Controls.Add(buttonRefresh);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(buttonSetLocation);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(732, 39);
            panel1.TabIndex = 4;
            // 
            // checkBoxAssets
            // 
            checkBoxAssets.AutoSize = true;
            checkBoxAssets.Location = new System.Drawing.Point(534, 9);
            checkBoxAssets.Name = "checkBoxAssets";
            checkBoxAssets.Size = new System.Drawing.Size(85, 19);
            checkBoxAssets.TabIndex = 5;
            checkBoxAssets.Text = "Assets only";
            checkBoxAssets.UseVisualStyleBackColor = true;
            checkBoxAssets.CheckedChanged += checkBoxAssets_CheckedChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(11, 7);
            textBox1.Margin = new System.Windows.Forms.Padding(2);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(359, 23);
            textBox1.TabIndex = 4;
            // 
            // buttonCopy
            // 
            buttonCopy.Location = new System.Drawing.Point(625, 6);
            buttonCopy.Name = "buttonCopy";
            buttonCopy.Size = new System.Drawing.Size(97, 23);
            buttonCopy.TabIndex = 6;
            buttonCopy.Text = "Copy missing";
            buttonCopy.UseVisualStyleBackColor = true;
            buttonCopy.Click += buttonCopy_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(732, 454);
            Controls.Add(panel1);
            Controls.Add(treeView);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "Form1";
            Text = "SADX Source Tool";
            FormClosing += Form1_FormClosing;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button buttonSetLocation;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBoxAssets;
        private System.Windows.Forms.Button buttonCopy;
    }
}
