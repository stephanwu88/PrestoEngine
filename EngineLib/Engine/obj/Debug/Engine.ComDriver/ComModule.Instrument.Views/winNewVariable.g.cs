﻿#pragma checksum "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "B3C0F2D383D5EBE31DDBF00BBFD56E420D779554"
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


namespace Engine.ComDriver.Instrument {
    
    
    /// <summary>
    /// winNewVariable
    /// </summary>
    public partial class winNewVariable : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CmdSure;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CmdCancel;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _DataName;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _DataAddr;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox _AddrType;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox _DataAccess;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox _Comment;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox _ContAddMode;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox _ChkAlarm;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox _RelatedVariable;
        
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
            System.Uri resourceLocater = new System.Uri("/Engine;component/engine.comdriver/commodule.instrument.views/winnewvariable.xaml" +
                    "", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
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
            this.CmdSure = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
            this.CmdSure.Click += new System.Windows.RoutedEventHandler(this.Cmd_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CmdCancel = ((System.Windows.Controls.Button)(target));
            
            #line 11 "..\..\..\..\Engine.ComDriver\ComModule.Instrument.Views\winNewVariable.xaml"
            this.CmdCancel.Click += new System.Windows.RoutedEventHandler(this.Cmd_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this._DataName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this._DataAddr = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this._AddrType = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this._DataAccess = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this._Comment = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this._ContAddMode = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 9:
            this._ChkAlarm = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 10:
            this._RelatedVariable = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

