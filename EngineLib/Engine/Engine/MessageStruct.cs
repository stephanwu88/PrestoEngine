
using System.Collections.Generic;

namespace Engine
{
    /// <summary>
    /// 消息传递结构
    /// </summary>
    public class MessageStruct
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MessageStruct()
        {

        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="data"></param>
        public MessageStruct(string content, List<object> data = null)
        {
            Content = content;
            Data = data;
        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public MessageStruct(object sender, List<object> data)
        {
            Sender = sender;
            Data = data;
        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public MessageStruct(object sender, string content, List<object> data)
        {
            Sender = sender;
            Content = content;
            Data = data;
        }

        public object Sender { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public List<object> Data { get; set; }
    }

    /// <summary>
    /// 消息传递泛型
    /// </summary>
    public class MessageGerneric<T> where T : new()
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MessageGerneric()
        {

        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="data"></param>
        public MessageGerneric(string content, T data)
        {
            Content = content;
            Data = data;
        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public MessageGerneric(object sender, T data)
        {
            Sender = sender;
            Data = data;
        }

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public MessageGerneric(object sender, string content, T data)
        {
            Sender = sender;
            Content = content;
            Data = data;
        }

        public object Sender { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public T Data { get; set; }
    }
}
