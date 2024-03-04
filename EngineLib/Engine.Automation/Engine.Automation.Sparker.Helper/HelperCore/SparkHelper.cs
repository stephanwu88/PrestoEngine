using Engine.Common;
using Engine.Data.DBFAC;
using System;
using System.Collections.Generic;
using System.Data;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 实例化 资源创建
    /// </summary>
    public partial class SparkHelper
    {
        /// <summary>
        /// 本地服务连接
        /// </summary>
        protected static IDBFactory<ServerNode> DbMyCon = DbFactory.Data;
        //分析元素排序
        private string ElementOrderBy = string.Empty;
        public IComparer<string> ElementOrderComparer { get; private set; } = null;

        /// <summary>
        /// 将仪器对象存储为字典
        /// </summary>
        public static Dictionary<string, SparkHelper> Factory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new Dictionary<string, SparkHelper>();
                    ModelBaseIns mod = new ModelBaseIns();
                    DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
                    List<ModelBaseIns> LstIns = ColumnDef.ToEntityList<ModelBaseIns>(dt);
                    foreach (ModelBaseIns item in LstIns)
                    {
                        SparkHelper _sparker = new SparkHelper() { BaseIns = item };
                        if (_sparker != null)
                            Factory.AppandDict(item.InsName, _sparker);
                    }
                }
                return _Factory;
            }
        }
        private static Dictionary<string, SparkHelper> _Factory;

        /// <summary>
        /// 当前实例
        /// 根据当前应用程序自动获取该仪器助手软件相关的样品信息
        /// </summary>
        public static SparkHelper Current
        {
            get
            {
                if (_Current == null)
                {
                    try
                    {
                        ModelBaseIns mod = new ModelBaseIns() { BindAppName = SystemDefault.AppName };
                        DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
                        if (dt.Rows.Count > 0)
                        {
                            ModelBaseIns baseIns = ColumnDef.ToEntity<ModelBaseIns>(dt.Rows[0]);
                            _Current = new SparkHelper() { BaseIns = baseIns };
                            _Current.AnaPgm = _Current.GetAnaPgmList().MySelectAny(0);
                            Factory.AppandDict(baseIns.InsName, _Current);
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                return _Current;
            }
        }
        private static SparkHelper _Current;

        /// <summary>
        /// 仪器基础信息
        /// </summary>
        public ModelBaseIns BaseIns { get; set; } = new ModelBaseIns();

        /// <summary>
        /// 仪器分析曲线
        /// </summary>
        public ModelSpecPgm AnaPgm
        {
            get => _AnaPgm;
            set
            {
                if (value == null)
                    return;
                ModelSpecPgm modAnaPgm = value;
                if (value.Token.IsEmpty() && value.Matrix.IsNotEmpty() && value.PgmName.IsNotEmpty())
                    modAnaPgm = GetSpecificAnaPgm(value);
                bool IsAnaPgmChanged = modAnaPgm?.Token != _AnaPgm?.Token;
                _AnaPgm = modAnaPgm;
                //if (IsAnaPgmChanged)
                {
                    //获取该分析方法的元素排序
                    GetElemenOrder();
                }
            }
        }
        private ModelSpecPgm _AnaPgm = new ModelSpecPgm();

        /// <summary>
        /// 配置仪器程序
        /// </summary>
        /// <param name="InsName"></param>
        /// <param name="Matrix"></param>
        /// <param name="PgmName"></param>
        public void Config(string InsName, string Matrix, string PgmName)
        {
            BaseIns.InsName = InsName;
            AnaPgm.Matrix = Matrix;
            AnaPgm.PgmName = PgmName;
        }
 
        /// <summary>
        /// 获取元素排序
        /// </summary>
        private void GetElemenOrder()
        {
            string strOrderBy = string.Empty;
            List<ModelElemBase> lstExtendElem = GetAnaPgmExtendElemList(AnaPgm);
            if (lstExtendElem.MyCount() == 0)
                ElementOrderBy = string.Empty;
            List<string> LstElem = new List<string>();
            foreach (ModelElemBase model in lstExtendElem)
            {
                if (model.Element.IsEmpty())
                    continue;
                if (strOrderBy.IsNotEmpty())
                    strOrderBy += ",";
                strOrderBy += string.Format("'{0}'", model.Element);
                LstElem.AppandList(model.Element);
            }
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                //strOrderBy = string.Format(" order by field(Element,{0})", strOrderBy);  //系统默认未指定到的排至头部，这不是预期的
                //按照指定顺序返回，未指定到的排到尾部
                strOrderBy = string.Format(" case when Element in({0}) then 0 else 1 end,field(Element,{0})", strOrderBy);
            }
            ElementOrderBy = strOrderBy;
            //更新元素排序比较器
            if (LstElem.MyCount() > 0)
                ElementOrderComparer = new CustomComparer(LstElem);
        }
    }

    /// <summary>
    /// 标准化 - 仪器分析程序管理
    /// </summary>
    public partial class SparkHelper
    {
        /// <summary>
        /// 获取分析曲线列表
        /// </summary>
        /// <returns></returns>
        public List<ModelSpecPgm> GetAnaPgmList()
        {
            ModelSpecPgm mod = new ModelSpecPgm() { InsName = BaseIns?.InsName };
            DataTable dt = DbMyCon.ExcuteQuery(mod, "OrderID asc").Result.ToMyDataTable();
            List<ModelSpecPgm> Lst = ColumnDef.ToEntityList<ModelSpecPgm>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取明确的分析程序对象，结果不明确时抛出异常
        /// </summary>
        /// <param name="InsName"></param>
        /// <param name="Matrix"></param>
        /// <param name="PgmName"></param>
        /// <returns></returns>
        public ModelSpecPgm GetSpecificAnaPgm(string InsName, string Matrix, string PgmName)
        {
            ModelSpecPgm mod = new ModelSpecPgm() { InsName = InsName, Matrix = Matrix, PgmName = PgmName };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            ModelSpecPgm result = null;
            if (dt.Rows.Count != 1)
                throw new Exception("获取仪器分析曲线时得到不明确的结果");
            result = ColumnDef.ToEntity<ModelSpecPgm>(dt.Rows[0]);
            return result;
        }

        /// <summary>
        /// 获取完整分析程序对象
        /// </summary>
        /// <param name="PgmToken"></param>
        /// <returns></returns>
        public ModelSpecPgm GetSpecificAnaPgm(string PgmToken)
        {
            ModelSpecPgm mod = new ModelSpecPgm() { Token = PgmToken };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            ModelSpecPgm result = null;
            if (dt.Rows.Count != 1)
                throw new Exception("获取仪器分析曲线时得到不明确的结果");
            result = ColumnDef.ToEntity<ModelSpecPgm>(dt.Rows[0]);
            return result;
        }

        /// <summary>
        /// 获取完整分析程序对象
        /// </summary>
        /// <param name="modlePgm"></param>
        /// <returns></returns>
        public ModelSpecPgm GetSpecificAnaPgm(ModelSpecPgm modelPgm)
        {
            if (modelPgm.Token.IsNotEmpty() && modelPgm.Matrix.IsNotEmpty() && modelPgm.PgmName.IsNotEmpty())
            {
                return modelPgm;
            }
            if (modelPgm.Token.IsEmpty() && modelPgm.Matrix.IsNotEmpty() && modelPgm.PgmName.IsNotEmpty())
            {
                modelPgm.InsName = BaseIns.InsName;
                return GetSpecificAnaPgm(modelPgm.InsName, modelPgm.Matrix, modelPgm.PgmName);
            }
            return null;
        }

        /// <summary>
        /// 添加分析程序
        /// </summary>
        /// <param name="ModelPgm"></param>
        /// <returns></returns>
        public CallResult AddAnaPgm(ModelSpecPgm ModelPgm)
        {
            CallResult res = new CallResult();
            if (ModelPgm == null)
            {
                res.Result = "仪器分析程序添加对象为空";
                return res;
            }
            return DbMyCon.ExcuteInsert(ModelPgm, InsertMode.IngoreInsert);            
        }

        /// <summary>
        /// 添加分析程序集合
        /// </summary>
        /// <param name="LstModelPgm"></param>
        /// <returns></returns>
        public CallResult AddAnaPgm(List<ModelSpecPgm> LstModelPgm)
        {
            CallResult res = new CallResult();
            if (LstModelPgm == null)
            {
                res.Result = "仪器分析程序添加对象为空";
                return res;
            }
            return DbMyCon.ExcuteInsert(LstModelPgm);
        }

        /// <summary>
        /// 更新分析程序
        /// </summary>
        /// <param name="ModelPgm"></param>
        /// <returns></returns>
        public CallResult UpdateAnaPgm(ModelSpecPgm ModelPgm)
        {
            ModelPgm.ValueAttachMark();
            ModelPgm.ID = ModelPgm.ID.MarkWhere();
            return DbMyCon.ExcuteUpdate(ModelPgm);
        }

        /// <summary>
        /// 更新分析程序列表
        /// </summary>
        /// <param name="LstModelPgm"></param>
        /// <returns></returns>
        public CallResult UpdateAnaPgm(List<ModelSpecPgm> LstModelPgm)
        {
            foreach (ModelSpecPgm item in LstModelPgm)
                item.ID = item.ID.MarkWhere();
            return DbMyCon.ExcuteUpdate(LstModelPgm);
        }

        /// <summary>
        /// 更新分析程序列表变化项
        /// </summary>
        /// <param name="LstModelPgm"></param>
        /// <returns></returns>
        public CallResult AnaPgmUpdateChanges(List<ModelSpecPgm> LstModelPgm)
        {
            string strSql = string.Empty;
            foreach (ModelSpecPgm item in LstModelPgm)
            {
                if (string.IsNullOrEmpty(item.ID))
                {
                    strSql += DbMyCon.SqlInsert(item).Result.ToMyString();
                }
                else
                {
                    item.ID = item.ID.MarkWhere();
                    strSql += DbMyCon.SqlUpdate(item).Result.ToMyString();
                }
            }
            return DbMyCon.ExcuteSQL(strSql);
        }

        /// <summary>
        /// 删除分析程序
        /// </summary>
        /// <param name="ModelPgm"></param>
        /// <returns></returns>
        public CallResult DeleteAnaPgm(ModelSpecPgm ModelPgm)
        {
            ModelPgm.Token = ModelPgm.Token.MarkWhere();
            return DbMyCon.ExcuteDelete(ModelPgm);
        }

        /// <summary>
        /// 删除分析程序集合
        /// </summary>
        /// <param name="LstModelPgm"></param>
        /// <returns></returns>
        public CallResult DeleteAnaPgm(List<ModelSpecPgm> LstModelPgm)
        {
            foreach (ModelSpecPgm item in LstModelPgm)
                item.Token = item.Token.MarkWhere();
            return DbMyCon.ExcuteDelete(LstModelPgm);
        }

        /// <summary>
        /// 获取分析方法的拓展元素
        /// </summary>
        /// <param name="AnaPgm"></param>
        /// <returns></returns>
        public List<ModelElemBase> GetAnaPgmExtendElemList(ModelSpecPgm _AnaPgm = null)
        {
            try
            {
                string PgmToken = string.Empty;
                if (_AnaPgm == null)
                    PgmToken = AnaPgm.Token;
                else
                {
                    if (!string.IsNullOrEmpty(_AnaPgm.Token))
                        PgmToken = _AnaPgm.Token;
                    else
                        PgmToken = GetSpecificAnaPgm(_AnaPgm?.InsName, _AnaPgm?.Matrix, _AnaPgm?.PgmName)?.Token;
                }
                if (PgmToken.IsEmpty())
                    return null;
                ModelElemBase mod = new ModelElemBase() { PgmToken = PgmToken };
                DataTable dt = DbMyCon.ExcuteQuery(mod, "OrderID asc").Result.ToMyDataTable();
                List<ModelElemBase> lst = ColumnDef.ToEntityList<ModelElemBase>(dt);
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 保存拓展元素列表
        /// </summary>
        /// <returns></returns>
        public CallResult SaveOrUpdateElemBaseItems(IEnumerable<ModelElemBase> LstElemBase)
        {
            CallResult res = new CallResult();
            List<ModelElemBase> LstAdd = new List<ModelElemBase>();
            List<ModelElemBase> LstUpd = new List<ModelElemBase>();
            foreach (ModelElemBase item in LstElemBase)
            {
                if (item.ID.IsEmpty() && item.Element.IsNotEmpty() && item.PgmToken.IsNotEmpty())
                {
                    LstAdd.Add(item.ModelClone());
                }
                else if (item.ID.IsNotEmpty() && item.IsModified)
                {
                    ModelElemBase itm = item.ModelClone();
                    itm.ValueAttachMark();
                    itm.ID = itm.ID.MarkWhere();
                    LstUpd.Add(item.ModelClone());
                }
            }
            string strSql = string.Empty;
            if (LstAdd.MyCount() > 0)
            {
                res = DbMyCon?.SqlInsert(LstAdd, InsertMode.IngoreInsert);
                if (res.Fail) return res;
                strSql += $"{res.Result.ToMyString()}";
            }
            if (LstUpd.MyCount() > 0)
            {
                 res = DbMyCon?.SqlUpdate(LstUpd);
                if (res.Fail) return res;
                strSql += $"{res.Result.ToMyString()}";
            }
            if (strSql.IsEmpty())
            {
                res.Result = "没有可更新的拓展元素信息";
                return res;
            }
            res = DbMyCon?.ExcuteSQL(strSql);
            return res;
        }

        /// <summary>
        /// 删除拓展元素列表
        /// </summary>
        /// <param name="SelectedList"></param>
        /// <returns></returns>
        public CallResult DeleteElemBaseItems(IEnumerable<ModelElemBase> SelectedList)
        {
            List<ModelElemBase> LstDel = new List<ModelElemBase>();
            foreach (ModelElemBase item in SelectedList)
            {
                if (item.PgmToken.IsEmpty())
                    continue;
                LstDel.Add(new ModelElemBase() { PgmToken = item.PgmToken, Element = item.Element });
            }
            CallResult res = DbMyCon?.ExcuteDelete(LstDel);
            return res;
        }
    }

    /// <summary>
    /// 标准化 - 控样、标样管理
    /// </summary>
    public partial class SparkHelper
    {
        /// <summary>
        /// 获取全局控样、标样列表
        /// </summary>
        /// <param name="ProbenType"></param>
        /// <returns></returns>
        public List<ModelProbenMain> GetGloableProbenMain(string ProbenType = "控样")
        {
            ModelProbenMain mod = new ModelProbenMain() { Type = ProbenType };
            DataTable dt = DbMyCon.ExcuteQuery(mod, " Name asc").Result.ToMyDataTable();
            List<ModelProbenMain> Lst = ColumnDef.ToEntityList<ModelProbenMain>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取控样、标样列表
        /// </summary>
        /// <param name="ProbenType"></param>
        /// <returns></returns>
        public List<ModelProbenMain> GetProbenMain(string ProbenType = "控样")
        {
            ModelProbenDetail mod = new ModelProbenDetail()
            {
                PgmToken = AnaPgm.Token,
                Type = ProbenType
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod,"Name asc").Result.ToMyDataTable();
            List<ModelProbenMain> Lst = ColumnDef.ToEntityList<ModelProbenMain>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取控样、标样列表
        /// </summary>
        /// <returns></returns>
        public List<ModelProbenDetail> GetProbenDetail(string ProbenType = "控样")
        {
            ModelProbenDetail mod = new ModelProbenDetail() { PgmToken = AnaPgm.Token };
            if (ProbenType.MyContains("|"))
            {
                List<string> LstTypeIn = ProbenType.MySplit("|", null, null, false);
                string strTypeIn = LstTypeIn.ToMyString(",", false, "'", "'");
                mod.Type = $"Type in({strTypeIn})".MarkExpress();
            }
            else
            {
                mod.Type = ProbenType;
            }
            DataTable dt = DbMyCon.ExcuteQuery(mod, "Name asc").Result.ToMyDataTable();
            List<ModelProbenDetail> Lst = ColumnDef.ToEntityList<ModelProbenDetail>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取某控样信息
        /// </summary>
        /// <param name="ProbenToken"></param>
        /// <returns></returns>
        public ModelProbenDetail GetProbenDetailItem(string ProbenToken)
        {
            if (ProbenToken.IsEmpty())
                return null;
            ModelProbenDetail mod = new ModelProbenDetail()
            {
                ProbenToken = ProbenToken
            };
            List<ModelProbenDetail> Lst = DbMyCon.ExcuteQuery(mod).Result.ToMyList<ModelProbenDetail>();
            if (Lst.MyCount() == 0)
                return null;
            return Lst[0];
        }

        /// <summary>
        /// 获取某控样信息
        /// </summary>
        /// <param name="ProbenToken"></param>
        /// <returns></returns>
        public ModelProbenDetail GetProbenDetailItem(ModelProbenDetail ModelProben)
        {
            if (ModelProben == null)
                return null;
            ModelProbenDetail mod = new ModelProbenDetail();
            if (ModelProben.ProbenToken.IsNotEmpty())
                mod.ProbenToken = ModelProben.ProbenToken;
            else if (ModelProben.Name.IsNotEmpty())
            {
                mod.Name = ModelProben.Name;
                mod.PgmToken = AnaPgm.Token;
            }
            List<ModelProbenDetail> Lst = DbMyCon.ExcuteQuery(mod).Result.ToMyList<ModelProbenDetail>();
            if (Lst.MyCount() == 0)
                return null;
            return Lst[0];
        }

        /// <summary>
        /// 获取样品标准库
        /// </summary>
        /// <returns></returns>
        public List<ModelProbenStd> GetProbenStd()
        {
            ModelProbenStd mod = new ModelProbenStd();
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelProbenStd> Lst = ColumnDef.ToEntityList<ModelProbenStd>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取控样对应的元素标准值列表
        /// </summary>
        /// <param name="ProbenName"></param>
        /// <param name="Matrix"></param>
        /// <param name="PgmName"></param>
        /// <returns></returns>
        public List<ModelProbenElem> GetProbenElementStd(string ProbenName)
        {
            ModelProbenStd mod = new ModelProbenStd() { Name = ProbenName };
            DataTable dt = DbMyCon.ExcuteQuery(mod, ElementOrderBy).Result.ToMyDataTable();
            List<ModelProbenElem> Lst = ColumnDef.ToEntityList<ModelProbenElem>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取控样在指定分析程序下的元素分析列表
        /// </summary>
        /// <param name="ProbenName"></param>
        /// <param name="InsName"></param>
        /// <param name="Matrix"></param>
        /// <param name="AnaPgm"></param>
        /// <returns></returns>
        public List<ModelProbenElem> GetProbenElement(string ProbenToken)
        {
            ModelProbenElem mod = new ModelProbenElem()
            {
                ProbenToken = ProbenToken
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod, ElementOrderBy).Result.ToMyDataTable();
            List<ModelProbenElem> Lst = ColumnDef.ToEntityList<ModelProbenElem>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取样品元素列表
        /// </summary>
        /// <param name="ModelProben"></param>
        /// <returns></returns>
        public List<ModelProbenElem> GetProbenElement(ModelProbenElem ModelProben)
        {
            if (ModelProben == null)
                return null;
            ModelProbenElem mod = null;
            if (ModelProben.ProbenToken.IsNotEmpty())
            {
                mod = new ModelProbenElem();
                mod.ProbenToken = ModelProben.ProbenToken;
            }
            else if (AnaPgm.Token.IsNotEmpty() && ModelProben.Type.IsNotEmpty() && ModelProben.Name.IsNotEmpty())
            {
                mod = new ModelProbenElem();
                mod.PgmToken = AnaPgm.Token;
                mod.Type = ModelProben.Type;
                mod.Name = ModelProben.Name;
            }
            if (mod == null) return null;
            List<ModelProbenElem> Lst = DbMyCon.ExcuteQuery(mod).Result.ToMyList<ModelProbenElem>();
            return Lst;
        }

        /// <summary>
        /// 获取样品关联牌号
        /// </summary>
        /// <param name="ProbenName"></param>
        /// <returns></returns>
        public List<ModelMaterialMain> GetRelatedMaterial(string ProbenName)
        {
            string strSql = string.Format("select distinct Material,Comment  from {0} where InsName='{1}' " +
                "and Matrix='{2}' and PgmName='{3}' and (T1_Name='{4}' or T2_Name='{4}')",
               TableAttribute.Table<ModelMaterialFull>().Name,
                BaseIns.InsName, AnaPgm.Matrix, AnaPgm.PgmName, ProbenName);
            DataTable dt = DbMyCon.ExcuteQuery(strSql).Result.ToMyDataTable();
            List<ModelMaterialMain> lst = ColumnDef.ToEntityList<ModelMaterialMain>(dt);
            return lst;
        }

        /// <summary>
        /// 添加样品
        /// </summary>
        /// <param name="main"></param>
        /// <param name="RepeatCheck">重复性检查</param>
        /// <returns></returns>
        public CallResult AddProbenMain(ModelProbenMain main,bool RepeatCheck = true)
        {
            CallResult result = new CallResult();
            if (RepeatCheck)
            {
                ModelProbenMain mod = new ModelProbenMain() { Name = main.Name };
                DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
                if (dt.Rows.Count > 0)
                {
                    result.Success = false;
                    result.Result = string.Format("添加样品【{0}】重复", main.Name);
                    return result;
                }
            }
            if (string.IsNullOrEmpty(main.MainToken))
                main.MainToken = SystemDefault.UUID;
            return DbMyCon.ExcuteInsert(main);
        }

        /// <summary>
        /// 添加样品配置信息
        /// </summary>
        /// <param name="full"></param>
        /// <param name="RepeatCheck">重复性检查</param>
        /// <returns></returns>
        public CallResult AddProbenMainFull(ModelProbenDetail full,bool RepeatCheck = true)
        {
            CallResult result = new CallResult();
            string strSql = string.Empty;
            //重复性检查
            if (RepeatCheck)
            {
                ModelProbenDetail mod = new ModelProbenDetail() { Name = full.Name, PgmToken = AnaPgm.Token };
                DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
                if (dt.Rows.Count > 0)
                {
                    result.Success = false;
                    result.Result = string.Format("添加样品【{0}】重复", full.Name);
                    return result;
                }
            }
            //主信息没有，则附加
            if (string.IsNullOrEmpty(full.MainToken))
            {
                full.MainToken = SystemDefault.UUID; 
                ModelProbenMain main = new ModelProbenMain()
                {
                    MainToken = full.MainToken,
                    Name = full.Name,
                    Type = full.Type,
                    Comment = full.Comment
                };
                result = DbMyCon?.SqlInsert(main);
                if(result.Success)
                    strSql += result.Result.ToMyString();
            }
            if(string.IsNullOrEmpty(full.ProbenToken))
                full.ProbenToken = SystemDefault.UUID;
            full.PgmToken = AnaPgm.Token;
            result = DbMyCon?.SqlInsert(full);
            if(result.Success)
                strSql += result.Result.ToMyString();
            if (string.IsNullOrEmpty(strSql))
            {
                result.Success = false;
                result.Result = "新增样品信息为空";
                return result;
            }
            return DbMyCon?.ExcuteSQL(strSql);
        }

        /// <summary>
        /// 修改样品主信息
        /// </summary>
        /// <param name="full"></param>
        /// <returns></returns>
        public CallResult UpdateProbenMain(ModelProbenMain main)
        {
            main.MainToken = main.MainToken.MarkWhere();
            return DbMyCon.ExcuteUpdate(main);
        }

        /// <summary>
        /// 修改样品信息
        /// </summary>
        /// <param name="full"></param>
        /// <returns></returns>
        public CallResult UpdateProbenFullMain(ModelProbenDetail full)
        {
            CallResult result = new CallResult();
            string strSql = string.Empty;
            //更新主信息
            if (!string.IsNullOrEmpty(full.MainToken))
            {
                ModelProbenMain main = new ModelProbenMain()
                {
                    MainToken = full.MainToken.MarkWhere(),
                    Name = full.Name,
                    Type = full.Type,
                    Comment = full.Comment
                };
                result = DbMyCon.SqlUpdate(main);
                if (result.Success)
                    strSql += result.Result.ToMyString();
            }
            //更新配置信息
            if (!string.IsNullOrEmpty(full.ProbenToken))
            {
                full.ProbenToken = full.ProbenToken.MarkWhere();
                result = DbMyCon.SqlUpdate(full);
                if (result.Success)
                    strSql += result.Result.ToMyString();
            }
            if (string.IsNullOrEmpty(strSql))
            {
                result.Success = false;
                result.Result = "新增样品信息为空";
                return result;
            }
            return DbMyCon.ExcuteSQL(strSql);
        }

        /// <summary>
        /// 删除样品
        /// </summary>
        /// <param name="full"></param>
        /// <param name="DeleteRelated"></param>
        /// <returns></returns>
        public CallResult DeleteProben(List<ModelProbenDetail> LstProben)
        {
            List<ModelProbenDetail> LstDel = new List<ModelProbenDetail>();
            foreach (ModelProbenDetail full in LstProben)
            {
                ModelProbenDetail modFull = new ModelProbenDetail() { ProbenToken = full.ProbenToken };
                LstDel.Add(modFull);
            }
            return DbMyCon.ExcuteDelete(LstDel);
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public CallResult AddProbenElement(List<ModelProbenElem> LstProbenElement)
        {
            foreach (var item in LstProbenElement)
            {
                if (string.IsNullOrEmpty(item.MainToken))
                    continue;
                item.PgmToken = AnaPgm.Token;
            }
            return DbMyCon.ExcuteInsert(LstProbenElement, InsertMode.IngoreInsert);
        }

        /// <summary>
        /// 删除元素
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public CallResult DeleteProbenElement(List<ModelProbenElem> LstProbenElement)
        {
            List<ModelProbenElem> lstDel = new List<ModelProbenElem>();
            foreach (ModelProbenElem item in LstProbenElement)
            {
                if (string.IsNullOrEmpty(item.ProbenToken) || string.IsNullOrEmpty(item.Element))
                    continue;
                ModelProbenElem modElem = new ModelProbenElem()
                {
                    ProbenToken = item.ProbenToken,
                    Element = item.Element
                };
                lstDel.Add(modElem);
            }
            return DbMyCon.ExcuteDelete(lstDel);
        }

        /// <summary>
        /// 修改元素
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public CallResult UpdateProbenElement(List<ModelProbenElem> LstProbenElement)
        {
            foreach (ModelProbenElem item in LstProbenElement)
            {
                if (!string.IsNullOrEmpty(item.ProbenToken) && 
                    !string.IsNullOrEmpty(item.Element))
                {
                    item.MainToken = item.MainToken.MarkWhere();
                    item.ProbenToken = item.ProbenToken.MarkWhere();
                    item.PgmToken = item.PgmToken.MarkWhere();
                    item.Element = item.Element.MarkWhere();
                }
                item.ValueAttachMark();
            }
            return DbMyCon.ExcuteUpdate(LstProbenElement);
        }
    }

    /// <summary>
    /// 标准化 - 牌号管理
    /// </summary>
    public partial class SparkHelper
    {
        /// <summary>
        /// 获取当前仪器配置牌号列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetMaterialNameList()
        {
            return GetMaterialNameList(BaseIns.InsName);
        }

        /// <summary>
        /// 获取仪器牌号名称列表
        /// </summary>
        /// <param name="InsName">仪器名称，不指定则返回所有</param>
        /// <returns></returns>
        public List<string> GetMaterialNameList(string InsName)
        {
            List<string> Lst = new List<string>();
            ModelMaterialMain mod = new ModelMaterialMain();
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(InsName))
                strWhere = string.Format("where {0}='{1}'", mod.GetColumnAttr(x => x.InsName).Name, InsName);
            string strSql = string.Format("select distinct {0} from {1} {2} order by {0} asc",
                mod.GetColumnAttr(x => x.Material).Name,
                mod.GetTableAttr().ViewName, strWhere);
            CallResult _result = DbMyCon.ExcuteQuery(strSql);
            if (_result.Fail) return Lst;
            DataTable dt = _result.Result.ToMyDataTable();
            string strMaterialColName = mod.GetColumnAttr(x => x.Material).Name;
            foreach (DataRow row in dt.Rows)
            {
                string strMaterial = row[strMaterialColName].ToMyString();
                if (string.IsNullOrEmpty(strMaterial))
                    continue;
                Lst.Add(strMaterial);
            }
            return Lst;
        }

        /// <summary>
        /// 牌号主信息列表
        /// </summary>
        /// <returns></returns>
        public List<ModelMaterialMain> GetMaterialMain()
        {
            ModelMaterialMain mMain = new ModelMaterialMain()
            {
                InsName = BaseIns?.InsName
            };
            DataTable dt = DbMyCon.ExcuteQuery(mMain, "Material asc").Result.ToMyDataTable();
            List<ModelMaterialMain> Lst = ColumnDef.ToEntityList<ModelMaterialMain>(dt);
            return Lst;
        }

        /// <summary>
        /// 牌号主信息列表
        /// </summary>
        /// <returns></returns>
        public ModelMaterialMain GetMaterialMain(string MaterialName)
        {
            if ((AnaPgm?.Token).IsEmpty())
                return null;
            ModelMaterialMain mMain = new ModelMaterialMain()
            {
                Material = MaterialName,
                PgmToken = AnaPgm.Token
            };
            List<ModelMaterialMain> Lst = DbMyCon.ExcuteQuery(mMain).Result.ToMyList<ModelMaterialMain>();
            return Lst.MySelectAny(0);
        }

        /// <summary>
        /// 牌号关联元素列表
        /// </summary>
        /// <param name="MaterialName"></param>
        /// <returns></returns>
        public List<ModelMaterialFull> GetMaterialElemByAnaPgm(string Material)
        {
            ModelMaterialFull mod = new ModelMaterialFull()
            {
                InsName = BaseIns?.InsName,
                Matrix = AnaPgm?.Matrix,
                PgmName = AnaPgm?.PgmName,
                Material = Material
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod, ElementOrderBy).Result.ToMyDataTable();
            List<ModelMaterialFull> Lst = ColumnDef.ToEntityList<ModelMaterialFull>(dt);
            return Lst;
        }

        /// <summary>
        /// 牌号关联所有项 控样、分析程序
        /// </summary>
        /// <param name="MaterialToken">牌号Token</param>
        /// <returns></returns>
        public List<ModelMaterialFull> GetMaterialRelatedAllByToken(string MaterialToken)
        {
            if (MaterialToken.IsEmpty()) return null;
            ModelMaterialFull mod = new ModelMaterialFull() { MaterialToken = MaterialToken };
            List<ModelMaterialFull> Lst = DbMyCon.ExcuteQuery(mod, ElementOrderBy).Result.ToMyList<ModelMaterialFull>();
            return Lst;
        }

        /// <summary>
        /// 根据牌号获取控样列表
        /// </summary>
        /// <param name="Material"></param>
        /// <returns></returns>
        public List<string> GetMaterialProbenName(string Material)
        {
            string strSql = string.Format("select distinct T1_Name,T2_Name FROM {0} where InsName='{1}' and " +
                "Matrix='{2}' and PgmName='{3}' and Material='{4}'",
                TableAttribute.Table<ModelMaterialFull>().Name,
                BaseIns.InsName, AnaPgm.Matrix, AnaPgm.PgmName, Material);
            List<string> LstRelatedProbenName = new List<string>();
            DataTable dt = DbMyCon.ExcuteQuery(strSql).Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                LstRelatedProbenName.AppandList(row["T1_Name"].ToMyString());
                LstRelatedProbenName.AppandList(row["T2_Name"].ToMyString());
            }
            return LstRelatedProbenName;
        }
        /// <summary>
        /// 根据牌号获取检查样列表
        /// </summary>
        /// <param name="Material"></param>
        /// <returns></returns>
        public List<string> GetMaterialProbenCSName(string Material)
        {
            string strSql = string.Format("select distinct CS_Name FROM {0} where InsName='{1}' and " +
                "Matrix='{2}' and PgmName='{3}' and Material='{4}'",
                TableAttribute.Table<ModelMaterialFull>().Name,
                BaseIns.InsName, AnaPgm.Matrix, AnaPgm.PgmName, Material);
            List<string> LstRelatedProbenName = new List<string>();
            DataTable dt = DbMyCon.ExcuteQuery(strSql).Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                LstRelatedProbenName.AppandList(row["CS_Name"].ToMyString());
            }
            return LstRelatedProbenName;
        }

        /// <summary>
        /// 获取牌号的检查样列表
        /// </summary>
        /// <param name="Material"></param>
        /// <returns></returns>
        public List<ModelProbenDetail> GetMaterialProbenCS(string Material)
        {
            if (Material.IsEmpty())
                return null;
            ModelMaterialElem CmdModel = new ModelMaterialElem()
            {
                Material = Material,
                InsName = BaseIns?.InsName,
                Matrix = AnaPgm?.Matrix,
                PgmName = AnaPgm?.PgmName,
            };
            List<ModelMaterialElem> LstRelatedElem = DbMyCon?.ExcuteQuery(CmdModel).Result.ToMyList<ModelMaterialElem>();
            List<string> LstCSToken = new List<string>();
            foreach (var item in LstRelatedElem)
            {
                if (!LstCSToken.MyContains(item.CS_Token))
                    LstCSToken.Add(item.CS_Token);
            }
            string strCSTokenIn = LstCSToken.ToMyString(",",false,"'","'");
            ModelProbenDetail mod = new ModelProbenDetail() { ProbenToken = $"ProbenToken in({strCSTokenIn})".MarkExpress() };
            List<ModelProbenDetail> LstCSProben = DbMyCon?.ExcuteQuery(mod).Result.ToMyList<ModelProbenDetail>();
            return LstCSProben;
        }

        /// <summary>
        /// 获取牌号关联的控样
        /// </summary>
        /// <returns></returns>
        public List<ModelProbenDetail> GetMaterialRelatedProbenList(string Material)
        {
            ModelSpecPgm pgm = GetSpecificAnaPgm(AnaPgm);
            if (pgm == null)
                return null;
            string strSql = $"select distinct TS1_Token,TS2_Token from view_ana_material_elem where Material='{Material}' and PgmToken='{pgm.Token}'";
            List<string> LstRelatedProbenToken = new List<string>();
            DataTable dt = DbMyCon?.ExcuteQuery(strSql).Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                LstRelatedProbenToken.AppandList(row["TS1_Token"].ToMyString());
                LstRelatedProbenToken.AppandList(row["TS2_Token"].ToMyString());
            }
            if (LstRelatedProbenToken.MyCount() == 0)
                return new List<ModelProbenDetail>();
            string strProbenToken = LstRelatedProbenToken.ToMyString(",", true, "'", "'");
            ModelProbenDetail mod = new ModelProbenDetail()
            {
                ProbenToken = $" ProbenToken in({strProbenToken}) ".MarkExpress()
            };
            List<ModelProbenDetail> lst =  DbMyCon?.ExcuteQuery(mod, "Name asc").Result.ToMyList<ModelProbenDetail>();
            return lst;
        }

        /// <summary>
        /// 添加牌号
        /// </summary>
        /// <param name="AnaPgmToken">分析程序</param>
        /// <param name="MaterialName">牌号名称</param>
        public CallResult AddMaterialMain(string AnaPgmToken, string MaterialName)
        {
            ModelMaterialMain main = new ModelMaterialMain()
            {
                Token = SystemDefault.UUID,
                PgmToken = AnaPgmToken,
                Material = MaterialName
            };
            return DbMyCon.ExcuteInsert(main, InsertMode.IngoreInsert);
        }

        /// <summary>
        /// 添加牌号
        /// </summary>
        /// <returns></returns>
        public CallResult AddMaterialMain(ModelMaterialMain main)
        {
            CallResult result = new CallResult();
            if ((main?.PgmToken).IsEmpty())
            {
                result.Result = "牌号未关联分析程序";
                return result;
            }
            if (string.IsNullOrEmpty(main?.Token))
                main.Token = SystemDefault.UUID;
            return DbMyCon.ExcuteInsert(main, InsertMode.IngoreInsert);
        }

        /// <summary>
        /// 修改牌号
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public CallResult UpdateMaterialMain(ModelMaterialMain main)
        {
            CallResult result = new CallResult();
            if (string.IsNullOrEmpty(main?.Token))
            {
                result.Result = "牌号数据错误，缺少主键";
                return result;
            }
            main.Token = main.Token.MarkWhere();
            return DbMyCon.ExcuteUpdate(main);
        }

        /// <summary>
        /// 删除牌号
        /// </summary>
        /// <param name="LstMaterial">牌号列表</param>
        /// <returns></returns>
        public CallResult DeleteMaterial(List<ModelMaterialMain> LstMaterial)
        {
            string strSql = string.Empty;
            foreach (ModelMaterialMain main in LstMaterial)
            {
                ModelMaterialMain mMain = new ModelMaterialMain() { Token = main.Token };
                strSql += DbMyCon.SqlDelete(mMain).Result.ToMyString();                
            }
            return DbMyCon.ExcuteSQL(strSql);
        }

        /// <summary>
        /// 删除牌号元素
        /// </summary>
        /// <param name="LstMaterialElement"></param>
        /// <returns></returns>
        public CallResult DeleteMaterialElememt(List<ModelMaterialElem> LstMaterialElement)
        {
            List<ModelMaterialElem> lstDel = new List<ModelMaterialElem>();
            foreach (ModelMaterialElem item in LstMaterialElement)
            {
                if (string.IsNullOrEmpty(item.MaterialToken) || string.IsNullOrEmpty(item.Element))
                    continue;
                ModelMaterialElem modElem = new ModelMaterialElem()
                {
                    MaterialToken = item.MaterialToken,
                    Element = item.Element
                };
                lstDel.Add(modElem);
            }
            return DbMyCon.ExcuteDelete(lstDel);
        }
    }

    /// <summary>
    /// 偏差管理
    /// </summary>
    public partial class SparkHelper
    {
        /// <summary>
        /// 获取分析曲线偏差管理
        /// </summary>
        /// <returns></returns>
        public List<ModelAnaDeviation> GetAnaDeviations()
        {
            ModelAnaDeviation mod = new ModelAnaDeviation()
            {
                PgmToken = AnaPgm.Token
            };
            List<ModelAnaDeviation> lst = DbMyCon?.ExcuteQuery(mod, "OrderID asc").Result.ToMyList<ModelAnaDeviation>();
            return lst;
        }

        /// <summary>
        /// 获取牌号检查管理
        /// </summary>
        /// <param name="MaterialName"></param>
        /// <returns></returns>
        public List<ModelMaterialDeviation> GetMaterialDeviations(string MaterialName)
        {
            List<ModelMaterialDeviation> lst = null;
            if (MaterialName.IsEmpty())
                return lst;
            ModelMaterialMain mMaterial = new ModelMaterialMain()
            {
                PgmToken = AnaPgm?.Token,
                Material = MaterialName
            };
            mMaterial = DbMyCon?.ExcuteQuery(mMaterial).Result.ToMyDataItem<ModelMaterialMain>();
            string MaterialToken = mMaterial?.Token;
            if (MaterialToken.IsEmpty())
                return lst;
            ModelMaterialDeviation mod = new ModelMaterialDeviation()
            {
                MaterialToken = MaterialToken
            };
            lst = DbMyCon?.ExcuteQuery(mod, ElementOrderBy).Result.ToMyList<ModelMaterialDeviation>();
            return lst;
        }

        /// <summary>
        /// 保存牌号偏差设置列表
        /// </summary>
        /// <param name="DeviationList"></param>
        /// <returns></returns>
        public CallResult SaveOrUpdateMaterialDeviation(IEnumerable<ModelMaterialDeviation> DeviationList)
        {
            CallResult res = new CallResult();
            List<ModelMaterialDeviation> LstUpd = new List<ModelMaterialDeviation>();
            foreach (ModelMaterialDeviation item in DeviationList)
            {
                if (item.MaterialToken.IsEmpty() || item.Element.IsEmpty())
                    continue;
                ModelMaterialDeviation itm = item.ModelClone();
                itm.ValueAttachMark();
                itm.MaterialToken = itm.MaterialToken.MarkWhere();
                itm.Element = itm.Element.MarkWhere();
                LstUpd.Add(itm);
            }
            if (LstUpd.MyCount() == 0)
            {
                res.Result = "没有可更新的钢种列表新信息";
                return res;
            }
            res = DbMyCon?.ExcuteUpdate(LstUpd);
            return res;
        }
    }

    /// <summary>
    /// 关联钢种
    /// </summary>
    public partial class SparkHelper
    {
        /// <summary>
        /// 获取钢种关联列表
        /// </summary>
        /// <returns></returns>
        public List<ModelRelatedSteelType> GetRelatedSteelTypeItems()
        {
            List<ModelRelatedSteelType> lst = null;
            ModelRelatedSteelType mod = new ModelRelatedSteelType()
            {
                InsName = BaseIns?.InsName
            };
            lst = DbMyCon?.ExcuteQuery(mod, "OrderID asc").Result.ToMyList<ModelRelatedSteelType>();
            return lst;
        }

        /// <summary>
        /// 获取钢种关联项
        /// </summary>
        /// <param name="SteelType"></param>
        /// <returns></returns>
        public ModelRelatedSteelType GetRelatedSteelTypeItem(string SteelType)
        {
            ModelRelatedSteelType item = null;
            ModelRelatedSteelType mod = new ModelRelatedSteelType()
            {
                InsName = BaseIns?.InsName,
                SteelType = SteelType
            };
            List<ModelRelatedSteelType>  lst = DbMyCon?.ExcuteQuery(mod, "OrderID asc").Result.ToMyList<ModelRelatedSteelType>();
            if (lst.MyCount() > 0)
                item = lst[0];
            return item;
        }

        /// <summary>
        /// 保存或更新钢种列表
        /// </summary>
        /// <param name="SteelList"></param>
        /// <returns></returns>
        public CallResult SaveOrUpdateRelatedSteelType(IEnumerable<ModelRelatedSteelType> SteelList)
        {
            CallResult res = new CallResult();
            List<ModelRelatedSteelType> LstAdd = new List<ModelRelatedSteelType>();
            List<ModelRelatedSteelType> LstUpd = new List<ModelRelatedSteelType>();
            foreach (ModelRelatedSteelType item in SteelList)
            {
                item.InsName = BaseIns?.InsName;
                if (item.ID.IsEmpty() && item.SteelType.IsNotEmpty())
                {
                    LstAdd.Add(item.ModelClone());
                }
                else if (item.ID.IsNotEmpty() && item.IsModified)
                {
                    ModelRelatedSteelType itm = item.ModelClone();
                    itm.ValueAttachMark();
                    itm.ID = itm.ID.MarkWhere();
                    LstUpd.Add(itm);
                }
            }
            string strSql = string.Empty;
            if (LstAdd.MyCount() > 0)
            {
                res = DbMyCon?.SqlInsert(LstAdd, InsertMode.IngoreInsert);
                if (res.Fail)  return res;
                strSql += res.Result.ToMyString();
            }
            if (LstUpd.MyCount() > 0)
            {
                 res = DbMyCon?.SqlUpdate(LstUpd);
                if (res.Fail) return res;
                strSql += res.Result.ToMyString();
            }
            if (strSql.IsEmpty())
            {
                res.Result ="没有可更新的钢种列表新信息";
                return res;
            }
            res = DbMyCon?.ExcuteSQL(strSql);
            return res;
        }

        /// <summary>
        /// 删除钢种列表
        /// </summary>
        /// <param name="SelectedList"></param>
        /// <returns></returns>
        public CallResult DeleteSteelRangeList(IEnumerable<ModelRelatedSteelType> SelectedList)
        {
            List<ModelRelatedSteelType> LstDel = new List<ModelRelatedSteelType>();
            foreach (ModelRelatedSteelType item in SelectedList)
            {
                LstDel.Add(new ModelRelatedSteelType() { InsName = BaseIns?.InsName, SteelType = item.SteelType });
            }
            CallResult res = DbMyCon?.ExcuteDelete(LstDel);
            return res;
        }
    }
}
