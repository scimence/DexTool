using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DexTool
{
    /// <summary>
    /// 对Byte数据进行解析、格式转化
    /// </summary>
    public class Byter
    {
        public byte[] bytes;    // byte数据
        public string HexStr;   // 16进制数据
        public string BinStr;   // 二进制数据
        public string Str_UTF8; // 普通字符串形式
        public long _long;      // long值

        public byte[] LEB128_bytes; // 从bytes按LEB128格式解析后的byte数据
        public string LEB128_HexStr;// 从bytes按LEB128格式解析后的16进制数据
        public string LEB128_BinStr;// 从bytes按LEB128格式解析后的二进制数据
        public string LEB128_Str;   // LEB128格式表示的字符串
        public long LEB128_long;    // LEB128格式表示的值

        public Byter(byte[] bytes)
        {
            this.bytes = bytes;
            HexStr = ToHexStr(bytes);
            BinStr = ToBinStr(bytes);
            Str_UTF8 = ToStr_UTF8(bytes);
            _long = To_Long(bytes);

            LEB128_bytes = To_LEB128_Bytes(bytes);
            LEB128_HexStr = To_LEB128_HexStr(bytes);
            LEB128_BinStr = To_LEB128_BinStr(bytes);
            LEB128_Str = To_LEB128_Str(bytes);
            LEB128_long = To_LEB128_Long(bytes);
        }

        public String ToString()
        {
            string Str = "";

            Str += "HexStr:" + "\t" + HexStr + "\r\n";
            Str += "BinStr:" + "\t" + BinStr + "\r\n";
            Str += "_long:" + "\t" + _long + "\r\n";
            Str += "Str_UTF8:" + "\t" + Str_UTF8.Replace("\0", "\\0") + "\r\n";
            Str += "\r\n";
            Str += "LEB128_HexStr:" + "\t" + LEB128_HexStr + "\r\n";
            Str += "LEB128_BinStr:" + "\t" + LEB128_BinStr + "\r\n";
            Str += "LEB128_long:" + "\t" + LEB128_long + "\r\n";
            Str += "LEB128_Str:" + "\t" + LEB128_Str.Replace("\0", "\\0") + "\r\n";

            return Str;
        }

        public String ToString2(bool native = false)
        {
            string Str = "";

            Str += "HexStr:" + "\t" + HexStr + "\r\n";
            Str += "BinStr:" + "\t" + BinStr + "\r\n";
            Str += "_long:" + "\t" + _long + "\r\n";
            Str += "Str_UTF8:" + "\t" + Str_UTF8 + "\r\n";
            Str += "\r\n";
            Str += "LEB128_HexStr:" + "\t" + LEB128_HexStr + "\r\n";
            Str += "LEB128_BinStr:" + "\t" + LEB128_BinStr + "\r\n";
            Str += "LEB128_long:" + "\t" + LEB128_long + "\r\n";
            Str += "LEB128_Str:" + "\t" + LEB128_Str + "\r\n";

            return Str;
        }


        #region byte转16进制串

        /// <summary>  
        /// 转化为16进制串
        /// </summary>  
        public static string ToHexStr(byte[] B)
        {
            StringBuilder Str = new StringBuilder();
            foreach (byte b in B)
            {
                Str.Append(ToHexStr(b) + " ");
            }
            return Str.ToString();
        }

        private static string ToHexStr(byte b)
        {
            return "" + ToChar(b / 16) + ToChar(b % 16);
        }

        private static char ToChar(int n)
        {
            if (0 <= n && n <= 9) return (char)('0' + n);
            else if (10 <= n && n <= 35) return (char)('a' + n - 10);
            else return ' ';
        }
        #endregion

        #region 16进制串转byte

        /// <summary>  
        /// 解析字符串为Bytes数组
        /// </summary>  
        public static byte[] HexToBytes(string data)
        {
            byte[] B = new byte[data.Length / 2];
            char[] C = data.ToLower().ToCharArray();

            for (int i = 0; i < C.Length; i += 2)
            {
                byte b = HexToByte(C[i], C[i + 1]);
                B[i / 2] = b;
            }

            return B;
        }

        /// <summary>  
        /// 每两个字母还原为一个字节  
        /// </summary>  
        private static byte HexToByte(char a1, char a2)
        {
            return (byte)( HexCValue(a1) * 16 + HexCValue(a2) );
        }

        /// <summary>
        /// 0-9a-z映射为对应值
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static int HexCValue(char c)
        {
            if ('0' <= c && c <= '9') return c - '0';
            else if ('a' <= c && c <= 'z') return c - 'a' + 10;
            else return 0;
        }

        #endregion



        #region byte转2进制串

        /// <summary>
        /// 转化为二进制串
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        public static string ToBinStr(byte[] B)
        {
            StringBuilder Str = new StringBuilder();
            foreach (byte b in B)
            {
                Str.Append(ToBinStr(b) + " ");
            }
            return Str.ToString();
        }

        private static string ToBinStr(byte b)
        {
            String Str = "";
            for (int i = 0; i < 8; i++)
            {
                Str = (b % 2 == 1 ? "1" : "0") + Str;
                b = (byte)((int)b >> 1);
            }
            return Str;
        }

        #endregion


        #region byte转String

        public static string ToStr_UTF8(byte[] B)
        {
            string str = Encoding.UTF8.GetString(B);
            return str;
        }

        #endregion



        /// <summary>
        /// 转换LEB128表示的数值
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        public static long To_Long(byte[] B)
        {
            long N = -1;
            for (int i = B.Length - 1; i >= 0; i--)
            //foreach (byte b in B_LE128)
            {
                byte b = B[i];
                if (N == -1) N = b;
                else
                {
                    N = (N << 8) + b;
                }
            }
            return N;
        }


        #region byte转LE128格式String

        public static byte[] To_LEB128_Bytes(byte[] B)
        {
            List<byte> L = new List<byte>();
            foreach (byte b in B)
            {
                if (((int)b & 0x80) != 0)   // 若最高位为1，则移除最高位，继续读取下一个byte
                {
                    L.Add((byte)((int)b & 0x7f));
                }
                else
                {                           // 若最高位为0，则不再读取下一个byte
                    L.Add(b);
                    break;
                }
            }
            return L.ToArray<byte>();
        }

        /// <summary>
        /// 转换LEB128表示的16进制字符串
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        public static string To_LEB128_HexStr(byte[] B)
        {
            byte[] B_LE128 = To_LEB128_Bytes(B);
            string str = ToHexStr(B_LE128);
            return str;
        }

        
        /// <summary>
        /// 转换LEB128表示的2进制字符串
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        public static string To_LEB128_BinStr(byte[] B)
        {
            byte[] B_LE128 = To_LEB128_Bytes(B);
            string str = ToBinStr(B_LE128);
            return str;
        }


        # region LEB128有效数据还原

        private static byte[] To_LEB128_Bytes_pre(byte[] B)
        {
            List<byte> pre = new List<byte>();
            List<bool> byteTmp = new List<bool>();

            byte[] B_LE128 = To_LEB128_Bytes(B);
            foreach (byte b in B_LE128)             // 读取数据中所有byte
            {
                List<bool> bit = To_LEB128_Bit(b);  // 获取LEB128格式表示的有效位
                foreach (bool bi in bit)
                {
                    byteTmp.Add(bi);                // 按位添加至byteTmp中
                    if (byteTmp.Count == 8)
                    {
                        byte preByte = To_byte(byteTmp); // 每8位还原为一个字节
                        pre.Add(preByte);

                        byteTmp.Clear();
                    }
                }
            }

            if (byteTmp.Count > 0)                  // 所有字节读取完成，剩余位大于0不足8位
            {
                byte preByte = To_byte(byteTmp);    // 还原为一个字节
                if(preByte != 0) pre.Add(preByte);  // 不足8为且为0时不添加
            }

            return pre.ToArray();
        }

        /// <summary>
        /// 将byte转化为2进制形式
        /// </summary>
        private static List<bool> To_LEB128_Bit(byte b0)
        {
            int b = (int) b0;
            List<bool> BitL = new List<bool>();
            for (int i = 0; i < 7; i++)
            {
                if ((b & 1) != 0) BitL.Add(true);
                else BitL.Add(false);

                b = b >> 1;
            }

            return BitL;
        }

        /// <summary>
        /// 将bit数据转化为byte,最高位在最后
        /// </summary>
        private static byte To_byte(List<bool> BitL)
        {
            int n = 0;
            for(int i= BitL.Count-1; i>=0; i--)
            {
                n = (n << 1) | (BitL[i] ? 1 : 0);
            }
            return (byte)n;
        }

        #endregion



        /// <summary>
        /// 转换LEB128表示的字符串
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        public static string To_LEB128_Str(byte[] B)
        {
            byte[] B_pre = To_LEB128_Bytes_pre(B);    // 还原LEB128编码为原数据

            string str = Encoding.UTF8.GetString(B_pre);  // 还原为字符串
            return str;
        }



        /// <summary>
        /// 转换LEB128表示的数值
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        public static long To_LEB128_Long(byte[] B)
        {
            long N = -1;
            byte[] B_pre = To_LEB128_Bytes_pre(B);    // 还原LEB128编码为原数据

            for (int i = B_pre.Length - 1; i >= 0; i--)
            //foreach (byte b in B_LE128)
            {
                byte b = B_pre[i];
                if (N == -1) N = b;
                else
                {
                    N = (N << 8) + b;
                }
            }
            return N;
        }

        #endregion
    }
}
