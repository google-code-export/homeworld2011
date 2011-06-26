using System;
using System.Collections.Generic;
using System.Reflection;

namespace PlagueEngine.Editor
{
    public class DynamicClass
    {
        public static Dictionary<Type, DynaClassInfo> ClassReferences = new Dictionary<Type, DynaClassInfo>();

        public class DynaClassInfo
        {
            public Type Type;
            public Object ClassObject;

            public DynaClassInfo()
            {
            }

            public DynaClassInfo(Type t, Object c)
            {
                Type = t;
                ClassObject = c;
            }
        }

        public static DynaClassInfo GetClassReference(string nameSpace, string className)
        {
            var classType = Type.GetType(nameSpace + "." + className);
            if (classType != null && classType.IsClass)
            {
                if (!ClassReferences.ContainsKey(classType))
                {
                    var ci = new DynaClassInfo(classType, Activator.CreateInstance(classType));
                    ClassReferences.Add(classType, ci);
                    return (ci);
                }
                return ClassReferences[classType];
            }
            return null;
        }

        public static Object InvokeMethod(DynaClassInfo ci, string methodName, Object[] args)
        {
            var result = ci.Type.InvokeMember(methodName,
                BindingFlags.Default | BindingFlags.InvokeMethod,
                   null,
                   ci.ClassObject,
                   args);
            return result;
        }

        public static Object InvokeMethod(string nameSpace, string className, string methodName, Object[] args)
        {
            var ci = GetClassReference(nameSpace, className);
            return ci != null ? (InvokeMethod(ci, methodName, args)) : null;

        }
    }
}
