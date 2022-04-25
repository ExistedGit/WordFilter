﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace WordFilter.Entities
{
    public class Analyzer : INotifyPropertyChanged
    {

        public string path { get; private set; }
        private int totalFileCount;
        private int analyzedFileCount;

        /// <summary>
        /// Список запрещённых строк
        /// </summary>
        public List<string> BannedStrings { get; private set; }
        
        private List<FileReport> fileReports = new List<FileReport>();
        /// <summary>
        /// Отчёты о проанализированных файлах(какие слова и сколько их найдено)
        /// </summary>
        public FileReport[] FileReports { get => fileReports.ToArray(); }
        /// <summary>
        /// Главный поток, в котором выполняется анализ
        /// </summary>
        private Thread thread = null;
        
        
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
                OnPropertyChanged();
            }
        }
        public string RootName {
            get => Path.GetDirectoryName(path); 
        }

        // *что-то на ломаном английском*
        public enum AnalyzerState
        {
            Running,
            Paused,
            Stopped
        }
        public AnalyzerState State { get; private set; } = AnalyzerState.Stopped;
        public Analyzer(string path)
        {
            if (!Directory.Exists(path))
                throw new ArgumentOutOfRangeException("path");

            this.path = path;
            TotalFileCount = 0;
            AnalyzedFileCount = 0;
        }
        /// <summary>
        /// Начать работу анализатора
        /// </summary>
        /// <returns>Удалось ли выполнить метод</returns>
        public bool Start()
        {
            if (State == AnalyzerState.Running)
                return false;
            if (State == AnalyzerState.Stopped)
            {
                thread = new Thread(new ThreadStart(()=> AnalyzeDirectory()));
                thread.IsBackground = true;
                thread.Start();
            }
            else if (State == AnalyzerState.Paused)
                thread.Resume();
            State = AnalyzerState.Running;
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
            thread.Suspend();
            State = AnalyzerState.Paused;
            return false;
        }
        /// <summary>
        /// Полностью остановить работу анализатора
        /// </summary>
        /// <returns>Удалось ли выполнить метод</returns>
        public bool Stop()
        {
            if (State == AnalyzerState.Stopped)
                return false;
            thread.Abort();

            State = AnalyzerState.Stopped;
            return true;
        }
        private void AnalyzeDirectory(object obj = null, int level = 0) 
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
            string[] files = Directory.GetFiles(dir, "*.txt");
            TotalFileCount += files.Length;


            foreach (var catalog in Catalogs)
                if (Directory.Exists(catalog)) {
                    if (level <= 2)
                    {
                        Thread subThread = new Thread(new ThreadStart(() => AnalyzeDirectory(catalog, level + 1)));
                        subThread.IsBackground = true;
                        subThread.Start();
                    }
                    else
                        AnalyzeDirectory(catalog, level + 1);
            }

            foreach (var item in files)
            {
                fileReports.Add(AnalyzeFile(item));
                AnalyzedFileCount++;
            }
        }

        private FileReport AnalyzeFile(string path)
        {
            FileReport result = new FileReport(path);
            
            foreach (var bannedString in BannedStrings)
            {
                using (StreamReader reader = new StreamReader(path)) {

                    string text = reader.ReadToEnd();
                    text= Regex.Replace(text, @"[\r\n]", " ");
                    string[] source = text.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    var matchQuery = from word in source
                                     where word.Equals(bannedString, StringComparison.InvariantCultureIgnoreCase)
                                     select word;
                    result.WordOccurences[bannedString] = matchQuery.Count(); 
                }
            }
            return result;
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
