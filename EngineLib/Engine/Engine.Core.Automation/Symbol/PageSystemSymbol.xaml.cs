using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Engine.Common;
using Engine.Data.DBFAC;
using System;
using Engine.Mod;

namespace Engine.Core
{
    /// <summary>
    /// PageSystemSymbol.xaml 的交互逻辑
    /// </summary>
    public partial class PageSystemSymbol : UserControl
    {
        public Window Owner;
        private ServerNode _ServerNode;
        private IDBFactory<ServerNode> _DB;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serverNode"></param>
        public PageSystemSymbol(ServerNode serverNode = null)
        {
            InitializeComponent();
            if (serverNode != null)
                _DB = DbFactory.Current[serverNode];
            else if (DbFactory.CPU != null)
                _DB = DbFactory.CPU;
            else
                throw new Exception("加载PageSystemSymbol未指定连接源");            
            _ServerNode = serverNode;
            _dSystemSymbolGroup.LoadDefaultStruct();
            _dSystemSymbol.LoadDefaultStruct();
            if (!UpdateDefaultGroupView())
                return;
        }

        /// <summary>
        /// 加载变量组列表
        /// </summary>
        private bool UpdateDefaultGroupView()
        {
            ModelSystemGroup modelGroup = new ModelSystemGroup()
            {
                MarkKey = SystemGroupType.Symbol.ToString()
            };
            CallResult _result = _DB.ExcuteQuery<ModelSystemGroup>(modelGroup);
            if (_result.Fail)
            {
                sCommon.MyMsgBox(string.Format("加载变量组失败!\r\n\r\n{0}", _result.Result.ToMyString()));
                return false;
            }
            DataTable dt = _result.Result.ToMyDataTable();
            _dSystemSymbolGroup.ItemsSource = dt.DefaultView;
            return true;
        }

        /// <summary>
        /// 加载配置列表
        /// </summary>
        /// <param name="strGroupToken"></param>
        /// <returns></returns>
        private bool UpdateDefaultSymbolView(string strGroupToken)
        {
            ModelSystemSymbol model = new ModelSystemSymbol() { GroupToken = strGroupToken };
            CallResult _result = _DB.ExcuteQuery<ModelSystemSymbol>(model);
            if (_result.Fail)
            {
                sCommon.MyMsgBox(string.Format("加载变量组失败!\r\n\r\n{0}", _result.Result.ToMyString()));
                return false;
            }
            DataTable dt = _result.Result.ToMyDataTable();
            _dSystemSymbol.ItemsSource = dt.DefaultView;
            return true;
        }

        /// <summary>
        /// 符号组切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dSystemSymbolGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dSystemSymbolGroup.SelectedItem == null)
                return;
            DataRowView rowView = _dSystemSymbolGroup.SelectedItem as DataRowView;
            var model = ColumnDef.ToEntity<ModelSystemGroup>(rowView.Row);
            UpdateDefaultSymbolView(model.Token);
            _GroupTitle.Content = model.Name;
        }

        /// <summary>
        /// 双击符号项修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dSystemSymbol_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Cmd_Click(new Button() { Name = "CmdEditSymbol" }, null);
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
            ViewSystemGroup viewGroup = null;
            ViewSystemSymbol viewItem = null;
            //ModelSystemSymbolGroup modelGroup = null ;
            //ModelSystemSymbol modelItem = null;
            if (_dSystemSymbolGroup.SelectedItem != null)
            {
                rowViewGroup = _dSystemSymbolGroup.SelectedItem as DataRowView;
                viewGroup = ColumnDef.ToEntity<ViewSystemGroup>(rowViewGroup.Row);
                //modelGroup = ColumnDef.ToEntity<ModelSystemSymbolGroup>(rowViewGroup.Row);
                //modelGroup = viewGroup;
            }
            if (_dSystemSymbol.SelectedItem != null)
            {
                rowViewItem = _dSystemSymbol.SelectedItem as DataRowView;
                viewItem = ColumnDef.ToEntity<ViewSystemSymbol>(rowViewItem.Row);
                //modelItem = ColumnDef.ToEntity<ModelSystemSymbol>(rowViewItem.Row);
                //modelItem = viewItem;
            }
            switch (strCmd)
            {
                #region 符号组管理
                case "CmdAddGroup":
                    win = new winAddSystemGroup(SystemGroupType.Symbol, EditMode.AddNew) { Owner = Owner };
                    (win as winAddSystemGroup).NodeUpdated += PageSystemSymbol_NodeUpdated;
                    win.OpenWindow(strCmd);
                    break;
                case "CmdEditGroup":
                    if (_dSystemSymbolGroup.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要修改的符号组!", MsgType.Warning);
                        break;
                    }
                    win = new winAddSystemGroup(SystemGroupType.Symbol, EditMode.Modify, viewGroup) { Owner = Owner };
                    (win as winAddSystemGroup).NodeUpdated += PageSystemSymbol_NodeUpdated;
                    win.OpenWindow(strCmd);
                    break;
                case "CmdDelGroup":
                    if (_dSystemSymbolGroup.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要删除的符号组!", MsgType.Warning);
                        return;
                    }
                    MessageBoxResult ret = sCommon.MyMsgBox("删除后将不可恢复,您是否确定删除该符号组？", MsgType.Question);
                    if (ret == MessageBoxResult.No)
                        return;
                    ModelSystemSymbol modelDel = new ModelSystemSymbol() { GroupToken = viewGroup.Token };
                    string strSql = _DB.SqlDelete<ModelSystemGroup>(viewGroup).Result.ToMyString();
                    strSql+= _DB.SqlDelete<ModelSystemSymbol>(modelDel).Result.ToMyString();
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
                    UpdateDefaultGroupView();
                    UpdateDefaultSymbolView(viewGroup.Token);
                    break;
                #endregion

                #region 符号分组管理
                case "CmdAddSymbol":
                    string strGroupToken = string.Empty;
                    if (viewGroup != null)
                        strGroupToken = viewGroup.Token;
                    else if(viewItem!=null)
                        strGroupToken = viewItem.GroupToken;
                    if (string.IsNullOrEmpty(strGroupToken))
                    {
                        sCommon.MyMsgBox("请选中一个符号分组!", MsgType.Warning);
                        return;
                    }
                    ViewSystemSymbol viewSymbol = new ViewSystemSymbol() { GroupToken = strGroupToken,GroupName = viewGroup.Name };
                    win = new winAddSymbol(EditMode.AddNew, viewSymbol) { Owner = Owner };
                    (win as winAddSymbol).NodeUpdated += PageSystemSymbol_NodeUpdated; 
                    win.OpenWindow(strCmd);
                    break;
                case "CmdEditSymbol":
                    if (_dSystemSymbol.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要修改的符号项!", MsgType.Warning);
                        return;
                    }
                    win = new winAddSymbol(EditMode.Modify, viewItem) { Owner = Owner };
                    (win as winAddSymbol).NodeUpdated += PageSystemSymbol_NodeUpdated;
                    win.OpenWindow(strCmd);
                    break;
                case "CmdDelSymbol":
                    if (_dSystemSymbol.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要删除的符号项!", MsgType.Warning);
                        return;
                    }
                    _result = _DB.ExcuteDelete<ModelSystemSymbol>(viewItem);
                    if (_result.Fail)
                    {
                        string strErr = "删除失败!\r\n\r\n";
                        strErr += _result.Result.ToMyString();
                        sCommon.MyMsgBox(strErr, MsgType.Error);
                    }
                    else
                    {
                        sCommon.MyMsgBox(string.Format("已删除符号【{0}】", viewItem.Name), MsgType.Infomation);
                    }
                    UpdateDefaultSymbolView(viewItem.GroupToken);
                    break;
                 #endregion
            }
        }

        /// <summary>
        /// 更新符号组
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void PageSystemSymbol_NodeUpdated(object arg1, EditMode arg2, ViewSystemGroup arg3)
        {
            if (arg3 == null)
                return;
            CallResult _result = new CallResult() { Success = false };
            if (arg2 == EditMode.AddNew)
            {
                _result = _DB.ExcuteInsert<ModelSystemGroup>(arg3);
            }
            else if (arg2 == EditMode.Modify)
            {
                arg3.Token = arg3.Token.MarkWhere();
                _result = _DB.ExcuteUpdate<ModelSystemGroup>(arg3);
            }
            if (_result.Fail)
                sCommon.MyMsgBox(_result.Result.ToMyString());
            UpdateDefaultGroupView();
            UpdateDefaultSymbolView(arg3.Token);
        }

        /// <summary>
        /// 更新符号项
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void PageSystemSymbol_NodeUpdated(object arg1, EditMode arg2, ViewSystemSymbol arg3)
        {
            if (arg3 == null)
                return;
         
            CallResult _result = new CallResult() { Success = false };
            if (arg2 == EditMode.AddNew)
            {
                _result = _DB.ExcuteInsert<ModelSystemSymbol>(arg3);
            }
            else if (arg2 == EditMode.Modify)
            {
                arg3.GroupToken = arg3.GroupToken.MarkWhere();
                arg3.Token = arg3.Token.MarkWhere();
                arg3.CurrentValue = arg3.CurrentValue.ValueAttachMark();
                _result = _DB.ExcuteUpdate<ModelSystemSymbol>(arg3);
            }
            if (_result.Fail)
                sCommon.MyMsgBox(_result.Result.ToMyString());
            UpdateDefaultSymbolView(arg3.GroupToken);
        }
    }
}
