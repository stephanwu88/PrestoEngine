using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using Engine.Files;
using Engine.Common;
using Engine.ComDriver;

namespace Engine.Access
{
    /// <summary>
    /// 注册申请
    /// </summary>
    [Serializable]
    public class ActivationRequest
    {
        /// <summary>
        /// 注册版本
        /// Standard        标准版
        /// Professional    专业版
        /// </summary>
        public string Edition { get; set; }
        /// <summary>
        /// 机器码
        /// </summary>
        public string MachineHash { get; set; }
        /// <summary>
        /// 注册序列号（随产品提供）
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 程序主版本号
        /// </summary>
        public string MajorVersion { get; set; }
        /// <summary>
        /// 程序次版本号
        /// </summary>
        public string MinorVerion { get; set; }
        /// <summary>
        /// 本次申请会话(类似腾讯会议号)（随机算法生成）
        /// </summary>
        public string Session { get; set; }
    }

    /// <summary>
    /// 注册结果
    /// </summary>
    [Serializable]
    public class ActivationResponse
    {
        /// <summary>
        /// 申请数据
        /// </summary>
        public ActivationRequest Data { get; set; }
        /// <summary>
        /// 验证签名
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// 许可状态
        /// </summary>
        //[NonSerialized]
        [XmlIgnore]
        public bool BeenLicensed = false;
        /// <summary>
        /// 该许可细节描述
        /// </summary>
        //[NonSerialized]
        [XmlIgnore]
        public string LicenseDesc = "";
    }

    /// <summary>
    /// 产品序列号参数
    /// </summary>
    public class SerialNumberArg
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 产品序列码
        /// </summary>
        public string ProductSerialNumber { get; set; }
        /// <summary>
        /// 产品注册版次
        /// </summary>
        public string Edition { get; set; }
    }

    /// <summary>
    /// 注册码生成器(商业版)
    /// </summary>
    public class KeyGenPro
    {
        /// <summary>
        /// 指定注册文件
        /// </summary>
        private string TargetFile;
        /// <summary>
        /// 建立访问对象
        /// </summary>
        private static KeyGenPro _HeaoKeyGen;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strTargetFile">指定注册文件目录，可默认</param>
        public KeyGenPro(string strTargetFile = "")
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
        public static KeyGenPro Default
        {
            get
            {
                if (_HeaoKeyGen == null)
                    _HeaoKeyGen = new KeyGenPro();
                return _HeaoKeyGen;
            }
        }

        /// <summary>
        /// 生成随机产品序列号
        /// </summary>
        /// <param name="Edition"></param>
        /// <returns></returns>
        public string GenerateSerialNumber(string Edition)
        {
            string strEdition = string.Empty;
            switch (Edition)
            {
                case "Standard":
                    strEdition = "STD";
                    break;
                case "Professional":
                    strEdition = "PRO";
                    break;
            }
            strEdition = strEdition.Substring(0, 3);
            string strRnd = Cryption.GetRandomString(14 - 3 - 4);
            string strSn = string.Format("{0}{1}", strEdition, strRnd);
            string strCRC = CRCASM.GenCheckCode(CheckType.CRC16_L_H, strSn).Replace(" ", "");
            strSn = strSn + strCRC;
            strSn = Base24Encoding.Default.ToBase24String(strSn);
            return strSn;
        }

        /// <summary>
        /// 获取激活申请文本
        /// </summary>
        /// <param name="strSerialNumber">指定产品序列号</param>
        /// <returns></returns>
        public string GetActivationRequestText(string strSerialNumber)
        {
            try
            {
                ActivationRequest ActReq = GetActivationRequest(strSerialNumber);
                return ActReq.XmlSerialize<ActivationRequest>();
            }
            catch (Exception )
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取激活申请
        /// </summary>
        /// <param name="strSerialNumber">正版序列号</param>
        /// <returns></returns>
        public ActivationRequest GetActivationRequest(string strSerialNumber)
        {
            try
            {
                SerialNumberArg SnArg = VerifySerialNumber(strSerialNumber);
                if (!VerifySerialNumber(strSerialNumber).IsValid)
                    new Exception("申请提交的序列号不符合要求");
                string strMachineUUID = WmiService.Default.ComputerSysProductUUID;
                string strMachineHash = Cryption.StringToMD5Hash(strMachineUUID, 16);
                strMachineHash = strMachineHash.ToMyString(4, "-");
                string fileName = System.Windows.Forms.Application.ExecutablePath;
                System.Diagnostics.FileVersionInfo VersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(fileName);
                ActivationRequest ActReq = new ActivationRequest()
                {
                    Edition = SnArg.Edition,
                    MachineHash = strMachineHash,
                    SerialNumber = strSerialNumber,
                    MajorVersion = VersionInfo.FileMajorPart.ToMyString(),
                    MinorVerion = VersionInfo.FileMinorPart.ToMyString(),
                    Session = Cryption.GetGuid()[0]
                };
                return ActReq;
            }
            catch (Exception err)
            {
                throw new Exception("获取申请码失败:" + err.ToString());
            }
        }

        /// <summary>
        /// 验证申请序列号
        /// xxxxx-xxxxx-xxxxx-xxxxx-xxxxx
        /// </summary>
        /// <returns></returns>
        public SerialNumberArg VerifySerialNumber(string strSerialNumber)
        {
            SerialNumberArg RegResult = new SerialNumberArg()
            {
                IsValid = false,
                ProductSerialNumber = strSerialNumber,
                Edition = ""
            };
            string[] strPart = strSerialNumber.Split('-');
            string strSn = Base24Encoding.Default.FromBase24String(strSerialNumber).Trim();
            if (strPart.Length != 5 || strSn.Length != 14)
                return RegResult;
            foreach (var str in strPart)
            {
                if (str.Length != 5)
                    return RegResult;
            }
            if (strSn.StartsWith("STD"))
                RegResult.Edition = "STD";
            else if (strSn.StartsWith("PRO"))
                RegResult.Edition = "PRO";
            string strCRC = CRCASM.GenCheckCode(CheckType.CRC16_L_H, strSn.Substring(0, 10)).Replace(" ", "");
            RegResult.IsValid = strCRC == strSn.Substring(10, 4);
            return RegResult;
        }

        /// <summary>
        /// 生成授权许可
        /// </summary>
        /// <param name="XmlActivationReq"></param>
        /// <param name="strEdition"></param>
        /// <returns></returns>
        public string GenerateResponseCode(string XmlActivationReq)
        {
            try
            {
                ActivationRequest ActReq = sCommon.XmlDeserialize<ActivationRequest>(XmlActivationReq);
                ActivationResponse ActResponse = new ActivationResponse() { Data = ActReq };
                if (ActResponse == null)
                    return string.Empty;
                //数据签名
                ActResponse.Signature = ActResponse.Data.SerializeBase64<ActivationRequest>();
                return ActResponse.XmlSerialize<ActivationResponse>();
            }
            catch (Exception err)
            {
                throw new Exception(string.Format("生成授权许可失败!\r\n{0}\r\n", err.ToString()));
            }
        }

        /// <summary>
        /// 验证本机注册状态(软件版本区分)
        /// </summary>
        /// <returns></returns>
        public bool VerifyRegStatusWithVersion()
        {
            bool IsMatchMajorVer = false;
            bool IsMatchMinorVer = false;
            ActivationResponse ActRes = GetActivationResponseFile();
            if (!ActRes.BeenLicensed)
                return false;
            else
            {
                IsMatchMajorVer = MyAssembly.Default.ProductMajorPart.ToString() == ActRes.Data.MajorVersion;
                IsMatchMinorVer = MyAssembly.Default.ProductMinorPart.ToString() == ActRes.Data.MinorVerion;
            }
            return IsMatchMajorVer && IsMatchMinorVer;
        }

        /// <summary>
        /// 验证本机注册状态
        /// </summary>
        /// <returns></returns>
        public bool VerifyRegStatus()
        {
            ActivationResponse ActRes = GetActivationResponseFile();
            return ActRes.BeenLicensed;
        }

        /// <summary>
        /// 从本地许可文件中验证获取应答许可实体
        /// </summary>
        /// <returns></returns>
        public ActivationResponse GetActivationResponseFile()
        {
            if (!File.Exists(TargetFile))
                return new ActivationResponse() { LicenseDesc = "未找到授权！", BeenLicensed = false };
            string strActResponseText = File.ReadAllText(TargetFile);
            return VerifyResponseText(strActResponseText);
        }

        /// <summary>
        /// 验证许可应答
        /// </summary>
        /// <param name="strActResponseText"></param>
        /// <returns></returns>
        private ActivationResponse VerifyResponseText(string strActResponseText)
        {
            try
            {
                ActivationResponse ActResponse = sCommon.XmlDeserialize<ActivationResponse>(strActResponseText);
                SerialNumberArg SnArg = VerifySerialNumber(ActResponse.Data.SerialNumber);
                string strMachineUUID = WmiService.Default.ComputerSysProductUUID;
                string strMachineHash = Cryption.StringToMD5Hash(strMachineUUID, 16);
                strMachineHash = strMachineHash.ToMyString(4, "-");
                ActivationRequest ActReq = sCommon.DeSerializeBase64<ActivationRequest>(ActResponse.Signature);
                if (!SnArg.IsValid)
                    ActResponse.LicenseDesc = "非法序列号";
                else if (SnArg.Edition != ActResponse.Data.Edition)
                    ActResponse.LicenseDesc = "【Edition】与产品不一致";
                else if (ActResponse.Data.MachineHash != strMachineHash)
                    ActResponse.LicenseDesc = "计算机信息验证不正确";
                else if (!ActResponse.Data.ObjectEquals<ActivationRequest>(ActReq))
                    ActResponse.LicenseDesc = "软件许可签名不正确";
                else
                    ActResponse.LicenseDesc = "BeenLicensed";
                ActResponse.BeenLicensed = ActResponse.LicenseDesc == "BeenLicensed";
                return ActResponse;
            }
            catch (Exception err)
            {
                string strErr = string.Format("授权文件错误！\r\n{0}", err.ToString());
                return new ActivationResponse() { LicenseDesc = strErr, BeenLicensed = false };
            }
        }

        /// <summary>
        /// 软件注册
        /// </summary>
        /// <returns></returns>
        public bool SoftRegister(string XmlActivationReq, string XmlActivationResponse)
        {
            try
            {
                ActivationResponse ActRes = VerifyResponseText(XmlActivationResponse);
                if (!ActRes.BeenLicensed)
                    return false;
                ActivationRequest ActReq = sCommon.XmlDeserialize<ActivationRequest>(XmlActivationReq);
                if (!ActReq.ObjectEquals<ActivationRequest>(ActRes.Data))
                    return false;
                return WriteRegMsg(XmlActivationResponse);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 记录注册信息
        /// </summary>
        /// <returns></returns>
        private bool WriteRegMsg(string strResponseText)
        {
            try
            {
                string strDirectory = sCommon.GetDirectoryName(TargetFile);
                if (!Directory.Exists(strDirectory))
                    Directory.CreateDirectory(strDirectory);
                if (File.Exists(TargetFile))
                    File.Delete(TargetFile);
                File.AppendAllText(TargetFile, strResponseText);
            }
            catch (Exception )
            {
                return false;
            }
            return true;
        }
    }
}
