namespace MethodDecoratorEx.Fody.Tests
{
    using System.Reflection;
    using global::MethodDecorator.Fody.Tests;
    using Xunit;

    public class WhenDecoratingByExternalInterceptor : TestsBase
    {
        private static readonly Assembly _assembly;
        private static readonly Assembly _externalAssembly;

        static WhenDecoratingByExternalInterceptor()
        {
            _assembly = CreateAssembly();
            var path = _assembly.Location.Replace("SimpleTest2.dll", "AnotherAssemblyAttributeContainer.dll");
            _externalAssembly = Assembly.LoadFile(path);
        }

        public WhenDecoratingByExternalInterceptor()
        {
            _assembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
            _externalAssembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
            TestClass = _assembly.GetInstance("SimpleTest.MarkedFromAnotherAssembly");
        }

        public dynamic TestClass { get; set; }

        protected override Assembly Assembly
        {
            get { return _assembly; }
        }

        protected override dynamic RecordHost
        {
            get { return _externalAssembly.GetStaticInstance("SimpleTest.TestRecords"); }
        }

        [Fact]
        public void ShouldNotifyOnInitModuleRegistered()
        {
            TestClass.ExternalInterceptorDecorated();
            CheckInit("SimpleTest.MarkedFromAnotherAssembly",
                "SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorDecorated");
        }

        [Fact]
        public void ShouldNotifyOnInitAssemblyRegistered()
        {
            TestClass.ExternalInterceptorAssemblyLevelDecorated();
            CheckInit("SimpleTest.MarkedFromAnotherAssembly",
                "SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorAssemblyLevelDecorated");
        }

        private static Assembly CreateAssembly()
        {
            var weaverHelper = new WeaverHelper(@"SimpleTest\SimpleTest.csproj");
            return weaverHelper.Weave();
        }
    }
}