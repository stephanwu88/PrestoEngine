using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Engine.Common
{
    public static partial class sCommon
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(this JToken jToken, string key)
        {
            string strValue = jToken.GetValue<string>(key);
            return strValue.ToMyString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jToken"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(this JToken jToken, string key)
        {
            try
            {
                if (jToken[key] != null)
                    return jToken[key].ToObject<T>();
            }
            catch (Exception ex)
            {

            }
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JToken SetValue(this JToken jToken, string key, object value)
        {
            try
            {
                jToken[key] = JToken.FromObject(value);
            }
            catch (Exception)
            {

            }
            return jToken;
        }

        /// <summary>
        /// 新节点
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="ObjectName"></param>
        /// <returns></returns>
        public static JToken NewJNode(this JToken jToken, string NodeName)
        {
            try
            {
                if (jToken is JObject jObj)
                {
                    if (!jObj.ContainsKey(NodeName))
                        jObj.SetValue(NodeName, new JObject());
                    return jObj[NodeName];
                }
            }
            catch (Exception)
            {

            }
            return null;
        }

        /// <summary>
        /// 新节点
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="ObjectName"></param>
        /// <returns></returns>
        public static JToken NewJNode(this JToken jToken, string NodeName,string NodeValue)
        {
            try
            {
                if (jToken is JObject jObj)
                {
                    jObj[NodeName] = NodeValue;
                    return jObj[NodeName];
                }
            }
            catch (Exception)
            {

            }
            return null;
        }

        /// <summary>
        /// List转换成字典-主键去重
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keyGetter"></param>
        /// <param name="valueGetter"></param>
        /// <returns></returns>
        public static JDictionary ToMyJDictionary<TElement, TKey, TValue>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keyGetter,
            Func<TElement, TValue> valueGetter)
        {
            JDictionary dict = new JDictionary();
            try
            {
                if (source == null) return dict;
                foreach (var e in source)
                {
                    TKey key = keyGetter(e);
                    if (valueGetter(e) == null || key == null)
                        continue;
                    dict[key] = JToken.FromObject(valueGetter(e));
                }
            }
            catch (Exception)
            {

            }
            return dict;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JDictionary 
    {
        public JObject Root { get; set; } = new JObject();

        public void FromList<T>(List<T> LstModel)
        {
            try
            {
                JArray jArray = JArray.FromObject(LstModel);
                Root = jArray.ToObject<JObject>();
            }
            catch (Exception)
            {

            }
        }

        public string GetString(string key)
        {
            string strValue = GetValue<string>(key);
            return strValue.ToMyString();
        }

        public void SetValue(string key, object value)
        {
            try
            {
                JToken token = Root[key];
                token = JToken.FromObject(value);
            }
            catch (Exception)
            {

            }
        }

        public T GetValue<T>(string key)
        {
            JToken token = Root[key];
            return token.ToObject<T>();
        }

        /// <summary>
        /// 对象克隆
        /// </summary>
        /// <param name="ObjectDic"></param>
        public bool CloneTo(out JDictionary ObjectDic)
        {
            try
            {               
                ObjectDic = new JDictionary();
                ObjectDic.Root = Root.DeepClone() as JObject;
                return true;
            }
            catch (Exception)
            {
                ObjectDic = null;
                return false;
            }          
        }

        /// <summary>
        /// 获取对象的Json字符串
        /// </summary>
        public string JsonString
        {
            get
            {
                try
                {
                    CallResult Result = Root.MyJsonSerialize();
                    if (Result.Success)
                        return Result.Result.ToMyString().FormatJsonString();
                }
                catch (Exception)
                {

                }
                return string.Empty;
            }
        }

        public void Clear()
        {
            Root.RemoveAll();
        }

        public JToken this[object key]
        {
            get
            {
                if (Root[key] == null)
                    Root[key] = new JObject();
                return Root[key];
            }
            set
            {
                Root[key] = value;
            }
        }
    }
}
