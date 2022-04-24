using System.Collections.Generic;
using System.Windows;
using WordFilter.Entities;

namespace WordFilter
{

    public partial class MainWindow : Window
    {
        public int TotalFileCount { get; set; } = 0;
        public int AnalyzedFileCount { get; set; } =0;


        public List<Analyzer> Analyzers { get; set; } = new List<Analyzer>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
