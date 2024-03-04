
namespace Engine.ComDriver
{
    /// <summary>
    /// 32位数据字节序
    /// </summary>
    public enum ByteOrder32
    {
        /// <summary>
        /// 西门子字节序
        /// </summary>
        ABCD,
        /// <summary>
        /// 施耐德字节序
        /// 计算机字节序
        /// </summary>
        DCBA,
        /// <summary>
        /// Modbus字节序
        /// </summary>
        BADC,   
        CDAB
    }

    /// <summary>
    /// 16位数据字节序
    /// </summary>
    public enum ByteOrder16
    {
        /// <summary>
        /// 正序
        /// </summary>
        AB,
        /// <summary>
        /// 逆序
        /// </summary>
        BA   
    }

    /// <summary>
    /// 数据格式
    /// </summary>
    public enum DataFormat
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,
        /// <summary>
        /// 默认无符号整数
        /// UnsignedDecimal
        /// </summary>
        UnSignedDecimal,
        /// <summary>
        /// 有符号
        /// </summary>
        SignedDecimal,
        /// <summary>
        /// 浮点
        /// </summary>
        Float
    }
}
