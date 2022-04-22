using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WordFilter.Entities
{
    [Serializable]
    public class WordCounter
    {
        [XmlAttribute]
        public string Word { get; set; }
        [XmlAttribute]
        public int Occurences { get; set; }
        public WordCounter()
        {

        }
        public WordCounter(string word, int occurences)
        {
            Word = word;
            Occurences = occurences;
        }
    }
}
