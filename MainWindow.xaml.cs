using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using WordFilter.Entities;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
namespace WordFilter
{

    public partial class MainWindow : Window
    {
        public int TotalFileCount { get; set; } = 0;
        public int AnalyzedFileCount { get; set; } = 0;

        public ObservableCollection<Analyzer> Analyzers { get; set; }
        public ObservableCollection<string> BannedStrings { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;


            Analyzers = CreateAnalyzers();

            LB_Drives.ItemsSource = Analyzers;

        }

        private void BTN_PauseDriveAnalysis_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((Button)sender).Tag.GetType().ToString());

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item.Name.Equals("OpenMenu"))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "WordFilter Config Files|*.wfc";
                dialog.Title = "Открыть файл конфигурации...";
                dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using(StreamReader reader = new StreamReader(dialog.FileName))
                    {
                        string text = reader.ReadToEnd().Trim();
                        BannedStrings = new ObservableCollection<string>(text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries));
                    }
                }
            }
            
        }
    }
}
