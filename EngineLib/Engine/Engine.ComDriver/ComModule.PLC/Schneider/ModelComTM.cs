using Engine.Data.DBFAC;
using System;

namespace Engine.ComDriver.Schneider
{
    /// <summary>
    /// 通讯数据地址范围结构
    /// </summary>
    public class VariableGroupNode
    {
        /// <summary>
        /// I,Q,M
        /// </summary>
        public string addrKey;
        /// <summary>
        /// DB块索引
        /// </summary>
        public int dbNo;
        /// <summary>
        /// 通讯地址范围下限
        /// </summary>
        public int min;
        /// <summary>
        /// 通讯地址范围上限定
        /// </summary>
        public int max;
        /// <summary>
        /// 通讯范围长度
        /// </summary>
        public int len;
    }


    /// <summary>
    /// 施耐德变量管理器
    /// </summary>
    public class PlcDataSet : PlcDataIO<ModelComPLC, VariableGroupNode>
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DataSource"></param>
        public PlcDataSet(string DataSource)
        {

        }
        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="serverNode"></param>
        public PlcDataSet(ServerNode serverNode) : base(serverNode)
        {
            if (serverNode == null)
                throw new ArgumentException("服务器连接信息无效");
        }
        #endregion
    }
}
