using Engine.Common;
using Engine.Data;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;           
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading;

namespace Engine.ComDriver.HGTECH
{
    /// <summary>
    /// 华工镭射头
    /// </summary>
    public partial class HGMarker : NotifyObject
    {
        private static readonly HGMarker _Instance =
            new HGMarker();

        public static object ObjectLock = new object();

        public event Action<object, string> StateChanged; 

        /// <summary>
        /// 当前模板
        /// </summary>
        public string CurrentModel { get; set; } = string.Empty;

        public static HGMarker Instance { get => _Instance; }

        public ImageSource EzdView { get; set; }
        
        /// <summary>
        /// 初始化完成
        /// </summary>
        public bool InitSuccessed { get; set; }

        /// <summary>
        /// 正在标刻
        /// </summary>
        public bool Marking { get; set; }

        /// <summary>
        /// 正在初始化
        /// </summary>
        public bool Initializing { get; set; }

        /// <summary>
        /// 红光开启
        /// </summary>
        public bool EnableRed { get; private set; }

        /// <summary>
        /// 最终错误信息
        /// </summary>
        public string LastError { get; set; }

        public LmcErrCode mLastError { get; set; }

        /// <summary>
        /// 激光头初始化
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public bool InitLaser(IntPtr hwnd)
        {
            Initializing = true;
            InitSuccessed = false;
            string strEzCadPath = sCommon.GetStartUpPath() + @"\";
            if (!File.Exists(strEzCadPath + "Markezd.dll"))
                return false;
            LmcErrCode code = LMC1_INITIAL(strEzCadPath, 0, hwnd);
            if (LmcErrCode.LMC1_ERR_SUCCESS == code)
                InitSuccessed = true;
            Initializing = false;
            this.RaisePropertyNotify();
            return InitSuccessed;
        }

        /// <summary>
        /// 加载EzCad模板文件
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="bDialog"></param>
        /// <returns></returns>
        public bool LoadEzdFile(string strFile)
        {
            if (!InitSuccessed)
            {
                LastError = LmcErrCode.LMC1_ERROR_NOINIT.ToString(); 
                return false;
            }
            if (!File.Exists(strFile))
            {
                LastError = LmcErrCode.LMC1_ERROR_NOEZDFILE.ToString();
                return false;
            }
            lock (ObjectLock)
            {
                LmcErrCode code = LMC1_LOADEZDFILE(strFile);
                if (LmcErrCode.LMC1_ERR_SUCCESS == code)
                {
                    CurrentModel = strFile;
                    return true;
                }
                LastError = code.ToString();
                return false;
            }
        }

        /// <summary>
        /// 标刻
        /// </summary>
        /// <param name="bFlay"></param>
        /// <returns></returns>
        public bool Mark(bool bFlay = false)
        {
            if (!InitSuccessed)
            {
                LastError = LmcErrCode.LMC1_ERROR_NOINIT.ToString();
                return false;
            }
            int nFly = 0;
            if (bFlay)
            {
                nFly = 1;
            }
            lock (ObjectLock)
            {
                Marking = true; this.RaisePropertyNotify();
                if (StateChanged != null)
                    StateChanged(this, "Marking");
                LmcErrCode code = LMC1_MARK(nFly);
                if (LmcErrCode.LMC1_ERR_SUCCESS == code)
                {
                    
                    Marking = false; this.RaisePropertyNotify();
                    if (StateChanged != null)
                        StateChanged(this, "MarkFinish");
                    return true;
                }
                LastError = code.ToString();
                return false;
            }
        }

        public bool ShowPreviewBmp(PictureBox pictureBox)
        {
            EventHandler method = null;
            int width;
            int height;
            if (!InitSuccessed)
            {
                LastError = "激光器初始化失败";
                return false;
            }
            else
            {
                try
                {
                    width = pictureBox.Size.Width;
                    height = pictureBox.Size.Height;
                    lock (ObjectLock)
                    {
                        if (pictureBox.InvokeRequired)
                        {
                            if (method == null)
                            {
                                method = delegate {
                                    IntPtr hbitmap = LMC1_GETPREVBITMAP2(width, height);
                                    pictureBox.Image = Image.FromHbitmap(hbitmap);
                                    DeleteObject(hbitmap);
                                };
                            }
                            pictureBox.Invoke(method);
                        }
                        else
                        {
                            IntPtr ptr = LMC1_GETPREVBITMAP2(width, height);
                            pictureBox.Image = Image.FromHbitmap(ptr);
                            DeleteObject(ptr);
                        }
                    }
                }
                catch (Exception)
                {

                }   
            }
            return true;
        }

        public bool WpfShowPreviewBmp(System.Windows.Controls.Image pictureBox)
        {
            if (!InitSuccessed)
            {
                LastError = "激光器初始化失败";
                return false;
            }
            else
            {
                try
                {
                    int width = (int)pictureBox.ActualWidth;
                    int height = (int)pictureBox.ActualHeight;
                    lock (ObjectLock)
                    {
                        IntPtr hbitmap = LMC1_GETPREVBITMAP2(width, height);
                        Bitmap bMap = Image.FromHbitmap(hbitmap);
                        //ImageSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        //    hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        ImageSource bitmapSource = ChangeBitmapToImageSource(bMap);
                        pictureBox.Source = bitmapSource;
                        bitmapSource.Freeze();
                        DeleteObject(hbitmap);
                    }
                }
                catch (Exception)
                {

                }               
            }
            return true;
        }

        public bool WpfShowPreviewBmp(System.Windows.Controls.Image pictureBox, double DefaultWidth = 200, double DefaultHeight = 200)
        {
            if (!InitSuccessed)
            {
                LastError = "激光器初始化失败";
                return false;
            }
            else
            {
                try
                {
                    int width = (int)pictureBox.ActualWidth;
                    int height = (int)pictureBox.ActualHeight;
                    if (width == 0) width = (int)DefaultWidth;
                    if (height == 0) height = (int)DefaultHeight;
                    lock (ObjectLock)
                    {
                        IntPtr hbitmap = LMC1_GETPREVBITMAP2(width, height);
                        Bitmap bMap = Image.FromHbitmap(hbitmap);
                        //ImageSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        //    hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        ImageSource bitmapSource = ChangeBitmapToImageSource(bMap);
                        pictureBox.Source = bitmapSource;
                        bitmapSource.Freeze();
                        DeleteObject(hbitmap);
                    }
                }
                catch (Exception)
                {

                }
            }
            return true;
        }


        public bool WpfShowPreviewBmp(System.Windows.Controls.Border PicBorder)
        {
            int width;
            int height;
            if (!InitSuccessed)
            {
                LastError = "激光器初始化失败";
                return false;
            }
            else
            {
                try
                {
                    width = (int)PicBorder.ActualWidth;
                    height = (int)PicBorder.ActualHeight;
                    lock (ObjectLock)
                    {
                        IntPtr hbitmap = LMC1_GETPREVBITMAP2(width, height);
                        Bitmap dd = Image.FromHbitmap(hbitmap);
                        //ImageSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        //    hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        ImageSource bitmapSource = ChangeBitmapToImageSource(dd);
                        ImageBrush brush = new ImageBrush();
                        brush.ImageSource = bitmapSource;
                        PicBorder.Background = brush;
                        bitmapSource.Freeze();
                        DeleteObject(hbitmap);
                    }
                }
                catch (Exception)
                {

                }                
            }
            return true;
        }
        public ImageSource PreviewImageSource(int width,int height)
        {
            lock (ObjectLock)
            {
                try
                {
                    IntPtr hbitmap = LMC1_GETPREVBITMAP2(width, height);
                    Bitmap dd = Image.FromHbitmap(hbitmap);
                    //ImageSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    //    hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    ImageSource bitmapSource = ChangeBitmapToImageSource(dd);
                    bitmapSource.Freeze();
                    DeleteObject(hbitmap);
                    return bitmapSource;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        {
            //Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            return wpfBitmap;
        }

        /// <summary>
        /// 开启红光
        /// </summary>
        /// <param name="redMode"></param>
        /// <returns></returns>
        public bool StartRed(int redMode = 0)
        {
            if (!InitSuccessed)
            {
                sCommon.MyMsgBox("激光器没有初始化");
                //LastError = LmcErrCode.LMC1_ERR_NOINITIAL;
                return false;
            }
            EnableRed = true;
            Task.Factory.StartNew(() =>
            {
                while (EnableRed)
                {
                    lock (ObjectLock)
                    {
                        LmcErrCode code;
                        if (redMode == 1)
                        {
                            code = Lmc1_RedLightMark();
                        }
                        else
                        {
                            code = Lmc1_RedLightMarkContour();
                        }
                        if (code > LmcErrCode.LMC1_ERR_SUCCESS)
                        {
                            break;
                        }
                    }
                    Thread.Sleep(100);
                }
            }
                );
            return true;
        }

        /// <summary>
        /// 关闭红光
        /// </summary>
        public void StopRed()
        {
            EnableRed = false;
        }

        /// <summary>
        /// 停止打码
        /// </summary>
        public void StopMark()
        {
            if (!InitSuccessed)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
            }
            else
            {
                LmcErrCode code = LMC1_STOPMARK();
                if (LmcErrCode.LMC1_ERR_SUCCESS != code)
                {
                    mLastError = code;
                }
            }
        }

        /// <summary>
        /// 更换模板字段
        /// </summary>
        /// <param name="strEntName"></param>
        /// <param name="strText"></param>
        /// <returns></returns>
        public bool ChangeTextByName(string strEntName, string strText)
        {
            if (!InitSuccessed)
            {
                //mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {
                LmcErrCode code = LMC1_CHANGETEXTBYNAME(strEntName, strText);
                if (LmcErrCode.LMC1_ERR_SUCCESS == code)
                {
                    return true;
                }
                //mLastError = code;
                return false;
            }
        }

        public bool GetTextByName(string strEntName, string strValue)
        {
            char[] strText = new char[0x100];
            lock (ObjectLock)
            {
                LmcErrCode code = LMC1_GETTEXTBYNAME(strEntName, strText);
                if (LmcErrCode.LMC1_ERR_SUCCESS == code)
                {
                    strValue = strText.ToString();
                    return true;
                }
                //mLastError = code;
                return false;
            }
        }

        public string GetLastError()
        {
            string str = "Success";
            switch (mLastError)
            {
                case LmcErrCode.LMC1_ERR_EZCADRUN:
                    return "已有一个程序在使用EzdCad";

                case LmcErrCode.LMC1_ERR_NOFINDCFGFILE:
                    return "未找到激光配置文件EZCAD.CFG";

                case LmcErrCode.LMC1_ERR_FAILEDOPEN:
                    return "打开LMC1卡失败";

                case LmcErrCode.LMC1_ERR_NODEVICE:
                    return "没有有效的lmc1设备";

                case LmcErrCode.LMC1_ERR_HARDVER:
                    return "lmc1版本错误";

                case LmcErrCode.LMC1_ERR_DEVCFG:
                    return "找不到设备配置文件";

                case LmcErrCode.LMC1_ERR_STOPSIGNAL:
                    return "报警信号";

                case LmcErrCode.LMC1_ERR_USERSTOP:
                    return "用户停止";

                case LmcErrCode.LMC1_ERR_UNKNOW:
                    return "不明错误";

                case LmcErrCode.LMC1_ERR_OUTTIME:
                    return "超时";

                case LmcErrCode.LMC1_ERR_NOINITIAL:
                case LmcErrCode.LMC1_ERR_READFILE:
                case LmcErrCode.LMC1_ERR_OWENWNDNULL:
                case LmcErrCode.LMC1_ERR_PENNO:
                case LmcErrCode.LMC1_ERR_NOTTEXT:
                case LmcErrCode.LMC1_ERR_NOFINDENT:
                case LmcErrCode.LMC1_ERR_PARAM:
                case LmcErrCode.LMC1_ERROR_OUTOFPORTRANGE:
                    return str;

                case LmcErrCode.LMC1_ERR_NOFINDFONT:
                    return "找不到该实体";

                case LmcErrCode.LMC1_ERR_SAVEFILE:
                    return "保存文件失败";

                case LmcErrCode.LMC1_ERR_STATUE:
                    return "当前状态下不能执行此操作";

                case LmcErrCode.LMC1_ERR_BRAND:
                    return "未连接HGLASER打标卡";

                case LmcErrCode.LMC1_ERROR_NOEZDFILE:
                    return "Ezd文件路径不存在";

                case LmcErrCode.LMC1_ERROR_NOINIT:
                    return "打标卡未初始化";

                case LmcErrCode.LMC1_ERROR_NOFINDMARKEZD:
                    return "文件Markezd.dll丢失";
            }
            return str;
        }

        public bool WritePort(int nPort, bool bState)
        {
            if (!InitSuccessed)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            if ((nPort < 0) || (nPort > 15))
            {
                mLastError = LmcErrCode.LMC1_ERROR_OUTOFPORTRANGE;
                return false;
            }
            int nData = 0;
            lock (ObjectLock)
            {
                LmcErrCode code = LMC1_GETOUTPORT(ref nData);
                if (LmcErrCode.LMC1_ERR_SUCCESS != code)
                {
                    mLastError = code;
                    return false;
                }
                int num2 = 0;
                if (bState)
                {
                    num2 = ((int)1) << nPort;
                    nData |= num2;
                }
                else
                {
                    num2 = ~(((int)1) << nPort);
                    nData &= num2;
                }
                code = LMC1_WRITEPORT(nData);
                if (LmcErrCode.LMC1_ERR_SUCCESS == code)
                {
                    return true;
                }
                mLastError = code;
                return false;
            }
        }
    }

    /// <summary>
    /// MarkEzd声明
    /// </summary>
    public partial class HGMarker
    {
        [DllImport("gdi32.dll")]
        protected static extern bool DeleteObject(IntPtr hObject);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AddCurveToLib", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_ADDCURVETOLIB([MarshalAs(UnmanagedType.LPArray)] double[,] PtBuf, int ptNum, string strEntName, int nPenNo, int bHatch);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AddFileToLib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_ADDFILETOLIB(string strFilePath, string strEntName, double dLeftDownXPos, double dLeftDowmYPos, double dZpos, int nAlign, double dScaleRation, int nPenNo, bool bHachFile);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AxisCorrectOrigin", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_AXISCORRECTORIGIN(int axis);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AxisMoveTo", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_AXISMOVETO(int axis, double GoalPos);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AxisMoveToPulse", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_AXISMOVETOPULSE(int axis, int nPluse);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_ChangeTextByName", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_CHANGETEXTBYNAME(string strEntName, string strText);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_Close", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_CLOSE();
        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferClear", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERCLEAR();
        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferFlyAdd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERFLYADD(int nNum, string Text1, string Text2, string Text3, string Text4, string Text5, string Text6);
        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferFlyGetParam", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERFLYGETPARAM(ref int nTotalMarkCount, ref int nBufferLeftCount);
        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferFlyStart", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERFLYSTART(int nCount);
        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferPartFinish", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERPARTFINISH();
        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferSetAddMode", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERSETADDMODE(bool FullMode);
        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferSetTextName", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERSETTEXTNAME(string Name1, string Name2, string Name3, string Name4, string Name5, string Name6);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_CopyEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_COPYENT(string strEntName, string strNewEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_DeleteEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_DELETEENT(string strEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetAxisCoorPulse", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int LMC1_GETAXISCOORPULSE(int axis);
        [DllImport("MarkEzd", EntryPoint = "lmc1_GetBitmapEntParam2", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_GETBITMAPENTPARAM2(string strEntName, StringBuilder strImageFileName, ref int nBmpAttrib, ref int nScanAttrib, ref double dBrightness, ref double dContrast, ref double dPointTime, ref int nImportDpi, ref int bDisableMarkLowGrayPt, ref int nMinLowGrayPt);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetBitmapEntParam3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_GETBITMAPENTPARAM3(string strEntName, ref double dDpiX, ref double dDpiY, byte[] bGrayScaleBuff);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetEntCount", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern int LMC1_GETENTCOUNT();
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetEntityName", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_GETENTITYNAME(int nEntityIndex, char[] strEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetEntSize", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_GETENTSIZE(string strEntName, ref double dMinx, ref double dMiny, ref double dMaxx, ref double dMaxy, ref double dZ);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetEntityCount", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern int LMC1_GETENTTITYCOUNT();
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetOutPort", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_GETOUTPORT(ref int nData);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPenNumberFromEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern int LMC1_GETPENNUMBERFROMENT(string strEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPenNumberFromName", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern int LMC1_GETPENNUMBERFROMNAME(string strEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPenParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_GETPENPARAM(int nPenNo, ref int nMarkLoop, ref double dMarkSpeed, ref double dPowerRatio, ref double dCurrent, ref int nFreq, ref double dQPulseWidth, ref int nStartTC, ref int nLaserOffTC, ref int nEndTC, ref int nPolyTC, ref double dJumpSpeed, ref int nJumpPosTC, ref int nJumpDistTC, ref double dEndComp, ref double dAccDist, ref double dPointTime, ref bool bPulsePointMode, ref int nPulseNum, ref double dFlySpeed);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPrevBitmap2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern IntPtr LMC1_GETPREVBITMAP2(int nBMPWIDTH, int nBMPHEIGHT);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetTextByName", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_GETTEXTBYNAME(string pEntName, char[] strText);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GotoPos", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_GOTOPOS(double dCenx, double dCeny);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GroupEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_GROUPENT(string strEntName1, string strEntName2, string strNewGroupName, int nPenNo);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_HatchEnt", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_HATCHENT(string pEntName, string strNewName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_Initial", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_INITIAL(string strEzCadPath, int bTestMode, IntPtr hOwenWnd);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_LoadEzdFile", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_LOADEZDFILE(string strFileName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_Mark", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_MARK(int nFly);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MarkEntity", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_MARKENTITY(string strEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MoveEnt", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_MOVEENT(string pEntName, double dMovex, double dMovey);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_ReadPort", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_READPORT(ref int nData);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_RedLightMark", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_REDLIGHTMARK();
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_Reset", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_RESET(bool bAxis1, bool bAxis2);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_RotateEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_ROTATEENT(string strEntName, double dCenx, double dCeny, double dAngle);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SaveEntLibToFile", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_SAVEENTLIBTOFILE(string strFileName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_ScaleEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_SCALEENT(string strEntName, double dCenterX, double dCenterY, double dScaleX, double dScaleY);
        [DllImport("MarkEzd", EntryPoint = "lmc1_SetBitmapEntParam2", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_SETBITMAPENTPARAM2(string strEntName, string strImageFileName, int nBmpAttrib, int nScanAttrib, double dBrightness, double dContrast, double dPointTime, int nImportDpi, bool bDisableMarkLowGrayPt, int nMinLowGrayPt);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetBitmapEntParam3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_SETBITMAPENTPARAM3(string strEntName, double dDpiX, double dDpiY, byte[] bGrayScaleBuff);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetDevCfg", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_SETDEVCFG();
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetEntityName", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_SETENTITYNAME(int nEntityIndex, string strEntName);
        [DllImport("MarkEzd", EntryPoint = "lmc1_SetHatchParam", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_SETHATCHPARAM(bool bEnableContour, int bEnableHacth1, int nPenNo1, int nHatchAttribute1, double dHatchEdge1, double dHatchDist1, double dHatchStartOffset1, double dHatchEndOffset1, double dHatchAngle1, int bEnableHacth2, int nPenNo2, int nHatchAttribute2, double dHatchEdge2, double dHatchDist2, double dHatchStartOffset2, double dHatchEndOffset2, double dHatchAngle2);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetHatchParam2", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_SETHATCHPARAM2(bool bEnableContour, int nParamIndex, int bEnableHatch, int nPenNo, int nHatchType, bool bHatchAllCalc, bool bHatchEdge, bool bHatchAverageLine, double dHatchAngle, double dHatchLineDist, double dHatchEdgeDist, double dHatchStartOffset, double dHatchEndOffset, double dHatchLineReduction, double dHatchLoopDist, int nEdgeLoop, bool nHatchLoopRev, bool bHatchAutoRotate, double dHatchRotateAngle);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetHatchParam3", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern LmcErrCode LMC1_SETHATCHPARAM3(bool bEnableContour, int nParamIndex, int bEnableHatch, int nPenNo, int nHatchType, bool bHatchAllCalc, bool bHatchEdge, bool bHatchAverageLine, double dHatchAngle, double dHatchLineDist, double dHatchEdgeDist, double dHatchStartOffset, double dHatchEndOffset, double dHatchLineReduction, double dHatchLoopDist, int nEdgeLoop, bool nHatchLoopRev, bool bHatchAutoRotate, double dHatchRotateAngle, bool bHatchCross);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetPenParam", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_SETPENPARAM(int nPenNo, int nMarkLoop, double dMarkSpeed, double dPowerRatio, double dCurrent, int nFreq, double dQPulseWidth, int nStartTC, int nLaserOffTC, int nEndTC, int nPolyTC, double dJumpSpeed, int nJumpPosTC, int nJumpDistTC, double dEndComp, double dAccDist, double dPointTime, bool bPulsePointMode, int nPulseNum, double dFlySpeed);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_StopMark", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_STOPMARK();
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_UnGroupEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_UNGROUPENT(string strGroupEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_WritePort", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_WRITEPORT(int nData);
        [DllImport("user32.dll", EntryPoint = "PostMessage", CharSet = CharSet.Auto)]
        protected static extern bool POSTMESSAGE(IntPtr hwnd, int msg, uint wParam, uint lParam);

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AddBarCodeToLib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_AddBarCodeToLib(string pStr, string pEntName, double dPosX, double dPosY, double dPosZ, int nAlign, int nPenNo, int bHatchText, int nBarcodeType, IntPtr wBarCodeAttrib, double dHeight, double dNarrowWidth, double[] dBarWidthScale, double[] dSpaceWidthScale, double dMidCharSpaceScale, double dQuietLeftScale, double dQuietMidScale, double dQuietRightScale, double dQuietTopScale, double dQuietBottomScale, int nRow, int nCol, int nCheckLevel, int nSizeMode, double dTextHeight, double dTextWidth, double dTextOffsetX, double dTextOffsetY, double dTextSpace, double dDiameter, string pTextFontName);

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AddDelayToLib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_AddDelayToLib(double dDelayMs);

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AddPointToLib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_AddPointToLib(double[,] ptArray, int nPtNum, string pEntName, int nPenNo);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AddTextToLib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_AddTextToLib(string strText, string strEntName, double dPosX, double dPosY, double dPosZ, int nAlign, double dTextRotateAngle, int nPenNo, bool bHatchText);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_AddWritePortToLib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_AddWritePortToLib(int nOutPutBit, bool bHigh, bool bPulse, double dPulseTimeMs);

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_ChangeEntName", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_ChangeEntName(string strEntName, string strNewEntName);

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_ClearEntLib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_ClearEntLib();

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_EntityIsBarcode", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern bool Lmc1_EntityIsBarcode(string strEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetAllFontRecord", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetAllFontRecord(ref int nFontNum);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetAxisCoor", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern double Lmc1_GetAxisCoor(int axis);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetBitmapEntParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetBitmapEntParam(string strEntName, StringBuilder BmpPath, ref int nBmpAttrib, ref int nScanAttrib, ref double dBrightness, ref double dContrast, ref double dPointTime, ref int nImportDpi);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetFlySpeed", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetFlySpeed(ref double dFlySpeed);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetHatchEntParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetHatchEntParam(string entName, ref bool bEnableContour, int nParamIndex, ref int bEnableHatch, ref int nPenNo, ref int nHatchType, ref bool bHatchAllCalc, ref bool bHatchEdge, ref bool bHatchAverageLine, ref double dHatchAngle, ref double dHatchLineDist, ref double dHatchEdgeDist, ref double dHatchStartOffset, ref double dHatchEndOffset, ref double dHatchLineReduction, ref double dHatchLoopDist, ref int nEdgeLoop, ref bool nHatchLoopRev, ref bool dHatchAutoRotate, ref double dHatchRotateAngle);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetHatchEntParam2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetHatchEntParam2(string entName, ref bool bEnableContour, int nParamIndex, ref int bEnableHatch, ref bool bContourFirst, ref int nPenNo, ref int nHatchType, ref bool bHatchAllCalc, ref bool bHatchEdge, ref bool bHatchAverageLine, ref double dHatchAngle, ref double dHatchLineDist, ref double dHatchEdgeDist, ref double dHatchStartOffset, ref double dHatchEndOffset, ref double dHatchLineReduction, ref double dHatchLoopDist, ref int nEdgeLoop, ref bool nHatchLoopRev, ref bool dHatchAutoRotate, ref double dHatchRotateAngle, ref bool bHatchCrossMode, ref int dCycCount);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPenDisableState", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetPenDisableState(int nPenNo, ref bool bDisableMark);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPenParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetPenParam(int nPenNo, ref int nMarkLoop, ref double dMarkSpeed, ref double dPowerRatio, ref double dCurrent, ref int nFreq, ref double nQPulseWidth, ref int nStartTC, ref int nLaserOffTC, ref int nEndTC, ref int nPolyTC, ref double dJumpSpeed, ref int nJumpPosTC, ref int nJumpDistTC, ref double dEndComp, ref double dAccDist, ref double dPointTime, ref bool bPulsePointMode, ref int nPulseNum, ref double dFlySpeed);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPenParam2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetPenParam2(int nPenNo, ref int nMarkLoop, ref double dMarkSpeed, ref double dPowerRatio, ref double dCurrent, ref int nFreq, ref double nQPulseWidth, ref int nStartTC, ref int nLaserOffTC, ref int nEndTC, ref int nPolyTC, ref double dJumpSpeed, ref int nJumpPosTC, ref int nJumpDistTC, ref double dPointTime, ref int nSpiWave, ref bool bWobbleMode, ref double bWobbleDiameter, ref double bWobbleDist);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPenParam4", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetPenParam4(int nPenNo, char[] sPenName, ref int clr, ref bool bDisableMark, ref bool bUseDefParam, ref int nMarkLoop, ref double dMarkSpeed, ref double dPowerRatio, ref double dCurrent, ref int nFreq, ref double nQPulseWidth, ref int nStartTC, ref int nLaserOffTC, ref int nEndTC, ref int nPolyTC, ref double dJumpSpeed, ref int nMinJumpDelayTCUs, ref int nMaxJumpDelayTCUs, ref double dJumpLengthLimit, ref double dPointTime, ref bool nSpiContinueMode, ref int nSpiWave, ref int nYagMarkMode, ref bool bPulsePointMode, ref int nPulseNum, ref bool bEnableACCMode, ref double dEndComp, ref double dAccDist, ref double dBreakAngle, ref bool bWobbleMode, ref double bWobbleDiameter, ref double bWobbleDist);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPrevBitmap", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern IntPtr Lmc1_GetPrevBitmap(IntPtr hwnd, int nBmpWidth, int nBmpHeight);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPrevBitmap3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern IntPtr Lmc1_GetPrevBitmap3(int nBmpWidth, int nBmpHeight, double dLogOriginX, double dLogOriginY, double dLogScale);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetPrevBitmapByName2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern IntPtr Lmc1_GetPrevBitmapByName2(string strEntName, int nBmpWidth, int nBmpHeight);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetTextEntParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetTextEntParam(string strEntName, char[] sFontName, ref double dCharHeight, ref double dCharWidth, ref double dCharAngle, ref double dCharSpace, ref double dLineSpace, ref bool bEqualCharWidth);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_GetTextEntParam2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_GetTextEntParam2(string strEntName, char[] sFontName, ref double dCharHeight, ref double dCharWidth, ref double dCharAngle, ref double dCharSpace, ref double dLineSpace, ref double dSpaceWidth, ref bool bEqualCharWidth);

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_Initial2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_Initial2(string strEzcadPath, bool bTestMode);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_IsMarking", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern bool Lmc1_IsMarking();
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_IsVarText", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_IsVarText(string strEntName, ref bool bVar);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MarkEntityFly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_MarkEntityFly(string strEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MarkFlyByStartSignal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_MarkFlyByStartSignal();
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MarkLine", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_MarkLine(double x1, double y1, double x2, double y2, int pen);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MarkPoint", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_MarkPoint(double x, double y, double delay, int pen);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MarkPointBuf2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_MarkPointBuf2(double[,] ptArray, double dJumpSpeed, double dLaserOnTimeMs);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MirrorEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_MirrorEnt(string strEntName, double dCenX, double dCeny, bool bMirrorX, bool bMirrorY);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MoveEntityAfter", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_MoveEntityAfter(int nMoveEnt, int nGoalEnt);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_MoveEntityBefore", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_MoveEntityBefore(int nMoveEnt, int nGoalEnt);

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_RedLightMark", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_RedLightMark();

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_RedLightMarkByEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_RedLightMarkByEnt(string strEntName, bool bContour);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_RedLightMarkContour", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_RedLightMarkContour();

        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_ReverseAllEntOrder", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_ReverseAllEntOrder();
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetDevCfg2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode LMC1_SetDevCfg2(bool bAxisShow0, bool bAxisShow1);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetEntAllChildPen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetEntAllChildPen(string strEntName, int nPenNo);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetFontParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetFontParam(string strFontName, double dCharHeight, double dCharWidth, double dCharAngle, double dCharSpace, double dLineSpace, bool bEqualCharWidth);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetHatchEntParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetHatchEntParam(string entName, bool bEnableContour, int nParamIndex, int bEnableHatch, int nPenNo, int nHatchType, bool bHatchAllCalc, bool bHatchEdge, bool bHatchAverageLine, double dHatchAngle, double dHatchLineDist, double dHatchEdgeDist, double dHatchStartOffset, double dHatchEndOffset, double dHatchLineReduction, double dHatchLoopDist, int nEdgeLoop, bool nHatchLoopRev, bool dHatchAutoRotate, double dHatchRotateAngle);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetHatchEntParam2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetHatchEntParam2(string entName, bool bEnableContour, int nParamIndex, int bEnableHatch, bool bContourFirst, int nPenNo, int nHatchType, bool bHatchAllCalc, bool bHatchEdge, bool bHatchAverageLine, double dHatchAngle, double dHatchLineDist, double dHatchEdgeDist, double dHatchStartOffset, double dHatchEndOffset, double dHatchLineReduction, double dHatchLoopDist, int nEdgeLoop, bool nHatchLoopRev, bool dHatchAutoRotate, double dHatchRotateAngle, bool bHatchCrossMode, int dCycCount);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetPenDisableState", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetPenDisableState(int nPenNo, bool bDisableMark);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetPenParam2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetPenParam2(int nPenNo, int nMarkLoop, double dMarkSpeed, double dPowerRatio, double dCurrent, int nFreq, double nQPulseWidth, int nStartTC, int nLaserOffTC, int nEndTC, int nPolyTC, double dJumpSpeed, int nJumpPosTC, int nJumpDistTC, double dPointTime, int nSpiWave, bool bWobbleMode, double bWobbleDiameter, double bWobbleDist);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetPenParam4", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetPenParam4(int nPenNo, string sPenName, int clr, bool bDisableMark, bool bUseDefParam, int nMarkLoop, double dMarkSpeed, double dPowerRatio, double dCurrent, int nFreq, double nQPulseWidth, int nStartTC, int nLaserOffTC, int nEndTC, int nPolyTC, double dJumpSpeed, int nMinJumpDelayTCUs, int nMaxJumpDelayTCUs, double dJumpLengthLimit, double dPointTime, bool nSpiContinueMode, int nSpiWave, int nYagMarkMode, bool bPulsePointMode, int nPulseNum, bool bEnableACCMode, double dEndComp, double dAccDist, double dBreakAngle, bool bWobbleMode, double bWobbleDiameter, double bWobbleDist);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetRotateMoveParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetRotateMoveParam(double dMoveX, double dMoveY, double dCenterX, double dCenterY, double dRotateAng);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetRotateMoveParam2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetRotateMoveParam2(double dMoveX, double dMoveY, double dCenterX, double dCenterY, double dRotateAng, double dScaleX, double dScaleY);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetTextEntParam", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetTextEntParam(string strEntName, double dCharHeight, double dCharWidth, double dCharAngle, double dCharSpace, double dLineSpace, bool bEqualCharWidth);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetTextEntParam2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_SetTextEntParam2(string strEntName, string strFontName, double dCharHeight, double dCharWidth, double dCharAngle, double dCharSpace, double dLineSpace, double dSpaceWidth, bool bEqualCharWidth);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_TextResetSn", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_TextResetSn(string strEntName);
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_UnHatchEnt", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        protected static extern LmcErrCode Lmc1_UnHatchEnt(string strEntName);
    }
}
