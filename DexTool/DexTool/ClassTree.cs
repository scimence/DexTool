using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DexTool
{
    /// <summary>
    /// 类目录树结构
    /// </summary>
    public class ClassTree
    {
        public String Name = "";    // 记录当前节点名称
        public bool isDir = false;  // 标记当前节点，为目录或叶节点

        Dictionary<String, ClassTree> childs = new Dictionary<string,ClassTree>(); // 子节点

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
        /// 添加子节点
        /// </summary>
        /// <param name="childName"></param>
        /// <param name="isDir"></param>
        public void AddChild(String childName, bool isDir=true)
        {
            if (!childs.ContainsKey(childName))
            {
                ClassTree child = new ClassTree(childName, isDir);
                childs.Add(childName, child);
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
        /// 删除指定名称的child
        /// </summary>
        /// <param name="childName"></param>
        public void RemoveChild(String childName)
        {
            if (childs.ContainsKey(childName))
            {
                childs.Remove(childName);
            }
        }

        /// <summary>
        /// 
        /// childPath形如：Lcom/ltsdk_167_xiaomihaiwai/v1_0_5/R$string;
        /// </summary>
        /// <param name="childPath"></param>
        public void AddChildPath(String childPath)
        {
            if (childPath.StartsWith("L") && childPath.EndsWith(";"))
            {
                // 获取所有子节点名称信息
                String[] A = childPath.Substring(1, childPath.Length - 2).Split('/');

                // 逐级添加子节点
                ClassTree tree = this;
                foreach (String name in A)
                {
                    tree.AddChild(name);        
                    tree = tree.childs[name];
                }

                // 最后一个设置为
                if (tree != null) tree.isDir = false;
            }
        }
    }
}
