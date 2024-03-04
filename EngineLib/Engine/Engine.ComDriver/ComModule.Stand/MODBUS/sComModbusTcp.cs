using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.ComDriver.MODBUS
{
    #region 报文结构
    //事务处理标识：2字节，报文的序列号，一般每次通信之后就要加1以区别不同的通信数据报文
    //协议标识符：  2字节，00 00表示ModbusTCP协议
    //长度：        2字节，表示接下来的数据长度，单位为字节
    //单元标识符 ： 1字节，可以理解为设备地址
    //------------------------------------------
    //---------------------功能码---------------
    //0x01：读线圈
    //0x05：写单个线圈
    //0x0F：写多个线圈
    //0x02：读离散量输入
    //0x04：读输入寄存器
    //0x03：读保持寄存器
    //0x06：写单个保持寄存器
    //0x10：写多个保持寄存器
    #endregion

    //public class sComModbusTcpClient : sComNetDevice, IComPlcData<DataType,VarType>
    public class sComModbusTcpClient : sComNetDevice
    { 
        public sComModbusTcpClient(string Ip,int port = 502)
        {
            DriverItem.ComParam.ComIP = Ip;
            DriverItem.ComParam.ComPort = port;
        }

        #region 公共方法

        /// <summary>
        /// 请求PLC数据
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="startAddr"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public byte[] ReadBytes(DataType dataType, int startAddr, int Count)
        {
            List<byte> resultBytes = new List<byte>();
            int index = startAddr;
            if (mClient == null || !mClient.Connected)
            {
                LastErrorCode = ErrorCode.ConnectionError;
                return resultBytes.ToArray();
            }
            else if (startAddr < 0 || startAddr > 65535 || Count == 0)
            {
                LastErrorCode = ErrorCode.WrongVarFormat;
                return resultBytes.ToArray();
            }
            int registerCount = (Count + 1) / 2;
            while (registerCount > 0)
            {
                var maxToRead = (int)Math.Min(registerCount, 200);
                byte[] bytes = ReadBytesWithASingleRequest(dataType, index, maxToRead);
                if (bytes == null)
                    return resultBytes.ToArray();
                resultBytes.AddRange(bytes);
                registerCount -= maxToRead;
                index += maxToRead;
            }
            return resultBytes.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="StartAddr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ErrorCode WriteBytes(DataType dataType, int StartAddr, byte[] value)
        {
            return WriteBytesWithASingleRequest(dataType, StartAddr, value);
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 发送报文头8字节
        /// </summary>
        /// <param name="byDuCount"></param>
        /// <returns></returns>
        private byte[] ReadHeaderPackage(byte byFuncCode, byte byDuCount = 0x06)
        {
            List<byte> package = new List<byte>();
            //事务处理标识 0x0F 0x01
            package.AddRange(new byte[] { 0xFF, 0x01 });
            //ModbusTCP协议标识符
            package.AddRange(new byte[] { 0x00, 0x00 });
            //数据长度
            package.AddRange(new byte[] { 0x00, byDuCount });
            //从站站号
            package.Add(DriverItem.ComParam.StationAddr);
            //功能码
            package.Add(byFuncCode);
            return package.ToArray();
        }

        /// <summary>
        /// 读数据请求报文
        /// </summary>
        /// <param name="byFuncCode">功能码</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="byCount">数据笔数</param>
        /// <returns></returns>
        private byte[] ReadBytesWithASingleRequest(DataType dataType, int startAddr, int Count)
        {
            byte[] bytes = new byte[Count];
            try
            {
                List<byte> package = new List<byte>();
                byte[] byStartDU = BitConverter.GetBytes((ushort)startAddr);
                byte[] byDuCount = BitConverter.GetBytes((ushort)Count);
                package.AddRange(ReadHeaderPackage((byte)dataType));
                package.AddRange(new byte[] { byStartDU[1], byStartDU[0] });
                package.AddRange(new byte[] { byDuCount[1], byDuCount[0] });
                mClient.Send(package.ToArray(),package.Count,SocketFlags.None);
                byte[] byReceived = new byte[512];
                int receivedCount = mClient.Receive(byReceived, 512, SocketFlags.None);
                //byReceived[8] : 真实数据的字节流数据总数
                if (receivedCount < 9)
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                if (byReceived[0] != 0xFF || byReceived[1] != 0x01 || byReceived[8] != Count * 2)
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                for (int i = 0; i < byReceived[8]; i = i + 2)
                {
                    bytes[i] = byReceived[i + 9 + 1];
                    bytes[i+1] = byReceived[i + 9];
                }
                return bytes;
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
        /// <param name="StartAddr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private ErrorCode WriteBytesWithASingleRequest(DataType dataType, int StartAddr,byte[] value)
        {
            try
            {
                List<byte> package = new List<byte>();
                byte registerCount = (byte)((value.Length + 1) / 2);    //需要写入的寄存器个数
                byte writeCount = (byte)(registerCount * 2);            //实际写入的字节个数

                byte[] byStartDU = BitConverter.GetBytes((ushort)StartAddr);
                byte[] byDuCount = BitConverter.GetBytes((ushort)registerCount);
                
                package.AddRange(ReadHeaderPackage(dataType.ModbusFuncCode(), (byte)(7 + writeCount)));
                package.AddRange(new byte[] { byStartDU[1], byStartDU[0] });
                package.AddRange(new byte[] { byDuCount[1], byDuCount[0] });
                package.Add(writeCount);

                for (int i = 0; i < writeCount - 2; i++)
                    package.Add(value[i]);
                
                //最后两个元素[最后的一个寄存器]的处理
                if (value.Length % 2 == 1)
                {
                    //若是奇数个，需要将最后一个寄存器的高位设置为0
                    package.AddRange(new byte[] { 0x00, value[value.Length - 1] });
                }
                else
                {
                    //若是偶数个，则一一对应
                    package[13 + writeCount - 2] = value[value.Length - 2];
                    package[13 + writeCount - 1] = value[value.Length - 1];
                }
                mClient.Send(package.ToArray(), package.Count,SocketFlags.None);
                byte[] byReceived = new byte[512];
                int receivedCount = mClient.Receive(byReceived, 512, SocketFlags.None);
                if (receivedCount < 9)
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                if (byReceived[0] != 0xFF || byReceived[1] != 0x01)
                    throw new Exception(ErrorCode.WrongNumberReceivedBytes.ToString());
                return ErrorCode.NoError;
            }
            catch (Exception exc)
            {
                LastErrorCode = ErrorCode.WriteData;
                LastErrorString = exc.Message;
                return LastErrorCode;
            }
        }

        #endregion
    }
}
