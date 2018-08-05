using MethodDecorator.Fody.Tests;

namespace MethodDecorator.Fody.Tests {
    public class ClassTestsBase : SimpleTestBase {
        protected ClassTestsBase(string className) {
            _className = className;
        }

        private readonly string _className;

        protected dynamic TestClass {
            get { return Assembly.GetInstance(_className); }
        }
    }
}