﻿#pragma checksum "..\..\..\..\Engine.Core\TaskSchedule\winScheduleEditor.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "23EE13B209BBABCC6A652533FF1D514B2FAC1FA7"
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
    /// winScheduleEditor
    /// </summary>
    public partial class winScheduleEditor : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\Engine.Core\TaskSchedule\winScheduleEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl _TabScheduleContentList;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\Engine.Core\TaskSchedule\winScheduleEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _CmdSure;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\Engine.Core\TaskSchedule\winScheduleEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _CmdCancel;
        
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
            System.Uri resourceLocater = new System.Uri("/Engine;component/engine.core/taskschedule/winscheduleeditor.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Engine.Core\TaskSchedule\winScheduleEditor.xaml"
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
            
            #line 8 "..\..\..\..\Engine.Core\TaskSchedule\winScheduleEditor.xaml"
            ((Engine.Core.TaskSchedule.winScheduleEditor)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this._TabScheduleContentList = ((System.Windows.Controls.TabControl)(target));
            return;
            case 3:
            this._CmdSure = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\..\..\Engine.Core\TaskSchedule\winScheduleEditor.xaml"
            this._CmdSure.Click += new System.Windows.RoutedEventHandler(this._CmdSure_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this._CmdCancel = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\..\..\Engine.Core\TaskSchedule\winScheduleEditor.xaml"
            this._CmdCancel.Click += new System.Windows.RoutedEventHandler(this._CmdCancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

