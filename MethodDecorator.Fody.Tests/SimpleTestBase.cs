using System.Reflection;

namespace MethodDecorator.Fody.Tests
{
    public class SimpleTestBase : TestsBase
    {
        protected static readonly Assembly _assembly = CreateAssembly();

        public SimpleTestBase()
        {
            _assembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
        }

        protected override Assembly Assembly
        {
            get { return _assembly; }
        }

        protected override dynamic RecordHost
        {
            get { return Assembly.GetStaticInstance("SimpleTest.TestRecords"); }
        }

        private static Assembly CreateAssembly()
        {
            var weaverHelper = new WeaverHelper(@"SimpleTest\SimpleTest.csproj");
            return weaverHelper.Weave();
        }
    }
}