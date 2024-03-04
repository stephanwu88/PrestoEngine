using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Engine.ComDriver
{
    /// <summary>
    /// PLC变量管理器
    /// </summary>
    /// <typeparam name="TVariableNode"></typeparam>
    public class PlcDataIO<TVariableNode, TVariableGroupNode> : NotifyObject
        where TVariableNode : new() 
        where TVariableGroupNode : new()
    {
        #region 内部变量
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        protected IDBFactory<ServerNode> mDB;
        /// <summary>
        /// 变量默认存储表
        /// </summary>
        private string DefaultTableName = "plc_variablelist";
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public PlcDataIO() { }
        /// <summary>
        /// 重载构造函数
        /// </summary>
        public PlcDataIO(ServerNode serverNode)
        {
            mServerNode = serverNode;
            mDB = DbCommon.CreateInstance(mServerNode);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 变量集合
        /// </summary>
        public List<TVariableNode> VariableNodes { get; set; }
        /// <summary>
        /// 编辑变量集合
        /// </summary>
        public List<TVariableNode> EditVariableNodes { get; set; }
        /// <summary>
        /// 变量分组
        /// 分组后方便批量读取
        /// </summary>
        protected List<TVariableGroupNode> VariableGroups { get; set; }
        /// <summary>
        /// 数据库服务器节点信息
        /// </summary>
        protected ServerNode mServerNode { get; set; }
        /// <summary>
        /// 变量数据组名称
        /// </summary>
        public List<string> ListDataGroupNames { get => _ListDataGroupNames; set { _ListDataGroupNames = value; RaisePropertyChanged("ListDataGroupNames"); } }
        private List<string> _ListDataGroupNames = new List<string>()
        { "All","Default","bit","dbBit","dbF","dbMan","dbWord","dido","word","Alarm"};
        #endregion

        #region 事件
        /// <summary>
        /// 变量节点
        /// </summary>
        public event Action<object, object> VariableList_Updated;
        #endregion

        #region 方法
        /// <summary>
        /// 从数据库加载变量
        /// </summary>
        /// <returns></returns>
        public bool LoadVaribles()
        {
            return LoadVaribles(DefaultTableName);
        }
        /// <summary>
        /// 从数据库加载变量
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public bool LoadVaribles(string strTableName)
        {
            if (mServerNode == null)
                return false;
            return LoadVaribles(mServerNode,strTableName);
        }
        /// <summary>
        /// 从数据库加载变量
        /// </summary>
        /// <param name="serverNode"></param>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public bool LoadVaribles(ServerNode serverNode, string strTableName)
        {
            if (mDB == null)
                return false;
            DataTable dt = mDB.ExcuteQuery(string.Format("select * from {0} where Driver_IDT='{1}'", "", "")).Result.ToMyDataTable();
            TVariableNode varNode = new TVariableNode();
            List<TVariableNode> ListVariables = dt.ToEntityList<TVariableNode>();
            VariableNodes = ListVariables;
            return true;
        }
        /// <summary>
        /// 从外部文件导入变量数据
        /// </summary>
        /// <param name="file">*.xlsx  *.xml *.json</param>
        /// <returns></returns>
        public bool ImportVariables(string fileName)
        {
            if (!File.Exists(fileName))
                throw new ArgumentException("未找到待导入目标文件");

            string strExtension = Path.GetExtension(fileName).ToUpper();
            if (strExtension == "XML")
                EditVariableNodes = ParseXmlDocument(fileName);
            else if (strExtension == "JSON")
                EditVariableNodes = ParseJsonDocument(fileName);
            else if (strExtension == "XLS" || strExtension == "XLSX")
                EditVariableNodes = ParseExcelDocument(fileName);
            else
                throw new ArgumentException("不正确的文件格式");
            return true;
        }
        /// <summary>
        /// 解析Excel文件变量
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public List<TVariableNode> ParseExcelDocument(string fileName) { return null; }
        /// <summary>
        /// 解析XML文件变量
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public List<TVariableNode> ParseXmlDocument(string fileName) { return null; }
        /// <summary>
        /// 解析json文件变量
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public List<TVariableNode> ParseJsonDocument(string fileName) { return null; }
        /// <summary>
        /// 导出各种格式的变量
        /// </summary>
        /// <returns></returns>
        public bool ExportVariables()
        {
            string strFileName = sCommon.ShowSaveFileDialog();
            if (File.Exists(strFileName))
            {
                System.Windows.MessageBoxResult ret = sCommon.MyMsgBox("文件已存在，是否需要覆盖?", MsgType.Question);
                if (ret == System.Windows.MessageBoxResult.Yes)
                    return ExportVariables(strFileName);
            }
            return false;
        }
        /// <summary>
        /// 导出各种格式的变量
        /// </summary>
        /// <param name="strFile"></param>
        /// <returns></returns>
        public bool ExportVariables(string fileName)
        {
            string strExtension = Path.GetExtension(fileName).ToUpper();
            if (strExtension == "XML")
            {
                return true;
            }
            else if (strExtension == "XLS")
            {
                return true;
            }
            else if (strExtension == "XLSX")
            {
                return true;
            }
            else if (strExtension == "JSON")
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 写入到数据库
        /// </summary>
        /// <returns></returns>
        public bool WriteToServer()
        {
            return WriteToServer(mServerNode, DefaultTableName);
        }
        /// <summary>
        /// 写入到数据库
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public bool WriteToServer(string strTableName)
        {
            return WriteToServer(mServerNode, strTableName);
        }
        /// <summary>
        /// 写入到数据库
        /// </summary>
        /// <param name="serverNode"></param>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public bool WriteToServer(ServerNode serverNode, string strTableName)
        {
            mDB.ExcuteSQL("");
            return true;
        }
        #endregion
        
        /// <summary>
        /// 变量访问索引器
        /// </summary>
        /// <param name="strVariableName">变量名称</param>
        /// <returns></returns>
        public object this[string strVariableName]
        {
            get => VariableNodes;
            set
            {
                if (strVariableName == "")
                {

                }
            }
        }
    }
}
