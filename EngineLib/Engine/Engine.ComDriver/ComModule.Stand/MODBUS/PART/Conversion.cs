using Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.ComDriver.MODBUS
{
    public static partial class Conversion
    {
        //0x01：读线圈
        //0x05：写单个线圈
        //0x0F：写多个线圈
        //0x02：读离散量输入
        //0x04：读输入寄存器
        //0x03：读保持寄存器
        //0x06：写单个保持寄存器
        //0x10：写多个保持寄存器
        public static byte ModbusFuncCode(this DataType dataType)
        {
            byte byFunc = 0x00;
            switch (dataType)
            {
                case DataType.Coil:
                    byFunc = 0x0F;
                    break;
                case DataType.HoldingRegister:
                    byFunc = 0x10;
                    break;
            }
            return byFunc;
        }
    }

    /// <summary>
    /// PLC数据类型转换
    /// </summary>
    public static partial class Conversion
    {
        public static int BytesToInt(byte[] byData)
        {
            return 0;
        }
    }
}
