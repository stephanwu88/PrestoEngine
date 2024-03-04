using System.Collections.Generic;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// 树模型
    /// </summary>
    public class PropertyNodeItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertyNodeItem()
        {
            Children = new List<PropertyNodeItem>();
        }
        /// <summary>
        /// 索引
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 父索引
        /// </summary>
        public string ParentID { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool Expand { get; set; }
        /// <summary>
        /// 编辑状态图标
        /// </summary>
        public string EditIcon { get; set; }
        /// <summary>
        /// 提示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 自定义信息
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 子项
        /// </summary>
        public List<PropertyNodeItem> Children { get; set; }
    }
}
