using Engine.ComDriver;
using Engine.Common;
using Engine.Core;
using Engine.Core.TaskSchedule;
using Engine.Data;
using Engine.Data.DBFAC;
using Engine.MVVM.Messaging;
using Engine.WpfBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Engine.MVVM
{
    /// <summary>
    /// 视图模型基类
    /// </summary>
    public abstract class ViewModelBase : DynamicEntityBase
    {
        private Dictionary<string, UserControl> DicWin = new Dictionary<string, UserControl>();

        #region 元件绑定方法
        /// <summary>
        /// 加载窗口模型
        /// </summary>
        /// <param name="SubWinName"></param>
        /// <param name="userControl"></param>
        public void AddSubWin(string SubWinName, UserControl userControl, string winName = "")
        {
            if (string.IsNullOrEmpty(SubWinName) || userControl == null)
                return;
            if (!DicWin.ContainsKey(winName) && !string.IsNullOrEmpty(winName))
                DicWin.Add(winName, userControl);
            else if (DicWin.ContainsKey(winName))
                userControl = DicWin[winName];
            TrySetProperty(SubWinName, userControl);
        }

        /// <summary>
        /// 绑定界面显示单元
        /// </summary>
        /// <param name="UiElems"></param>
        public void BindComponents(UIElementCollection UiElems)
        {
            List<Control> UiControls = UiElems.OfType<Control>().Where(x => x.Tag != null).ToList();
            foreach (Control control in UiControls)
            {
                string strControlTag = control.Tag.ToMyString();
                try
                {
                    ComponentModel CompoModel = JsonConvert.DeserializeObject<ComponentModel>(strControlTag);
                    foreach (PropItem prop in CompoModel.PropItems)
                    {
                        Binding mBind = GetBindingCondfig(prop);
                        if (prop.PropName == "Content")
                            control.SetBinding(ContentControl.ContentProperty, mBind);
                        else if (prop.PropName == "Visibility")
                            control.SetBinding(ContentControl.VisibilityProperty, mBind);
                    }
                }
                catch (Exception )
                {
                    continue;
                }

            }
            List<Shape> Shapes = UiElems.OfType<Shape>().Where(x => x.Tag != null).ToList();
            foreach (Shape shape in Shapes)
            {
                string strControlTag = shape.Tag.ToMyString();
                try
                {
                    ComponentModel CompoModel = JsonConvert.DeserializeObject<ComponentModel>(strControlTag);
                    foreach (PropItem prop in CompoModel.PropItems)
                    {
                        Binding mBind = GetBindingCondfig(prop);
                        if (prop.PropName == "Visibility")
                            shape.SetBinding(Shape.VisibilityProperty, mBind);
                        else if (prop.PropName == "Fill")
                            shape.SetBinding(Shape.FillProperty, mBind);
                        else if (prop.PropName == "Stroke")
                            shape.SetBinding(Shape.StrokeProperty, mBind);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
        /// <summary>
        /// 获取绑定设定
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private Binding GetBindingCondfig(PropItem prop)
        {
            if (prop == null)
                return null;
            Binding mBind = new Binding() { Mode = BindingMode.TwoWay, Path = new PropertyPath(prop.Path.Replace(".", "-")) };
            if (prop.ConverterKey.ToMyString() == "StateColorConvert")
                mBind.Converter = new StateColorConvert();
            else if (prop.ConverterKey.ToMyString() == "StateColorConvertInv")
                mBind.Converter = new StateColorConvertInv();
            else if (prop.ConverterKey.ToMyString() == "VisibleConvert")
                mBind.Converter = new VisibleConvert();
            else if (prop.ConverterKey.ToMyString() == "VisibleConvertInv")
                mBind.Converter = new VisibleConvertInv();
            return mBind;
        }
        #endregion
    }

    /// <summary>
    /// 视图模型运行环境数据源
    /// </summary>
    public sealed class ViewModel : ViewModelBase
    {
        public static readonly string PosItemChanged = SystemDefault.UUID;
        public static readonly string TableInvoked = SystemDefault.UUID;
        /// <summary>
        /// 数据库连接
        /// </summary>
        private IDBFactory<ServerNode> _DB;
        /// <summary>
        /// 集合符号键信息
        /// </summary>
        private Dictionary<string, string> DicSymbolKeys = new Dictionary<string, string>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string> DicSymbolValues = new Dictionary<string, string>();
        /// <summary>
        /// 回调表变化
        /// </summary>
        private Dictionary<string, ModelTableInvoke> DicTableInvoke = new Dictionary<string, ModelTableInvoke>();
        /// <summary>
        /// 扫描线程工作中
        /// </summary>
        public bool WorkerStarted { get; set; }
        /// <summary>
        /// 网络模型
        /// </summary>
        public bool NetworkModel { get; set; } = true;
        /// <summary>
        /// 网络模型是否整个列表（列表中可能包含其他App的连接资源）
        /// </summary>
        public bool NetListAll { get; set; }
        /// <summary>
        /// 数据库变量模型
        /// </summary>
        public bool DBVarModel { get; set; } = true;
        /// <summary>
        /// 视图集合模型
        /// </summary>
        public bool ViewSetModel { get; set; } = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serverNode"></param>
        public ViewModel(ServerNode serverNode = null)
        {
            if (serverNode == null)
                _DB = DbFactory.CPU.CloneInstance("MVVM");
            else
                _DB = DbFactory.Current[serverNode];
            if (_DB != null)
                UpdateDataModel(true);
        }

        /// <summary>
        /// 运行环境实例
        /// </summary>
        public static ViewModel RunTime
        {
            get
            {
                if (_RunTime == null)
                    _RunTime = new ViewModel();
                return _RunTime;
            }
        }
        private static ViewModel _RunTime;

        #region 自动更新数据模型
        /// <summary>
        /// 启动运行
        /// </summary>
        public void StartWorker()
        {
            if (WorkerStarted)
                return;
            WorkerStarted = true;
            ThreadPool.QueueUserWorkItem((Object state) =>
            {
                while (WorkerStarted)
                {
                    UpdateDataModel();
                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// 实时内存中实时更新的数据模型
        /// </summary>
        private void UpdateDataModel(bool LoadDefault = false)
        {
            #region 网络状态模型
            try
            {
                if (NetworkModel)
                {
                    ModelComLink model = new ModelComLink()
                    {
                        AppName = NetListAll ? "" : SystemDefault.AppName
                    };
                    CallResult _result = _DB.ExcuteQuery<ModelComLink>(model);
                    if (_result.Success)
                    {
                        DataTable dt = _result.Result.ToMyDataTable();
                        foreach (DataRow row in dt.Rows)
                        {
                            string strDriverName = row["DriverName"].ToMyString().Replace(".", "-");
                            string strItemVal = LoadDefault ? "0" : row["LinkState"].ToMyString();
                            string strKey = string.Format("{0}-LinkState", strDriverName);
                            TrySetProperty(strKey, strItemVal);
                        }
                    }
                }
            }
            catch (Exception )
            {

            }
            #endregion

            Dictionary<string, DataRow> DicSymbolUpdateReqRow = new Dictionary<string, DataRow>();

            #region 数据库变量模型 - 非集合类型
            try
            {
                if (DBVarModel)
                {
                    ModelSystemSymbol modelSymbol = new ModelSystemSymbol() { IsView = "1" };
                    CallResult _result = _DB.ExcuteQuery<ModelSystemSymbol>(modelSymbol);
                    if (_result.Success)
                    {
                        DataTable dt = _result.Result.ToMyDataTable();
                        foreach (DataRow row in dt.Rows)
                        {
                            string strPosKey = row["GroupName"].ToMyString().Replace(".", "-");
                            string strItemKey = row["Name"].ToMyString();
                            string strDataType = row["DataType"].ToMyString();
                            string strItemVal = LoadDefault ? "" : row["CurrentValue"].ToMyString();
                            string strKey = string.Format("{0}-{1}", strPosKey, strItemKey);
                            if (DicSymbolValues.DictFieldValue(strKey) != strItemVal)
                            {
                                DicSymbolValues.AppandDict(strKey, strItemVal);
                                //数据库变量变化时通知
                                ModelSystemSymbol symbol = ColumnDef.ToEntity<ModelSystemSymbol>(row);
                                Messenger.Default.Send<ModelSystemSymbol>(symbol, PosItemChanged);
                            }
                            if (strDataType.IfGatherData())
                            {
                                if (!DicSymbolKeys.ContainsKey(strKey))
                                {
                                    if (!DicSymbolUpdateReqRow.ContainsKey(strKey))
                                        DicSymbolUpdateReqRow.Add(strKey, row);
                                    DicSymbolKeys.Add(strKey, strItemVal);
                                }
                                else
                                {
                                    if (!DicSymbolKeys[strKey].Equals(strItemVal))
                                    {
                                        if (!DicSymbolUpdateReqRow.ContainsKey(strKey))
                                            DicSymbolUpdateReqRow.Add(strKey, row);
                                    }
                                }
                            }
                            else
                            {
                                TrySetProperty(strKey, strItemVal);
                            }
                        }
                    }
                }
            }
            catch (Exception )
            {

            }
            #endregion

            #region 数据库变量模型 - 集合类型  子工位视图、报警视图、其他自定义视图
            try
            {
                if (ViewSetModel)
                {
                    if (DicSymbolUpdateReqRow.Count > 0)
                    {
                        foreach (string key in DicSymbolUpdateReqRow.Keys)
                        {
                            DataRow row = DicSymbolUpdateReqRow[key];
                            object ObjUpdReq = MacroCommand.ParseCommand(row["LogicExpress"].ToMyString());
                            if (ObjUpdReq is null)
                                continue;
                            if (ObjUpdReq is ModelDryer subPosDryer)
                            {
                                CallResult _result = _DB.ExcuteQuery<ModelDryer>(subPosDryer);
                                if (_result.Success)
                                {
                                    DataTable dt = _result.Result.ToMyDataTable();
                                    List<ModelDryer> LstSubPos = ColumnDef.ToEntityList<ModelDryer>(dt);
                                    TrySetProperty(key, LstSubPos);
                                    DicSymbolKeys[key] = row["CurrentValue"].ToMyString();
                                }
                            }
                            else if (ObjUpdReq is ModelCabinet subPosCabinet)
                            {
                                CallResult _result = _DB.ExcuteQuery<ModelCabinet>(subPosCabinet);
                                if (_result.Success)
                                {
                                    DataTable dt = _result.Result.ToMyDataTable();
                                    List<ModelCabinet> LstSubPos = ColumnDef.ToEntityList<ModelCabinet>(dt);
                                    TrySetProperty(key, LstSubPos);
                                    DicSymbolKeys[key] = row["CurrentValue"].ToMyString();
                                }
                            }
                            else if (ObjUpdReq is ModelSubPos subPosNormal)
                            {
                                CallResult _result = _DB.ExcuteQuery<ModelSubPos>(subPosNormal);
                                if (_result.Success)
                                {
                                    DataTable dt = _result.Result.ToMyDataTable();
                                    List<ModelSubPos> LstSubPos = ColumnDef.ToEntityList<ModelSubPos>(dt);
                                    TrySetProperty(key, LstSubPos);
                                    DicSymbolKeys[key] = row["CurrentValue"].ToMyString();
                                }
                            }
                            else
                                DicSymbolKeys[key] = row["CurrentValue"].ToMyString();
                        }
                    }
                }
            }
            catch (Exception )
            {

            }
            #endregion

            #region 表调用通知
            try
            {
                //if (Project.Current.StartUP[""].IsFunctionOn())
                {
                    ModelTableInvoke mod = new ModelTableInvoke();
                    DataTable dt = _DB.ExcuteQuery(mod).Result.ToMyDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        List<ModelTableInvoke> LstInk = ColumnDef.ToEntityList<ModelTableInvoke>(dt);
                        foreach (ModelTableInvoke item in LstInk)
                        {
                            string strInkKey = item.InvokeKey;
                            string strChangedToken = item.ChangedToken.ToMyString();
                            string strUpdateTime = item.UpdateTime.ToMyString();
                            string strLastToken = DicTableInvoke.DictFieldValue(strInkKey)?.ChangedToken.ToMyString();
                            string strLastUpdateTime = DicTableInvoke.DictFieldValue(strInkKey)?.UpdateTime.ToMyString();
                            if (!strChangedToken.IsEquals(strLastToken) || !strUpdateTime.IsEquals(strLastUpdateTime))
                            {
                                //发出表调用通知
                                Messenger.Default.Send(item, TableInvoked);
                            }
                            DicTableInvoke.AppandDict(strInkKey, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            #endregion 
        }
        #endregion
    }

    /// <summary>
    /// 数据服务中心
    /// </summary>
    public abstract class ViewDataServer : DynamicEntityBase
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        protected IDBFactory<ServerNode> _DataCon;
        /// <summary>
        /// 扫描线程工作中
        /// </summary>
        public bool WorkerStarted { get; set; }

        public ViewDataServer(ServerNode serverNode = null, bool StartAutoUpdater = false)
        {
            if (serverNode == null)
                _DataCon = DbFactory.CPU.CloneInstance("MVVM");
            else
                _DataCon = DbFactory.Current[serverNode];
            if (_DataCon != null)
                UpdateDataModel(true);
            if (_DataCon != null && StartAutoUpdater)
                StartWorker();
        }

        /// <summary>
        /// 启动运行
        /// </summary>
        public void StartWorker()
        {
            if (WorkerStarted)
                return;
            WorkerStarted = true;
            ThreadPool.QueueUserWorkItem((Object state) =>
            {
                while (WorkerStarted)
                {
                    try
                    {
                        UpdateDataModel();
                    }
                    catch (Exception )
                    {

                    }                   
                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// 数据更新器，由外部重写
        /// </summary>
        /// <param name="LoadDefault"></param>
        protected virtual void UpdateDataModel(bool LoadDefault = false)
        {
            
        }
    }
}
