namespace Engine.Mod
{
    /// <summary>
    /// 字段管理器 
    /// </summary>
    public partial class FieldManager
    {
        /// <summary>
        /// 表格列字段
        /// </summary>
        public static BookFielder Instance => BookFielder.Instance;
        /// <summary>
        /// 系统选项字段
        /// </summary>
        public static OptionFielder Option => OptionFielder.Instance;
    }
}
