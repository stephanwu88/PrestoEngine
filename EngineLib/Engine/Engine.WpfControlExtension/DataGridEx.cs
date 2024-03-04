using Engine.Common;
using Engine.Core;
using Engine.Data.DBFAC;
using Engine.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;
using Engine.Mod;

namespace Engine.WpfControl
{
    /// <summary>
    /// 列表配置源
    /// </summary>
    public enum ColumnSource
    {
        Database,
        ExcelFile
    }

    /// <summary>
    /// 表格控件父类，定义基本行为特性
    /// </summary>
    public abstract class DataGridFather : DataGrid
    {
        #region 成员变量
        /// <summary>
        /// 列宽调整中
        /// </summary>
        private bool _ColumnWidthChanging;
        /// <summary>
        /// 自定义列宽度变化事件
        /// </summary>
        public event Action<object> ColumnWidthChanged;
        #endregion

        #region 属性
        private bool _IsGridValid = false;
        /// <summary>
        /// Grid是否已经加载有效列并绑定数据源
        /// </summary>
        public bool IsGridValid
        {
            get => _IsGridValid;
            set
            {
                _IsGridValid = value;
            }
        }

        /// <summary>
        /// 列配置来源
        /// </summary>
        public ColumnSource ColumnSource
        {
            get { return (ColumnSource)GetValue(ColumnSourceProperty); }
            set { SetValue(ColumnSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnSourceProperty =
            DependencyProperty.Register("ColumnSource", typeof(ColumnSource), typeof(DataGridFather), new PropertyMetadata(ColumnSource.Database));


        /// <summary>
        /// 配置文件
        /// ex: $\Locales\DataSheet.xls
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(DataGridFather), new PropertyMetadata(""));

        /// <summary>
        /// 表名称
        /// </summary>
        public string SheetName
        {
            get { return (string)GetValue(SheetNameProperty); }
            set { SetValue(SheetNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SheetName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SheetNameProperty =
            DependencyProperty.Register("SheetName", typeof(string), typeof(DataGridFather), new PropertyMetadata(""));

        /// <summary>
        /// 数据库节点
        /// </summary>
        public ServerNode ServerNode
        {
            get { return (ServerNode)GetValue(ServerNodeProperty); }
            set { SetValue(ServerNodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ServerNode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServerNodeProperty =
            DependencyProperty.Register("ServerNode", typeof(ServerNode), typeof(DataGridFather), new PropertyMetadata(null));

        #endregion

        #region 成员函数、事件和方法
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataGridFather()
        {
            //设置本模式使用之固定属性
            CanUserResizeColumns = true;
            CanUserReorderColumns = true;
            IsReadOnly = true;
            AutoGenerateColumns = false;
            CanUserAddRows = false;
            HorizontalGridLinesBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            VerticalGridLinesBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            Background = Brushes.Transparent;
            //ColumnReordered += DataGrid_ColumnReordered;
        }

        /// <summary>
        /// 自动加载表结构、并处理相关绑定
        /// </summary>
        /// <returns></returns>
        public bool LoadDefaultStruct(List<ModelSheetColumn> TabHead)
        {
            if (LoadGridColumns(TabHead))
            {
                ListenColumnWidthChangeAndBindEvent();
                _IsGridValid = true;
            }
            return true;
        }

        /// <summary>
        /// 设置数据表结构，加载列信息
        /// </summary>
        /// <returns>True:加载成功  False：加载失败</returns>
        public bool LoadGridColumns(List<ModelSheetColumn> LstColumns)
        {
            Columns.Clear();
            foreach (ModelSheetColumn s in LstColumns)
            {
                if (string.IsNullOrWhiteSpace(s.ColHeader))
                    continue;
                switch (s.ColStyle)
                {
                    case "Text":
                        this.Columns.Add(new DataGridTextColumn()
                        {
                            Header = s.ColHeader.Trim(),
                            Width = s.ColWidth.ToMyInt(),
                            Visibility = s.ColVisible.ToMyBool() == true ? Visibility.Visible : Visibility.Hidden,
                            Binding = new Binding(s.ColName)
                        });
                        break;
                    case "CheckBox":
                        this.Columns.Add(new DataGridCheckBoxColumn()
                        {
                            Header = s.ColHeader.Trim(),
                            Width = s.ColWidth.ToMyInt(),
                            Visibility = s.ColVisible.ToMyBool() == true ? Visibility.Visible : Visibility.Hidden,
                            Binding = new Binding(s.ColName)
                        });
                        break;
                    case "TextBlock":
                        this.Columns.Add(new DataGridTextColumn()
                        {
                            Header = s.ColHeader.Trim(),
                            Width = s.ColWidth.ToMyInt(),
                            Visibility = s.ColVisible.ToMyBool() == true ? Visibility.Visible : Visibility.Hidden,
                            Binding = new Binding(s.ColName)
                        });
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// 侦听列宽变化并添加相应的处理事件
        /// </summary>
        private void ListenColumnWidthChangeAndBindEvent()
        {
            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));
            foreach (DataGridColumn col in this.Columns)
            {
                descriptor.AddValueChanged(col, new EventHandler(ColumnWidth_Changed));
            }
        }

        /// <summary>
        /// 列宽度变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnWidth_Changed(object sender, EventArgs e)
        {
            _ColumnWidthChanging = true;
            if (sender != null)
                Mouse.AddPreviewMouseUpHandler(this, BaseDataGrid_MouseLeftButtonUp);
        }

        /// <summary>
        /// 捕捉列宽变化结束时，通知事件ColumnWidthChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseDataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_ColumnWidthChanging)
            {
                _ColumnWidthChanging = false;
                if (ColumnWidthChanged != null)
                    ColumnWidthChanged(this);
            }
        }
        #endregion
    }

    /// <summary>
    /// 自定义表格控件
    /// </summary>
    public sealed class DataGridEx : DataGridFather
    {
        private static IDBFactory<ServerNode> _DB;
        private static ExcelEIO _Excel;
        private string GroupToken;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataGridEx() : base()
        {
            ColumnWidthChanged += ColumnWidth_Changed;
            ColumnReordered += DataGrid_ColumnReordered;
        }

        /// <summary>
        /// 列宽调整
        /// </summary>
        /// <param name="sender"></param>
        public void ColumnWidth_Changed(object sender)
        {
            AutoSaveGridColumnMsg();
        }

        /// <summary>
        /// 列顺序发生变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            AutoSaveGridColumnMsg();
        }

        /// <summary>
        /// 加载表列结构
        /// </summary>
        /// <returns></returns>
        public bool LoadDefaultStruct()
        {
            try
            {
                if (ColumnSource == ColumnSource.Database)
                    return LoadDefaultStruct(ServerNode);
                else if (ColumnSource == ColumnSource.ExcelFile)
                    return LoadDefaultStruct("");
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 从数据库中获取报表结构
        /// </summary>
        /// <param name="serverNode"></param>
        /// <returns></returns>
        public bool LoadDefaultStruct(ServerNode node)
        {
            try
            {
                if (string.IsNullOrEmpty(FileName))
                    throw new Exception("未设置数据连接表!");
                if (node != null)
                    _DB = DbFactory.Current[node];
                else if (ServerNode != null)
                    _DB = DbFactory.Current[ServerNode];
                else
                    throw new Exception("未设置列配置服务!");
                return LoadDefaultStruct(_DB);
            }
            catch (Exception ex)
            {
                throw ex;
            }  
        }

        /// <summary>
        /// 从数据库中获取报表结构
        /// </summary>
        /// <param name="DbCon"></param>
        /// <returns></returns>
        public bool LoadDefaultStruct(IDBFactory<ServerNode> DbCon)
        {
            try
            {
                if (string.IsNullOrEmpty(FileName))
                    throw new Exception("未设置数据连接表!");
                _DB = DbCon;
                List<ModelSheetColumn> SheetColumns = new List<ModelSheetColumn>();
                ModelSheetColumn modelSheet = new ModelSheetColumn() { GroupName = FileName };
                CallResult _result = _DB.ExcuteQuery(modelSheet);
                if (_result.Fail)
                    throw new Exception(_result.Result.ToMyString());
                DataTable dt = _result.Result.ToMyDataTable();
                SheetColumns = ColumnDef.ToEntityList<ModelSheetColumn>(dt);
                return LoadDefaultStruct(SheetColumns);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 从Excel文件中获取报表结构
        /// </summary>
        /// <returns></returns>
        public bool LoadDefaultStruct(string ExcelFileName)
        {
            if (!string.IsNullOrEmpty(ExcelFileName))
                FileName = ExcelFileName;
            string strFileName = FileName.Replace("$", Directory.GetCurrentDirectory());
            if (string.IsNullOrEmpty(strFileName) || !File.Exists(strFileName))
                throw new Exception("未找到列配置文件!");
            if (_Excel == null)
                _Excel = new ExcelEIO();
            LocalSource ds = new LocalSource() { SourceFile = strFileName, ProviderName = "engine.data.msexcel" };
            DataTable dt = _Excel.GetDataTable(ds, SheetName).ToMyDataTable();
            List<ModelSheetColumn> TabHead = sCommon.ToEntityList<ModelSheetColumn>(dt);
            return base.LoadDefaultStruct(TabHead);
        }

        /// <summary>
        /// 自动保存表格列宽和列顺序
        /// </summary>
        /// <returns></returns>
        public bool AutoSaveGridColumnMsg()
        {
            try
            {
                //string[] GroupKey  Header  Width  OrderID
                List<string[]> ColMsg = new List<string[]>();
                if (this.Tag.ToMyString().Length == 0 || this.Columns.Count == 0)
                    return false;
                string strGroupKey = this.Tag.ToMyString();
                foreach (var s in this.Columns)
                {
                    if (s is DataGridTextColumn)
                    {
                        DataGridTextColumn Col = s as DataGridTextColumn;
                        ColMsg.Add(new string[] { strGroupKey, Col.Header.ToMyString(), Col.Width.ToMyString(), Col.DisplayIndex.ToString() });
                    }
                    else if (s is DataGridCheckBoxColumn)
                    {
                        DataGridCheckBoxColumn Col = s as DataGridCheckBoxColumn;
                        ColMsg.Add(new string[] { strGroupKey, Col.Header.ToMyString(), Col.Width.ToMyString(), Col.DisplayIndex.ToString() });
                    }
                    else if (s is DataGridTemplateColumn)
                    {
                        DataGridTemplateColumn Col = s as DataGridTemplateColumn;
                        ColMsg.Add(new string[] { strGroupKey, Col.Header.ToMyString(), Col.Width.ToMyString(), Col.DisplayIndex.ToString() });
                    }
                    else if (s is DataGridColumn)
                    {
                        DataGridColumn Col = s as DataGridColumn;
                        ColMsg.Add(new string[] { strGroupKey, Col.Header.ToMyString(), Col.Width.ToMyString(), Col.DisplayIndex.ToString() });
                    }
                }

                //string[] GroupKey  Header  Width  OrderID
                string strSql = string.Empty;
                if (ColMsg.Count > 0)
                {
                    List<ModelSheetColumn> ListColumns = new List<ModelSheetColumn>();
                    foreach (string[] s in ColMsg)
                    {
                        ListColumns.Add(new ModelSheetColumn()
                        {
                            ColHeader = s[1],
                            ColWidth = s[2],
                            OrderID = s[3]
                        });
                    }
                    return Accessor.Current.UpdateSheetColumns(strGroupKey, ListColumns);
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
