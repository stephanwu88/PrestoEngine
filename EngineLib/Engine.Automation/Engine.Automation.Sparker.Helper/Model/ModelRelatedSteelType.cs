using Engine.Data;
using Engine.Data.DBFAC;
using System;

namespace Engine.Automation.Sparker
{
    [Table(Name = "ana_related_steel", ViewName = "view_ana_related_steel", Comments = "")]
    public partial class ModelRelatedSteelType : NotifyObject
    {
        [Column(Name = "ID", AI = true, PK = true, Comments = "")]
        public string ID { get; set; }

        [Column(Name = "InsName", Comments = "仪器名称")]
        public string InsName { get; set; }

        [Column(Name = "SteelType", Comments = "钢种")]
        public string SteelType { get; set; }

        [Column(Name = "MaterialToken", Comments = "")]
        public string MaterialToken { get; set; }

        [Column(Name = "PgmToken", ReadOnly = true, Comments = "")]
        public string PgmToken { get; set; }

        [Column(Name = "Matrix", ReadOnly = true, Comments = "")]
        public string Matrix { get; set; }

        [Column(Name = "PgmName", ReadOnly = true, Comments = "")]
        public string PgmName { get; set; }

        [Column(Name = "Material", ReadOnly = true, Comments = "")]
        public string Material { get; set; }

        [Column(Name = "CurrentState", ReadOnly = true, Comments = "")]
        public string CurrentState { get; set; }

        [Column(Name = "OrderID", Comments = "排序号")]
        public string OrderID
        {
            get { return _OrderID; }
            set
            {
                if (!string.IsNullOrEmpty(_OrderID) && _OrderID != value)
                    IsModified = true;
                _OrderID = value;
                RaisePropertyChanged();
            }
        }
        private string _OrderID;

        /// <summary>
        /// 数据已修改
        /// </summary>
        public bool IsModified
        {
            get { return _IsModified; }
            set { _IsModified = value; RaisePropertyChanged(); }
        }
        private bool _IsModified;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public override string ToString()
        //{
        //    return SteelType;
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class ModelRelatedSteelType : IEquatable<ModelRelatedSteelType>
    {
        public bool Equals(ModelRelatedSteelType other)
        {
            if (other == null)
                return false;

            return ID == other.ID;
        }
    }
}
