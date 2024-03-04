using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Engine.Files
{
    /// <summary>
    /// 文本文件读取
    /// </summary>
    public partial class FileReader
    {
        /// <summary>
        /// 获取文本文件内容
        /// </summary>
        /// <param name="TargetFile">目标文件路径</param>
        /// <returns>字符串列表</returns>
        public static List<string> GetLines(string TargetFile)
        {
            try
            {
                List<string> strLines = new List<string>();
                StreamReader sr = new StreamReader(TargetFile, Encoding.GetEncoding("gb2312"));
                while (!sr.EndOfStream)
                {
                    string strLine = sr.ReadLine();
                    strLines.Add(strLine);
                }
                sr.Close();
                return strLines;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取文本文件内容
        /// </summary>
        /// <param name="TargetFile">目标文件路径</param>
        /// <returns>字符串</returns>
        public static string GetAllText(string TargetFile)
        {
            try
            {
                string strText = string.Empty;
                strText = File.ReadAllText(TargetFile, Encoding.GetEncoding("gb2312"));
                return strText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 二进制文件读取
    /// </summary>
    public partial class FileReader
    {
        /// <summary>
        /// 读取文件并反序列化出结果
        /// </summary>
        /// <returns></returns>
        public static List<object> ReadBinary(string TargetFile)  //读取二进制文件，反序列化到列表
        {
            List<object> lst = new List<object>();//初始化列表对象
            if (!File.Exists(TargetFile))
                throw new ArgumentException("文件不存在");

            try
            {
                FileStream fs = new FileStream(TargetFile, FileMode.OpenOrCreate);//打开或创建文件流
                BinaryFormatter bf = new BinaryFormatter();
                lst = bf.Deserialize(fs) as List<object>;//反序列化文件流 到列表
                fs.Close();//关闭文件流
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 文本文件写入
    /// </summary>
    public partial class FileWriter
    {
        /// <summary>
        /// 内容写入文件
        /// </summary>
        /// <returns></returns>
        public static bool Write(string strObjectFile,string content)
        {
            try
            {
                string strDirect = System.IO.Path.GetDirectoryName(strObjectFile);
                if (!Directory.Exists(strDirect))
                    Directory.CreateDirectory(strDirect);
                if (!File.Exists(strObjectFile))
                {
                    FileStream fs1 = new FileStream(strObjectFile, FileMode.Create, FileAccess.Write);//创建写入文件   
                    StreamWriter strmsave = new StreamWriter(fs1, Encoding.GetEncoding("gb2312"));
                    strmsave.Write(content);
                    strmsave.Close();
                    fs1.Close();
                }
                else
                {
                    StreamWriter strmsave = new StreamWriter(strObjectFile, true, Encoding.GetEncoding("gb2312"));
                    strmsave.Write(content);
                    strmsave.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 内容写入文件
        /// </summary>
        /// <param name="LstText"></param>
        /// <returns></returns>
        public static bool Write(string strObjectFile,List<string> LstText)
        {
            try
            {
                if (LstText == null)
                    throw new ArgumentException("未指定写入文件的内容");
                string strWrite = string.Empty;
                foreach (string str in LstText)
                {
                    strWrite += str;
                    if (!str.Contains("\r\n"))
                    {
                        strWrite += "\r\n";
                    }
                }
                bool ret = Write(strObjectFile, strWrite);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 二进制文件写入
    /// </summary>
    public partial class FileWriter
    {
        public static void SaveBinary(List<object> lst,string TargetFile)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(TargetFile)))//目录不存在，则创建目录
                    Directory.CreateDirectory(Path.GetDirectoryName(TargetFile));
                FileStream fs = new FileStream(TargetFile, FileMode.OpenOrCreate);//打开或创建文件流
                BinaryFormatter bf = new BinaryFormatter(); //初始化二进制格式化器
                bf.Serialize(fs, lst);//将列表数据 序列化 写入文件流                
                fs.Close();//关闭文件流
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
