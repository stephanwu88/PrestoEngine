using System;
using System.Windows;
using System.ComponentModel;
using System.Threading;

namespace Engine.Util
{
    /// <summary>
    /// winLoading.xaml 的交互逻辑
    /// </summary>
    public partial class winLoading : Window
    {
        public BackgroundWorker backgroundWorker1;
        private DateTime LoadTime = new DateTime(); 

        public winLoading(string strTitle= "查询样品")
        {
            InitializeComponent();
            _Title.Content = strTitle;
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DateTime UnLoadTime = DateTime.Now;
            double secSpan = (UnLoadTime - LoadTime).TotalSeconds;
            if (secSpan < 1)
                Thread.Sleep(1000);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(!this.backgroundWorker1.IsBusy)
            {
                LoadTime = DateTime.Now;
                this.backgroundWorker1.RunWorkerAsync();
            }
        }

        public void SetProgressText(string desc)
        {    
           
            //this.message.Content = desc;

            this.Dispatcher.Invoke(new Action(() =>
                {
                    this.message.Content = desc;
                }));

            //new Thread(() =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        this.message.Content = desc;
            //    }));
            //}).Start();

        }
    }
}
