using Engine.Data.DBFAC;

namespace Engine
{
    [Table(Name = "app_window", Comments = "窗体信息表")]
    public class ModelWindow
    {
        [Column(Name = "ID", PK = true, AI = true, Comments = "索引")]
        public string ID { get; set; }

        [Column(Name = "WindowType", Comments = "窗体类型")]
        public string WindowType { get; set; }

        [Column(Name = "WindowKey", Comments = "窗体键名")]
        public string WindowKey { get; set; }

        [Column(Name = "Left", Comments = "左侧位置")]
        public double Left { get; set; }

        [Column(Name = "Top", Comments = "顶端位置")]
        public double Top { get; set; }

        [Column(Name = "Height", Comments = "窗体高度")]
        public double Height { get; set; }

        [Column(Name = "Width", Comments = "窗体宽度")]
        public double Width { get; set; }

        public ModelWindow()
        {
            Left = SystemDefault.InValidDouble;
            Top = SystemDefault.InValidDouble;
            Height = SystemDefault.InValidDouble;
            Width = SystemDefault.InValidDouble;
        }
    }
}
