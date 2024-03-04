using Engine.Common;
using System;
using System.Collections.Generic;

namespace Engine.ComDriver.Schneider
{
    /// <summary>
    /// 获取功能码
    /// </summary>
    public class FunCodeAttribute : BasicAttribute
    {
        public FunCodeAttribute(byte by1, byte by2) : base(new List<object>() { by1, by2 })
        {

        }

        public static byte ReadCode(Enum value)
        {
            return (byte)GetEnumCustomAttributeContent(value)[0];
        }

        public static byte WriteCode(Enum value)
        {
            return (byte)GetEnumCustomAttributeContent(value)[1];
        }

        public static List<object> GetFunCodeContent(Enum value)
        {
            return GetEnumCustomAttributeContent(value);
        }
    }
}
