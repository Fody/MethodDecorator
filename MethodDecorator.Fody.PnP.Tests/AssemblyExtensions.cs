using System;
using System.Reflection;

namespace MethodDecorator.Fody.Tests {
    public static class AssemblyExtensions {
        public static dynamic GetInstance(this Assembly assembly, string className) {
            Type type = assembly.GetType(className, true);
            //dynamic instance = FormatterServices.GetUninitializedObject(type);
            return Activator.CreateInstance(type);
        }

        public static dynamic GetStaticInstance(this Assembly assembly, string className) {
            Type type = assembly.GetType(className, true);
            return new StaticMembersDynamicWrapper(type);
        }
    }
}