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

        private List<string> FilesToAnalysis = new List<string>();

        public int NumberOfFilesToAnalysis { get; set; } = 0;
        public int NumberOfCurrentProgress { get; set; } = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public string RootName {
            get { return Path.GetDirectoryName(path); }

        }





        public Analyzer(string path)
        {
            if (Directory.Exists(path))
                this.path = path;
            else throw new ArgumentOutOfRangeException("path");
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



        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
