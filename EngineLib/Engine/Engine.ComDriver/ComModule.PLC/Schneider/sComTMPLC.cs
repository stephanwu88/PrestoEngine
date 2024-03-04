using Engine.Common;
using Engine.ComDriver.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.ObjectModel;
using PlcTypes = Engine.ComDriver.Types;

namespace Engine.ComDriver.Schneider
{
    /// <summary>
    /// 施耐德TM系列PLC网络通讯
    /// </summary>
    public class sComTMPLC : sComNetDevice, IComDriverNetCom
    {
        #region 内部变量
        private PlcDataSet _DataSet;
        #endregion

        #region 属性
        private ObservableCollection<ModelComPLC> _CommList;
        public ObservableCollection<ModelComPLC> CommList { get=> _CommList; set { _CommList = value; } }
        /// <summary>
        /// CPU类型
        /// </summary>
        public CpuType CPU { get; set; }
        /// <summary>
        /// PLC数据集
        /// </summary>
        public PlcDataSet DataSet { get => _DataSet; set { _DataSet = value; RaisePropertyChanged("DataSet"); } }
        #endregion

        #region 事件
        /// <summary>
        /// 通讯轮询接收到数据
        /// </summary>
        public event Action<object, ObservableCollection<ModelComPLC>> ComData_Received;
        /// <summary>
        /// 通讯启动通知
        /// </summary>
        public event Action<object> Com_Started;
        #endregion

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public sComTMPLC()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CpuType"></param>
        /// <param name="ComIP"></param>
        public sComTMPLC(CpuType eCpuType, string ComIP)
        {
            _DriverItem.CpuType = eCpuType.ToString();
            _DriverItem.ComParam.ComIP = ComIP;
            _DriverItem.ComParam.ComPort = 502;
            _DriverItem.ComParam.CycleTime = 100;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Drv"></param>
        public sComTMPLC(DriverItem<NetworkCommParam> Drv)
        {
            if (!Config(Drv))
                return;
            if (ValidateCpuType(_DriverItem.DriverPtl) != ErrorCode.NoError)
                return;
        }
        #endregion

        #region 接口实现
        /// <summary>
        /// 通讯连续采样开启
        /// </summary>
        /// <returns></returns>
        public void ComStart()
        {
            if (ComWorking)
                return;
            ComWorking = true;
            AutoSetDataComRange();
            Open();
            ThreadPool.QueueUserWorkItem(Queue_Com, _DriverItem);
            if (Com_Started != null)
                Com_Started(this);
            EventRise_ComNote(EventType.ComReadStarted, string.Format("PLC({0})已启动周期循环", _DriverItem.ComParam.ComIP));
        }
        /// <summary>
        /// 通讯
        /// </summary>
        /// <param name="state"></param>
        private void Queue_Com(object state)
        {
            while (ComWorking)
            {
                try
                {
                    byte[] byBuffer = ReadBytes(DataType.Memory,0, 0, 0);

                    EventRise_Error(Error.VarListOutOfRange,string.Format(""));
                }
                catch (Exception err)
                {
                    EventRise_Error(Error.ProgError, err.Message);
                }
                Thread.Sleep(DriverItem.ComParam.CycleTime);
            }
        }
        /// <summary>
        /// 通讯采样停止
        /// </summary>
        /// <returns></returns>
        public void ComStop()
        {
            if(ComWorking)
                EventRise_ComNote(EventType.ComReadStoped, string.Format("PLC({0})已停止周期循环",DriverItem.ComParam.ComIP));
            ComWorking = false;
            Close();
        }

        /// <summary>
        /// 从PLC读取单个变量
        /// 通过 LastErrorCode or LastErrorString判别成功与否
        /// </summary>
        /// <param name="variable">ex: "IX0.0", "MX10.0", "MB20", "T45", etc.</param>
        /// <param name="Format"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object Read(string variable, DataFormat Format = DataFormat.Default)
        {
            DataType mDataType;
            VarType mVarType;
            int mVarNumOfByte = variable.ToVarNumOfBytes();
            variable = variable.ToStandardVariable();

            try
            {
                mDataType = variable.ToDataType() ?? default(DataType);
                mVarType = variable.ToVarType(Format) ?? default(VarType);
                int mStartAddr = variable.Substring(2).MidString("", ".").ToMyInt();
                int mBit = variable.MidString(".","").ToMyInt();
                if (variable.Length < 3 || mStartAddr == -1)
                    goto Error;
                if (mVarType == VarType.BYTE)
                {
                    byte dRet = (byte)Read(mDataType, mVarNumOfByte, mStartAddr, mVarType, 1);
                    return dRet;
                }
                else if (mVarType == VarType.INT)
                {
                    Int16 dRet = (Int16)Read(mDataType, mVarNumOfByte, mStartAddr, mVarType, 1);
                    return dRet;
                }
                else if (mVarType == VarType.WORD)
                {
                    UInt16 dRet = (UInt16)Read(mDataType, mVarNumOfByte, mStartAddr, mVarType, 1);
                    return dRet;
                }
                else if (mVarType == VarType.DINT)
                {
                    Int32 dRet = (Int32)Read(mDataType, mVarNumOfByte, mStartAddr, mVarType, 1);
                    return dRet;
                }
                else if (mVarType == VarType.DWORD)
                {
                    UInt32 dRet = (UInt32)Read(mDataType, mVarNumOfByte, mStartAddr, mVarType, 1);
                    return dRet;
                }
                else if (mVarType == VarType.REAL)
                {
                    double dRet = (double)Read(mDataType, mVarNumOfByte, mStartAddr, mVarType, 1);
                    return dRet;
                }
                else if (mVarType == VarType.BOOL)
                {
                    if (mBit > 7) throw new Exception();
                    var obj3 = (byte)Read(mDataType, mVarNumOfByte, mStartAddr, VarType.BYTE, 1);
                    BitArray objBoolArray = new BitArray(new byte[] { obj3 });
                    return objBoolArray[mBit];
                }
            Error:
                LastErrorCode = ErrorCode.WrongVarFormat;
                LastErrorString = "变量 '" + variable + "' 无法读取,请检查语法并重试!";
                return LastErrorCode;
            }
            catch (Exception err)
            {
                LastErrorCode = ErrorCode.ReadData;
                LastErrorString = err.Message;
                return LastErrorCode;
            }
        }
        /// <summary>
        /// 请求PLC格式化数据
        /// </summary>
        /// <param name="dataType">I Q M 数据类别</param>
        /// <param name="numOfBytes">变量字节数</param>
        /// <param name="startAddr">PLC起始地址</param>
        /// <param name="varType">PLC变量数据类型</param>
        /// <param name="varCount">PLC变量数据个数</param>
        /// <param name="bitAdr">位变量偏移</param>
        /// <returns></returns>
        public object Read(DataType dataType, int numOfBytes, int startAddr, VarType varType, int varCount, byte bitAdr = 0)
        {
            byte[] bytes = ReadBytes(dataType, numOfBytes, startAddr, varCount);
            return bytes.ParseBytes(varType, varCount, bitAdr);
        }
        /// <summary>
        /// 请求PLC字节序数据
        /// </summary>
        /// <param name="dataType">I Q M 数据类别</param>
        /// <param name="numOfBytes">变量字节数</param>
        /// <param name="startAddr">PLC起始地址</param>
        /// <param name="count">PLC读取变量个数</param>
        /// <returns>返回字节数组</returns>
        public byte[] ReadBytes(DataType dataType, int numOfBytes, int startAddr, int count)
        {
            int readBytes = numOfBytes * count;
            List<byte> resultBytes = new List<byte>();
            int indexVarID = startAddr; 

            if (mClient == null || !mClient.Connected)
            {
                LastErrorCode = ErrorCode.ConnectionError;
                return resultBytes.ToArray();
            }
            else if (startAddr < 0 || startAddr > 65535 || readBytes == 0)
            {
                LastErrorCode = ErrorCode.WrongVarFormat;
                return resultBytes.ToArray();
            }
            while (readBytes > 0)
            {
                var maxToRead = (int)Math.Min(readBytes, 200);
                //获取起始Modbus数据域地址
                int regCnt = maxToRead / numOfBytes;
                int modbusDUStart = Conversion.ToDataUnitStartAddr(dataType, numOfBytes, indexVarID);
                int modbusDULen = Conversion.ToDataUnitLength(dataType, numOfBytes, regCnt);
                List<byte> bytes = ReadBytesWithASingleRequest(dataType, modbusDUStart, modbusDULen).ToList();
                if (bytes == null)
                    return resultBytes.ToArray();
                //添加Modbus奇数字节为起始地址的 【字形式读取，起始地址位奇数字节形式访问，例如MB1】回码数据修正
                //Modbus只能以16位数据（高字节在前，低字节在后）的形式回码，访问MB奇数字节开始，必须多读取一个字节，确保偶数（小1个ID）开始读取
                //修正方法：基于正序后的回码去除首个字节
                if (numOfBytes <= 1 && dataType == DataType.Memory && indexVarID % 2 == 1)
                    bytes.RemoveAt(0);
                resultBytes.AddRange(bytes);
                readBytes -= maxToRead;
                indexVarID += regCnt;
            }
            return resultBytes.ToArray();
        }

        /// <summary>
        /// 写入PLC单个变量
        /// </summary>
        /// <param name="variable">>ex: "MX10.0", "MW20", etc.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ErrorCode Write(string variable, object value, DataFormat Format = DataFormat.Default)
        {
            DataType mDataType;
            value = Conversion.ToCSharpTypeValue(variable, value, Format);
            if (value == null)
                throw new ArgumentException("不正确的变量设定值,写入时转化失败");
            int mVarNumOfByte = variable.ToVarNumOfBytes();
            variable = variable.ToStandardVariable();
            try
            {
                mDataType = variable.ToDataType() ?? default(DataType);
                int mStartAddr = variable.Substring(2).MidString("", ".").ToMyInt();
                string bitAddr = variable.MidString(".", "");
                int mBit = -1;
                if (bitAddr.Length > 0)
                    int.TryParse(bitAddr, out mBit);
                if (variable.Length < 3 || mStartAddr == -1)
                    throw new ArgumentException(string.Format("写入变量格式错误:{0}", variable));
                VarType mVarType = variable.ToVarType(Format) ?? default(VarType);
                ErrorCode err = Write(mDataType, mVarNumOfByte, mStartAddr, mVarType, value, mBit);
                return err;
            }
            catch (Exception err)
            {
                LastErrorCode = ErrorCode.WriteData;
                return LastErrorCode;
            }
        }
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
        public ErrorCode Write(DataType dataType, int numOfBytes, int startAddr, VarType varType, object value, int bitAdr = -1)
        {
            byte[] package = default(byte[]);
            switch (varType)
            {
                case VarType.BOOL:
                    if (bitAdr == -1)
                        throw new ArgumentException("位变量写入值指定错误", nameof(value));
                    bool bitValue = false;
                    if (value is bool)
                    {
                        bitValue = (bool)value;
                    }
                    else if (value is int)
                    {
                        var intValue = (int)value;
                        if ((numOfBytes <= 1 && (intValue < 0 || intValue > 7)) ||
                            (numOfBytes == 2 && (intValue < 0 || intValue > 15)) ||
                            numOfBytes > 2)
                            throw new Exception(string.Format("位寻址范围访问第{0}位无效", bitAdr));
                        bitValue = intValue == 1;
                    }
                    return WriteBit(dataType, numOfBytes, startAddr, bitAdr, bitValue);
                case VarType.BYTE:
                    package = PlcTypes.Byte.ToByteArray((byte)value);
                    return WriteSingleMB(dataType, startAddr, package);
                case VarType.INT:
                    package = PlcTypes.Int.ToByteArray((Int16)value, ByteOrder16.AB);
                    break;
                case VarType.WORD:
                    package = PlcTypes.Word.ToByteArray((UInt16)value, ByteOrder16.AB);
                    break;
                case VarType.DINT:
                    package = PlcTypes.DInt.ToByteArray((Int32)value, ByteOrder32.CDAB);
                    break;
                case VarType.DWORD:
                    package = PlcTypes.DWord.ToByteArray((UInt32)value, ByteOrder32.CDAB);
                    break;
                case VarType.REAL:
                    if (dataType == DataType.Memory)
                        package = PlcTypes.Real.ToByteArray((double)value, ByteOrder32.CDAB);
                    else if (dataType == DataType.Input || dataType == DataType.Output)
                        package = PlcTypes.Real.ToByteArray((double)value, ByteOrder32.DCBA);
                    break;
                case VarType.STRING:
                    package = PlcTypes.String.ToByteArray(value as string);
                    break;
                default:
                    return ErrorCode.WrongVarFormat;
            }
            return WriteBytes(dataType, numOfBytes, startAddr, package);
        }
        /// <summary>
        /// 写入PLC字节序数据
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="numOfBytes">变量字节数</param>
        /// <param name="startAddr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ErrorCode WriteBytes(DataType dataType, int numOfBytes, int startAddr, byte[] value)
        {
            int localIndex = 0;
            int count = value.Length;
            int readBytes = numOfBytes * count;
            int indexVarID = startAddr; 
            if (mClient == null || !mClient.Connected)
            {
                LastErrorCode = ErrorCode.ConnectionError;
                return LastErrorCode;
            }
            else if (startAddr < 0 || startAddr > 65535 || readBytes == 0)
            {
                LastErrorCode = ErrorCode.WrongVarFormat;
                return LastErrorCode;
            }
            
            var maxToWrite = (int)Math.Min(count, 200);
            //获取起始Modbus数据域地址
            int regCnt = maxToWrite / numOfBytes;
            int modbusDUStart = Conversion.ToDataUnitStartAddr(dataType, numOfBytes, indexVarID);
            int modbusDULen = Conversion.ToDataUnitLength(dataType, numOfBytes, regCnt);
            ErrorCode lastError = WriteBytesWithASingleRequest(dataType, modbusDUStart, value);
            if (lastError != ErrorCode.NoError)
            {
                return lastError;
            }
            count -= maxToWrite;
            localIndex += maxToWrite;
            
            return ErrorCode.NoError;
        }
        /// <summary>
        /// 写入PLC位
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="numOfBytes"></param>
        /// <param name="startAddr"></param>
        /// <param name="bitAddr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ErrorCode WriteBit(DataType dataType, int numOfBytes, int startAddr, int bitAddr, object value)
        {
            bool bitVal = false;
            if (value is int)
                bitVal = (int)value == 1;
            else if (value is byte)
                bitVal = (byte)value == 1;
            else if (value is bool)
                bitVal = (bool)value;
            return WriteBitWithASingleRequest(dataType, numOfBytes, startAddr, bitAddr, bitVal);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 设定通讯分区边界
        /// </summary>
        private void AutoSetDataComRange()
        {

        }

        /// <summary>
        /// 验证CPU信息
        /// </summary>
        /// <param name="cpuType"></param>
        /// <returns></returns>
        private ErrorCode ValidateCpuType(string cpuType)
        {
            try
            {
                CPU = (CpuType)Enum.Parse(typeof(CpuType), cpuType);
            }
            catch (Exception )
            {
                LastErrorCode = ErrorCode.WrongCPU_Type;
                LastErrorString = "CPU类型设定错误";
                return LastErrorCode;
            }
            return ErrorCode.NoError;
        }

        /// <summary>
        /// 发送报文头8字节
        /// </summary>
        /// <param name="byDuCount"></param>
        /// <returns></returns>
        private byte[] ReadHeaderPackage(byte byFuncCode, int DuCount = 6)
        {
            List<byte> package = new List<byte>();
            //事务处理标识 这两个字节用户指定
            package.AddRange(new byte[] { 0xFF, 0x01 });
            //ModbusTCP协议标识符
            package.AddRange(new byte[] { 0x00, 0x00 });
            //数据长度
            byte[] byDuCount = BitConverter.GetBytes((ushort)DuCount);
            package.AddRange(new byte[] { byDuCount[1], byDuCount[0] });
            //从站站号
            package.Add(DriverItem.ComParam.StationAddr);
            //功能码
            package.Add(byFuncCode);
            return package.ToArray();
        }
        /// <summary>
        /// 读数据请求报文
        /// </summary>
        /// <param name="dataType">I Q M 数据类别</param>
        /// <param name="numOfBytes">变量字节数</param>
        /// <param name="modbusDUAddr">数据域起始地址</param>
        /// <param name="modbusDULen">数据域数据长度</param>
        /// <returns></returns>
        private byte[] ReadBytesWithASingleRequest(DataType dataType, int modbusDUAddr, int modbusDULen)
        {
            int dataByteLen;
            byte[] OrderBytes;
            try
            {
                List<byte> package = new List<byte>();
                byte[] byStartDU = BitConverter.GetBytes((ushort)modbusDUAddr);
                byte[] byDuCount = BitConverter.GetBytes((ushort)modbusDULen);
                package.AddRange(ReadHeaderPackage(dataType.ToReadFunCode()));
                package.AddRange(new byte[] { byStartDU[1], byStartDU[0] });
                package.AddRange(new byte[] { byDuCount[1], byDuCount[0] });
                mClient.Send(package.ToArray(), package.Count, SocketFlags.None);
                byte[] byReceived = new byte[512];
                int receivedCount = mClient.Receive(byReceived, 512, SocketFlags.None);
                //byReceived[8] : 真实数据的字节流数据总数
                if (receivedCount < 9)
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                if (byReceived[0] != 0xFF || byReceived[1] != 0x01)
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                dataByteLen = receivedCount - 9;
                // Modbus读取寄存器顺序为BA逆序，调整为AB正序，其他字节顺序保持正序
                if (byReceived[7] == 0x03 || byReceived[7] == 0x04)
                {
                    dataByteLen = ((dataByteLen + 1) / 2) * 2;    //圆整为偶数个
                    OrderBytes = new byte[dataByteLen];
                    for (int i = 0; i < byReceived[8]; i = i + 2)
                    {
                        OrderBytes[i] = byReceived[i + 9 + 1];
                        OrderBytes[i + 1] = byReceived[i + 9];
                    }
                }
                else
                {
                    OrderBytes = new byte[dataByteLen];
                    for (int i = 0; i < byReceived[8]; i++)
                        OrderBytes[i] = byReceived[i + 9];
                }
                return OrderBytes;
            }
            catch (SocketException socketException)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = socketException.Message;
                return null;
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = exc.Message;
                return null;
            }
        }
        /// <summary>
        /// 写入字节
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="modbusDUAddr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private ErrorCode WriteBytesWithASingleRequest(DataType dataType, int modbusDUAddr, byte[] value)
        {
            byte byFunCode = dataType.ToWriteFunCode();
            if (dataType == DataType.Input || value.Length == 0)
                throw new ArgumentException("写入变量类型错误或写入值为空");
            if ((byFunCode == 0x06 || byFunCode == 0x10) && (value.Length % 2 == 1))
                throw new ArgumentException("写入数据长度错误");
            try
            {
                List<byte> package = new List<byte>();
                int WriteDataQuantity = value.Length;                   //写入数据域笔数
                int WriteByteCount = value.Length;                      //实际写入的字节个数
                if (dataType == DataType.Output)
                    WriteDataQuantity *= 8;                             //写入位个数
                else if (dataType == DataType.Memory)
                {
                    WriteDataQuantity = (WriteDataQuantity + 1) / 2;    //写入寄存器笔数
                    WriteByteCount = WriteDataQuantity * 2;
                }
                byte[] byStartDU = BitConverter.GetBytes((ushort)modbusDUAddr);
                byte[] byDataQuantity = BitConverter.GetBytes((ushort)WriteDataQuantity);
                package.AddRange(ReadHeaderPackage(byFunCode, value.Length + 7));
                package.AddRange(new byte[] { byStartDU[1], byStartDU[0] });
                package.AddRange(new byte[] { byDataQuantity[1], byDataQuantity[0] });
                package.Add((byte)WriteByteCount);
                package.AddRange(value.ToList());

                mClient.Send(package.ToArray(), package.Count, SocketFlags.None);
                byte[] byReceived = new byte[512];
                int receivedCount = mClient.Receive(byReceived, 512, SocketFlags.None);
                if (receivedCount < 9)
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                if (byReceived[0] != 0xFF || byReceived[1] != 0x01)
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                if (byReceived[7] == 0x80 + package[7])
                {
                    LastErrorCode = ErrorCode.WriteData;
                    return LastErrorCode;
                }
                return ErrorCode.NoError;
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = exc.Message;
                return LastErrorCode;
            }
        }
        /// <summary>
        /// PLC位变量写入值 MX0.0 QX0.0 MB0.0 MW0.0~15
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="startAddr"></param>
        /// <param name="bitAddr"></param>
        /// <param name="bitValue"></param>
        /// <returns></returns>
        private ErrorCode WriteBitWithASingleRequest(DataType dataType, int numOfBytes, int startAddr, int bitAddr, bool bitValue)
        {
            if (dataType == DataType.Input)
            {
                LastErrorCode = ErrorCode.WriteData;
                return LastErrorCode;
            }
            try
            {
                List<byte> package = new List<byte>();
                byte FunCode = dataType == DataType.Output ? (byte)0x05 : (byte)0x06;
                package.AddRange(ReadHeaderPackage(FunCode, 6));
                if (dataType == DataType.Output)
                {
                    if (numOfBytes <= 1)
                    {
                        object objInit = Read("QB" + startAddr);
                        if (objInit is byte)
                        {
                            byte byWrite = ((byte)objInit).SetBit(bitAddr, bitValue);
                            return Write("QB" + startAddr, byWrite);
                        }
                        return ErrorCode.WriteData;
                    }
                    else
                    {
                        int modbusDUStart = Conversion.ToDataUnitStartAddr(dataType, numOfBytes, startAddr);
                        object objInit = Read("QW" + modbusDUStart, DataFormat.UnSignedDecimal);
                        if (objInit is UInt16)
                        {
                            UInt16 uiWrite = ((UInt16)objInit).SetBit(bitAddr, bitValue);
                            return Write("QW" + modbusDUStart, uiWrite);
                        }
                        return ErrorCode.WriteData;
                    }
                }
                else
                {
                    int modbusDUStart;
                    if (numOfBytes == 2)
                        modbusDUStart = startAddr;
                    else
                        modbusDUStart = Conversion.ToDataUnitStartAddr(dataType, numOfBytes, startAddr);
                    object objInit = Read("MW" + modbusDUStart, DataFormat.UnSignedDecimal);
                    if (objInit is UInt16)
                    {
                        UInt16 uiWrite = ((UInt16)objInit).SetBit(bitAddr, bitValue);
                        return Write("MW" + modbusDUStart, uiWrite);
                    }
                    return ErrorCode.WriteData;
                }
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = exc.Message;
                return LastErrorCode;
            }
        }
        /// <summary>
        /// 单独增加其他厂家不具备的MB写入操作
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="startAddr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private ErrorCode WriteSingleMB(DataType dataType, int startAddr, byte[] value)
        {
            int modbusDUAddr = Conversion.ToDataUnitStartAddr(dataType, 1, startAddr);
            if (dataType == DataType.Memory && value.Length == 1)
            {
                object objInit = Read("MW" + modbusDUAddr, DataFormat.UnSignedDecimal);
                if (objInit is UInt16)
                {
                    UInt16 uiWrite;
                    if (startAddr % 2 == 0)
                        uiWrite = (UInt16)(((UInt16)objInit & 0xFF00) | (UInt16)value[0]);
                    else
                        uiWrite = (UInt16)(((UInt16)objInit & 0x00FF) | value[0] << 8);
                    return Write("MW" + modbusDUAddr, uiWrite);
                }
                return ErrorCode.WriteData;
            }
            return ErrorCode.WriteData;
        }
        #endregion
    }
}
