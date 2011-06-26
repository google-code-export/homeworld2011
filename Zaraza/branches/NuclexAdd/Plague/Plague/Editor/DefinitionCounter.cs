namespace PlagueEngine.Editor
{
    class DefinitionCounter
    {
        public int Count { get; set; }

        public string LevelName { get; set; }

        public DefinitionCounter(string levelName)
        {
            LevelName = levelName;
        }
        public void Add()
        {
            Count++;
        }
    }
}
