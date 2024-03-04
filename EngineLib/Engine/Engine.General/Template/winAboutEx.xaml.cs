using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Engine.Common;

namespace Engine.Template
{
    /// <summary>
    /// winAboutEx.xaml 的交互逻辑
    /// </summary>
    public partial class winAboutEx : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public winAboutEx()
        {
            InitializeComponent();
            LoadAssemInfo();
            //LoadViewFromUri(this, "/HeaoKeygenEx;component/winaboutex.xaml");
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="SoftIconBitMap">
        /// 指定软件图标
        /// new BitmapImage(new Uri("/Image/my.jpg",UriKind.Relative))
        /// </param>
        public winAboutEx(BitmapImage SoftIconBitMap)
        {
            InitializeComponent();
            LoadAssemInfo();
            try
            {
                if (SoftIconBitMap != null)
                {
                    _SoftIcon.Source = SoftIconBitMap;
                    GC.Collect();
                }
            }
            catch (Exception) {   }
        }

        /// <summary>
        /// 加载程序集信息
        /// </summary>
        private void LoadAssemInfo()
        {
            MyAssembly Assem = MyAssembly.Default;
            this.Title = string.Format("About {0}", Assem.AssemblyNameWithoutExtension);
            this._DevelopBy.Text = Assem.FileDescription;
            this._Copyright.Text = Assem.LegalCopyright;
            this._SoftVersion.Text = string.Format("{0} Soft {1}", Assem.AssemblyNameWithoutExtension, Assem.AssemblyVersion);
            _ProductSn.Text = GetProductSn();
            _ProductEdition.Text = "";
        }

        /// <summary>
        /// 获取产品序列号
        /// </summary>
        /// <returns></returns>
        private string GetProductSn()
        {
            return "";
        }

        //public void LoadViewFromUri(Window window, string baseUri)
        //{
        //    try
        //    {
        //        var resourceLocater = new Uri(baseUri, UriKind.Relative);
        //        var exprCa = (PackagePart)typeof(Application).GetMethod("GetResourceOrContentPart", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { resourceLocater });
        //        var stream = exprCa.GetStream();
        //        var uri = new Uri((Uri)typeof(BaseUriHelper).GetProperty("PackAppBaseUri", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null), resourceLocater);
        //        var parserContext = new ParserContext()
        //        {
        //            BaseUri = uri
        //        };
        //        typeof(XamlReader).GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { stream, parserContext, window, true });
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }
}
