using Engine.WpfBase;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: System.Windows.ThemeInfo(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("Engine")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("南京普瑞斯通科技有限公司")]
[assembly: AssemblyProduct("Engine")]
[assembly: AssemblyCopyright("Copyright © Presto 2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 会使此程序集中的类型
//对 COM 组件不可见。若需要从 COM 访问此程序集中的类型
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 若此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("238f5ade-5043-4d62-b20a-c464044986b8")]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
// 可以指定所有值，也可以使用以下所示的 "*" 预置版本号和修订号
//通过使用 "*"，如下所示:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyKeyFile(@".\snk\PrivateKey\heao.snk")]

[assembly: XmlnsDefinition("QQ:178876839", "Engine.WpfBase")]
[assembly: XmlnsDefinition("QQ:178876839", "Engine.WpfControl")]
//xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
//or: xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
[assembly: XmlnsPrefix("QQ:178876839", "h")]

//MVC 模块标记
[assembly: ModuleAttribute()]
