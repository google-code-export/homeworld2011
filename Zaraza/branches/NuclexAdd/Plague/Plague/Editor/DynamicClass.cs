using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace PlagueEngine.Editor
{
    public class DynamicClass
    {
        public class DynaClassInfo
        {
            public Type type;
            public Object ClassObject;

            public DynaClassInfo()
            {
            }

            public DynaClassInfo(Type t, Object c)
            {
                type = t;
                ClassObject = c;
            }
        }

        public static Dictionary<Type, DynaClassInfo> ClassReferences = new Dictionary<Type, DynaClassInfo>();

        public static DynaClassInfo GetClassReference(string NameSpace, string ClassName)
        {
            Type classType = Type.GetType(NameSpace + "." + ClassName);
            if (classType.IsClass == true)
            {
                if (!ClassReferences.ContainsKey(classType))
                {
                    DynaClassInfo ci = new DynaClassInfo(classType, Activator.CreateInstance(classType));
                    ClassReferences.Add(classType, ci);
                    return (ci);
                }
                return ClassReferences[classType];
            }
            return null;
        }

        public static Object InvokeMethod(DynaClassInfo ci, string MethodName, Object[] args)
        {
            Object Result = ci.type.InvokeMember(MethodName,
                BindingFlags.Default | BindingFlags.InvokeMethod,
                   null,
                   ci.ClassObject,
                   args);
            return Result;
        }

        public static Object InvokeMethod(string NameSpace, string ClassName, string MethodName, Object[] args)
        {
            DynaClassInfo ci = GetClassReference(NameSpace, ClassName);
            return ci != null ? (InvokeMethod(ci, MethodName, args)) : null;

        }
    }
}
