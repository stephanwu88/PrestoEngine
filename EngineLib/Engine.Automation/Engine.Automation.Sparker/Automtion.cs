using Engine.Common;
using Engine.Data;
using Engine.Data.DBFAC;
using System.Collections.Generic;
using System.Data;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 光谱自动化类库
    /// </summary>
    public partial class SparkerAutomtion : NotifyObject
    {
        /// <summary>
        /// 本地服务连接
        /// </summary>
        protected static IDBFactory<ServerNode> DbMyCon = DbFactory.Data;

        /// <summary>
        /// 助手
        /// </summary>
        public SparkHelper Helper { get; set; }
        
        /// <summary>
        /// 默认实例
        /// </summary>
        public static SparkerAutomtion Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new SparkerAutomtion();
                    _Instance.LoadHelper();
                }
                return _Instance;
            }
        }
        private static SparkerAutomtion _Instance;

        /// <summary>
        /// 加载助手库
        /// </summary>
        private void LoadHelper()
        {
            ModelBaseIns mod = new ModelBaseIns() { BindAppName = SystemDefault.AppName };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            if (dt.Rows.Count > 0)
            {
                ModelBaseIns baseIns = ColumnDef.ToEntity<ModelBaseIns>(dt.Rows[0]);
                Helper = new SparkHelper() { BaseIns = baseIns };
            }
        }
    }

    /// <summary>
    /// 关键成员
    /// </summary>
    public partial class SparkerAutomtion
    {
        /// <summary>
        /// 自动化分析指定的分析程序
        /// </summary>
        public ModelSpecPgm AnaPgm
        {
            get { return _AnaPgm; }
            set
            {
                if (value == null)
                    return;
                ModelSpecPgm modAnaPgm = value;
                if (value.Token.IsEmpty() && value.Matrix.IsNotEmpty() && value.PgmName.IsNotEmpty())
                    modAnaPgm = Helper.GetSpecificAnaPgm(value);
                bool IsAnaPgmChanged = modAnaPgm?.Token != _AnaPgm?.Token;
                _AnaPgm = modAnaPgm;
                Helper.AnaPgm = _AnaPgm;
                RaisePropertyChanged();
            }
        }
        private ModelSpecPgm _AnaPgm = new ModelSpecPgm();

        /// <summary>
        /// 分析任务类型
        /// </summary>
        public string JobType
        {
            get { return _JobType; }
            set { _JobType = value; RaisePropertyChanged(); }
        }
        private string _JobType;

        /// <summary>
        /// 样品分析元素数据列表
        /// </summary>
        public AnaElemData SampleElemData
        {
            get { return _SampleElemData; }
            set { _SampleElemData = value;RaisePropertyChanged(); }
        }
        private AnaElemData _SampleElemData = new AnaElemData();

        /// <summary>
        /// 已注册
        /// </summary>
        public bool Registered
        {
            get { return _Registered; }
            set { _Registered = value; RaisePropertyChanged(); }
        }
        private bool _Registered;

        /// <summary>
        /// 样品激发点编码
        /// </summary>
        public string SparkPotsCode
        {
            get { return _SparkPotsCode; }
            set { _SparkPotsCode = value;RaisePropertyChanged(); }
        }
        private string _SparkPotsCode;

        /// <summary>
        /// 选定钢种
        /// </summary>
        public ModelRelatedSteelType SteelTypeItem
        {
            get { return _SteelTypeItem; }
            set
            {
                _SteelTypeItem = value;
                RaisePropertyChanged();
                if (SteelTypeItem?.Material.IsNotEmpty() == true)
                {
                    //获取钢种关联牌号的Gama判定设置项
                    CurrentMaterialDeviations = Helper.GetMaterialDeviations(SteelTypeItem?.Material);
                }
            }
        }
        private ModelRelatedSteelType _SteelTypeItem = new ModelRelatedSteelType();

        /// <summary>
        /// 当前分析样品
        /// </summary>
        public ModelProbenDetail CurrentAnaProben
        {
            get { return _CurrentAnaProben; }
            set
            {
                _CurrentAnaProben = value;
                RaisePropertyChanged();
            }
        }
        private ModelProbenDetail _CurrentAnaProben = new ModelProbenDetail();

        /// <summary>
        /// 当前牌号的偏差设置项
        /// </summary>
        public List<ModelMaterialDeviation> CurrentMaterialDeviations
        {
            get { return _CurrentMaterialDeviations; }
            set
            {
                _CurrentMaterialDeviations = value;
                RaisePropertyChanged();
            }
        }
        private List<ModelMaterialDeviation> _CurrentMaterialDeviations;

        /// <summary>
        /// 关联牌号
        /// </summary>
        public ModelMaterialMain MaterialMain
        {
            get { return _MaterialMain; }
            set { _MaterialMain = value;RaisePropertyChanged(); }
        }
        private ModelMaterialMain _MaterialMain  =new ModelMaterialMain();

        /// <summary>
        /// 关联控样列表
        /// </summary>
        public List<ModelProbenDetail> LstRelatedProben
        {
            get { return _LstRelatedProben; }
            set { _LstRelatedProben = value;RaisePropertyChanged(); ConvertToRelatedProbenView(); }
        }
        private List<ModelProbenDetail> _LstRelatedProben = new List<ModelProbenDetail>();

        /// <summary>
        /// 关联控样显示文字
        /// </summary>
        public string RelatedProbenViewText
        {
            get { return _RelatedProbenViewText; }
            set { _RelatedProbenViewText = value; RaisePropertyChanged(); }
        }
        private string _RelatedProbenViewText;

        /// <summary>
        /// 转换关联控样显示文字
        /// </summary>
        private void ConvertToRelatedProbenView()
        {
            if (LstRelatedProben.MyCount() == 0)
            {
                RelatedProbenViewText = string.Empty;
                return;
            }
            string strView = string.Empty;
            foreach (ModelProbenDetail item in LstRelatedProben)
            {
                if (strView.IsNotEmpty())
                    strView += ",";
                string strCurrentState = string.Empty;
                if (item.CurrentState.IsNotEmpty() && item.CurrentState!="有效")
                    strCurrentState = $"【{item.CurrentState}】";
                strView += $"{item.Name} {strCurrentState}";
            }
            RelatedProbenViewText = strView;
        }

        /// <summary>
        /// 复位所有
        /// </summary>
        public void ResetAll()
        {
            CurrentAnaProben = new ModelProbenDetail();
            //AnaPgm = new ModelSpecPgm();
            //AnaJobType = string.Empty;
            SteelTypeItem = new ModelRelatedSteelType();
            MaterialMain = new ModelMaterialMain();
            LstRelatedProben = new List<ModelProbenDetail>();
        }
    }
}
