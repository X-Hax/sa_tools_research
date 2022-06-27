using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace SADXSourceView
{
    // Most of this is from https://stackoverflow.com/questions/48376642/xml-represantation-of-treeview-nodes
    public static class SourceUtils
    {
        private static XmlTextWriter? xr;

        // Save
        public static string ParseNameForXML(string name)
        {
            string parsedname = name.Replace(" ", "_THIS_IS_SPACE_");
            parsedname = parsedname.Replace("@", "_THIS_IS_AT_");
            switch (parsedname.Substring(0, 1))
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    parsedname = parsedname.Insert(0, "_TRIM_");
                    break;
                default:
                    break;
            }
            return parsedname;
        }

        public static void exportToXml2(TreeView tv, string filename)
        {
            xr = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            xr.WriteStartDocument();
            //Write our root node
            foreach (TreeNode node in tv.Nodes)
            {
                //Console.WriteLine("Node {0}, count {1}", node.Text, node.Nodes.Count);
                xr.WriteStartElement(node.Text);
                saveNode2(node.Nodes);
            }

            //Close the root node
            xr.WriteEndElement();
            xr.Close();
        }

        private static void saveNode2(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                if (node.Nodes.Count > 0)
                {
                    xr.WriteStartElement(node.Text);
                    saveNode2(node.Nodes);
                    xr.WriteEndElement();
                }
                else
                {
                    xr.WriteRaw($"<{node.Text}/>");
                }
            }
        }

        // Load
        public static string ParseXMLNameForTreeView(string name)
        {
            string parsename = name.Replace("_TRIM_", "");
            parsename = parsename.Replace("_THIS_IS_AT_", "@");
            parsename = parsename.Replace("_THIS_IS_SPACE_", " ");
            return parsename;
        }

        public static void PopulateTreeview(TreeView tv)
        {
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Title = "Open XML Document";
            //dlg.Filter = "XML Files (*.xml)|*.xml";
            //dlg.FileName = Application.StartupPath + "\\..\\..\\example.xml";

           // if (dlg.ShowDialog() == DialogResult.OK)
            //{
                try
                {
                    //First, we'll load the Xml document
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "source.xml"));

                    // Now, clear out the treeview, 
                    // and add the first (root) node
                    tv.Nodes.Clear();

                    tv.Nodes.Add(new TreeNode(SourceUtils.ParseXMLNameForTreeView(xDoc.DocumentElement.Name)));

                    TreeNode tNode = new TreeNode();
                    tNode = (TreeNode)tv.Nodes[0];

                    // We make a call to addTreeNode, 
                    // where we'll add all of our nodes
                    addTreeNode(xDoc.DocumentElement, tNode);

                    // Expand the treeview to show all nodes
                    //tv.ExpandAll();
                }
                catch (XmlException xExc)
                {
                    // Exception is thrown is there is an error in the Xml
                    MessageBox.Show(xExc.Message);
                }
                catch (Exception ex) //General exception
                {
                    MessageBox.Show(ex.Message);
                }
            //}

            
        }

        //Open the XML file, and start to populate the treeview

        // This function is called recursively until all nodes are loaded
        public static void addTreeNode(XmlNode xmlNode, TreeNode treeNode)
        {
            char[] karakter = new char[] { '<', '>', '/' };
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList xNodeList;
            if (xmlNode.HasChildNodes) //The current node has children
            {
                xNodeList = xmlNode.ChildNodes;
                for (int x = 0; x <= xNodeList.Count - 1; x++)
                {
                    xNode = xmlNode.ChildNodes[x];
                    treeNode.Nodes.Add(new TreeNode(SourceUtils.ParseXMLNameForTreeView(xNode.Name)));
                    tNode = treeNode.Nodes[x];
                    addTreeNode(xNode, tNode);
                }
            }
            else
            {
                treeNode.Text = ParseXMLNameForTreeView(xmlNode.OuterXml.Trim(karakter));
            }
        }
    }
}
