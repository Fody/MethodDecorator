namespace MethodDecoratorEx.Fody.Tests
{
    using global::MethodDecorator.Fody.Tests;

    public class ClassTestsBase : SimpleTestBase
    {
        private readonly string _className;

        protected ClassTestsBase(string className)
        {
            this._className = className;
        }

        protected dynamic TestClass
        {
            get { return Assembly.GetInstance(this._className); }
        }
    }
}