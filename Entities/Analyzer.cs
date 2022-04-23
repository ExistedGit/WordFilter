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
        //TODO I think this enum must have more fields to display when read files and when analysing files
        public enum AnalyzerState
        {
            Running,
            Paused,
            Stoped
        }

        private string path { get; set; }
        private List<string> Files { get; set; } = new List<string>();
        private int fileCount;
        private int analyzedFileCount;

        public List<string> BannedStrings { get; set; }
        public List<FileReport> FileReports { get; set; }
        private Thread thread;
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
        public AnalyzerState State { get; private set; }


        public Analyzer(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentOutOfRangeException("path");

            this.path = path;
            FileCount = 0;
            AnalyzedFileCount = 0;

        }


        public bool StartReading()
        {
            if (State == AnalyzerState.Running)
                return false;

            if (State == AnalyzerState.Stoped)
            {
                thread = new Thread(ReadAllTxtFiles);
                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                if(State == AnalyzerState.Paused)
                    thread.Resume();
            }
              
            State = AnalyzerState.Running;
            return true;
        }

        public bool PauseReading()
        {
            if (State != AnalyzerState.Running)
                return false;

            thread.Suspend();

            State = AnalyzerState.Paused;
            return false; 
        }

        public bool StopReading()
        {
            if (State == AnalyzerState.Stoped)
                return false;

            thread.Abort();
            State = AnalyzerState.Stoped;
            return true;
        }

        private void ReadAllTxtFiles(object obj = null) 
        {
            if(obj is string)
            {
                string dir = (string)obj;

                if (dir == null)
                    dir = path;

                string[] Catalogs = null;
                try
                {
                    Catalogs = Directory.GetDirectories(dir);
                }
                catch (Exception)
                {
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


        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
