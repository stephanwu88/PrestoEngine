using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace Engine.Common
{
    /// <summary>
    /// 文件及文件夹操作、权限操作
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 给指定文件添加"Everyone,Users,Authenticated Users"用户组的完全控制权限
        /// </summary>
        /// <param name="fileName"></param>
        public static void SetFileAccess(this string fileName)
        {
            if (!File.Exists(fileName))
                return;
            FileInfo fi = new FileInfo(fileName);
            System.Security.AccessControl.FileSecurity fileSecurity = fi.GetAccessControl();
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Authenticated Users", FileSystemRights.FullControl, AccessControlType.Allow));
            fi.SetAccessControl(fileSecurity);
        }
        /// <summary>
        /// 给指定文件所在目录添加"Everyone,Users,Authenticated Users"用户组的完全控制权限
        /// </summary>
        /// <param name="fileName"></param>
        public static void SetFolderAccessByFile(this string fileName)
        {
            if (!File.Exists(fileName))
                return;
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(fileName));
            System.Security.AccessControl.DirectorySecurity dirSecurity = di.GetAccessControl();
            dirSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            dirSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            dirSecurity.AddAccessRule(new FileSystemAccessRule("Authenticated Users", FileSystemRights.FullControl, AccessControlType.Allow));
            di.SetAccessControl(dirSecurity);
        }
        /// <summary>
        /// 给指定文件夹添加"Everyone,Users,Authenticated Users"用户组的完全控制权限
        /// </summary>
        /// <param name="FolderPath"></param>
        public static void SetFolderAccess(this string FolderPath)
        {
            if (!Directory.Exists(FolderPath))
                return;
            DirectoryInfo di = new DirectoryInfo(FolderPath);
            System.Security.AccessControl.DirectorySecurity dirSecurity = di.GetAccessControl();
            dirSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            dirSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            dirSecurity.AddAccessRule(new FileSystemAccessRule("Authenticated Users", FileSystemRights.FullControl, AccessControlType.Allow));
            di.SetAccessControl(dirSecurity);
        }
        /// <summary>
        /// 文件复制
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destFile"></param>
        /// <returns></returns>
        public static bool FileCopy(string sourceFile, string destFile)
        {
            if (!File.Exists(sourceFile) || sourceFile == destFile)
                return false;
            string strDestFile = destFile;
            if (string.IsNullOrEmpty(strDestFile))
            {
                string strDirName = sCommon.GetDirectoryName(sourceFile);
                string strFileName = sCommon.GetFileName(sourceFile).MidString("", ".") + "_Copy";
                string strExtention = sCommon.GetExtension(sourceFile);
                strDestFile = string.Format("{0}\\{1}{2}", strDirName, strFileName, strExtention);
            }
            if (File.Exists(strDestFile))
                File.Delete(strDestFile);
            File.Copy(sourceFile, strDestFile);
            return true;
        }
        /// <summary>
        /// 文件复制
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destFile"></param>
        /// <returns></returns>
        public static bool FileCopy(string sourceFile, string destFile, out string dFile)
        {
            if (!File.Exists(sourceFile) || sourceFile == destFile)
            {
                dFile = string.Empty;
                return false;
            }
            string strDestFile = destFile;
            if (string.IsNullOrEmpty(strDestFile))
            {
                string strDirName = sCommon.GetDirectoryName(sourceFile);
                string strFileName = sCommon.GetFileName(sourceFile).MidString("", ".") + "_Copy";
                string strExtention = sCommon.GetExtension(sourceFile);
                strDestFile = string.Format("{0}\\{1}{2}", strDirName, strFileName, strExtention);
            }
            if (File.Exists(strDestFile))
                File.Delete(strDestFile);
            File.Copy(sourceFile, strDestFile);
            dFile = strDestFile;
            return true;
        }
        /// <summary>
        /// 文件移动
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destFile"></param>
        /// <param name="AutoCreateDirect"></param>
        /// <param name="KillSource"></param>
        /// <returns></returns>
        public static bool FileMove(string sourceFile, string destFile, bool AutoCreateDirect = true, bool KillSource = true)
        {
            //检查目标文件夹，根据需要创建
            string strDestDirect = destFile.GetDirectoryName();
            if (AutoCreateDirect && !Directory.Exists(strDestDirect))
                Directory.CreateDirectory(strDestDirect);
            //目标文件夹内没有目标文件则移动
            if (!File.Exists(destFile))
                File.Move(sourceFile, destFile);
            //是否要删除源文件
            if (KillSource && File.Exists(sourceFile))
                File.Delete(sourceFile);
            return true;
        }
        /// <summary>
        /// 流转为文本列表
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static List<string> StreamToList(StreamReader sr)
        {
            List<string> strLines = new List<string>();
            if (sr != null)
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    strLines.Add(line);
                }
                sr.Close();
            }
            return strLines;
        }

        /// <summary>
        /// 创建并写入文件
        /// 如果文件不在自动创建
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="LstContent"></param>
        /// <param name="encoding"></param>
        /// <param name="AppandMode">附加到文本还是覆盖文本</param>
        public static void WriteFile(string fileName, List<string> LstContent, Encoding encoding, bool AppandMode = false)
        {
            if (encoding == null)
                encoding = Encoding.Default;
            using (StreamWriter file = new StreamWriter(fileName, AppandMode, encoding))
            {
                foreach (string line in LstContent)
                {
                    file.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// 创建并写入文件
        /// 如果文件不在自动创建
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="LstContent"></param>
        /// <param name="encoding"></param>
        public static void WriteFile(string fileName, List<string> LstContent, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;
            File.WriteAllLines(fileName, LstContent.ToArray(), encoding);
        }

        /// <summary>
        /// 创建并写入文件
        /// 如果文件不在自动创建
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="LstContent"></param>
        /// <param name="encoding"></param>
        public static void WriteFile(string fileName, string Content, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;
            File.WriteAllText(fileName, Content, encoding);
        }

        /// <summary>
        /// 按行读取文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static List<string> ReadFile(string fileName, bool removeEmptyLine = false,
            Encoding encoding = null)
        {
            List<string> LstLines = new List<string>();
            try
            {
                if (!File.Exists(fileName))
                    throw new Exception("文件不存在");
                if (encoding == null)
                    encoding = Encoding.Default;
                StreamReader sr = new StreamReader(fileName, encoding);
                string strLine = string.Empty;
                while ((strLine = sr.ReadLine()) != null)
                {
                    if (removeEmptyLine && string.IsNullOrEmpty(strLine.Trim()))
                        continue;
                    //string[] strArr = strLine.Split('\t');
                    //foreach (string s in strArr)
                    //{
                    //    if (s.Length > 0)
                    //        LstLines.Add(s);
                    //}
                    LstLines.Add(strLine);
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return LstLines;
        }
        /// <summary>
        /// 获取指定路径下的文件对象
        /// </summary>
        /// <param name="PathUrl">文件路径</param>
        /// <param name="SearchPattern">
        /// 要与文件名匹配的搜索字符串。 此参数可以包含有效文本路径和通配符（* 和 ?）的组合，但不支持正则表达式
        /// </param>
        /// <returns>文件对象数组</returns>
        public static FileInfo[] GetFileInfoArray(string PathUrl, string SearchPattern = "*.csv")
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(PathUrl);
                FileInfo[] fiArray = directoryInfo.GetFiles(SearchPattern, SearchOption.TopDirectoryOnly);
                return fiArray;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取指定文件的文件信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileInfo GetFileInfo(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            return fi;
        }
    }


}
