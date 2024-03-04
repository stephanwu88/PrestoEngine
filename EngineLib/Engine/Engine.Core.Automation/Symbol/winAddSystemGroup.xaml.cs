using Engine.Common;
using Engine.Mod;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Core
{
    /// <summary>
    /// winAddSystemGroup.xaml 的交互逻辑
    /// </summary>
    public partial class winAddSystemGroup : Window
    {
        private EditMode _EditMode;
        private ViewSystemGroup _ViewSystemGroup;
        public event Action<object, EditMode, ViewSystemGroup> NodeUpdated;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="GroupType"></param>
        /// <param name="EditMode"></param>
        /// <param name="ViewGroup"></param>
        public winAddSystemGroup(SystemGroupType GroupType, EditMode EditMode, ViewSystemGroup ViewGroup = null)
        {
            InitializeComponent();
            _EditMode = EditMode;
            if (_EditMode == EditMode.Modify && ViewGroup == null)
                new ArgumentException("未指定变量组修改对象");
            if (ViewGroup != null)
                _ViewSystemGroup = ViewGroup;
            if (_EditMode == EditMode.AddNew)
            {
                Title = string.Format("添加{0}", GroupType.FetchDescription());
                if (_ViewSystemGroup == null)
                    _ViewSystemGroup = new ViewSystemGroup();
            }
            else if (_EditMode == EditMode.Modify)
            {
                Title = string.Format("修改{0}", GroupType.FetchDescription());
                _ContAddMode.Visibility = Visibility.Visible;
            }
            _ViewSystemGroup.MarkKey = GroupType.ToString();
            DataContext = _ViewSystemGroup;
        }

        /// <summary>
        /// 窗口指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            switch (strCmd)
            {
                case "CmdSure":
                    CallResult _result = _ViewSystemGroup.Validate();
                    if (_result.Fail)
                    {
                        sCommon.MyMsgBox(_result.Result.ToMyString());
                        return;
                    }
                    if (_EditMode == EditMode.AddNew)
                        _ViewSystemGroup.Token = SystemDefault.UUID;
                    if (NodeUpdated != null)
                        NodeUpdated(this, _EditMode, _ViewSystemGroup);
                    if (_EditMode == EditMode.AddNew && _ContAddMode.IsChecked == true)
                        return;
                    break;

                case "CmdCancel":
                    break;
            }
            Close();
        }
    }
}
