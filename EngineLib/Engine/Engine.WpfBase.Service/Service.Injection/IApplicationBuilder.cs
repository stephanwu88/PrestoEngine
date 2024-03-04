﻿using Engine.WpfTheme;
using System;

namespace Engine.WpfBase
{
    public class ApplicationBuilder : IApplicationBuilder
    {

    }

    public interface IApplicationBuilder
    {

    }

    public static class ApplicationBuilderExtention
    {
        /// <summary>
        /// 设置主题
        /// </summary>
        /// <param name="builder"> 主程序构建对象 </param>
        /// <param name="theme"> 主题 </param>
        /// <returns></returns>
        public static IApplicationBuilder UseTheme(this IApplicationBuilder builder, Action<ThemeService> theme)
        {
            theme?.Invoke(ThemeService.Current);

            return builder;
        }

        /// <summary>
        /// 用本地保存的配置初始化主题，若本地没有保存业务则使用默认主题
        /// </summary>
        /// <param name="builder"> 主程序构建对象 </param>
        /// <param name="useDefaultTheme">  默认主题 </param>
        /// <returns></returns>
        public static IApplicationBuilder UseLocalTheme(this IApplicationBuilder builder, Action<ThemeService> useDefaultTheme, int version=0)
        {

            IThemeLocalizeService localConfig = ServiceRegistry.Instance.GetInstance<IThemeLocalizeService>();

            ThemeLocalizeConfig local = localConfig?.LoadTheme();

            if (local != null && local.Version == version)
            {
                //  Do：设置默认主题
                builder.UseTheme(l =>
                { 
                    l.LoadFrom(local);

                    l.Version = version;
                });

                return builder;
            }

            useDefaultTheme?.Invoke(ThemeService.Current);

            return builder;


        }

        /// <summary>
        /// 主题保存
        /// </summary>
        /// <param name="builder"> 主程序构建对象 </param>
        /// <returns></returns>
        public static bool SaveLocalTheme(this IApplicationBuilder builder)
        {

            IThemeLocalizeService localConfig = ServiceRegistry.Instance.GetInstance<IThemeLocalizeService>();

            if (localConfig == null) return false;

            return localConfig.SaveTheme(ThemeService.Current.SaveTo());
        }
    }
}
