using Engine.Common;
using Engine.Data;
using Engine.Files;
using System.Data;
using Engine.WpfControl;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Engine.ComDriver.Schneider
{
    /// <summary>
    /// VarEditor.xaml 的交互逻辑
    /// </summary>
    public partial class PageSchneiderTM : UserControl
    {
        #region 内部变量
        private sComTMPLC mComTM;
        private bool win_isLoaded = false;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ComTM"></param>
        public PageSchneiderTM(sComTMPLC ComTM)
        {
            InitializeComponent();
            mComTM = ComTM;
            DataContext = mComTM;
        }

        #region 窗口方法
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (win_isLoaded)
                return;
            win_isLoaded = true;

            //侦听数据源变化事件
            //if (Com == null)
            //    return;

            //Com.DataSource_Changed += PlcDataSource_Changed;
        }

        /// <summary>
        /// 双击变量默认编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dgVarList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExcuteCommand("MiVarEdit");
        }

        /// <summary>
        /// 右键菜单响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_VarList_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = string.Empty;
            if (sender is MenuItem)
                strCmd = (sender as MenuItem).Tag.ToMyString();
            else if (sender is MediaButton)
                strCmd = (sender as MediaButton).Tag.ToMyString();
            ExcuteCommand(strCmd);
        }

        /// <summary>
        /// 工具菜单响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = string.Empty;
            if (sender is Button)
                strCmd = (sender as Button).Tag.ToMyString();
            else if (sender is MediaButton)
                strCmd = (sender as MediaButton).Tag.ToMyString();
            ExcuteCommand(strCmd);
        }

        /// <summary>
        /// 变量数据组切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _DataGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!win_isLoaded || _DataGroup.SelectedItem == null)
                return;
            //VariableNode _BaseVarNode = mComTM.DataSet.EditVariableNodes.FirstOrDefault
            //    (x => x.V_GROUP == _DataGroup.SelectedItem.ToString());
        }
        #endregion


        /// <summary>
        /// 更新事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void VarNodeUpdated(object sender, object arg)
        {
            ModelComPLC node = arg as ModelComPLC;
            //switch (node.strTag)
            //{
            //    case "New":
            //        //mComTM.UpdateVarNode(DataSourceChangeKey.Add, node);
            //        break;
            //    case "Edit":
            //        //mComTM.UpdateVarNode(DataSourceChangeKey.Modify, node);
            //        break;
            //}
        }

        private object ExcuteCommand(string strCmd)
        {
            if (strCmd.MatchOpenedWindow() != null)
                return null;

            switch (strCmd)
            {
                case "_CmdCycMon":
                    if (mComTM.ComWorking)
                    {
                        sCommon.MyMsgBox("请先将该设备转至在线!", MsgType.Exclamation);
                        return null;
                    }
                    break;
                case "_CmdOnceMon":
                    if (!mComTM.ComWorking)
                    {
                        sCommon.MyMsgBox("请先将该设备转至在线!", MsgType.Exclamation);
                        return null;
                    }
                    break;
            
                case "AddOneVariable":
                case "EditOneVariable":
                    ModelComPLC node = this._dgVarList.SelectedItem as ModelComPLC;
                    if (node != null)
                    {
                        winNewVariable win = new winNewVariable(node, "Edit")
                        {
                            Name = strCmd,
                            Owner = this.Tag as Window
                        };
                        win.NodeUpdated += VarNodeUpdated;
                        win.OpenWindow(strCmd);
                    }
                    break;

                case "MiVarValSet":
                    break;

                case "MiMoveUp":
                    break;

                case "MiMoveDown":
                    break;

                case "MiCut":
                    break;
                case "MiCopy":
                    break;
                case "MiPaste":
                    break;
                case "MiDel":   //删除变量
                case "_CmdVarDel":
                    if (_dgVarList.SelectedItems.Count == 0)
                    {
                        sCommon.MyMsgBox("请选中需要删除的变量！", MsgType.Warning);
                        return null;
                    }
                    MessageBoxResult ret = sCommon.MyMsgBox("您是否确定删除所有选中变量？", MsgType.Question);
                    if (ret == MessageBoxResult.Yes)
                    {
                        ObservableCollection<ModelComPLC> DelNodes = new ObservableCollection<ModelComPLC>();
                        foreach (ModelComPLC nd in _dgVarList.SelectedItems)
                            DelNodes.Add(nd);
                        //Com.UpdateVarNode(DataSourceChangeKey.Del, DelNodes);
                    }
                    break;
                case "MiSelectAll":
                    break;
                case "_CmdExport":
                case "MiExport":
                    if (_dgVarList.Items.Count > 0)
                    {
                        DataTable dt = sCommon.GetDataTable((ObservableCollection<ModelComPLC>)this._dgVarList.ItemsSource);
                        FileEIO.ExportCSV(dt);
                    }
                    else
                        sCommon.MyMsgBox("没有找到可导出的数据项!", MsgType.Warning);
                    break;

                case "_CmdImport":
                case "MiImport":

                    break;

            }
            return null;
        }
    }
}
