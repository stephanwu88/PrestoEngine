using Engine.Common;
using Engine.Data.DBFAC;
using System;

namespace Engine.ComDriver
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ServerNodeAttribute : ModelAttribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ProjectStartUPAttribute : ModelAttribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DriverItemAttribute : ModelAttribute
    {

    }

    [Table(Name = "driver_comlink", Comments = "通讯连接状态表")]
    public class ModelComLink 
    {
        //[Column(Name = "ID", AI = true, PK = true, Comments = "索引")]
        //public string ID { get; set; }

        [Column(Name = "AppName", Comments = "应用程序名称")]
        public string AppName { get; set; } = SystemDefault.AppName;

        [Column(Name = "DriverName", Comments = "通讯名称")]
        [ServerNode(Name = "ServerName", Comments ="服务名称")]
        [DriverItem(Name = "Name", Comments = "通讯驱动名称")]
        public string DriverName { get; set; }

        [Column(Name = "ComLink", Comments = "通讯连接类型")]
        [DriverItem(Name = "ComLink", Comments = "通讯连接类型")]
        public string ComLink { get; set; }

        [Column(Name = "ComIP", Comments = "通讯IP")]
        [ServerNode(Name = "ServerIP", Comments = "服务器IP")]
        [DriverItem(Name = "ComIP", Comments = "通讯IP")]
        public string ComIP { get; set; }

        [Column(Name = "ComPort", Comments = "通讯端口")]
        [ServerNode(Name = "ServerPort", Comments = "服务器端口")]
        [DriverItem(Name = "ComPort", Comments = "通讯端口")]
        public int ComPort { get; set; }

        [Column(Name = "Comment", Comments = "通讯描述")]
        [ProjectStartUP(Name = "Remark", Comments = "描述")]
        [DriverItem(Name = "Comment", Comments = "通讯描述")]
        public string Comment { get; set; }

        [Column(Name = "ServerStatus", Comments = "端口服务状态")]
        public string ServerStatus { get; set; }

        [Column(Name = "LinkState", Comments = "连接状态")]
        public string LinkState { get; set; }

        public ModelComLink()
        {
            ComPort = SystemDefault.InValidInt;
        }
    }
}
