using Engine.ComDriver.HEAO;
using Engine.ComDriver.Instrument;
using Engine.ComDriver.Siemens;
using Engine.Common;
using Engine.Data.DBFAC;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.ComDriver
{
    /// <summary>
    /// PageController.xaml 的交互逻辑
    /// </summary>
    public partial class PageController : UserControl
    {
        public Window Owner;
        private ServerNode _ServerNode;
        private IDBFactory<ServerNode> _DB;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serverNode"></param>
        public PageController(ServerNode serverNode = null)
        {
            InitializeComponent();
            if (serverNode != null)
                _DB = DbFactory.Current[serverNode];
            else if (DbFactory.CPU != null)
                _DB = DbFactory.CPU;
            else
                throw new Exception("加载PageController时未指定连接数据源");
            _ServerNode = serverNode;
            _dDriverMain.LoadDefaultStruct();
            if (!UpdateDefaultView())
                return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool UpdateDefaultView()
        {
            ModelDriverItem item = new ModelDriverItem();
            CallResult _result = _DB.ExcuteQuery<ModelDriverItem>(item);
            if (_result.Fail)
            {
                sCommon.MyMsgBox(string.Format("加载控制器列表错误!\r\n\r\n{0}",_result.Result.ToMyString()));
                return false;
            }
            DataTable dt = _result.Result.ToMyDataTable();
            _dDriverMain.ItemsSource = dt.DefaultView;
            return true;
        }

        /// <summary>
        /// 命令处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            Window win = strCmd.MatchOpenedWindow();
            if (win != null)
                return;
            switch (strCmd)
            {
                case "CmdAddController":    //添加控制器
                    win = new winNewController(EditMode.AddNew) { Name = strCmd, Owner = Owner };
                    (win as winNewController).NodeUpdated += PageController_NodeUpdated;
                    win.Show();
                    break;

                case "CmdEditController":   //修改控制器
                    if (_dDriverMain.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要修改的控制器!", MsgType.Error);
                        return;
                    }
                    DataRowView rowView = _dDriverMain.SelectedItem as DataRowView;
                    ViewDriverItem model = ColumnDef.ToEntity<ViewDriverItem>(rowView.Row);
                    win = new winNewController(EditMode.Modify,model) { Name = strCmd, Owner = Owner };
                    (win as winNewController).NodeUpdated += PageController_NodeUpdated;
                    win.Show();
                    break;

                case "CmdDelController":    //删除控制器
                    if (_dDriverMain.SelectedItem == null)
                    {
                        sCommon.MyMsgBox("请选中需要删除的控制器!", MsgType.Error);
                        return;
                    }
                    rowView = _dDriverMain.SelectedItem as DataRowView;
                    model = ColumnDef.ToEntity<ViewDriverItem>(rowView.Row);
                    ModelDriverItem modelDel = new ModelDriverItem() { DriverToken = model.DriverToken };
                    CallResult _result = _DB.ExcuteDelete<ModelDriverItem>(modelDel);
                    if (_result.Fail)
                    {
                        string strErr = "删除失败!\r\n\r\n";
                        strErr += _result.Result.ToMyString();
                        sCommon.MyMsgBox(strErr, MsgType.Error);
                    }
                    else
                    {
                        sCommon.MyMsgBox(string.Format("已删除控制器【{0}】", model.Name), MsgType.Infomation);
                    }
                    UpdateDefaultView();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void PageController_NodeUpdated(object arg1, EditMode arg2, ViewDriverItem arg3)
        {
            if (arg3 == null)
                return;
            CallResult _result = new CallResult() { Success = false };
            if (arg2 == EditMode.AddNew)
            {
                _result = _DB.ExcuteInsert<ModelDriverItem>(arg3);
            }
            else if (arg2 == EditMode.Modify)
            {
                arg3.DriverToken = arg3.DriverToken.MarkWhere();
                _result = _DB.ExcuteUpdate<ModelDriverItem>(arg3);
            }
            if (_result.Fail)
                sCommon.MyMsgBox(_result.Result.ToMyString());
            UpdateDefaultView();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dDriverMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_dDriverMain.SelectedItem == null)
                return;
            DataRow row = (_dDriverMain.SelectedItem as DataRowView).Row;
            ModelDriverItem comDrv = ColumnDef.ToEntity<ModelDriverItem>(row);
            if (string.IsNullOrEmpty(comDrv.DriverToken))
                return;
            switch (comDrv.Protocol)
            {
                case "HeaoInter":
                    _TabComMain.AddTab(comDrv.Name, new PageHeaoHCP(comDrv,_ServerNode) { Owner = Owner}, true);
                    break;

                case "S71200":
                case "S7200SMART":
                case "S71500":
                case "S7300":
                    _TabComMain.AddTab(comDrv.Name, new PageVarList(comDrv,_ServerNode), true);
                    break;
            }
            if (comDrv.ComLibTable.ToMyString().ToLower().Contains("driver_instr"))
            {
                _TabComMain.AddTab(comDrv.Name, new PageComInstr(comDrv, _ServerNode), true);
            }
        }
    }
}
