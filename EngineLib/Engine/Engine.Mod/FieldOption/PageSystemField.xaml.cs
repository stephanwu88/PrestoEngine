using Engine.Common;
using Engine.Data.DBFAC;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.Mod
{
    /// <summary>
    /// PageSystemField.xaml 的交互逻辑
    /// </summary>
    public partial class PageSystemField : UserControl
    {
        public Window Owner;
        private ServerNode _ServerNode;
        private IDBFactory<ServerNode> _DB;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PageSystemField()
        {
            InitializeComponent();
            _DB = DbFactory.CPU;
            if (!InitializeUiView())
                return;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PageSystemField(ServerNode serverNode = null)
        {
            InitializeComponent();
            if (serverNode != null)
                _DB = DbFactory.Current[serverNode];
            else if (DbFactory.CPU != null)
                _DB = DbFactory.CPU;
            else
                throw new Exception("未指定PageSystemField连接数据源");
            _ServerNode = serverNode;
            if (!InitializeUiView())
                return;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Owner"></param>
        /// <param name="serverNode"></param>
        public PageSystemField(IDBFactory<ServerNode> DbConn)
        {
            InitializeComponent();
            if (DbConn == null)
                throw new Exception("未指定PageSystemField连接数据源");
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
            _dGridGroup.LoadDefaultStruct();
            _dGridList.LoadDefaultStruct();
            if (!UpdateDefaultSheetGroupView())
                return false;
            return true;
        }

        /// <summary>
        /// 加载字段组
        /// </summary>
        private bool UpdateDefaultSheetGroupView()
        {
            ModelFieldGroup modelGroup = new ModelFieldGroup()
            {
                MarkKey = SystemGroupType.Field.ToString()
            };
            CallResult _result = _DB.ExcuteQuery<ModelFieldGroup>(modelGroup);
            if (_result.Fail)
            {
                sCommon.MyMsgBox(string.Format("加载分组失败!\r\n\r\n{0}", _result.Result.ToMyString()));
                return false;
            }
            DataTable dt = _result.Result.ToMyDataTable();
            _dGridGroup.ItemsSource = dt.DefaultView;
            return true;
        }


        /// <summary>
        /// 加载字段配置
        /// </summary>
        /// <param name="strGroupToken"></param>
        /// <returns></returns>
        private bool UpdateDefaultSheetColumnView(string strGroupToken)
        {
            ModelFieldItem model = new ModelFieldItem() { GroupToken = strGroupToken };
            CallResult _result = _DB.ExcuteQuery<ModelFieldItem>(model);
            if (_result.Fail)
            {
                sCommon.MyMsgBox(string.Format("加载配置项失败!\r\n\r\n{0}", _result.Result.ToMyString()));
                return false;
            }
            DataTable dt = _result.Result.ToMyDataTable();
            _dGridList.ItemsSource = dt.DefaultView;
            return true;
        }

        /// <summary>
        /// 切换分组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dGridGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dGridGroup.SelectedItem == null)
                return;
            DataRowView rowView = _dGridGroup.SelectedItem as DataRowView;
            var model = ColumnDef.ToEntity<ModelFieldGroup>(rowView.Row);
            UpdateDefaultSheetColumnView(model.Token);
            _GroupTitle.Content = model.Name;
        }

        /// <summary>
        /// 修改列配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dGridList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Cmd_Click(CmdEditItem, null);
        }

        private void Cmd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            Window win = strCmd.MatchOpenedWindow();
            if (win != null)
                return;
            DataRowView rowViewGroup = null;
            DataRowView rowViewItem = null;
            ModelFieldGroup viewGroup = null;
            ModelFieldItem viewItem = null;
            //ModelFieldGroup modelGroup = null;
            //ModelFieldItem modelItem = null;
            if (_dGridGroup.SelectedItem != null)
            {
                rowViewGroup = _dGridGroup.SelectedItem as DataRowView;
                viewGroup = ColumnDef.ToEntity<ModelFieldGroup>(rowViewGroup.Row);
                //modelGroup = ColumnDef.ToEntity<ModelFieldGroup>(rowViewGroup.Row);
                //modelGroup = viewGroup;
            }
            if (_dGridList.SelectedItem != null)
            {
                rowViewItem = _dGridList.SelectedItem as DataRowView;
                viewItem = ColumnDef.ToEntity<ModelFieldItem>(rowViewItem.Row);
                //modelItem = ColumnDef.ToEntity<ModelFieldItem>(rowViewItem.Row);
                //modelItem = viewItem;
            }
            switch (strCmd)
            {
                #region 字段组管理
                case "CmdAddGroup":
                    win = new winAddFieldGroup(SystemGroupType.Field, EditMode.AddNew) { Owner = Owner };
                    (win as winAddFieldGroup).NodeUpdated += PageSystemField_NodeUpdated;
                    win.OpenWindow(strCmd);
                    break;
                case "CmdEditGroup":
                    if (_dGridGroup.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要修改的项！", MsgType.Warning);
                        return;
                    }
                    if (win == null)
                    {
                        win = new winAddFieldGroup(SystemGroupType.Field, EditMode.Modify, viewGroup) { Owner = Owner };
                        (win as winAddFieldGroup).NodeUpdated += PageSystemField_NodeUpdated;
                        win.OpenWindow(strCmd);
                    }
                    break;
                case "CmdDelGroup":
                    if (_dGridGroup.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要处理的项！", MsgType.Warning);
                        return;
                    }
                    MessageBoxResult ret = sCommon.MyMsgBox("删除后将不可恢复,您是否确定删除该报表分组？", MsgType.Question);
                    if (ret == MessageBoxResult.No)
                        return;
                    ModelFieldItem modelDel = new ModelFieldItem() { GroupToken = viewGroup.Token };
                    string strSql = _DB.SqlDelete<ModelFieldGroup>(viewGroup).Result.ToMyString();
                    strSql += _DB.SqlDelete<ModelFieldItem>(modelDel).Result.ToMyString();
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

                #region 字段项配置
                case "CmdAddItem":
                    string strGroupToken = string.Empty;
                    if (viewGroup != null)
                        strGroupToken = viewGroup.Token;
                    else if (viewItem != null)
                        strGroupToken = viewItem.GroupToken;
                    if (string.IsNullOrEmpty(strGroupToken))
                    {
                        sCommon.MyMsgBox("请选中一个分组!", MsgType.Warning);
                        return;
                    }
                    ModelFieldItem modItem = new ModelFieldItem()
                    {
                        GroupToken = strGroupToken,
                        GroupName = viewGroup.Name,
                        MarkKey = viewGroup.MarkKey,
                        IsEditable = 0,
                        IsActive = 1
                    };
                    win = new winAddFieldItem(EditMode.AddNew, modItem) { Owner = Owner };
                    (win as winAddFieldItem).NodeUpdated += PageSystemField_NodeUpdated;
                    win.OpenWindow(strCmd);
                    break;

                case "CmdEditItem":
                    if (_dGridList.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要修改的配置项!", MsgType.Warning);
                        return;
                    }
                    win = new winAddFieldItem(EditMode.Modify, viewItem) { Owner = Owner };
                    (win as winAddFieldItem).NodeUpdated += PageSystemField_NodeUpdated; ;
                    win.OpenWindow(strCmd);
                    break;

                case "CmdDelItem":
                    if (_dGridList.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要删除的配置项!", MsgType.Warning);
                        return;
                    }
                    _result = _DB.ExcuteDelete<ModelFieldItem>(viewItem);
                    if (_result.Fail)
                    {
                        string strErr = "删除失败!\r\n\r\n";
                        strErr += _result.Result.ToMyString();
                        sCommon.MyMsgBox(strErr, MsgType.Error);
                    }
                    else
                    {
                        string strFieldText = viewItem.FieldCode;
                        if (!string.IsNullOrEmpty(viewItem.Comment))
                            strFieldText = string.Format("【{0}】", viewItem.Comment);
                        sCommon.MyMsgBox(string.Format("已删除配置项【{0}】", strFieldText), MsgType.Infomation);
                    }
                    UpdateDefaultSheetColumnView(viewItem.GroupToken);
                    break;

                case "_CmdMoveItemUp":
                    break;
                case "_CmdMoveItemDown":
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// 更新分组
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void PageSystemField_NodeUpdated(object arg1, EditMode arg2, ModelFieldGroup arg3)
        {
            if (arg3 == null)
                return;
            CallResult _result = new CallResult();
            if (arg2 == EditMode.AddNew)
            {
                _result = _DB.ExcuteInsert<ModelFieldGroup>(arg3);
            }
            else if (arg2 == EditMode.Modify)
            {
                arg3.Token = arg3.Token.MarkWhere();
                ModelFieldItem fieldItem = new ModelFieldItem()
                {
                    GroupToken = arg3.Token,
                    GroupName = arg3.Name,
                    MarkKey = arg3.MarkKey
                };
                fieldItem.ValueAttachMark();
                arg3.ValueAttachMark();
                string strSql = _DB.SqlUpdate<ModelFieldItem>(fieldItem).Result.ToMyString();
                strSql += _DB.SqlUpdate<ModelFieldGroup>(arg3).Result.ToMyString();
                _result = _DB.ExcuteSQL(strSql);
            }
            if (_result.Fail)
                sCommon.MyMsgBox(_result.Result.ToMyString());
            UpdateDefaultSheetGroupView();
            UpdateDefaultSheetColumnView(arg3.Token);
        }

        /// <summary>
        /// 更新配置项
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void PageSystemField_NodeUpdated(object arg1, EditMode arg2, ModelFieldItem arg3)
        {
            if (arg3 == null)
                return;
            CallResult _result = new CallResult() { Success = false };
            if (arg2 == EditMode.AddNew)
            {
                _result = _DB.ExcuteInsert<ModelFieldItem>(arg3);
            }
            else if (arg2 == EditMode.Modify)
            {
                arg3.GroupToken = arg3.GroupToken.MarkWhere();
                arg3.Token = arg3.Token.MarkWhere();
                arg3.ValueAttachMark();
                _result = _DB.ExcuteUpdate<ModelFieldItem>(arg3);
            }
            if (_result.Fail)
                sCommon.MyMsgBox(_result.Result.ToMyString());
            UpdateDefaultSheetColumnView(arg3.GroupToken);
        }
    }
}
