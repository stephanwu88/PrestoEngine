using System;
using System.Windows.Controls;

namespace Engine.Common
{
    /// <summary>
    /// Xaml页面加载控制
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 根据Uri加载xaml页面
        /// </summary>
        /// <param name="UriString">ex: /YourAssemblyName;component/YourPageName.xaml</param>
        /// <returns></returns>
        public static Frame LoadUIFrame(string UriString)
        {
            // 创建一个新的Frame对象
            Frame frame = new Frame();
            Page pd = new Page();
            // 创建一个Uri对象来指定页面的路径
            Uri uri = new Uri(UriString, UriKind.Relative);
            // 使用Frame导航到指定的Uri
            frame.Source = uri;
            // 将Frame显示在窗口中
            return frame;
        }
    }
}
