using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DexTool
{
    /// <summary>
    /// Dex文件格式处理类
    /// </summary>
    public class Dex : DataTypeBase
    {
        /// <summary>
        /// Dex文件格式，Field定义
        /// </summary>
        /// <param name="fileName"></param>
        public Dex(string fileName):base(fileName)
        {
            AddFiled("magic", 8);
            AddFiled("checksum", 4);
            AddFiled("siganature", 20);
            AddFiled("file_size", 4);
            AddFiled("header_size", 4);
            AddFiled("endian_tag", 4);

            AddFiled("link_size", 4);
            AddFiled("link_off", 4);
            AddFiled("map_off", 4);

            AddFiled("string_ids_size", 4);
            AddFiled("string_ids_off", 4);
            AddFiled("type_ids_size", 4);
            AddFiled("type_ids_off", 4);
            AddFiled("proto_ids_size", 4);
            AddFiled("proto_ids_off", 4);
            AddFiled("field_ids_size", 4);
            AddFiled("field_ids_off", 4);
            AddFiled("method_ids_size", 4);
            AddFiled("method_ids_off", 4);
            AddFiled("class_defs_size", 4);
            AddFiled("class_defs_off", 4);
            AddFiled("data_size", 4);
            AddFiled("data_off", 4);

            GenExtendsField();
            LoadFieldInfos();
        }

        private void GenExtendsField()
        {
            string[] fieldNames = GetFieldNames();
            List<string> tmp = new List<string>();
            foreach (string name0 in fieldNames)
            {
                string name = name0;
                if (name.EndsWith("_size"))
                {
                    name = name.Substring(0, name.Length - 5);
                    if (tmp.Contains(name + "_off"))
                    {
                        AddExtendsField("@Ex_" + name, name + "_off", name + "_size");
                    }
                    else
                    {
                        tmp.Add(name + "_size");
                    }
                }
                else if (name.EndsWith("_off"))
                {
                    name = name.Substring(0, name.Length - 4);
                    if (tmp.Contains(name + "_size"))
                    {
                        AddExtendsField("@Ex_" + name, name + "_off", name + "_size");
                    }
                    else
                    {
                        tmp.Add(name + "_off");
                    }
                }
            }
        }


        public string magic;        // 文件类型标识
        public string checksum;     // 校验位
        public string siganature;   // 签名信息
        public long file_size = 0;  // Dex文件大小
        public long header_size = 0;// header大小
        public string endian_tag;

        public List<string> string_ids = new List<string>();

        
        private void LoadFieldInfos()
        {
            magic = GetField_Str("magic");
            checksum = GetField_Str_Hex("checksum");
            siganature = GetField_Str_Hex("siganature");
            file_size = GetField_Long("file_size");
            header_size = GetField_Long("header_size");
            endian_tag = GetField_Str("endian_tag");

            LoadStringIds();
        }

        /// <summary>
        /// 载入所有StringId数据
        /// </summary>
        private void LoadStringIds()
        {
            long off = GetField_Long("string_ids_off");
            long count = GetField_Long("string_ids_size");

            for (int i = 0; i < count; i++)
            {
                byte[] data = GetBytes(off + i * 4, 4);
                long StringIndex = Byter.To_Long(data);

                string str = GetString(StringIndex + 1);
                string_ids.Add(str);
            }
        }


        public string Test(string filedName)
        {
            byte[] data = GetFieldBytes(filedName);
            Byter byter = new Byter(data);

            //byte[] B = new byte[] { 0x02, 0xB0 };
            //string B_Str = Byter.ToHexStr(B);
            //string LEB128_B_Str = Byter.To_LEB128_HexStr(B);
            //long L = Byter.To_LEB128_Long(B);

            return byter.ToString();
        }


        # region 绑定控件，查看显示field信息

        /// <summary>
        /// 将field区块与控件绑定，实现field信息查看
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="text"></param>
        public void BindField(ComboBox comboBox, TextBox text)
        {
            // 记录绑定的控件
            BindComboBox = comboBox;
            BindTextBox = text;

            // ComBox添加下拉选择逻辑
            comboBox.Items.Clear();
            comboBox.Items.AddRange(GetFieldNames());

            BindComboBox.SelectedIndexChanged += new System.EventHandler(comboBox_SelectedIndexChanged);

        }
        ComboBox BindComboBox;
        TextBox BindTextBox;

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fieldName = BindComboBox.SelectedItem.ToString().Trim();     // 获取选中的field名称
            BindTextBox.Text = GetField_ByterStr(fieldName);                    // 显示解析信息至TextBox
        }

        #endregion


    }
}
