using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Engine.Common
{
    /// <summary>
    /// TabControl页管理
    /// </summary>
    public static partial class sCommon
    {
        private static TabControl _TabControl;

        //添加Tab窗口
        public static void AddTab(this TabControl _TabMain, string tabTitle, UserControl _UserControl, bool WithCloseWin = false)
        {
            _TabControl = _TabMain;
            int tabCount = _TabMain.Items.Count;
            //string tabTitle = _UserControl.Name;
            for (int i = 0; i < tabCount; i++)
            {

                if (((TabItem)_TabMain.Items[i]).Tag.ToMyString() == tabTitle)
                {
                    _TabMain.SelectedIndex = i;
                    return;
                }
            }
            TabItem item = new TabItem();
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            Label headerString = new Label();
            headerString.Content = tabTitle;
            headerString.HorizontalContentAlignment = HorizontalAlignment.Center;
            headerString.VerticalContentAlignment = VerticalAlignment.Center;
            // headerString.MouseDoubleClick += Header_DoubleClick;
            panel.Children.Add(headerString);

            if (WithCloseWin)
            {
                Button winCloseBtn = new Button();
                winCloseBtn.Content = "×";
                winCloseBtn.Background = new SolidColorBrush(Colors.Transparent);
                winCloseBtn.BorderBrush = new SolidColorBrush(Colors.Transparent);
                //winCloseBtn.Width = 50;
                winCloseBtn.Height = 18;
                //winCloseBtn.Margin = new Thickness(80, 2, 2, 2);
                winCloseBtn.HorizontalContentAlignment = HorizontalAlignment.Center;
                winCloseBtn.VerticalContentAlignment = VerticalAlignment.Center;
                winCloseBtn.Click += winCloseBtn_Click;
                winCloseBtn.Tag = item;
                panel.Children.Add(winCloseBtn);
            }

            item.Content = _UserControl;

            item.Tag = tabTitle;
            item.Header = panel;

            _TabMain.Items.Add(item);
            _TabMain.SelectedIndex = _TabMain.Items.Count - 1;
        }

        //移除Tab窗口
        public static void RemoveTab(this TabControl _TabMain, string tabTitle)
        {
            int tabCount = _TabMain.Items.Count;
            //string tabTitle = _UserControl.Name;
            for (int i = 0; i < tabCount; i++)
            {
                TabItem ti = _TabMain.Items[i] as TabItem;
                if (ti.Tag.ToMyString() == tabTitle)
                {
                    _TabMain.Items.Remove(ti);
                    return;
                }
            }
        }

        /// <summary>
        /// 关闭当前Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void winCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            _TabControl.Items.Remove(((Button)sender).Tag);
        }
    }
}
