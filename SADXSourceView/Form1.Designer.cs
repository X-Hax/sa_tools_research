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
            this.treeView = new System.Windows.Forms.TreeView();
            this.buttonSetLocation = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.treeView.Location = new System.Drawing.Point(0, 64);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(800, 581);
            this.treeView.TabIndex = 0;
            // 
            // buttonSetLocation
            // 
            this.buttonSetLocation.Location = new System.Drawing.Point(653, 12);
            this.buttonSetLocation.Name = "buttonSetLocation";
            this.buttonSetLocation.Size = new System.Drawing.Size(135, 34);
            this.buttonSetLocation.TabIndex = 2;
            this.buttonSetLocation.Text = "Set Location...";
            this.buttonSetLocation.UseVisualStyleBackColor = true;
            this.buttonSetLocation.Click += new System.EventHandler(this.buttonSetLocation_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(12, 12);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(92, 34);
            this.buttonRefresh.TabIndex = 3;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonRefresh);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.buttonSetLocation);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 58);
            this.panel1.TabIndex = 4;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(110, 13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(537, 31);
            this.textBox1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 645);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.treeView);
            this.Name = "Form1";
            this.Text = "SADX Source Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button buttonSetLocation;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
    }
}
