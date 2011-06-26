using System;

namespace PlagueEngine.Editor
{
    class GameObjectClassName
    {
        public string ClassName;
        public Type ClassType;
        public Type DataClassType;
        public Type[] Interfaces;
        public Type BaseType;
        public bool CanBeCreated;

        public GameObjectClassName()
        { }
        public GameObjectClassName(string className, Type classType, Type dataClassType, bool canBeCreated)
        {
            ClassName = className;
            ClassType = classType;
            Interfaces = classType.GetInterfaces();
            BaseType = classType.BaseType;
            DataClassType = dataClassType;
            CanBeCreated = canBeCreated;
        }
    }
}
