using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WordFilter
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

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
            //args = new string[] { @"B:\config.wfc", @"G:\VS Repo\WordFilter\bin\Debug\reports" };
            if (args.Length == 0)
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);
                wnd.Show();
            }
            else if (args.Length == 2)
            {
                //MessageBox.Show($"{args[0]}\n{args[1]}");
                string wfcPath = args[0], reportFolder = args[1];

                if (!File.Exists(wfcPath))
                    Environment.Exit(0);
                if (Directory.Exists(reportFolder))
                    Directory.Delete(reportFolder, true);
                Directory.CreateDirectory(reportFolder);

                Console.CursorVisible = false;

                wnd.LoadWfc(wfcPath);
                Console.WriteLine("Запрещённые слова: " + string.Join(", ", wnd.BannedStrings));
                wnd.ReportFolderPath = reportFolder;
                wnd.PropertyChanged += Wnd_PropertyChanged;
                wnd.SilentAllFilesCounted += Wnd_SilentAllFilesCounted;
                wnd.CreateAnalyzers();
            }

        }

        private static void Wnd_SilentAllFilesCounted(MainWindow wnd)
        {
            wnd.StartAllAnalyzersBTN(null, null);
        }

        private static string GenerateStatistics(MainWindow wnd)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var a in wnd.Analyzers)
                sb.AppendLine(a.Path + ": " + (a.AnalyzedFileCount / (float)a.TotalFileCount * 100).ToString("0.00") + "%");
            sb.Append("Total: " + (wnd.AnalyzedFileCount / (float)wnd.TotalFileCount * 100).ToString("0.00") + "%");

            return sb.ToString();
        }
        public static object sync = new object();
        private static void Wnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MainWindow wnd = sender as MainWindow;
            if (e.PropertyName == "AnalyzedFileCount")
            {
                lock (sync)
                {
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine(GenerateStatistics(wnd));
                }

                if (wnd.AnalyzedFileCount == wnd.TotalFileCount)
                    Environment.Exit(0);
            }
        }
    }
}
