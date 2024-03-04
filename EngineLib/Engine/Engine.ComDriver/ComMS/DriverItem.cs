
namespace Engine.ComDriver
{
    /// <summary>
    /// 通讯驱动信息
    /// </summary>
    public class DriverItem<TComParam> where TComParam : new()
    {
        #region 内部变量
        private TComParam _ComParam;
        #endregion
        /// <summary>
        /// 设备主键
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 控制器ID
        /// </summary>
        public string DriverToken { get; set; }
        /// <summary>
        /// 控制区域
        /// </summary>
        public string SnDomain { get; set; }
        /// <summary>
        /// 驱动设备名称
        /// </summary>
        public string DriverName { get; set; }
        /// <summary>
        /// 通讯链路 以太网/串口
        /// </summary>
        public string ComLink { get; set; }
        /// <summary>
        /// 通讯协议
        /// </summary>
        public string DriverPtl { get; set; }
        /// <summary>
        /// 驱动库
        /// 格式: 程序集|驱动类库|驱动字符类型|字符编码格式|信息描述
        /// ex: Engine | Engine.ComDriver.HEAO.sComHeaoHCP | Hex | gb2312 | V1.0
        /// </summary>
        public string Provider { get; set; }
        /// <summary>
        /// 设备码/报文起始码
        /// </summary>
        public string ComHeader { get; set; }
        /// <summary>
        /// 报文结束码
        /// </summary>
        public string ComEDX { get; set; }
        /// <summary>
        /// 通讯数据格式
        /// </summary>
        public string CharType { get; set; }
        /// <summary>
        /// 关联数据表
        /// </summary>
        public string RelatedDataTable { get; set; }
        /// <summary>
        /// 主机型号
        /// </summary>
        public string CpuType { get; set; }
        /// <summary>
        /// 通讯参数
        /// </summary>
        public TComParam ComParam
        {
            get
            {
                if (_ComParam == null)
                    _ComParam = new TComParam();
                return _ComParam;
            }
            set { _ComParam = value; }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Comment { get; set; } = string.Empty;
        /// <summary>
        /// 启用状态
        /// </summary>
        public string DriverEn { get; set; }
        /// <summary>
        /// 开启服务器控制
        /// </summary>
        public string ServerCmdEn { get; set; } = string.Empty;
        /// <summary>
        /// 状态自动更新到服务器
        /// </summary>
        public string ServerUpdEn { get; set; } = string.Empty;
    }

    /// <summary>
    /// 网络通讯参数
    /// </summary>
    public class NetworkCommParam
    {
        #region 内部变量
        private byte _StationAddr = 0xFF;
        private int _CycleTime = 100;
        #endregion
        /// <summary>
        /// 从站站号
        /// </summary>
        public byte StationAddr { get => _StationAddr; set { _StationAddr = value; } }
        /// <summary>
        /// 通讯IP
        /// </summary>
        public string ComIP { get; set; }
        /// <summary>
        /// 通讯端口
        /// </summary>
        public int ComPort { get; set; }
        /// <summary>
        /// 通讯循环周期 ms
        /// </summary>
        public int CycleTime { get => _CycleTime; set { _CycleTime = value; } }
        /// <summary>
        /// 工作模式 Client or Server
        /// </summary>
        public string WorkMode { get; set; }
    }

    /// <summary>
    /// 串口通讯参数
    /// </summary>
    public class SerialCommParam
    {
        #region 内部变量
        private byte _StationAddr = 0x01;
        private int _CycleTime = 100;
        #endregion
        /// <summary>
        /// 从站站号
        /// </summary>
        public byte StationAddr { get => _StationAddr; set { _StationAddr = value; } }
        /// <summary>
        /// 串口号
        /// </summary>
        public string ComPort { get; set; }
        /// <summary>
        /// 波特率
        /// </summary>
        public int BundRate { get; set; }
        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBits { get; set; }
        /// <summary>
        /// 停止位
        /// </summary>
        public int StopBits { get; set; }
        /// <summary>
        /// 奇偶校验位
        /// </summary>
        public string Parity { get; set; }

    }
}