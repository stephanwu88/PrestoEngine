using Engine.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Core
{
    /// <summary>
    /// winAddSymbol.xaml 的交互逻辑
    /// </summary>
    public partial class winAddSymbol : Window
    {
        private EditMode _EditMode;
        private ViewSystemSymbol _SymbolItem;
        public event Action<object, EditMode, ViewSystemSymbol> NodeUpdated;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="EditMode"></param>
        /// <param name="SymbolItem"></param>
        public winAddSymbol(EditMode EditMode, ViewSystemSymbol SymbolItem = null)
        {
            InitializeComponent();
            _EditMode = EditMode;
            if (_EditMode == EditMode.Modify && SymbolItem == null)
                new ArgumentException("未指定变量编辑对象");
            if (SymbolItem != null)
                _SymbolItem = SymbolItem;
            if (_EditMode == EditMode.AddNew)
            {
                if (_SymbolItem == null)
                    _SymbolItem = new ViewSystemSymbol();
            }
            else if (_EditMode == EditMode.Modify)
            {
                Title = "修改变量";
                _ContAddMode.Visibility = Visibility.Visible;
            }
            DataContext = _SymbolItem;
        }

        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            switch (strCmd)
            {
                case "CmdSure":
                    CallResult _result = _SymbolItem.Validate();
                    if (_result.Fail)
                    {
                        sCommon.MyMsgBox(_result.Result.ToMyString());
                        return;
                    }
                    if (_EditMode == EditMode.AddNew)
                        _SymbolItem.Token = SystemDefault.UUID;
                    if (NodeUpdated != null)
                        NodeUpdated(this, _EditMode, _SymbolItem);
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
