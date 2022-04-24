using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WordFilter.Entities
{
    /// <summary>
    /// Вспомогательный класс для XML-сериализации словаря <c>FileReport</c>. 
    /// Каждый объект представляет из себя пару слово-количество(string, int).
    /// </summary>
    [Serializable]
    public class WordCounter
    {
        [XmlAttribute]
        public string Word { get; set; }

        [XmlAttribute]
        public int Occurences { get; set; }

        /// <summary>
        /// Для XML-сериализации
        /// </summary>
        public WordCounter() { }

        public WordCounter(string word, int occurences)
        {
            Word = word;
            Occurences = occurences;
        }
    }
}
