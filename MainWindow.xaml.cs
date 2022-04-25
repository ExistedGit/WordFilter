using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WordFilter.Entities;

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
        public ObservableCollection<Analyzer> Analyzers { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private int totalFileCount;
        private int analyzedFileCount;

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
    }
}
