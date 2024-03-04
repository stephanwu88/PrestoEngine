using Engine.Common;
using System.Collections.Generic;
using System.Linq;

namespace Engine.ComDriver.Siemens
{
    /// <summary>
    /// 西门子常用类
    /// </summary>
    public static class S7Common
    {
        /// <summary>
        /// 根据变量类型，自动确定变量字节右限
        /// </summary>
        /// <param name="DataType"></param>
        /// <returns></returns>
        public static int ToMyIncByte(this string DataType)
        {
            int ret = 0;
            string strDataType = DataType.ToUpper();
            switch (strDataType)
            {
                case "BOOL":
                    ret = 0;
                    break;
                case "INT":
                case "WORD":
                    ret = 1;
                    break;
                case "DINT":
                case "DWORD":
                case "REAL":
                    ret = 3;
                    break;
                default:
                    if (strDataType.Contains("STRING"))
                    {
                        int iLen = strDataType.MidString("STRING[", "]").ToMyInt();
                        if (iLen > 0)
                            ret = iLen + 2 - 1;
                    }
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 将西门子200 V地址转换成DB地址给通讯识别器
        /// </summary>
        /// <returns></returns>
        public static string MySmartVToDB(this string strV)
        {
            if (strV.Contains("V"))
            {
                if (strV.Contains("."))
                    strV = strV.Replace("V", "DB1.DBX");
                else
                    strV = strV.Replace("V", "DB1.DB");
            }
            return strV;
        }

        /// <summary>
        /// 将西门子规格数据地址转换至程序结构地址
        /// </summary>
        /// <param name="DataAddr">西门子规格地址</param>
        /// <param name="ObjData"></param>
        /// <returns></returns>
        public static object ToMyVarStruct(this object DataAddr, string ObjData)
        {
            object ret = null;
            DataType eDataType = new DataType();
            string strDataAddr = DataAddr.ToMyString();
            string strValKey = string.Empty;
            string strByteStart = string.Empty;
            string strBitOffset = string.Empty;
            if (strDataAddr.Contains("I"))
            {
                strValKey = "I";
                strDataAddr = strDataAddr.RemoveString(new string[] { "IB", "IW", "ID", "I" });
                strByteStart = strDataAddr.MidString("", ".");
                strBitOffset = strDataAddr.MidString(".", "");
                eDataType = DataType.Input;
            }
            else if (strDataAddr.Contains("Q"))
            {
                strValKey = "Q";
                strDataAddr = strDataAddr.RemoveString(new string[] { "QB", "QW", "QD", "Q", });
                strByteStart = strDataAddr.MidString("", ".");
                strBitOffset = strDataAddr.MidString(".", "");
                eDataType = DataType.Output;
            }
            else if (strDataAddr.Contains("M"))
            {
                strValKey = "M";
                strDataAddr = strDataAddr.RemoveString(new string[] { "MB", "MW", "MD", "M" });
                strByteStart = strDataAddr.MidString("", ".");
                strBitOffset = strDataAddr.MidString(".", "");
                eDataType = DataType.Memory;
            }
            else if (strDataAddr.Contains("DB"))
            {
                //ex: DB10.DBX0.0  DB10.DBW0  DB10.DBDS 字符串型  1200,1500PLC地址
                strValKey = strDataAddr.MidString("", "."); //ex: DB10
                strDataAddr = strDataAddr.RemoveString(new string[] { strValKey + "." });   //去除DB10.
                strDataAddr = strDataAddr.RemoveString(new string[] { "DBB", "DBX", "DBW", "DBD", "DBS" });
                strByteStart = strDataAddr.MidString("", ".");
                strBitOffset = strDataAddr.MidString(".", "");

                eDataType = DataType.DataBlock;
            }
            else if (strDataAddr.Contains("V"))
            {
                //ex: V10.0  VB10 VW10 VD10  西门子200Smart地址  相当于1200 DB1.DBX10.0 DB1.DBB10 DB1.DBW10 DB1.DBD10
                //将200Smart V地址格式转换成1200地址格式，程序同意处理
                strDataAddr = strDataAddr.MySmartVToDB();
                strValKey = "V";    // strDataAddr.MidString("", "."); //ex: V  相当于 DB1
                string strValKeyInstead = strDataAddr.MidString("", ".");
                strDataAddr = strDataAddr.RemoveString(new string[] { strValKeyInstead + "." });   //去除DB10.
                strDataAddr = strDataAddr.RemoveString(new string[] { "DBB", "DBX", "DBW", "DBD", "DBS" });
                strByteStart = strDataAddr.MidString("", ".");
                strBitOffset = strDataAddr.MidString(".", "");
                eDataType = DataType.DataBlock;
            }
            switch (ObjData)
            {
                case "VarKey":
                    ret = strValKey;
                    break;
                case "ByteStart":
                    int iRet = 0;
                    int.TryParse(strByteStart, out iRet);
                    ret = iRet;
                    break;
                case "BitOffset":
                    iRet = 0;
                    int.TryParse(strBitOffset, out iRet);
                    ret = iRet;
                    break;
                case "DataType":
                    ret = eDataType;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 根据变量节点调整通讯数据地址范围
        /// </summary>
        /// <param name="TempGrpRange"></param>
        /// <param name="sVarNode"></param>
        /// <returns></returns>
        public static List<AddrRange> BuildDataAddrRange(List<AddrRange> TempGrpRange, VarStruct sVarNode, int MaxReadByte, out int StartByte)
        {
            string result = "NewRange";
            AddrRange FindAddrRange = new AddrRange();
            StartByte = 0;
            foreach (AddrRange ran in TempGrpRange)
            {
                //找到有效区间
                if (ran.addrKey == sVarNode.VarKey)
                {
                    //根据变量确定的右边界，判断是否在现有的右边界范围内，不在，则按数据格式拓展右侧边界
                    //当拓展后的右侧边界超过200长度时，则以该地址为起始定义新区间
                    if (sVarNode.ByteStart >= ran.min && sVarNode.ByteStart + sVarNode.DataType.ToMyIncByte() <= ran.min + MaxReadByte)
                    {
                        FindAddrRange = ran;
                        if (sVarNode.ByteStart + sVarNode.DataType.ToMyIncByte() > ran.max)
                        {
                            FindAddrRange.max = sVarNode.ByteStart + sVarNode.DataType.ToMyIncByte();
                            FindAddrRange.len = FindAddrRange.max - FindAddrRange.min + 1;
                            result = "FindAndAlterRange";
                        }
                        else
                            result = "Find";
                        break;
                    }
                }
            }
            switch (result)
            {
                case "NewRange":
                    //不在区间内，则以该地址为起始定义新区间
                    AddrRange rang = new AddrRange();
                    rang.addrKey = sVarNode.VarKey;
                    //将西门子200 V地址转换成DB地址给通讯识别器
                    sVarNode.VarKey = sVarNode.VarKey.MySmartVToDB();

                    int.TryParse(sVarNode.VarKey.MidString("DB", "."), out rang.dbNo);
                    rang.min = sVarNode.ByteStart;
                    rang.max = sVarNode.ByteStart + sVarNode.DataType.ToMyIncByte();
                    rang.len = rang.max - rang.min + 1;
                    TempGrpRange.Add(rang);
                    StartByte = rang.min;
                    break;
                case "FindAndAlterRange":
                    //修改右侧范围
                    TempGrpRange.Where(x => x.addrKey == FindAddrRange.addrKey && x.min == FindAddrRange.min).ToList()[0].max = FindAddrRange.max;
                    TempGrpRange.Where(x => x.addrKey == FindAddrRange.addrKey && x.min == FindAddrRange.min).ToList()[0].len = FindAddrRange.len;
                    StartByte = FindAddrRange.min;
                    break;
                case "Find":
                    //找到范围
                    StartByte = FindAddrRange.min;
                    break;
            }
            return TempGrpRange;
        }
    }
}
