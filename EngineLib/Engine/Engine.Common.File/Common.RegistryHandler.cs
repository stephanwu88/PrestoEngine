using Microsoft.Win32;

namespace Engine.Common
{
    /// <summary>
    /// 注册表处理器
    /// </summary>
    public class RegistryHandler
    {
        private RegistryKey _Root = Registry.LocalMachine;

        public RegistryKey Root { get => _Root; set { _Root = value; } }

        public RegistryHandler(RegistryKey root)
        {
            _Root = root;
        }

        /// <summary>
        /// 获取注册表值
        /// </summary>
        /// <param name="RegeditPath">注册表路径</param>
        /// <param name="ItemName">项名称</param>
        /// <returns></returns>
        public string GetValue(string RegeditPath, string ItemName = "")
        {
            string strItemVal = default(string);
            //RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
            RegistryKey key = _Root.OpenSubKey(RegeditPath);
            strItemVal = key.GetValue(ItemName).ToMyString();
            return strItemVal;
        }

        /// <summary>
        /// 写入注册表值
        /// </summary>
        /// <param name="RegeditPath">注册表路径</param>
        /// <param name="ItemName">项名称</param>
        /// <param name="ItemVal">项值</param>
        public void SetValue(string RegeditPath, string ItemName,string ItemVal)
        {
            RegistryKey key = _Root.OpenSubKey(RegeditPath, true);
            Registry.SetValue("HKEY_LOCAL_MACHINE//SOFTWARE//Fjptlzx//AVRdisplay", "UserTimes", 1, RegistryValueKind.DWord);
            key.SetValue(ItemName,ItemVal);
        }

        public void Test()
        {
            RegistryKey runs = _Root.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (runs == null)
            {
                RegistryKey key2 = _Root.CreateSubKey("SOFTWARE");
                RegistryKey key3 = key2.CreateSubKey("Microsoft");
                RegistryKey key4 = key3.CreateSubKey("Windows");
                RegistryKey key5 = key4.CreateSubKey("CurrentVersion");
                RegistryKey key6 = key5.CreateSubKey("Run");
                runs = key6;
            }
            string[] runsName = runs.GetValueNames();
        }

        /// <summary>
        /// 创建注册路径
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        private bool CreateSubKey(string strPath)
        {

            return true;
        }
    }
}
