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
}