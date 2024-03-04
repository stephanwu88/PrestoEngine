using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace Engine.Common
{
    public sealed class DotNetZipper 
    {
        /// <summary>
        /// 压缩文件/文件夹
        /// </summary>
        /// <param name="filePath">需要压缩的文件/文件夹路径</param>
        /// <param name="zipFile">压缩文件</param>
        /// <param name="password">密码</param>
        /// <param name="filterExtenList">需要过滤的文件后缀名</param>
        public static bool Compression(string filePath, string zipFile, string password = "", List<string> filterExtenList = null)
        {
            try
            {
                if (!Directory.Exists(filePath))
                    throw new ArgumentException(string.Format("压缩源:[{0}]指定错误", filePath));
                if (!File.Exists(zipFile))
                    throw new ArgumentException(string.Format("压缩目标:[{0}]指定错误", zipFile));
                using (ZipFile zip = new ZipFile(Encoding.UTF8))
                {
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        zip.Password = password;
                    }
                    if (Directory.Exists(filePath))
                    {
                        if (filterExtenList == null)
                            zip.AddDirectory(filePath);
                        else
                            AddDirectory(zip, filePath, filePath, filterExtenList);
                    }
                    else if (File.Exists(filePath))
                    {
                        zip.AddFile(filePath, "");
                    }
                    zip.Save(zipFile);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipFile">Zip文件路径</param>
        /// <param name="filePath">解压至目标路径</param>
        /// <returns></returns>
        public static bool DeCompression(string zipFile, string filePath, string password = "")
        {
            try
            {
                if (!Directory.Exists(filePath))
                    throw new ArgumentException(string.Format("解压目标路径:[{0}]指定错误", filePath));
                if (!File.Exists(zipFile))
                    throw new ArgumentException(string.Format("待解压目标:[{0}]指定错误", zipFile));
                using (ZipFile zip = ZipFile.Read(zipFile))
                {
                    zip.Password = password;
                    //zip.ExtractProgress += ExtractProgress;
                    foreach (ZipEntry entry in zip)
                    {
                        entry.Extract(filePath);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool justHadByteUpdate = false;
        public static void ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
            {
                if (justHadByteUpdate)
                    Console.SetCursorPosition(0, Console.CursorTop);

                Console.Write(" {0}/{1} ({2:N0}%)", e.BytesTransferred, e.TotalBytesToTransfer,
                   e.BytesTransferred / (0.01 * e.TotalBytesToTransfer));
                justHadByteUpdate = true;
            }
            else if (e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
            {
                if (justHadByteUpdate)
                    Console.WriteLine();
                Console.WriteLine("Extracting: {0}", e.CurrentEntry.FileName);
                justHadByteUpdate = false;
            }
        }

        /// <summary>
        /// 添加文件夹
        /// </summary>
        /// <param name="zip">ZipFile对象</param>
        /// <param name="dirPath">需要压缩的文件夹路径</param>
        /// <param name="rootPath">根目录路径</param>
        /// <param name="filterExtenList">需要过滤的文件后缀名</param>
        private static void AddDirectory(ZipFile zip, string dirPath, string rootPath, List<string> filterExtenList)
        {
            var files = Directory.GetFiles(dirPath);
            for (int i = 0; i < files.Length; i++)
            {
                //如果Contains不支持第二个参数，就用.ToLower()
                //if (filterExtenList == null || (filterExtenList != null && !filterExtenList.Any(d => Path.GetExtension(files[i]).Contains(d, StringComparison.OrdinalIgnoreCase))))
                if (filterExtenList == null || (filterExtenList != null && filterExtenList.Any(d => Path.GetExtension(files[i]).IndexOf(d, StringComparison.OrdinalIgnoreCase) != -1)))
                {
                    //获取相对路径作为zip文件中目录路径
                    //zip.AddFile(files[i], Path.GetRelativePath(rootPath, dirPath));

                    //如果没有Path.GetRelativePath方法，可以用下面代码替换
                    string relativePath = Path.GetFullPath(dirPath).Replace(Path.GetFullPath(rootPath), "");
                    zip.AddFile(files[i], relativePath);
                }
            }
            var dirs = Directory.GetDirectories(dirPath);
            for (int i = 0; i < dirs.Length; i++)
            {
                AddDirectory(zip, dirs[i], rootPath, filterExtenList);
            }
        }
    }
}
