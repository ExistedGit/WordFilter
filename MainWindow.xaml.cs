using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            set { 
                bannedStrings = value;
                if (Analyzers != null)
                    foreach (var analyzer in Analyzers)
                        analyzer.SetBannedStrings(bannedStrings);
                OnPropertyChanged();
            } 
        }

        private int totalFileCount;
        private int analyzedFileCount;
        private ObservableCollection<string> bannedStrings;

        public MainWindow()
        {
            InitializeComponent();
            TotalFileCount = 0;
            AnalyzedFileCount = 0;
            DataContext = this;

            LB_Drives.ItemsSource = Analyzers = CreateAnalyzers();
        }


     
        private void BTN_PauseOrResumeAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Analyzer analyzer = (Analyzer)button.Tag;
            
            if(analyzer.State == Analyzer.AnalyzerState.Paused)
            {
                analyzer.Start();
                button.Content = "Pause";
            }
            else
            {
                if(analyzer.State == Analyzer.AnalyzerState.Running)
                {
                    analyzer.Pause();
                    button.Content = "Resume";
                }    
            }
        }

        private void BTN_StopAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Analyzer analyzer = (Analyzer)button.Tag;

            if (analyzer.State == Analyzer.AnalyzerState.Running)
                analyzer.Stop();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;

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
                        BannedStrings = new ObservableCollection<string>(text.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries));
                        LB_BannedStrings.ItemsSource = BannedStrings;
                        
                    }
                }
            }

        }

        

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
