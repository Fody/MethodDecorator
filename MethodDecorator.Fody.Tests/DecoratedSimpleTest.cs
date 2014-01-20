using System.Reflection;

namespace MethodDecoratorEx.Fody.Tests {
    public class DecoratedSimpleTest {
        public DecoratedSimpleTest() {
            var weaverHelper = new WeaverHelper(@"SimpleTest\SimpleTest.csproj");
            this.Assembly = weaverHelper.Weave();
        }

        public Assembly Assembly { get; set; }
    }
}