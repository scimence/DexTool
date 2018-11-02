using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DexTool
{
    public partial class ClassesForm : Form
    {
        public ClassesForm(List<string> classes)
        {
            InitializeComponent();

            ClassTree tree = new ClassTree(classes, "class.dex");
            tree.ShowIn(treeView1);
        }

        //private void setTreeNode(List<string> classes)
        //{
        //    treeView1.Nodes.Clear();
        //    for
        //}
    }
}
