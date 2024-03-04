using System.Collections.Generic;

namespace Engine.ComDriver.Siemens
{
    /// <summary>
    /// 西门子PLC变量表 - 视图
    /// </summary>
    public class ViewComPLC : ModelComPLC
    {
        public ViewComPLC()
        {
            if (AddrTypeItems == null)
            {
                AddrTypeItems = new List<string>();
                AddrTypeItems.Add("Bool");
                AddrTypeItems.Add("Byte");
                AddrTypeItems.Add("Int");
                AddrTypeItems.Add("DInt");
                AddrTypeItems.Add("Word");
                AddrTypeItems.Add("DWord");
                AddrTypeItems.Add("Real");
                AddrTypeItems.Add("String");
            }
            if (DataAccessItems == null)
            {
                DataAccessItems = new List<string>();
                DataAccessItems.Add("读/写");
                DataAccessItems.Add("只读");
            }

            AddrType = AddrTypeItems[0];
            DataAccess = DataAccessItems[0];
        }

        public List<string> AddrTypeItems { get; set; }
        public List<string> DataAccessItems { get; set; }

        /// <summary>
        /// 验证对象
        /// </summary>
        /// <returns></returns>
        public override CallResult Validate()
        {
            return Validate(null, true);
        }
    }

    /// <summary>
    /// 程序内使用简易变量结构
    /// </summary>
    public class VarStruct
    {
        /// <summary>
        /// 变量类型
        /// </summary>
        public string DataType;
        /// <summary>
        /// 变量关键字  I，Q,M,(DB:S71200) (V:S7200Smart 相当于DB1)
        /// </summary>
        public string VarKey;
        /// <summary>
        /// 起始字节地址索引
        /// </summary>
        public int ByteStart;
        /// <summary>
        /// 位地址偏移
        /// </summary>
        public int BitOffset;
        /// <summary>
        /// 通讯地址范围的起始地址
        /// </summary>
        public int AddrRangeStart;
    }

    /// <summary>
    /// 通讯数据地址范围结构
    /// </summary>
    public class AddrRange
    {
        /// <summary>
        /// I,Q,M,(DB:S71200) (V:S7200Smart 相当于DB1)
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

}
