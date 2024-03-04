using Engine.Automation.Sparker;
using Engine.Common;
using Engine.Data.DBFAC;
using Engine.Mod;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Engine.Automation.OBLF
{
    /// <summary>
    /// 基于Oblf的SparkHelper
    /// </summary>
    public partial class SparkHelperOblf: SparkHelper
    {
        /// <summary>
        /// Oblf数据库连接
        /// </summary>
        private static IDBFactory<LocalSource> DbConOblf;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SparkHelperOblf()
        {
            BuildOblfResource();
        }

        /// <summary>
        /// 创建Oblf数据获取源
        /// </summary>
        private void BuildOblfResource()
        {
            LocalSource LsOblf = new LocalSource()
            {
                //SourceFile = sCommon.GetStartUpPath() + @"\OBLFwin.mdb",
                SourceFile = @"C:\OBLFwin\data\OBLFwin.mdb",
                ProviderName = "engine.data.msaccess",
                Password = "flbO"
            };
            DbConOblf = DbCommon.CreateInstance<LocalSource>(LsOblf);
        }
    }

    /// <summary>
    /// OBLF仪器数据同步 同步后的数据查询
    /// </summary>
    public partial class SparkHelperOblf
    {
        #region OBLF数据同步 - 同步分析程序

        /// <summary>
        /// 同步Oblf仪器分析程序设置到本地(OblfWin.mdb-Pgm)
        /// </summary>
        /// <param name="InsName">仪器名称</param>
        /// <returns></returns>
        public string SynchronizeProgram()
        {
            //string objectFile = sCommon.GetStartUpPath() + @"\OBLFwin.mdb";
            //string sourceFile = @"C:\OBLFwin\data\OBLFwin.mdb";
            //bool CopySuccess = sCommon.FileCopy(sourceFile, objectFile);
            //if (!CopySuccess)
            //{
            //    return "获取OBLF材质数据文件失败";
            //}
            string strSql = "select * from Pgm order by PgmNr asc";
            DataTable dt = DbConOblf.ExcuteQuery(strSql).Result.ToMyDataTable();
            if (dt.Rows.Count == 0)
                return "未获取到仪器程序配置";
            List<ModelLocalSpecPgm> LstPgm = ColumnDef.ToEntityList<ModelLocalSpecPgm>(dt);
            foreach (ModelLocalSpecPgm item in LstPgm)
            {
                item.InsName = BaseIns.InsName;
            }
            ModelLocalSpecPgm mod = new ModelLocalSpecPgm() { InsName = BaseIns.InsName };
            CallResult _result1 = DbMyCon.SqlDelete<ModelLocalSpecPgm>(mod);
            CallResult _result2 = DbMyCon.SqlInsert<ModelLocalSpecPgm>(LstPgm);
            if (_result1.Fail) return _result1.Result.ToMyString();
            if (_result2.Fail) return _result2.Result.ToMyString();
            strSql = _result1.Result.ToMyString();
            strSql += _result2.Result.ToMyString();
            CallResult res = DbMyCon.ExcuteSQL(strSql);
            return res.Fail ? res.Result.ToMyString() : string.Empty;
        }

        #endregion

        #region OBLF数据同步 - 同步控样样本数据

        /// <summary>
        /// 同步控样列表以及控样的元素标定
        /// </summary>
        /// <returns></returns>
        public CallResult SynchronizeProbenAndElem()
        {
            CallResult result = new CallResult();
            CallResult _result1 = SynchronizeProben();
            CallResult _result2 = SynchronizeProbenElem();
            if (_result1.Fail)
            {
                result.Result = "同步控样列表发生错误\r\n" + _result1.Result.ToMyString() + "\r\n";
                return result;
            }
            if (_result2.Fail)
            {
                result.Result += "同步控样元素标定发生错误\r\n" + _result2.Result.ToMyString();
                return result;
            }
            string strSql = _result1.Result.ToMyString();
            strSql += _result2.Result.ToMyString();
            result = DbMyCon.ExcuteSQL(strSql);
            if (result.Fail)
            {
                result.Result = "执行控样数据同步时错误\r\n" + result.Result.ToMyString();
            }
            return result;
        }

        /// <summary>
        /// 同步样本数据，控样列表以及控样元素标定(OblfWin.mdb-Proben、ProbenEle)
        /// 同步样本列表
        /// </summary>
        /// <param name="BaseIns"></param>
        /// <returns></returns>
        private CallResult SynchronizeProben()
        {
            string strSql = "select ProbenID,ProbenNr,Name,ProbenTyp as ProbenType,Pgm as PgmNr from Proben order by Name asc";
            DataTable dt = DbConOblf.ExcuteQuery(strSql).Result.ToMyDataTable();
            List<ModelLocalProbenMain> LstProben = ColumnDef.ToEntityList<ModelLocalProbenMain>(dt);
            foreach (ModelLocalProbenMain item in LstProben)
            {
                item.InsName = BaseIns.InsName;
            }
            //移除本地原有数据，添加新数据
            ModelLocalProbenMain mod = new ModelLocalProbenMain() { InsName = BaseIns.InsName };
            CallResult _result1 = DbMyCon.SqlDelete(mod);
            CallResult _result2 = DbMyCon.SqlInsert(LstProben);
            if (_result1.Fail) return _result1;
            if (_result2.Fail) return _result2;
            strSql = _result1.Result.ToMyString();
            strSql += _result2.Result.ToMyString();
            return new CallResult() { Success = true, Result = strSql }; ;
        }

        /// <summary>
        /// 同步样本数据，控样列表以及控样元素标定(OblfWin.mdb-Proben、ProbenEle)
        /// 同步控样元素标定
        /// </summary>
        /// <returns></returns>
        private CallResult SynchronizeProbenElem()
        {
            string strSql = "select ProbenID, Element, Sollwert as SetPoint,MessRekalwert as StartValue, Messwert as ActualValue, " +
                "TolMess as MaxTolToStart,Additiv as Additive  " +
                "from ProbenEle where len(Element)>0 order by ProbenID asc";
            DataTable dt = DbConOblf.ExcuteQuery(strSql).Result.ToMyDataTable();
            List<ModelLocalProbenElem> LstProbenElem = ColumnDef.ToEntityList<ModelLocalProbenElem>(dt);
            foreach (ModelLocalProbenElem item in LstProbenElem)
            {
                item.InsName = BaseIns.InsName;
            }
            //移除本地原有数据，添加新数据
            ModelLocalProbenElem mod = new ModelLocalProbenElem() { InsName = BaseIns.InsName };
            CallResult _result1 = DbMyCon.SqlDelete(mod);
            CallResult _result2 = DbMyCon.SqlInsert(LstProbenElem);
            if (_result1.Fail) return _result1;
            if (_result2.Fail) return _result2;
            strSql = _result1.Result.ToMyString();
            strSql += _result2.Result.ToMyString();
            return new CallResult() { Success = true, Result = strSql };
        }

        #endregion

        #region OBLF数据同步 - 同步材质牌号程序
        /// <summary>
        /// 同步Oblf材质牌号文件werkstof.dta, 光谱材质牌号列表
        /// 材质牌号以及关联的控样
        /// </summary>
        public CallResult SynchronizeMaterial()
        {
            CallResult _result = new CallResult();
            _result = ReadMaterialMain();
            if (_result.Fail)
            {
                _result.Result = "读取光谱牌号名称遇到错误\r\n" + _result.Result.ToMyString();
                return _result;
            }
            List<ModelLocalMaterialMain> LstMaterialMain = _result.Result as List<ModelLocalMaterialMain>;
            _result = ReadMaterialElem();
            if (_result.Fail)
            {
                _result.Result = "读取光谱牌号元素遇到错误\r\n" + _result.Result.ToMyString();
                return _result;
            }
            Dictionary<string, ModelLocalMaterialElem> DicElem = new Dictionary<string, ModelLocalMaterialElem>();
            List<ModelLocalMaterialElem> LstMaterElem = _result.Result as List<ModelLocalMaterialElem>;
            DicElem = LstMaterElem.ToMyDictionary(x => x.Material, x => x);

            CallResult _result1 = DbMyCon.SqlDelete<ModelLocalMaterialMain>(new ModelLocalMaterialMain() { InsName = BaseIns.InsName });
            CallResult _result2 = DbMyCon.SqlDelete<ModelLocalMaterialElem>(new ModelLocalMaterialElem() { InsName = BaseIns.InsName });
            CallResult _result3 = DbMyCon.SqlInsert<ModelLocalMaterialMain>(LstMaterialMain);
            CallResult _result4 = DbMyCon.SqlInsert<ModelLocalMaterialElem>(LstMaterElem);
            if (_result1.Fail || _result2.Fail || _result3.Fail || _result4.Fail)
            {
                _result.Result = "执行牌号数据本地化发生错误\r\n";
                _result.Result += _result1.Fail ? _result1.Result.ToMyString() : string.Empty;
                _result.Result += _result2.Fail ? _result1.Result.ToMyString() : string.Empty;
                _result.Result += _result3.Fail ? _result1.Result.ToMyString() : string.Empty;
                _result.Result += _result4.Fail ? _result1.Result.ToMyString() : string.Empty;
            }
            if (_result.Fail) return _result;
            string strSql = _result1.Result.ToMyString();
            strSql += _result2.Result.ToMyString();
            strSql += _result3.Result.ToMyString();
            strSql += _result4.Result.ToMyString();
            _result = DbMyCon.ExcuteSQL(strSql);
            if (_result.Fail)
                _result.Result = "执行牌号数据本地化发生错误\r\n" + _result.Result.ToMyString();
            return _result;
        }

        //从Oblf数据文件中获取材质主信息
        private CallResult ReadMaterialMain()
        {
            CallResult _result = new CallResult();
            try
            {
                //指定目标文件
                string objectFile = sCommon.GetStartUpPath() + @"\werkstof.dta";
                string sourceFile = @"C:\OBLFwin\data\werkstof.dta";
                bool CopySuccess = sCommon.FileCopy(sourceFile, objectFile);
                if (!CopySuccess)
                {
                    _result.Result = "获取OBLF材质数据文件失败";
                    return _result;
                }
                //读取二进制流
                FileStream fs = new FileStream(objectFile, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                long len = br.BaseStream.Length;
                //将数据流装载到数组中
                byte[] fileBbyte = new byte[len];
                for (int i = 0; i < len; i++)
                    fileBbyte[i] = br.ReadByte();
                br.Close();
                fs.Close();
                File.Delete(objectFile);

                //循环遍历数据装载到实体
                int k = 0, l;
                List<ModelLocalMaterialMain> LstMaterial = new List<ModelLocalMaterialMain>();
                for (int i = 1; i < len; i++)
                {
                    if (Convert.ToChar(fileBbyte[i]) == 'S' && fileBbyte[i - 1] == 0)
                    {
                        ModelLocalMaterialMain mater = new ModelLocalMaterialMain()
                        {
                            InsName = BaseIns.InsName
                        };
                        k = i;
                        for (l = k + 1; l < k + 26; l++)
                        {
                            mater.Material += Convert.ToChar(fileBbyte[l]);
                        }
                        k = l + 25;
                        for (l = k; l < k + 25; l++)
                        {
                            //mater.MaterialName += Convert.ToChar(fileBbyte[l]);
                        }
                        k = l + 1;
                        for (l = k; l < k + 3; l++)
                        {
                            mater.Matrix += Convert.ToChar(fileBbyte[l]);
                        }
                        k = l + 1;
                        for (l = k; l < k + 15; l++)
                        {
                            mater.PgmName += Convert.ToChar(fileBbyte[l]);
                        }
                        i = l;
                        mater.Material = mater.Material.MidString("", "\0").Trim();
                        //mater.MaterialName = mater.MaterialName.Trim();
                        mater.Matrix = mater.Matrix.MidString("", "\0").Trim();
                        mater.PgmName = mater.PgmName.MidString("", "\0").Trim();
                        if (!string.IsNullOrEmpty(mater.Material) && !string.IsNullOrEmpty(mater.Matrix) &&
                            !string.IsNullOrEmpty(mater.PgmName))
                        {
                            LstMaterial.Add(mater);
                        }
                    }
                }
                _result.Result = LstMaterial;
                _result.Success = true;
            }
            catch (Exception ex)
            {
                _result.Result = ex.Message;
            }
            return _result;
        }

        /// <summary>
        /// 从Oblf数据文件中获取材质元素列表信息
        /// </summary>
        /// <returns></returns>
        private CallResult ReadMaterialElem()
        {
            CallResult _result = new CallResult();
            try
            {
                //指定目标文件
                string objectFile = sCommon.GetStartUpPath() + @"\werkstof.dta";
                string sourceFile = @"C:\OBLFwin\data\werkstof.dta";
                bool CopySuccess = sCommon.FileCopy(sourceFile, objectFile);
                if (!CopySuccess)
                {
                    _result.Result = "获取OBLF材质数据文件失败";
                    return _result;
                }
                //读取二进制流
                FileStream fs = new FileStream(objectFile, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                long len = br.BaseStream.Length;
                Byte[] fileBbyte = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    fileBbyte[i] = br.ReadByte();
                }
                br.Close();
                fs.Close();
                //删除文件
                File.Delete(objectFile);
                //循环遍历数据装载到实体
                List<ModelLocalMaterialElem> LstElem = new List<ModelLocalMaterialElem>();
                int l = 0;
                for (int i = 1; i < len; i++)
                {
                    if (Convert.ToChar(fileBbyte[i]) == 'T' && fileBbyte[i - 1] == 0)
                    {
                        ModelLocalMaterialElem Elem = new ModelLocalMaterialElem()
                        {
                            InsName = BaseIns.InsName
                        };
                        l = i;
                        string strHeader = string.Empty;
                        for (l = i + 1; l < i + 78; l++)
                        {
                            strHeader += Convert.ToChar(fileBbyte[l]);
                        }
                        Elem.Material = strHeader.MidString("", "\0").Trim();
                        //Elem.MaterialName = strHeader.MidString("\0T", "\0").Trim();
                        int TsCount = fileBbyte[l].ToMyInt();
                        Elem.ElemCount = TsCount.ToMyString();
                        l++; l++;
                        int n = 0;
                        for (n = 0; n < TsCount; n++)
                        {
                            Elem.Element = Convert.ToChar(fileBbyte[l + 26 * n]).ToString() + Convert.ToChar(fileBbyte[l + 1 + 26 * n]).ToString() + Convert.ToChar(fileBbyte[l + 2 + 26 * n]).ToString();
                            Elem.Type1stID = (fileBbyte[l + 15 + 26 * n] * 256 + fileBbyte[l + 16 + 26 * n]).ToString();
                            Elem.Type2stID = (fileBbyte[l + 19 + 26 * n] * 256 + fileBbyte[l + 20 + 26 * n]).ToString();

                            Elem.Element = Elem.Element.MidString("", "\0").Trim();
                            if (!string.IsNullOrEmpty(Elem.Material) && !string.IsNullOrEmpty(Elem.Element))
                                LstElem.CloneAdd(Elem);
                        }
                        i = l + 26 * n;
                    }
                }
                _result.Result = LstElem;
                _result.Success = true;
            }
            catch (System.Exception ex)
            {
                _result.Result = ex.Message;
            }
            return _result;
        }
        #endregion

        #region 同步数据本地化查询

        /// <summary>
        /// 获取仪器分析程序列表
        /// </summary>
        /// <returns></returns>
        public List<ModelLocalSpecPgm> GetLocalAnaPgmList()
        {
            ModelLocalSpecPgm mod = new ModelLocalSpecPgm()
            {
                ID = string.Format("InsName='{0}' order by PgmName asc", BaseIns.InsName)
                   .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalSpecPgm> LstSpecPgm = ColumnDef.ToEntityList<ModelLocalSpecPgm>(dt);
            return LstSpecPgm;
        }

        /// <summary>
        /// 获取控样列表
        /// </summary>
        /// <returns></returns>
        public List<ModelLocalProbenMain> GetLocalProbenMain()
        {
            ModelLocalProbenMain mod = new ModelLocalProbenMain()
            {
                ID = string.Format("InsName='{0}' order by Name asc", BaseIns.InsName)
                .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalProbenMain> Lst = ColumnDef.ToEntityList<ModelLocalProbenMain>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取指定控样的元素标定
        /// </summary>
        /// <returns></returns>
        public List<ModelLocalProbenElem> GetLocalProbenElem(string ProbenID)
        {
            string strOrderBy = string.Empty;
            List<ModelSheetColumn> Book = FieldManager.Instance["TB_Data_Steel"];
            foreach (ModelSheetColumn model in Book)
            {
                if (!model.ColName.ToMyString().ToLower().Contains("data_") ||
                    string.IsNullOrEmpty(model.ColDesc))
                    continue;
                if (!string.IsNullOrEmpty(strOrderBy))
                    strOrderBy += ",";
                strOrderBy += string.Format("'{0}'", model.ColDesc);
            }
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                //strOrderBy = string.Format(" order by field(Element,{0})", strOrderBy);  //系统默认未指定到的排至头部，这不是预期的
                //按照指定顺序返回，未指定到的排到尾部
                strOrderBy = string.Format(" order by case when Element in({0}) then 0 else 1 end,field(Element,{0})", strOrderBy);
            }
            ModelLocalProbenElem mod = new ModelLocalProbenElem()
            {
                ID = string.Format("InsName='{0}' and ProbenID='{1}' {2}", BaseIns.InsName, ProbenID, strOrderBy)
                .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalProbenElem> Lst = ColumnDef.ToEntityList<ModelLocalProbenElem>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取控样元素列表，封装至字典
        /// 控样名称【元素列表】
        /// </summary>
        /// <param name="LstProbenMain"></param>
        /// <returns></returns>
        public Dictionary<string, List<ModelLocalProbenElem>> GetLocalProbenDict(List<ModelLocalProbenMain> LstProbenMain)
        {
            Dictionary<string, List<ModelLocalProbenElem>> Dic = new Dictionary<string, List<ModelLocalProbenElem>>();
            if (LstProbenMain == null) return Dic;
            List<string> LstProbenID = LstProbenMain.Select(x => x.ProbenID).Distinct().ToList();
            if (LstProbenID.MyCount() == 0) return Dic;

            string strProbenID = LstProbenID.ToMyString(",", true, "'", "'");
            List<string> LstInsName = LstProbenMain.Select(x => x.InsName).Distinct().ToList();
            string WhereInsName = LstInsName.ToMyString(",", true, "'", "'");
            if (!string.IsNullOrEmpty(WhereInsName))
                WhereInsName = string.Format(" and InsName in({0})", WhereInsName);
            ModelLocalProbenElem mod = new ModelLocalProbenElem()
            {
                ID = string.Format("ProbenID in({0}) " + WhereInsName, strProbenID)
                    .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalProbenElem> LstElement = ColumnDef.ToEntityList<ModelLocalProbenElem>(dt);
            foreach (ModelLocalProbenMain item in LstProbenMain)
            {
                string strProbenName = item.Name;
                Dic.AppandDict(strProbenName, LstElement.Where(x => x.InsName == item.InsName &&
                        x.ProbenID == item.ProbenID).Distinct().ToList());
            }
            return Dic;
        }

        /// <summary>
        /// 获取本地同步后的牌号列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetLocalMaterialNameList()
        {
            List<string> Lst = new List<string>();
            ModelLocalMaterialMain mod = new ModelLocalMaterialMain()
            {
                ID = string.Format("InsName='{0}' order by Material asc", BaseIns.InsName.ToMyString())
                .MarkExpress()
            };
            CallResult _result = DbMyCon.ExcuteQuery(mod);
            if (_result.Fail) return Lst;
            DataTable dt = _result.Result.ToMyDataTable();
            foreach (DataRow row in dt.Rows)
            {
                string strMaterial = row["Material"].ToMyString();
                if (string.IsNullOrEmpty(strMaterial))
                    continue;
                Lst.Add(strMaterial);
            }
            return Lst;
        }

        /// <summary>
        /// 获取材质牌号列表 - 主信息
        /// </summary>
        /// <returns></returns>
        public List<ModelLocalMaterialMain> GetLocalMaterialMain()
        {
            ModelLocalMaterialMain mod = new ModelLocalMaterialMain()
            {
                ID = string.Format("InsName='{0}' order by Material asc", BaseIns.InsName)
                        .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalMaterialMain> Lst = ColumnDef.ToEntityList<ModelLocalMaterialMain>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取指定控样的元素标定
        /// </summary>
        /// <returns></returns>
        public List<ModelLocalMaterialElem> GetLocalMaterialElem(string Material)
        {
            string strOrderBy = string.Empty;
            List<ModelSheetColumn> Book = FieldManager.Instance["TB_Data_Steel"];
            foreach (ModelSheetColumn model in Book)
            {
                if (!model.ColName.ToMyString().ToLower().Contains("data_") ||
                    string.IsNullOrEmpty(model.ColDesc))
                    continue;
                if (!string.IsNullOrEmpty(strOrderBy))
                    strOrderBy += ",";
                strOrderBy += string.Format("'{0}'", model.ColDesc);
            }
            if (!string.IsNullOrEmpty(strOrderBy))
            {
                //strOrderBy = string.Format(" order by field(Element,{0})", strOrderBy);  //系统默认未指定到的排至头部，这不是预期的
                //按照指定顺序返回，未指定到的排到尾部
                strOrderBy = string.Format(" order by case when Element in({0}) then 0 else 1 end,field(Element,{0})", strOrderBy);
            }
            ModelLocalMaterialElem mod = new ModelLocalMaterialElem()
            {
                ID = string.Format("InsName='{0}' and Material='{1}' {2}", BaseIns.InsName, Material, strOrderBy)
                .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalMaterialElem> Lst = ColumnDef.ToEntityList<ModelLocalMaterialElem>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取控样元素列表，封装至字典
        /// </summary>
        /// <param name="LstProbenMain"></param>
        /// <returns></returns>
        public Dictionary<string, List<ModelLocalMaterialElem>> GetLocalMaterial(List<ModelLocalMaterialMain> LstMaterialMain)
        {
            Dictionary<string, List<ModelLocalMaterialElem>> Dic = new Dictionary<string, List<ModelLocalMaterialElem>>();
            if (LstMaterialMain == null) return Dic;
            List<string> LstMaterialName = LstMaterialMain.Select(x => x.Material).Distinct().ToList();
            if (LstMaterialName.MyCount() == 0) return Dic;

            string strMaterialName = LstMaterialName.ToMyString(",", true, "'", "'");
            List<string> LstInsName = LstMaterialMain.Select(x => x.InsName).Distinct().ToList();
            string WhereInsName = LstInsName.ToMyString(",", true, "'", "'");
            if (!string.IsNullOrEmpty(WhereInsName))
                WhereInsName = string.Format(" and InsName in({0})", WhereInsName);
            ModelLocalMaterialElem mod = new ModelLocalMaterialElem()
            {
                ID = string.Format("Material in({0}) " + WhereInsName, strMaterialName)
                    .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalMaterialElem> LstElement = ColumnDef.ToEntityList<ModelLocalMaterialElem>(dt);
            foreach (ModelLocalMaterialMain item in LstMaterialMain)
            {
                string strName = item.Material;
                if (string.IsNullOrEmpty(strName))
                    continue;
                Dic.AppandDict(strName, LstElement.Where(x => x.InsName == item.InsName &&
                        x.Material == strName).Distinct().ToList());
            }
            return Dic;
        }

        #endregion
    }

    /// <summary>
    /// 本地化同步数据 - 导入到设计环境
    /// </summary>
    public partial class SparkHelperOblf
    {
        /// <summary>
        /// 从本地同步表查询样品列表 - 样品源选择窗口
        /// </summary>
        /// <param name="InstrName">指定仪器名称，不指定则返回全部</param>
        /// <returns></returns>
        public List<ModelLocalProbenMain> GetLocalProbenSource(string InstrName = "")
        {
            string strWhereInsName = string.Empty;
            if (!string.IsNullOrEmpty(InstrName))
                strWhereInsName = string.Format(" and InsName='{0}' ", InstrName);
            ModelLocalProbenMain mod = new ModelLocalProbenMain()
            {
                ID = " ID>0 " + strWhereInsName + " order by field(InsName,'OBLF1','OBLF2'),Name asc"
                .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalProbenMain> Lst = ColumnDef.ToEntityList<ModelLocalProbenMain>(dt);
            return Lst;
        }

        /// <summary>
        /// 在同步后的本地表中 - 根据样品名称获取控样全信息
        /// </summary>
        /// <param name="LstProbenNameWhere"></param>
        /// <returns></returns>
        public List<ModelLocalProbenMain> GetLocalProbenSource(List<string> LstProbenNameWhere)
        {
            string strWhereInsName = string.Empty;
            ModelLocalProbenMain mod = new ModelLocalProbenMain()
            {
                InsName = BaseIns.InsName,
                Name = string.Format("Name in({0})", LstProbenNameWhere.ToMyString(",", true, "'", ","))
                .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalProbenMain> Lst = ColumnDef.ToEntityList<ModelLocalProbenMain>(dt);
            return Lst;
        }

        /// <summary>
        /// 导入控样样本数据 - 至导入接口表实现
        /// </summary>
        /// <param name="InstrName">仪器名称</param>
        /// <param name="ImportList">样本主信息列表</param>
        /// <returns></returns>
        public CallResult ImportProbenAddOrUpdateToIF(List<ModelLocalProbenMain> ImportList, bool ImportAnaValue = true)
        {
            CallResult res = new CallResult();
            if (ImportList == null)
            {
                res.Result = "导入列表为空，执行过程将被忽略！";
                return res;
            }
            ModelSpecPgm mod = new ModelSpecPgm();
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelSpecPgm> LstSpecPgm = ColumnDef.ToEntityList<ModelSpecPgm>(dt);
            //获取OBLF本地同步数据源，并创建成数据字典
            Dictionary<string, List<ModelLocalProbenElem>> Dic = GetLocalProbenDict(ImportList);
            //将数据字典转换到控样设计器数据表中
            List<ModelImportProben> LstImpProben = new List<ModelImportProben>();
            foreach (ModelLocalProbenMain item in ImportList)
            {
                //创建ProbenMain包含详细信息
                string strProbenMainToken = SystemDefault.UUID;
                string strProbenMainName = item.Name.ToMyString();
                foreach (ModelSpecPgm pgm in LstSpecPgm)
                {
                    if (string.IsNullOrEmpty(pgm.Token))
                        continue;
                    string strProbenToken = SystemDefault.UUID;
                    //添加ProbenMain
                    ModelImportProben Proben = new ModelImportProben()
                    {
                        ProbenToken = strProbenToken,
                        MainToken = strProbenMainToken,
                        PgmToken = pgm.Token,
                        Name = item.Name
                    };
                    LstImpProben.Add(Proben);
                    //创建ProbenElement,并关联到ProbenMain
                    List<ModelLocalProbenElem> LstElementSource = Dic.DictFieldValue(strProbenMainName);
                    if (LstElementSource == null)
                        continue;
                    //创建Proben元素标准值,并关联到ProbenMain
                    strProbenToken = SystemDefault.UUID;
                    foreach (ModelLocalProbenElem elem in LstElementSource)
                    {
                        LstImpProben.Add(new ModelImportProben()
                        {
                            MainToken = strProbenMainToken,
                            ProbenToken = strProbenToken,
                            PgmToken = pgm.Token,
                            Name = strProbenMainName,
                            Element = elem.Element,
                            SetPoint = elem.SetPoint,
                            StartValue = ImportAnaValue ? elem.StartValue : SystemDefault.StringEmpty,
                            ActualValue = ImportAnaValue ? elem.ActualValue : SystemDefault.StringEmpty,
                            TolStart = ImportAnaValue ? elem.MaxTolToStart : SystemDefault.StringEmpty,
                            HandleKey = ImportAnaValue ? "ImportAnaValue" : SystemDefault.StringEmpty
                        });
                    }
                }
            }
            res = DbMyCon.ExcuteInsert(LstImpProben);
            DbMyCon.ExcuteSQL("truncate table import_proben");
            return res;
        }

        /// <summary>
        /// 导入控样样本数据
        /// 从本地同步表获取数据导入到设计环境表
        /// </summary>
        /// <param name="ImportList"></param>
        /// <param name="ImportAnaValue"></param>
        /// <returns></returns>
        public CallResult ImportProbenAddOrUpdate(List<ModelLocalProbenMain> ImportList, bool ImportAnaValue = true)
        {
            CallResult res = new CallResult();
            if (ImportList == null)
            {
                res.Success = false;
                res.Result = "导入列表为空，执行过程将被忽略！";
                return res;
            }

            //获取OBLF本地同步数据源，并创建成数据字典
            Dictionary<string, List<ModelLocalProbenElem>> DicSrcProben = GetLocalProbenDict(ImportList);
            //将数据字典转换到控样设计器数据表中
            List<ModelImportProben> LstImpProben = new List<ModelImportProben>();
            foreach (ModelLocalProbenMain item in ImportList)
            {
                //创建ProbenMain包含详细信息
                string strProbenMainToken = SystemDefault.UUID;
                //string strProbenToken = SystemDefault.UUID;
                string strProbenMainName = item.Name.ToMyString();
                //添加ProbenMain
                ModelImportProben Proben = new ModelImportProben()
                {
                    //ProbenToken = strProbenToken,
                    //MainToken = strProbenMainToken,
                    PgmToken = AnaPgm.Token,
                    Name = item.Name
                };
                LstImpProben.Add(Proben);
                //创建ProbenElement,并关联到ProbenMain
                List<ModelLocalProbenElem> LstElementSource = DicSrcProben.DictFieldValue(strProbenMainName);
                if (LstElementSource == null)
                    continue;
                //创建Proben元素标准值,并关联到ProbenMain
                //strProbenToken = SystemDefault.UUID;
                foreach (ModelLocalProbenElem elem in LstElementSource)
                {
                    LstImpProben.Add(new ModelImportProben()
                    {
                        //MainToken = strProbenMainToken,
                        //ProbenToken = strProbenToken,
                        PgmToken = AnaPgm.Token,
                        InsName = BaseIns.InsName,
                        Name = strProbenMainName,
                        Element = elem.Element,
                        SetPoint = elem.SetPoint,
                        StartValue = ImportAnaValue ? elem.StartValue : SystemDefault.StringEmpty,
                        ActualValue = ImportAnaValue ? elem.ActualValue : SystemDefault.StringEmpty,
                        TolStart = ImportAnaValue ? elem.MaxTolToStart : SystemDefault.StringEmpty,
                    });
                }
            }
            //获取当前设计数据
            List<ModelImportProben> LstProbenImported = GetProbenImported("控样");

            //执行导入过程
            List<string> LstSql = new List<string>();
            string strSql = string.Empty;
            int i = 0;
            foreach (ModelImportProben item in LstImpProben)
            {
                //ProbenMain
                if (string.IsNullOrEmpty(item.Name))
                    continue;

                ModelImportProben Imp = LstProbenImported.FirstOrDefault(x => x.Name == item.Name);
                item.MainToken = Imp == null ? SystemDefault.UUID : Imp.MainToken;
                ModelProbenMain mMain = new ModelProbenMain()
                {
                    MainToken = item.MainToken,
                    Name = item.Name,
                    Type = "控样",
                    Comment = ""
                };
                if (Imp == null)
                {
                    strSql += DbMyCon.SqlInsert(mMain).Result.ToMyString();
                }
                else if (Imp.Type != item.Type)
                {
                    mMain.MainToken = mMain.MainToken.MarkWhere();
                    strSql += DbMyCon.SqlUpdate(mMain).Result.ToMyString();
                    mMain.MainToken = mMain.MainToken.GoOriginal();
                }

                //ProbenStd
                Imp = LstProbenImported.FirstOrDefault(x => x.Name == item.Name && x.Element == item.Element);
                if (!string.IsNullOrEmpty(item.Element))
                {
                    ModelProbenStd mStd = new ModelProbenStd()
                    {
                        MainToken = mMain.MainToken,
                        Element = item.Element,
                        SetPoint = item.SetPoint
                    };
                    if (Imp == null)
                        strSql += DbMyCon.SqlInsert(mStd).Result.ToMyString();
                    else if (Imp.SetPoint != item.SetPoint && ImportAnaValue)
                    {
                        mStd.MainToken = mStd.MainToken.MarkWhere();
                        mStd.Element = mStd.Element.MarkWhere();
                        strSql += DbMyCon.SqlUpdate(mStd).Result.ToMyString();
                    }
                }

                //ProbenDetail
                Imp = LstProbenImported.FirstOrDefault(x => x.Name == item.Name && x.PgmToken == item.PgmToken && x.ProbenToken.MyLength() > 0);
                item.ProbenToken = Imp == null ? SystemDefault.UUID : Imp.ProbenToken;
                ModelProbenDetail mDetail = new ModelProbenDetail()
                {
                    ProbenToken = item.ProbenToken,
                    MainToken = mMain.MainToken,
                    PgmToken = AnaPgm.Token,
                };
                if (Imp == null)
                    strSql += DbMyCon.SqlInsert(mDetail).Result.ToMyString();

                //ProbenElement
                Imp = LstProbenImported.FirstOrDefault(x => x.Name == item.Name && x.Element == item.Element && x.PgmToken == item.PgmToken);
                if (!string.IsNullOrEmpty(item.Element))
                {
                    ModelProbenElem mElem = new ModelProbenElem();
                    item.MapperToModel(ref mElem);
                    mElem.MainToken = mMain.MainToken;
                    mElem.ProbenToken = mDetail.ProbenToken;
                    mElem.PgmToken = AnaPgm.Token;
                    if (Imp == null)
                        strSql += DbMyCon.SqlInsert(mElem).Result.ToMyString();
                    else if ((Imp.StartValue != item.StartValue || Imp.ActualValue != item.ActualValue) &&
                        ImportAnaValue)
                    {
                        mElem.MainToken = mElem.MainToken.MarkWhere();
                        mElem.ProbenToken = mElem.ProbenToken.MarkWhere();
                        mElem.PgmToken = mElem.PgmToken.MarkWhere();
                        mElem.Element = mElem.Element.MarkWhere();
                        strSql += DbMyCon.SqlUpdate(mElem).Result.ToMyString();
                    }
                }
                LstProbenImported.Add(item);
                if (i++ >= 400 && !string.IsNullOrEmpty(strSql))
                {
                    LstSql.Add(strSql);
                    strSql = string.Empty;
                    i = 0;
                }
            }
            if (!string.IsNullOrEmpty(strSql))
                LstSql.Add(strSql);
            res.Success = true;
            foreach (string sql in LstSql)
            {
                CallResult result = DbMyCon.ExcuteSQL(sql);
                if (result.Fail)
                {
                    res.Success = false;
                    res.Result += result.Result.ToMyString() + "\r\n";
                }
            }
            return res;
        }

        /// <summary>
        /// 导入控样样本标准值 - 国标文件，建样品标准库
        /// </summary>
        /// <param name="ImportList">样本主信息列表</param>
        /// <returns></returns>
        public CallResult ImportProbenStd(List<ModelLocalProbenMain> ImportList)
        {
            CallResult res = new CallResult();
            if (ImportList == null)
            {
                res.Result = "导入列表为空，执行过程将被忽略！";
                return res;
            }
            ModelSpecPgm mod = new ModelSpecPgm();
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelSpecPgm> LstSpecPgm = ColumnDef.ToEntityList<ModelSpecPgm>(dt);
            //获取OBLF本地同步数据源，并创建成数据字典
            Dictionary<string, List<ModelLocalProbenElem>> Dic = GetLocalProbenDict(ImportList);
            //将数据字典转换到控样设计器数据表中
            List<ModelProbenMain> LstProbenMain = new List<ModelProbenMain>();
            List<ModelProbenDetail> LstProbenFull = new List<ModelProbenDetail>();
            List<ModelProbenStd> LstProbenStd = new List<ModelProbenStd>();
            List<ModelProbenElem> LstProbenElem = new List<ModelProbenElem>();
            foreach (ModelLocalProbenMain item in ImportList)
            {
                //创建ProbenMain
                string strProbenMainToken = SystemDefault.UUID;
                string strProbenToken = SystemDefault.UUID;
                string strProbenMainName = item.Name.ToMyString();
                ModelProbenMain ProbenMain = new ModelProbenMain()
                {
                    MainToken = strProbenMainToken,
                    Name = item.Name,
                    Comment = ""
                };
                LstProbenMain.Add(ProbenMain);
                //创建ProbenElement,并关联到ProbenMain
                List<ModelLocalProbenElem> LstElementSource = Dic.DictFieldValue(strProbenMainName);
                if (LstElementSource != null) continue;
                //创建Proben元素标准值,并关联到ProbenMain
                foreach (ModelLocalProbenElem elem in LstElementSource)
                {
                    LstProbenStd.Add(new ModelProbenStd()
                    {
                        MainToken = strProbenMainToken,
                        Element = elem.Element,
                        SetPoint = elem.SetPoint
                    });
                }
                //为每一个分析程序添加此控样关联信息
                foreach (ModelSpecPgm pgm in LstSpecPgm)
                {
                    if (string.IsNullOrEmpty(pgm.Token))
                        continue;
                    ModelProbenDetail ProbenFull = new ModelProbenDetail()
                    {
                        ProbenToken = strProbenToken,
                        MainToken = strProbenMainToken,
                        PgmToken = pgm.Token,
                    };
                    LstProbenFull.Add(ProbenFull);
                    //创建ProbenElement,并关联到ProbenMain
                    foreach (ModelLocalProbenElem elem in LstElementSource)
                    {
                        LstProbenElem.Add(new ModelProbenElem()
                        {
                            MainToken = strProbenMainToken,
                            ProbenToken = strProbenToken,
                            PgmToken = pgm.Token,
                            Element = elem.Element,
                            SetPoint = elem.SetPoint,
                            StartValue = elem.StartValue,
                            ActualValue = elem.ActualValue,
                            TolStart = elem.MaxTolToStart,
                        });
                    }
                }
            }
            CallResult result1 = DbMyCon.SqlInsert(LstProbenMain);
            CallResult result2 = DbMyCon.SqlInsert(LstProbenFull);
            CallResult result3 = DbMyCon.SqlInsert(LstProbenStd);
            CallResult result4 = DbMyCon.SqlInsert(LstProbenElem);
            if (result1.Fail) return result1;
            if (result2.Fail) return result2;
            if (result3.Fail) return result3;
            if (result4.Fail) return result4;
            string strSql = result1.Result.ToMyString();
            strSql += result2.Result.ToMyString();
            strSql += result3.Result.ToMyString();
            strSql += result4.Result.ToMyString();
            res = DbMyCon.ExcuteSQL(strSql);
            return res;
        }

        /// <summary>
        /// 获取牌号选择源列表
        /// </summary>
        /// <param name="InstrName">指定仪器名称，不指定则返回全部</param>
        /// <returns></returns>
        public List<ModelLocalMaterialMain> GetLocalMaterialSource(string InstrName = "")
        {
            string strWhereInsName = string.Empty;
            if (!string.IsNullOrEmpty(InstrName))
                strWhereInsName = string.Format(" and InsName='{0}' ", InstrName);
            ModelLocalMaterialMain mod = new ModelLocalMaterialMain()
            {
                ID = " ID>0 " + strWhereInsName + " order by field(InsName,'OBLF1','OBLF2'),Material asc"
                .MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelLocalMaterialMain> Lst = ColumnDef.ToEntityList<ModelLocalMaterialMain>(dt);
            return Lst;
        }

        /// <summary>
        /// 导入牌号及关联数据
        /// </summary>
        /// <param name="ImportList"></param>
        /// <returns></returns>
        public CallResult ImportMaterialAddOrUpdateToIF(List<ModelLocalMaterialMain> ImportList)
        {
            CallResult res = new CallResult();
            if (ImportList == null)
            {
                res.Result = "导入列表为空，执行过程将被忽略！";
                return res;
            }
            //获取OBLF本地同步数据源，并创建成数据字典
            Dictionary<string, List<ModelLocalMaterialElem>> Dic = GetLocalMaterial(ImportList);
            //将数据字典转换到控样设计器数据表中
            List<ModelImportMaterial> LstImpMaterial = new List<ModelImportMaterial>();
            foreach (ModelLocalMaterialMain item in ImportList)
            {
                //创建ProbenMain包含详细信息
                string strMaterialToken = SystemDefault.UUID;
                string strMaterialName = item.Material.ToMyString();
                //添加MaterialMain
                ModelImportMaterial mMaterial = new ModelImportMaterial()
                {
                    MaterialToken = strMaterialToken,
                    Material = item.Material
                };
                LstImpMaterial.Add(mMaterial);
                //创建ProbenElement,并关联到ProbenMain
                List<ModelLocalMaterialElem> LstElementSource = Dic.DictFieldValue(strMaterialName);
                if (LstElementSource == null)
                    continue;
                //创建Proben元素标准值,并关联到ProbenMain
                strMaterialToken = SystemDefault.UUID;
                foreach (ModelLocalMaterialElem elem in LstElementSource)
                {
                    LstImpMaterial.Add(new ModelImportMaterial()
                    {
                        MaterialToken = strMaterialToken,
                        Material = elem.Material,
                        Element = elem.Element,
                        T1_Name = elem.Type1st,
                        T2_Name = elem.Type2st
                    });
                }
            }
            res = DbMyCon.ExcuteInsert(LstImpMaterial);
            DbMyCon.ExcuteSQL("truncate table import_material");
            return res;
        }

        /// <summary>
        /// 导入牌号及关联数据
        /// </summary>
        /// <param name="ImportList"></param>
        /// <returns></returns>
        public CallResult ImportMaterialAddOrUpdate(List<ModelLocalMaterialMain> ImportList)
        {
            CallResult res = new CallResult();
            if (ImportList == null)
            {
                res.Result = "导入列表为空，执行过程将被忽略！";
                return res;
            }
            //获取OBLF本地同步数据源，并创建成数据字典
            Dictionary<string, List<ModelLocalMaterialElem>> Dic = GetLocalMaterial(ImportList);
            //将数据字典转换到控样设计器数据表中
            List<ModelImportMaterial> LstImpMaterial = new List<ModelImportMaterial>();
            foreach (ModelLocalMaterialMain item in ImportList)
            {
                //创建ProbenMain包含详细信息
                string strMaterialToken = SystemDefault.UUID;
                string strMaterialName = item.Material.ToMyString();
                //添加MaterialMain
                ModelImportMaterial mMaterial = new ModelImportMaterial()
                {
                    //MaterialToken = strMaterialToken,
                    Material = item.Material
                };
                LstImpMaterial.Add(mMaterial);
                //创建ProbenElement,并关联到ProbenMain
                List<ModelLocalMaterialElem> LstElementSource = Dic.DictFieldValue(strMaterialName);
                if (LstElementSource == null)
                    continue;
                //创建Proben元素标准值,并关联到ProbenMain
                strMaterialToken = SystemDefault.UUID;
                foreach (ModelLocalMaterialElem elem in LstElementSource)
                {
                    LstImpMaterial.Add(new ModelImportMaterial()
                    {
                        //MaterialToken = strMaterialToken,
                        Material = elem.Material,
                        Element = elem.Element,
                        T1_Name = elem.Type1st,
                        T2_Name = elem.Type2st,
                        CS_Name = ""
                    });
                }
            }

            //获取当前设计数据
            List<ModelImportMaterial> LstMaterialImported = GetMaterialImported();
            List<ModelProbenMain> LstProbenMain = GetProbenMain("控样");
            Dictionary<string, ModelProbenMain> DicProbenMain = LstProbenMain.ToMyDictionary(x => x.Name, x => x);

            //执行导入过程
            List<string> LstSql = new List<string>();
            string strSql = string.Empty;
            int i = 0;
            foreach (ModelImportMaterial item in LstImpMaterial)
            {
                //MaterialMain
                if (string.IsNullOrEmpty(item.Material))
                    continue;
                ModelImportMaterial Imp = LstMaterialImported.FirstOrDefault(x => x.Material == item.Material);
                item.MaterialToken = Imp == null ? SystemDefault.UUID : Imp.MaterialToken;
                ModelMaterialMain mMain = new ModelMaterialMain()
                {
                    Token = item.MaterialToken,
                    Material = item.Material,
                    PgmToken = AnaPgm.Token,
                    Comment = item.Comment
                };
                if (Imp == null)
                    strSql += DbMyCon.SqlInsert(mMain).Result.ToMyString();
                else if (Imp.Comment != item.Comment)
                {
                    mMain.Token = mMain.Token.MarkWhere();
                    strSql += DbMyCon.SqlUpdate(mMain).Result.ToMyString();
                    mMain.Token = mMain.Token.GoOriginal();
                }

                //MaterialElement
                //ProbenStd
                Imp = LstMaterialImported.FirstOrDefault(x => x.Material == item.Material && x.Element == item.Element);
                if (!string.IsNullOrEmpty(item.Element))
                {
                    ModelMaterialElem mElem = new ModelMaterialElem();
                    item.MapperToModel(ref mElem);
                    mElem.MaterialToken = mMain.Token;
                    mElem.Element = item.Element;
                    if (!string.IsNullOrEmpty(item.T1_Name) && DicProbenMain.ContainsKey(item.T1_Name))
                        mElem.TS1_Token = DicProbenMain[item.T1_Name].ProbenToken;
                    else
                    if (!string.IsNullOrEmpty(item.T2_Name) && DicProbenMain.ContainsKey(item.T2_Name))
                        mElem.TS2_Token = DicProbenMain[item.T2_Name].ProbenToken;
                    if (!string.IsNullOrEmpty(item.CS_Name) && DicProbenMain.ContainsKey(item.CS_Name))
                        mElem.CS_Token = DicProbenMain[item.CS_Name].ProbenToken;

                    if (Imp == null)
                        strSql += DbMyCon.SqlInsert(mElem).Result.ToMyString();
                    else
                    {
                        mElem.MaterialToken = mElem.MaterialToken.MarkWhere();
                        mElem.Element = mElem.Element.MarkWhere();
                        mElem.TS1_Token = mElem.TS1_Token.ValueAttachMark();
                        mElem.TS2_Token = mElem.TS2_Token.ValueAttachMark();
                        mElem.CS_Token = mElem.CS_Token.ValueAttachMark();
                        strSql += DbMyCon.SqlUpdate(mElem).Result.ToMyString();

                    }
                }

                LstMaterialImported.Add(item);
                if (i++ >= 400 && !string.IsNullOrEmpty(strSql))
                {
                    LstSql.Add(strSql);
                    strSql = string.Empty;
                    i = 0;
                }
            }

            if (!string.IsNullOrEmpty(strSql))
                LstSql.Add(strSql);
            res.Success = true;
            foreach (string sql in LstSql)
            {
                CallResult result = DbMyCon.ExcuteSQL(sql);
                if (result.Fail)
                {
                    res.Success = false;
                    res.Result += result.Result.ToMyString() + "\r\n";
                }
            }
            return res;
        }

        /// <summary>
        /// 获取设计环境已导入的样品
        /// </summary>
        /// <param name="ProbenType"></param>
        /// <returns></returns>
        public List<ModelImportMaterial> GetMaterialImported()
        {
            ModelMaterialFull mod = new ModelMaterialFull()
            {
                ID = string.Format("InsName='{0}' and Matrix = '{1}' and PgmName='{2}' order by Material asc",
                   BaseIns.InsName, AnaPgm.Matrix, AnaPgm.PgmName).MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelImportMaterial> Lst = ColumnDef.ToEntityList<ModelImportMaterial>(dt);
            return Lst;
        }

        /// <summary>
        /// 获取该仪器设计环境已导入的样品
        /// </summary>
        /// <param name="ProbenType"></param>
        /// <returns></returns>
        public List<ModelImportProben> GetProbenImported(string ProbenType = "控样")
        {
            ModelProbenElem mod = new ModelProbenElem()
            {
                ID = string.Format("InsName='{0}' and Matrix = '{1}' and PgmName='{2}'  and Type = '{3}' order by Name asc",
                   BaseIns.InsName, AnaPgm.Matrix, AnaPgm.PgmName, ProbenType).MarkExpress()
            };
            DataTable dt = DbMyCon.ExcuteQuery(mod).Result.ToMyDataTable();
            List<ModelImportProben> Lst = ColumnDef.ToEntityList<ModelImportProben>(dt);

            //附加已导入的标准值记录行
            ModelProbenStd std = new ModelProbenStd() { Type = ProbenType };
            CallResult res = DbMyCon.ExcuteQuery(std);
            dt = DbMyCon.ExcuteQuery(std).Result.ToMyDataTable();
            List<ModelImportProben> Lst2 = ColumnDef.ToEntityList<ModelImportProben>(dt);
            Lst.AddRange(Lst2);
            return Lst;
        }
    }
}
