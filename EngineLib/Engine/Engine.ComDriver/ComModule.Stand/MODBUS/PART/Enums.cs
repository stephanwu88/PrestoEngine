namespace Engine.ComDriver.MODBUS
{
    /// <summary>
    /// Modbus寄存器类型
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 线圈，0x,寄存器范围00001~09999
        /// </summary>
        Coil = 0x01,
        /// <summary>
        /// 输入，1x,寄存器范围10001~19999
        /// </summary>
        Input = 0x02,
        /// <summary>
        /// 输入寄存器，3x,寄存器范围30001~39999
        /// </summary>
        InputRegister = 0x04,
        /// <summary>
        /// 保持型寄存器，4x,寄存器范围40001~49999
        /// </summary>
        HoldingRegister = 0x03,
    }

    /// <summary>
    /// 变量类型
    /// </summary>
    public enum VarType
    {
        /// <summary>
        /// 布尔型
        /// </summary>
        Bool,
        /// <summary>
        /// 字节型
        /// </summary>
        Byte,
        /// <summary>
        /// 浮点型
        /// </summary>
        Real,
        /// <summary>
        /// 整型
        /// </summary>
        Int,
        /// <summary>
        /// 4字节整型
        /// </summary>
        DInt,
        /// <summary>
        /// 字符串型
        /// </summary>
        String
    }
}
