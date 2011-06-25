using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlagueEngine.Editor
{
    class GameObjectClassName
    {
        public string ClassName;
        public Type ClassType;
        public Type DataClassType;
        public Type[] Interfaces;
        public Type BaseType;

        public GameObjectClassName()
        { }
        public GameObjectClassName(string className, Type classType, Type dataClassType)
        {
            ClassName = className;
            ClassType = classType;
            Interfaces = classType.GetInterfaces();
            BaseType = classType.BaseType;
            DataClassType = dataClassType;
        }
    }
}
