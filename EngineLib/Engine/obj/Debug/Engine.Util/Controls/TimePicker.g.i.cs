﻿#pragma checksum "..\..\..\..\Engine.Util\Controls\TimePicker.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "672409BF4623F6DB64B9B57CB9867E7E1A072F92"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace Engine.Util.Controls {
    
    
    /// <summary>
    /// TimePicker
    /// </summary>
    public partial class TimePicker : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _Hour;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _Minite;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _Second;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CmdUp;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CmdDown;
        
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
            System.Uri resourceLocater = new System.Uri("/Engine;component/engine.util/controls/timepicker.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
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
            this._Hour = ((System.Windows.Controls.TextBox)(target));
            
            #line 19 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
            this._Hour.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextChanged);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
            this._Hour.SelectionChanged += new System.Windows.RoutedEventHandler(this.TextSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this._Minite = ((System.Windows.Controls.TextBox)(target));
            
            #line 21 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
            this._Minite.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextChanged);
            
            #line default
            #line hidden
            
            #line 21 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
            this._Minite.SelectionChanged += new System.Windows.RoutedEventHandler(this.TextSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this._Second = ((System.Windows.Controls.TextBox)(target));
            
            #line 23 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
            this._Second.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextChanged);
            
            #line default
            #line hidden
            
            #line 23 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
            this._Second.SelectionChanged += new System.Windows.RoutedEventHandler(this.TextSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.CmdUp = ((System.Windows.Controls.Button)(target));
            
            #line 29 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
            this.CmdUp.Click += new System.Windows.RoutedEventHandler(this.Cmd_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.CmdDown = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\..\..\Engine.Util\Controls\TimePicker.xaml"
            this.CmdDown.Click += new System.Windows.RoutedEventHandler(this.Cmd_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

