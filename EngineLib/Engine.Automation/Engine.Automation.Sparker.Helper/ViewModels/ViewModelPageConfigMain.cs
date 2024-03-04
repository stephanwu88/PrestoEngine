using Engine.Common;
using Engine.MVVM;
using System.Collections.ObjectModel;

namespace Engine.Automation.Sparker
{
    public class ViewModelPageConfigMain : ViewFrameBase
    {
        /// <summary>
        /// 仪器名称
        /// </summary>
        private string InsName;

        public ViewModelPageConfigMain(string insName)
        {
            if (!IsDesignMode)
            {
                InsName = insName;
                MenuItemList = new ObservableCollection<PrsMenuItem>()
                {
                    //new PrsMenuItem(){ PageID="1",Type="RadioButtonTemplate", Icon="&#xe63c;".UnicodeToString(),Label="仪器同步",Page = new PageOblfSynchronize(){ InsName = InsName } },
                    new PrsMenuItem(){ PageID="2",Type="RadioButtonTemplate", Icon="&#xe63c;".UnicodeToString(),Label="分析组",Page = new PageAnaPgm() },
                    new PrsMenuItem(){ PageID="3",Type="RadioButtonTemplate", Icon="&#xe63c;".UnicodeToString(),Label="控样管理",Page = new PageProben() },
                    new PrsMenuItem(){ PageID="4",Type="RadioButtonTemplate", Icon="&#xe63c;".UnicodeToString(),Label="牌号管理",Page = new PageMaterial() },
                    new PrsMenuItem(){ PageID="5",Type="RadioButtonTemplate", Icon="&#xe63c;".UnicodeToString(),Label="标准化样品",Page = new PageProbenStd() },
                    new PrsMenuItem(){ PageID="6",Type="RadioButtonTemplate", Icon="&#xe63c;".UnicodeToString(),Label="牌号检查",Page = null },
                };
                MenuCommand.Execute(MenuItemList[2]);
            }
        }
    }
}
