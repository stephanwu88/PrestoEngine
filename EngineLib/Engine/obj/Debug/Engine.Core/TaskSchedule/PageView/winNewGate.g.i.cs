﻿#pragma checksum "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "569FCEB28AB9A027AC4F0D29016994004C5E6F4E"
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
    /// winNewGate
    /// </summary>
    public partial class winNewGate : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox _GateType;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border _Border_ByStationState;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox _Group;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox _Item;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox _Sign;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox _ObjectVal;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox _Chk_Enable;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _Cmd_Sure;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button _Cmd_Cancel;
        
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
            System.Uri resourceLocater = new System.Uri("/Engine;component/engine.core/taskschedule/pageview/winnewgate.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
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
            
            #line 8 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
            ((Engine.Core.TaskSchedule.winNewGate)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this._GateType = ((System.Windows.Controls.ComboBox)(target));
            
            #line 11 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
            this._GateType.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this._GateType_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this._Border_ByStationState = ((System.Windows.Controls.Border)(target));
            return;
            case 4:
            this._Group = ((System.Windows.Controls.ComboBox)(target));
            
            #line 18 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
            this._Group.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this._Group_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this._Item = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this._Sign = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this._ObjectVal = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this._Chk_Enable = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 9:
            this._Cmd_Sure = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
            this._Cmd_Sure.Click += new System.Windows.RoutedEventHandler(this._Cmd_Sure_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this._Cmd_Cancel = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\..\..\Engine.Core\TaskSchedule\PageView\winNewGate.xaml"
            this._Cmd_Cancel.Click += new System.Windows.RoutedEventHandler(this._Cmd_Cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

