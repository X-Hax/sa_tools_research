using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SADXSourceView
{
    public partial class Form1 : Form
    {
        public string modfolder = "D:/GitHub/SADX-Decomp-Mod";
        public bool suspend = true;
        private List<string> missing;

        public Form1()
        {
            InitializeComponent();
            string iniPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SADXSourceView.ini");
            if (File.Exists(iniPath))
            {
                string[] lines = File.ReadAllLines(iniPath);
                modfolder = lines[0];
                if (lines.Length > 1 && lines[1].ToLowerInvariant().Contains("true"))
                    checkBoxAssets.Checked = SourceUtils.assetsOnly = true;
            }
            suspend = false;
            textBox1.Text = modfolder;
            RebuildTreeView();
            RecolorTreeView();
        }

        private void SetNodeParentColor(TreeNode node, Color color)
        {
            if (node == null)
                return;
            if (node.BackColor != Color.Red && node.BackColor != Color.Green && node.BackColor != Color.Yellow)
                node.BackColor = color;
            if (node.BackColor == Color.Green && color == Color.Red)
                node.BackColor = Color.Yellow;
            if (node.BackColor == Color.Red && color == Color.Green)
                node.BackColor = Color.Yellow;
            if (color == Color.Yellow)
                node.BackColor = Color.Yellow;
            if (node.Parent != null)
                SetNodeParentColor(node.Parent, node.BackColor);
        }

        private void ColorTreeNodes(TreeNode node)
        {
            // Check if it's a folder; if not, compare files
            if (node.FullPath.Contains("<") || node.FullPath.Contains(">"))
                MessageBox.Show(node.FullPath);
            if (Path.GetExtension(node.FullPath) != string.Empty)
            {
                if (node.FullPath.Length > 7)
                {
                    string file_mod = Path.Combine(modfolder, node.FullPath.Substring(7, node.FullPath.Length - 7));
                    if (File.Exists(file_mod))
                        node.BackColor = Color.Green;
                    else
                    {
                        missing.Add(node.FullPath);
                        node.BackColor = Color.Red;
                    }
                    SetNodeParentColor(node.Parent, node.BackColor);
                }
            }
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                ColorTreeNodes(node.Nodes[i]);
            }
        }

        private void RebuildTreeView()
        {
            treeView.Nodes.Clear();
            SourceUtils.PopulateTreeview(treeView);
        }

        private void RecolorTreeView()
        {
            missing = new List<string>();
            ResetTreeViewColor();
            for (int i = 0; i < treeView.Nodes.Count; i++)
            {
                ColorTreeNodes(treeView.Nodes[i]);
            }
        }

        private void ResetNodeColor(TreeNode node)
        {
            node.BackColor = Color.White;
            foreach (TreeNode child in node.Nodes)
                ResetNodeColor(child);
        }

        private void ResetTreeViewColor()
        {
            foreach (TreeNode node in treeView.Nodes)
                ResetNodeColor(node);
        }

        private void GetFilesForTreeView(DirectoryInfo dir, TreeNode nodeToAddTo)
        {
            FileInfo[] files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
                for (int f = 0; f < files.Length; f++)
                    nodeToAddTo.Nodes.Add(SourceUtils.ParseNameForXML(files[f].Name));
        }

        private void GetDirectoriesForTreeView(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(SourceUtils.ParseNameForXML(subDir.Name), 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectoriesForTreeView(subSubDirs, aNode);
                }
                GetFilesForTreeView(subDir, aNode);
                if (nodeToAddTo != null)
                    nodeToAddTo.Nodes.Add(aNode);
                else
                    treeView.Nodes.Add(aNode);
            }
        }

        private void buttonSetLocation_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fd = new FolderBrowserDialog())
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = modfolder = fd.SelectedPath;
                    RecolorTreeView();
                }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            modfolder = textBox1.Text;
            RecolorTreeView();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SADXSourceView.ini");
            if (!Directory.Exists(Path.GetDirectoryName(logPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            TextWriter logWriter = File.CreateText(logPath);
            logWriter.WriteLine(modfolder);
            logWriter.WriteLine(SourceUtils.assetsOnly.ToString());
            logWriter.Flush();
            logWriter.Close();
        }

        private void checkBoxAssets_CheckedChanged(object sender, EventArgs e)
        {
            if (suspend)
                return;
            SourceUtils.assetsOnly = checkBoxAssets.Checked;
            MessageBox.Show("Pls restart");
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in missing)
                sb.AppendLine(item);
            Clipboard.SetText(sb.ToString());
            MessageBox.Show("Copied to clipboard");
        }
    }
}