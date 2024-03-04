using System;

namespace Engine.WpfBase
{
    /// <summary> 应用再Mvc中的特性，根据此特性查找View中的对应的名称 </summary>
    public sealed class ViewAttribute : Attribute
    {
        public string Name { get; set; }

        public ViewAttribute(string path)
        {
            this.Name = path;
        }
    }
}
