﻿using Engine.Data;

namespace Engine.WpfBase
{
    /// <summary> 日志实体类 </summary>
    public class Log : NotifyObject
    {
        private string _flag;
        /// <summary> 标识  </summary>
        public string Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                RaisePropertyChanged("Flag");
            }
        }

        private string _time;
        /// <summary> 时间  </summary>
        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                RaisePropertyChanged("Time");
            }
        }

        private string _title;
        /// <summary> 标题  </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string _message;
        /// <summary> 日志信息  </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

    }
}
