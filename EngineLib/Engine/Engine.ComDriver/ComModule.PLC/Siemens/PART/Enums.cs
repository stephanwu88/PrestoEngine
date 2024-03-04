namespace Engine.ComDriver.Siemens
{
    /// <summary>
    /// S7PLC CPU类型
    /// </summary>
    public enum CpuType
    {
        S7200SMART = 0,
        S7300 = 10,
        S7400 = 20,
        S71200 = 30,
        S71500 = 40,
    }

    /// <summary>
    /// 软元件类型
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// I
        /// </summary>
        Input = 129,
        /// <summary>
        /// Q
        /// </summary>
        Output = 130,
        /// <summary>
        /// M (M0, M0.0, ...)
        /// </summary>
        Memory = 131,
        /// <summary>
        /// DB (DB1, DB2, ...)
        /// </summary>
        DataBlock = 132,
        /// <summary>
        /// Timer (T1, T2, ...)
        /// </summary>
        Timer = 29,
        /// <summary>
        /// Counter  (C1, C2, ...)
        /// </summary>
        Counter = 28
    }

    /// <summary>
    /// 变量类型
    /// </summary>
    public enum VarType
    {
        /// <summary>
        /// S7 bool
        /// </summary>
        Bit,
        /// <summary>
        /// S7 Byte(8 bits)
        /// </summary>
        Byte,
        /// <summary>
        /// 
        /// </summary>
        ByteArray,
        /// <summary>
        /// S7 Word (16 bits, 2 bytes)
        /// </summary>
        Word,
        /// <summary>
        /// 
        /// </summary>
        WordArray,
        /// <summary>
        /// S7 DWord (32 bits, 4 bytes)
        /// </summary>
        DWord,
        /// <summary>
        /// 
        /// </summary>
        DWordArray,
        /// <summary>
        /// S7 Int (16 bits, 2 bytes)
        /// </summary>
        Int,
        /// <summary>
        /// 
        /// </summary>
        IntArray,
        /// <summary>
        /// DInt (32 bits, 4 bytes)
        /// </summary>
        DInt,
        /// <summary>
        /// 
        /// </summary>
        DIntArray,
        /// <summary>
        /// Real (32 bits, 4 bytes)
        /// </summary>
        Real,
        /// <summary>
        /// 
        /// </summary>
        RealArray,
        /// <summary>
        /// String (variable)
        /// </summary>
        String,
        /// <summary>
        /// S7String (variable)
        /// </summary>
        S7String,
        /// <summary>
        /// S7WString (variable)
        /// </summary>
        S7WString,
        /// <summary>
        /// Timer
        /// </summary>
        Timer,
        /// <summary>
        /// Counter
        /// </summary>
        Counter
    }
}
