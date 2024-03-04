using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace Engine.SharpPresser
{
    public class SharpPresser
    {
        /// <summary>
        /// 压缩文件/文件夹
        /// </summary>
        /// <param name="filePath">需要压缩的文件/文件夹路径</param>
        /// <param name="zipPath">压缩文件路径（zip后缀）</param>
        /// <param name="filterExtenList">需要过滤的文件后缀名</param>
        public static bool Compression(string filePath, string zipPath, string password = "", List<string> filterExtenList = null)
        {
            try
            {
                if (!Directory.Exists(filePath))
                    throw new ArgumentException(string.Format("压缩源:[{0}]指定错误", filePath));
                if (!File.Exists(zipPath))
                    throw new ArgumentException(string.Format("压缩目标:[{0}]指定错误", zipPath));

                using (var zip = File.Create(zipPath))
                {
                    var option = new WriterOptions(CompressionType.Deflate)
                    {
                        ArchiveEncoding = new SharpCompress.Common.ArchiveEncoding()
                        {
                            Default = Encoding.UTF8
                        }
                    };
                    using (var zipWriter = WriterFactory.Open(zip, ArchiveType.Zip, option))
                    {
                        if (Directory.Exists(filePath))
                        {
                            //添加文件夹
                            zipWriter.WriteAll(filePath, "*",
                              (path) => filterExtenList == null ? true : filterExtenList.Any(d => Path.GetExtension(path).IndexOf(d, StringComparison.OrdinalIgnoreCase) != -1), SearchOption.AllDirectories);
                        }
                        else if (File.Exists(filePath))
                        {
                            zipWriter.Write(Path.GetFileName(filePath), filePath);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipPath">压缩文件路径</param>
        /// <param name="dirPath">解压到文件夹路径</param>
        /// <param name="password">密码</param>
        public static bool DeCompression(string zipPath, string dirPath, string password = "")
        {
            try
            {
                if (!File.Exists(zipPath))
                    throw new ArgumentException("zipPath压缩文件不存在");

                Directory.CreateDirectory(dirPath);
                using (Stream stream = File.OpenRead(zipPath))
                {
                    var option = new ReaderOptions()
                    {
                        ArchiveEncoding = new SharpCompress.Common.ArchiveEncoding()
                        {
                            Default = Encoding.UTF8
                        }
                    };
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        option.Password = password;
                    }

                    var reader = ReaderFactory.Open(stream, option);
                    while (reader.MoveToNextEntry())
                    {
                        if (reader.Entry.IsDirectory)
                        {
                            Directory.CreateDirectory(Path.Combine(dirPath, reader.Entry.Key));
                        }
                        else
                        {
                            //创建父级目录，防止Entry文件,解压时由于目录不存在报异常
                            var file = Path.Combine(dirPath, reader.Entry.Key);
                            Directory.CreateDirectory(Path.GetDirectoryName(file));
                            reader.WriteEntryToFile(file);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
