using System;
using System.Collections.ObjectModel;

namespace Engine.ComDriver
{
    /// <summary>
    /// 通讯数据规约
    /// </summary>
    public interface IComPlcWorker
    {
        #region 属性
        /// <summary>
        /// 通讯变量列表
        /// </summary>
        ObservableCollection<ModelComPLC> CommList { get; set; }
        #endregion

        #region 事件
        ///// <summary>
        ///// 通讯变量值发生变化,数据变化更新
        ///// </summary>
        //event Action<object, DataTable> DataValue_Changed;
        ///// <summary>
        ///// 报警项发生变化
        ///// </summary>
        //event Action<object, DataRow[]> AlarmNode_Changed;
        /// <summary>
        /// 通讯轮询接收到数据
        /// </summary>
        event Action<object, ObservableCollection<ModelComPLC>> ComData_Received;
        /// <summary>
        /// 通讯启动通知
        /// </summary>
        event Action<object> Com_Started;
        #endregion

        #region 方法
        /// <summary>
        /// 通讯连续采样开启
        /// </summary>
        /// <returns></returns>
        void ComStart();
        /// <summary>
        /// 通讯采样停止
        /// </summary>
        /// <returns></returns>
        void ComStop();
        /// <summary>
        /// 从PLC读取单个变量
        /// 通过 LastErrorCode or LastErrorString判别成功与否
        /// </summary>
        /// <param name="variable">ex: "IX0.0", "MX10.0", "MB20", "T45", etc.</param>
        /// <param name="Format"></param>
        /// <returns></returns>
        object Read(string variable, DataFormat Format = DataFormat.Default);
        /// <summary>
        /// 写入PLC单个变量
        /// </summary>
        /// <param name="variable">>ex: "MX10.0", "MW20", etc.</param>
        /// <param name="value"></param>
        /// <param name="Format"></param>
        /// <returns></returns>
        ErrorCode Write(string variable, object value, DataFormat Format = DataFormat.Default);
        #endregion
    }

    /// <summary>
    /// 通讯数据规约
    /// </summary>
    /// <typeparam name="TPlcDataType">Input,Output,Memory</typeparam>
    /// <typeparam name="TVarType">Bool,Byte,Int,DInt,Word,DWord,Real</typeparam>
    public interface IComPlcWorker<TPlcDataType, TVarType>
    {
        #region 属性
        /// <summary>
        /// 通讯变量列表
        /// </summary>
        ObservableCollection<ModelComPLC> CommList { get; set; }
        #endregion

        #region 事件
        ///// <summary>
        ///// 通讯变量值发生变化,数据变化更新
        ///// </summary>
        //event Action<object, DataTable> DataValue_Changed;
        ///// <summary>
        ///// 报警项发生变化
        ///// </summary>
        //event Action<object, DataRow[]> AlarmNode_Changed;
        /// <summary>
        /// 通讯轮询接收到数据
        /// </summary>
        event Action<object, ObservableCollection<ModelComPLC>> ComData_Received;
        /// <summary>
        /// 通讯启动通知
        /// </summary>
        event Action<object> Com_Started;
        #endregion

        #region 方法
        /// <summary>
        /// 通讯连续采样开启
        /// </summary>
        /// <returns></returns>
        void ComStart();
        /// <summary>
        /// 通讯采样停止
        /// </summary>
        /// <returns></returns>
        void ComStop();
        /// <summary>
        /// 从PLC读取单个变量
        /// 通过 LastErrorCode or LastErrorString判别成功与否
        /// </summary>
        /// <param name="variable">ex: "IX0.0", "MX10.0", "MB20", "T45", etc.</param>
        /// <param name="Format"></param>
        /// <returns></returns>
        object Read(string variable, DataFormat Format = DataFormat.Default);
        /// <summary>
        /// 请求PLC格式化数据
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="numOfBytes">变量字节数</param>
        /// <param name="startByteAdr"></param>
        /// <param name="varType"></param>
        /// <param name="varCount"></param>
        /// <param name="bitAdr"></param>
        /// <returns>按数据类型返回</returns>
        object Read(TPlcDataType dataType, int numOfBytes, int startByteAddr, TVarType varType, int varCount, byte bitAdr = 0);
        /// <summary>
        /// 请求PLC字节序数据
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="numOfBytes">变量字节数</param>
        /// <param name="startAddr"></param>
        /// <param name="Count"></param>
        /// <returns>返回字节数组</returns>
        byte[] ReadBytes(TPlcDataType dataType, int numOfBytes, int startAddr, int count);
        /// <summary>
        /// 写入PLC单个变量
        /// </summary>
        /// <param name="variable">>ex: "MX10.0", "MW20", etc.</param>
        /// <param name="value"></param>
        /// <param name="Format"></param>
        /// <returns></returns>
        ErrorCode Write(string variable, object value, DataFormat Format = DataFormat.Default);
        /// <summary>
        /// 写入PLC格式化数据
        /// 通过 LastErrorCode or LastErrorString判别成功与否
        /// </summary>
        /// <param name="dataType"> Input,Output,Memory</param>
        /// <param name="numOfBytes"> 变量字节数 </param>
        /// <param name="startAddr"> PLC变量起始地址 </param>
        /// <param name="value"> 值 </param>
        /// <param name="bitAdr"> (0-7)</param>
        /// <returns></returns>
        ErrorCode Write(TPlcDataType dataType, int numOfBytes, int startAddr, TVarType varType, object value, int bitAdr = -1);
        /// <summary>
        /// 写入PLC字节序数据
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="numOfBytes">变量字节数</param>
        /// <param name="startAddr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ErrorCode WriteBytes(TPlcDataType dataType, int numOfBytes, int startAddr, byte[] value);
        /// <summary>
        /// 写入PLC位
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="numOfBytes"></param>
        /// <param name="startAddr"></param>
        /// <param name="bitAddr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ErrorCode WriteBit(TPlcDataType dataType, int numOfBytes, int startAddr, int bitAddr, object value);
        #endregion
    }
}
