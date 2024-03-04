using System;

namespace Engine.ComDriver.Types
{
    /// <summary>
    /// Contains the conversion methods to convert Int from plc to C#.
    /// </summary>
    public static class Int
    {
        /// <summary>
        /// Converts a S7 Int (2 bytes) to short (Int16)
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns></returns>
        public static Int16 FromByteArray(byte[] bytes,ByteOrder16 byteOrder)
        {
            if (bytes.Length != 2)
                throw new ArgumentException("Wrong number of bytes. Bytes array must contain 2 bytes.");
            Int16 iRetVal;
            if (byteOrder == ByteOrder16.BA)
                iRetVal = FromBytes(bytes[1], bytes[0]);
            else
                iRetVal = FromBytes(bytes[0], bytes[1]);
            return iRetVal;
        }

        /// <summary>
        /// Converts a plc Int (2 bytes) to short (Int16)
        /// 参数为计算机字节序
        /// </summary>
        /// <param name="HiVal"></param>
        /// <param name="LoVal"></param>
        /// <returns></returns>
        public static Int16 FromBytes(byte HiVal, byte LoVal)
        {
            return (Int16)(HiVal * 256 + LoVal);
        }

        /// <summary>
        /// Converts a short (Int16) to a plc Int byte array (2 bytes)
        /// </summary>
        /// <param name="value">Int值</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns></returns>
        public static byte[] ToByteArray(Int16 value, ByteOrder16 byteOrder)
        {
            byte[] bytes = new byte[2];
            byte[] RetByte;
            int x = 2;
            long valLong = (long)((Int16)value);
            for (int cnt = 0; cnt < x; cnt++)
            {
                Int64 x1 = (Int64)Math.Pow(256, (cnt));
                Int64 x3 = (Int64)(valLong / x1);
                bytes[x - cnt - 1] = (byte)(x3 & 255);
                valLong -= bytes[x - cnt - 1] * x1;
            }
            if (byteOrder == ByteOrder16.BA)
                RetByte = new byte[] { bytes[1], bytes[0] };
            else
                RetByte = bytes;
            return RetByte;
        }

        /// <summary>
        /// Converts an array of short (Int16) to a plc Int byte array (2 bytes)
        /// </summary>
        /// <param name="value">Int数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns>字节数组</returns>
        public static byte[] ToByteArray(Int16[] value, ByteOrder16 byteOrder)
        {
            ByteArray arr = new ByteArray();
            foreach (Int16 val in value)
                arr.Add(ToByteArray(val, byteOrder));
            return arr.array;
        }

        /// <summary>
        /// Converts an array of plc Int to an array of short (Int16)
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns>Int数组</returns>
        public static Int16[] ToArray(byte[] bytes, ByteOrder16 byteOrder)
        {
            Int16[] values = new Int16[bytes.Length / 2];
            int counter = 0;
            for (int cnt = 0; cnt < bytes.Length / 2; cnt++)
                values[cnt] = FromByteArray(new byte[] { bytes[counter++], bytes[counter++] }, byteOrder);
            return values;
        }
        
        /// <summary>
        /// Converts a C# int value to a C# short value, to be used as word.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int16 CWord(int value)
        {
            if (value > 32767)
            {
                value -= 32768;
                value = 32768 - value;
                value *= -1;
            }
            return (short)value;
        }

    }
}
