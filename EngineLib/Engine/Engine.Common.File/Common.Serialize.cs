using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Engine.Common
{
    /// <summary>
    ///  串行化数据流序列化及反序列化，以Base64形式存储
    ///  此流可以引用多种后备存储（如文件、网络、内存等）
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 实体序列化为Base64字符串 
        /// </summary>
        /// <param name="obj"></param>
        public static string SerializeBase64<T>(this T obj)
        {
            string ret = string.Empty;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
                ret = Convert.ToBase64String(buffer);
            }
            catch (Exception ex)
            {
                throw new Exception("序列化失败,原因:" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 实体序列化为Base64字符串,并写入文件
        /// </summary>
        /// <param name="obj"></param>
        public static string SerializeBase64<T>(this T obj, string filename)
        {
            string ret = string.Empty;
            try
            {
                string strText = obj.SerializeBase64<T>();
                File.WriteAllText(filename, strText);
            }
            catch (Exception ex)
            {
                throw new Exception("序列化失败,原因:" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 字符串反序列化为实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="str">反序列化字符串</param>
        /// <returns></returns>
        public static T DeSerializeBase64<T>(string str)
        {
            T obj;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                byte[] buffer = Convert.FromBase64String(str);
                MemoryStream stream = new MemoryStream(buffer);
                obj = (T)formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("反序列化失败,原因:" + ex.Message);
            }
            return obj;
        }
    }

    /// <summary>
    /// 二进制字符串序列化及反序列化
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 实体序列化为二进制字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool SerializeBinary<T>(this T obj, string filename)
        {
            try
            {
                obj.FormatterSerialize(filename, new BinaryFormatter());
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("序列化失败,原因:" + ex.Message);
            }
        }

        /// <summary>
        /// 二进制或Soap文件反序列化到实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="filename"></param>
        /// <param name="mode"> BinaryFile or SoapFile</param>
        /// <returns></returns>
        public static T DeSerializeBin<T>(string filename)
        {
            T obj;
            try
            {
                obj = DeSerializeBin<T>(filename, new BinaryFormatter());
            }
            catch (Exception ex)
            {
                throw new Exception("反序列化失败,原因:" + ex.Message);
            }
            return obj;
        }
    }

    /// <summary>
    /// Soap网络协议字符串序列化及反序列化
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 实体序列化为二进制字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool SerializeSoap<T>(this T obj, string filename)
        {
            try
            {
                obj.FormatterSerialize(filename, new SoapFormatter());
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("序列化失败,原因:" + ex.Message);
            }
        }

        /// <summary>
        /// 二进制或Soap文件反序列化到实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="filename"></param>
        /// <param name="mode"> BinaryFile or SoapFile</param>
        /// <returns></returns>
        public static T DeSerializeSoap<T>(string filename)
        {
            T obj;
            try
            {
                obj = DeSerializeBin<T>(filename, new SoapFormatter());
            }
            catch (Exception ex)
            {
                throw new Exception("反序列化失败,原因:" + ex.Message);
            }
            return obj;
        }
    }

    /// <summary>
    /// 格式序列化反序列化器
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 格式序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="formater"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool FormatterSerialize<T>(this T obj, string filename, IFormatter formater)
        {
            try
            {
                IFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(filename, FileMode.Create);
                bf.Serialize(fs, obj);
                fs.Flush();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("序列化失败,原因:" + ex.Message);
            }
        }

        /// <summary>
        /// 格式反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static T DeSerializeBin<T>(string filename, IFormatter formatter)
        {
            T obj;
            try
            {
                //FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                FileStream fs = new FileStream(filename, FileMode.Open);
                obj = (T)formatter.Deserialize(fs);
                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("反序列化失败,原因:" + ex.Message);
            }
            return obj;
        }
    }

    /// <summary>
    /// 自定义格式序列化
    /// 宝信人员自定义格式将属性值用标记分割拼接
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 将对象序列化为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Model"></param>
        /// <param name="SplitMark"></param>
        /// <returns></returns>
        public static CallResult MySerialize<T>(this T Model, string SplitMark = "$")
        {
            CallResult _result = new CallResult() { Success = false, Result = "" };
            try
            {
                var Types = Model.GetType();
                string strSerialized = string.Empty;
                foreach (PropertyInfo sp in Types.GetProperties())
                {
                    string spName = sp.Name;
                    string spVal = sp.GetValue(Model, null).ToMyString();
                    strSerialized += $"{spName}{SplitMark}{spVal}{SplitMark}";
                }
                _result.Result = strSerialized;
                _result.Success = true;
            }
            catch (Exception ex)
            {
                _result.Success = false;
                _result.Result = ex.Message;
            }
            return _result;
        }
    }
}
