using System;
using System.Reflection;

namespace MethodDecorator.Fody.Tests
{
    public class DecoratedSimpleTest
    {
        public DecoratedSimpleTest()
        {
            var weaverHelper = new WeaverHelper(@"SimpleTest\SimpleTest.csproj");
            Assembly = weaverHelper.Weave();
        }

        public Assembly Assembly { get; private set; }
    }

    public static class AssemblyExtensions
    {
        public static dynamic GetInstance(this Assembly assembly, string className)
        {
            var type = assembly.GetType(className, true);
            //dynamic instance = FormatterServices.GetUninitializedObject(type);
            return Activator.CreateInstance(type);
        }
    }
}