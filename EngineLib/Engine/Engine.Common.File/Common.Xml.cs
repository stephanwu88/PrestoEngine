using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Engine.Common
{
    /// <summary>
    /// 添加模式
    /// </summary>
    public enum AppandMode
    {
        /// <summary>
        /// 附加
        /// </summary>
        Add,
        /// <summary>
        /// 重复时候忽略
        /// </summary>
        RepeatIngore,
        /// <summary>
        /// 重复时覆盖
        /// </summary>
        OverCover
    }

    /// <summary>
    /// Xml文件常用方法
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 格式化Xml字符串
        /// </summary>
        /// <param name="XmlDoc"></param>
        /// <returns></returns>
        public static string ToMyString(this XmlDocument XmlDoc, bool Formatted = true)
        {
            try
            {
                if (Formatted)
                {
                    return XmlDoc.FormatXmlString();
                }
            }
            catch (Exception ex)
            {

            }
            return XmlDoc?.OuterXml.ToMyString();
        }

        /// <summary>
        /// XmlDoc格式化获取Xml字符串
        /// </summary>
        /// <param name="XmlDoc"></param>
        /// <returns></returns>
        public static string FormatXmlString(this XmlDocument XmlDoc)
        {
            try
            {
                StringWriter _StringWriter = new StringWriter();
                XmlWriterSettings _WriterSettings = new XmlWriterSettings()
                {
                    Indent = true,              //设置缩进
                    NewLineChars = "\r\n",      //设置换行符
                    OmitXmlDeclaration = false, //忽略XML声明
                };
                using (XmlWriter writer = XmlWriter.Create(_StringWriter, _WriterSettings))
                {
                    XmlDoc.WriteTo(writer);
                }
                //获取格式化Xml字符串
                string FormatedXmlString = _StringWriter.ToString();
                return FormatedXmlString;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 将字符串格式化Xml字符串
        /// </summary>
        /// <param name="SourceXml"></param>
        /// <returns></returns>
        public static string FormatXmlString(this string SourceXml)
        {
            try
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(SourceXml);
                return XmlDoc.FormatXmlString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// XML格式序列化及反序列化
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 实体序列化为Xml字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string XmlSerialize<T>(this T obj)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                    System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
                    XmlWriterSettings setting = new XmlWriterSettings();
                    setting.Encoding = new UTF8Encoding(false);
                    setting.Indent = true;
                    ns.Add("", "");
                    using (XmlWriter writer = XmlWriter.Create(ms, setting))
                    {
                        serializer.Serialize(writer, obj, ns);
                        //serializer.Serialize(ms, obj, ns);
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("序列化失败,原因:" + ex.Message);
            }
        }

        /// <summary>
        /// Xml字符串反序列化实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(this string xml) where T : class
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    return serializer.Deserialize(sr) as T;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("序列化失败,原因:" + ex.Message);
            }
        }

        /// <summary>
        /// 序列化实体到Xml文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool XmlSerialize<T>(this T obj, string filename)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                TextWriter writer = new StreamWriter(filename);
                serializer.Serialize(writer, obj);
                writer.Flush();
                writer.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// XML文件反序列化为实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="type">实体类型</param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(Type type, string filename)
        {
            T Obj;
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
                FileStream fs = new FileStream(filename, FileMode.Open);
                Obj = (T)serializer.Deserialize(fs);
                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("反序列化失败,原因:" + ex.Message); ;
            }
            return Obj;
        }
    }

    /// <summary>
    /// Xml文件
    /// </summary>
    public partial class sCommon
    {
        /*
        //若资源文件位于DLL的根目录下，而且没有命名空间：
        //string resourceName = "ResourceFile.xml";
        //若资源文件位于DLL的根目录下，但有一个命名空间：
        //string resourceName = "Namespace.ResourceFile.xml";
        //若资源文件位于DLL中的文件夹中，同时带有命名空间：
        //string resourceName = "Namespace.Folder.ResourceFile.xml";

        //读取资源文件
        XmlDocument doc = sCommon.LoadResourceXml("Engine.dll","Engine", "Engine.ComDriver\\ComModule.Instrument/Bruker.ComField.xml",out bool s1);
        XmlDocument doc1 = sCommon.LoadResourceXml(MyAssembly.Excuting, "Engine", "Engine.ComDriver\\ComModule.Instrument/Bruker.ComField.xml",out bool s);
        //读取外部文件
        XmlDocument doc2 = sCommon.LoadXml(MyAssembly.Excuting, "Engine.ComDriver/ComModule.Instrument/Bruker.ComField.xml",out bool s3);
        */

        /// <summary>
        /// 读取Xml文件
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="RelPath"></param>
        /// <param name="Success"></param>
        /// <returns></returns>
        public static XmlDocument LoadXml(MyAssembly assembly, string RelPath, out bool Success)
        {
            string assemblyDirectory = Path.GetDirectoryName(assembly.Location);
            string file = Path.Combine(assemblyDirectory, RelPath);
            XmlDocument xmlDoc = new XmlDocument();
            Success = false;
            //using (Stream stream = assembly.AssemblyInfo.GetManifestResourceStream(resourceName))
            //{
            //    if (stream != null)
            //    {
            //        using (StreamReader reader = new StreamReader(stream))
            //        {
            //            string xmlContent = reader.ReadToEnd();
            //            xmlDoc.LoadXml(xmlContent);
            //            Success = true;
            //        }
            //    }
            //}
            return xmlDoc;
        }

        /// <summary>
        /// 读取xml资源文件-本程序集使用
        /// </summary>
        /// <param name="ResouceFileName"></param>
        /// <param name="Success"></param>
        /// <returns></returns>
        internal static XmlDocument LoadResourceXml(string ResouceFileName, out bool Success)
        {
            return LoadResourceXml("Engine.dll", "Engine", ResouceFileName, out Success);
        }

        /// <summary>
        /// 读取xml资源文件
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="NameSpace"></param>
        /// <param name="ResourceFileName"></param>
        /// <param name="Success"></param>
        /// <returns></returns>
        public static XmlDocument LoadResourceXml(MyAssembly assembly, string NameSpace, string ResourceFileName, out bool Success)
        {
            string AssemblyName = assembly.FileFullName;
            return LoadResourceXml(AssemblyName, NameSpace, ResourceFileName, out Success);
        }

        /// <summary>
        /// 读取xml资源文件
        /// ex: Engine.Engine.ComDriver.ComModule.Instrument.Bruker.ComField.xml
        /// 命名控件：Engine  
        /// 文件路径：Engine.ComDriver  ComModule.Instrument  Bruker 
        /// 文件名：  ComField.xml
        /// </summary>
        /// <param name="AssemblyName">程序集</param>
        /// <param name="NameSpace">命名空间</param>
        /// <param name="ResourceFileName">Engine.ComDriver\ComModule.Instrument\Bruker</param>
        /// <param name="Success"></param>
        /// <returns></returns>
        public static XmlDocument LoadResourceXml(string AssemblyName, string NameSpace, string ResourceFileName, out bool Success)
        {
            Assembly assembly = Assembly.LoadFrom(AssemblyName);

            string resourceName = string.Empty;
            if (string.IsNullOrEmpty(NameSpace))
            {
                resourceName = $"{ResourceFileName}";
            }
            else
            {
                ResourceFileName = ResourceFileName.Replace("\\\\", ".").Replace("\\", ".").Replace("/", ".");
                resourceName = $"{NameSpace}.{ResourceFileName}";
            }
            XmlDocument xmlDoc = new XmlDocument();
            Success = false;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {

                xmlDoc = LoadXmlWithValidate(stream, true);
                Success = xmlDoc != null;
            }
            return xmlDoc;
        }

        /// <summary>
        /// 加载Xml并验证
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="IgnoreComments">过滤注释</param>
        /// <returns></returns>
        public static XmlDocument LoadXmlWithValidate(string fileName, bool IgnoreComments = true)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = IgnoreComments;
            settings.DtdProcessing = DtdProcessing.Prohibit;
            settings.ValidationType = ValidationType.DTD;
            return LoadXmlWithValidate(fileName, settings);
        }

        /// <summary>
        /// 加载Xml并验证
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static XmlDocument LoadXmlWithValidate(string fileName, XmlReaderSettings settings)
        {
            try
            {
                XmlReader reader = default(XmlReader);
                if (settings == null)
                    reader = XmlReader.Create(fileName);
                else
                    reader = XmlReader.Create(fileName, settings);
                XmlDocument document = new XmlDocument();
                document.Load(reader);
                return document;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 加载Xml并验证
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="IgnoreComments">过滤注释</param>
        /// <returns></returns>
        public static XmlDocument LoadXmlWithValidate(Stream stream, bool IgnoreComments = true)
        {
            if (stream == null) return null;
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = IgnoreComments,
                DtdProcessing = DtdProcessing.Prohibit,
                ValidationType = ValidationType.DTD
            };
            return LoadXmlWithValidate(stream, settings);
        }

        /// <summary>
        /// 加载Xml并验证
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static XmlDocument LoadXmlWithValidate(Stream stream, XmlReaderSettings settings)
        {
            try
            {
                XmlReader reader = default(XmlReader);
                if (settings == null)
                    reader = XmlReader.Create(stream);
                else
                    reader = XmlReader.Create(stream, settings);
                XmlDocument document = new XmlDocument();
                document.Load(reader);
                return document;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 新建XmlDoc，导入节点
        /// </summary>
        /// <param name="ImportNode"></param>
        /// <param name="Deep">是否深层克隆</param>
        /// <returns></returns>
        public static XmlDocument ImportToNewDoc(this XmlNode ImportNode, bool Deep = true)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.AppendChild(dec);
            XmlDocument newDoc = new XmlDocument();
            newDoc.LoadXml(ImportNode.OuterXml);
            XmlNode node = doc.ImportNode(newDoc.DocumentElement, Deep);
            doc.AppendChild(node);
            return doc;
        }

        /// <summary>
        /// 导入到已有Doc
        /// </summary>
        /// <param name="ImportNode"></param>
        /// <param name="Doc"></param>
        /// <returns></returns>
        public static bool ImportToDoc(this XmlNode ImportNode, ref XmlDocument Doc, out string ErrorMessage)
        {
            try
            {
                ErrorMessage = string.Empty;
                XmlDocument newDoc = new XmlDocument();
                newDoc.LoadXml(ImportNode.OuterXml);
                XmlNode node = Doc.ImportNode(newDoc.DocumentElement, true);
                Doc.AppendChild(node);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 模式化添加Xml节点
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="xmlElem"></param>
        /// <param name="appandMode"></param>
        public static void AppandXmlNode(this XmlNode ParentNode, XmlElement xmlElem, AppandMode appandMode = AppandMode.OverCover)
        {
            if (ParentNode is XmlDocument)
                ParentNode = (ParentNode as XmlDocument)?.DocumentElement;
            if (ParentNode == null)
                return;
            XmlNodeList nodeLst = ParentNode.SelectNodes($"{xmlElem.Name}");
            if (nodeLst?.Count == 0)
                ParentNode.AppendChild(xmlElem);
            else
            {
                if (appandMode == AppandMode.OverCover)
                {
                    foreach (XmlNode item in nodeLst)
                    {
                        ParentNode.RemoveChild(item);
                    }
                }
                if (appandMode != AppandMode.RepeatIngore)
                    ParentNode.AppendChild(xmlElem);
            }
        }

        /// <summary>
        /// 添加Xml节点到Document
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="xParentPath"></param>
        /// <param name="xmlElem"></param>
        /// <param name="appandMode"></param>
        public static void AppandXmlNode(this XmlDocument doc, string xParentPath, XmlElement xmlElem,
            AppandMode appandMode = AppandMode.OverCover)
        {
            if (doc?.DocumentElement == null)
                return;
            XmlNode node = doc.DocumentElement.SelectSingleNode(xParentPath);
            node.AppandXmlNode(xmlElem, appandMode);
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="xPath"></param>
        public static void RemoveXmlNode(this XmlDocument doc, string xPath)
        {
            if (doc?.DocumentElement == null)
                return;
            XmlNodeList nodeLst = doc.DocumentElement.SelectNodes(xPath);
            foreach (XmlNode node in nodeLst)
            {
                XmlNode parent = node.ParentNode;
                if (parent != null)
                    parent.RemoveChild(node);
            }
        }
    }

    /// <summary>
    /// Xml Linq
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// XmlDocument 转 XDocument
        /// </summary>
        /// <param name="document"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static XDocument ToXDocument(this XmlDocument document, LoadOptions options = LoadOptions.None)
        {
            using (XmlNodeReader reader = new XmlNodeReader(document))
            {
                return XDocument.Load(reader, options);
            }
        }

        /// <summary>
        /// 获取XElement节点
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="DeepString">ex: AutomationCommandList/AutomationCommand[@Name='{_SendContent.Text}']</param>
        /// <returns></returns>
        public static XElement GetElement(this XDocument xdoc, string xpath)
        {
            List<string> lstPath = xpath.MySplit("/");
            if (xdoc.Document == null || lstPath.MyCount() == 0)
                return null;
            XElement xElem = xdoc.Document.Element(lstPath[0]);
            foreach (var item in lstPath)
            {
                xElem = xElem.Element(item);
            }
            return xElem;
        }

        /// <summary>
        /// Xml字符串转XElement
        /// </summary>
        /// <param name="strXml"></param>
        /// <returns></returns>
        public static XElement ToXElement(this string strXml)
        {
            XElement xElem = XElement.Parse(strXml);
            return xElem;
        }

        /// <summary>
        /// XmlDocument转XElement
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static XElement ToXElement(this XmlDocument doc)
        {
            XElement xElem = XElement.Parse(doc.OuterXml);
            return xElem;
        }

        /// <summary>
        /// XmlNode转XElement
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static XElement ToXElement(this XmlNode node)
        {
            XElement xElem = XElement.Parse(node.OuterXml);
            return xElem;
        }

        /// <summary>
        /// XElement转字符串
        /// </summary>
        /// <param name="xElem"></param>
        /// <returns></returns>
        public static string ToMyString(this XElement xElem)
        {
            return xElem.ToString();
        }

        /// <summary>
        /// XDocument转字符串
        /// </summary>
        /// <param name="xElem"></param>
        /// <returns></returns>
        public static string ToMyString(this XDocument xDoc)
        {
            return xDoc.ToString();
        }
    }
}
