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

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!timer1.Enabled)
            {
                timer1.Enabled = true;
                timer1.Interval = 300;
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }



        #region 获取选取的类路径信息

        private void timer1_Tick(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            List<string> list = getSelectPath(treeView1);
            foreach (String classPath in list)
            {
                builder.AppendLine(classPath);
            }

            String data = builder.ToString();

            textBox1.Text = data;
            timer1.Enabled = false;
        }

        /// <summary>
        /// 获取已选中的类路径信息
        /// </summary>
        private List<string> getSelectPath(TreeView tree)
        {
            List<string> list = new List<string>();
            foreach (TreeNode node in tree.Nodes)
            {
                getSelectPath(node, list);
            }

            return list;
        }

        /// <summary>
        /// 获取选择的class路径信息
        /// </summary>
        private void getSelectPath(TreeNode node, List<string> list)
        {
            foreach (TreeNode child in node.Nodes)
            {
                getSelectPath(child, list);
            }

            if (node.Checked)
            {
                String path = node.Tag as String;
                if (node.Text.EndsWith(".class") && !list.Contains(path)) list.Add(path);
            }
        }

        #endregion


    }
}
