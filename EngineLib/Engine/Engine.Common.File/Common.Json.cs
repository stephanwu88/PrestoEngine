using Newtonsoft.Json;
using System;
using System.IO;

namespace Engine.Common
{
    /// <summary>
    /// Json
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// json字符串反序列化实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        public static CallResult MyJsonDeserialize<T>(this string JsonString)
        {
            CallResult ret = new CallResult() { Success = false };
            try
            {
                if (string.IsNullOrEmpty(JsonString))
                {
                    ret.Result = "Json字符串为空";
                    ret.Success = false;
                    return ret;
                }
                if (JsonString.Contains("\\"))
                    JsonString = JsonString.FormatPath();
                T Obj = JsonConvert.DeserializeObject<T>(JsonString);
                ret.Success = true;
                ret.Result = Obj;
            }
            catch (Exception ex)
            {
                ret.Result = string.Format("Json反序列化实体失败:\r\n{0}\r\n", ex.Message);
                ret.Success = false;
            }

            return ret;
        }

        /// <summary>
        /// 对象序列化json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CallResult MyJsonSerialize(this object obj)
        {
            CallResult ret = new CallResult() { Success = false };
            try
            {
                ret.Result = JsonConvert.SerializeObject(obj);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Result = string.Format("Json序列化字符串失败:\r\n{0}\r\n", ex.Message);
                ret.Success = false;
            }
            return ret;
        }

        /// <summary>
        /// 对象序列化json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public static string MyJsonSerialize(this object obj, out bool Error)
        {
            CallResult ret = obj.MyJsonSerialize();
            Error = ret.Fail;
            return ret.Result.ToMyString();
        }

        /// <summary>
        /// 格式化Json字符串显示
        /// </summary>
        /// <param name="SourceJson">格式化之前的字符串</param>
        /// <returns>格式化显示后的字符串</returns>
        public static string FormatJsonString(this string SourceJson)
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                TextReader tr = new StringReader(SourceJson);
                JsonTextReader jtr = new JsonTextReader(tr);
                object obj = serializer.Deserialize(jtr);
                if (obj != null)
                {
                    StringWriter textWriter = new StringWriter();
                    JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 4,
                        IndentChar = ' '
                    };
                    serializer.Serialize(jsonWriter, obj);
                    return textWriter.ToString();
                }
            }
            catch (Exception)
            {

            }
            return SourceJson;
        }
    }
}