﻿#pragma checksum "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7FD7CA813891F6DE7E6C94EA6484CEFF779C29FE"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Engine.Core.TaskSchedule;
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


namespace Engine.Core.TaskSchedule {
    
    
    /// <summary>
    /// winTaskSchedule
    /// </summary>
    public partial class winTaskSchedule : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 138 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _CmdItemAdd;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _CmdItemDel;
        
        #line default
        #line hidden
        
        
        #line 144 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _CmdItemEdit;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid _ScheduleFrame;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView tvProperties;
        
        #line default
        #line hidden
        
        
        #line 190 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid _ScheduleContent;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Engine;component/engine.service/engine.service.core/taskschedule/wintaskschedule" +
                    ".xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
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
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
            ((Engine.Core.TaskSchedule.winTaskSchedule)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 137 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
            ((System.Windows.Controls.ToolBar)(target)).Loaded += new System.Windows.RoutedEventHandler(this.ToolBar_Loaded);
            
            #line default
            #line hidden
            return;
            case 3:
            this._CmdItemAdd = ((System.Windows.Controls.Button)(target));
            
            #line 138 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
            this._CmdItemAdd.Click += new System.Windows.RoutedEventHandler(this._ToolBar_Button_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this._CmdItemDel = ((System.Windows.Controls.Button)(target));
            
            #line 141 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
            this._CmdItemDel.Click += new System.Windows.RoutedEventHandler(this._ToolBar_Button_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this._CmdItemEdit = ((System.Windows.Controls.Button)(target));
            
            #line 144 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
            this._CmdItemEdit.Click += new System.Windows.RoutedEventHandler(this._ToolBar_Button_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this._ScheduleFrame = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            this.tvProperties = ((System.Windows.Controls.TreeView)(target));
            
            #line 158 "..\..\..\..\..\Engine.Service\Engine.Service.Core\TaskSchedule\winTaskSchedule.xaml"
            this.tvProperties.SelectedItemChanged += new System.Windows.RoutedPropertyChangedEventHandler<object>(this.TvProperties_SelectedItemChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this._ScheduleContent = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

