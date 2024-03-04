using Engine.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Mod
{
    /// <summary>
    /// winAddFieldGroup.xaml 的交互逻辑
    /// </summary>
    public partial class winAddFieldGroup : Window
    {
        private EditMode _EditMode;
        private ModelFieldGroup _ModGroup;
        public event Action<object, EditMode, ModelFieldGroup> NodeUpdated;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="GroupType"></param>
        /// <param name="EditMode"></param>
        /// <param name="ViewGroup"></param>
        public winAddFieldGroup(SystemGroupType GroupType, EditMode EditMode, ModelFieldGroup ViewGroup = null)
        {
            InitializeComponent();
            _EditMode = EditMode;
            if (_EditMode == EditMode.Modify && ViewGroup == null)
                throw new ArgumentException("未指定变量组修改对象");
            if (ViewGroup != null)
                _ModGroup = ViewGroup;
            if (_EditMode == EditMode.AddNew)
            {
                Title = string.Format("添加{0}", GroupType.FetchDescription());
                if (_ModGroup == null)
                    _ModGroup = new ModelFieldGroup();
            }
            else if (_EditMode == EditMode.Modify)
            {
                Title = string.Format("修改{0}", GroupType.FetchDescription());
                _ContAddMode.Visibility = Visibility.Visible;
            }
            _ModGroup.MarkKey = GroupType.ToString();
            DataContext = _ModGroup;
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
                    CallResult _result = _ModGroup.Validate();
                    if (_result.Fail)
                    {
                        sCommon.MyMsgBox(_result.Result.ToMyString());
                        return;
                    }
                    if (_EditMode == EditMode.AddNew)
                        _ModGroup.Token = SystemDefault.UUID;
                    if (NodeUpdated != null)
                        NodeUpdated(this, _EditMode, _ModGroup);
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
