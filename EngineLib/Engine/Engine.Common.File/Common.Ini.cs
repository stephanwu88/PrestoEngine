using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Engine.Common
{
    /// <summary>
    /// INI文件读写类
    /// </summary>
    public class IniFile
    {
        private string _FilePath;

        //声明API函数
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public IniFile(string filePath)
        {
            _FilePath = filePath;
        }

        /// <summary> 
        /// 读出INI文件字段值
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        public string ReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, _FilePath);
            string strVal = temp.ToMyString();
            //对 http:// 做特殊处理
            strVal = strVal.Replace("http://","http:/");
            strVal = strVal.MidString("", "//", EndStringSearchMode.FromTailAndToEndWhenUnMatch);
            strVal = strVal.Replace("http:/", "http://");
            return strVal;
            //if (ret.IndexOf("//") != -1)
            //    ret = ret.Remove(ret.IndexOf("//"));
            //return ret.Trim();
        }

        /// <summary>
        /// 写入INI文件字段值
        /// </summary>
        /// <param name="Section">项目名称(如 [TypeName]</param>
        /// <param name="Key">键</param>
        /// <param name="Value">键值</param>
        public void WriteData(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, _FilePath);
        }

        /// ﹤summary﹥  
        /// 验证文件是否存在  
        /// ﹤/summary﹥  
        /// ﹤returns﹥布尔值﹤/returns﹥  
        public bool ExistFile()
        {
            return File.Exists(_FilePath);
        }
    }
}
