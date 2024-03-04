﻿using Engine.WpfBase;
using System.Collections.ObjectModel;
using System.Linq;

namespace Engine.WpfControl
{
    public class WindowLinkViewModel : LoginViewModel
    {
        private ObservableCollection<Link> _settingLinks = new ObservableCollection<Link>();
        /// <summary> 说明  </summary>
        public ObservableCollection<Link> SettingLinks
        {
            get { return _settingLinks; }
            set
            {
                _settingLinks = value;
                RaisePropertyChanged("SettingLinks");
            }
        }

        private ObservableCollection<TabLink> _tabLinks = new ObservableCollection<TabLink>();
        /// <summary> 说明  </summary>
        public ObservableCollection<TabLink> TabLinks
        {
            get { return _tabLinks; }
            set
            {
                _tabLinks = value;
                RaisePropertyChanged("TabLinks");
            }
        }


        private TabLink _currentLink;
        /// <summary> 说明  </summary>
        public TabLink CurrentLink
        {
            get { return _currentLink; }
            set
            {
                _currentLink = value;
                RaisePropertyChanged("CurrentLink");
            }
        }

        public void GoLinkAction(string controller, string action)
        {
            CurrentLink = this.TabLinks.FirstOrDefault(l => l.Controller == controller && l.Action == action);
        }

        public void GoLinkAction(string controller, string action, object[] parameter)
        {
            var link = this.TabLinks.FirstOrDefault(l => l.Controller == controller && l.Action == action);

            if (link != null) link.Parameter = parameter;

            //  Do ：若指向的Link相同则使用刷新
            if (CurrentLink == link)
            {
                CurrentLink = null;
            } 

            CurrentLink = link;
        }

    }
}
