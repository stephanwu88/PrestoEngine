using Engine.WpfControl;
using System;
using System.Collections.Generic;

namespace Engine.Common
{
    /// <summary>
    /// 基于CustomFrame自动缓存页面导航
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 导航到指定Uri页面
        /// </summary>
        /// <param name="frame">页面导航内容控件</param>
        /// <param name="PageUri">页面Uri</param>
        public static void NavigateToPage(this CustomFrame frame, string PageUri)
        {
            //导航至指定页面
            frame.NavigateToPageWithExtraData(PageUri, null);
        }

        /// <summary>
        /// 导航到指定Uri页面
        /// </summary>
        /// <param name="frame">页面导航内容控件</param>
        /// <param name="PageUri">页面Uri</param>
        /// <param name="ExtraData">传递参数</param>
        public static void NavigateToPageWithExtraData(this CustomFrame frame, string PageUri, object ExtraData)
        {
            //创建Uri
            Uri uri = new Uri(PageUri, UriKind.RelativeOrAbsolute);
            //导航至指定页面
            frame.Navigate(uri, ExtraData);
        }

        /// <summary>
        /// 导航到指定Uri页面
        /// </summary>
        /// <param name="frame">页面导航内容控件</param>
        /// <param name="PageUri">页面Uri</param>
        /// <param name="ExtraData">传递参数</param>
        public static void NavigateToPageWithExtraData(this CustomFrame frame, Uri PageUri, object ExtraData)
        {
            //导航至指定页面
            frame.Navigate(PageUri, ExtraData);
        }

        /// <summary>
        /// 导航到自动创建的实例化页面
        /// </summary>
        /// <param name="frame">页面导航内容控件</param>
        /// <param name="InstanceString">实例化程序集类信息 ex: Engine.Automation | Engine.Automation.PageAnaConfig</param>
        /// <param name="Args">实例化参数</param>
        /// <returns>页面对象</returns>
        public static object NavigateToInstancePage(this CustomFrame frame, string InstanceString, params object[] Args)
        {
            List<string> LstInstanceField = InstanceString.MySplit("|");
            if (LstInstanceField.MyCount() != 2)
                return null;
            
            string  AssemblyeName = LstInstanceField[0].Trim();
            string TypeName = LstInstanceField[1].Trim();
            if (string.IsNullOrEmpty(AssemblyeName) || string.IsNullOrEmpty(TypeName) || frame == null)
            {
                throw new Exception($"定位页面{InstanceString}异常");
            }
            //实例页面缓存优先取用
            if (frame.PageTypedCache.ContainsKey(TypeName))
            {
                frame.NavigateToInstancePage(frame.PageTypedCache[TypeName]);
                return frame.PageTypedCache[TypeName];
            }
            //自动创建页面实例
            object Instance = CreateInstance(AssemblyeName, TypeName, Args);
            frame.NavigateToInstancePage(Instance);
            return Instance;
        }

        /// <summary>
        /// 根据泛型自动创建实例化页面
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="InstanceType"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public static object NavigateToInstancePage<T>(this CustomFrame frame, params object[] Args)
        {
            Type InstanceType = typeof(T);
            return frame.NavigateToInstancePage(InstanceType, Args);
        }

        /// <summary>
        /// 根据类型自动创建实例化页面
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="InstanceType"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public static object NavigateToInstancePage(this CustomFrame frame, Type InstanceType, params object[] Args)
        {
            string TypeName = InstanceType.ToMyString();
            //实例页面缓存优先取用
            if (frame.PageTypedCache.ContainsKey(TypeName))
            {
                frame.NavigateToInstancePage(frame.PageTypedCache[TypeName]);
                return frame.PageTypedCache[TypeName];
            }
            //自动创建页面实例
            object Instance = InstanceType.CreateInstance(Args);
            frame.NavigateToInstancePage(Instance);
            return Instance;
        }

        /// <summary>
        /// 导航到指定的实例化页面
        /// </summary>
        /// <param name="frame">页面导航内容控件</param>
        /// <param name="PageInstance">页面实例</param>
        public static void NavigateToInstancePage(this CustomFrame frame, object PageInstance)
        {
            if (PageInstance == null)
                return;
            //导航至指定页面
            frame.Navigate(PageInstance);
        }
    }

    /// <summary>
    /// 页面自动导航器
    /// </summary>
    public class PageNavigater
    {
        /// <summary>
        /// 传递构造参数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public delegate object[] DelegatePassConParameter(Type type);

        /// <summary>
        /// 
        /// </summary>
        public DelegatePassConParameter PassConstructParameter;

        /// <summary>
        /// 页面构造完成事件
        /// </summary>
        public event Action<object> PageConstruted;

        /// <summary>
        /// CustomFrame集合
        /// </summary>
        public Dictionary<string, CustomFrame> FrameDictionary = new Dictionary<string, CustomFrame>();

        /// <summary>
        /// 构造方法委托字典
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, Delegate>> _factories = new Dictionary<Type, Dictionary<string, Delegate>>();

        /// <summary>
        /// 默认实例主键
        /// </summary>
        private readonly string _defaultKey = SystemDefault.UUID;

        /// <summary>
        /// 定义委托 用于构造页面前获取构造参数
        /// </summary>
        /// <param name="AssemblyName"></param>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public delegate object[] ConstructionParameter(string AssemblyName, string TypeName);

        /// <summary>
        /// 默认实例
        /// </summary>
        public static PageNavigater Default
        {
            get
            {
                if (_Default == null)
                    _Default = new PageNavigater();
                return _Default;
            }
        }
        private static PageNavigater _Default;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public CustomFrame GetFrame(string Name)
        {
            return FrameDictionary.DictFieldValue(Name);
        }

        /// <summary>
        /// 添加到Frame集合
        /// </summary>
        /// <param name="Name">强命名名称</param>
        /// <param name="mFrame">Frame对象</param>
        public void AddFrame(string Name, CustomFrame mFrame)
        {
            FrameDictionary.AppandDict(Name, mFrame);
        }

        /// <summary>
        /// 添加到Frame集合
        /// </summary>
        /// <param name="mFrame">Frame对象</param>
        public void AddFrame(CustomFrame mFrame)
        {
            if (!string.IsNullOrEmpty(mFrame.Name))
                FrameDictionary.AppandDict(mFrame.Name, mFrame);
            else
                FrameDictionary.AppandDict(SystemDefault.UUID, mFrame);
        }

        /// <summary>
        /// 导航到指定页面
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="PageExpress">
        ///    Uri                ex:   /Views.SparkStandard/PageMainMenu.xaml
        /// or: 实例化程序集类信息 ex:  Engine.Automation | Engine.Automation.Sparker.PageAnaPgm
        /// </param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public void NavigateToPage(CustomFrame frame, string PageExpress, object[] Args = null)
        {            
            if (PageExpress.Contains("/") || PageExpress.Contains("\\") || PageExpress.MyContains(".xaml", StringMatchMode.IgnoreCase))
            {
                //Uri导航
                frame.NavigateToPage(PageExpress);
            }
            else
            {
                //构造函数参数委托
                if (Args == null)
                {
                    //查看有无委托 -- 也就是查看有无带参构造需求的依赖注册
                    List<string> LstInstanceField = PageExpress.MySplit("|");
                    string AssemblyeName = string.Empty;
                    string TypeName = string.Empty;
                    if (LstInstanceField.MyCount() == 2)
                    {
                        AssemblyeName = LstInstanceField[0].Trim();
                        TypeName = LstInstanceField[1].Trim();
                    }
                    if (string.IsNullOrEmpty(AssemblyeName) || string.IsNullOrEmpty(TypeName) || frame == null)
                    {
                        throw new Exception($"定位页面{PageExpress}异常");
                    }
                    Type type = sCommon.CreateType(AssemblyeName, TypeName);
                    if (_factories.ContainsKey(type))
                    {
                        //有指定委托方法专门处理构造
                        object objInstance = _factories[type][_defaultKey]?.DynamicInvoke();
                        frame.NavigateToInstancePage(objInstance);
                        return;
                    }
                    else
                    {
                        //有无委托传递构造参数
                        Args = PassConstructParameter?.Invoke(AssemblyeName.CreateType(TypeName));
                    }
                }
                //实例导航
                object page = frame.NavigateToInstancePage(PageExpress, Args);
                PageConstruted?.Invoke(page);
            }
        }

        /// <summary>
        /// 注册委托
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="factory"></param>
        /// <param name="key"></param>
        public void Register<TClass>(Func<TClass> factory, string key="") where TClass : class
        {
            Type classType = typeof(TClass);
            if (string.IsNullOrEmpty(key))
                key = _defaultKey;
            Dictionary<string, Delegate> list = new Dictionary<string, Delegate>
                {
                    {
                        key,
                        factory
                    }
                };
            _factories.Add(classType, list);
        }

        /// <summary>
        /// 注册委托到字典
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="classType"></param>
        /// <param name="factory"></param>
        /// <param name="key"></param>
        private void DoRegister<TClass>(Type classType, Func<TClass> factory, string key)
        {
            if (!this._factories.ContainsKey(classType))
            {
                Dictionary<string, Delegate> list = new Dictionary<string, Delegate>
                {
                    {
                        key,
                        factory
                    }
                };
                this._factories.Add(classType, list);
                return;
            }
            if (this._factories[classType].ContainsKey(key))
            {
                return;
            }
            this._factories[classType].Add(key, factory);
        }
    }
}
