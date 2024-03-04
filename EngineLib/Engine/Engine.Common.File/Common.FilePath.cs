using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Engine.Common
{
    /// <summary>
    /// 路径处理
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 获取启动目录
        /// </summary>
        /// <param name="WithNextInlet">路径尾部是否包含下一级文件入口"\"</param>
        /// <returns></returns>
        public static string GetStartUpPath(bool WithNextInlet = false)
        {
            string strStartUpPath = string.Empty;
            if (WithNextInlet)
            {
                //最后包含“\”
                strStartUpPath = AppDomain.CurrentDomain.BaseDirectory;
                //strStartUpPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
            else
            {
                //最后不包含“\”
                strStartUpPath = Environment.CurrentDirectory;
                //strStartUpPath = System.IO.Directory.GetCurrentDirectory();
                //strStartUpPath = System.Windows.Forms.Application.StartupPath;
            }
            return strStartUpPath;
        }
        /// <summary>
        /// 获取启动文件全名称（包含目录）
        /// </summary>
        /// <returns></returns>
        public static string GetStartUpFullName()
        {
            string strStartUpFullName = string.Empty;
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            strStartUpFullName = proc.MainModule.FileName;
            //strStartUpFullName =  System.Windows.Forms.Application.ExecutablePath;
            return strStartUpFullName;
        }
        /// <summary>
        /// 获取当前启动进程的短名称
        /// </summary>
        /// <returns></returns>
        public static string GetShortFileName(bool WithExtension = false)
        {
            return GetShortFileName(GetStartUpFullName(), WithExtension);
        }
        /// <summary>
        /// 获取文件短名称
        /// </summary>
        /// <param name="strFullName">@"C:\mydir\myfile.ext"</param>
        /// <param name="WithExtension">是否返回文件后缀名</param>
        /// <returns></returns>
        public static string GetShortFileName(this string strFullName, bool WithExtension = false)
        {
            try
            {
                string strFileName = string.Empty;
                if (WithExtension)
                    strFileName = System.IO.Path.GetFileName(strFullName);
                else
                    strFileName = System.IO.Path.GetFileNameWithoutExtension(strFullName);
                return strFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取路径的相关信息
        /// </summary>
        /// <param name="path">@"C:\mydir\myfile.ext";</param>
        /// <param name="infoContent">""</param>
        /// <returns></returns>
        public static string GetPathDirectoryInfo(string path, string infoContent)
        {
            string strRet = string.Empty;
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(path);
            switch (infoContent)
            {
                case "Parent":
                    strRet = info.Parent.FullName;
                    break;
                case "Root":
                    strRet = info.Root.FullName;
                    //strRet = System.IO.Path.GetPathRoot(path);
                    break;
                case "Extension":
                    strRet = info.Extension;
                    //strRet = System.IO.Path.GetExtension(path);
                    break;
            }
            return strRet;
        }
        /// <summary>
        /// //获取登录用户的AppData\Local文件夹
        /// </summary>
        /// <returns></returns>
        public static string GetLocalAppPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
        }
        /// <summary>
        /// 格式化路径字符串
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public static string FormatPath(this string strPath)
        {
            //\加bfrnt\/‘"为合法分隔符，其它不是，替换
            //string pattern = @"(\\[^bfrnt\\/‘\""])";
            //strPath = Regex.Replace(strPath, pattern, "\\$1");
            //string str = JToken.Parse(strPath);
            //strPath = strPath.Replace("$", Directory.GetCurrentDirectory());

            //return strPath;

            strPath = strPath.Replace("\\", "/");
            strPath = strPath.Replace("$", Directory.GetCurrentDirectory());
            strPath = strPath.Replace("..", Directory.GetCurrentDirectory());
            strPath = strPath.Replace("\\", "/");
            return strPath;
        }
        /// <summary>
        /// 校验文件路径，并检验是否为指定类型文件
        /// </summary>
        /// <param name="mPath">文件路径</param>
        /// <param name="mExtension">文件类型</param>
        /// <returns>true：非法文件(路径)  false：合法文件(路径)</returns>
        public static bool IsIllegalPath(this string mPath, string mExtension)
        {
            mPath = mPath.Trim();
            if (string.IsNullOrWhiteSpace(mPath)) return true;
            var reg = new Regex(@"^(?<fpath>([a-zA-Z]:\\)([\s\.\-\w]+\\)*)(?<fname>[\w]+.[\w]+)");
            var match = reg.Match(mPath);
            return !match.Success || !System.IO.Path.GetExtension(mPath).Equals(mExtension);
        }
        /// <summary>
        /// 获取文件名
        /// D:\dir\asp.net\readme.txt -  readme.txt
        /// D:\dir\asp.net\readme.    -  readme.
        /// D:\dir\asp.net\readme     -  readme
        /// D:\dir\asp.net\readme\    -  空
        /// D:\                       -  空
        /// D:                        -  空
        /// 路径中的 \ 和 / 是一样的结果。只要不是以 \ 或 / 结束，都是当作文件对待（盘符除外）
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static string GetFileName(this string strFilePath)
        {
            return Path.GetFileName(strFilePath);
        }
        /// <summary>
        /// 获取文件后缀名
        /// 统一转化小写 .zip .txt
        /// D:\dir\asp.net\readme.txt  -  .txt
        /// D:\dir\asp.net\readme.     -  空
        /// D:\dir\asp.net\readme      -  空
        /// D:\dir\asp.net\readme\     -  空
        /// D:\                        -  空
        /// D:                         -  空
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="ToLower">统计小写</param>
        /// <returns>.zip .txt</returns>
        public static string GetExtension(this string filePath, bool ToLower = true)
        {
            string strExtension = Path.GetExtension(filePath).ToMyString();
            return strExtension.ToLower();
        }
        /// <summary>
        /// 获取文件所在文件夹路径
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public static string GetDirectoryName(this string strFilePath)
        {
            //string s1 = Path.GetDirectoryName(“D:\dir\asp.net / readme.txt”); // D:\dir\asp.net
            //string s2 = Path.GetDirectoryName(“D:\dir\asp.net / readme.”); // D:\dir\asp.net
            //string s3 = Path.GetDirectoryName(“D:\dir\asp.net / readme”); // D:\dir\asp.net
            //string s4 = Path.GetDirectoryName(“D:\dir\asp.net / readme /”); // D:\dir\asp.net\readme
            //string s5 = Path.GetDirectoryName(“D:\”); // null，注意是 null
            //string s6 = Path.GetDirectoryName(“D:”); // null，注意是 null
            //这里，我们故意在路径中使用“/”，可以发现最终还是会转换成“\”
            return Path.GetDirectoryName(strFilePath).ToMyString();
        }

        ///// <summary>
        ///// 相对路径转换为绝对路径
        ///// </summary>
        ///// <param name="RelativePath"></param>
        ///// <returns></returns>
        //public static string RelativePathToAbs(this string RelativePath)
        //{
        //    string strAbsPath = RelativePath.Replace("\\", "/");
        //    strAbsPath = strAbsPath.Replace("$", Directory.GetCurrentDirectory());
        //    strAbsPath = strAbsPath.Replace("..", Directory.GetCurrentDirectory());
        //    return strAbsPath;
        //}

        /// <summary>
        /// 获取上级目录
        /// </summary>
        /// <param name="SourcePath">源目录</param>
        /// <param name="UpLevels">上级目录层数</param>
        /// <returns>不包含 \ 的上级目录</returns>
        public static string GetUpLevelPath(this string SourcePath, int UpLevels = 1)
        {
            string strRootPath = string.Empty;
            for (int i = 0; i < UpLevels; i++)
                SourcePath = SourcePath.Substring(0, SourcePath.LastIndexOf("\\"));
            if (Directory.Exists(SourcePath))
                strRootPath = SourcePath;
            return strRootPath;
        }
    }
}
