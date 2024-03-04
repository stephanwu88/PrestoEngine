using Engine.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Engine.WpfControl
{
    /// <summary>
    /// 自定义带页面缓存的Frame控件
    /// </summary>
    public class CustomFrame : Frame
    {
        /// <summary>
        /// 加载时的默认页
        /// </summary>
        public Uri DefaultUri
        {
            get { return (Uri)GetValue(SourUriProperty); }
            set { SetValue(SourUriProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourUri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourUriProperty =
            DependencyProperty.Register("DefaultUri", typeof(Uri), typeof(CustomFrame), new PropertyMetadata(null, DefaultUriPropertyChanged));

        private static void DefaultUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomFrame _frame = d as CustomFrame;
            if (_frame.Content == null)
            {
                if (_frame.DefaultUri != null)
                {
                    _frame.Source = _frame.DefaultUri;
                    //_frame.Navigate(_frame.DefaultUri);
                }
            }
        }

        /// <summary>
        /// 页面缓存
        /// </summary>
        public Dictionary<Uri, object> PageCache { get; protected set; } = new Dictionary<Uri, object>();

        /// <summary>
        /// 根据Type类型存储页
        /// </summary>
        public Dictionary<string, object> PageTypedCache { get; protected set; } = new Dictionary<string, object>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomFrame()
        {
            AddEventHandler();
            Loaded += (s, e) =>
              {
                  PageNavigater.Default.AddFrame(this);
              };
        }

        /// <summary>
        /// 添加事件响应
        /// </summary>
        private void AddEventHandler()
        {
            //Step1. 导航开始 定位页面 
            // 使用Uri导航从此开始, 实例导航的不经过这儿，直接到OnChanged
            Navigating += (s, e) =>
            {
                Uri currentUri = e.Uri;
                if (currentUri == null)
                    return;
                // 检查页面是否已缓存
                if (PageCache.ContainsKey(currentUri))
                {
                    // 若已缓存，则直接使用缓存的页面,这里会跳过构造过程                    
                    Content = PageCache[currentUri];
                    e.Cancel = true;
                }
            };

            //此时窗体进入构造函数

            //Step2. 窗体构造过程  
            //这个根据页面大小加载过程分页会触发多次  
            //使用缓存页面时跳过构造过程
            NavigationProgress += (s, e) =>
            {
                //Console.WriteLine($"加载进度：{e.BytesRead} {e.MaxBytes}");
            };

            //窗体构造结束

            //Step3. 导航完成
            Navigated += (s, e) =>
            {
                //e.Content is PageMainMenu uPage
                if (e.Content != null && e.ExtraData != null && e.ExtraData?.GetType() == typeof(MessageStruct))
                {

                }
            };

            //Step4. 加载
            //LoadCompleted += (s, e) =>
            //{
                
            //};

            //终止导航
            //NavigationStopped += (s, e) =>
            //{
               
            //};
        }

        //Step2. 构造过程Content切换 不论Uri导航还是实例导航，内容切换都经过这儿
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            Type typed = newContent?.GetType();
            if (typed != null)
            {
                string strType = typed.ToMyString();
                if (!PageTypedCache.ContainsKey(strType))
                    PageTypedCache.Add(strType, newContent);
            }
            Uri currentUri = this.CurrentSource;
            if (currentUri != null)
            {
                // Uri导航  若未缓存，则将页面添加到缓存中
                if (!PageCache.ContainsKey(currentUri))
                {
                    PageCache.Add(currentUri, newContent);
                }
            }
        }
    }
}
