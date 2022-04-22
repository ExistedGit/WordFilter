using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace WordFilter.Entities
{
    public class Analyzer : INotifyPropertyChanged
    {
        private string path { get; set; }
        private List<string> FilesToAnalysis { get; set; } = new List<string>();

        private int numberOfFilesToAnalysis;

        private int numberOfCurrentProgress;
        public List<string> BannedStrings { get; set; }


        public int NumberOfFilesToAnalysis
        {
            get { return numberOfFilesToAnalysis; }
            set
            {
                numberOfFilesToAnalysis = value;
                OnPropertyChanged();
            }

        }

        public int NumberOfCurrentProgress
        {
            get { return numberOfCurrentProgress; }
            set
            {
                numberOfCurrentProgress = value;
                OnPropertyChanged();
            }
        }

        public string RootName {
            get { return Path.GetDirectoryName(path); }

        }


        public Analyzer(string path)
        {
            if (Directory.Exists(path))
            {
                this.path = path;
                NumberOfFilesToAnalysis = 0;
                NumberOfCurrentProgress = 0;
            }
            else
                throw new ArgumentOutOfRangeException("path");
        }


        public void ReadAllTxtFiles(string dir = null) {

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
                FilesToAnalysis.Add(item);
            }


        }

        public void StartAnalysisFiles()
        {
            NumberOfFilesToAnalysis = FilesToAnalysis.Count;
            //TODO




        }

        public Analyzer SetBannedStrings(IEnumerable<string> BannedStrings)
        {
            this.BannedStrings = (List<string>)BannedStrings;
            return this;
        }


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
