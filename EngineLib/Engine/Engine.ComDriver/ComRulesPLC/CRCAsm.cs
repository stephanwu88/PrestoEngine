using System;
using System.Text;

namespace Engine.ComDriver
{
    /// <summary>
    /// 校验方式
    /// </summary>
    public enum CheckType
    {
        CRC16_L_H,
        CRC16_H_L,
        CRC32_L_H,
        CRC32_H_L,
        CheckSum,
        CheckXor,
        CRC8
    }

    public class CRCASM
    {
        public static string GenCheckCode(CheckType ChkType, string msg)
        {
            msg = msg.Replace("\r", "").Replace("\r", "").Replace("\t", "");
            byte[] bytes = strToHexByte(msg);
            string checkRlt = "";
            switch (ChkType)
            {
                case CheckType.CRC16_L_H:
                    byte[] buff = CRC16.CRC(bytes, true);      
                    checkRlt = ByteToOctString(buff);
                    break;
                case CheckType.CRC16_H_L:
                    buff = CRC16.CRC(bytes, false);      
                    checkRlt = ByteToOctString(buff);
                    break;
                case CheckType.CRC32_L_H:
                    int crc = CRC32.CRC(bytes);
                    byte b1 = (byte)((crc & 0xFF000000) >> 24);  //高位置
                    byte b2 = (byte)((crc & 0xFF0000) >> 16);    //低位置
                    byte b3 = (byte)((crc & 0xFF00) >> 8);       //高位置
                    byte b4 = (byte)(crc & 0xFF);                //低位置
                    buff = new byte[4] { b4, b3, b2, b1 };
                    //H->L
                    checkRlt = ByteToOctString(buff);
                    break;
                case CheckType.CRC32_H_L:
                    crc = CRC32.CRC(bytes);
                    b1 = (byte)((crc & 0xFF000000) >> 24);  //高位置
                    b2 = (byte)((crc & 0xFF0000) >> 16);    //低位置
                    b3 = (byte)((crc & 0xFF00) >> 8);       //高位置
                    b4 = (byte)(crc & 0xFF);                //低位置
                    buff = new byte[4] { b1, b2, b3, b4 };
                    //H->L
                    checkRlt = ByteToOctString(buff);
                    break;
                case CheckType.CheckSum:
                    byte checkSum = 0;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        checkSum += bytes[i];
                    }
                    checkRlt = checkSum.ToString("X2");
                    break;
                case CheckType.CheckXor:
                    checkSum = 0;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        checkSum ^= bytes[i];
                    }
                    checkRlt = checkSum.ToString("X2");
                    break;
                case CheckType.CRC8:
                    int c8 = CRC8.CRC(bytes);
                    checkRlt = c8.ToString("X2");
                    break;
            }
            return checkRlt;
        }

        public static byte[] strToHexByte(string hex)
        {
            hex = hex.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
            byte[] bytes = new byte[(hex.Length + 1) / 2];
            string h = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    try
                    {
                        h = hex.Substring(i * 2, 2);
                    }
                    catch             
                    {
                        h = hex.Substring(i * 2);
                    }
                    // 每两个字符是一个 byte。 
                    bytes[i] = byte.Parse(h,
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Rethrow an exception with custom message. 
                    //MessageBox.Show(Lang.Get("16进制数输入格式不正确!"), h);
                }
            }
            return bytes;
        }

        public static string ByteToOctString(byte[] buff, bool AddSpace = true)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buff.Length; i++)
            {
                sb.Append(buff[i].ToString("X2"));
                if (AddSpace)
                    sb.Append(' ');
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// 查表和下面的算法一致
    /// </summary>
    public class CRC8
    {
        /// <summary> 
        /// CRC8位校验表 
        /// </summary> 
        private static byte[] CRC8Table = new byte[] {
                        0,94,188,226,97,63,221,131,194,156,126,32,163,253,31,65,
                        157,195,33,127,252,162,64,30, 95,1,227,189,62,96,130,220,
                        35,125,159,193,66,28,254,160,225,191,93,3,128,222,60,98,
                        190,224,2,92,223,129,99,61,124,34,192,158,29,67,161,255,
                        70,24,250,164,39,121,155,197,132,218,56,102,229,187,89,7,
                        219,133,103,57,186,228,6,88,25,71,165,251,120,38,196,154,
                        101,59,217,135,4,90,184,230,167,249,27,69,198,152,122,36,
                        248,166,68,26,153,199,37,123,58,100,134,216,91,5,231,185,
                        140,210,48,110,237,179,81,15,78,16,242,172,47,113,147,205,
                        17,79,173,243,112,46,204,146,211,141,111,49,178,236,14,80,
                        175,241,19,77,206,144,114,44,109,51,209,143,12,82,176,238,
                        50,108,142,208,83,13,239,177,240,174,76,18,145,207,45,115,
                        202,148,118,40,171,245,23,73,8,86,180,234,105,55,213,139,
                        87,9,235,181,54,104,138,212,149,203, 41,119,244,170,72,22,
                        233,183,85,11,136,214,52,106,43,117,151,201,74,20,246,168,
                        116,42,200,150,21,75,169,247,182,232,10,84,215,137,107,53 };

        public static byte CRC(byte[] buffer)
        {
            return CRC(buffer, 0, buffer.Length);
        }

        public static byte CRC(byte[] buffer, int off, int len)
        {
            byte crc = 0;
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (off < 0 || len < 0 || off + len > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            for (int i = off; i < len; i++)
            {
                crc = CRC8Table[crc ^ buffer[i]];
            }
            return crc;
        }

        /// <summary>
        /// 和上面的查表算法一致       
        /// </summary>
        public static byte CRC_8(byte[] buffer)
        {
            byte crc = 0;
            for (int j = 0; j < buffer.Length; j++)
            {
                crc ^= buffer[j];
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x01) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0x8c;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }
    }

    /// <summary>
    /// CRC16算法
    /// </summary>
    public class CRC16
    {
        public static byte[] CRC(byte[] data, bool Reverse = false)
        {
            int len = data.Length;
            if (len > 0)
            {
                ushort crc = 0xFFFF;
                for (int i = 0; i < len; i++)
                {
                    crc = (ushort)(crc ^ (data[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }
                byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
                byte lo = (byte)(crc & 0x00FF);         //低位置
                if (Reverse)
                    return new byte[] { lo, hi };
                else
                    return new byte[] { hi, lo };
            }
            return new byte[] { 0, 0 };
        }
    }

    /// <summary>
    /// CRC 效验
    /// 快速检测算法
    /// </summary>
    public class CRC32
    {
        static UInt32[] crcTable = {
                        0x0, 0x77073096, 0xee0e612c, 0x990951ba, 0x76dc419, 0x706af48f, 0xe963a535, 0x9e6495a3,
                        0xedb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x9b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,
                        0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
                        0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,
                        0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
                        0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
                        0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,
                        0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,
                        0x76dc4190, 0x1db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x6b6b51f, 0x9fbfe4a5, 0xe8b8d433,
                        0x7807c9a2, 0xf00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x86d3d2d, 0x91646c97, 0xe6635c01,
                        0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457,
                        0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
                        0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb,
                        0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,
                        0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
                        0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,
                        0xedb88320, 0x9abfb3b6, 0x3b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x4db2615, 0x73dc1683,
                        0xe3630b12, 0x94643b84, 0xd6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0xa00ae27, 0x7d079eb1,
                        0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7,
                        0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
                        0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
                        0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,
                        0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f,
                        0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
                        0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x26d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x5005713,
                        0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0xcb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0xbdbdf21,
                        0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
                        0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,
                        0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db,
                        0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
                        0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf,
                        0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d,
                    };

        public static int CRC(byte[] bytes)
        {
            int iCount = bytes.Length;
            UInt32 crc = 0xFFFFFFFF;
            for (int i = 0; i < iCount; i++)
            {
                crc = ((crc >> 8) & 0x00FFFFFF) ^ crcTable[(crc ^ bytes[i]) & 0xFF];
            }
            UInt32 temp = crc ^ 0xFFFFFFFF;
            int t = (int)temp;
            return (t);
        }
    }
}
