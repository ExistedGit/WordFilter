using System.Windows;

namespace WordFilter
{

    public partial class MainWindow : Window
    {
        public int NumberOfFilesToAnalysis { get; set; } = 0;
        public int NumberOfCurrentProgress { get; set; } = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
