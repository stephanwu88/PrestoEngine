using System;

namespace Engine.ComDriver.Types
{
    /// <summary>
    /// Contains the methods to convert from bytes to byte arrays
    /// </summary>
    public static class Byte
    {
        /// <summary>
        /// Converts a byte to byte array
        /// </summary>
        public static byte[] ToByteArray(byte value)
        {
            byte[] bytes = new byte[] { value};
            return bytes;
        }
       
        /// <summary>
        /// Converts a byte array to byte
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte FromByteArray(byte[] bytes)
        {
            if (bytes.Length != 1)
            {
                throw new ArgumentException("Wrong number of bytes. Bytes array must contain 1 bytes.");
            }
            return bytes[0];
        }

        public static byte SetBitOn(this byte value, int bitOffset)
        {
            return SetBit(value, bitOffset, true);
        }

        public static byte SetBitOff(this byte value, int bitOffset)
        {
            return SetBit(value, bitOffset, false);
        }

        public static byte SetBit(this byte value, int bitOffset, bool bitVal)
        {
            if (bitOffset < 0 || bitOffset > 7)
            {
                throw new ArgumentException("位寻址错误");
            }

            if (bitVal)
                return (byte)((UInt16)value | (0x0001 << bitOffset));
            else
                return (byte)((UInt16)value & ~(0x0001 << bitOffset));
        }
        
    }
}
