using Engine.Common;
using Engine.Data.DBFAC;
using Engine.Mod;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 激发数据处理结果 移点 结束
    /// </summary>
    public enum DataPointParseResult
    {
        /// <summary>
        /// 需要激发下一个点
        /// </summary>
        NeedNexPot,
        /// <summary>
        /// 分析结束
        /// </summary>
        AnaFinish
    }

    /// <summary>
    /// 关于光谱分析Gama值的判定
    /// 各点分析元素汇总 Mean、Range、RSD统计 数据修约  激发点判定
    /// </summary>
    public partial class SparkerAutomtion
    {
        /// <summary>
        /// 统计分析数据
        /// </summary>
        /// <param name="SourceDic"></param>
        /// <param name="AnaNodeID"></param>
        /// <param name="CurrentPotElem"></param>
        /// <param name="ObjectDic"></param>
        /// <param name="RoundingSinglePotElem"></param>
        /// <param name="RoundingAvgPotElem">平均数据 元素-值</param>
        /// <param name="SampleAnaDataState">样品分析数据状态</param>
        /// <param name="ParseResult">MovePot or AnaFinish</param>
        /// <returns></returns>
        public bool StatisticAnaData(ref JDictionary SourceDic, int AnaNodeID, List<string[]> CurrentPotElem,
            out JDictionary ObjectDic,
            out List<string[]> RoundingSinglePotElem,
            out List<string[]> RoundingAvgPotElem,
            ref string SampleAnaDataState,
            out string ParseResult)
        {
            RoundingSinglePotElem = new List<string[]>();
            RoundingAvgPotElem = new List<string[]>();
            ObjectDic = new JDictionary();
            try
            {
                //Step1. 附加数据点
                AppandAnaResult(ref SourceDic, AnaNodeID, CurrentPotElem, out RoundingSinglePotElem);
                //Step2. 综合判定数据  MovePot or AnaFinish
                ParseResult = ParseDataPoint(SourceDic, AnaNodeID, out ObjectDic, out RoundingAvgPotElem, ref SampleAnaDataState);
                //Step3. 标准化测量，自动添加到标准化数据库
                if (ParseResult == "AnaFinish" && (CurrentAnaProben?.Type.MyContains("标样|控样|类标样", "|") == true))
                {
                    //标准化样的偏差计算
                    ModelProbenElem ModProben = new ModelProbenElem() { ProbenToken = CurrentAnaProben.ProbenToken };
                    CalucateElementList_TS(ModProben, RoundingAvgPotElem);
                }
                if (ParseResult == "AnaFinish" && CurrentAnaProben?.Type == "检查样")
                {
                    //标准化样的偏差计算
                    ModelProbenElem ModProben = new ModelProbenElem() { ProbenToken = CurrentAnaProben.ProbenToken };
                    CalucateElementList_CS(MaterialMain, ModProben, RoundingAvgPotElem);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Task.Write(LOG_TYPE.ERROR, $"StatisticAnaData 发生错误[{ex.ToString()}]");
                ParseResult = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 仪器分析点 单点元素附加到字典数据点段落 
        /// </summary>
        /// <param name="jDic"></param>
        /// <param name="AnaNodeID"></param>
        /// <param name="LstElemData">分析元素-值列表</param>
        public void AppandAnaResult(ref JDictionary jDic, int AnaNodeID, List<string[]> LstElemData, out List<string[]> RoundingSinglePotElem)
        {
            try
            {
                //获取分析方法对应的拓展元素格式设置
                List<ModelElemBase> LstExtentElemBase = Helper.GetAnaPgmExtendElemList();
                List<ModelMaterialFull> LstMaterialFull = null;
                //添加数据到字典
                JDictionary AnaData = new JDictionary();
                RoundingSinglePotElem = new List<string[]>();
                //带控样修正
                if ((CurrentAnaProben?.Type).MyContains("生产样|检查样", "|") && (SteelTypeItem?.MaterialToken).IsNotEmpty())
                    LstMaterialFull = Helper.GetMaterialRelatedAllByToken(SteelTypeItem.MaterialToken);

                //将单点数据数据附加到字典中
                bool SparkPotOK = true;
                foreach (string[] item in LstElemData)
                {
                    //分析元素-值 添加到Element段落
                    double dbData = item[1].ToMyDouble();
                    AnaData["Element"].SetValue(item[0], dbData);

                    //带控样修正
                    ModelMaterialFull modelMaterial = LstMaterialFull?.MySelectFirst(x => x.Element == item[0]);
                    if (modelMaterial != null && (modelMaterial?.TolMethod).IsNotEmpty())
                        dbData = ElementCorrectionValue(item, modelMaterial, ref jDic);
                    //修正后值添加 - 无关联修正项添加原值
                    AnaData["CorrectionElement"].SetValue(item[0], dbData);

                    //单点值修约 添加到RoundingElement段落
                    ModelElemBase ElemBase = LstExtentElemBase.MySelectFirst(x => x.Element == item[0]);
                    string RoundValue = RoundingElementData(dbData, ElemBase);
                    AnaData["RoundingElement"].SetValue(item[0], RoundValue);
                    RoundingSinglePotElem.Add(new string[] { item[0], RoundValue });

                    //通过分析元素上下限判定数据点状态 OK NG
                    bool IsElementOutRange = ElementOutRange(dbData, ElemBase);
                    bool IsMaterialOutRange = MaterialOutRange(dbData, modelMaterial);
                    if (IsElementOutRange || IsMaterialOutRange) SparkPotOK = false;
                    AnaData["ElementOutRange"].SetValue(item[0], IsElementOutRange ? "Y" : "N");
                }

                //添加数据点状态
                AnaData["PotState"] = SparkPotOK ? "OK" : "NG";

                //计算公式元素 单点 ex:Ceq
                IEnumerable<ModelElemBase> LstElemBase = LstExtentElemBase.MySelect(x => x.MassCalcuteExpress.IsNotEmpty());
                foreach (ModelElemBase item in LstElemBase)
                {
                    if (item.Element.IsEmpty())
                        continue;
                    //取修正后的单点数据 作为数据源公式计算 伪元素 并修约
                    Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(AnaData["CorrectionElement"].ToMyString());
                    double dbData = DynamicCalculate(item.MassCalcuteExpress, dic);
                    AnaData["CorrectionElement"].SetValue(item.Element, dbData);
                    //伪元素 单点值修约 添加到RoundingElement段落
                    string RoundValue = RoundingElementData(dbData, item);
                    AnaData["RoundingElement"].SetValue(item.Element, RoundValue);
                    RoundingSinglePotElem.Add(new string[] { item.Element, RoundValue });
                    //通过分析元素上下限判定数据点状态 OK NG
                    bool IsElementOutRange = ElementOutRange(dbData, item);
                    if (IsElementOutRange) SparkPotOK = false;
                    AnaData["ElementOutRange"].SetValue(item.Element, IsElementOutRange ? "Y" : "N");
                }

                //整个节点 附加到 数据点号段落
                jDic[AnaNodeID.ToString()] = AnaData.Root;
            }
            catch (Exception ex)
            {
                throw new Exception($"AppandAnaResult 发生错误：[{ex.ToString()}]");
            }
        }

        /// <summary>
        /// 解析数据点、统计、修约
        /// </summary>
        /// <param name="SourceDic"></param>
        /// <param name="ObjectDic"></param>
        /// <param name="RoundingAvgPotElem"></param>
        /// <param name="SampleAnaDataState"></param>
        /// <returns></returns>
        public string ParseDataPoint(JDictionary SourceDic,int AnaNodeID, out JDictionary ObjectDic, out List<string[]> RoundingAvgPotElem,ref string SampleAnaDataState)
        {
            try
            {
                //将原始字典克隆到目标(显示)字典
                SourceDic.CloneTo(out ObjectDic);

                //拓展元素格式控制  包含修约
                List<ModelElemBase> LstExtentElemBase = Helper.GetAnaPgmExtendElemList();
                //对于生产样 定义偏差Gama设置列表
                List<ModelMaterialDeviation> LstAnaGama = null;
                List<ModelMaterialFull> LstMaterialFull = null;
                if (CurrentAnaProben?.Type.MyContains("生产样|检查样", "|") == true)
                {
                    //获取钢种关联牌号的偏差设置列表
                    LstAnaGama = CurrentMaterialDeviations;
                }
                RoundingAvgPotElem = new List<string[]>();
                //存在有效的激发点
                bool HasValidSparkPots = SparkPotsCode.IsNotEmpty() && SparkPotsCode.CountOfSub("1") > AnaNodeID;
                string result = string.Empty;
                try
                {
                    //Step1. 筛选数据
                    //剔除无效激发点，将有效数据点集参与统计运算
                    Dictionary<string, List<double>> ValidElemValueDic = new Dictionary<string, List<double>>();
                    Dictionary<string, List<int>> ValidElemPotNumDic = new Dictionary<string, List<int>>();
                    foreach (JProperty item in ObjectDic.Root.Children())
                    {
                        if (item.Value is JObject jObj)
                        {
                            //获取各激发点的数据列表
                            JProperty elementProperty = jObj.Property("CorrectionElement");
                            if (elementProperty == null)
                                continue;
                            JObject element = (JObject)item.Value["CorrectionElement"];
                            //剔除NG激发点
                            string PotState = item.Value["PotState"].ToMyString();
                            if (PotState == "NG")
                                continue;
                            //字典  元素名称 元素每次激发值
                            foreach (JProperty ElemItem in element.Children())
                            {
                                if (!ValidElemValueDic.ContainsKey(ElemItem.Name))
                                {
                                    ValidElemValueDic.AppandDict(ElemItem.Name, new List<double>());
                                    ValidElemPotNumDic.AppandDict(ElemItem.Name, new List<int>());
                                }
                                ValidElemValueDic[ElemItem.Name].Add(ElemItem.Value.ToMyDouble());
                                ValidElemPotNumDic[ElemItem.Name].Add(item.Name.ToMyInt());
                            }
                        }
                    }

                    //Step2. 有效数据点统计并进行Gama判定
                    //数据统计 有效数据点进行 Gama判定 Key; 元素  值：点集
                    //Dictionary<string, List<double>> ValidElemValueDic
                    int iDataPointNum = 0;  //有效数据点数量
                    bool ErrorHappend = false;
                    foreach (KeyValuePair<string, List<double>> ElemKeyVal in ValidElemValueDic)
                    {
                        try
                        {
                            string eleName = ElemKeyVal.Key;
                            List<double> eleValues = ElemKeyVal.Value;
                            iDataPointNum = eleValues.MyCount();
                            if (iDataPointNum < 2)
                            {
                                //点不够,再要一个点
                                result = "NeedNextPot";
                                continue;
                            }
                            //计算并修约 Range 极差  Mean 所有值平均 SD 标准差  RSD 相对标准差
                            double dRange = eleValues.CalculateRange();
                            double dMean = eleValues.CalculateMean();
                            double dSD = eleValues.CalculateStandardDeviation();
                            double dRSD = eleValues.CalculateRelativeStandardDeviation();
                            ModelElemBase ElemBase = LstExtentElemBase.MySelectFirst(x => x.Element == eleName);
                            string strMean = RoundingElementData(dMean, ElemBase);
                            string strRange = RoundingElementData(dRange, ElemBase);
                            string strSD = RoundingElementData(dSD, ElemBase);
                            string strRSD = RoundingElementData(dRSD, ElemBase);
                            //装载到显示字典
                            ObjectDic["Mean"].SetValue(eleName, strMean);
                            ObjectDic["SD"].SetValue(eleName, strSD);
                            ObjectDic["RSD"].SetValue(eleName, strRSD);
                            ObjectDic["Range"].SetValue(eleName, strRange);

                            //控样 标样 检查样 至少3个点
                            if (CurrentAnaProben?.Type.MyContains("标样|控样|类标样", "|") == true)
                            {
                                //直接报出所有点平均值
                                ObjectDic["AVG"].SetValue(eleName, strMean);
                                RoundingAvgPotElem.Add(new string[] { eleName, strMean });
                                if (iDataPointNum < 3)
                                    result = "NeedNextPot";
                            }
                            else
                            {
                                //生产样报出平均数 国标逻辑 目的： 1、判断移点、结束 2、报出平均数
                                //情况1. 没有启用Gama判定 或者 Gama判定通过 报出平均数
                                ModelMaterialDeviation gamaSet = LstAnaGama?.MySelectFirst(x => x.Element == eleName);
                                bool HasGamaSetting = gamaSet?.IsEnabled.ToMyBool() == true && (gamaSet?.GamaMode).IsNotEmpty();
                                bool GamaComparePass = false;
                                if (gamaSet?.IsEnabled.ToMyBool() == true)
                                {
                                    //情况2. 该元素有Gama设置项，计算Gama值与极差比较判定
                                    //取最后的数据元素值 根据该元素Gama设置项 计算 mass -> Gama值
                                    //double dElemMass = ObjectDic[iDataPointNum.ToString()]["Element"][eleName].ToMyDouble();
                                    double dElemMass = SourceDic["SetPoint"].GetString(eleName).ToMyDouble();
                                    double dGamaValue = GetGamaValue(iDataPointNum, dElemMass, gamaSet, ref SourceDic, ref ObjectDic);
                                    //当前分析点的元素 在Gama-极差 偏差管理通过 or Gama值计算失败 都算通过
                                    GamaComparePass = (dGamaValue != SystemDefault.InValidDouble && dRange <= dGamaValue) ||
                                        dGamaValue == SystemDefault.InValidDouble;
                                    //记录数据点极差状态 极差-Gama 过 不过
                                    ObjectDic["RangeState"].SetValue(eleName, GamaComparePass ? "OK" : "NG");
                                }
                                //没有启用Gama判定 或者 Gama判定通过 报出平均数
                                if (!HasGamaSetting || GamaComparePass)
                                {
                                    ObjectDic["AVG"].SetValue(eleName, strMean);
                                    RoundingAvgPotElem.Add(new string[] { eleName, strMean });
                                    for (int i = 1; i <= iDataPointNum; i++)
                                        ObjectDic[i.ToString()].NewJNode("ElementGamaState").SetValue(eleName, "OK");
                                }
                                //启用Gama判定 并且 判定没有通过 则 
                                //1、两个点 - 再测量一个  
                                //2、三个点 - 可取中间、可再测量1个  主要取决于视觉结果
                                //3、四个点 - 取中间两个点平均
                                if (HasGamaSetting && !GamaComparePass)
                                {
                                    if (iDataPointNum == 2 || (iDataPointNum == 3 && HasValidSparkPots))
                                    {
                                        ObjectDic["AVG"].SetValue(eleName, strMean);
                                        RoundingAvgPotElem.Add(new string[] { eleName, strMean });
                                        result = "NeedNextPot";
                                    }
                                    else
                                    {
                                        //去除最大最小值报平均
                                        List<double> dLst = ValidElemValueDic[eleName];
                                        int iMinIndex = dLst.IndexOf(dLst.Min());
                                        int iMaxIndex = dLst.IndexOf(dLst.Max());
                                        List<int> indexes = Enumerable.Range(0, dLst.Count).Where(i => i != iMaxIndex && i != iMinIndex).ToList();
                                        //标记参与平均的元素点 ElementGamaState标记为NG表示不参与平均
                                        for (int idx = 0; idx < dLst.Count; idx++)
                                        {
                                            int index = ValidElemPotNumDic.DictFieldValue(eleName).MySelectAny(idx);
                                            ObjectDic[index.ToString()].NewJNode("ElementGamaState").SetValue(eleName, indexes.Contains(idx) ? "OK" : "NG");
                                        }

                                        //将去除最大最小后的剩余元素 加权平均
                                        List<double> dLeave = new List<double>();
                                        for (int i = 0; i < dLst.Count; i++)
                                        {
                                            if (i == iMaxIndex || i == iMinIndex)
                                                continue;
                                            dLeave.Add(dLst[i]);
                                        }
                                        //去除最大值最小值 报平均
                                        if (dLeave.Count > 0)
                                        {
                                            double dAvg = dLeave.CalculateMean();
                                            string strAvg = RoundingElementData(dAvg, ElemBase);
                                            ObjectDic["AVG"].SetValue(eleName, strAvg);
                                            RoundingAvgPotElem.Add(new string[] { eleName, strAvg });
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorHappend = true;
                        }
                    }

                    //Step3. 处理配置的伪元素的数据及相关统计
                    //附加需要计算理论值的 虚拟元素的 各项统计指标 ex:Ceq 的 Means RSD AVG
                    //取得 公式计算型  拓展元素配置项
                    List<string> LstField = new List<string>() { "Mean", "RSD", "AVG" };
                    IEnumerable<ModelElemBase> LstElemBase = LstExtentElemBase.MySelect(x => x.MassCalcuteExpress.IsNotEmpty());
                    if (LstElemBase.MyCount() > 0)
                    {
                        foreach (ModelElemBase item in LstElemBase)
                        {
                            if (item.Element.IsEmpty())
                                continue;
                            //循环统计指标
                            foreach (string fld in LstField)
                            {
                                Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(ObjectDic[fld].ToMyString());
                                if (dic?.Values?.Count <= 0)
                                    continue;
                                //将计算基础数据字典放到公式计算器中计算 得到 计算元素的 对应统计指标
                                double dbData = DynamicCalculate(item.MassCalcuteExpress, dic);
                                string RoundValue = RoundingElementData(dbData, item);
                                //附加到字典
                                ObjectDic[fld].SetValue(item.Element, RoundValue);
                                if (fld == "AVG")
                                    RoundingAvgPotElem.Add(new string[] { item.Element, RoundValue });
                            }
                        }
                    }

                    //Step4. 产生激发点需求结果
                    if (result == "NeedNextPot" || ErrorHappend)
                        result = "MovePot";
                    if (!ErrorHappend && string.IsNullOrEmpty(result))
                        result = "AnaFinish";

                    //Step5. 对于逻辑还需要点的情况，判断还有没有点可用
                    //if ((CurrentAnaProben?.Type).MyContains("生产样|检查样", "|") && result == "MovePot" && !HasValidSparkPots)
                    //{
                    //    SampleAnaDataState = "激发点不足";
                    //    result = "AnaFinish";
                    //}

                    //Step6. 分析结果完成时，将AVG报数进行牌号上下限判定，并标记数据状态
                    if (result == "AnaFinish")
                    {
                        try
                        {
                            SampleAnaDataState = "数据合格";
                            Dictionary<string, string> AvgDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(ObjectDic["AVG"].ToMyString());
                            if ((CurrentAnaProben?.Type).MyContains("生产样|检查样", "|") && (SteelTypeItem?.MaterialToken).IsNotEmpty())
                                LstMaterialFull = Helper.GetMaterialRelatedAllByToken(SteelTypeItem.MaterialToken);
                            IEnumerable<ModelMaterialFull> LstMaterialElem = LstMaterialFull.MySelect(x => x.LimitMin.IsNotEmpty() || x.LimitMax.IsNotEmpty());
                            if (AvgDic != null && LstMaterialElem.MyCount() > 0)
                            {
                                //判定牌号上下限
                                foreach (ModelMaterialFull materialItem in LstMaterialElem)
                                {
                                    string avgData = AvgDic.DictFieldValue(materialItem.Element);
                                    if (materialItem.Element.IsEmpty() || !avgData.IsNumeric())
                                        continue;
                                    double dbData = avgData.ToMyDouble();
                                    bool IsOutRange = MaterialOutRange(dbData, materialItem);
                                    if (IsOutRange) SampleAnaDataState = "牌号超限";
                                    ObjectDic["MaterialOutRange"].SetValue(materialItem.Element, IsOutRange ? "Y" : "N");
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Task.Write(LOG_TYPE.ERROR, $"ParseDataPoint 发生错误[{ex.ToString()}]");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"ParseDataPoint 发生错误：[{ex.ToString()}]");
            }
        }

        /// <summary>
        /// 获取gama值 固定Gama or Gama公式
        /// </summary>
        /// <param name="DataPointID"></param>
        /// <param name="ElemMass"></param>
        /// <param name="gamaSet"></param>
        /// <returns></returns>
        public double GetGamaValue(int DataPointID, double ElemMass, ModelMaterialDeviation gamaSet, ref JDictionary SourceDic, ref JDictionary ObjectDic)
        {
            if (gamaSet == null || !DataPointID.IsInOfRange(2, 4))
                return SystemDefault.InValidDouble;
            double dGamaValue = SystemDefault.InValidDouble;
            try
            {
                if (gamaSet.GamaMode == "r常量")
                {
                    if (DataPointID == 2 && !string.IsNullOrEmpty(gamaSet.Gama1))
                    {
                        dGamaValue = gamaSet.Gama1.ToMyDouble();
                    }
                    else if (DataPointID == 3)
                    {
                        if (!string.IsNullOrEmpty(gamaSet.Gama2))
                            dGamaValue = gamaSet.Gama2.ToMyDouble();
                        else if (!string.IsNullOrEmpty(gamaSet.Gama1))
                            dGamaValue = 1.2 * gamaSet.Gama1.ToMyDouble();
                    }
                    else if (DataPointID == 4)
                    {
                        if (!string.IsNullOrEmpty(gamaSet.Gama3))
                            dGamaValue = gamaSet.Gama3.ToMyDouble();
                        else if (!string.IsNullOrEmpty(gamaSet.Gama1))
                            dGamaValue = 1.3 * gamaSet.Gama1.ToMyDouble();
                    }
                }
                else if (gamaSet.GamaMode == "r公式")
                {
                    double dCalcGama = CalculateGama(gamaSet.GamaExpress, gamaSet.A.ToMyDouble(), gamaSet.B.ToMyDouble(), ElemMass);
                    if (dCalcGama != SystemDefault.InValidDouble)
                    {
                        if (DataPointID == 2)
                            dGamaValue = dCalcGama;
                        else if (DataPointID == 3)
                            dGamaValue = 1.2 * dCalcGama;
                        else if (DataPointID == 4)
                            dGamaValue = 1.3 * dCalcGama;
                    }
                }

                if (DataPointID == 2)
                {
                    ObjectDic?["Gama1"].SetValue(gamaSet?.Element, dGamaValue.RoundToDefaultDecimalAndFormat());
                    SourceDic?["Gama1"].SetValue(gamaSet?.Element, dGamaValue.RoundToDefaultDecimalAndFormat());
                }
                else if (DataPointID == 3)
                {
                    ObjectDic?["Gama2"].SetValue(gamaSet?.Element, dGamaValue.RoundToDefaultDecimalAndFormat());
                    SourceDic?["Gama2"].SetValue(gamaSet?.Element, dGamaValue.RoundToDefaultDecimalAndFormat());
                }
                else if (DataPointID == 4)
                {
                    ObjectDic?["Gama3"].SetValue(gamaSet?.Element, dGamaValue.RoundToDefaultDecimalAndFormat());
                    SourceDic?["Gama3"].SetValue(gamaSet?.Element, dGamaValue.RoundToDefaultDecimalAndFormat());
                }
            }
            catch (Exception)
            {

            }           
            return dGamaValue;
        }

        /// <summary>
        /// 元素值修约
        /// </summary>
        /// <returns></returns>
        public string RoundingElementData(string ElemName, double dbData, List<ModelElemBase> LstExtentElemBase)
        {
            string RoundValue = string.Empty;
            ModelElemBase ElemBase = LstExtentElemBase.MySelectFirst(x => x.Element == ElemName);
            if (!string.IsNullOrEmpty(ElemBase?.DecimalDigits) && !string.IsNullOrEmpty(ElemBase?.DataFormat))
                RoundValue = dbData.RoundToDecimalAndFormat(ElemBase.DecimalDigits.ToMyInt(), ElemBase.DataFormat);
            if (RoundValue.IsEmpty())
                RoundValue = dbData.RoundToDefaultDecimalAndFormat();
            return RoundValue;
        }

        /// <summary>
        /// 元素值修约
        /// </summary>
        /// <param name="dbData"></param>
        /// <param name="ElemBase"></param>
        /// <returns></returns>
        public string RoundingElementData(double dbData, ModelElemBase ElemBase)
        {
            string RoundValue = string.Empty;
            if (!string.IsNullOrEmpty(ElemBase?.DecimalDigits) && !string.IsNullOrEmpty(ElemBase?.DataFormat))
                RoundValue = dbData.RoundToDecimalAndFormat(ElemBase.DecimalDigits.ToMyInt(), ElemBase.DataFormat);
            if (RoundValue.IsEmpty())
                RoundValue = dbData.RoundToDefaultDecimalAndFormat();
            return RoundValue;
        }

        /// <summary>
        /// 判断分析曲线数据上下限
        /// </summary>
        /// <param name="ElemBase"></param>
        /// <returns></returns>
        public bool ElementOutRange(double dbData, ModelElemBase ElemBase)
        {
            bool IsElementOutRange = false;
            try
            {
                if (ElemBase == null)
                    return IsElementOutRange;
                bool IsMassUEnabled = ElemBase.MassRangeU.IsNotEmpty() && ElemBase.MassRangeU.IsNumeric();
                bool IsMassDEnabled = ElemBase.MassRangeD.IsNotEmpty() && ElemBase.MassRangeD.IsNumeric();
                if (IsMassUEnabled && IsMassDEnabled && ElemBase.MassRangeU.ToMyDouble() < ElemBase.MassRangeD.ToMyDouble())
                {
                    IsMassUEnabled = false;
                    IsMassDEnabled = false;
                }
                if ((IsMassUEnabled && dbData > ElemBase.MassRangeU.ToMyDouble()) ||
                    (IsMassDEnabled && dbData < ElemBase.MassRangeD.ToMyDouble()))
                {
                    IsElementOutRange = true;
                }
            }
            catch (Exception ex)
            {
                
            }
            return IsElementOutRange;
        }

        /// <summary>
        /// 判断牌号上下限超范围
        /// </summary>
        /// <param name="dbData"></param>
        /// <param name="materialItem"></param>
        /// <returns></returns>
        public bool MaterialOutRange(double dbData, ModelMaterialFull materialItem)
        {
            bool IsMaterialOutRange = false;
            try
            {
                if (materialItem.Element.IsEmpty())
                    return IsMaterialOutRange;
                bool LimitUValid = (materialItem?.LimitMax).IsNumeric() && (materialItem?.LimitMax).IsNotEmpty();
                bool LimitDValid = (materialItem?.LimitMin).IsNumeric() && (materialItem?.LimitMin).IsNotEmpty();
                bool StdValueHas = (materialItem?.T1_SetPoint).IsNumeric() && (materialItem?.T1_SetPoint).IsNotEmpty();
                double dLimitU = materialItem.LimitMax.ToMyDouble();
                double dLimitD = materialItem.LimitMin.ToMyDouble();
                double dStd = materialItem.T1_SetPoint.ToMyDouble();
                bool RangeValid = dLimitU > dLimitD;
                bool LimitUOutRange = LimitUValid && RangeValid && dbData > dLimitU;
                bool LimitDOutRange = LimitDValid && RangeValid && dbData < dLimitD;
            }
            catch (Exception ex)
            {

            }
            return IsMaterialOutRange;           
        }

        /// <summary>
        /// 计算Gama公式
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private double CalculateGama(string expression, double A, double B, double m)
        {
            try
            {
                //ex: lgr=A *  lgm - B
                if (expression.Contains("="))
                {
                    string expLeft = expression.MidString("", "=").Trim();
                    string expRight = expression.MidString("=", "").Trim();
                    if (expLeft == "lgr")
                        expression = $"Math.Pow(10,{expRight})";
                    else
                        expression = expRight;
                }
                expression = expression.Replace("lgm", "Math.Log10(m)");
                //创建参数
                ParameterExpression parameterA = Expression.Parameter(typeof(double), "A");
                ParameterExpression parameterB = Expression.Parameter(typeof(double), "B");
                ParameterExpression parameterM = Expression.Parameter(typeof(double), "m");
                //构建表达式树
                Expression body = DynamicExpressionParser.ParseLambda(new[] { parameterA, parameterB, parameterM }, typeof(double), expression).Body;
                //编译表达式树成委托函数
                Func<double, double, double, double> calculator = Expression.Lambda<Func<double, double, double, double>>(body, parameterA, parameterB, parameterM).Compile();
                //调用委托函数计算结果
                double result = calculator(A, B, m);
                return result;
            }
            catch (Exception)
            {
                return SystemDefault.InValidDouble;
            }
        }

        /// <summary>
        /// 动态计算
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="DicParam"></param>
        /// <param name="ParamFieldMate"></param>
        /// <returns></returns>
        private double DynamicCalculate(string expression, Dictionary<string,string> DicParam, string ParamFieldMate = @"\[.*?\]")
        {
            if (DicParam.MyCount() == 0)
                return SystemDefault.InValidDouble;
            List<string[]> LstParam = new List<string[]>();
            foreach (KeyValuePair<string, string> item in DicParam)
            {
                LstParam.Add(new string[] { item.Key, item.Value });
            }
            return DynamicCalculate(expression, LstParam, ParamFieldMate);
        }

        /// <summary>
        /// 动态计算
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="LstParamKeyPair"></param>
        /// <param name="ParamFieldMate">默认 [x] 格式</param>
        /// <returns></returns>
        private double DynamicCalculate(string expression, List<string[]> LstParamKeyPair, string ParamFieldMate = @"\[.*?\]")
        {
            //https://baike.baidu.com/item/ceq/10235817?fr=ge_ala
            //碳钢及合金结构钢的碳当量经验公式：
            //C当量 =[C + Mn / 6 + (Cr + Mo + V) / 5 + (Ni + Cu) / 15] * 100 %
            //式中：C、Mn、Cr、Mo、V、Ni、Cu为钢中该元素含量
            //碳当量Ceq（百分比）值可按以下公式计算：
            //Ceq = C + Mn / 6 + (Cr + V + Mo) / 5 + (Cu + Ni) / 15
            //（碳当量Ceq的允许偏差为 + 0.03 %）

            //[C] + [Mn]/6 + ([Cr]+[Mo]+[V])/5 + ([Cu]+[Ni])/15 
            try
            {
                //找到参数元
                List<string> lstField = expression.RegexMatchedList(ParamFieldMate, true);
                //格式化源字典
                //创建参数
                string strHead = ParamFieldMate.MidString(1, 1);    //[
                string strEnd = ParamFieldMate.EndSubString(1);     //]
                foreach (var item in lstField)
                {
                    string OrginParam = item.MidString(strHead, strEnd, EndStringSearchMode.FromTailAndToEndWhenUnMatch);
                    string[] ParamKeyPair = LstParamKeyPair.MySelectFirst(x => x[0] == OrginParam);
                    if (ParamKeyPair.MyCount() != 2)
                        continue;
                    expression = expression.Replace(item, ParamKeyPair[1].ToMyString());
                }
                bool RegexMactched = expression.IsRegexMatched(ParamFieldMate);
                if(RegexMactched)
                    return SystemDefault.InValidDouble;
                //构建无参表达式树
                ParameterExpression[] parameters = new ParameterExpression[0];
                Expression body = DynamicExpressionParser.ParseLambda(parameters, null, expression).Body;
                //编译表达式树成委托函数
                Func<double> calculate = Expression.Lambda<Func<double>>(body).Compile();
                //执行委托获取计算结果
                return calculate();
            }
            catch (Exception ex)
            {
                return SystemDefault.InValidDouble;
            }
        }
    }

    /// <summary>
    /// 即时分析数据
    /// </summary>
    public partial class SparkerAutomtion
    {

    }

    /// <summary>
    /// 标准化样品分析数据处理 - 起始值，起始偏差，校正分析值，校正偏差，验证分析值，验证偏差
    /// </summary>
    public partial class SparkerAutomtion
    {
        /// <summary>
        /// 计算并更新控样元素列表
        /// </summary>
        /// <param name="ModelElem"></param>
        /// <param name="AvgPotElem"></param>
        public void CalucateElementList_TS(ModelProbenElem ModelElem,List<string[]> AvgPotElem = null)
        {
            try
            {
                List<ModelProbenElem> ElemList = Helper.GetProbenElement(ModelElem);
                if (ElemList == null)
                    return;
                //元素起始值需要填充
                bool StartValueNeeded = ElemList.MyContains(x => x.StartValue.IsEmpty());
                List<ModelProbenElem> LstUpd = new List<ModelProbenElem>();
                foreach (ModelProbenElem item in ElemList)
                {
                    if (item.ProbenToken.IsEmpty())
                        continue;
                    //创建修改对象
                    ModelProbenElem modUpd = item.ModelClone();
                    //若有分析点值填充，则先更改值
                    if (AvgPotElem != null)
                    {
                        string[] MatchArray = AvgPotElem?.MySelectFirst(x => x[0] == item.Element);
                        if (item.ProbenToken.IsEmpty() || MatchArray == null)
                            continue;

                        //若起始值存在缺失，则整个元素列表重更新起始值
                        if (StartValueNeeded) modUpd.StartValue = MatchArray.MySelectAny(1);
                        modUpd.ActualValue = MatchArray.MySelectAny(1);
                    }
                    //首次偏差
                    double dTolStart = Calculate("A-B-C", modUpd.SetPoint.ToMyDouble(), modUpd.StartValue.ToMyDouble(), 0);
                    if (dTolStart != SystemDefault.InValidDouble)
                        modUpd.TolStart = dTolStart.RoundToDefaultDecimalAndFormat();
                    //实际偏差
                    double dTolActual = Calculate("A-B-C", modUpd.SetPoint.ToMyDouble(), modUpd.ActualValue.ToMyDouble(), 0);
                    if (dTolActual != SystemDefault.InValidDouble)
                        modUpd.TolActual = dTolActual.RoundToDefaultDecimalAndFormat();
                    //乘法系数
                    double dMulScale = Calculate("A/B-C", modUpd.SetPoint.ToMyDouble(), modUpd.ActualValue.ToMyDouble(), 0);
                    if (dMulScale == SystemDefault.InValidDouble)
                        dMulScale = 1.0;
                    modUpd.MultiplitiveValue = dMulScale.RoundToDefaultDecimalAndFormat();
                    //加法偏移
                    double dAddOff = Calculate("A-B-C", modUpd.SetPoint.ToMyDouble(), modUpd.ActualValue.ToMyDouble(), 0);
                    if (dAddOff != SystemDefault.InValidDouble)
                        modUpd.AdditiveValue = dAddOff.RoundToDefaultDecimalAndFormat();
                    //数据更新到库
                    if (modUpd.ProbenToken.IsNotEmpty() && modUpd.Element.IsNotEmpty())
                    {
                        modUpd.ProbenToken = modUpd.ProbenToken.MarkWhere();
                        modUpd.Element = modUpd.Element.MarkWhere();
                        LstUpd.Add(modUpd);
                    }
                }
                if (LstUpd.MyCount() > 0)
                    DbMyCon?.ExcuteUpdate(LstUpd);
            }
            catch (Exception ex)
            {
                Logger.Task.Write(LOG_TYPE.ERROR, $"CalucateElementList_TS 发生错误[{ex.ToString()}]");
            }            
        }

        /// <summary>
        /// 计算并更新检查样元素列表
        /// </summary>
        /// <param name="ModelElem"></param>
        /// <param name="AvgPotElem"></param>
        public void CalucateElementList_CS(ModelMaterialMain MaterialMain, ModelProbenElem ModelElem, List<string[]> AvgPotElem = null)
        {
            try
            {
                List<ModelMaterialElem> LstUpd = new List<ModelMaterialElem>();
                List<ModelProbenElem> ElemList = Helper.GetProbenElement(ModelElem);
                if (ElemList == null || (MaterialMain?.Token).IsEmpty())
                    return;
                foreach (ModelProbenElem item in ElemList)
                {
                    if (item.ProbenToken.IsEmpty() || item.Element.IsEmpty())
                        continue;
                    //创建修改对象
                    ModelMaterialElem modUpd = new ModelMaterialElem()
                    {
                        MaterialToken = MaterialMain.Token.MarkWhere(),
                        CS_Token = item.ProbenToken.MarkWhere(),
                        Element = item.Element.MarkWhere()
                    };
                    //若有分析点值填充，则先更改值
                    if (AvgPotElem != null)
                    {
                        string[] MatchArray = AvgPotElem?.MySelectFirst(x => x[0] == item.Element);
                        if (item.ProbenToken.IsEmpty() || MatchArray == null)
                            continue;
                        modUpd.CheckValue = MatchArray.MySelectAny(1);
                    }
                    //检查偏差
                    double dTolCheck = Calculate("A-B-C", item.SetPoint.ToMyDouble(), modUpd.CheckValue.ToMyDouble(), 0);
                    if (dTolCheck != SystemDefault.InValidDouble)
                        modUpd.TolCheck = dTolCheck.RoundToDefaultDecimalAndFormat();
                    //数据更新到库
                    if (modUpd.CheckValue.IsNotEmpty())
                    {
                        LstUpd.Add(modUpd);
                    }
                }
                if (LstUpd.MyCount() > 0)
                    DbMyCon?.ExcuteUpdate(LstUpd);
            }
            catch (Exception ex)
            {
                Logger.Task.Write(LOG_TYPE.ERROR, $"CalucateElementList_CS 发生错误[{ex.ToString()}]");
            }
        }

        /// <summary>
        /// 牌号元素修正值
        /// </summary>
        /// <param name="ElemKeyPair">元素名称-值</param>
        /// <param name="Elem"></param>
        /// <returns></returns>
        public double ElementCorrectionValue(string[] ElemKeyPair, ModelMaterialFull MaterialItem, ref JDictionary SourceDic)
        {
            if (ElemKeyPair.MyCount() != 2 || ElemKeyPair.MySelectAny(1).IsEmpty() || MaterialItem == null)
                return SystemDefault.InValidDouble;
            string ElemName = ElemKeyPair[0];
            double dElemValue = ElemKeyPair[1].ToMyDouble();
            double dCorrectionValue = SystemDefault.InValidDouble;
            List<double> LstMultiSource = new List<double> { MaterialItem.T1_SetPoint.ToMyDouble(), MaterialItem.T2_SetPoint.ToMyDouble() };
            SourceDic["SetPoint"].SetValue(MaterialItem.Element,MaterialItem.T1_SetPoint.ToMyDouble());
            if (MaterialItem.TolMethod == "乘法")
            {
                dCorrectionValue = MultiplitiveCorrection(dElemValue, MaterialItem);
            }
            else if (MaterialItem.TolMethod == "加法")
            {
                dCorrectionValue = AdditiveCorrection(dElemValue, MaterialItem);
            }
            else if (MaterialItem.TolMethod == "动态")
            {
                //找到离目标最近的那个修正结果
                double dCorrectionMul = MultiplitiveCorrection(dElemValue, MaterialItem);
                double dCorrectionAdd = AdditiveCorrection(dElemValue, MaterialItem);
                List<double> lstDest = new List<double>() { dCorrectionMul, dCorrectionAdd };
                dCorrectionValue = MaterialItem.T1_SetPoint.ToMyDouble().GetNearestValue(lstDest, out int NearIndex);
            }
            return (dCorrectionValue == SystemDefault.InValidDouble) ? dElemValue : dCorrectionValue;
        }

        /// <summary>
        /// 乘法修正
        /// </summary>
        /// <param name="dElemValue"></param>
        /// <param name="MaterialItem"></param>
        /// <returns></returns>
        private double MultiplitiveCorrection(double dElemValue,ModelMaterialFull MaterialItem)
        {
            bool TS1MultiValid = MaterialItem.TS1_Token.IsNotEmpty() && MaterialItem.T1_MultiplitiveValue.IsNotEmpty();
            bool TS2MultiValid = MaterialItem.TS2_Token.IsNotEmpty() && MaterialItem.T2_MultiplitiveValue.IsNotEmpty();
            double dCorrectionValue = SystemDefault.InValidDouble;
            List<double> LstMultiSource = new List<double> { MaterialItem.T1_SetPoint.ToMyDouble(), MaterialItem.T2_SetPoint.ToMyDouble() };
            //单控样 or 双控样
            if (TS1MultiValid && !TS2MultiValid)
            {
                dCorrectionValue = Calculate("A*B+C", dElemValue, MaterialItem.T1_MultiplitiveValue.ToMyDouble(), 0);
            }
            else if (TS1MultiValid && TS2MultiValid)
            {
                dElemValue.GetNearestValue(LstMultiSource, out int MatchedIndex);
                if (MatchedIndex == 1)
                    dCorrectionValue = Calculate("A*B+C", dElemValue, MaterialItem.T2_MultiplitiveValue.ToMyDouble(), 0);
                else
                    dCorrectionValue = Calculate("A*B+C", dElemValue, MaterialItem.T1_MultiplitiveValue.ToMyDouble(), 0);
            }
            return dCorrectionValue;
        }

        /// <summary>
        /// 加法修正
        /// </summary>
        /// <param name="dElemValue"></param>
        /// <param name="MaterialItem"></param>
        /// <returns></returns>
        private double AdditiveCorrection(double dElemValue, ModelMaterialFull MaterialItem)
        {
            bool TS1AddtiValid = MaterialItem.TS1_Token.IsNotEmpty() && MaterialItem.T1_AdditiveValue.IsNotEmpty();
            bool TS2AddtiValid = MaterialItem.TS2_Token.IsNotEmpty() && MaterialItem.T2_AdditiveValue.IsNotEmpty();
            List<double> LstMultiSource = new List<double> { MaterialItem.T1_SetPoint.ToMyDouble(), MaterialItem.T2_SetPoint.ToMyDouble() };
            double dCorrectionValue = SystemDefault.InValidDouble;
            //单控样 or 双控样
            if (TS1AddtiValid && !TS2AddtiValid)
            {
                dCorrectionValue = Calculate("A+B+C", dElemValue, MaterialItem.T1_AdditiveValue.ToMyDouble(), 0);
            }
            else if (TS1AddtiValid && TS2AddtiValid)
            {
                dElemValue.GetNearestValue(LstMultiSource, out int MatchedIndex);
                if (MatchedIndex == 1)
                    dCorrectionValue = Calculate("A+B+C", dElemValue, MaterialItem.T2_AdditiveValue.ToMyDouble(), 0);
                else
                    dCorrectionValue = Calculate("A+B+C", dElemValue, MaterialItem.T1_AdditiveValue.ToMyDouble(), 0);
            }
            return dCorrectionValue;
        }

        /// <summary>
        /// 计算公式
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        private double Calculate(string expression, double A, double B, double C)
        {
            try
            {
                //ex: ??? =A *  lgm - B
                //创建参数
                ParameterExpression parameterA = Expression.Parameter(typeof(double), "A");
                ParameterExpression parameterB = Expression.Parameter(typeof(double), "B");
                ParameterExpression parameterC = Expression.Parameter(typeof(double), "C");
                //构建表达式树
                Expression body = DynamicExpressionParser.ParseLambda(new[] { parameterA, parameterB, parameterC }, typeof(double), expression).Body;
                //编译表达式树成委托函数
                Func<double, double, double, double> calculator = Expression.Lambda<Func<double, double, double, double>>(body, parameterA, parameterB, parameterC).Compile();
                //调用委托函数计算结果
                double result = calculator(A, B, C);
                return result;
            }
            catch (Exception)
            {
                return SystemDefault.InValidDouble;
            }
        }
    }
}
