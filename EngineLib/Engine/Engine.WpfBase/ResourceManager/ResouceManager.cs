using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace Engine.WpfBase
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResouceManager
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static ResouceManager Default
        {
            get
            {
                return _Default ?? new ResouceManager();
            }
        }
        private static ResouceManager _Default;

        /// <summary>
        /// 解析资源字典文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ResourceDictionary ParseResourceDict(string filePath)
        {
            string xml = File.ReadAllText(filePath);
            var xamlReader = new XmlTextReader(new StringReader(xml));
            var resourceDictionary = XamlReader.Load(xamlReader) as ResourceDictionary;
            return resourceDictionary;
        }

        /// <summary>
        /// 查找资源
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        public object FindResource(string ResourceName)
        {
            try
            {
                Collection<ResourceDictionary> MergedDict = Application.Current.Resources.MergedDictionaries;
                foreach (ResourceDictionary Dic in MergedDict)
                {
                    if (Dic.Contains(ResourceName))
                        return Dic[ResourceName];
                    Collection<ResourceDictionary> SubMergedDict = new Collection<ResourceDictionary>();
                    foreach (var subDic in Dic.MergedDictionaries)
                    {
                        if (subDic.Contains(ResourceName))
                            return subDic[ResourceName];
                    }
                }
            }
            catch (Exception)
            {

            }           
            return null;
        }

        public bool AppandStyle()
        {
            return true;
        }
    }
}
