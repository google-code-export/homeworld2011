using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.Editor
{
    class DefinitionCounter
    {
        private int _count;
        private string _levelName;
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }
        public string LevelName
        {
            get { return _levelName; }
            set { _levelName = value; }
        }
        public DefinitionCounter(string levelName)
        {
            _levelName = levelName;
        }
        public void Add()
        {
            _count++;
        }
    }
}
