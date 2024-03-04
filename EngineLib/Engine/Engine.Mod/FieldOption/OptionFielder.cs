using Engine.Common;
using Engine.Data.DBFAC;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Engine.Mod
{
    /// <summary>
    /// 选项字段管理
    /// </summary>
    public class OptionFielder
    {
        private IDBFactory<ServerNode> _DB = DbFactory.CPU;

        public List<ModelFieldItem> FieldList = new List<ModelFieldItem>();

        private OptionFielder() { }

        private static OptionFielder _Instance;
        public static OptionFielder Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new OptionFielder();
                    _Instance.LoadDefaultList();
                }
                return _Instance;
            }
        }

        /// <summary>
        /// 默认加载所有字段
        /// </summary>
        private void LoadDefaultList()
        {
            FieldList = GetFieldList();
        }

        /// <summary>
        /// 获取所有字段设置项
        /// </summary>
        /// <returns></returns>
        public List<ModelFieldItem> GetFieldList()
        {
            return GetFieldList(new ModelFieldItem());
        }

        /// <summary>
        /// 获取字段设置项
        /// </summary>
        /// <param name="modelWhere"></param>
        /// <returns></returns>
        public List<ModelFieldItem> GetFieldList(ModelFieldItem modelWhere)
        {
            List<ModelFieldItem> LstModel = new List<ModelFieldItem>();
            if (modelWhere == null) return LstModel;
            DataTable dt = _DB.ExcuteQuery(modelWhere).Result.ToMyDataTable();
            LstModel = ColumnDef.ToEntityList<ModelFieldItem>(dt);
            return LstModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="FieldCode"></param>
        /// <returns></returns>
        public ModelFieldItem this[string GroupName, string FieldCode]
        {
            get
            {
                ModelFieldItem mod = default(ModelFieldItem);
                if (FieldList != null)
                {
                    if (string.IsNullOrEmpty(GroupName))
                        mod = FieldList.Where(x => x.FieldCode == FieldCode).FirstOrDefault();
                    else
                        mod = FieldList.Where(x => x.GroupName == GroupName && x.FieldCode == FieldCode).FirstOrDefault();
                }   
                return mod;
            }
        }
    }
}
