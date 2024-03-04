using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Engine.ComDriver.Schneider
{
    /// <summary>
    /// TM系列CPU类型
    /// </summary>
    public enum CpuType
    {
        NONE = 0,
        TM218 = 1,
        TM241 = 10,
        TM238 = 20,
        TM258 = 30,
        LMC058 = 40,
        LMC078 = 50,
    }

    /// <summary>
    /// 数据寄存器类型
    /// </summary>
    public enum DataType
    {
        #region 数据功能码
        /*
        | 代码 | 中文名称         | 英文名                   | 位操作/字操作 | 操作数量   | Modbus地址    | 施耐德地址  |
        | ---- | ---------------- | ------------------------ | ------------- | ---------- | ------------- | ----------- |
        | 0x01 | 读线圈状态       | READ  COIL STATUS        | 位操作        | 单个或多个 | 00001-09999位 | QX QB QW QD |
        | 0x02 | 读离散输入状态   | READ  INPUT STATUS       | 位操作        | 单个或多个 | 10001-19999位 | IX IB IW ID |
        | 0x03 | 读保持寄存器     | READ  HOLDING REGISTER   | 字操作        | 单个或多个 | 40001-49999字 | MX MB MW MD |
        | 0x04 | 读输入寄存器     | READ  INPUT REGISTER     | 字操作        | 单个或多个 | 30000-39999字 | IW          |
        | 0x05 | 写线圈状态       | WRITE  SINGLE COIL       | 位操作        | 单个       | 00001-09999位 | QX          |
        | 0x06 | 写单个保持寄存器 | WRITE  SINGLE REGISTER   | 字操作        | 单个       | 40001-49999字 | MX MB MW    |
        | 0x0F | 写多个线圈       | WRITE  MULTIPLE COIL     | 位操作        | 多个       | 00001-09999位 | QX QB QW QD |
        | 0x10 | 写多个保持寄存器 | WRITE  MULTIPLE REGISTER | 字操作        | 多个       | 40001-49999字 | MX MB MW MD |
        */
        #endregion

        /// <summary>
        /// 输入寄存器
        /// </summary>
        [FunCode(0x02 ,0x00)]
        Input,

        /// <summary>
        /// 输出寄存器
        /// </summary>
        [FunCode(0x01, 0x0F)]
        Output,

        /// <summary>
        /// 装载寄存器
        /// </summary>
        [FunCode(0x03, 0x10)]
        Memory,
    }

    /// <summary>
    /// PLC变量数据类型
    /// </summary>
    public enum VarType
    {
        /// <summary>
        /// bool
        /// </summary>
        BOOL,

        /// <summary>
        /// Byte (8 bits)
        /// </summary>
        BYTE,

        /// <summary>
        /// Word (16 bits, 2 bytes)
        /// </summary>
        WORD,

        /// <summary>
        /// DWord(32 bits, 4 bytes)
        /// </summary>
        DWORD,

        /// <summary>
        /// Int (16 bits, 2 bytes)
        /// </summary>
        INT,

        /// <summary>
        /// DInt(32 bits, 4 bytes)
        /// </summary>
        DINT,

        /// <summary>
        /// Real(32 bits, 4 bytes)
        /// </summary>
        REAL,

        /// <summary>
        /// String (variable)
        /// </summary>
        STRING,
    }
}
