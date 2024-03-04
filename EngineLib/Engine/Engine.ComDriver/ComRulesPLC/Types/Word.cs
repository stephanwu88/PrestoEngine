using System;

namespace Engine.ComDriver.Types
{
    /// <summary>
    /// Contains the conversion methods to convert Words from plc to C#.
    /// </summary>
    public static class Word
    {
        /// <summary>
        /// Converts a word (2 bytes) to ushort (UInt16)
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns></returns>
        public static UInt16 FromByteArray(byte[] bytes, ByteOrder16 byteOrder)
        {
            if (bytes.Length != 2)
                throw new ArgumentException("Wrong number of bytes. Bytes array must contain 2 bytes.");
            UInt16 uiRetVal;
            if (byteOrder == ByteOrder16.BA)
                uiRetVal = FromBytes(bytes[1], bytes[0]);
            else
                uiRetVal = FromBytes(bytes[0], bytes[1]);
            return uiRetVal;
        }

        /// <summary>
        /// Converts a word (2 bytes) to ushort (UInt16)
        /// 参数为计算机字节序
        /// </summary>
        /// <param name="HiVal"></param>
        /// <param name="LoVal"></param>
        /// <returns></returns>
        public static UInt16 FromBytes( byte HiVal,byte LoVal)
        {
            return (UInt16)(HiVal * 256 + LoVal);
        }

        /// <summary>
        /// Converts a ushort (UInt16) to word (2 bytes)
        /// </summary>
        /// <param name="value">Word值</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns>字节数组</returns>
        public static byte[] ToByteArray(UInt16 value, ByteOrder16 byteOrder)
        {
            byte[] bytes = new byte[2];
            byte[] RetByte;
            int x = 2;
            long valLong = (long)((UInt16)value);
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
        /// Converts an array of ushort (UInt16) to an array of bytes
        /// </summary>
        /// <param name="value">Word数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns>字节数组</returns>
        public static byte[] ToByteArray(UInt16[] value, ByteOrder16 byteOrder)
        {
            ByteArray arr = new ByteArray();
            foreach (UInt16 val in value)
                arr.Add(ToByteArray(val,byteOrder));
            return arr.array;
        }

        /// <summary>
        /// Converts an array of bytes to an array of ushort
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns></returns>
        public static UInt16[] ToArray(byte[] bytes, ByteOrder16 byteOrder)
        {
            UInt16[] values = new UInt16[bytes.Length / 2];
            int counter = 0;
            for (int cnt = 0; cnt < bytes.Length / 2; cnt++)
                values[cnt] = FromByteArray(new byte[] { bytes[counter++], bytes[counter++] }, byteOrder);
            return values;
        }

        public static UInt16 SetBitOn(this UInt16 value, int bitOffset)
        {
            return SetBit(value, bitOffset, true);
        }

        public static UInt16 SetBitOff(this UInt16 value, int bitOffset)
        {
            return SetBit(value, bitOffset, false);
        }

        public static UInt16 SetBit(this UInt16 value, int bitOffset, bool bitVal)
        {
            if (bitOffset < 0 || bitOffset > 15)
            {
                throw new ArgumentException("位寻址错误");
            }
            if (bitVal)
                return (UInt16)(value | (0x0001 << bitOffset));
            else
                return (UInt16)(value & ~(0x0001 << bitOffset));
        }
    }
}
