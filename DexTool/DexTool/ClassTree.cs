using DexTool.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DexTool
{
    /// <summary>
    /// 类目录树结构
    /// </summary>
    public class ClassTree
    {
        // ---------------------------
        // ClassTree定义逻辑

        public String Name = "";    // 记录当前节点名称
        public bool isDir = false;  // 标记当前节点，为目录或叶节点

        public Dictionary<String, ClassTree> childs = new Dictionary<string,ClassTree>(); // 子节点
        public ClassTree parent;    // 记录当前节点的父节点

        /// <summary>
        /// 从指定的名称构建ClassTree
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="isDir"></param>
        public ClassTree(String nodeName, bool isDir=true)
        {
            this.Name = nodeName;
            this.isDir = isDir;
        }

        /// <summary>
        /// 解析classStr中的所有文件路径信息为Tree数据
        /// </summary>
        public ClassTree(List<string> classStr, String treeName)
        {
            this.Name = treeName;

            foreach (string str0 in classStr)
            {
                string str = str0.Trim();
                AddChild(str);
            }
        }


        /// <summary>
        /// 获取ClassTree的字符串形式
        /// </summary>
        public string ToString()
        {
            if (parent == null || parent.Name.ToLower().EndsWith(".dex")) return Name;
            else return parent.ToString() + "/" + Name;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="childName"></param>
        /// <param name="isDir"></param>
        public void AddChild(String childName, bool isDir=true)
        {
            if (childName.Contains("/")) AddChildByPath(childName);
            else
            {
                if (!childs.ContainsKey(childName))
                {
                    ClassTree child = new ClassTree(childName, isDir);
                    childs.Add(childName, child);       // 添加至当前节点的子节点
                    childs[childName].parent = this;    // 设置当前节点为父节点
                }
            }
        }

        /// <summary>
        /// 添加tree至当前节点的子节点中
        /// </summary>
        /// <param name="tree"></param>
        public void AddChild(ClassTree tree)
        {
            // 若子节点中不含有该节点，则直接添加
            if (!childs.ContainsKey(tree.Name)) 
            {
                childs.Add(tree.Name, tree);
                tree.parent = this;
            }
            // 若子节点中已含有该节点，则合并至同名子节点
            else
            {
                ClassTree child = childs[tree.Name];                // 获取子节点
                foreach (ClassTree sChild in tree.childs.Values)    // 添加tree的所有子节点
                {
                    child.AddChild(sChild);
                }
            }
        }

        /// <summary>
        /// childPath形如：Lcom/ltsdk_167_xiaomihaiwai/v1_0_5/R$string;
        /// </summary>
        /// <param name="childPath"></param>
        private void AddChildByPath(String childPath)
        {
            // 获取所有子节点名称信息
            String[] A = childPath.Split('/');

            // 逐级添加子节点
            ClassTree tree = this;
            for (int i = 0; i < A.Length; i++)
            {
                string name = A[i];
                if (i == A.Length - 1) name += ".class";

                tree.AddChild(name);
                tree = tree.childs[name];

                if (i == A.Length - 1) tree.isDir = false;  // 最后一个设置为
            }
        }

        /// <summary>
        /// 删除指定名称的child
        /// </summary>
        /// <param name="childName"></param>
        public void RemoveChild(String childName)
        {
            if (childName.Contains("/")) RemoveChildByPath(childName);
            else
            {
                if (childs.ContainsKey(childName))
                {
                    childs.Remove(childName);
                }
            }
        }

        /// <summary>
        /// 获取指定名称路径下的Tree节点
        /// </summary>
        public ClassTree GetChildByPath(String childPath)
        {
            ClassTree tree = this;

            // 获取所有子节点名称信息
            String[] A = childPath.Split('/');
            foreach (string name in A)
            {
                if (tree.childs.ContainsKey(name))
                {
                    tree = tree.childs[name];
                }
                else tree = null;
            }

            return tree;
        }

        /// <summary>
        /// 删除指定名称路径下的child
        /// </summary>
        private void RemoveChildByPath(String childPath)
        {
            ClassTree treeNode = GetChildByPath(childPath);     // 获取路径下的
            if (treeNode != null && treeNode.parent != null)    
            {
                treeNode.parent.RemoveChild(treeNode.Name);
            }
        }

        // ---------------------------
        // ClassTree相关功能逻辑

        /// <summary>
        /// 从当前数据信息创建TreeNode
        /// </summary>
        public TreeNode ToTreeNode()
        {
            TreeNode node = new TreeNode();
            node.Name = this.Name;
            node.Text = Name;
            node.Tag = this.ToString();

            // 设置图标
            if (parent == null)
                node.ImageIndex = 2;
            else 
                node.ImageIndex = isDir ? 0 : 1;
            node.SelectedImageIndex = node.ImageIndex;

            foreach (string key in childs.Keys)
            {
                ClassTree child = childs[key];
                TreeNode childNode = child.ToTreeNode();
                
                node.Nodes.Add(childNode);
            }

            return node;
        }


        /// <summary>
        /// 获取设置的图像资源信息为ImageList
        /// </summary>
        private ImageList getImageList()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(24, 24);

            imageList.Images.Add(Resources._3);     // 添加文件夹图标
            imageList.Images.Add(Resources._70);    // 添加文件图标
            imageList.Images.Add(Resources.java);   // 添加文件图标

            return imageList;
        }

        /// <summary>
        /// 在TreeView中显示类路径树
        /// </summary>
        /// <param name="tree"></param>
        public void ShowIn(TreeView tree)
        {
            if (tree.ImageList == null)
            {
                tree.ImageList = getImageList();    // 设置图标
                tree.CheckBoxes = true;             // 显示复选框
                tree.ShowLines = true;              // 显示连接线

                tree.AfterCheck += new TreeViewEventHandler(treeView_AfterCheck);
            }

            TreeNode thisNode = ToTreeNode();       // 生成TreeNode
            foreach(TreeNode node in tree.Nodes)    // 遍历所有节点
            {
                if (node.Text.Equals(this.Name))
                {
                    tree.Nodes.Remove(node);        // 移除同名节点
                    break;
                }
            }
            tree.Nodes.Add(thisNode);
        }

        /// <summary>
        /// 全选/全不选 当前节点和子节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            bool checkState = node.Checked;

            foreach (TreeNode child in node.Nodes)
            {
                child.Checked = checkState;
            }
        }

    }
}
