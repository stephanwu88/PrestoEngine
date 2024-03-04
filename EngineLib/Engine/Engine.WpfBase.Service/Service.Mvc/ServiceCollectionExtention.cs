
namespace Engine.WpfBase
{
    public static class MvcServiceCollectionExtention
    {
        /// <summary> 注入单例模式 </summary>
        public static IServiceCollection UseMvc(this IServiceCollection service)
        {
            ServiceRegistry.Instance.UseMvc();

            return service;
        }
    }
}
