using Engine.WpfTheme;

namespace Engine.WpfBase
{
    public interface IThemeLocalizeService
    {
        ThemeLocalizeConfig LoadTheme();

        bool SaveTheme(ThemeLocalizeConfig theme);
    }
}