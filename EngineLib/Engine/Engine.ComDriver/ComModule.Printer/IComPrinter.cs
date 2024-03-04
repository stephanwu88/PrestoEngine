
namespace Engine.ComDriver
{
    /// <summary>
    /// 打印机接口
    /// </summary>
    public interface IComPrinter
    {
        ///// <summary>
        ///// 驱动名称
        ///// </summary>
        //string DriverName { get; }
        ///// <summary>
        ///// 标签机设定
        ///// </summary>
        //PrintSet PrintSet { get; set; }
        ///// <summary>
        ///// 设置打印机
        ///// </summary>
        ///// <param name="label"></param>
        ///// <returns></returns>
        //bool SetUp(PrintSet label);
        /// <summary>
        /// 打印标签 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <returns></returns>
        bool PrintLabel(string PrnFile, string[] FieldContent);
        /// <summary>
        /// 打印标签 - 文件模板排版
        /// </summary>
        /// <param name="PrnFile">模板文件路径</param>
        /// <param name="FieldContent">字段内容</param>
        /// <param name="CopyCount">打印份数</param>
        /// <returns></returns>
        bool PrintLabel(string PrnFile, string[] FieldContent, int CopyCount);
        ///// <summary>
        ///// 打印标签 - 代码排版
        ///// </summary>
        ///// <param name="LstItem">排版列表</param>
        ///// <param name="CopyCount">打印份数</param>
        ///// <returns></returns>
        //bool PrintLabel(List<object> LstItem, int CopyCount);
    }
}
