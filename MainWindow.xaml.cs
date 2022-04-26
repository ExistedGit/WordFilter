using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

using WordFilter.Entities;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
namespace WordFilter
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public int TotalFileCount
        {
            get { return totalFileCount; }
            set
            {
                totalFileCount = value;
                OnPropertyChanged();
            }
        }
        public int AnalyzedFileCount
        {
            get { return analyzedFileCount; }
            set
            {
                analyzedFileCount = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Analyzer> Analyzers { get; set; }
        public ObservableCollection<string> BannedStrings { 
            get => bannedStrings; 
            set { bannedStrings = value; OnPropertyChanged(); } 
        }

        private int totalFileCount;
        private int analyzedFileCount;
        private ObservableCollection<string> bannedStrings;

        public MainWindow()
        {
            InitializeComponent();

            Analyzers = CreateAnalyzers();
            LB_Drives.ItemsSource = Analyzers;
            DataContext = this;
            
        }



        private void BTN_PauseDriveAnalysis_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(((Button)sender).Tag.GetType().ToString());

        }


        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (StreamReader reader = new StreamReader(dialog.FileName))
                    {
                        string text = reader.ReadToEnd().Trim();
                        BannedStrings = new ObservableCollection<string>(text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries));
                        LB_BannedStrings.ItemsSource = bannedStrings;
                    }
                }
            }

        }
    }
}
