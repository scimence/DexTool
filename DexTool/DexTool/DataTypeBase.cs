using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DexTool
{
    /// <summary>
    /// 数据类型基类
    /// </summary>
    public class DataTypeBase
    {
        private static Dictionary<string, FileStream> FileStreamDic = new Dictionary<string, FileStream>(); // 记录指向文件的FileStream
        private static Dictionary<string, long> FileStreamOcupyDic = new Dictionary<string, long>();        // 记录指向文件的FileStream占用数


        /// <summary>
        /// 待解析的数据文件路径
        /// </summary>
        string filePath = "";

        /// <summary>
        /// 文件创建输入流
        /// </summary>
        FileStream FileIn;

        /// <summary>
        /// 数据区块<名称, 字节大小>
        /// </summary>
        Dictionary<string, long> FieldsSize = new Dictionary<string, long>();

        /// <summary>
        /// 数据区块<名称, 起始地址>
        /// </summary>
        Dictionary<string, long> FieldsAddress = new Dictionary<string, long>();

        /// <summary>
        /// 当前偏移地址
        /// </summary>
        long curAddress = 0;


        /// <summary>
        /// 创建DataTypeBase
        /// </summary>
        /// <param name="filePath">数据对应的文件路径</param>
        public DataTypeBase(string filePath)
        {
            //输入文件校验
            if (filePath == null || !System.IO.File.Exists(filePath))
            {
                throw new Exception("DataTypeBase创建错误，文件不存在：" + filePath);
            }
            else
            {
                this.filePath = filePath;                           // 记录文件路径
                if (!FileStreamDic.ContainsKey(filePath))
                {
                    FileIn = new FileStream(filePath, FileMode.Open);   // 创建文件输入流

                    FileStreamDic.Add(filePath, FileIn);            // 记录文件流信息
                    FileStreamOcupyDic.Add(filePath, 1);            
                }
                else
                {
                    FileIn = FileStreamDic[filePath];               // 获取对应的文件流
                    FileStreamOcupyDic[filePath]++;                 // 占用数加1
                }
            }
        }

        /// <summary>
        /// 在析构函数调用时，关闭文件输入流
        /// </summary>
        ~DataTypeBase()
        {
            if (FileIn != null)
            {
                FileStreamOcupyDic[filePath]--;
                if (FileStreamOcupyDic[filePath] == 0)
                {
                    FileIn.Close();
                    FileStreamDic.Remove(filePath);
                    FileStreamOcupyDic.Remove(filePath);
                }

                FileIn = null;
            }
        }

        /// <summary>
        /// 添加数据块
        /// </summary>
        /// <param name="fieldName">数据块名称</param>
        /// <param name="fieldSize">大小/byte</param>
        public void AddFiled(string fieldName, long fieldSize)
        {
            if (FieldsSize.Keys.Contains(fieldName))
                throw new Exception("数据块名称" + fieldName + "已存在，不可重复添加，请使用其他名称！");
            else
            {
                FieldsSize.Add(fieldName, fieldSize);       // 记录数据块大小
                FieldsAddress.Add(fieldName, curAddress);   // 记录数据块起始地址

                curAddress += fieldSize;                    // 修改至新的位置
            }
        }

        /// <summary>
        /// 添加拓展区块信息
        /// </summary>
        /// <param name="newFieldName">拓展区块名称</param>
        /// <param name="fieldName_Off">区块偏移值</param>
        /// <param name="fieldName_Size">区块大小</param>
        public void AddExtendsField(string newFieldName, string fieldName_Off, string fieldName_Size)
        {
            long off = GetField_Long(fieldName_Off);
            long size = GetField_Long(fieldName_Size);
            //if (newFieldName.Equals("@Ex_" + "string_ids")) size *= 4;

            if (FieldsSize.Keys.Contains(newFieldName))
                throw new Exception("数据块名称" + newFieldName + "已存在，不可重复添加，请使用其他名称！");
            else
            {
                FieldsSize.Add(newFieldName, size);     // 记录数据块大小
                FieldsAddress.Add(newFieldName, off);   // 记录数据块起始地址
            }
        }


        /// <summary>
        /// 获取所有Field名称信息
        /// </summary>
        /// <returns></returns>
        public string[] GetFieldNames()
        {
            return FieldsSize.Keys.ToArray();
        }

        /// <summary>
        /// 获取数据块fieldName中，index位置的byte数据
        /// </summary>
        /// <param name="fieldName">数据块名称</param>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public byte GetFieldByte(string fieldName, long index)
        {
            byte value = 0;

            long postion = FieldsAddress[fieldName];        // 获取数据块起始位置
            FileIn.Seek(postion + index, SeekOrigin.Begin); // 定位流至数据块index字节处
            value = (byte) FileIn.ReadByte();               // 读取1个字节

            return value;
        }


        /// <summary>
        /// 获取数据块fieldName中，index位置的byte数据
        /// </summary>
        /// <param name="fieldName">数据块名称</param>
        /// <param name="size">数据块大小</param>
        /// <returns></returns>
        public byte[] GetFieldBytes(string fieldName, long size = -1)
        {
            if (size == -1) size = FieldsSize[fieldName];

            byte[] data = new byte[(int)size];

            long postion = FieldsAddress[fieldName];        // 获取数据块起始位置
            FileIn.Seek(postion, SeekOrigin.Begin);         // 定位流至数据块处
            int readLen = FileIn.Read(data, 0, (int) size);

            if (readLen == 0) return new byte[0];
            else return data;
        }

        /// <summary>
        /// 从文件任意位置开始读取任意大小
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] GetBytes(long position = 0, long size = 1)
        {
            byte[] data = new byte[(int)size];

            FileIn.Seek(position, SeekOrigin.Begin);         // 定位流至数据块处
            int readLen = FileIn.Read(data, 0, (int)size);  // 读取size大小个字节

            if (readLen == 0) return new byte[0];
            else return data;
        }

        /// <summary>
        /// 从position位置获取字符串
        /// </summary>
        /// <param name="position">起始位置</param>
        /// <returns></returns>
        public string GetString(long position)
        {
            FileIn.Seek(position, SeekOrigin.Begin);         // 定位流至数据块处
            int B = 0;

            List<byte> tmp = new List<byte>();
            while ((B = FileIn.ReadByte()) != 0)            // 一直读取到0
            {
                tmp.Add((byte)B);
            }

            string Str = Byter.ToStr_UTF8(tmp.ToArray());
            return Str;
        }

        #region 数据格式转化函数

        /// <summary>
        /// 获取Field的字符串形式值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetField_Str_Hex(string fieldName)
        {
            byte[] data = GetFieldBytes(fieldName);
            return Byter.ToHexStr(data);
        }

        /// <summary>
        /// 获取Field的字符串形式值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetField_Str(string fieldName)
        {
            byte[] data = GetFieldBytes(fieldName);
            return Byter.ToStr_UTF8(data);
        }

        /// <summary>
        /// 获取Field的LEB128字符串形式值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetField_Str_LEB128(string fieldName)
        {
            byte[] data = GetFieldBytes(fieldName);
            return Byter.To_LEB128_Str(data);
        }

        /// <summary>
        /// 获取Field的LEB128表示的值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public long GetField_Long_LEB128(string fieldName)
        {
            byte[] data = GetFieldBytes(fieldName);
            return Byter.To_LEB128_Long(data);
        }


        /// <summary>
        /// 获取Field表示的数值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public long GetField_Long(string fieldName)
        {
            byte[] data = GetFieldBytes(fieldName);
            return Byter.To_Long(data);
        }
        #endregion


        /// <summary>
        /// 获取Field对应数据的所有Byter解析信息
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetField_ByterStr(string fieldName)
        {
            byte[] data = GetFieldBytes(fieldName); // 获取field对应数据
            Byter byter = new Byter(data);          // 对数据进行解析

            if(fieldName.StartsWith("@Ex_" )) return byter.ToString2();
            return byter.ToString();
        }

    }

}
