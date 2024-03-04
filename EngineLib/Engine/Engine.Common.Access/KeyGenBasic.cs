using System;
using System.IO;
using System.Reflection;
using Engine.Common;

namespace Engine.Access
{
    /// <summary>
    /// 注册码生成器(简易版)
    /// </summary>
    public class HeaoKeyGen
    {
        /// <summary>
        /// 指定注册文件
        /// </summary>
        private string TargetFile;
        /// <summary>
        /// 建立访问对象
        /// </summary>
        private static HeaoKeyGen _HeaoKeyGen;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strTargetFile">指定注册文件目录，可默认</param>
        public HeaoKeyGen(string strTargetFile = "")
        {
            if (!string.IsNullOrEmpty(strTargetFile) && strTargetFile.IsIllegalPath(".lic"))
                TargetFile = strTargetFile;
            else
            {
                string fileName = System.Windows.Forms.Application.ExecutablePath;
                Assembly AssemblyInfo = Assembly.LoadFrom(fileName);
                string strInsName = AssemblyInfo.GetName().Name.TrimEnd(".exe");
                TargetFile = string.Format("{0}{1}{2}.lic", sCommon.GetLocalAppPath(), @"\Heao\Licenses\", strInsName);
            }
        }

        /// <summary>
        /// 获取默认实例
        /// </summary>
        public static HeaoKeyGen Default
        {
            get
            {
                if (_HeaoKeyGen == null)
                    _HeaoKeyGen = new HeaoKeyGen();
                return _HeaoKeyGen;
            }
        }

        /// <summary>
        /// 获取机器编码
        /// </summary>
        /// <returns></returns>
        public string GetMachineCode()
        {
            //混合形成动态机器码
            string strDynamicMachineCode = "Elvis.Wu" + WmiService.Default.ComputerSysProductUUID;
            string strCrypMachineCode = Cryption.StringToMD5Hash(strDynamicMachineCode, 32);
            //string strCrypMachineCode = (new DESCryption()).Encode(strDynamicMachineCode);
            return strCrypMachineCode;
        }

        /// <summary>
        /// 生成注册码
        /// </summary>
        /// <param name="MachineCode">机器编码，默认本机</param>
        /// <returns></returns>
        public string GenerateKeyCode(string MachineCode = "")
        {
            string strMachineCode = string.IsNullOrEmpty(MachineCode) ? GetMachineCode() : MachineCode;
            string strDesMachineCode = (new DESCryption()).Encode(strMachineCode);
            return Cryption.StringToMD5Hash(strDesMachineCode, 24);
        }

        /// <summary>
        /// 验证本机注册状态
        /// </summary>
        /// <returns></returns>
        public bool VerifyRegStatus()
        {
            if (!File.Exists(TargetFile))
                return false;
            string strMachineCode = GetMachineCode();
            string strKeyCode = GenerateKeyCode();
            string strRegKey = File.ReadAllText(TargetFile);
            //byte[] bytes = Encoding.UTF8.GetBytes(strRegKey);
            //strRegKey = Encoding.UTF8.GetString(bytes);
            return strKeyCode == strRegKey;
        }

        /// <summary>
        /// 软件注册
        /// </summary>
        /// <returns></returns>
        public bool SoftRegister(string MachineCode, string KeyCode)
        {
            if (string.IsNullOrEmpty(MachineCode) || string.IsNullOrEmpty(KeyCode))
                return false;
            string strMachineCode = GetMachineCode();
            string strKeyCode = GenerateKeyCode();
            if (!(strMachineCode == MachineCode && strKeyCode == KeyCode))
                return false;
            return WriteRegMsg(strKeyCode);
        }

        /// <summary>
        /// 记录注册信息
        /// </summary>
        /// <returns></returns>
        private bool WriteRegMsg(string strRegMsg)
        {
            try
            {
                string strDirectory = sCommon.GetDirectoryName(TargetFile);
                if (!Directory.Exists(strDirectory))
                    Directory.CreateDirectory(strDirectory);
                if (File.Exists(TargetFile))
                    File.Delete(TargetFile);
                File.AppendAllText(TargetFile, strRegMsg);
            }
            catch (Exception )
            {
                return false;
            }
            return true;
        }
    }
}
