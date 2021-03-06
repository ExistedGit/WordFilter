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
using Microsoft.Win32;
using System.Runtime.InteropServices;

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
        public bool FilesCounted
        {
            get => filesCounted;
            set
            {
                filesCounted = value;
                OnPropertyChanged();
            }
        }
        public bool ListLoaded
        {
            get => listLoaded;
            set
            {
                listLoaded = value;
                OnPropertyChanged();
            }
        }

        public bool ReportFolderSelected
        {
            get => reportFolderSelected;
            set { reportFolderSelected = value; OnPropertyChanged(); }
        }

        private bool reportFolderSelected = false;
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
                OnPropertyChanged();
                if (Analyzers != null)
                    foreach (var analyzer in Analyzers)
                        analyzer.SetBannedStrings(bannedStrings);
            }
        }
        public string ReportFolderPath
        {
            get => reportFolderPath;
            set
            {
                ReportFolderSelected = true;
                reportFolderPath = value;
                OnPropertyChanged();
            }

        }

        private string reportFolderPath;
        private int totalFileCount;
        private int analyzedFileCount;
        private ObservableCollection<string> bannedStrings;

        private const bool DEBUG = true;
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern void SHChangeNotify(
                                    int wEventId,
                                    uint uFlags,
                                    IntPtr dwItem1,
                                    IntPtr dwItem2);
        public MainWindow()
        {
            InitializeComponent();
            TotalFileCount = 0;
            AnalyzedFileCount = 0;

            DataContext = this;

            RegistryKey rk = Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.wfc\\Shell\\Open in WordFilter\\Command", true);

            if (rk == null)
            {
                rk = Registry.ClassesRoot.CreateSubKey("SystemFileAssociations\\.wfc\\Shell\\Open in WordFilter\\Command", true);
            }
            if (rk.GetValue("") == null)
                    rk.SetValue("", $"\"{Path.Combine(Environment.CurrentDirectory, "WordFilter.exe")}\" \"%1\"", RegistryValueKind.String);
            else
                if ((string)rk.GetValue("") != $"\"{Path.Combine(Environment.CurrentDirectory, "WordFilter.exe")}\" \"%1\"")
                    rk.SetValue("", $"\"{Path.Combine(Environment.CurrentDirectory, "WordFilter.exe")}\" \"%1\"", RegistryValueKind.String);
            rk.Close();
            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
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
        private bool filesCounted = false;
        private bool listLoaded = false;

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
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadWfc(dialog.FileName);
                    CreateAnalyzers();
                }
            } else if (item.Name.Equals("HelpMenu"))
            {
                MessageBox.Show("To start the analysis, select a report folder and open a .wfc file with a word list.\nTo create a word list, use WFCEditor(supplied with WordFilter).");
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
                ListLoaded = true;
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

        private void RescanBTN(object sender, RoutedEventArgs e)
        {
            foreach (var a in Analyzers)
                a.Stop();

            TotalFileCount = 0;
            AnalyzedFileCount = 0;


            CreateAnalyzers();
        }
    }
}
