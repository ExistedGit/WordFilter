using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace WordFilter.Entities
{
    [Serializable]

    [XmlRoot("report")]
    public class FileReport
    {

        [XmlAttribute("Path")]
        public string FullPath { get; set; }
        [XmlIgnore]
        public string Name 
        { 
            get => new FileInfo(FullPath).Name;
        }

        [XmlIgnore]
        public readonly Dictionary<string, int> WordOccurences = new Dictionary<string, int>();
        
        [XmlElement("WordOccurences")]
        public WordCounter[] WordOccurenceAttribute
        { 
            get => WordOccurences.Select(pair => new WordCounter(pair.Key, pair.Value)).ToArray();
        }    
        
        /// <summary>
        /// Для XML-сериализации.
        /// </summary>
        public FileReport() { }

        public FileReport(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentOutOfRangeException("path");
            FullPath = path;
        }

        /// <summary>
        /// Соединяет два словаря <c>FileReport</c> в один, суммируя количество у слов, найденных в обоих файлах.
        /// </summary>
        /// <param name="first">Первый словарь</param>
        /// <param name="second">Второй словарь</param>
        /// <returns>Соединённый словарь</returns>
        public static Dictionary<string, int> Merge(Dictionary<string, int> first, Dictionary<string, int> second)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            first.ToList().ForEach(x => result[x.Key] += x.Value);
            second.ToList().ForEach(x => result[x.Key] += x.Value);
            return result;
        }
    }
}
