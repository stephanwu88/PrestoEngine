using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.Mod
{
    /// <summary>
    /// TXT文件类
    /// </summary>
    public class TxtFile
    {
        private string _filePath;
        public TxtFile(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// 从文件头读至文件尾部
        /// </summary>
        /// <returns>文件内容字符串</returns>
        public string ReadTxt()
        {
            string str = null;
            try
            {
                if (File.Exists(_filePath))
                {
                    StreamReader sr = new StreamReader(_filePath, System.Text.Encoding.Default);
                    while (!sr.EndOfStream)
                    {
                        //str += sr.ReadLine();
                        str += (char)sr.Read();
                    }
                    sr.Close();
                }
                return str;
            }
            catch (Exception )
            {
                return null;
            }
        }

        /// <summary>
        /// 从文件尾部按开始时间读取日志
        /// </summary>
        /// <param name="Start">日志记录开始时间</param>
        /// <param name="Max">最大读取数，为0则不限定</param>
        /// <returns></returns>
        public List<string> ReadTxtFromLast(DateTime Start, int Max)
        {
            List<string> str = null;
            try
            {
                if (File.Exists(_filePath))
                {
                    StreamReader sr = new StreamReader(_filePath, System.Text.Encoding.Default);
                    string strLine = string.Empty;
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        string strTime = strLine.Substring(1, strLine.IndexOf(']') - 1);
                        DateTime DT_Time = Convert.ToDateTime(strTime);
                        if (DateTime.Compare(DT_Time, Start) >= 0)
                        {
                            str = (strLine + "\n" + sr.ReadToEnd().Replace("\r", "")).Split('\n').ToList();
                        }
                    }
                    sr.Close();
                }
                if (str.Count > Max && Max != 0)
                    str.RemoveRange(0, str.Count - Max - 1);
                return str;
            }
            catch (Exception )
            {
                return null;
            }
        }

        /// <summary>
        /// 写入文件,如文件存在则添加写入,文件不存在则创建新文件写入
        /// </summary>
        /// <param name="content"></param>
        public void WriteTxt(string content)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    FileStream fs1 = new FileStream(_filePath, FileMode.Create, FileAccess.Write);//创建写入文件   
                    StreamWriter strmsave = new StreamWriter(fs1, System.Text.Encoding.Default);
                    strmsave.Write(content);
                    strmsave.Close();
                    fs1.Close();
                }
                else
                {
                    StreamWriter strmsave = new StreamWriter(_filePath, true, System.Text.Encoding.Default);
                    strmsave.Write(content);
                    strmsave.Close();
                }
            }
            catch (Exception )
            {

            }
        }
    }

}
