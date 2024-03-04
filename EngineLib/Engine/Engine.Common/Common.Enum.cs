using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Engine.Common
{
    /// <summary>
    /// 枚举转换类
    /// </summary>
    public static partial class sCommon
    {
        /// <summary> 
        /// 获取枚举值的描述文本 
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static string FetchDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static T StringToEnum<T>(this string Source)
        {
            try
            {
                var result = (T)Enum.Parse(typeof(T), Source);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;             
            }
        }
        /// <summary>
        /// 根据枚举的值获取枚举名称
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="status">枚举的值</param>
        /// <returns></returns>
        public static string GetEnumName<T>(this int status)
        {
            return Enum.GetName(typeof(T), status);
        }
        /// <summary>
        /// 获取枚举名称集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetNamesArr<T>()
        {
            return Enum.GetNames(typeof(T));
        }
        /// <summary>
        /// 将枚举转换成字典集合
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns></returns>
        public static Dictionary<string, int> getEnumDic<T>()
        {

            Dictionary<string, int> resultList = new Dictionary<string, int>();
            Type type = typeof(T);
            var strList = GetNamesArr<T>().ToList();
            foreach (string key in strList)
            {
                string val = Enum.Format(type, Enum.Parse(type, key), "d");
                resultList.Add(key, int.Parse(val));
            }
            return resultList;
        }
        /// <summary>
        /// 将枚举转换成字典
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, int> GetDic<TEnum>()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            Type t = typeof(TEnum);
            var arr = Enum.GetValues(t);
            foreach (var item in arr)
            {
                dic.Add(item.ToString(), (int)item);
            }

            return dic;
        }
    }
}
