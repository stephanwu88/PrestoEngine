using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Engine.Common;
using Engine.Data.DBFAC;
using System;

namespace Engine.Mod
{
    /// <summary>
    /// PageReportMan.xaml 的交互逻辑
    /// </summary>
    public partial class PageSheetManage : UserControl
    {
        public Window Owner;
        private ServerNode _ServerNode;
        private IDBFactory<ServerNode> _DB;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Owner"></param>
        /// <param name="serverNode"></param>
        public PageSheetManage(ServerNode serverNode = null)
        {
            InitializeComponent();
            if (serverNode != null)
                _DB = DbFactory.Current[serverNode];
            else if (DbFactory.CPU != null)
                _DB = DbFactory.CPU;
            else
                throw new Exception("未指定SheetManage连接数据源");
            _ServerNode = serverNode;
            if (!InitializeUiView())
                return;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Owner"></param>
        /// <param name="serverNode"></param>
        public PageSheetManage(IDBFactory<ServerNode> DbConn)
        {
            InitializeComponent();
            if (DbConn == null)
                throw new Exception("未指定SheetManage连接数据源");
            _DB = DbConn;
            _ServerNode = DbConn.ConNode;
            if (!InitializeUiView())
                return;
        }

        /// <summary>
        /// 初始化UI加载
        /// </summary>
        /// <returns></returns>
        private bool InitializeUiView()
        {
            _dSheetGroup.LoadDefaultStruct();
            _dSheetColumn.LoadDefaultStruct();
            if (!UpdateDefaultSheetGroupView())
                return false;
            return true;
        }

        /// <summary>
        /// 加载符号组列表
        /// </summary>
        private bool UpdateDefaultSheetGroupView()
        {
            ModelSheetGroup modelGroup = new ModelSheetGroup()
            {
                MarkKey = SystemGroupType.DataSheet.ToString()
            };
            CallResult _result = _DB.ExcuteQuery<ModelSheetGroup>(modelGroup);
            if (_result.Fail)
            {
                sCommon.MyMsgBox(string.Format("加载报表组失败!\r\n\r\n{0}", _result.Result.ToMyString()));
                return false;
            }
            DataTable dt = _result.Result.ToMyDataTable();
            _dSheetGroup.ItemsSource = dt.DefaultView;
            return true;
        }

        /// <summary>
        /// 加载列配置
        /// </summary>
        /// <param name="strGroupToken"></param>
        /// <returns></returns>
        private bool UpdateDefaultSheetColumnView(string strGroupToken)
        {
            ModelSheetColumn model = new ModelSheetColumn() { GroupToken = strGroupToken };
            CallResult _result = _DB.ExcuteQuery<ModelSheetColumn>(model);
            if (_result.Fail)
            {
                sCommon.MyMsgBox(string.Format("加载列配置失败!\r\n\r\n{0}", _result.Result.ToMyString()));
                return false;
            }
            DataTable dt = _result.Result.ToMyDataTable();
            _dSheetColumn.ItemsSource = dt.DefaultView;
            return true;
        }

        /// <summary>
        /// 切换报表组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dSheetGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dSheetGroup.SelectedItem == null)
                return;
            DataRowView rowView = _dSheetGroup.SelectedItem as DataRowView;
            var model = ColumnDef.ToEntity<ModelSheetGroup>(rowView.Row);
            UpdateDefaultSheetColumnView(model.Token);
            _GroupTitle.Content = model.Name;
        }

        /// <summary>
        /// 修改列配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dSheetColumn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Cmd_Click(new Button() { Name = "CmdEditColumn" }, null);
        }

        /// <summary>
        /// 窗口命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            Window win = strCmd.MatchOpenedWindow();
            if (win != null)
                return;
            DataRowView rowViewGroup = null;
            DataRowView rowViewItem = null;
            ViewSheetGroup viewGroup = null;
            ViewSheetColumn viewItem = null;
            //ModelSheetGroup modelGroup = null;
            //ModelSheetColumn modelItem = null;
            if (_dSheetGroup.SelectedItem != null)
            {
                rowViewGroup = _dSheetGroup.SelectedItem as DataRowView;
                viewGroup = ColumnDef.ToEntity<ViewSheetGroup>(rowViewGroup.Row);
                //modelGroup = ColumnDef.ToEntity<ModelSheetGroup>(rowViewGroup.Row);
                //modelGroup = viewGroup;
            }
            if (_dSheetColumn.SelectedItem != null)
            {
                rowViewItem = _dSheetColumn.SelectedItem as DataRowView;
                viewItem = ColumnDef.ToEntity<ViewSheetColumn>(rowViewItem.Row);
                //modelItem = ColumnDef.ToEntity<ModelSheetColumn>(rowViewItem.Row);
                //modelItem = viewItem;
            }
            switch (strCmd)
            {
                #region 报表组管理
                case "CmdAddGroup":
                    win = new winAddSheetGroup(SystemGroupType.DataSheet, EditMode.AddNew) { Owner = Owner };
                    (win as winAddSheetGroup).NodeUpdated += PageSheetManage_NodeUpdated; 
                    win.OpenWindow(strCmd);
                    break;
                case "CmdEditGroup":
                    if (_dSheetGroup.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要修改的项！", MsgType.Warning);
                        return;
                    }
                    if (win == null)
                    {
                        win = new winAddSheetGroup(SystemGroupType.DataSheet, EditMode.Modify, viewGroup) { Owner = Owner };
                        (win as winAddSheetGroup).NodeUpdated += PageSheetManage_NodeUpdated;
                        win.OpenWindow(strCmd);
                    }
                    break;
                case "CmdDelGroup":
                    if (_dSheetGroup.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要处理的项！", MsgType.Warning);
                        return;
                    }
                    MessageBoxResult ret = sCommon.MyMsgBox("删除后将不可恢复,您是否确定删除该报表分组？",  MsgType.Question);
                    if (ret == MessageBoxResult.No)
                        return;
                    ModelSheetColumn modelDel = new ModelSheetColumn() { GroupToken = viewGroup.Token };
                    string strSql = _DB.SqlDelete<ModelSheetGroup>(viewGroup).Result.ToMyString();
                    strSql += _DB.SqlDelete<ModelSheetColumn>(modelDel).Result.ToMyString();
                    CallResult _result = _DB.ExcuteSQL(strSql);
                    if (_result.Fail)
                    {
                        string strErr = "删除失败!\r\n\r\n";
                        strErr += _result.Result.ToMyString();
                        sCommon.MyMsgBox(strErr, MsgType.Error);
                    }
                    else
                    {
                        sCommon.MyMsgBox(string.Format("已删除符号组【{0}】", viewGroup.Name), MsgType.Infomation);
                    }
                    UpdateDefaultSheetGroupView();
                    UpdateDefaultSheetColumnView(viewGroup.Token);
                    break;
                #endregion

                #region 报表列配置
                case "CmdAddColumn":
                    string strGroupToken = string.Empty;
                    if (viewGroup != null)
                        strGroupToken = viewGroup.Token;
                    else if (viewItem != null)
                        strGroupToken = viewItem.GroupToken;
                    if (string.IsNullOrEmpty(strGroupToken))
                    {
                        sCommon.MyMsgBox("请选中一个报表分组!", MsgType.Warning);
                        return;
                    }
                    ViewSheetColumn viewColumn = new ViewSheetColumn() { GroupToken = strGroupToken, ColVisible = 0 };
                    win = new winAddSheetColumn(EditMode.AddNew, viewColumn) { Owner = Owner };
                    (win as winAddSheetColumn).NodeUpdated += PageSheetManage_NodeUpdated; 
                    win.OpenWindow(strCmd);
                    break;

                case "CmdEditColumn":
                    if (_dSheetColumn.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要修改的列配置项!", MsgType.Warning);
                        return;
                    }
                    win = new winAddSheetColumn(EditMode.Modify, viewItem) { Owner = Owner };
                    (win as winAddSheetColumn).NodeUpdated += PageSheetManage_NodeUpdated;
                    win.OpenWindow(strCmd);
                    break;

                case "CmdDelColumn":
                    if (_dSheetColumn.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要删除的列配置项!", MsgType.Warning);
                        return;
                    }
                    _result = _DB.ExcuteDelete<ModelSheetColumn>(viewItem);
                    if (_result.Fail)
                    {
                        string strErr = "删除失败!\r\n\r\n";
                        strErr += _result.Result.ToMyString();
                        sCommon.MyMsgBox(strErr, MsgType.Error);
                    }
                    else
                    {
                        sCommon.MyMsgBox(string.Format("已删除列配置项【{0}】", viewItem.ColName), MsgType.Infomation);
                    }
                    UpdateDefaultSheetColumnView(viewItem.GroupToken);
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// 更新报表组
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void PageSheetManage_NodeUpdated(object arg1, EditMode arg2, ViewSheetGroup arg3)
        {
            if (arg3 == null)
                return;
            CallResult _result = new CallResult() { Success = false };
            if (arg2 == EditMode.AddNew)
            {
                _result = _DB.ExcuteInsert<ModelSheetGroup>(arg3);
            }
            else if (arg2 == EditMode.Modify)
            {
                arg3.Token = arg3.Token.MarkWhere();
                _result = _DB.ExcuteUpdate<ModelSheetGroup>(arg3);
            }
            if (_result.Fail)
                sCommon.MyMsgBox(_result.Result.ToMyString());
            UpdateDefaultSheetGroupView();
            UpdateDefaultSheetColumnView(arg3.Token);
        }

        /// <summary>
        /// 更新报表列配置项
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void PageSheetManage_NodeUpdated(object arg1, EditMode arg2, ViewSheetColumn arg3)
        {
            if (arg3 == null)
                return;

            CallResult _result = new CallResult() { Success = false };
            if (arg2 == EditMode.AddNew)
            {
                _result = _DB.ExcuteInsert<ModelSheetColumn>(arg3);
            }
            else if (arg2 == EditMode.Modify)
            {
                arg3.GroupToken = arg3.GroupToken.MarkWhere();
                arg3.Token = arg3.Token.MarkWhere();
                _result = _DB.ExcuteUpdate<ModelSheetColumn>(arg3);
            }
            if (_result.Fail)
                sCommon.MyMsgBox(_result.Result.ToMyString());
            UpdateDefaultSheetColumnView(arg3.GroupToken);
        }
    }
}
