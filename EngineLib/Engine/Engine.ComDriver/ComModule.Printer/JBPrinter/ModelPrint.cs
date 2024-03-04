
using Engine.Common;
using System.Collections.Generic;

namespace Engine.ComDriver
{
    /// <summary>
    /// 打印设置
    /// </summary>
    public class PrintSet
    {
        /// <summary>
        /// 标签宽度 mm
        /// </summary>
        public string Width { get; set; } = "40";
        /// <summary>
        /// 标签高度 mm
        /// </summary>
        public string Height { get; set; } = "30";
        /// <summary>
        /// 打印速度 每秒/英寸
        /// 1.0 - 1.5 - 2.0 - 3.0 - 4.0 - 5.0 - 6.0
        /// </summary>
        public string Speed { get; set; } = "4.0";
        /// <summary>
        /// 打印浓度 0-15
        /// </summary>
        public string Thickness { get; set; } = "7";
        /// <summary>
        /// 传感器检测类型
        /// 0： 使用垂直间距传感器
        /// 1： 使用黑标触感器
        /// </summary>
        public string SensorType { get; set; } = "0";
        /// <summary>
        /// 垂直间距高度 mm
        /// </summary>
        public string MarkHeight { get; set; } = "2";
        /// <summary>
        /// 偏移距离 mm
        /// </summary>
        public string MarkOffset { get; set; } = "0";
    }

    /// <summary>
    /// 条形码 一维码
    /// </summary>
    public class BarCode
    {
        /// <summary>
        /// X方向起始点  Point  
        /// 200 DPI，1 点=1/8 mm, 300 DPI，1 点=1/12 mm
        /// </summary>
        public string PointX { get; set; } = "20";
        /// <summary>
        /// Y方向起始点  Point  
        /// 200 DPI，1 点=1/8 mm, 300 DPI，1 点=1/12 mm
        /// </summary>
        public string PointY { get; set; } = "40";
        /// <summary>
        /// 字符串编码类型
        /// 128 - 128M ..
        /// </summary>
        public string CharCodeType { get; set; } = "128";
        /// <summary>
        /// 条形码高度，高度单位:点
        /// </summary>
        public string Height { get; set; } = "80";
        /// <summary>
        /// 0: 不打印码文  1：打印码文
        /// </summary>
        public string Readable { get; set; } = "1";
        /// <summary>
        /// 旋转角度  0 -90 - 180 - 270
        /// </summary>
        public string Rotation { get; set; } = "0";
        /// <summary>
        /// 比例因子
        /// </summary>
        public string Narrow { get; set; } = "1.5";
        /// <summary>
        /// 比例因子
        /// </summary>
        public string Wide { get; set; } = "1.5";
        /// <summary>
        /// 条码内容
        /// </summary>
        public string Code { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static BarCode Parse(string Source)
        {
            BarCode barCode = new BarCode();
            List<string> Lst = Source.MySplit(",");
            if (Lst.Count == 9)
            {
                if (Lst[0].ToMyInt() > 0) barCode.PointX = Lst[0];
                if (Lst[1].ToMyInt() > 0) barCode.PointY = Lst[1];
                barCode.CharCodeType = Lst[2]; 
                if (Lst[3].ToMyInt() > 0) barCode.Height = Lst[3];
                if (Lst[4].ToMyInt() >= 0 && Lst[4].ToMyInt() <= 1) barCode.Readable = Lst[4];
                if (Lst[5].ToMyInt() > 0 && Lst[5].ToMyInt() < 360) barCode.Rotation = Lst[5];
                if (Lst[6].ToMyDouble() > 0) barCode.Narrow = Lst[6];
                if (Lst[7].ToMyDouble() > 0) barCode.Wide = Lst[7];
                barCode.Code = Lst[8];
            }
            return barCode;
        }
    }

    /// <summary>
    /// 使用Windows TTF 字型打印文字
    /// </summary>
    public class WindowsFont
    {
        /// <summary>
        /// X方向起始点  Point  
        /// </summary>
        public int PointX { get; set; }
        /// <summary>
        /// Y方向起始点  Point  
        /// </summary>
        public int PointY { get; set; }
        /// <summary>
        /// 字体高度  Point  
        /// </summary>
        public int FontHeight { get; set; } = 24;
        /// <summary>
        /// 旋转角度 0 -90 - 180 - 270
        /// </summary>
        public int Rotation { get; set; } = 0;
        /// <summary>
        /// 0-> 标准(Normal) 1-> 斜体(Italic) 2-> 粗体 Bold  3-> 粗斜体 Bold and Italic
        /// </summary>
        public int FontStyle { get; set; } = 0;
        /// <summary>
        /// 下划线 0 1
        /// </summary>
        public int FontUnderline { get; set; } = 0;
        /// <summary>
        /// 字体名称 ex: Arial, Times new Roman, 细名体, 标楷体
        /// </summary>
        public string FontName { get; set; } = "楷体";
        /// <summary>
        /// 打印内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 20,40,24,0,0,0,楷体,样品种类：新区混匀矿
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static WindowsFont Parse(string Source)
        {
            WindowsFont winFont = new WindowsFont();
            List<string> Lst = Source.MySplit(",");
            if (Lst.Count == 8)
            {
                if (Lst[0].ToMyInt() > 0) winFont.PointX = Lst[0].ToMyInt();
                if (Lst[1].ToMyInt() > 0) winFont.PointY = Lst[1].ToMyInt();
                if (Lst[2].ToMyInt() > 0) winFont.FontHeight = Lst[2].ToMyInt();
                if (Lst[3].ToMyInt() > 0) winFont.Rotation = Lst[3].ToMyInt();
                if (Lst[4].ToMyInt() >= 0 && Lst[4].ToMyInt() <= 3) winFont.FontStyle = Lst[4].ToMyInt();
                if (Lst[5].ToMyInt() >= 0 && Lst[5].ToMyInt() <= 1) winFont.FontUnderline = Lst[5].ToMyInt();
                if (!string.IsNullOrEmpty(Lst[6]) && Lst[6].ToMyInt() == 0) winFont.FontName = Lst[6];
                winFont.Content = Lst[7];
            }
            return winFont;
        }
    }
}
