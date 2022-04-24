using System.Collections.Generic;
using System.Windows;
using WordFilter.Entities;

namespace WordFilter
{

    public partial class MainWindow : Window
    {
        public int NumberOfFilesToAnalysis { get; set; } = 0;
        public int NumberOfCurrentProgress { get; set; } = 0;
        public List<Analyzer> Analyzers { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
