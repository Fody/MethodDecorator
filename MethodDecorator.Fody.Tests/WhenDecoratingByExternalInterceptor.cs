using System.Reflection;

using MethodDecorator.Fody.Tests;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class WhenDecoratingByExternalInterceptor : SimpleTestBase {

        private static readonly Assembly _externalAssembly;

        static WhenDecoratingByExternalInterceptor() {
            var path = _assembly.Location.Replace("SimpleTest2.dll", "AnotherAssemblyAttributeContainer.dll");
            _externalAssembly = Assembly.LoadFile(path);
        }

        public WhenDecoratingByExternalInterceptor() {
            _assembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
            _externalAssembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
            this.TestClass = _assembly.GetInstance("SimpleTest.MarkedFromAnotherAssembly");
        }

        public dynamic TestClass { get; set; }
        protected override dynamic RecordHost
        {
            get { return _externalAssembly.GetStaticInstance("SimpleTest.TestRecords"); }
        }

        [Fact]
        public void ShouldNotifyOnInitModuleRegistered() {
            this.TestClass.ExternalInterceptorDecorated();
            this.CheckInit("SimpleTest.MarkedFromAnotherAssembly", "SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorDecorated");
        }

        [Fact]
        public void ShouldNotifyOnInitAssemblyRegistered() {
            this.TestClass.ExternalInterceptorAssemblyLevelDecorated();
            this.CheckInit("SimpleTest.MarkedFromAnotherAssembly", "SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorAssemblyLevelDecorated");
        }

    }
}