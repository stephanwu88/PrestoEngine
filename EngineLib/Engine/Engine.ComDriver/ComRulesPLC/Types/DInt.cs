using System;

namespace Engine.ComDriver.Types
{
    /// <summary>
    /// Contains the conversion methods to convert DInt from plc to C# int (Int32).
    /// </summary>
    public static class DInt
    {
        /// <summary>
        /// PLC DInt (4 bytes) to int (Int32)
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns></returns>
        public static Int32 FromByteArray(byte[] bytes, ByteOrder32 byteOrder)
        {
            if (bytes.Length != 4)
                throw new ArgumentException("Wrong number of bytes. Bytes array must contain 4 bytes.");
            byte byteA = bytes[0];
            byte byteB = bytes[1];
            byte byteC = bytes[2];
            byte byteD = bytes[3];
            Int32 diVal = 0;
            if (byteOrder == ByteOrder32.ABCD)
                diVal = FromBytes(byteA, byteB, byteC, byteD);
            else if (byteOrder == ByteOrder32.DCBA)
                diVal = FromBytes(byteD, byteC, byteB, byteA);
            else if (byteOrder == ByteOrder32.BADC)
                diVal = FromBytes(byteB, byteA, byteD, byteC);
            else if (byteOrder == ByteOrder32.CDAB)
                diVal = FromBytes(byteC, byteD, byteA, byteB);
            return diVal;
        }

        /// <summary>
        /// Converts a plc DInt (4 bytes) to int (Int32)
        /// 参数为计算机字节序
        /// </summary>
        /// <param name="v4"></param>
        /// <param name="v3"></param>
        /// <param name="v2"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Int32 FromBytes(byte v4, byte v3, byte v2, byte v1)
        {
            return (Int32)(v1 + v2 * Math.Pow(2, 8) + v3 * Math.Pow(2, 16) + v4 * Math.Pow(2, 24));
        }

        /// <summary>
        /// Converts a int (Int32) to plc DInt (4 bytes)
        /// </summary>
        /// <param name="value">DInt值</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns></returns>
        public static byte[] ToByteArray(Int32 value,ByteOrder32 byteOrder)
        {
            byte[] bytes = new byte[4];
            byte[] RetByte;
            int x = 4;
            long valLong = (long)((Int32)value);
            for (int cnt = 0; cnt < x; cnt++)
            {
                Int64 x1 = (Int64)Math.Pow(256, (cnt));
                Int64 x3 = (Int64)(valLong / x1);
                bytes[x - cnt - 1] = (byte)(x3 & 255);
                valLong -= bytes[x - cnt - 1] * x1;
            }
            if (byteOrder == ByteOrder32.DCBA)
                RetByte = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            else if (byteOrder == ByteOrder32.BADC)
                RetByte = new byte[] { bytes[1], bytes[0], bytes[3], bytes[2] };
            else if (byteOrder == ByteOrder32.CDAB)
                RetByte = new byte[] { bytes[2], bytes[3], bytes[0], bytes[1] };
            else
                RetByte = bytes;
            return RetByte;
        }

        /// <summary>
        /// Converts an array of int (Int32) to an array of bytes
        /// </summary>
        /// <param name="value">DInt数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns></returns>
        public static byte[] ToByteArray(Int32[] value, ByteOrder32 byteOrder)
        {
            ByteArray arr = new ByteArray();
            foreach (Int32 val in value)
                arr.Add(ToByteArray(val, byteOrder));
            return arr.array;
        }

        /// <summary>
        /// Converts an array of plc DInt to an array of int (Int32)
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns>DInt数组</returns>
        public static Int32[] ToArray(byte[] bytes, ByteOrder32 byteOrder)
        {
            Int32[] values = new Int32[bytes.Length / 4];
            int counter = 0;
            for (int cnt = 0; cnt < bytes.Length / 4; cnt++)
                values[cnt] = FromByteArray(new byte[] { bytes[counter++], bytes[counter++], bytes[counter++], bytes[counter++] }, byteOrder);
            return values;
        }
        
        /// <summary>
        /// Converts from C# long (Int64) to C# int (Int32)
        /// </summary>
        public static Int32 CDWord(Int64 value)
        {
            if (value > Int32.MaxValue)
            {
                value -= (long)Int32.MaxValue + 1;
                value = (long)Int32.MaxValue + 1 - value;
                value *= -1;
            }
            return (int)value;
        }
    }
}
