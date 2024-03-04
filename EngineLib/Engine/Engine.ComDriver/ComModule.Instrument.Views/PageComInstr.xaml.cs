using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;
using Engine.Files;
using Engine.WpfControl;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Engine.ComDriver.Instrument
{
    /// <summary>
    /// PageComInstr.xaml 的交互逻辑
    /// </summary>
    public partial class PageComInstr : UserControl
    {
        public Window Owner;
        private IDBFactory<ServerNode> _DB = DbFactory.CPU;
        private ServerNode _ServerNode;
        private string _DriverToken;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DriverItem"></param>
        /// <param name="serverNode"></param>
        public PageComInstr(ModelDriverItem DriverItem, ServerNode serverNode = null)
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
                throw new Exception("加载PageVarList时未获取到指定连接源");
            }
            _DriverToken = DriverItem.DriverToken;
            _dgVarList.LoadDefaultStruct();
            LoadUiDefault();
            UpdateDefaultView();
        }

        private void LoadUiDefault()
        {
            _DataGroup.Items.Clear();
            _DataGroup.Items.Add("Default");
            _DataGroup.Items.Add("bit");
            _DataGroup.Items.Add("dbBit");
            _DataGroup.Items.Add("dbF");
            _DataGroup.Items.Add("dbMan");
            _DataGroup.Items.Add("dbPort");
            _DataGroup.Items.Add("dbRcp");
            _DataGroup.Items.Add("dbS");
            _DataGroup.Items.Add("dbWord");
            _DataGroup.Items.Add("dido");
            _DataGroup.Items.Add("word");
            _DataGroup.Items.Add("Alarm");
            _DataGroup.Items.Add("inf");
            _DataGroup.SelectedIndex = 0;
        }

        /// <summary>
        /// 更新通讯变量表
        /// </summary>
        private void UpdateDefaultView()
        {
            string strColumnName = ColumnAttribute.Column<ModelComInstr>("DriverToken").Name.ToMyString();
            ModelComInstr com = new ModelComInstr()
            {
                DriverToken = string.Format("{0}='{1}' order by ID asc", strColumnName, _DriverToken)
                .MarkExpress().MarkWhere()
            };
            CallResult _result = _DB.ExcuteQuery<ModelComInstr>(com);
            DataTable dt = _result.Result.ToMyDataTable();
            _dgVarList.ItemsSource = dt.DefaultView;
        }

        /// <summary>
        /// 变量分组切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DataGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_DataGroup.SelectedItem != null)
            {
                
            }
        }

        /// <summary>
        /// 双击变量默认编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dgVarList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void PageVarList_NodeUpdated(object arg1, EditMode arg2, ViewComSymbol arg3)
        {
            if (arg3 == null)
                return;
            CallResult _result = new CallResult() { Success = false };
            if (arg2 == EditMode.AddNew)
            {
                arg3.DriverToken = _DriverToken;
                _result = _DB.ExcuteInsert<ViewComSymbol>(arg3);
            }
            else if (arg2 == EditMode.Modify)
            {
                arg3.DriverToken = _DriverToken.MarkWhere();
                _result = _DB.ExcuteUpdate<ViewComSymbol>(arg3);
            }
            if (_result.Fail)
                sCommon.MyMsgBox(_result.Result.ToMyString());
            UpdateDefaultView();
        }

        /// <summary>
        /// 工具菜单响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = string.Empty;
            if (sender is MenuItem mItem)
                strCmd = mItem.Tag.ToMyString();
            else if (sender is MediaButton medBtn)
                strCmd = medBtn.Tag.ToMyString();
            else if (sender is Button button)
                strCmd = button.Tag.ToMyString();
            Window win = strCmd.MatchOpenedWindow();
            if (win != null)
                return;
            switch (strCmd)
            {
                case "CmdAdd":
                    win = new winNewVariable(EditMode.AddNew) { Owner = Owner };
                    (win as winNewVariable).NodeUpdated += PageVarList_NodeUpdated;
                    win.OpenWindow(strCmd);
                    break;
                case "CmdEdit":
                    if (_dgVarList.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中修改项", MsgType.Error);
                        return;
                    }
                    DataRowView rowView = this._dgVarList.SelectedItem as DataRowView;
                    ViewComSymbol model = ColumnDef.ToEntity<ViewComSymbol>(rowView.Row);
                    win = new winNewVariable(EditMode.Modify, model) { Owner = Owner };
                    (win as winNewVariable).NodeUpdated += PageVarList_NodeUpdated; ;
                    win.OpenWindow(strCmd);
                    break;
                case "CmdWrite":
                    break;
                case "CmdMoveUp":
                    break;
                case "CmdMoveDown":
                    break;
                case "CmdImport":
                    break;
                case "CmdExport":
                    if (_dgVarList.Items.Count > 0)
                    {
                        DataTable dt = sCommon.GetDataTable((ObservableCollection<ModelComPLC>)this._dgVarList.ItemsSource);
                        FileEIO.ExportCSV(dt);
                    }
                    else
                        sCommon.MyMsgBox("没有找到可导出的数据项!", MsgType.Warning);
                    break;

                case "CmdCut":
                    break;
                case "CmdCopy":
                    break;
                case "CmdPaste":
                    break;
                case "CmdDel":
                    if (_dgVarList.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中删除项", MsgType.Error);
                        return;
                    }
                    MessageBoxResult ret = sCommon.MyMsgBox("请确认删除项？", MsgType.Question);
                    if (ret == MessageBoxResult.No)
                        return;
                    rowView = _dgVarList.SelectedItem as DataRowView;
                    model = ColumnDef.ToEntity<ViewComSymbol>(rowView.Row);
                    ViewComSymbol modelDel = new ViewComSymbol() { ID = model.ID };
                    CallResult _result = _DB.ExcuteDelete<ViewComSymbol>(modelDel);
                    if (_result.Fail)
                    {
                        string strErr = "删除失败!\r\n\r\n";
                        strErr += _result.Result.ToMyString();
                        sCommon.MyMsgBox(strErr, MsgType.Error);
                    }
                    UpdateDefaultView();
                    break;


                case "CmdCycleMon":

                    break;
                case "CmdOnceMon":
                    UpdateDefaultView();
                    break;
            }
        }
    }
}
