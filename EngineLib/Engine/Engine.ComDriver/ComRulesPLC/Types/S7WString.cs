using System;
using System.Linq;
using System.Text;

namespace Engine.ComDriver.Types
{
    public class S7WString
    {
        public static string FromByteArray(byte[] bytes)
        {
            string str;
            if (bytes.Length < 4)
            {
                throw new Exception("Malformed S7 WString / too short");
            }
            int num2 = (bytes[2] << 8) | bytes[3];
            if (num2 > ((bytes[0] << 8) | bytes[1]))
            {
                throw new Exception("Malformed S7 WString / length larger than capacity");
            }
            try
            {
                str = Encoding.BigEndianUnicode.GetString(bytes, 4, num2 * 2);
            }
            catch (Exception )
            {
                throw new Exception("Failed to parse from data.");
            }
            return str;
        }

        public static byte[] ToByteArray(string value, int reservedLength)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (reservedLength > 0x3ffe)
            {
                throw new ArgumentException("The maximum string length supported is 16382.");
            }
            byte[] bytes = new byte[4 + (reservedLength * 2)];
            bytes[0] = (byte)((reservedLength >> 8) & 0xff);
            bytes[1] = (byte)(reservedLength & 0xff);
            bytes[2] = (byte)((value.Length >> 8) & 0xff);
            bytes[3] = (byte)(value.Length & 0xff);
            int num = Encoding.BigEndianUnicode.GetBytes(value, 0, value.Length, bytes, 4) / 2;
            if (num > reservedLength)
            {
                throw new ArgumentException(string.Format("The provided string length ({0} is larger than the specified reserved length ({1}).", num, reservedLength));
            }
            return bytes;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(string strValue)
        {
            if (strValue == null) throw new ArgumentNullException("S7 ToByteArray Value is null");
            byte[] value = Encoding.BigEndianUnicode.GetBytes(strValue);
            if (value.Length > 508)
                throw new ArgumentException("The maximum string length supported is 508.");
            //byte[] head = BitConverter.GetBytes((short)508);
            byte[] head = BitConverter.GetBytes((short)strValue.Length);
            byte[] length = BitConverter.GetBytes((short)strValue.Length);
            Array.Reverse(head);
            Array.Reverse(length);
            head = head.Concat(length).ToArray();
            value = head.Concat(value).ToArray();
            return value;
        }
    }
}
