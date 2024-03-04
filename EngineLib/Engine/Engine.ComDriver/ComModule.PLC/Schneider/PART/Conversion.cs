using Engine.Common;
using System;
using Engine.ComDriver.Types;

namespace Engine.ComDriver.Schneider
{
    /// <summary>
    /// PLC数据类型转换
    /// </summary>
    public static partial class Conversion
    {
        /// <summary>
        /// double to DWord (DBD)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static UInt32 ConvertToUInt(this double input)
        {
            uint output;
            output = DWord.FromByteArray(Types.Real.ToByteArray(input, ByteOrder32.DCBA), ByteOrder32.DCBA);
            return output;
        }

        /// <summary>
        /// DWord (DBD) to double
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double ConvertToDouble(this uint input)
        {
            double output;
            output = Types.Real.FromByteArray(DWord.ToByteArray(input, ByteOrder32.DCBA), ByteOrder32.DCBA);
            return output;
        }
    }

    /// <summary>
    /// 程序内部转换
    /// </summary>
    public static partial class Conversion
    {
        /// <summary>
        /// 变量标准化
        /// </summary>
        /// <param name="strVariable"></param>
        /// <returns></returns>
        public static string ToStandardVariable(this string strVariable)
        {
            return strVariable.ToUpper().Replace(" ", "").Replace("%", "");
        }

        /// <summary>
        /// 获取变量数据类别
        /// </summary>
        /// <param name="strVariable"></param>
        /// <returns>I Q M 数据类别</returns>
        public static DataType? ToDataType(this string strVariable)
        {
            strVariable = strVariable.ToStandardVariable();
            if (strVariable.Length < 2) return null;
            DataType dataType;
            string strDataType = strVariable.Substring(0, 1);
            if (strDataType == "I")
                dataType = DataType.Input;
            else if (strDataType == "Q")
                dataType = DataType.Output;
            else if (strDataType == "M")
                dataType = DataType.Memory;
            else
                return null;
            return dataType;
        }

        /// <summary>
        /// 获取变量数据类型
        /// </summary>
        /// <param name="strVariable"></param>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static VarType? ToVarType(this string strVariable, DataFormat Format = DataFormat.UnSignedDecimal)
        {
            strVariable = strVariable.ToStandardVariable();
            if (strVariable.Length < 2) return null;
            VarType varType;
            string strVarType = strVariable.Substring(1, 1);
            if (strVariable.Contains("."))
                varType = VarType.BOOL;
            else
            {
                if (strVarType == "B")
                    varType = VarType.BYTE;
                else if (strVarType == "W")
                    varType = Format == DataFormat.SignedDecimal ? VarType.INT : VarType.WORD;
                else if (strVarType == "D")
                    varType = Format == DataFormat.Float ? VarType.REAL : (Format == DataFormat.SignedDecimal ? VarType.DINT : VarType.DWORD);
                else if (strVarType == "X")
                    varType = VarType.BOOL;
                else
                    return null;
            }
            return varType;
        }

        /// <summary>
        /// 将变量设定值转化为C#标准变量类型
        /// </summary>
        /// <param name="strVariable"></param>
        /// <param name="value"></param>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static object ToCSharpTypeValue(string strVariable,object value, DataFormat Format = DataFormat.Default)
        {
            if (value == null)
                return null;
            string strVal = value.ToString();
            VarType varType = strVariable.ToVarType(Format) ?? default(VarType);
            if (varType == VarType.BOOL)
            {
                bool bitVal = strVal == "1" || strVal.ToUpper() == "TRUE";
                return bitVal;
            }
            else if (varType == VarType.BYTE)
            {
                byte byVal;
                if (byte.TryParse(strVal, out byVal))
                    return byVal;
            }
            else if (varType == VarType.INT)
            {
                Int16 intVal;
                if (Int16.TryParse(strVal, out intVal))
                    return intVal;
            }
            else if (varType == VarType.WORD)
            {
                UInt16 uintVal;
                if (UInt16.TryParse(strVal, out uintVal))
                    return uintVal;
            }
            else if (varType == VarType.DINT)
            {
                Int32 int32Val;
                if (Int32.TryParse(strVal, out int32Val))
                    return int32Val;
            }
            else if (varType == VarType.DWORD)
            {
                UInt32 uint32Val;
                if (UInt32.TryParse(strVal, out uint32Val))
                    return uint32Val;
            }
            else if (varType == VarType.REAL)
            {
                double dbVal;
                if (double.TryParse(strVal, out dbVal))
                    return dbVal;
            }
            return null;
        }

        /// <summary>
        /// 获取变量本身占用的字节数
        /// 也就是所谓的n字节变量
        /// </summary>
        /// <param name="strVariable"></param>
        /// <returns></returns>
        public static int ToVarNumOfBytes(this string strVariable)
        {
            int NumOfBytes = 0;
            strVariable = strVariable.ToStandardVariable();
            string strVarType = strVariable.Substring(1, 1);
            if (strVarType == "X" || strVarType == "B")
                NumOfBytes = 1;
            else if (strVarType == "W")
                NumOfBytes = 2;
            else if (strVarType == "D")
                NumOfBytes = 4;
            return NumOfBytes;
        }

        /// <summary>
        /// 获取数据域中的起始地址
        /// </summary>
        /// <param name="strVariable"></param>
        /// <returns></returns>
        public static int ToDataUnitStartAddr(this string strVariable)
        {
            int StartAddr = strVariable.Substring(2).MidString("", ".").ToMyInt();
            return ToDataUnitStartAddr(strVariable.ToDataType() ?? default(DataType), strVariable.ToVarNumOfBytes(), StartAddr);
        }

        /// <summary>
        /// 获取数据域中的起始地址
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="NumOfBytes"></param>
        /// <param name="startAddr"></param>
        /// <returns></returns>
        public static int ToDataUnitStartAddr(DataType dataType, int NumOfBytes, int startAddr, int bitAddr = -1)
        {
            int ModbusStartAddr = 0;
            int StartByteID = NumOfBytes <= 1 ? startAddr : startAddr * NumOfBytes;
            if (dataType == DataType.Input || dataType == DataType.Output)
            {
                ModbusStartAddr = bitAddr == -1 ? StartByteID * 8 : StartByteID * 8 + bitAddr;
            }
            else if (dataType == DataType.Memory)
            {
                if (NumOfBytes <= 1)
                    ModbusStartAddr = (StartByteID % 2 == 0) ? (StartByteID + 1) / 2 : ((StartByteID + 1) / 2 - 1);
                else
                    ModbusStartAddr = (StartByteID + 1) / 2;
            }
            return ModbusStartAddr;
        }

        /// <summary>
        /// 获取数据域中的数据长度
        /// </summary>
        /// <param name="strVariable"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static int ToDataUnitLength(string strVariable, int Count = 1)
        {
            if (Count <= 0) Count = 1;
            return ToDataUnitLength(strVariable.ToDataType() ?? default(DataType), strVariable.ToVarNumOfBytes(), Count);
        }

        /// <summary>
        /// 获取数据域中的数据长度
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="NumOfBytes"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static int ToDataUnitLength(DataType dataType, int NumOfBytes, int Count)
        {
            int ModbusDataUnitCnt = 0;
            int ByteDataUnitCnt = NumOfBytes <= 0 ? 1 : NumOfBytes;
            if (Count <= 0) Count = 1;
            ByteDataUnitCnt *= Count;
            if (dataType == DataType.Input || dataType == DataType.Output)
            {
                ModbusDataUnitCnt = ByteDataUnitCnt * 8;
            }
            else if (dataType == DataType.Memory)
            {
                ModbusDataUnitCnt = (ByteDataUnitCnt + 1) / 2;
            }
            return ModbusDataUnitCnt;
        }

        /// <summary>
        /// 获取读取使用的功能码
        /// </summary>
        /// <param name="dataType">I,Q,M变量类别</param>
        /// <returns></returns>
        public static byte ToReadFunCode(this DataType dataType)
        {
            return FunCodeAttribute.ReadCode(dataType);
        }

        /// <summary>
        /// 获取读取使用的功能码
        /// </summary>
        /// <param name="strVariable"></param>
        /// <returns></returns>
        public static byte ToReadFunCode(string strVariable)
        {
            return ToReadFunCode(strVariable.ToDataType() ?? default(DataType));
        }

        /// <summary>
        /// 获取写入使用的功能码
        /// </summary>
        /// <param name="dataType">I,Q,M变量类别</param>
        /// <returns></returns>
        public static byte ToWriteFunCode(this DataType dataType)
        {
            return FunCodeAttribute.WriteCode(dataType);
        }

        /// <summary>
        /// 获取写入使用的功能码
        /// </summary>
        /// <param name="strVariable"></param>
        /// <returns></returns>
        public static byte ToWriteFunCode(string strVariable)
        {
            return ToWriteFunCode(strVariable.ToDataType() ?? default(DataType));
        }

        ///// <summary>
        ///// 获取变量类型的字节长度
        ///// </summary>
        ///// <param name="varType"></param>
        ///// <param name="varCount"></param>
        ///// <returns></returns>
        //public static int ToLengthOfVarType(this VarType varType,DataType dataType, int varCount = 1)
        //{
        //    switch (varType)
        //    {
        //        case VarType.BOOL:
        //            return 8;
        //        case VarType.BYTE:
        //            int iCnt = 0;
        //            if (dataType == DataType.Memory)
        //                iCnt = (varCount + 1) / 2;
        //            else if (dataType == DataType.Input || dataType == DataType.Output)
        //                iCnt = varCount * 8;
        //            return iCnt;
        //        case VarType.STRING:
        //            return varCount;
        //        case VarType.WORD:
        //        case VarType.INT:
        //            iCnt = 0;
        //            if (dataType == DataType.Memory)
        //            {
        //                iCnt = varCount * 2;
        //                iCnt = (iCnt + 1) / 2;
        //            }
        //            else if (dataType == DataType.Input || dataType == DataType.Output)
        //                iCnt = varCount * 16;
        //            return iCnt;
        //        case VarType.DWORD:
        //        case VarType.DINT:
        //        case VarType.REAL:
        //            iCnt = 0;
        //            if (dataType == DataType.Memory)
        //            {
        //                iCnt = varCount * 4;
        //                iCnt = (iCnt + 1) / 2;
        //            }
        //            else if (dataType == DataType.Input || dataType == DataType.Output)
        //                iCnt = varCount * 32;
        //            return iCnt;
        //        default:
        //            return 0;
        //    }
        //}

        ///// <summary>
        ///// 解析地址字节和位索引
        ///// MX100.1 - MB100.1
        ///// MB100 - MB100
        ///// MW100 - MB200
        ///// MD100 - MB400
        ///// </summary>
        ///// <param name="varType"></param>
        ///// <param name="startVarIndex"></param>
        ///// <returns></returns>
        //public static int? ToStartByteOfVarType(this string strVarible)
        //{
        //    if (strVarible.Length > 2)
        //    {
        //        int StartAddr;
        //        string strDataType = strVarible.Substring(0, 1);
        //        string strVarType = strVarible.Substring(1, 1);
        //        if (strVarType == "X" || strVarType == "B")
        //        {
        //            StartAddr = strVarible.Substring(2).MidString("", ".").ToMyInt();
        //            if (strDataType == "I" || strDataType == "Q")
        //                StartAddr = StartAddr * 8;
        //            else if (strDataType == "M")
        //            {
        //                if (StartAddr % 2 == 0)
        //                    StartAddr = (StartAddr + 1) / 2;
        //                else
        //                    StartAddr = (StartAddr + 1) / 2 - 1;
        //            }
        //            return StartAddr;
        //        }
        //        else if (strVarType == "W")
        //        {
        //            StartAddr = strVarible.Substring(2).MidString("", ".").ToMyInt() * 2;
        //            StartAddr = (StartAddr + 1) / 2;
        //            return StartAddr;
        //        }
        //        else if (strVarType == "D")
        //        {
        //            StartAddr = strVarible.Substring(2).MidString("", ".").ToMyInt() * 4;
        //            StartAddr = (StartAddr + 1) / 2;
        //            return StartAddr;
        //        }   
        //    }
        //    return null;
        //}

        /// <summary>
        /// 按PLC变量类型(Bool, Word, DWord, etc.)解析字节数据
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="bytes"></param>
        /// <param name="varCount"></param>
        /// <param name="bitAdr"></param>
        /// <returns></returns>
        public static object ParseBytes(this byte[] bytes, VarType varType, int varCount, byte bitAdr = 0)
        { 
            if (bytes == null) return null;

            switch (varType)
            {
                case VarType.BYTE:
                    if (varCount == 1)
                        return bytes[0];
                    else
                        return bytes;
                case VarType.WORD:
                    if (varCount == 1)
                        return Types.Word.FromByteArray(bytes, ByteOrder16.BA);
                    else
                        return Types.Word.ToArray(bytes, ByteOrder16.BA);
                case VarType.INT:
                    if (varCount == 1)
                        return Types.Int.FromByteArray(bytes, ByteOrder16.BA);
                    else
                        return Types.Int.ToArray(bytes, ByteOrder16.BA);
                case VarType.DWORD:
                    if (varCount == 1)
                        return Types.DWord.FromByteArray(bytes, ByteOrder32.DCBA);
                    else
                        return Types.DWord.ToArray(bytes, ByteOrder32.DCBA);
                case VarType.DINT:
                    if (varCount == 1)
                        return Types.DInt.FromByteArray(bytes, ByteOrder32.DCBA);
                    else
                        return Types.DInt.ToArray(bytes, ByteOrder32.DCBA);
                case VarType.REAL:
                    if (varCount == 1)
                        return Types.Real.FromByteArray(bytes, ByteOrder32.DCBA);
                    else
                        return Types.Real.ToArray(bytes, ByteOrder32.DCBA);
                case VarType.STRING:
                    return Types.String.FromByteArray(bytes);
                case VarType.BOOL:
                    if (varCount == 1)
                        if (bitAdr > 7)
                            return null;
                        else
                            return Bit.FromByte(bytes[0], bitAdr);
                    else
                        return Bit.ToBitArray(bytes);
                default:
                    return null;
            }
        }
    }
}
