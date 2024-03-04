using Engine.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Engine.WpfControl
{
    /// <summary>
    /// TimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class TimePicker : UserControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TimePicker()
        {
            InitializeComponent();
            InitParameters();
        }

        /// <summary>
        /// 初始化参数设置
        /// </summary>
        private void InitParameters()
        {
            string strTimeNow = DateTime.Now.ToString("HH:mm:ss");
            _Hour.Background = Brushes.White;
            _Minite.Background = Brushes.White;
            _Second.Background = Brushes.White;
            _Hour.Text = strTimeNow.Split(':')[0];
            _Minite.Text = strTimeNow.Split(':')[1];
            _Second.Text = strTimeNow.Split(':')[2];
        }

        #region 事件处理过程
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            string strCmd = (sender as Button).Name;
            switch (strCmd)
            {
                case "CmdUp":
                    if (_Hour.Background == Brushes.Gray)
                    {
                        int temp = System.Int32.Parse(this._Hour.Text);
                        temp++;
                        if (temp > 24)
                        {
                            temp = 0;
                        }
                        _Hour.Text = temp.ToString();
                    }
                    else if (_Minite.Background == Brushes.Gray)
                    {
                        int temp = System.Int32.Parse(_Minite.Text);
                        temp++;
                        if (temp > 60)
                        {
                            temp = 0;
                        }
                        _Minite.Text = temp.ToString();
                    }
                    else if (_Second.Background == Brushes.Gray)
                    {
                        int temp = System.Int32.Parse(_Second.Text);
                        temp++;
                        if (temp > 60)
                        {
                            temp = 0;
                        }
                        _Second.Text = temp.ToString();
                    }
                    break;
                case "CmdDown":
                    if (_Hour.Background == Brushes.Gray)
                    {
                        int temp = System.Int32.Parse(_Hour.Text);
                        temp--;
                        if (temp < 0)
                        {
                            temp = 24;
                        }
                        _Hour.Text = temp.ToString();
                    }
                    else if (_Minite.Background == Brushes.Gray)
                    {
                        int temp = System.Int32.Parse(_Minite.Text);
                        temp--;
                        if (temp < 0)
                        {
                            temp = 60;
                        }
                        _Minite.Text = temp.ToString();
                    }
                    else if (_Second.Background == Brushes.Gray)
                    {
                        int temp = System.Int32.Parse(_Second.Text);
                        temp--;
                        if (temp < 0)
                        {
                            temp = 60;
                        }
                        _Second.Text = temp.ToString();
                    }
                    break;
            }
        }

        /// <summary>
        /// 更改选中状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextSelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                switch (tb.Name)
                {
                    case "textbox_hour":
                        //tb.Background = Brushes.Gray;
                        _Minite.Background = Brushes.White;
                        _Second.Background = Brushes.White;
                        break;
                    case "textbox_minute":
                        //tb.Background = Brushes.Gray;
                        _Hour.Background = Brushes.White;
                        _Second.Background = Brushes.White;
                        break;
                    case "textbox_second":
                        //tb.Background = Brushes.Gray;
                        _Hour.Background = Brushes.White;
                        _Minite.Background = Brushes.White;
                        break;
                }
            }
        }

        /// <summary>
        /// 数字标准化处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                try
                {
                    if ((this.isNum(tb.Text) == false) || (tb.Text.Length > 2))
                    {
                        tb.Text = "0";
                        return;
                    }
                }
                catch (Exception )
                {

                }
            }
        }

        /// <summary>
        /// 判断是否为数字，是--->true，否--->false
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool isNum(string str)
        {
            bool ret = true;
            foreach (char c in str)
            {
                if ((c < 48) || (c > 57))
                {
                    return false;
                }
            }

            return ret;
        }
        #endregion

        #region 属性接口
        /// <summary>
        /// 时间值
        /// </summary>
        public string Time
        {
            get
            {
                string strDateTime = string.Format("{0} {1}:{2}:{3}", DateTime.Now.ToString("yyyy/MM/dd"), _Hour.Text, _Minite.Text, _Second.Text);
                DateTime _DateTime = DateTime.Now;
                if (DateTime.TryParse(strDateTime, out _DateTime))
                    return _DateTime.ToString("HH:mm:ss");
                else
                    return "00:00:00";
            }
            set
            {
                string strDateTime = string.Format("{0} {1}", DateTime.Now.ToString("yyyy/MM/dd"), value);
                DateTime _DateTime = DateTime.Now;
                if (DateTime.TryParse(strDateTime, out _DateTime))
                {
                    string strTime = _DateTime.ToString("HH:mm:ss");
                    List<string> LstTime = strTime.MySplit(":");
                    if (LstTime.MyCount() == 3)
                    {
                        _Hour.Text = LstTime[0];
                        _Minite.Text = LstTime[1];
                        _Second.Text = LstTime[2];
                    }
                    else
                    {
                        _Hour.Text = "00";
                        _Minite.Text = "00";
                        _Second.Text = "00";
                    }
                }
                else
                {
                    _Hour.Text = _DateTime.Hour.ToString();
                    _Minite.Text = _DateTime.Minute.ToString();
                    _Second.Text = _DateTime.Second.ToString();
                }
            }
        }
        #endregion
    }
}
