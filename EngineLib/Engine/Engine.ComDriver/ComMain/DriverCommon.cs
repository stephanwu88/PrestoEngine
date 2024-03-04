using Engine.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Engine.ComDriver
{
    public class DriverCommon
    {
        /// <summary>
        /// 创建网络类通讯实例,根据DriverItem提供不同通讯驱动库
        /// </summary>
        /// <typeparam name="TBufData">数据格式byte[] string</typeparam>
        /// <param name="DriverItem">通讯配置信息</param>
        /// <returns></returns>
        public static IComRuleNetCom<TBufData> CreateInstance<TBufData,TConNode>(DriverItem<NetworkCommParam> DriverItem) 
        {
            try
            {
                IComRuleNetCom<TBufData> ComRule = null;
                object ComObject = null;
                switch (DriverItem.DriverPtl.ToLower())
                {
                    case "":
                        ComObject = CreateInstance<TBufData, NetworkCommParam, TConNode>("Engine", "Engine.ComDriver.HEAO.sComHeaoHCP", DriverItem);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(DriverItem.Provider))
                        {
                            List<string> providerField = DriverItem.Provider.MySplit("|");
                            if (providerField.Count >= 2)
                                ComObject = CreateInstance<TBufData, NetworkCommParam, TConNode> (providerField[0], providerField[1], DriverItem);
                        }
                        break;
                }
                if (ComObject != null)
                    ComRule = ComObject as IComRuleNetCom<TBufData>;
                return ComRule != null ? ComRule : null;
            }
            catch (Exception ex)
            {
                sCommon.MyMsgBox(string.Format("通讯【{0}】实例化失败：\r\n\r\n{1}", DriverItem.DriverName.ToMyString(), ex.Message), MsgType.Error);
                sCommon.ExitEnvironment();
                return null;
            }
        }

        /// <summary>
        /// 创建串行类通讯实例,根据DriverItem提供不同通讯驱动库
        /// </summary>
        /// <typeparam name="TBufData">数据格式byte[] string</typeparam>
        /// <param name="DriverItem">通讯配置信息</param>
        public static IComRuleSerialCom<TBufData> CreateInstance<TBufData, TConNode>(DriverItem<SerialCommParam> DriverItem)
        {
            try
            {
                IComRuleSerialCom<TBufData> ComRule = null;
                object ComObject = null;
                switch (DriverItem.DriverPtl.ToLower())
                {
                    case "":
                        ComObject = CreateInstance<TBufData, SerialCommParam, TConNode>("Engine", "Engine.ComDriver.HEAO.sComHeaoHCP", DriverItem);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(DriverItem.Provider))
                        {
                            List<string> providerField = DriverItem.Provider.MySplit("|");
                            if (providerField.Count >= 2)
                                ComObject = CreateInstance<TBufData, SerialCommParam, TConNode>(providerField[0], providerField[1], DriverItem);
                        }
                        break;
                }
                return ComObject != null ? ComRule : null;
            }
            catch (Exception ex)
            {
                sCommon.MyMsgBox(string.Format("通讯【{0}】实例化失败：\r\n\r\n{1}", DriverItem.DriverName.ToMyString(), ex.Message), MsgType.Error);
                return null;
            }
        }

        /// <summary>
        /// 创建网络类通讯实例,根据DriverItem提供不同通讯驱动库
        /// </summary>
        /// <typeparam name="TBufData">数据格式byte[] string</typeparam>
        /// <param name="DriverItem">通讯配置信息</param>
        /// <returns></returns>
        public static IComDriverNetCom CreateInstance<TConNode>(DriverItem<NetworkCommParam> DriverItem)
        {
            try
            {
                IComDriverNetCom ComRule = null;
                object ComObject = null;
                switch (DriverItem.DriverPtl.ToUpper())
                {
                    case "":
                        ComObject = CreateInstance<NetworkCommParam, TConNode>("Engine", "Engine.ComDriver.Siemens.sComS7PLC", DriverItem);
                        break;
               
                    default:
                        if (!string.IsNullOrEmpty(DriverItem.Provider))
                        {
                            List<string> providerField = DriverItem.Provider.MySplit("|");
                            if (providerField.Count >= 2)
                                ComObject = CreateInstance<NetworkCommParam, TConNode>(providerField[0], providerField[1], DriverItem);
                        }
                        break;
                }
                if (ComObject != null)
                    ComRule = ComObject as IComDriverNetCom;
                return ComRule != null ? ComRule : null;
            }
            catch (Exception ex)
            {
                sCommon.MyMsgBox(string.Format("通讯【{0}】实例化失败：\r\n\r\n{1}", DriverItem.DriverName.ToMyString(), ex.Message), MsgType.Error);
                return null;
            }
        }

        /// <summary>
        /// 创建串行类通讯实例,根据DriverItem提供不同通讯驱动库
        /// </summary>
        /// <typeparam name="TBufData">数据格式byte[] string</typeparam>
        /// <param name="DriverItem">通讯配置信息</param>
        public static IComDriverSerialCom CreateInstance<TConNode>(DriverItem<SerialCommParam> DriverItem)
        {
            try
            {
                IComDriverSerialCom ComRule = null;
                object ComObject = null;
                switch (DriverItem.DriverPtl.ToLower())
                {
                    case "":
                        ComObject = CreateInstance<SerialCommParam, TConNode>("Engine", "Engine.ComDriver.HEAO.sComHeaoHCP", DriverItem);
                        break;
                    default:
                        if (!string.IsNullOrEmpty(DriverItem.Provider))
                        {
                            List<string> providerField = DriverItem.Provider.MySplit("|");
                            if (providerField.Count >= 2)
                                ComObject = CreateInstance<SerialCommParam, TConNode>(providerField[0], providerField[1], DriverItem);
                        }
                        break;
                }
                return ComObject != null ? ComRule : null;
            }
            catch (Exception ex)
            {
                sCommon.MyMsgBox(string.Format("通讯【{0}】实例化失败：\r\n\r\n{1}", DriverItem.DriverName.ToMyString(), ex.Message), MsgType.Error);
                return null;
            }
        }

        /// <summary>
        /// 实例化Socket通讯对象 
        /// </summary>
        /// <typeparam name="TBufData">数据格式byte[] string</typeparam>
        /// <typeparam name="TCommParam">通讯格式</typeparam>
        /// <param name="assemString">程序集名称</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="DriverItem">驱动信息</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static object CreateInstance<TBufData, TCommParam, TConNode>(string assemString, string typeName, DriverItem<TCommParam> DriverItem)
            where TCommParam : new()
        {
            try
            {
                Assembly assembly = Assembly.Load(assemString);
                Type type = assembly.GetType(typeName);
                if (typeName.EndsWith("`1"))
                    type = type.MakeGenericType(typeof(TConNode));
                object objCom = Activator.CreateInstance(type, DriverItem);
                return objCom;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 实例化PLC通讯对象
        /// </summary>
        /// <typeparam name="TCommParam">通讯格式</typeparam>
        /// <param name="assemString">程序集名称</param>
        /// <param name="typeName">类型名称</param>
        /// <param name="DriverItem">驱动信息</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static object CreateInstance<TComParam, TConNode>(string assemString, string typeName, DriverItem<TComParam> DriverItem)
            where TComParam : new()
        {
            try
            {
                Assembly assembly = Assembly.Load(assemString);
                Type type = assembly.GetType(typeName);
                if (typeName.EndsWith("`1"))
                    type = type.MakeGenericType(typeof(TConNode));
                object objCom = Activator.CreateInstance(type, DriverItem);
                return objCom;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
