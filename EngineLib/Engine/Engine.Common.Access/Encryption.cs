using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Engine.Access
{
    /// <summary>
    /// DES可逆加密解密
    /// U2FsdGVkX1+PX0/VZiqtq/fOMdgdX7mTQh3RNACgods=
    /// </summary>
    public class DESCryption
    {
        private SymmetricAlgorithm mobjCryptoService;
        private string DesKey;
        private string DesIV;
        private static DESCryption _Default;

        public DESCryption()
        {
            mobjCryptoService = new RijndaelManaged();
            DesKey = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";
            DesIV = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
        }


        /// <summary>
        /// 默认实例
        /// </summary>
        public static DESCryption Default
        {
            get
            {
                if (_Default == null)
                    _Default = new DESCryption();
                return _Default;
            }
        }

        /// <summary> 
        /// 获得密钥 32位
        /// </summary> 
        /// <returns>密钥</returns> 
        private byte[] GetLegalKey()
        {
            string sTemp = DesKey;
            mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary> 
        /// 获得初始向量IV 16位
        /// </summary> 
        /// <returns>初始向量IV</returns> 
        private byte[] GetLegalIV()
        {
            string sTemp = DesIV;
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        /// <summary>
        /// 是否带加密标记
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public bool IfWithEncryptMark(string encryptString)
        {
            if (encryptString == null)
                return false;
            return Decode(encryptString).Contains(SystemDefault.EnCryptWord);
        }

        /// <summary>
        /// DES加密带标记字符串 
        /// </summary>
        /// <param name="encryptString">加密字符串</param>
        /// <returns></returns>
        public string EncodeWithMark(string encryptString)
        {
            string strWithKey = string.Format(string.Format("{0}{1}", SystemDefault.EnCryptWord, encryptString));
            return Encode(strWithKey);
        }

        /// <summary>
        /// DES解带标记字符串 
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public string DecodeWithMark(string decryptString)
        {
            string strMarkedPwd = Decode(decryptString);
            if (strMarkedPwd == SystemDefault.EnCryptWord)
                strMarkedPwd = SystemDefault.DbConPwd;
            else if (strMarkedPwd.Contains(SystemDefault.EnCryptWord))
                strMarkedPwd = strMarkedPwd.Replace(SystemDefault.EnCryptWord, "");
            return strMarkedPwd;
        }

        /// <summary>
        /// DES加密字符串 
        /// </summary>
        /// <param name="encryptString">加密字符串</param>
        /// <returns></returns>
        public string Encode(string encryptString)
        {
            byte[] byKey = GetLegalKey();
            byte[] byIV = GetLegalIV();
            //DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            //int i = cryptoProvider.KeySize;
            Rijndael aes = Rijndael.Create();
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, aes.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cst);
            sw.Write(encryptString);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// DES解字符串 
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public string Decode(string decryptString)
        {
            byte[] byKey = GetLegalKey();
            byte[] byIV = GetLegalIV();
            try
            {
                byte[] byEnc = Convert.FromBase64String(decryptString);
                //DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                Rijndael aes = Rijndael.Create();
                MemoryStream ms = new MemoryStream(byEnc);
                CryptoStream cst = new CryptoStream(ms, aes.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cst);
                return sr.ReadToEnd();
            }
            catch
            {
                return string.Empty ;
            }
        }
    }

    /// <summary>
    /// 加密通用库
    /// </summary>
    public class Cryption
    {
        /// <summary>
        /// MD5不可逆加密 
        /// 产生128位二进制，16字节，32位字符串
        /// </summary>
        /// <param name="inputString">给定字符串</param>
        /// <param name="bits">加密长度不超过32 </param>
        /// <returns></returns>
        public static string StringToMD5Hash(string inputString, int bits = 32)
        {
            string[] strArray = new string[3];
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            string strMd5Cryp = BitConverter.ToString(encryptedBytes).Replace("-", "");
            return strMd5Cryp.Length > bits ? strMd5Cryp.Substring(0, bits) : strMd5Cryp;
        }

        /// <summary>
        /// 32位唯一标识符(随机，每次不同)
        /// ex0: 5bf99df1-dc49-4023-a34a-7bd80a42d6bb
        /// ex1: ece4f4a60b764339b94a07c84e338a27
        /// ex2: {2280f8d7-fd18-4c72-a9ab-405de3fcfbc9}
        /// ex3: (25e6e09f-fb66-4cab-b4cd-bfb429566549)
        /// ex4: {0x8dbca70a,0xef47,0x4fda,{0xa6,0x65,0x53,0x22,0xab,0x18,0x5d,0x17}}
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string[] GetGuid()
        {
            string[] strGuid = new string[5];
            Guid objGuid = Guid.NewGuid();
            strGuid[0] = objGuid.ToString("D");  //Guid.NewGuid().ToString()相同
            strGuid[1] = objGuid.ToString("N");
            strGuid[2] = objGuid.ToString("B");
            strGuid[3] = objGuid.ToString("P");
            strGuid[4] = objGuid.ToString("X");
            return strGuid;
        }

        /// <summary>
        /// 获取随机种子
        /// </summary>
        /// <returns></returns>
        public static int GetNewSeed()
        {
            byte[] rndBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(rndBytes);
            return BitConverter.ToInt32(rndBytes, 0);
        }

        /// <summary>
        /// 产生随机字符串
        /// </summary>
        /// <param name="len"></param>
        /// <param name="NewSeed">0:代表随机种子 其他则为指定种子</param>
        /// <param name="strIV">初始向量字符库，默认为全字符</param>
        /// <returns></returns>
        public static string GetRandomString(int len, int NewSeed = 0,string strIV = "")
        {
            string src = "123456789abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ";
            if (!string.IsNullOrEmpty(strIV))
                src = strIV;
            string reValue = string.Empty;
            int Seed = NewSeed == 0 ? GetNewSeed() : NewSeed;
            Random rnd = new Random(Seed);
            while (reValue.Length < len)
            {
                string s1 = src[rnd.Next(0, src.Length)].ToString();
                if (reValue.IndexOf(s1) == -1)
                    reValue += s1;
            }
            return reValue;
        }

        /// <summary>
        /// 获取随机序列号
        /// </summary>
        /// <returns></returns>
        public static string GetRandSerialNumber(int PartNum = 6,int PartCharNum = 4)
        {
            string strSerialNumber = string.Empty;
            string[] strPart = new string[PartNum];
            for (int i = 0; i < PartCharNum; i++)
                strPart[i] = GetRandomString(PartCharNum).ToUpper();
            for (int i = 0; i < PartCharNum; i++)
            {
                if (strSerialNumber.Length > 0)
                    strSerialNumber += "-";
                strSerialNumber += strPart[i];
            }
            return strSerialNumber;
        }
    }
}

