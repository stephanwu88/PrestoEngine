using System;
using System.Windows;

namespace Engine.Common
{
    /// <summary>
    /// 资源文件
    /// </summary>
    public partial class sCommon
    {
        /// <summary>
        /// 查找资源对象
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        public static object FindResurce(object resourceKey)
        {
            try
            {
                return Application.Current.TryFindResource(resourceKey);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取Uri对象
        /// </summary>
        /// <param name="UriString"></param>
        /// <param name="UriKind"></param>
        /// <returns></returns>
        public static Uri CreateUri(string UriString, UriKind UriKind = UriKind.RelativeOrAbsolute)
        {
            try
            {
                Uri.TryCreate(UriString, UriKind, out Uri uri);
                return uri;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}