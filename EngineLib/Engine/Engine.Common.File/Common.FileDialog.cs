
namespace Engine.Common
{
    /// <summary>
    /// 文件浏览
    /// 打开、保存、浏览文件对话框
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 打开文件对话框
        /// </summary>
        /// <returns>文件全路径（含文件名）</returns>
        public static string ShowOpenFileDialog(string Filter = "所有文件(*.*)|*.*", int FilterIndex = 1)
        {
            string strFileName = string.Empty;
            //添加引用，选择System.Windows.Forms.dll
            //using System.Windows.Forms; 命名空间引用。
            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Multiselect = true;
                dialog.Title = "选择目标文件";
                //Filter缺省值0，系统默认取1，相当于有效ID从1开始
                //dialog.Filter = "所有文件(*.*)|*.*|其他文件(*.xml,*.dat,*.bin)|*.xml;*.dat;*.bin";
                dialog.Filter = Filter;
                dialog.FilterIndex = FilterIndex;
                //值为false，初始目录为最后选择目录；为true，初始目录是固定的  
                //dialog.InitialDirectory = @"C:\";
                dialog.RestoreDirectory = false;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    strFileName = dialog.FileName;
            }
            return strFileName;
        }
        /// <summary>
        /// 保存文件对话框
        /// </summary>
        /// <returns>文件全路径（含文件名）</returns>
        public static string ShowSaveFileDialog(string DefaultFile = "", string Filter = "所有文件(*.*)|*.*", int FilterIndex = 1, string winTitle = "保存文件")
        {
            string strFileName = string.Empty;
            //添加引用，选择System.Windows.Forms.dll
            //using System.Windows.Forms; 命名空间引用
            using (System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog())
            {
                dialog.Title = winTitle;
                dialog.FileName = DefaultFile;
                //dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                //Filter缺省值0，系统默认取1，相当于有效ID从1开始
                //dialog.Filter = "所有文件(*.*)|*.*|其他文件(*.xml,*.dat,*.bin)|*.xml;*.dat;*.bin";
                dialog.Filter = Filter;
                dialog.FilterIndex = FilterIndex;
                //dialog.DefaultExt = ".xml";
                //文件重复时提示是否覆盖(默认为true)
                //文件不存在时提示是否需要创建
                dialog.CreatePrompt = false;
                //值为false，初始目录为最后选择目录；为true，初始目录是固定的  
                //dialog.InitialDirectory = @"C:\";
                dialog.RestoreDirectory = false;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    strFileName = dialog.FileName;
            }
            return strFileName;
        }
        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <returns>返回指定文件夹绝对路径</returns>
        public static string FolderBrowser(string strDefaultPath = "C:\\")
        {
            string foldPath = string.Empty;
            System.Windows.Forms.FolderBrowserDialog fDialog = new System.Windows.Forms.FolderBrowserDialog();
            fDialog.Description = "请选择一个文件夹";
            fDialog.ShowNewFolderButton = true;     //显示新建文件夹按钮
            if (fDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                foldPath = fDialog.SelectedPath;  // 获取文件的路径
            if (foldPath.Length == 0)
                foldPath = strDefaultPath;
            return foldPath;
        }
        /// <summary>
        /// 打开文件夹或文件
        /// </summary>
        /// <param name="fileName">文件夹路径或文件名</param>
        /// <returns></returns>
        public static bool OpenFileOrFolder(string fileName)
        {
            if (System.IO.Directory.Exists(fileName) || System.IO.File.Exists(fileName))
            {
                System.Diagnostics.Process.Start("explorer.exe", fileName);
                return true;
            }
            else
                return false;
        }
    }
}
