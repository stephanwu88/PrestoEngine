using Engine.Common;
using Engine.Data.DBFAC;
using Engine.WpfControl;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.ComDriver.HEAO
{
    /// <summary>
    /// PageHeaoHCP.xaml 的交互逻辑
    /// </summary>
    public partial class PageHeaoHCP : UserControl
    {
        public Window Owner;
        private IDBFactory<ServerNode> _DB = DbFactory.CPU;
        private ServerNode _ServerNode;
        private string _DriverToken;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DriverItem"></param>
        /// <param name="serverNode"></param>
        public PageHeaoHCP(ModelDriverItem DriverItem, ServerNode serverNode = null)
        {
            InitializeComponent();
            if (DriverItem == null)
                throw new ArgumentException("未指定PageHeaoHCP构造参数");
            if (serverNode != null)
            {
                _ServerNode = serverNode;
                _DB = DbFactory.Current[_ServerNode];
            }
            else if (DbFactory.CPU != null)
            {
                _DB = DbFactory.CPU;
            }
            else
            {
                throw new Exception("加载PageHeaoHCP时未获取到指定连接源");
            }
            _DriverToken = DriverItem.DriverToken;
            _dGridComHeao.LoadDefaultStruct();
            UpdateDefaultView();
        }

        /// <summary>
        /// 更新通讯变量表
        /// </summary>
        private void UpdateDefaultView()
        {
            string strColumnName = ColumnAttribute.Column<ModelComHeao>("DriverToken").Name.ToMyString();
            ModelComHeao com = new ModelComHeao()
            {
                DriverToken = string.Format("{0}='{1}' order by ID asc", strColumnName, _DriverToken)
                .MarkExpress().MarkWhere()
            };
            CallResult _result = _DB.ExcuteQuery<ModelComHeao>(com);
            DataTable dt = _result.Result.ToMyDataTable();
            _dGridComHeao.ItemsSource = dt.DefaultView;
        }

        /// <summary>
        /// 窗口指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = string.Empty;
            if (sender is MenuItem mItem)
                strCmd = mItem.Tag.ToMyString();
            else if (sender is MediaButton medButton)
                strCmd = medButton.Tag.ToMyString();
            else if (sender is Button button)
                strCmd = button.Tag.ToMyString();
            Window win = strCmd.MatchOpenedWindow();
            if (win != null)
                return;
            switch (strCmd)
            {
                case "CmdAdd":
                    win = new winAddNew(EditMode.AddNew) { Owner = Owner };
                    (win as winAddNew).NodeUpdated += PageHeaoHCP_NodeUpdated;
                    win.OpenWindow(strCmd);
                    break;
                case "CmdEdit":
                    if (_dGridComHeao.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中修改项", MsgType.Error);
                        return;
                    }
                    DataRowView rowView = _dGridComHeao.SelectedItem as DataRowView;
                    ViewComHeao model = ColumnDef.ToEntity<ViewComHeao>(rowView.Row);
                    win = new winAddNew(EditMode.Modify, model) { Owner = Owner };
                    (win as winAddNew).NodeUpdated += PageHeaoHCP_NodeUpdated;
                    win.OpenWindow(strCmd);
                    break;
                case "CmdDel":
                    if (_dGridComHeao.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中删除项", MsgType.Error);
                        return;
                    }
                    MessageBoxResult ret = sCommon.MyMsgBox("请确认删除项？", MsgType.Question);
                    if (ret == MessageBoxResult.No)
                        return;
                    rowView = _dGridComHeao.SelectedItem as DataRowView;
                    model = ColumnDef.ToEntity<ViewComHeao>(rowView.Row);
                    ModelComHeao modelDel = new ModelComHeao() { ID = model.ID };
                    CallResult _result = _DB.ExcuteDelete<ModelComHeao>(modelDel);
                    if (_result.Fail)
                    {
                        string strErr = "删除失败!\r\n\r\n";
                        strErr += _result.Result.ToMyString();
                        sCommon.MyMsgBox(strErr, MsgType.Error);
                    }
                    UpdateDefaultView();
                    break;

                case "CmdMoveUp":
                    break;
                case "CmdMoveDown":
                    break;

                case "CmdExport":
                    break;
                case "CmdImport":
                    break;

                case "CmdCopy":
                    break;
                case "CmdCut":
                    break;
                case "CmdPaste":
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void PageHeaoHCP_NodeUpdated(object arg1, EditMode arg2, ViewComHeao arg3)
        {
            if (arg3 == null)
                return;
            CallResult _result = new CallResult() { Success = false };
            if (arg2 == EditMode.AddNew)
            {
                arg3.DriverToken = _DriverToken;
                _result = _DB.ExcuteInsert<ViewComHeao>(arg3);
            }
            else if (arg2 == EditMode.Modify)
            {
                arg3.DriverToken = _DriverToken.MarkWhere();
                _result = _DB.ExcuteUpdate<ViewComHeao>(arg3);
            }
            if (_result.Fail)
                sCommon.MyMsgBox(_result.Result.ToMyString());
            UpdateDefaultView();
        }

        private void _dGridComHeao_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void _FCType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
