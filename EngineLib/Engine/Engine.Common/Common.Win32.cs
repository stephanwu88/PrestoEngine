using System;
using System.Runtime.InteropServices;

namespace Engine.Common
{
    /// <summary>
    /// Windows消息定义 Window Message
    /// </summary>
    public partial class sCommon
    {
        const int WM_CREATE = 0x0001;               //应用程序创建一个窗口
        const int WM_DESTROY = 0x0002;              //一个窗口被销毁
        const int WM_MOVE = 0x0003;                 //移动一个窗口
        const int WM_SIZE = 0x0005;                 //改变一个窗口的大小
        const int WM_ACTIVATE = 0x0006;             //一个窗口被激活或失去激活状态
        const int WM_SETFOCUS = 0x0007;             //获得焦点后
        const int WM_KILLFOCUS = 0x0008;            //失去焦点
        const int WM_ENABLE = 0x000A;               //改变enable状态
        const int WM_SETREDRAW = 0x000B;            //设置窗口是否能重画
        const int WM_SETTEXT = 0x000C;              //应用程序发送此消息来设置一个窗口的文本
        const int WM_GETTEXT = 0x000D;              //应用程序发送此消息来复制对应窗口的文本到缓冲区
        const int WM_GETTEXTLENGTH = 0x000E;        //得到与一个窗口有关的文本的长度（不包含空字符）
        const int WM_PAINT = 0x000F;                //要求一个窗口重画自己
        const int WM_CLOSE = 0x0010;                //当一个窗口或应用程序要关闭时发送一个信号
        const int WM_QUERYENDSESSION = 0x0011;      //当用户选择结束对话框或程序自己调用ExitWindows函数
        const int WM_QUIT = 0x0012;                 //用来结束程序运行或当程序调用postquitmessage函数
        const int WM_QUERYOPEN = 0x0013;            //当用户窗口恢复以前的大小位置时，把此消息发送给某个图标
        const int WM_ERASEBKGND = 0x0014;           //当窗口背景必须被擦除时（例在窗口改变大小时）
        const int WM_SYSCOLORCHANGE = 0x0015;       //当系统颜色改变时，发送此消息给所有顶级窗口
        const int WM_ENDSESSION = 0x0016;           //当系统进程发出_WM_QUERYENDSESSION消息后，此消息发送给应用程序，通知它对话是否结束
        const int WM_SYSTEMERROR = 0x0017;          //系统错误
        const int WM_SHOWWINDOW = 0x0018;           //当隐藏或显示窗口是发送此消息给这个窗口
        const int WM_ACTIVATEAPP = 0x001C;          //发此消息给应用程序哪个窗口是激活的，哪个是非激活的
        const int WM_FONTCHANGE = 0x001D;           //当系统的字体资源库变化时发送此消息给所有顶级窗口
        const int WM_TIMECHANGE = 0x001E;           //当系统的时间变化时发送此消息给所有顶级窗口
        const int WM_CANCELMODE = 0x001F;           //发送此消息来取消某种正在进行的摸态（操作）
        const int WM_SETCURSOR = 0x0020;            //若鼠标引起光标在某个窗口中移动且鼠标输入没有被捕获时，就发消息给某个窗口
        const int WM_MOUSEACTIVATE = 0x0021;        //当光标在某个非激活的窗口中而用户正按着鼠标的某个键发送此消息给当前窗口
        const int WM_CHILDACTIVATE = 0x0022;        //发送此消息给MDI子窗口当用户点击此窗口的标题栏，或当窗口被激活，移动，改变大小
        const int WM_QUEUESYNC = 0x0023;            //此消息由基于计算机的训练程序发送，通过WH_JOURNALPALYBACK的hook程序 分离出用户输入消息
        const int WM_GETMINMAXINFO = 0x0024;        //此消息发送给窗口当它将要改变大小或位置；
        const int WM_PAINTICON = 0x0026;            //发送给最小化窗口当它图标将要被重画
        const int WM_ICONERASEBKGND = 0x0027;       //此消息发送给某个最小化窗口，仅当它在画图标前它的背景必须被重画
        const int WM_NEXTDLGCTL = 0x0028;           //发送此消息给一个对话框程序去更改焦点位置
        const int WM_SPOOLERSTATUS = 0x002A;        //每当打印管理列队增加或减少一条作业时发出此消息
        const int WM_DRAWITEM = 0x002B;             //当button，combobox，listbox，menu的可视外观改变时发送 此消息给这些空件的所有者
        const int WM_MEASUREITEM = 0x002C;          //当button, combo box, list box, list view control, or menu item 被创建时 发送此消息给控件的所有者
        const int WM_DELETEITEM = 0x002D;           //当the list box 或 combo box 被销毁 或 当 某些项被删除通过 LB_DELETESTRING, LB_RESETCONTENT, CB_DELETESTRING, or CB_RESETCONTENT 消息
        const int WM_VKEYTOITEM = 0x002E;           //此消息有一个LBS_WANTKEYBOARDINPUT风格的发出给它的所有者来响应
        const int WM_CHARTOITEM = 0x002F;           //此消息由一个LBS_WANTKEYBOARDINPUT风格的列表框发送给他的所有者来响应const int WM_CHAR消息
        const int WM_SETFONT = 0x0030;              //当绘制文本时程序发送此消息得到控件要用的颜色
        const int WM_GETFONT = 0x0031;              //应用程序发送此消息得到当前控件绘制文本的字体
        const int WM_SETHOTKEY = 0x0032;            //应用程序发送此消息让一个窗口与一个热键相关连
        const int WM_GETHOTKEY = 0x0033;            //应用程序发送此消息来判断热键与某个窗口是否有关联
        const int WM_QUERYDRAGICON = 0x0037;        //此消息发送给最小化窗口，当此窗口将要被拖放而它的类中没有定义图标，应用程序能返回一个图标或光标的句柄，当用户拖放图标时系统显示这个图标或光标
        const int WM_COMPAREITEM = 0x0039;          //发送此消息来判定combobox或listbox新增加的项的相对位置
        const int WM_GETOBJECT = 0x003D;            //
        const int WM_COMPACTING = 0x0041;           //显示内存已经很少了
        const int WM_WINDOWPOSCHANGING = 0x0046;    //发送此消息给那个窗口的大小和位置将要被改变时，来调用setwindowpos函数或其它窗口管理函数
        const int WM_WINDOWPOSCHANGED = 0x0047;     //发送此消息给那个窗口的大小和位置已经被改变时，来调用setwindowpos函数或其它窗口管理函数
        const int WM_POWER = 0x0048;                //（适用于16位的windows）当系统将要进入暂停状态时发送此消息
        const int WM_COPYDATA = 0x004A;             //当一个应用程序传递数据给另一个应用程序时发送此消息
        const int WM_CANCELJOURNAL = 0x004B;        //当某个用户取消程序日志激活状态，提交此消息给程序
        const int WM_NOTIFY = 0x004E;               //当某个控件的某个事件已经发生或这个控件需要得到一些信息时，发送此消息给它的父窗口
        const int WM_INPUTLANGCHANGEREQUEST = 0x0050; //当用户选择某种输入语言，或输入语言的热键改变
        const int WM_INPUTLANGCHANGE = 0x0051;      //当平台现场已经被改变后发送此消息给受影响的最顶级窗口
        const int WM_TCARD = 0x0052;                //当程序已经初始化windows帮助例程时发送此消息给应用程序
        const int WM_HELP = 0x0053;                 //此消息显示用户按下了F1，若某个菜单是激活的，就发送此消息个此窗口关联的菜单，否则就发送给有焦点的窗口，若当前都没有焦点，就把此消息发送给当前激活的窗口
        const int WM_USERCHANGED = 0x0054;          //当用户已经登入或退出后发送此消息给所有的窗口，当用户登入或退出时系统更新用户的具体设置信息，在用户更新设置时系统马上发送此消息；
        const int WM_NOTIFYFORMAT = 0x0055;         //公用控件，自定义控件和他们的父窗口通过此消息来判断控件是使用ANSI还是UNICODE结构在const int WM_NOTIFY消息，使用此控件能使某个控件与它的父控件之间进行相互通信
        const int WM_CONTEXTMENU = 0x007B;          //当用户某个窗口中点击了一下右键就发送此消息给这个窗口
        const int WM_STYLECHANGING = 0x007C;        //当调用SETWINDOWLONG函数将要改变一个或多个 窗口的风格时发送此消息给那个窗口
        const int WM_STYLECHANGED = 0x007D;         //当调用SETWINDOWLONG函数一个或多个 窗口的风格后发送此消息给那个窗口
        const int WM_DISPLAYCHANGE = 0x007E;        //当显示器的分辨率改变后发送此消息给所有的窗口
        const int WM_GETICON = 0x007F;              //此消息发送给某个窗口来返回与某个窗口有关连的大图标或小图标的句柄；
        const int WM_SETICON = 0x0080;              //程序发送此消息让一个新的大图标或小图标与某个窗口关联；
        const int WM_NCCREATE = 0x0081;             //当某个窗口第一次被创建时，此消息在const int WM_CREATE消息发送前发送；
        const int WM_NCDESTROY = 0x0082;            //此消息通知某个窗口，非客户区正在销毁
        const int WM_NCCALCSIZE = 0x0083;           //当某个窗口的客户区域必须被核算时发送此消息
        const int WM_NCHITTEST = 0x0084;            //移动鼠标，按住或释放鼠标时发生
        const int WM_NCPAINT = 0x0085;              //程序发送此消息给某个窗口当它（窗口）的框架必须被绘制时；
        const int WM_NCACTIVATE = 0x0086;           //此消息发送给某个窗口 仅当它的非客户区需要被改变来显示是激活还是非激活状态；
        const int WM_GETDLGCODE = 0x0087;           //发送此消息给某个与对话框程序关联的控件，widdows控制方位键和TAB键使输入进入此控件 通过响应const int WM_GETDLGCODE消息，应用程序可以把他当成一个特殊的输入控件并能处理它
        const int WM_NCMOUSEMOVE = 0x00A0;          //当光标在一个窗口的非客户区内移动时发送此消息给这个窗口的边框体 //非客户区为：窗体的标题栏及窗 
        const int WM_NCLBUTTONDOWN = 0x00A1;        //当光标在一个窗口的非客户区同时按下鼠标左键时提交此消息
        const int WM_NCLBUTTONUP = 0x00A2;          //当用户释放鼠标左键同时光标某个窗口在非客户区十发送此消息；
        const int WM_NCLBUTTONDBLCLK = 0x00A3;      //当用户双击鼠标左键同时光标某个窗口在非客户区十发送此消息
        const int WM_NCRBUTTONDOWN = 0x00A4;        //当用户按下鼠标右键同时光标又在窗口的非客户区时发送此消息
        const int WM_NCRBUTTONUP = 0x00A5;          //当用户释放鼠标右键同时光标又在窗口的非客户区时发送此消息
        const int WM_NCRBUTTONDBLCLK = 0x00A6;      //当用户双击鼠标右键同时光标某个窗口在非客户区十发送此消息
        const int WM_NCMBUTTONDOWN = 0x00A7;        //当用户按下鼠标中键同时光标又在窗口的非客户区时发送此消息
        const int WM_NCMBUTTONUP = 0x00A8;          //当用户释放鼠标中键同时光标又在窗口的非客户区时发送此消息
        const int WM_NCMBUTTONDBLCLK = 0x00A9;      //当用户双击鼠标中键同时光标又在窗口的非客户区时发送此消息
        const uint WM_BM_CLICK = 0x00F5;            //按钮点击
        const int WM_KEYFIRST = 0x0100;             //
        const int WM_KEYDOWN = 0x0100;              //按下一个键
        const int WM_KEYUP = 0x0101;                //释放一个键
        const int WM_CHAR = 0x0102;                 //按下某键，并已发出const int WM_KEYDOWN， const int WM_KEYUP消息
        const int WM_DEADCHAR = 0x0103;             //当用translatemessage函数翻译const int WM_KEYUP消息时发送此消息给拥有焦点的窗口
        const int WM_SYSKEYDOWN = 0x0104;           //当用户按住ALT键同时按下其它键时提交此消息给拥有焦点的窗口；
        const int WM_SYSKEYUP = 0x0105;             //当用户释放一个键同时ALT 键还按着时提交此消息给拥有焦点的窗口
        const int WM_SYSCHAR = 0x0106;              //当const int WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后提交此消息给拥有焦点的窗口
        const int WM_SYSDEADCHAR = 0x0107;          //当const int WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后发送此消息给拥有焦点的窗口
        const int WM_KEYLAST = 0x0108;              //
        const int WM_INITDIALOG = 0x0110;           //初始化 在一个对话框程序被显示前发送此消息给它，通常用此消息初始化控件和执行其它任务
        const int WM_COMMAND = 0x0111;              //当用户选择一条菜单命令项或当某个控件发送一条消息给它的父窗口，一个快捷键被翻译
        const int WM_SYSCOMMAND = 0x0112;           //当用户选择窗口菜单的一条命令或当用户选择最大化或最小化时那个窗口会收到此消息
        const int WM_TIMER = 0x0113;                //发生了定时器事件
        const int WM_HSCROLL = 0x0114;              //当一个窗口标准水平滚动条产生一个滚动事件时发送此消息给那个窗口，也发送给拥有它的控件
        const int WM_VSCROLL = 0x0115;              //当一个窗口标准垂直滚动条产生一个滚动事件时发送此消息给那个窗口也，发送给拥有它的控件
        const int WM_INITMENU = 0x0116;             //当一个菜单将要被激活时发送此消息，它发生在用户菜单条中的某项或按下某个菜单键，它允许程序在显示前更改菜单
        const int WM_INITMENUPOPUP = 0x0117;        //当一个下拉菜单或子菜单将要被激活时发送此消息，它允许程序在它显示前更改菜单，而不要改变全部
        const int WM_MENUSELECT = 0x011F;           //当用户选择一条菜单项时发送此消息给菜单的所有者（一般是窗口）
        const int WM_MENUCHAR = 0x0120;             //当菜单已被激活用户按下了某个键（不同于加速键），发送此消息给菜单的所有者；
        const int WM_ENTERIDLE = 0x0121;            //当一个模态对话框或菜单进入空载状态时发送此消息给它的所有者，一个模态对话框或菜单进入空载状态就是在处理完一条或几条先前的消息后没有消息它的列队中等待
        const int WM_MENURBUTTONUP = 0x0122;        //
        const int WM_MENUDRAG = 0x0123;             //
        const int WM_MENUGETOBJECT = 0x0124;        //
        const int WM_UNINITMENUPOPUP = 0x0125;      //
        const int WM_MENUCOMMAND = 0x0126;          //
        const int WM_CHANGEUISTATE = 0x0127;        //
        const int WM_UPDATEUISTATE = 0x0128;        //
        const int WM_QUERYUISTATE = 0x0129;         //
        const int WM_CTLCOLORMSGBOX = 0x0132;       //在windows绘制消息框前发送此消息给消息框的所有者窗口，通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置消息框的文本和背景颜色
        const int WM_CTLCOLOREDIT = 0x0133;         //当一个编辑型控件将要被绘制时发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置编辑框的文本和背景颜色
        const int WM_CTLCOLORLISTBOX = 0x0134;      //当一个列表框控件将要被绘制前发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置列表框的文本和背景颜色
        const int WM_CTLCOLORBTN = 0x0135;          //当一个按钮控件将要被绘制时发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置按纽的文本和背景颜色
        const int WM_CTLCOLORDLG = 0x0136;          //当一个对话框控件将要被绘制前发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置对话框的文本背景颜色
        const int WM_CTLCOLORSCROLLBAR = 0x0137;    //当一个滚动条控件将要被绘制时发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置滚动条的背景颜色
        const int WM_CTLCOLORSTATIC = 0x0138;       //当一个静态控件将要被绘制时发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置静态控件的文本和背景颜色
        const int WM_MOUSEFIRST = 0x0200;           //
        const int WM_MOUSEMOVE = 0x0200;            //移动鼠标
        const int WM_LBUTTONDOWN = 0x0201;          //按下鼠标左键
        const int WM_LBUTTONUP = 0x0202;            //释放鼠标左键
        const int WM_LBUTTONDBLCLK = 0x0203;        //双击鼠标左键
        const int WM_RBUTTONDOWN = 0x0204;          //按下鼠标右键
        const int WM_RBUTTONUP = 0x0205;            //释放鼠标右键
        const int WM_RBUTTONDBLCLK = 0x0206;        //双击鼠标右键
        const int WM_MBUTTONDOWN = 0x0207;          //按下鼠标中键
        const int WM_MBUTTONUP = 0x0208;            //释放鼠标中键
        const int WM_MBUTTONDBLCLK = 0x0209;        //双击鼠标中键
        const int WM_MOUSEWHEEL = 0x020A;           //当鼠标轮子转动时发送此消息个当前有焦点的控件
        const int WM_MOUSELAST = 0x020A;            //
        const int WM_PARENTNOTIFY = 0x0210;         //当MDI子窗口被创建或被销毁，或用户按了一下鼠标键而光标在子窗口上时发送此消息给它的父窗口
        const int WM_ENTERMENULOOP = 0x0211;        //发送此消息通知应用程序的主窗口that已经进入了菜单循环模式
        const int WM_EXITMENULOOP = 0x0212;         //发送此消息通知应用程序的主窗口that已退出了菜单循环模式
        const int WM_NEXTMENU = 0x0213;             //
        const int WM_SIZING = 532;                  //当用户正在调整窗口大小时发送此消息给窗口；通过此消息应用程序可以监视窗口大小和位置也可以修改他们
        const int WM_CAPTURECHANGED = 533;          //发送此消息 给窗口当它失去捕获的鼠标时；
        const int WM_MOVING = 534;                  //当用户在移动窗口时发送此消息，通过此消息应用程序可以监视窗口大小和位置也可以修改他们；
        const int WM_POWERBROADCAST = 536;          //此消息发送给应用程序来通知它有关电源管理事件；
        const int WM_DEVICECHANGE = 537;            //当设备的硬件配置改变时发送此消息给应用程序或设备驱动程序
        const int WM_IME_STARTCOMPOSITION = 0x010D; //
        const int WM_IME_ENDCOMPOSITION = 0x010E;   //
        const int WM_IME_COMPOSITION = 0x010F;      //
        const int WM_IME_KEYLAST = 0x010F;          //
        const int WM_IME_SETCONTEXT = 0x0281;       //
        const int WM_IME_NOTIFY = 0x0282;           //
        const int WM_IME_CONTROL = 0x0283;          //
        const int WM_IME_COMPOSITIONFULL = 0x0284;  //
        const int WM_IME_SELECT = 0x0285;           //
        const int WM_IME_CHAR = 0x0286;             //
        const int WM_IME_REQUEST = 0x0288;          //
        const int WM_IME_KEYDOWN = 0x0290;          //
        const int WM_IME_KEYUP = 0x0291;            //
        const int WM_MDICREATE = 0x0220;            //应用程序发送此消息给多文档的客户窗口来创建一个MDI 子窗口
        const int WM_MDIDESTROY = 0x0221;           //应用程序发送此消息给多文档的客户窗口来关闭一个MDI 子窗口
        const int WM_MDIACTIVATE = 0x0222;          //应用程序发送此消息给多文档的客户窗口通知客户窗口激活另一个MDI子窗口，当客户窗口收到此消息后，它发出const int WM_MDIACTIVE消息给MDI子窗口（未激活）激活它；
        const int WM_MDIRESTORE = 0x0223;           //程序 发送此消息给MDI客户窗口让子窗口从最大最小化恢复到原来大小
        const int WM_MDINEXT = 0x0224;              //程序 发送此消息给MDI客户窗口激活下一个或前一个窗口
        const int WM_MDIMAXIMIZE = 0x0225;          //程序发送此消息给MDI客户窗口来最大化一个MDI子窗口；
        const int WM_MDITILE = 0x0226;              //程序 发送此消息给MDI客户窗口以平铺方式重新排列所有MDI子窗口
        const int WM_MDICASCADE = 0x0227;           //程序 发送此消息给MDI客户窗口以层叠方式重新排列所有MDI子窗口
        const int WM_MDIICONARRANGE = 0x0228;       //程序 发送此消息给MDI客户窗口重新排列所有最小化的MDI子窗口
        const int WM_MDIGETACTIVE = 0x0229;         //程序 发送此消息给MDI客户窗口来找到激活的子窗口的句柄
        const int WM_MDISETMENU = 0x0230;           //程序 发送此消息给MDI客户窗口用MDI菜单代替子窗口的菜单
        const int WM_ENTERSIZEMOVE = 0x0231;        //
        const int WM_EXITSIZEMOVE = 0x0232;         //
        const int WM_DROPFILES = 0x0233;            //
        const int WM_MDIREFRESHMENU = 0x0234;       //
        const int WM_MOUSEHOVER = 0x02A1;           //
        const int WM_MOUSELEAVE = 0x02A3;           //
        const int WM_CUT = 0x0300;                  //程序发送此消息给一个编辑框或combobox来删除当前选择的文本
        const int WM_COPY = 0x0301;                 //程序发送此消息给一个编辑框或combobox来复制当前选择的文本到剪贴板
        const int WM_PASTE = 0x0302;                //程序发送此消息给editcontrol或combobox从剪贴板中得到数据
        const int WM_CLEAR = 0x0303;                //程序发送此消息给editcontrol或combobox清除当前选择的内容；
        const int WM_UNDO = 0x0304;                 //程序发送此消息给editcontrol或combobox撤消最后一次操作
        const int WM_RENDERFORMAT = 0x0305;
        const int WM_RENDERALLFORMATS = 0x0306;     //
        const int WM_DESTROYCLIPBOARD = 0x0307;     //当调用ENPTYCLIPBOARD函数时 发送此消息给剪贴板的所有者
        const int WM_DRAWCLIPBOARD = 0x0308;        //当剪贴板的内容变化时发送此消息给剪贴板观察链的第一个窗口；它允许用剪贴板观察窗口来 显示剪贴板的新内容；
        const int WM_PAINTCLIPBOARD = 0x0309;       //当剪贴板包含CF_OWNERDIPLAY格式的数据并且剪贴板观察窗口的客户区需要重画；
        const int WM_VSCROLLCLIPBOARD = 0x030A;     //
        const int WM_SIZECLIPBOARD = 0x030B;        //当剪贴板包含CF_OWNERDIPLAY格式的数据并且剪贴板观察窗口的客户区域的大小已经改变是此消息通过剪贴板观察窗口发送给剪贴板的所有者；
        const int WM_ASKCBFORMATNAME = 0x030C;      //通过剪贴板观察窗口发送此消息给剪贴板的所有者来请求一个CF_OWNERDISPLAY格式的剪贴板的名字
        const int WM_CHANGECBCHAIN = 0x030D;        //当一个窗口从剪贴板观察链中移去时发送此消息给剪贴板观察链的第一个窗口；    
        const int WM_HSCROLLCLIPBOARD = 0x030E;     //此消息通过一个剪贴板观察窗口发送给剪贴板的所有者 ；它发生在当剪贴板包含CFOWNERDISPALY格式的数据并且有个事件在剪贴板观察窗的水平滚动条上；所有者应滚动剪贴板图象并更新滚动条的值；
        const int WM_QUERYNEWPALETTE = 0x030F;      //此消息发送给将要收到焦点的窗口，此消息能使窗口在收到焦点时同时有机会实现他的逻辑调色板
        const int WM_PALETTEISCHANGING = 0x0310;    //当一个应用程序正要实现它的逻辑调色板时发此消息通知所有的应用程序
        const int WM_PALETTECHANGED = 0x0311;       //此消息在一个拥有焦点的窗口实现它的逻辑调色板后发送此消息给所有顶级并重叠的窗口，以此来改变系统调色板
        const int WM_HOTKEY = 0x0312;               //当用户按下由REGISTERHOTKEY函数注册的热键时提交此消息
        const int WM_PRINT = 791;                   //应用程序发送此消息仅当WINDOWS或其它应用程序发出一个请求要求绘制一个应用程序的一部分；
        const int WM_PRINTCLIENT = 792;             //
        const int WM_HANDHELDFIRST = 856;           //
        const int WM_HANDHELDLAST = 863;            //
        const int WM_PENWINFIRST = 0x0380;          //
        const int WM_PENWINLAST = 0x038F;           //
        const int WM_COALESCE_FIRST = 0x0390;       //
        const int WM_COALESCE_LAST = 0x039F;        //
        const int WM_DDE_FIRST = 0x03E0;            //
        const int WM_DDE_INITIATE = 0x03E0 + 0;     //一个DDE客户程序提交此消息开始一个与服务器程序的会话来响应那个指定的程序和主题名；
        const int WM_DDE_TERMINATE = 0x03E0 + 1;    //一个DDE应用程序（无论是客户还是服务器）提交此消息来终止一个会话；
        const int WM_DDE_ADVISE = 0x03E0 + 2;       //一个DDE客户程序提交此消息给一个DDE服务程序来请求服务器每当数据项改变时更新它   
        const int WM_DDE_UNADVISE = 0x03E0 + 3;     //一个DDE客户程序通过此消息通知一个DDE服务程序不更新指定的项或一个特殊的剪贴板格式的项
        const int WM_DDE_ACK = 0x03E0 + 4;          //此消息通知一个DDE（动态数据交换）程序已收到并正在处理const int WM_DDE_POKE,
        const int WM_DDE_DATA = 0x03E0 + 5;         //一个DDE服务程序提交此消息给DDE客户程序来传递个一数据项给客户或通知客户的一条可用数据项
        const int WM_DDE_REQUEST = 0x03E0 + 6;      //一个DDE客户程序提交此消息给一个DDE服务程序来请求一个数据项的值；
        const int WM_DDE_POKE = 0x03E0 + 7;         //一个DDE客户程序提交此消息给一个DDE服务程序，客户使用此消息来请求服务器接收一个未经同意的数据项；服务器通过答复const int WM_DDE_ACK消息提示是否它接收这个数据项；
        const int WM_DDE_EXECUTE = 0x03E0 + 8;      //一个DDE客户程序提交此消息给一个DDE服务程序来发送一个字符串给服务器让它象串行命令一样被处理，服务器通过提交const int WM_DDE_ACK消息来作回应；
        const int WM_DDE_LAST = 0x03E0 + 8;         //
        const int WM_APP = 0x8000;                  //
        const int WM_USER = 0x0400;                 //一般自定义的消息应当大于0x400

        const int SC_MOVE = 0xF012;                 //
    }

    /// <summary>
    /// Win32常数定义
    /// </summary>
    public partial class sCommon
    {
        const Int32 GWL_STYLE = -16;
        const Int32 WS_BORDER = (Int32)0x00800000L;
        const Int32 WS_THICKFRAME = (Int32)0x00040000L;
        const Int32 SWP_NOMOVE = 0x0002;
        const Int32 SWP_NOSIZE = 0x0001;
        const Int32 SWP_NOZORDER = 0x0004;
        const Int32 SWP_FRAMECHANGED = 0x0020;
        const Int32 SW_MAXIMIZE = 3;
        static IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    }

    /// <summary>
    /// Win32发送消息 - user32
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="wMsg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessage(this IntPtr hwnd, uint wMsg, int wParam, int lParam);

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(this IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
    }

    /// <summary>
    /// Win32窗口操作 - user32
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <param name="lpEnumFunc"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpEnumFunc"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public extern static int EnumWindows(EnumWindowsProc lpEnumFunc, int lParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// 获取窗口句柄
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 窗口上获取元件句柄
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <param name="hwndChildAfter"></param>
        /// <param name="lpszClass"></param>
        /// <param name="lpszWindow"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        public static extern IntPtr FindWindowEx(this IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// 获取类的名字
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <param name="lpString">类名</param>
        /// <param name="nMaxCount">最大值</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd,string lpString, int nMaxCount);

        /// <summary>
        /// 获取当前窗口的句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern Int32 GetWindowLong(this IntPtr hWnd,Int32 nIndex);

        /// <summary>
        /// 获取对象坐标尺寸
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;     //最左坐标
            public int Top;      //最上坐标
            public int Right;    //最右坐标
            public int Bottom;   //最下坐标
        }


        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpString"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(this IntPtr hWnd, string lpString, int nMaxCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(this IntPtr hWnd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="bRevert"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        public static extern IntPtr GetSystemMenu(this IntPtr hWnd, IntPtr bRevert);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hMenu"></param>
        /// <param name="nPos"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        public static extern int RemoveMenu(this IntPtr hMenu, int nPos, int flags);

        /// <summary>
        /// 让窗口置于所有页面最前端
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="fAltTab"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SwitchToThisWindow(this IntPtr hwnd, bool fAltTab);

        /// <summary>
        /// 获取窗体Class文本
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetClassText")]
        public static extern int GetClassText(this IntPtr hwnd, string text, int maxLength);

        /// <summary>
        /// 获取窗口是否可见
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(this IntPtr hWnd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern Int32 SetWindowLong(this IntPtr hWnd,Int32 nIndex,Int32 dwNewLong);

        /// <summary>
        /// 设置父窗口
        /// </summary>
        /// <param name="hWndChild"></param>
        /// <param name="hWndNewParent"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "SetParent")]
        public static extern IntPtr SetParent(this IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// 设置窗口位置
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndlnsertAfter"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="Flags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(this IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);

        /// <summary>
        /// 设置当前窗口的句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
        public static extern void SetForegroundWindow(this IntPtr hwnd);

        /// <summary>
        /// 0-关闭/隐藏窗口 1-正常大小显示 2最小化窗口 3-最大化窗口 5-显示窗体
        /// 隐藏窗体后，窗体代码不能工作
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="_value"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(this IntPtr hWnd, int _value);
    }

    /// <summary>
    /// 进程操作 - kernel32
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(this IntPtr proc, int min, int max);
    }
}
