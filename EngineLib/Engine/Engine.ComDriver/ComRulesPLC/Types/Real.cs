using Engine.Common;
using System;

namespace Engine.ComDriver.Types
{
    /// <summary>
    /// Contains the conversion methods to convert Real from plc to C# double.
    /// </summary>
    public static class Real
    {
        /// <summary>
        /// Converts a plc Real (4 bytes) to double
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="byteOrder">字节序</param>
        /// <returns></returns>
        public static double FromByteArray(byte[] bytes, ByteOrder32 byteOrder)
        {
            if (bytes.Length != 4)
                throw new ArgumentException("Wrong number of bytes. Bytes array must contain 4 bytes.");
            byte byteA = bytes[0];
            byte byteB = bytes[1];
            byte byteC = bytes[2];
            byte byteD = bytes[3];
            double real = 0;
            if (byteOrder == ByteOrder32.ABCD)
                real = FromBytes(byteA, byteB, byteC, byteD);
            else if (byteOrder == ByteOrder32.DCBA)
                real = FromBytes(byteD, byteC, byteB, byteA);
            else if (byteOrder == ByteOrder32.BADC)
                real = FromBytes(byteB, byteA, byteD, byteC);
            else if (byteOrder == ByteOrder32.CDAB)
                real = FromBytes(byteC, byteD, byteA, byteB);
            return real;
        }

        /// <summary>
        /// 4字节转化为浮点数
        /// 参数为计算机字节序
        /// </summary>
        /// <param name="v4"></param>
        /// <param name="v3"></param>
        /// <param name="v2"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double FromBytes(byte v4,byte v3,byte v2,byte v1)
        {
            //16进制转换为10进制的公式如下： 
            //SGL = (-1)^SIGN* 1.MANTISSA* 2^(EXPONENT-127)
            if ((int)v1 + v2 + v3 + v4 == 0)
            {
                return 0.0;
            }
            else
            {
                // nun String bilden
                string txt = ValToBinString(v4) + ValToBinString(v3) + ValToBinString(v2) + ValToBinString(v1);
                // erstmal das Vorzeichen
                int vz = int.Parse(txt.Substring(0, 1));
                int exd = sCommon.BinStringToInt32(txt.Substring(1, 8));
                string ma = txt.Substring(9, 23);
                double mantisse = 1;
                double faktor = 1.0;

                //das ist die Anzahl der restlichen bit's
                for (int cnt = 0; cnt <= 22; cnt++)
                {
                    faktor = faktor / 2.0;
                    //entspricht 2^-y
                    if (ma.Substring(cnt, 1) == "1")
                    {
                        mantisse = mantisse + faktor;
                    }
                }
                return Math.Pow((-1), vz) * Math.Pow(2, (exd - 127)) * mantisse;
            }
        }

        /// <summary>
        /// 4字节转换到浮点数
        /// </summary>
        /// <param name="byArr"></param>
        /// <returns></returns>
        private static double FromBytes2(byte data4, byte data3, byte data2, byte data1)
        {
            int data = data4 << 24 | data3 << 16 | data2 << 8 | data1;
            int nSign;
            if ((data & 0x80000000) > 0)
            {
                nSign = -1;
            }
            else
            {
                nSign = 1;
            }
            int nExp = data & (0x7F800000);
            nExp = nExp >> 23;
            double nMantissa = data & (0x7FFFFF);
            if (nMantissa != 0)
                nMantissa = 1 + nMantissa / 8388608;
            double value = nSign * nMantissa * (2 << (nExp - 128));
            return value;
        }

        /// <summary>
        /// Converts a plc DInt to double
        /// </summary>
        /// <param name="value">DInt值</param>
        /// <param name="byteOrder">字节数组</param>
        /// <returns></returns>
        public static double FromDWord(Int32 value, ByteOrder32 byteOrder)
        {
            byte[] b = DInt.ToByteArray(value, byteOrder);
            double d = FromByteArray(b,byteOrder);
            return d;
        }

        /// <summary>
        /// Converts a plc DWord to double
        /// </summary>
        public static double FromDWord(UInt32 value, ByteOrder32 byteOrder)
        {
            byte[] b = DWord.ToByteArray(value, byteOrder);
            double d = FromByteArray(b, byteOrder);
            return d;
        }

        /// <summary>
        /// IEEE浮点数转换到字节数组
        /// 此转换西门子IEEE浮点转换亲测有效
        /// 例如：MD300中的浮点数- MB300-MB303
        /// 等效： ToByteArray(value,ByteOrder32.ABCD);
        /// </summary>
        /// <param name="value">IEEE浮点数</param>
        /// <returns></returns>
        public static byte[] ToByteArray(double value)
        {
            double data = value;
            int nValue = 0;
            int nSign;
            if (data >= 0)
            {
                nSign = 0x00;
            }
            else
            {
                nSign = 0x01;
                data = data * (-1);
            }
            int nHead = (int)data;
            double fTail = data % 1;
            string str = Convert.ToString(nHead, 2);
            int nHead_Length = str.Length;
            nValue = nHead;
            int nShift = nHead_Length;
            while (nShift < 24)   // (nHead_Length + nShift < 23)
            {
                if ((fTail * 2) >= 1)
                {
                    nValue = (nValue << 1) | 0x00000001;
                }
                else
                {
                    nValue = (nValue << 1);
                }
                fTail = (fTail * 2) % 1;
                nShift++;
            }
            int nExp = nHead_Length - 1 + 127;
            nExp = nExp << 23;
            nValue = nValue & 0x7FFFFF;
            nValue = nValue | nExp;
            nSign = nSign << 31;
            nValue = nValue | nSign;
            int data1, data2, data3, data4;
            data1 = nValue & 0x000000FF;
            data2 = (nValue & 0x0000FF00) >> 8;
            data3 = (nValue & 0x00FF0000) >> 16;
            data4 = (nValue >> 24) & 0x000000FF;
            if (data == 0)
            {
                data1 = 0x00;
                data2 = 0x00;
                data3 = 0x00;
                data4 = 0x00;
            }
            return new byte[] { (byte)data4, (byte)data3, (byte)data2, (byte)data1 };
        }

        /// <summary>
        /// Converts a double to plc Real (4 bytes)
        /// </summary>
        /// <param name="value">PLC浮点数值</param>
        /// <param name="byteOrder">字节数</param>
        /// <returns></returns>
        public static byte[] ToByteArray(double value, ByteOrder32 byteOrder)
        {
            double wert = (double)value;
            string binString = "";
            byte[] bytes = new byte[4];
            byte[] RetByte;
            if (wert != 0f)
            {
                if (wert < 0)
                {
                    wert *= -1;
                    binString = "1";
                }
                else
                {
                    binString = "0";
                }
                int exponent = (int)Math.Floor((double)Math.Log(wert) / Math.Log(2.0));
                wert = wert / (Math.Pow(2, exponent)) - 1;

                binString += ValToBinString((byte)(exponent + 127));
                for (int cnt = 1; cnt <= 23; cnt++)
                {
                    if (!(wert - System.Math.Pow(2, -cnt) < 0))
                    {
                        wert = wert - System.Math.Pow(2, -cnt);
                        binString += "1";
                    }
                    else
                        binString += "0";
                }
                bytes[0] = (byte)BinStringToByte(binString.Substring(0, 8));
                bytes[1] = (byte)BinStringToByte(binString.Substring(8, 8));
                bytes[2] = (byte)BinStringToByte(binString.Substring(16, 8));
                bytes[3] = (byte)BinStringToByte(binString.Substring(24, 8));

            }
            else
            {
                bytes[0] = 0;
                bytes[1] = 0;
                bytes[2] = 0;
                bytes[3] = 0;
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
        /// Converts an array of double to an array of bytes 
        /// </summary>
        public static byte[] ToByteArray(double[] value, ByteOrder32 byteOrder)
        {
            ByteArray arr = new ByteArray();
            foreach (double val in value)
                arr.Add(ToByteArray(val, byteOrder));
            return arr.array;
        }

        /// <summary>
        /// Converts an array of plc Real to an array of double
        /// </summary>
        public static double[] ToArray(byte[] bytes, ByteOrder32 byteOrder)
        {
            double[] values = new double[bytes.Length / 4];

            int counter = 0;
            for (int cnt = 0; cnt < bytes.Length / 4; cnt++)
                values[cnt] = FromByteArray(new byte[] { bytes[counter++], bytes[counter++], bytes[counter++], bytes[counter++] }, byteOrder);

            return values;
        }

        /// <summary>
        /// 浮点数转10进制
        /// </summary>
        /// <returns></returns>
        public static int ToInt(this double value, ByteOrder32 byteOrder)
        {
            byte[] byData = ToByteArray(value, byteOrder);
            return DInt.FromByteArray(byData, byteOrder);
        }
        
        private static string ValToBinString(byte value)
        {
            string txt = "";

            for (int cnt = 7; cnt >= 0; cnt += -1)
            {
                if ((value & (byte)Math.Pow(2, cnt)) > 0)
                    txt += "1";
                else
                    txt += "0";
            }
            return txt;
        }
        
        private static byte? BinStringToByte(string txt)
        {
            int cnt = 0;
            int ret = 0;

            if (txt.Length == 8)
            {
                for (cnt = 7; cnt >= 0; cnt += -1)
                {
                    if (int.Parse(txt.Substring(cnt, 1)) == 1)
                    {
                        ret += (int)(Math.Pow(2, (txt.Length - 1 - cnt)));
                    }
                }
                return (byte)ret;
            }
            return null;
        }
    }
}
