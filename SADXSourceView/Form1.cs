using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SADXSourceView
{
    public partial class Form1 : Form
    {
        public string modfolder = "D:/GitHub/SADX-Decomp-Mod";
        //public string outfolder = "D:/GitHub/sa_tools_research/SADXSourceView/out";

        public Form1()
        {
            InitializeComponent();
            string iniPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SADXSourceView.ini");
            if (File.Exists(iniPath))
                modfolder = File.ReadAllText(iniPath);
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
            if (Path.GetExtension(node.FullPath) != string.Empty)
            {
                if (node.FullPath.Length > 7)
                {
                    string file_mod = Path.Combine(modfolder, node.FullPath.Substring(7, node.FullPath.Length - 7));
                    if (File.Exists(file_mod))
                        node.BackColor = Color.Green;
                    else
                        node.BackColor = Color.Red;
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
            //PopulateTreeViewFromFolder(outfolder);
            SourceUtils.PopulateTreeview(treeView);
        }

        private void RecolorTreeView()
        {
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
            //exportToXml2(treeView1, "test.xml");
            RecolorTreeView();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "SADXSourceView.ini");
            if (!Directory.Exists(Path.GetDirectoryName(logPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            File.WriteAllText(logPath, modfolder);
        }

        // Old
        private void PopulateTreeViewFromFolder(string directory)
        {
            DirectoryInfo info = new DirectoryInfo(directory);
            if (info.Exists)
            {
                GetDirectoriesForTreeView(info.GetDirectories(), null);
            }
        }
    }
}
