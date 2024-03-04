using System;

namespace Engine.Common
{
#if 长治
    /// <summary>
    /// 长治快分钢样样品编码 - 炉号月度序列号，机号序列号
    /// 月度序列号： 3位
    /// 月度序列号：001- 999，超过999表示，999,A00,A01..A99，B00，B01..B99,C01.. 1099 1100
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 月度序列号： 3位
        /// 月度序列号：001- 999，超过999表示，999,A00,A01..A99，B00，B01..B99,C01.. 1099 1100
        /// </summary>
        /// <param name="MouthSerialNum">1-1599</param>
        /// <returns>001..F99</returns>
        public static string ToMouthSerialNum(this string MouthSerialNum)
        {
            try
            {
                if (!MouthSerialNum.IsNumeric())
                {
                    string strHead = MouthSerialNum.MidString(0, 1);
                    string strIntHead = Convert.ToInt32(strHead, 16).ToMyString();
                    MouthSerialNum = strIntHead + MouthSerialNum.MidString(1);
                }
                int iMouthSerial = MouthSerialNum.ToMyInt();
                string strMouthSerial = string.Empty;
                if (iMouthSerial >= 1 && iMouthSerial <= 999)
                    strMouthSerial = iMouthSerial.ToMyString();
                else if (iMouthSerial > 999)
                {
                    strMouthSerial = iMouthSerial.ToMyString();
                    int DecHeadTwo = strMouthSerial.MidString(0, 2).ToMyInt();
                    string strTail = strMouthSerial.MidString(2);
                    string strHexHead = DecHeadTwo.DecToHex(1);
                    strMouthSerial = $"{strHexHead}{strTail}";
                }
                return strMouthSerial.MidString(0, 3).PadLeft(3, '0');
            }
            catch (Exception)
            {
                return "000";
            }
        }

        /// <summary>
        /// 月度序列号： 3位
        /// 月度序列号：001- 999，超过999表示，999,A00,A01..A99，B00，B01..B99,C01.. 1099 1100
        /// </summary>
        /// <param name="FormatMouthSerialNum">001..F99</param>
        /// <returns>1-1599</returns>
        public static string ToIntSerialNum(this string FormatMouthSerialNum)
        {
            try
            {
                if (!FormatMouthSerialNum.IsNumeric())
                {
                    string strHead = FormatMouthSerialNum.MidString(0, 1);
                    string strIntHead = Convert.ToInt32(strHead, 16).ToMyString();
                    FormatMouthSerialNum = strIntHead + FormatMouthSerialNum.MidString(1);
                }
                return FormatMouthSerialNum;
            }
            catch (Exception)
            {
                return "000";
            }
        }
    }

    /// <summary>
    /// 长治快分钢样样品编码 - 中包样 年、月份标识定义
    /// 年位示意（1位）：2023:  3 
    /// 月位示意（1位）：1-9月：1-9     	10月： 0        11月： +  	 12月： - 
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 年位： 1位 末尾年份
        /// 年位示意（1位）：2023:  3 
        /// </summary>
        /// <param name="Mouth"></param>
        /// <returns></returns>
        public static string ToChangzhiYearBit(this string Year)
        {
            return Year.MidString(-1, 1);
        }

        /// <summary>
        /// 月份： 1位
        /// 月位示意（1位）：1-9月：1-9     	10月： 0        11月： +  	 12月： - 
        /// </summary>
        /// <param name="Mouth"></param>
        /// <returns></returns>
        public static string ToChangzhiMouthBit(this string Mouth)
        {
            int iMouth = Mouth.ToMyInt();
            if (iMouth >= 1 && iMouth <= 9)
                return iMouth.ToMyString();
            else if (iMouth == 10)
                return "0";
            else if (iMouth == 11)
                return "+";
            else if (iMouth == 12)
                return "-";
            return "";
        }
    }
#endif
}
