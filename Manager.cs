using System;
using System.Collections.Generic;
using System.IO;

namespace FileManager
{
    public class Manager
    {
        private string path { get; set; }

        private List<string> Files = new List<string>();

        public string RootName {
            get { return Path.GetDirectoryName(path); }

        }

        public Manager(string path)
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
                Files.Add(item);
            }
        }
    }
}
