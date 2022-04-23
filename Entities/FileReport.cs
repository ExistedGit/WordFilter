using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace WordFilter.Entities
{
    [Serializable]
    public class FileReport
    {

        [XmlAttribute("Path")]
        public string FullPath { get; private set; }

        [XmlIgnore()]
        public string Name 
        { 
            get => new FileInfo(FullPath).Name;
        }

        public readonly Dictionary<string, int> WordOccurences = new Dictionary<string, int>();
        
        [XmlElement("WordOccurences")]
        public WordCounter[] WordOccurenceAttribute
        { 
            get => WordOccurences.Select(pair => new WordCounter(pair.Key, pair.Value)).ToArray();
        }    
        

        public FileReport() { }

        public FileReport(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentOutOfRangeException("path");

            this.FullPath = path;
        }

        public static Dictionary<string, int> Merge(Dictionary<string, int> first, Dictionary<string, int> second)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            first.ToList().ForEach(x => result[x.Key] += x.Value);
            second.ToList().ForEach(x => result[x.Key] += x.Value);
            return result;
        }
    }
}
