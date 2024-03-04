using Engine.MVVM;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Engine.WpfBase
{
    /// <summary>
    /// 菜单栏模板选择器
    /// </summary>
    public class MenuTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            try
            {
                var myControl = container as FrameworkElement;
                ResourceDictionary resourceDict = new ResourceDictionary();
                resourceDict.Source = new Uri("/Engine;component/Engine.WpfBase/DataTemplate/DataTemplate.xaml", UriKind.RelativeOrAbsolute);
                //resourceDict.Source = new Uri("DataTemplate.xaml", UriKind.RelativeOrAbsolute);
                //Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                if (item is PrsMenuItem mi)
                {
                    //return (DataTemplate)myControl.FindResource("PopMenuButtonTemplate");
                    //return Application.Current.FindResource("PopMenuButtonTemplate") as DataTemplate;
                    return resourceDict[mi.Type] as DataTemplate;
                }
            }
            catch (Exception ex)
            {

            }            
            return null;
        }
    }
}
