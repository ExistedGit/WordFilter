using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WordFilter.Entities;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Management;

namespace WordFilter
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public int TotalFileCount
        {
            get => totalFileCount;
            set
            {
                totalFileCount = value;
                OnPropertyChanged();
            }
        }
        public int AnalyzedFileCount
        {
            get => analyzedFileCount;
            set
            {
                analyzedFileCount = value;
                OnPropertyChanged();
            }
        }
        public event Action<MainWindow> SilentAllFilesCounted;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Analyzer> Analyzers
        {
            get => analyzers; set
            {
                analyzers = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> BannedStrings
        {
            get => bannedStrings;
            set
            {
                bannedStrings = value;
                if (Analyzers != null)
                    foreach (var analyzer in Analyzers)
                        analyzer.SetBannedStrings(bannedStrings);
                OnPropertyChanged();
            }
        }
        public string ReportFolderPath
        {
            get => reportFolderPath;
            set
            {
                reportFolderPath = value;

                if (Analyzers == null)
                {
                    CreateAnalyzers();
                    LB_Drives.ItemsSource = Analyzers;
                }
                if (DEBUG)
                {
                    BannedStrings = new ObservableCollection<string>(new string[] { "Москва", "Кремль", "Путин", "fuck" });
                    LB_BannedStrings.ItemsSource = BannedStrings;
                }
                OnPropertyChanged();
            }

        }

        private string reportFolderPath;
        private int totalFileCount;
        private int analyzedFileCount;
        private ObservableCollection<string> bannedStrings;

        private const bool DEBUG = true;

        public bool AnalyzersLoaded { get => analyzersLoaded;
            private set { analyzersLoaded = value; OnPropertyChanged(); } }
        public MainWindow()
        {
            InitializeComponent();
            TotalFileCount = 0;
            AnalyzedFileCount = 0;

            DataContext = this;
        }



        private void BTN_PauseOrResumeAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Analyzer analyzer = (Analyzer)button.Tag;

            if (analyzer.State != AnalyzerState.Running)
            {
                if (analyzer.BannedStrings == null)
                {
                    MessageBox.Show("Перед началом анализа загрузите список запрещённых слов.");
                    return;
                }
                analyzer.Start();
            }
            else
                analyzer.Pause();

        }

        private void BTN_StopAnalyzer_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Analyzer analyzer = (Analyzer)button.Tag;
            analyzer.Stop();
        }

        private string curFilePath = null;
        private ObservableCollection<Analyzer> analyzers;
        private bool analyzersLoaded;

        public string CurFilePath
        {
            get => curFilePath;
            private set
            {
                curFilePath = value; OnPropertyChanged();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            if (item.Name.Equals("OpenMenu"))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "WordFilter Config Files|*.wfc";
                dialog.Title = "Открыть файл конфигурации...";
                dialog.InitialDirectory = CurFilePath ?? AppDomain.CurrentDomain.BaseDirectory;
                dialog.Multiselect=false;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadWfc(dialog.FileName);
                }
            }
        }
        public void LoadWfc(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string text = reader.ReadToEnd().Trim();
                BannedStrings = new ObservableCollection<string>(text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                LB_BannedStrings.ItemsSource = BannedStrings;
                CurFilePath = path;
            }
        }
        private void BTN_SelectFolderForReport_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = false;
                dialog.InitialDirectory = Environment.CurrentDirectory;
                dialog.Title = "Выбрать папку для отчёта...";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ReportFolderPath = dialog.FileName;
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void StartAllAnalyzersBTN(object sender, RoutedEventArgs e)
        {
            foreach (var a in Analyzers)
                a.Start();
        }

        public void StopAllAnalyzersBTN(object sender, RoutedEventArgs e)
        {

            foreach (var a in Analyzers)
                a.Stop();
        }

        public void PauseAllAnalyzers(object sender, RoutedEventArgs e)
        {

            foreach (var a in Analyzers)
                a.Pause();
        }
    }
}
