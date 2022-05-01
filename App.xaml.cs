using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WordFilter
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public enum WFERR
        {
            Success = 0,
            InvalidArgCount = -1,
            MissingDirectory = 1
        }
        // wordfilter.exe [WFC FILE] [OUTPUT FOLDER]
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            string[] args = e.Args;
            //args = new string[]{ "B:\\config.wfc", @"B:\Reports\"};
            if (args.Length == 0)
            {
                wnd.Show();
            }
            else if (args.Length == 2)
            {
                //MessageBox.Show($"{args[0]}\n{args[1]}");
                string wfcPath = args[0], reportFolder = args[1];

                if (!File.Exists(wfcPath))
                    Environment.Exit(0);
                if(Directory.Exists(reportFolder))
                    Directory.Delete(reportFolder, true);
                Directory.CreateDirectory(reportFolder);

                wnd.LoadWfc(wfcPath);
                wnd.ReportFolderPath = reportFolder;
                wnd.CreateAnalyzers();
                wnd.SilentAllFilesCounted += Wnd_SilentAllFilesCounted;
                wnd.PropertyChanged += Wnd_PropertyChanged;
            }
            
        }

        private static void Wnd_SilentAllFilesCounted(MainWindow wnd)
        {
            wnd.StartAllAnalyzersBTN(null, null);
        }

        private static void Wnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MainWindow wnd = sender as MainWindow;
            if (e.PropertyName == "AnalyzedFileCount")
            {
                Trace.WriteLine((wnd.AnalyzedFileCount / (float)wnd.TotalFileCount * 100).ToString("0.00") + "%");
                if (wnd.AnalyzedFileCount == wnd.TotalFileCount)
                    Environment.Exit(0);
            }
        }
    }
}
