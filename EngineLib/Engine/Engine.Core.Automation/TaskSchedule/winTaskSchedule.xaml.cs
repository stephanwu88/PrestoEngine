using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Engine.Core.TaskSchedule
{
   
    /// <summary>
    /// winTaskSchedule.xaml 的交互逻辑
    /// </summary>
    public partial class winTaskSchedule : Window
    {
        public winTaskSchedule()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Height = 30;
            this.Width = 300;
            this.Top = 150;
            this.Left = SystemParameters.PrimaryScreenWidth - 400;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
              
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
            //_ScheduleContent.BindingGroup
            //ucScheduleContent ucContent = new ucScheduleContent();
            //ucContent.Tag = this;
            //_ScheduleFrame.Children.Add(ucContent);
            //ucContent.SetValue(Grid.ColumnProperty,2);

            ShowTreeView();
        }

        private void ShowTreeView()
        {
            //定义树
            List<PropertyNodeItem> itemList = new List<PropertyNodeItem>()
            {
                new PropertyNodeItem(){ ID="1", ParentID="0", DisplayName = "全自动快分检测系统",Name = "全自动快分检测系统", Tag="main", Icon = "/Engine;component/Assets/Image/Schedule2.png",EditIcon=""},
                new PropertyNodeItem(){ ID="1.1", ParentID="1", DisplayName = "实验室快分系统",Name = "实验室快分系统",Tag="lab", Icon = "/Engine;component/Assets/Image/ScheduleLib.png",EditIcon=""},
                new PropertyNodeItem(){ ID="1.1.1", ParentID="1.1", DisplayName = "风动圈制样任务",Name = "风动圈制样任务",Tag="schedule", Icon = "/Engine;component/Assets/Image/TaskMain.png",EditIcon=""},
                new PropertyNodeItem(){ ID="1.1.2", ParentID="1.1", DisplayName = "快分制样任务",Name = "快分制样任务",Tag="task_kf1",Icon = "/Engine;component/Assets/Image/TaskMain.png",EditIcon=""},
            };
            this.tvProperties.ItemsSource = Bind(itemList);
        }

        //绑定树
        List<PropertyNodeItem> Bind(List<PropertyNodeItem> Items)
        {
            List<PropertyNodeItem> listItems = new List<PropertyNodeItem>();
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ParentID == "0")
                    listItems.Add(Items[i]);
                else
                    FindDownward(Items, Items[i].ParentID).Children.Add(Items[i]);
            }
            return listItems;
        }
        //递归树
        PropertyNodeItem FindDownward(List<PropertyNodeItem> Items, string id)
        {
            if (Items == null)
                return null;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ID == id)
                    return Items[i];
                PropertyNodeItem node = FindDownward(Items[i].Children, id);
                if (node != null)
                    return node;
            }
            return null;
        }

        private void TvProperties_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string sel = ((sender as TreeView).SelectedValue as PropertyNodeItem).DisplayName ;
            string id = ((sender as TreeView).SelectedValue as PropertyNodeItem).ID;
            string treeGroup = ((sender as TreeView).SelectedValue as PropertyNodeItem).Tag;
            //_ScheduleFrame.Children.Remove(_ScheduleFrame.Tag);
            if (this._ScheduleContent.Children.Count>0)
            this._ScheduleContent.Children.RemoveAt(0);
            if (id.Split('.').Length == 3 && treeGroup.Length>0)
            {
                PageScheduleContent ucContent = new PageScheduleContent(treeGroup);
                ucContent.Tag = this;
                _ScheduleFrame.Tag = ucContent;
                //ucContent.SetValue(Grid.ColumnProperty, 2);
                //_ScheduleFrame.Children.Add(ucContent);
                this._ScheduleContent.Children.Add(ucContent);
            }

            //if (id.Split('.').Length == 4)
            //{
            //    // _ScheduleContent.BindingGroup
            //    ucScheduleContentOfTrigger ucContent = new ucScheduleContentOfTrigger("ReadWrite");
            //    ucContent.Tag = this;
            //    _ScheduleFrame.Tag = ucContent;
            //    //ucContent.SetValue(Grid.ColumnProperty, 2);
            //    //_ScheduleFrame.Children.Add(ucContent);
            //    this._ScheduleContent.Children.Add(ucContent);
               
            //}
            //else
            //{
            //    ucScheduleContent ucContent = new ucScheduleContent();
            //    ucContent.Tag = this;
            //    _ScheduleFrame.Tag = ucContent;
            //    //ucContent.SetValue(Grid.ColumnProperty, 2);
            //    //_ScheduleFrame.Children.Add(ucContent);
            //    this._ScheduleContent.Children.Add(ucContent);
            //}
        }

        private void _ToolBar_Button_Click(object sender, RoutedEventArgs e)
        {
            Button _Cmd = (Button)sender;
            switch (_Cmd.Name)
            {
                case "_CmdItemAdd":  //添加项目

                    break;
                case "_CmdItemDel":  //删除项目

                    break;
                case "_CmdItemEdit": //编辑项目

                    break;
            }
        }
    }
}
