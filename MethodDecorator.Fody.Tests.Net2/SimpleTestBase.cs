using System.Reflection;

using MethodDecorator.Fody.Tests;

namespace MethodDecoratorEx.Fody.Tests
{
    public class SimpleTestBase : TestsBase {
        private static readonly Assembly _assembly = CreateAssembly();

        public SimpleTestBase() {
            _assembly.GetStaticInstance("SimpleTest.Net2.TestRecords").Clear();
        }

        protected override Assembly Assembly {
            get { return _assembly; }
        }

        protected override dynamic RecordHost {
            get { return this.Assembly.GetStaticInstance("SimpleTest.Net2.TestRecords"); }
        }

        private static Assembly CreateAssembly() {
            var weaverHelper = new WeaverHelper(@"SimpleTest.Net2\SimpleTest.Net2.csproj");
            return weaverHelper.Weave();
        }
    }
}