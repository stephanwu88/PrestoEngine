using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Engine.Common
{
    /// <summary>
    /// 压缩工具类型
    /// </summary>
    public enum ZipperType
    {
        [Description("Engine.DotNetZipper | Engine.DotNetZipper.DotNetZipper")]
        DotNetZip,
        [Description("Engine.SharpZipLib | Engine.SharpZipLib.SharpZipLib")]
        SharpZipLib,
        [Description("Engine.SevenSharpZip | Engine.SevenSharpZip.SevenSharpZip")]
        SevenSharpZip,
        [Description("Engine.SharpPresser | Engine.SharpPresser.SharpPresser")]
        SharpPress,
        [Description("Engine | Engine.Common.WinRarZip")]
        WinRAR,
    }

    /// <summary>
    /// 压缩/解压缩
    /// </summary>
    public interface IFileCompress
    {
        /// <summary>
        /// 压缩文件/文件夹
        /// </summary>
        /// <param name="filePath">需要压缩的文件/文件夹路径</param>
        /// <param name="zipFile">压缩文件</param>
        /// <param name="password">密码</param>
        /// <param name="filterExtenList">需要过滤的文件后缀名</param>
        bool Compression(string filePath, string zipFile, string password = "", List<string> filterExtenList = null);
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipFile">压缩文件</param>
        /// <param name="filePath">解压至目标路径</param>
        /// <returns></returns>
        bool DeCompression(string zipFile, string filePath, string password = "");
    }

    /// <summary>
    /// 文件解压缩
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 创建压缩文件对象
        /// </summary>
        /// <param name="zipType"></param>
        /// <returns></returns>
        public static IFileCompress CreateFileCompress(ZipperType zipType = ZipperType.DotNetZip)
        {
            try
            {
                List<string> LstAssem = zipType.FetchDescription().MySplit("|");
                object obj = CreateInstance(LstAssem[0], LstAssem[1]);
                IFileCompress iFileCompress = obj as IFileCompress;
                return iFileCompress;
            }
            catch (Exception )
            {
                return null;
            }
        }
    }
}
