using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WordFilter.Entities;

namespace WordFilter
{

    public partial class MainWindow : Window
    {
        public int TotalFileCount { get; set; } = 0;
        public int AnalyzedFileCount { get; set; } = 0;

        public ObservableCollection<Analyzer> Analyzers { get; set; }


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
    }
}
