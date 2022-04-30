using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WordFilter.Entities
{
    public partial class Analyzer : INotifyPropertyChanged
    {

        public event Action<Analyzer> FilesCounted;
        public event Action<Analyzer> Completed;

        private int totalFileCount;
        private int analyzedFileCount;
        private AnalyzerState state;
        /// <summary>
        /// Список запрещённых строк
        /// </summary>
        public List<string> BannedStrings { get; private set; }

        private List<FileReport> fileReports = new List<FileReport>();
        /// <summary>
        /// Отчёты о проанализированных файлах(какие слова и сколько их найдено)
        /// </summary>
        public FileReport[] FileReports => fileReports.ToArray();
        /// <summary>
        /// Главный поток, в котором выполняется анализ
        /// </summary>
        private Thread thread = null;
        private Task taskFileCount = null;
        private ManualResetEvent mre = new ManualResetEvent(false);
        private bool ready;

        /// <summary>
        /// Путь к директории, которую необходимо проанализировать
        /// </summary>
        public string path { get; private set; }
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
                if (analyzedFileCount == TotalFileCount)
                {
                    State = AnalyzerState.Completed;
                    Completed?.Invoke(this);
                }
                OnPropertyChanged();
            }
        }
        public string Root => path;

        public AnalyzerState State
        {
            get => state;
            private set
            {
                state = value;
                OnPropertyChanged();
            }
        }

        public Analyzer(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentOutOfRangeException("ANALYZER PATH WRONG");

            State = AnalyzerState.Stopped;
            this.path = path;
            TotalFileCount = 0;
            AnalyzedFileCount = 0;
        }
        public void CountFilesAsync()
        {
            taskFileCount = Task.Factory.StartNew(() => CountFiles(path));
        }
        /// <summary>
        /// Начать работу анализатора
        /// </summary>
        /// <returns>Удалось ли выполнить метод</returns>
        public bool Start()
        {
            if (State == AnalyzerState.Running)
                return false;
            if (State == AnalyzerState.Stopped
                || 
                State == AnalyzerState.Completed)
            {
                fileReports.Clear();
                AnalyzedFileCount = 0;
                thread = new Thread(new ThreadStart(() => AnalyzeDirectory()));
                thread.IsBackground = true;
                thread.Start();
            }
            State = AnalyzerState.Running;
            mre.Set();
            return true;
        }
        /// <summary>
        /// Приостановить работу анализатора
        /// </summary>
        /// <returns>Удалось ли выполнить метод</returns>
        public bool Pause()
        {
            if (State != AnalyzerState.Running)
                return false;
            State = AnalyzerState.Paused;
            mre.Reset();
            return false;
        }
        /// <summary>
        /// Полностью остановить работу анализатора
        /// </summary>
        /// <returns>Удалось ли выполнить метод</returns>
        public bool Stop()
        {
            if (State == AnalyzerState.Stopped
                ||
                State ==AnalyzerState.Completed)
                return false;
            mre.Reset();
            thread.Abort();
            State = AnalyzerState.Stopped;
            return true;
        }
        public bool Ready { get => ready; private set
            {
                ready = value;
                OnPropertyChanged();    
            }
        }

        /// <summary>
        /// Считает файлы в папке
        /// </summary>
        /// <param name="obj">Путь к папке</param>
        /// <param name="level">Уровень вложения</param>
        private void CountFiles(object obj, int level = 0)
        {
            string dir = obj as string;
            string[] Catalogs = null;
            try
            {
                Catalogs = Directory.GetDirectories(dir);
            }
            catch (Exception)
            {
                return;
            }



            try
            {
                TotalFileCount += Directory.GetFiles(dir, "*.txt").Count();
            }
            catch (Exception)
            {

            }
            foreach (var catalog in Catalogs)
                CountFiles(catalog, level + 1);
            if (level == 0)
            {
                Ready = true;
                FilesCounted?.Invoke(this);
            }
        }
        /// <summary>
        /// Рекурсивно анализирует папку
        /// </summary>
        /// <param name="obj">Строка с путём к папке(object из-за потоков)</param>
        /// <param name="level">Уровень вложенности, от которого зависит создание новых потоков</param>
        private void AnalyzeDirectory(object obj = null, int level = 0)
        {
            if (!Ready)
                CountFiles(path);
            string dir = obj as string;

            if (dir == null)
            {
                dir = path;
            }

            string[] Catalogs = null;
            string[] files = null;
            try
            {
                Catalogs = Directory.GetDirectories(dir);
                files = Directory.GetFiles(dir, "*.txt");
            }
            catch (Exception)
            {
                return;
            }
            foreach (var item in files)
            {
                mre.WaitOne();
                fileReports.Add(AnalyzeFile(item));
                AnalyzedFileCount++;
                Console.WriteLine(AnalyzedFileCount);
            }

            List<Task> subTasks = new List<Task>();
            // Если уровень вложения больше двух, новый поток на папку не создаётся.
            foreach (var catalog in Catalogs)
                if (Directory.Exists(catalog))
                {
                    if (level <= 2)
                        subTasks.Add(Task.Factory.StartNew(() => AnalyzeDirectory(catalog, level + 1)));
                    else
                        AnalyzeDirectory(catalog, level + 1);
                }
        }

        private FileReport AnalyzeFile(string path)
        {
            FileReport result = new FileReport(path);

            foreach (var bannedString in BannedStrings)
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string text = reader.ReadToEnd().Trim();
                    result.WordOccurences[bannedString] = Regex.Matches(text, $@"\b{bannedString}\b").Count;
                }
            }
            return result;
        }

        public Analyzer SetBannedStrings(IEnumerable<string> strings)
        {
            BannedStrings = strings.ToList();
            return this;
        }


        private void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
