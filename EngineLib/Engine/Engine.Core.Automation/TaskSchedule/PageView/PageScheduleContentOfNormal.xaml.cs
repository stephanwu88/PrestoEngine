using Engine.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Engine.Core.TaskSchedule
{
    /// <summary>
    /// PageScheduleContentOfNormal.xaml 的交互逻辑
    /// </summary>
    public partial class PageScheduleContentOfNormal : UserControl
    {
        private string _authority = string.Empty;
        private string _creator = string.Empty;
        private NormalCard _NormalCard = new NormalCard();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="authority">"ReadOnly" or "Add" or "Edit"</param>
        public PageScheduleContentOfNormal(string creator,string authority)
        {
            InitializeComponent();
            _authority = authority;
            _creator = creator;
            _OptionCard_Normal_Name.Text = "";
            _OptionCard_Normal_Description.Text = "";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _OptionCard_Normal_Creator.Content = _creator;

            if (_authority == "Add" || _authority == "Edit")
            {
                //设定窗口可输入 Add Edit模式
                _OptionCard_Normal_Name.IsReadOnly = false;
                _OptionCard_Normal_Description.IsReadOnly = false;
                _OptionCard_Normal_Name.Background = new SolidColorBrush(Colors.White);
                _OptionCard_Normal_Description.Background = new SolidColorBrush(Colors.White);
            }
            if(_authority == "ReadOnly")
            {
                //绑定事件 ucScheduleContent选中数据变化时，只读选项卡需要同步显示
                //Tag = ucScheduleContent.xaml
                if (Tag != null)
                    (Tag as PageScheduleContent).ScheduleContent_SelectionChanged += ScheduleContent_SelectionChanged;
            }
            if (_authority == "Edit")
            {
                //在编辑窗口模式下，窗口加载时，将主窗口点选的信息加载到默认显示
                NormalCard OptionCard = (Tag as PageScheduleContent).OptionCard_Normal;
                if(OptionCard != null)
                {
                    _OptionCard_Normal_Name.Text = OptionCard.Name;
                    _OptionCard_Normal_Creator.Content = OptionCard.Creator;
                    _OptionCard_Normal_Description.Text = OptionCard.Comment;
                }
            }
        }

        /// <summary>
        /// 主窗口选中项发生变化
        /// </summary>
        /// <param name=""></param>
        private void ScheduleContent_SelectionChanged(PageScheduleContent sender, ScheduleContent arg2, ParamSetting arg3)
        {
            if (arg2 == null)
                return;
            ScheduleContent content = arg2;
            _OptionCard_Normal_Name.Text = content.Name.ToMyString();
            _OptionCard_Normal_Creator.Content = content.Creator.ToMyString();
            _OptionCard_Normal_Description.Text = content.Comment.ToMyString();
        }

        /// <summary>
        /// Normal选项卡页面设定项编码
        /// </summary>
        /// <returns></returns>
        private NormalCard NormalOfContent()
        {
            NormalCard card = new NormalCard();
            string Normal_name = _OptionCard_Normal_Name.Text.Trim();
            string Normal_Creator = _OptionCard_Normal_Creator.Content.ToString().Trim();
            string Normal_Desc = _OptionCard_Normal_Description.Text.Trim();
            card.Name = Normal_name;
            card.Creator = Normal_Creator;
            card.Comment = Normal_Desc;
            return card;
        }

        /// <summary>
        /// 常规选项卡内容模型
        /// </summary>
        public NormalCard NormalContent
        {
            get => NormalOfContent();
            set
            {
                NormalContent = value;
            }
        }

        /// <summary>
        /// "ReadOnly" or "Add" or "Edit"
        /// </summary>
        public string Authority
        {
            get => _authority;
        }
    }
}
