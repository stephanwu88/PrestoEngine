using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Engine.MVVM
{
    /// <summary>
    /// 属性项
    /// </summary>
    public class PropItem
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropName { get; set; }
        /// <summary>
        /// 属性绑定源
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 属性转换器
        /// </summary>
        public string ConverterKey { get; set; }
    }

    /// <summary>
    /// 画面元件
    /// </summary>
    public class ComponentModel
    {
        /// <summary>
        /// 元件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 元件类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 属性项
        /// </summary>
        public List<PropItem> PropItems { get; set; }

        /// <summary>
        /// 解析元件设置
        /// </summary>
        /// <param name="ConfigString"></param>
        /// <returns></returns>
        public static ComponentModel ParseSource(string ConfigString)
        {
            ComponentModel CompoModel = new ComponentModel();
            try
            {
                CompoModel = JsonConvert.DeserializeObject<ComponentModel>(ConfigString);
            }
            catch (Exception )
            {

            }
            return CompoModel;
        }
    }
}
