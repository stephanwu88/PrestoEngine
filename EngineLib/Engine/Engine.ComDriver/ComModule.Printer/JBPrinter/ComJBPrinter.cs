using Engine.Common;
using Engine.Mod;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Engine.ComDriver.JiaBo
{
    //调用示例：
    //LabelPrinter.prn 文件内容
    ////热敏标签机配置/////
    //[宝武特冶快分标签机]
    //BarCode | 80,20,128,80,0,0,1.5,1.5,{0}
    //WindowsFont | 30,120,24,0,0,0,楷体,样品编号：{0}
    //WindowsFont | 30,150,24,0,0,0,楷体,钢种牌号：{1}
    //WindowsFont | 30,180,24,0,0,0,楷体,样品来源：{2}
    //WindowsFont | 30,210,24,0,0,0,楷体,打印时间：{3}
    //try
    //{
    //    //PrintLabel(string PrnFile, string[] FieldContent, int CopyCount = 1)
    //    string strFile = sCommon.GetStartUpPath() + @"\Locales\LabelPrinter.prn";
    //    new ComJBPrinter("Gprinter_SiloSpot").PrintLabel(strFile, new string[] { "text1", "text2", "text3" }, 3);
    //}
    //catch (Exception ex)
    //{
    //    sCommon.MyMsgBox(ex.Message);
    //}

    /// <summary>
    /// 接口实现
    /// </summary>
    public partial class ComJBPrinter : IComPrinter
    {
        /// <summary>
        /// 打印标签 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <returns></returns>
        public bool PrintLabel(string PrnFile, string[] FieldContent)
        {
            return PrintLabel(PrnFile, FieldContent, 1);
        }

        /// <summary>
        /// 打印标签 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <param name="CopyCount">打印份数</param>
        /// <returns></returns>
        public bool PrintLabel(string PrnFile, string[] FieldContent, int CopyCount = 1)
        {
            //WindowsFont | 20 - 40 - 24 - 0 - 0 - 0 - 楷体 - 样品种类：新区混匀矿
            //BarCode | 45 - 80 - 128 - 80 - 1 - 0 - 1.5 - 1.5 -{ 1}
            //WindowsFont | 20 - 40 - 24 - 0 - 0 - 0 - 楷体 -{ 2}
            //WindowsFont | 20 - 40 - 24 - 0 - 0 - 0 - 楷体 -{ 3}
            if (!File.Exists(PrnFile))
                throw new Exception(string.Format("打印机【{0}】模板文件未找到", DriverName));
            string strFileContent = File.ReadAllText(PrnFile);
            if (strFileContent.MatchParamCount() > FieldContent.Length)
                throw new Exception(string.Format("打印机【{0}】内容设置错误", DriverName));
            if (CopyCount < 1) CopyCount = 1;
            strFileContent = string.Format(strFileContent, FieldContent);
            List<object> LstObj = new List<object>();
            List<string> LstLine = strFileContent.MySplit("\r\n");
            foreach (string item in LstLine)
            {
                string strLine = item.MidString("", "//");
                string dType = strLine.MidString("", "|").Trim();
                string dParam = strLine.MidString("|", "").Trim();
                if (dType == "WindowsFont") LstObj.Add(WindowsFont.Parse(dParam));
                else if (dType == "BarCode") LstObj.Add(BarCode.Parse(dParam));
            }
            if (LstObj.Count == 0) throw new Exception(string.Format("打印机【{0}】内容设置错误", DriverName));
            return PrintLabel(LstObj, CopyCount);
        }

        /// <summary>
        /// 打印标签 - 代码排版
        /// </summary>
        /// <param name="LstItem">排版列表</param>
        /// <param name="CopyCount">打印份数</param>
        /// <returns></returns>
        public bool PrintLabel(List<object> LstItem, int CopyCount = 1)
        {
            try
            {
                SetUp(new PrintSet());
                clearbuffer();
                if (CopyCount < 1) CopyCount = 1;
                //windowsfont(20, 40, 24, 0, 0, 0, "楷体", windowsFont.Content);
                //barcode("45", "80", "128", "80", "1", "0", "1.5", "1.5", barCode.Code);
                foreach (object item in LstItem)
                {
                    if (item is WindowsFont font)
                    {
                        windowsfont(font.PointX, font.PointY, font.FontHeight,
                            font.Rotation, font.FontStyle, font.FontUnderline, font.FontName, font.Content);
                    }
                    else if (item is BarCode bar)
                    {
                        barcode(bar.PointX, bar.PointY, bar.CharCodeType,
                            bar.Height, bar.Readable, bar.Rotation, bar.Narrow, bar.Wide, bar.Code);
                    }
                }
                printlabel("1", CopyCount.ToString());
                closeport();
            }
            catch (Exception ex)
            {
                Logger.Error.Write(LOG_TYPE.ERROR, string.Format("【{0}】热敏标签机打印时发生错误 - {1}",
                    DriverName.ToMyString(), ex.Message));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 打印示例
        /// </summary>
        public void PrintDemo1()
        {
            List<object> List = new List<object>();
            WindowsFont font = new WindowsFont() { PointX = 20, PointY = 40, Content = "样品种类:新区混匀矿" };
            List.Add(font);
            font = new WindowsFont() { PointX = 20, PointY = 90, Content = "样品编号:" };
            List.Add(font);
            font = new WindowsFont() { PointX = 20, PointY = 140, Content = "text3" };
            List.Add(font);
            font = new WindowsFont()
            {
                PointX = 20,
                PointY = 190,
                Content = string.Format("打印时间:{0}", DateTime.Now.ToString("HH:mm:ss"))
            };
            List.Add(font);
            BarCode bar = new BarCode() { PointX = "45", PointY = "80", Code = "123123123" };
            List.Add(bar);
            PrintLabel(List, 4);
        }
    }

    /// <summary>
    /// 山东佳博热敏标签机
    /// </summary>
    public partial class ComJBPrinter
    {
        public ComJBPrinter(string driverName)
        {
            DriverName = driverName;
        }
        /// <summary>
        /// 驱动名称
        /// </summary>
        public string DriverName { get; private set; }
        /// <summary>
        /// 标签机设置
        /// </summary>
        public PrintSet PrintSet { get; set; }

        /// <summary>
        /// 配置打印机
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public bool SetUp(PrintSet set)
        {
            openport(DriverName);
            setup(set.Width, set.Height, set.Speed, set.Thickness,
                set.SensorType, set.MarkHeight, set.MarkOffset);
            return true;
        }
    }

    /// <summary>
    /// GP-3150TIN 
    /// 山东佳博热敏标签机
    /// 函数声明
    /// </summary>
    public partial class ComJBPrinter
    {
        [DllImport("TSCLIB.dll", EntryPoint = "about")]
        public static extern int about();

        [DllImport("TSCLIB.dll", EntryPoint = "openport")]
        public static extern int openport(string printername);

        [DllImport("TSCLIB.dll", EntryPoint = "barcode")]
        public static extern int barcode(string x, string y, string type,
                    string height, string readable, string rotation,
                    string narrow, string wide, string code);

        [DllImport("TSCLIB.dll", EntryPoint = "clearbuffer")]
        public static extern int clearbuffer();

        [DllImport("TSCLIB.dll", EntryPoint = "closeport")]
        public static extern int closeport();

        [DllImport("TSCLIB.dll", EntryPoint = "downloadpcx")]
        public static extern int downloadpcx(string filename, string image_name);

        [DllImport("TSCLIB.dll", EntryPoint = "formfeed")]
        public static extern int formfeed();

        [DllImport("TSCLIB.dll", EntryPoint = "nobackfeed")]
        public static extern int nobackfeed();

        [DllImport("TSCLIB.dll", EntryPoint = "printerfont")]
        public static extern int printerfont(string x, string y, string fonttype,
                        string rotation, string xmul, string ymul,
                        string text);

        [DllImport("TSCLIB.dll", EntryPoint = "printlabel")]
        public static extern int printlabel(string set, string copy);

        [DllImport("TSCLIB.dll", EntryPoint = "sendcommand")]
        public static extern int sendcommand(string printercommand);

        [DllImport("TSCLIB.dll", EntryPoint = "setup")]
        public static extern int setup(string width, string height,
                        string speed, string density,
                        string sensor, string vertical,
                        string offset);

        [DllImport("TSCLIB.dll", EntryPoint = "windowsfont")]
        public static extern int windowsfont(int x, int y, int fontheight,
                        int rotation, int fontstyle, int fontunderline,
                        string szFaceName, string content);
    }
}
