#pragma checksum "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6EA3F5B1E85E904B844A50A8F350D797F9A23AD1"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Engine.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Engine.Core
{


    /// <summary>
    /// ucResultConfig
    /// </summary>
    public partial class PageReportConfig : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector
    {


#line 10 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid _dgReportGroupList;

#line default
#line hidden


#line 17 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid _dgResultConfig;

#line default
#line hidden


#line 35 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label _ProgConentTitle;

#line default
#line hidden


#line 38 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _NewReportGroup;

#line default
#line hidden


#line 39 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _ModifyReportGroup;

#line default
#line hidden


#line 40 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _DelReportGroup;

#line default
#line hidden


#line 41 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _NewResultConfigNode;

#line default
#line hidden


#line 42 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _DelResultConfigNode;

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Engine;component/engine.core/report/pagereportconfig.xaml", System.UriKind.Relative);

#line 1 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:

#line 8 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
                    ((Engine.Core.ucResultConfig)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);

#line default
#line hidden
                    return;
                case 2:
                    this._dgReportGroupList = ((System.Windows.Controls.DataGrid)(target));

#line 10 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
                    this._dgReportGroupList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this._dgReportGroupList_SelectionChanged);

#line default
#line hidden
                    return;
                case 3:
                    this._dgResultConfig = ((System.Windows.Controls.DataGrid)(target));

#line 17 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
                    this._dgResultConfig.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this._dgResultConfig_MouseDoubleClick);

#line default
#line hidden
                    return;
                case 4:
                    this._ProgConentTitle = ((System.Windows.Controls.Label)(target));
                    return;
                case 5:
                    this._NewReportGroup = ((System.Windows.Controls.Button)(target));

#line 38 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
                    this._NewReportGroup.Click += new System.Windows.RoutedEventHandler(this._Cmd_Click);

#line default
#line hidden
                    return;
                case 6:
                    this._ModifyReportGroup = ((System.Windows.Controls.Button)(target));

#line 39 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
                    this._ModifyReportGroup.Click += new System.Windows.RoutedEventHandler(this._Cmd_Click);

#line default
#line hidden
                    return;
                case 7:
                    this._DelReportGroup = ((System.Windows.Controls.Button)(target));

#line 40 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
                    this._DelReportGroup.Click += new System.Windows.RoutedEventHandler(this._Cmd_Click);

#line default
#line hidden
                    return;
                case 8:
                    this._NewResultConfigNode = ((System.Windows.Controls.Button)(target));

#line 41 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
                    this._NewResultConfigNode.Click += new System.Windows.RoutedEventHandler(this._Cmd_Click);

#line default
#line hidden
                    return;
                case 9:
                    this._DelResultConfigNode = ((System.Windows.Controls.Button)(target));

#line 42 "..\..\..\..\Engine.Core\Report\PageReportConfig.xaml"
                    this._DelResultConfigNode.Click += new System.Windows.RoutedEventHandler(this._Cmd_Click);

#line default
#line hidden
                    return;
            }
            this._contentLoaded = true;
        }
    }
}
