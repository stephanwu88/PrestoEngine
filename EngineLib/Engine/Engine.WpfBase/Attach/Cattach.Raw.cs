using System.Windows;

namespace Engine.WpfBase
{
    /// <summary>
    /// 钢铁行业样品信息
    /// </summary>
    public static partial class Cattach
    {
        #region SampleID 试样编码
        /// <summary>
        /// SampleID 试样编码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetSampleID(this DependencyObject obj)
        {
            return (string)obj.GetValue(SampleIDProperty);
        }

        public static void SetSampleID(this DependencyObject obj, string value)
        {
            obj.SetValue(SampleIDProperty, value);
        }

        // Using a DependencyProperty as the backing store for SampleID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SampleIDProperty =
            DependencyProperty.RegisterAttached("SampleID", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region SampleName 物料名称
        /// <summary>
        /// SampleName 物料名称
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetSampleName(this DependencyObject obj)
        {
            return (string)obj.GetValue(SampleNameProperty);
        }

        public static void SetSampleName(this DependencyObject obj, string value)
        {
            obj.SetValue(SampleNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for SampleName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SampleNameProperty =
            DependencyProperty.RegisterAttached("SampleName", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region SampleCode 物料代码
        /// <summary>
        /// SampleCode 物料代码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetSampleCode(this DependencyObject obj)
        {
            return (string)obj.GetValue(SampleCodeProperty);
        }

        public static void SetSampleCode(this DependencyObject obj, string value)
        {
            obj.SetValue(SampleCodeProperty, value);
        }

        // Using a DependencyProperty as the backing store for SampleCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SampleCodeProperty =
            DependencyProperty.RegisterAttached("SampleCode", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region Supplier 供货方
        /// <summary>
        /// Supplier 供货方
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetSupplier(this DependencyObject obj)
        {
            return (string)obj.GetValue(SupplierProperty);
        }

        public static void SetSupplier(this DependencyObject obj, string value)
        {
            obj.SetValue(SupplierProperty, value);
        }

        // Using a DependencyProperty as the backing store for Supplier.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SupplierProperty =
            DependencyProperty.RegisterAttached("Supplier", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region DelegateOdd 委托单号
        /// <summary>
        /// DelegateOdd 委托单号
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public static string GetDelegateOdd(this DependencyObject obj)
        {
            return (string)obj.GetValue(DelegateOddProperty);
        }

        public static void SetDelegateOdd(this DependencyObject obj, string value)
        {
            obj.SetValue(DelegateOddProperty, value);
        }

        // Using a DependencyProperty as the backing store for DelegateOdd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelegateOddProperty =
            DependencyProperty.RegisterAttached("DelegateOdd", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region DelegateTime 委托时间
        /// <summary>
        /// DelegateTime 委托时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetDelegateTime(this DependencyObject obj)
        {
            return (string)obj.GetValue(DelegateTimeProperty);
        }

        public static void SetDelegateTime(this DependencyObject obj, string value)
        {
            obj.SetValue(DelegateTimeProperty, value);
        }

        // Using a DependencyProperty as the backing store for DelegateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelegateTimeProperty =
            DependencyProperty.RegisterAttached("DelegateTime", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region TaskGenerateTime 任务生成时间
        /// <summary>
        /// TaskGenerateTime 任务生成时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTaskGenerateTime(this DependencyObject obj)
        {
            return (string)obj.GetValue(TaskGenerateTimeProperty);
        }

        public static void SetTaskGenerateTime(this DependencyObject obj, string value)
        {
            obj.SetValue(TaskGenerateTimeProperty, value);
        }

        // Using a DependencyProperty as the backing store for TaskGenerateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskGenerateTimeProperty =
            DependencyProperty.RegisterAttached("TaskGenerateTime", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region PrepTime 制样开始时间
        /// <summary>
        /// PrepTime 制样开始时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetPrepTime(this DependencyObject obj)
        {
            return (string)obj.GetValue(PrepTimeProperty);
        }

        public static void SetPrepTime(this DependencyObject obj, string value)
        {
            obj.SetValue(PrepTimeProperty, value);
        }

        // Using a DependencyProperty as the backing store for PrepTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrepTimeProperty =
            DependencyProperty.RegisterAttached("PrepTime", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region GradeSampleCopies 粒度样份样数
        /// <summary>
        /// GradeSampleCopies 粒度样份样数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetGradeSampleCopies(this DependencyObject obj)
        {
            return (int)obj.GetValue(GradeSampleCopiesProperty);
        }

        public static void SetGradeSampleCopies(this DependencyObject obj, int value)
        {
            obj.SetValue(GradeSampleCopiesProperty, value);
        }

        // Using a DependencyProperty as the backing store for GradeSampleCopies.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GradeSampleCopiesProperty =
            DependencyProperty.RegisterAttached("GradeSampleCopies", typeof(int), typeof(Cattach), new PropertyMetadata(0));
        #endregion

        #region CompositionSampleCopies 成分样份样数
        /// <summary>
        /// CompositionSampleCopies 成分样份样数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetCompositionSampleCopies(this DependencyObject obj)
        {
            return (int)obj.GetValue(CompositionSampleCopiesProperty);
        }

        public static void SetCompositionSampleCopies(this DependencyObject obj, int value)
        {
            obj.SetValue(CompositionSampleCopiesProperty, value);
        }

        // Using a DependencyProperty as the backing store for CompositionSampleCopies.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompositionSampleCopiesProperty =
            DependencyProperty.RegisterAttached("CompositionSampleCopies", typeof(int), typeof(Cattach), new PropertyMetadata(0));
        #endregion

        #region BeltCode 皮带代码 ex: XT106
        /// <summary>
        /// BeltCode 皮带代码 ex: XT106
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetBeltCode(this DependencyObject obj)
        {
            return (string)obj.GetValue(BeltCodeProperty);
        }

        public static void SetBeltCode(this DependencyObject obj, string value)
        {
            obj.SetValue(BeltCodeProperty, value);
        }

        // Using a DependencyProperty as the backing store for BeltCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeltCodeProperty =
            DependencyProperty.RegisterAttached("BeltCode", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region BeltComment 皮带描述 ex:焦化翻车机-西排筒仓
        /// <summary>
        /// BeltComment 皮带描述 ex:焦化翻车机-西排筒仓
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetBeltComment(this DependencyObject obj)
        {
            return (string)obj.GetValue(BeltCommentProperty);
        }

        public static void SetBeltComment(this DependencyObject obj, string value)
        {
            obj.SetValue(BeltCommentProperty, value);
        }

        // Using a DependencyProperty as the backing store for BeltComment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeltCommentProperty =
            DependencyProperty.RegisterAttached("BeltComment", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region BeltState 皮带状态 
        /// <summary>
        /// BeltState 皮带状态 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetBeltState(this DependencyObject obj)
        {
            return (string)obj.GetValue(BeltStateProperty);
        }

        public static void SetBeltState(this DependencyObject obj, string value)
        {
            obj.SetValue(BeltStateProperty, value);
        }

        // Using a DependencyProperty as the backing store for BeltState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeltStateProperty =
            DependencyProperty.RegisterAttached("BeltState", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion


        #region SamplingState 取样状态
        /// <summary>
        /// SamplingState 取样状态 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetSamplingState(this DependencyObject obj)
        {
            return (string)obj.GetValue(SamplingStateProperty);
        }

        public static void SetSamplingState(this DependencyObject obj, string value)
        {
            obj.SetValue(SamplingStateProperty, value);
        }

        // Using a DependencyProperty as the backing store for SamplingState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SamplingStateProperty =
            DependencyProperty.RegisterAttached("SamplingState", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region SamplingCode 取样码 ex:N9BC2305130268
        /// <summary>
        /// SamplingCode 取样码 ex:N9BC2305130268
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetSamplingCode(this DependencyObject obj)
        {
            return (string)obj.GetValue(SamplingCodeProperty);
        }

        public static void SetSamplingCode(this DependencyObject obj, string value)
        {
            obj.SetValue(SamplingCodeProperty, value);
        }

        // Using a DependencyProperty as the backing store for SamplingCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SamplingCodeProperty =
            DependencyProperty.RegisterAttached("SamplingCode", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region PrepCode 制样码 ex:230511M153472
        /// <summary>
        /// PrepCode 制样码 ex:230511M153472
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetPrepCode(this DependencyObject obj)
        {
            return (string)obj.GetValue(PrepCodeProperty);
        }

        public static void SetPrepCode(this DependencyObject obj, string value)
        {
            obj.SetValue(PrepCodeProperty, value);
        }

        // Using a DependencyProperty as the backing store for PrepCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrepCodeProperty =
            DependencyProperty.RegisterAttached("PrepCode", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region CreateTime 创建时间
        /// <summary>
        /// CreateTime 创建时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetCreateTime(this DependencyObject obj)
        {
            return (string)obj.GetValue(CreateTimeProperty);
        }

        public static void SetCreateTime(this DependencyObject obj, string value)
        {
            obj.SetValue(CreateTimeProperty, value);
        }

        // Using a DependencyProperty as the backing store for CreateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CreateTimeProperty =
            DependencyProperty.RegisterAttached("CreateTime", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region ShipSeq 航次号  ex:4C2305003
        /// <summary>
        /// ShipSeq 航次号  ex:4C2305003
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetShipSeq(this DependencyObject obj)
        {
            return (string)obj.GetValue(ShipSeqProperty);
        }

        public static void SetShipSeq(this DependencyObject obj, string value)
        {
            obj.SetValue(ShipSeqProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShipSeq.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShipSeqProperty =
            DependencyProperty.RegisterAttached("ShipSeq", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region ShipName 船名 ex:国裕15
        /// <summary>
        /// ShipName 船名 ex:国裕15
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetShipName(this DependencyObject obj)
        {
            return (string)obj.GetValue(ShipNameProperty);
        }

        public static void SetShipName(this DependencyObject obj, string value)
        {
            obj.SetValue(ShipNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShipName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShipNameProperty =
            DependencyProperty.RegisterAttached("ShipName", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region TrainSeq 车次号 ex:49102
        /// <summary>
        /// TrainSeq 车次号 ex:49102
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetTrainSeq(this DependencyObject obj)
        {
            return (string)obj.GetValue(TrainSeqProperty);
        }

        public static void SetTrainSeq(this DependencyObject obj, string value)
        {
            obj.SetValue(TrainSeqProperty, value);
        }

        // Using a DependencyProperty as the backing store for TrainSeq.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrainSeqProperty =
            DependencyProperty.RegisterAttached("TrainSeq", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region CurrentFlux 瞬时流量 ex:14.62
        /// <summary>
        /// CurrentFlux 瞬时流量 ex:14.62
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetCurrentFlux(this DependencyObject obj)
        {
            return (string)obj.GetValue(CurrentFluxProperty);
        }

        public static void SetCurrentFlux(this DependencyObject obj, string value)
        {
            obj.SetValue(CurrentFluxProperty, value);
        }

        // Using a DependencyProperty as the backing store for CurrentFlux.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentFluxProperty =
            DependencyProperty.RegisterAttached("CurrentFlux", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region CurrentWeight 累计重量 ex:896104 
        /// <summary>
        /// CurrentWeight 累计重量 ex: 896104
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetCurrentWeight(this DependencyObject obj)
        {
            return (string)obj.GetValue(CurrentWeightProperty);
        }

        public static void SetCurrentWeight(this DependencyObject obj, string value)
        {
            obj.SetValue(CurrentWeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for CurrentWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentWeightProperty =
            DependencyProperty.RegisterAttached("CurrentWeight", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region UnloadWeight 卸货吨位   
        /// <summary>
        /// UnloadWeight 卸货吨位 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetUnloadWeight(this DependencyObject obj)
        {
            return (string)obj.GetValue(UnloadWeightProperty);
        }

        public static void SetUnloadWeight(this DependencyObject obj, string value)
        {
            obj.SetValue(UnloadWeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for UnloadWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnloadWeightProperty =
            DependencyProperty.RegisterAttached("UnloadWeight", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region SendWeight  发货吨位
        /// <summary>
        /// SendWeight  发货吨位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetSendWeight(this DependencyObject obj)
        {
            return (string)obj.GetValue(SendWeightProperty);
        }

        public static void SetSendWeight(this DependencyObject obj, string value)
        {
            obj.SetValue(SendWeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for SendWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SendWeightProperty =
            DependencyProperty.RegisterAttached("SendWeight", typeof(string), typeof(Cattach), new PropertyMetadata(""));
        #endregion

        #region UnloadPerent 卸货进度
        /// <summary>
        /// UnloadPerent 卸货进度
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetUnloadPerent(this DependencyObject obj)
        {
            return (string)obj.GetValue(UnloadPerentProperty);
        }

        public static void SetUnloadPerent(this DependencyObject obj, string value)
        {
            obj.SetValue(UnloadPerentProperty, value);
        }

        // Using a DependencyProperty as the backing store for UnloadPerent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnloadPerentProperty =
            DependencyProperty.RegisterAttached("UnloadPerent", typeof(string), typeof(Cattach), new PropertyMetadata("0"));
        #endregion
    }
}
