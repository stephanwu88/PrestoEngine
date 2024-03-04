using Engine.Common;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.ComDriver.SPECTROL
{
    /// <summary>
    /// 分析数据处理
    /// </summary>
    /// <typeparam name="TConNode"></typeparam>
    public partial class sComLASCmd<TConNode>
    {
        /// <summary>
        /// 定义数据统计判定委托
        /// </summary>
        /// <param name="SourceDic"></param>
        /// <param name="AnaNodeID"></param>
        /// <param name="CurrentPotElem"></param>
        /// <param name="ObjectDic"></param>
        /// <param name="RoundingSinglePotElem"></param>
        /// <param name="RoundingAvgPotElem"></param>
        /// <param name="SampleDataState"></param>
        /// <param name="ParseResult"></param>
        /// <returns></returns>
        public delegate bool FuncStatisticAnaData(ref JDictionary SourceDic, int AnaNodeID, List<string[]> CurrentPotElem,
            out JDictionary ObjectDic,
            out List<string[]> RoundingSinglePotElem,
            out List<string[]> RoundingAvgPotElem,
            ref string SampleDataState,
            out string ParseResult);

        /// <summary>
        /// 数据判定方法
        /// </summary>
        public FuncStatisticAnaData ActionStatisticAnaData { get; set; }

        /// <summary>
        /// 当前分析值
        /// </summary>
        public ModelOESData CurrentData = new ModelOESData();
        /// <summary>
        /// 修约后分析值
        /// </summary>
        public ModelOESData RoundingCurrentData = new ModelOESData();
        /// <summary>
        /// 修约后平均值
        /// </summary>
        public ModelOESData RoundingAvgData = new ModelOESData();
        /// <summary>
        /// 样品分析数据状态
        /// </summary>
        public string SampleAnaDataState = string.Empty;

        /// <summary>
        /// 分析数据源字典
        /// </summary>
        public JDictionary AnaDataSourceDic { get => _AnaDataSourceDic; set { _AnaDataSourceDic = value; } }

        private JDictionary _AnaDataSourceDic = new JDictionary();

        /// <summary>
        /// 数据统计运算字典
        /// </summary>
        public JDictionary AnaStatisticDic { get=> _AnaStatisticDic; set { _AnaStatisticDic = value; } }

        private JDictionary _AnaStatisticDic = new JDictionary();

        /// <summary>
        /// 解析分析数据点
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        public bool ParseResultDataPoint(AutomationCommand Command, ref JDictionary SourceDic, out JDictionary ObjectDic,
            out List<string[]> CurrentPotElem,
            out List<string[]> RoundingSinglePotElem,
            out List<string[]> RoundingAvgPotElem,
            out string ParseResult)
        {
            int AnaNodeID = 0;
            ObjectDic = null;
            CurrentPotElem = new List<string[]>();
            RoundingSinglePotElem = new List<string[]>();
            RoundingAvgPotElem = new List<string[]>();
            ParseResult = string.Empty;
            try
            {    //解析分析点数据电文
                bool ParseSuccess = ParseTelegramResultDataPoint(Command, out AnaNodeID, out CurrentPotElem);
                if (!ParseSuccess)
                {
                    ObjectDic = SourceDic;
                    ParseResult = "数据报文解析失败";
                    return false;
                }

                //获取当前数据点
                //int CurrentAnaNodeID = AnaNodeID;
                SourceDic["SparkPotsNumber"] = SourceDic["SparkPotsNumber"].ToMyInt() + 1;
                int CurrentAnaNodeID = SourceDic["SparkPotsNumber"].ToMyInt();

                //单点数据封封装到EF对象
                CurrentData = new ModelOESData()
                {
                    SampleID = Command?.Return?.SampleResults?.SampleResult?.Name.ToMyString(),
                    DataGroup = $"S{CurrentAnaNodeID.ToMyString()}"
                };
                //添加元素分析值到SrcField字典
                foreach (string[] item in CurrentPotElem)
                {
                    CurrentData.DicSrcField.AppandDict($"ELEM_{item[0].ToUpper()}", item[1].RoundToDecimalAndFormat(6, "0.000000"));
                }
                //附加数据时间
                string strTime = Command?.Return?.SampleResults?.XMLCreationDateTime.ToMyString().ToMyDateTimeStr();
                CurrentData.RecTime = SystemDefault.StringTimeNow;
                CurrentData.AnaEndTime = strTime;

                //判定有没有 附加【数据统计判定】委托，没有则根据数据点数简单报结果
                if (ActionStatisticAnaData == null)
                {
                    if (CurrentAnaNodeID >= 2)
                        ParseResult = "AnaFinish";
                    else
                        ParseResult = "MovePot";
                    return true;
                }

                //调用委托方法判定数据
                bool IsHandled = ActionStatisticAnaData.Invoke(ref SourceDic, CurrentAnaNodeID, CurrentPotElem,
                    out ObjectDic, out RoundingSinglePotElem, out RoundingAvgPotElem,ref SampleAnaDataState, out ParseResult);

                //添加修约后单点数据值
                RoundingCurrentData = new ModelOESData()
                {
                    SampleID = Command?.Return?.SampleResults?.SampleResult?.Name.ToMyString(),
                    DataGroup = $"SD{CurrentAnaNodeID.ToMyString()}"
                };
                //添加元素分析值到SrcField字典
                foreach (string[] item in RoundingSinglePotElem)
                {
                    RoundingCurrentData.DicSrcField.AppandDict($"ELEM_{item[0].ToUpper()}", item[1]);
                }
                //附加数据时间
                strTime = Command?.Return?.SampleResults?.XMLCreationDateTime.ToMyString().ToMyDateTimeStr();
                RoundingCurrentData.RecTime = SystemDefault.StringTimeNow;
                RoundingCurrentData.AnaEndTime = strTime;

                //分析结束时添加修约后AVG平均值
                if (IsHandled && ParseResult== "AnaFinish")
                {
                    //取统计后的AvgPotElem 平均数据封装入库
                    RoundingAvgData = new ModelOESData()
                    {
                        SampleID = Command?.Return?.SampleResults?.SampleResult?.Name.ToMyString(),
                        DataGroup = $"AVG"
                    };
                    foreach (string[] item in RoundingAvgPotElem)
                    {
                        RoundingAvgData.DicSrcField.AppandDict($"ELEM_{item[0].ToUpper()}", item[1]);
                    }
                    RoundingAvgData.RecTime = SystemDefault.StringTimeNow;
                    RoundingAvgData.AnaEndTime = SystemDefault.StringTimeNow;
                }
                return IsHandled;
            }
            catch (Exception ex)
            {
                ParseResult = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 解析数据结果电文
        /// </summary>
        /// <param name="_command">指令报文对象</param>
        /// <param name="AnaNodeID">分析数据点</param>
        /// <param name="LstElemData">元素字段-值</param>
        /// <returns></returns>
        public bool ParseTelegramResultDataPoint(AutomationCommand _command, out int AnaNodeID, out List<string[]> LstElemData)
        {
            string strNodeID = string.Empty;
            try
            {
                //AutomationCommand _command = sCommon.XmlDeserialize<AutomationCommand>(xml);
                LstElemData = new List<string[]>();
                //去除报文-数据点号
                AnaNodeID = _command.Return.SampleResults.SampleResult.MeasurementReplicates.Count.ToMyInt();
                strNodeID = AnaNodeID.ToMyString();
                //获取分析数据元素-值段落
                var lst = _command.Return.SampleResults.SampleResult.MeasurementReplicates.MeasurementReplicate.ToList();
                foreach (var item in lst)
                {
                    var lst2 = item.Measurement.Elements.ToList();
                    foreach (var item2 in lst2)
                    {
                        string strElemName = item2.ElementName.ToMyString();
                        string strElemValue = item2.ElementResult[0].ResultValue;
                        LstElemData.Add(new string[] { strElemName, strElemValue });
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                AnaNodeID = 0;
                Logger.Error.Write(LOG_TYPE.ERROR, $"数据激发点[{strNodeID}]电文解析错误: {_command?.Name}");
                LstElemData = null;
                return false;
            }
        }
    }
}
