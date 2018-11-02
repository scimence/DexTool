using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DexTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Dex dex;

        private void button1_Click(object sender, EventArgs e)
        {
            //string filePath = @"F:\sc\桌面快捷存储\网游sdk\20180926\classes.dex";
            string filePath = textBox2.Text.Trim();
            //FileInfo info = new FileInfo(filePath);
            //long fileLen = info.Length;

            dex = new Dex(filePath);
            dex.BindField(comboBox1, textBox1);
        }

        /// <summary>
        /// 双击时选中所有文本
        /// </summary>
        private void textBox_DoubleClick(object sender, EventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            string hexStr = (sender as TextBox).Text.Trim().Replace(" ", "");
            byte[] data = Byter.HexToBytes(hexStr); // 解析16进制数据为byte

            Byter byter = new Byter(data);          // 数据转码
            textBox4.Text = checkBox1.Checked ? byter.ToString() : byter.ToString2();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            long _off = long.Parse(textBox6.Text.Trim());
            long _size = long.Parse(textBox7.Text.Trim());

            byte[] data = dex.GetBytes(_off, _size);
            textBox5.Text = Byter.ToHexStr(data); 
        }

        private void dEXStringsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dex.stringList.Count > 0)
            {
                StringForm form = new StringForm(dex.stringList.ToArray());
                form.Show();
            }
        }

        /// <summary>
        /// 以文件目录树的形式展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dEXExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            List<string> classes = dex.getClasses();
            if (classes.Count > 0)
                new ClassesForm(classes).Show();
        }
    }
}
