using Engine.Common;
using Engine.Core.TaskSchedule;
using Engine.Data.DBFAC;
using System.Collections.Generic;

namespace Engine.Core
{
    /// <summary>
    /// 宏指令
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MacroCommand<TCommandParam> where TCommandParam : new()
    {
        /// <summary>
        /// 指令对象
        /// </summary>
        public TCommandParam CommandParam { get; set; }

        /// <summary>
        /// 验证是否有效
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExpressFormat { get; set; }

    }

    /// <summary>
    /// 宏定义
    /// </summary>
    public class MacroDef
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 指令名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分组标记
        /// </summary>
        public string GroupMark { get; set; }
    }

    /// <summary>
    /// 脚本函数解析器 -- 实例及定义
    /// </summary>
    public partial class MacroCommand
    {
        private static MacroCommand _Default;
        public static MacroCommand Default
        {
            get
            {
                if (_Default == null)
                {
                    _Default = new MacroCommand();
                }
                return _Default;
            }
        }
        private Dictionary<string, MacroDef> DicMacro = new Dictionary<string, MacroDef>();
        private List<MacroDef> LstMacro = new List<MacroDef>();
        public MacroCommand()
        {
            LstMacro = new List<MacroDef>()
            {
                new MacroDef(){ GroupName = "DataAccess",Name = "GetView",GroupMark = "数据访问" },
                new MacroDef(){ GroupName = "DataAccess",Name = "GetSubPosView",GroupMark = "数据访问" },
                new MacroDef(){ GroupName = "DataConvert",Name = "$ToBinString",GroupMark = "数据转换" },
            };
            DicMacro = LstMacro.ToMyDictionary(x => x.Name, x => x);
        }
    }

    /// <summary>
    /// 脚本函数解析器 -- 宏指令解析
    /// </summary>
    public partial class MacroCommand
    {
        /// <summary>
        /// 获取表达式宏指令函数
        /// </summary>
        /// <param name="MacroExpess"></param>
        /// <returns></returns>
        public MacroDef GetMacroCommand(string MacroExpess)
        {
            string strStartWith = MacroExpess.MidString("", "(").Trim();
            if (!DicMacro.ContainsKey(strStartWith))
                return null;
            return DicMacro[strStartWith];
        }

        /// <summary>
        /// 是否宏指令函数
        /// </summary>
        /// <param name="MacroExpess"></param>
        /// <returns></returns>
        public bool IsMacroCommand(string MacroExpess)
        {
            return GetMacroCommand(MacroExpess) != null ;
        }

        /// <summary>
        /// 解析表达语句
        /// </summary>
        /// <returns></returns>
        public string ParseExpStatement(string strExpStatement, object data)
        {
            List<string> LstSection = strExpStatement.MySplit("=");
            if (LstSection.Count != 2) return strExpStatement;
            string expLeft = LstSection[0];
            string expRight = LstSection[1];
            if (!IsMacroCommand(expRight))
                return strExpStatement;
            expRight = ParseExpressValue(expRight, data);
            strExpStatement = string.Format("{0}={1}", expLeft, expRight);
            return strExpStatement;
        }

        /// <summary>
        /// 解析表达式值
        /// </summary>
        /// <param name="express"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ParseExpressValue(string express, object data)
        {
            MacroDef macro = GetMacroCommand(express);
            return ParseExpressValue(macro, data);
        }

        /// <summary>
        /// 解析表达式值
        /// </summary>
        /// <returns></returns>
        public string ParseExpressValue(MacroDef macro,object data)
        {
            if (macro == null) return string.Empty;
            switch (macro.GroupName.ToMyString())
            {
                case "DataConvert":
                    DataConvert(macro, data);
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// 通讯获取数据源入参转换
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="byData"></param>
        public string DataConvert(MacroDef macro,object data)
        {
            if (macro == null) return string.Empty;
            switch (macro.Name.ToMyString())
            {
                case "$ToBinString":    //
                    return data.ValToBinString();
                default:
                    return string.Empty;                    
            }
        }
    }

    /// <summary>
    /// 脚本函数解析器 -- DataAccess 数据访问
    /// </summary>
    public partial class MacroCommand: Accessor
    {
        /// <summary>
        /// 解析脚本函数- 转换成命令
        /// </summary>
        /// <param name="MacroExpress"></param>
        /// <returns></returns>
        public static object ParseCommand(string MacroExpress)
        {
            object ObjRet = null;
            string strFuncName = MacroExpress.MidString("", "(").Trim();
            List<string> ArrParam = MacroExpress.MidString("(", ")").MySplit(",");
            switch (strFuncName)
            {
                case "GetSubPosView":
                    if (!string.IsNullOrEmpty(ArrParam[0]))
                    {
                        //if(ArrParam[0].ToLower().Contains("dryer"))
                        //    ObjRet = new ModelDryer() { GroupKey = ArrParam[0] };
                        //else if (ArrParam[0].ToLower().Contains("preparelib"))
                        //    ObjRet = new ModelCabinet() { GroupKey = ArrParam[0] };
                        //else 
                            ObjRet = new ModelSubPos() { GroupKey = ArrParam[0] };
                    }
                        
                    break;

                case "GetView":
                    break;
            }
            return ObjRet;
        }

        /// <summary>
        /// 调用脚本函数
        /// </summary>
        /// <param name="FuncName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object CallMacroFunc(string MacroExpress)
        {
            object ObjRet = null;
            string strFuncName = MacroExpress.MidString("", "(");
            List<string> ArrParam = MacroExpress.MidString("(", ")").MySplit(",");
            switch (strFuncName)
            {
                case "GetSubPosView":
                    if (ArrParam.Count == 1)
                        ObjRet = GetSubPosView(ArrParam[0]);
                    break;
                case "GetView":
                    break;
            }
            return ObjRet;
        }

        /// <summary>
        /// 获取子工位视图
        /// </summary>
        /// <param name="PosName"></param>
        /// <returns></returns>
        private List<ModelSubPos> GetSubPosView(string PosName)
        {
            ModelSubPos model = new ModelSubPos()
            {
                GroupKey =
                string.Format("GroupKey='{0}' Order by InjectTime,OrderID asc", PosName)
                .MarkExpress().MarkWhere()
            };
            List<ModelSubPos> LstPosItem = GetSubPosList<ModelSubPos>(model);
            return LstPosItem;
        }
    }
}
