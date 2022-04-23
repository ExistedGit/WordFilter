using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WordFilter.Entities
{
    public class Analyzer : INotifyPropertyChanged
    {
        private string path { get; set; }
        private List<string> Files { get; set; } = new List<string>();
        private int fileCount;
        private int analyzedFileCount;
        
        
        public List<string> BannedStrings { get; set; }
        public List<FileReport> FileReports { get; set; }
        
        private Thread thread = null;

        public int FileCount
        {
            get => fileCount;
            set
            {
                fileCount = value;
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
        public string RootName {
            get => Path.GetDirectoryName(path); 
        }

        public Analyzer(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentOutOfRangeException("path");

            this.path = path;
            FileCount = 0;
            AnalyzedFileCount = 0;

        }

        public enum AnalyzerState
        {
            RUNNING,
            PAUSED,
            STOPPED
        }
        public AnalyzerState State { get; private set; }
        public bool StartReading()
        {
            if (State == AnalyzerState.RUNNING)
                return false;
            if (State == AnalyzerState.STOPPED)
            {
                thread = new Thread(ReadAllTxtFiles);
                thread.IsBackground = true;
                thread.Start();
            }
            else if (State == AnalyzerState.PAUSED)
                thread.Resume();
            State = AnalyzerState.RUNNING;
            return true;
        }
        public bool PauseReading()
        {
            if (State != AnalyzerState.RUNNING)
                return false;
            thread.Suspend();
            State = AnalyzerState.PAUSED;
            return false; 
        }
        public bool StopReading()
        {
            if (State == AnalyzerState.STOPPED)
                return false;
            thread.Abort();
            State = AnalyzerState.STOPPED;
            return true;
        }
        private void ReadAllTxtFiles(object obj = null) 
        {
            string dir = obj as string;
            
            if(dir == null)
                dir = path;

            string[] Catalogs = null;
            try
            {
                Catalogs = Directory.GetDirectories(dir);
            }
            catch (Exception)  {
                return;
            }


            foreach (var catalog in Catalogs)
            {

                if (Directory.Exists(catalog))
                    ReadAllTxtFiles(catalog);
            }

            foreach (var item in Directory.GetFiles(dir, "*.txt"))
            {
                Console.WriteLine(item);
                Files.Add(item);
            }


        }

        public void StartAnalysisFiles()
        {
            FileCount = Files.Count;
            //TODO




        }

        public Analyzer SetBannedStrings(IEnumerable<string> strings)
        {
            BannedStrings = strings.ToList();
            return this;
        }


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
