﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "44D8DB057A7F646B1FF10B39FFEDA76EBF316FE628DD6470EF2F709309E9D01D"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
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
using WFCEditor;


namespace WFCEditor {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 246 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemOpenAs;
        
        #line default
        #line hidden
        
        
        #line 251 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemSaveAs;
        
        #line default
        #line hidden
        
        
        #line 254 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemSave;
        
        #line default
        #line hidden
        
        
        #line 292 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TB_NewElement;
        
        #line default
        #line hidden
        
        
        #line 300 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BTN_AddElement;
        
        #line default
        #line hidden
        
        
        #line 310 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BTN_DeleteElement;
        
        #line default
        #line hidden
        
        
        #line 321 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox LB_Elements;
        
        #line default
        #line hidden
        
        
        #line 376 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TB_CurrentPath;
        
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
            System.Uri resourceLocater = new System.Uri("/WFCEditor;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
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
            
            #line 9 "..\..\MainWindow.xaml"
            ((WFCEditor.MainWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MenuItemOpenAs = ((System.Windows.Controls.MenuItem)(target));
            
            #line 247 "..\..\MainWindow.xaml"
            this.MenuItemOpenAs.Click += new System.Windows.RoutedEventHandler(this.MenuItemOpenAs_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.MenuItemSaveAs = ((System.Windows.Controls.MenuItem)(target));
            
            #line 252 "..\..\MainWindow.xaml"
            this.MenuItemSaveAs.Click += new System.Windows.RoutedEventHandler(this.MenuItemSaveAs_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.MenuItemSave = ((System.Windows.Controls.MenuItem)(target));
            
            #line 255 "..\..\MainWindow.xaml"
            this.MenuItemSave.Click += new System.Windows.RoutedEventHandler(this.MenuItemSave_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.TB_NewElement = ((System.Windows.Controls.TextBox)(target));
            
            #line 293 "..\..\MainWindow.xaml"
            this.TB_NewElement.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TB_NewElement_TextChanged);
            
            #line default
            #line hidden
            
            #line 294 "..\..\MainWindow.xaml"
            this.TB_NewElement.KeyDown += new System.Windows.Input.KeyEventHandler(this.TB_NewElement_KeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.BTN_AddElement = ((System.Windows.Controls.Button)(target));
            
            #line 301 "..\..\MainWindow.xaml"
            this.BTN_AddElement.Click += new System.Windows.RoutedEventHandler(this.BTN_AddElement_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BTN_DeleteElement = ((System.Windows.Controls.Button)(target));
            
            #line 312 "..\..\MainWindow.xaml"
            this.BTN_DeleteElement.Click += new System.Windows.RoutedEventHandler(this.BTN_DeleteElement_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.LB_Elements = ((System.Windows.Controls.ListBox)(target));
            
            #line 322 "..\..\MainWindow.xaml"
            this.LB_Elements.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.LB_Elemnts_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 9:
            this.TB_CurrentPath = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

