using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueLocalizationExtension
{
    [Serializable]
    public class LangFile
    {
        public string language;
        public double version;
        public string author;
        public string package;
        public DateTime revision;
        public Dictionary<string, string> keys;

        public LangFile()
        {
            keys = new Dictionary<string, string>();
        }
    }
}
