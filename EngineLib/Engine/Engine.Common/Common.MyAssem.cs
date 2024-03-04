using System.Diagnostics;
using System.Reflection;

namespace Engine.Common
{
    #region 程序集对象说明
    //当前执行的代码的程序集，代码在dll中为 Engine.dll，代码在app中为 app.exe
    //Assembly ass1 = Assembly.GetExecutingAssembly();
    //调用者程序集，代码在dll中为 Engine.dll  
    //代码中app中为 file:///C:/WINDOWS/Microsoft.Net/assembly/GAC_32/PresentationCore/v4.0_4.0.0.0__31bf3856ad364e35/PresentationCore.dll
    //Assembly ass2 = Assembly.GetCallingAssembly();
    //程序入口程序集 app.exe
    //Assembly ass3 = Assembly.GetEntryAssembly();
    #endregion

    /// <summary>
    /// Default：加载当前执行程序的程序集文件的内容  exe
    /// Executing：当前执行的代码的程序集            engine.dll
    /// </summary>
    public class MyAssembly
    {
        private FileVersionInfo VersionInfo;
        public Assembly AssemblyInfo;

        /// <summary>
        /// Default：加载当前执行程序的程序集文件的内容
        /// Executing：当前执行的代码的程序集
        /// </summary>
        /// <param name="IsDefaultAssembly">是否选择默认程序集</param>
        public MyAssembly(bool IsDefaultAssembly = true)
        {

            if (IsDefaultAssembly)
            {
                // Default：加载当前执行程序的程序集文件的内容
                //string fileName = System.Windows.Forms.Application.ExecutablePath;
                //VersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(fileName);
                //AssemblyInfo = Assembly.LoadFrom(fileName);
                AssemblyInfo = Assembly.GetEntryAssembly();
                string fileName = AssemblyInfo.Location;
                VersionInfo = FileVersionInfo.GetVersionInfo(fileName);
            }
            else
            {
                // Executing：当前执行的代码的程序集
                AssemblyInfo = Assembly.GetExecutingAssembly();
                string fileName = AssemblyInfo.Location;
                VersionInfo = FileVersionInfo.GetVersionInfo(fileName);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="assemblyFile">程序集名称</param>
        public MyAssembly(string assemblyFile)
        {
            string fileName = System.Windows.Forms.Application.ExecutablePath;
            VersionInfo = FileVersionInfo.GetVersionInfo(fileName);
            AssemblyInfo = Assembly.LoadFrom(fileName);
        }

        /// <summary>
        /// Default：加载当前执行程序的程序集文件的内容 exe程序集
        /// </summary>
        public static MyAssembly Default
        {
            get
            {
                if (defaultInstance == null)
                    defaultInstance = new MyAssembly();
                return defaultInstance;
            }
        }
        private static MyAssembly defaultInstance;

        /// <summary>
        /// Executing：当前执行的代码的程序集 - engine程序集
        /// </summary>
        public static MyAssembly Excuting
        {
            get
            {
                if (excutingInstance == null)
                    excutingInstance = new MyAssembly(false);
                return excutingInstance;
            }
        }
        private static MyAssembly excutingInstance;

        /// <summary>
        /// 运行文件的文件名称
        /// </summary>
        public string FileFullName { get => VersionInfo.FileName; }
        /// <summary>
        /// 详细信息-原始文件名
        /// </summary>
        public string OriginalFileName { get => VersionInfo.OriginalFilename; }
        /// <summary>
        /// 程序集文件位置(fileName含文件名称)
        /// </summary>
        public string Location { get => AssemblyInfo.Location; }
        /// <summary>
        /// 程序集名称
        /// 与原始文件名同
        /// </summary>
        public string AssemblyName
        {
            get
            {
                AssemblyName assemblyName = AssemblyInfo.GetName();
                return assemblyName.Name;
            }
        }
        /// <summary>
        /// 程序集名称(不带文件后缀)
        /// </summary>
        public string AssemblyNameWithoutExtension
        {
            get => AssemblyName.MidString("",".");
        }
        /// <summary>
        /// 程序集PublicKey
        /// </summary>
        public string PublicKey
        {
            get => AssemblyInfo.GetName().GetPublicKey().ByteArrayToHexString("{0:x2}");
        }
        /// <summary>
        /// 程序集PublicKeyToken
        /// </summary>
        public string PublicKeyToken
        {
            get => AssemblyInfo.FullName.MidString("PublicKeyToken=", "").Replace("null", "");
        }
        /// <summary>
        /// 程序集信息-文件版本 
        /// 文件属性-详细信息-文件版本 
        /// 提示窗口-文件版本
        /// 与ProductVersion相同
        /// </summary>
        public string FileVersion { get => VersionInfo.FileVersion; }
        /// <summary>
        /// 程序集信息-文件版本 
        /// 文件属性-详细信息-文件版本 
        /// 提示窗口-文件版本
        /// 与FileVersion相同
        /// </summary>
        public string ProductVersion { get => VersionInfo.ProductVersion; }
        /// <summary>
        /// 程序集信息-程序集版本 
        /// </summary>
        public string AssemblyVersion
        {
            get
            {
                AssemblyName assemblyName = AssemblyInfo.GetName();
                return assemblyName.Version.ToString();
            }
        }
        /// <summary>
        /// 程序集信息-标题 
        /// 文件属性-详细信息-文件说明   
        /// 文件提示窗口-文件说明
        /// </summary>
        public string FileTitle { get => VersionInfo.FileDescription; }
        /// <summary>
        /// 程序集信息-说明
        /// </summary>
        public string FileDescription
        {
            get
            {
                AssemblyDescriptionAttribute DescAttr = (AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(AssemblyInfo, typeof(AssemblyDescriptionAttribute));
                return DescAttr.Description;
            }
        }
        /// <summary>
        /// 程序集信息-产品 
        /// 文件属性-详细信息-产品名称
        /// </summary>
        public string ProductName { get => VersionInfo.ProductName; }
        /// <summary>
        /// 程序集信息-公司 
        /// 提示窗口-公司
        /// </summary>
        public string CompanyName { get => VersionInfo.CompanyName; }
        /// <summary>
        /// 程序集信息-文件版本-1-主版本号
        /// </summary>
        public int ProductMajorPart { get => VersionInfo.ProductMajorPart; }
        /// <summary>
        /// 程序集信息-文件版本-2-次版本号
        /// </summary>
        public int ProductMinorPart { get => VersionInfo.ProductMinorPart; }
        /// <summary>
        /// 程序集信息-文件版本-3-生成号
        /// </summary>
        public int ProductBuildPart { get => VersionInfo.ProductBuildPart; }
        /// <summary>
        /// 程序集信息-文件版本-4-专用部件号
        /// </summary>
        public int ProductPrivatePart { get => VersionInfo.ProductPrivatePart; }
        /// <summary>
        /// 程序集信息-非特定语言 详细信息-语言(中文一般显示【语言中性】)
        /// </summary>
        public string Language { get => VersionInfo.Language; }
        /// <summary>
        /// //程序集信息-版权 详细信息-版权
        /// </summary>
        public string LegalCopyright { get => VersionInfo.LegalCopyright; }

        /// <summary>
        /// 验证程序集调用者公钥是否与自己一致 使用相同snk
        /// </summary>
        /// <returns></returns>
        internal bool ValidateUser()
        {
            //获取执行App程序的FullName -- HeaoIPS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=52b73f80d00d732b
            string ExeFullName = Assembly.GetEntryAssembly().GetName().FullName;
            string ExePublicKey = Assembly.GetEntryAssembly().GetName().GetPublicKey().ByteArrayToHexString("{0:x2}");
            string ExePublicKeyToken = Assembly.GetEntryAssembly().GetName().GetPublicKeyToken().ByteArrayToHexString("{0:x2}");

            //获取Engine.dll文件  -- PublicKey
            string fileName = Assembly.GetCallingAssembly().Location;
            Assembly AssemblyInfo = Assembly.LoadFrom(fileName);
            string EngineFileFullName = AssemblyInfo.GetName().FullName;
            string EngineFilePublicKey = AssemblyInfo.GetName().GetPublicKey().ByteArrayToHexString("{0:x2}");
            string EngineFilePublicKeyToken = AssemblyInfo.GetName().GetPublicKeyToken().ByteArrayToHexString("{0:x2}");
            return ExePublicKeyToken == EngineFilePublicKeyToken;
        }
    }
}
