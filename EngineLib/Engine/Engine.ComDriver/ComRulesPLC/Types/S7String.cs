using System;
using System.Linq;
using System.Text;

namespace Engine.ComDriver.Types
{
    public static class S7String
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FromByteArray(byte[] bytes)
        {
            string strValue = string.Empty;
            if (bytes.Length < 2) throw new Exception("Malformed S7 String / too short");
            int num = bytes[0];
            int count = bytes[1];
            if (count > num)
                throw new Exception("Malformed S7 String / length larger than capacity");
            try
            {
                strValue = Encoding.Default.GetString(bytes, 2, count);
            }
            catch (Exception )
            {
                throw new Exception("Failed to parse from data.");
            }
            return strValue;
        }

        /// <summary>
        /// 将字符串转化为西门子格式字节数字
        /// 西门子字符串格式：
        /// 字符串定义长度 （1字节） + 数据字节长度（1字节）+  C# 字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(string value)
        {
            if (value == null)  throw new ArgumentNullException("S7 ToByteArray Value is null");
            byte[] byValue = Encoding.Default.GetBytes(value);
            byte[] head = new byte[2];
            int byCnt = byValue.Length;
            if (byCnt > 254)
                throw new ArgumentException("The maximum string length supported is 254.");
            head[0] = Convert.ToByte(byCnt);
            head[1] = Convert.ToByte(byCnt);
            byValue = head.Concat(byValue).ToArray();
            return byValue;
        }
    }
}
