using MethodDecorator.Fody.Tests;

namespace MethodDecoratorEx.Fody.Tests {
    public abstract class ClassTestsBase<T> : TestsBase<T>
        where T : DecoratedSimpleTest, new() {
        protected ClassTestsBase(string className) {
            this._className = className;
            this.TestClass = this.Assembly.GetInstance(this._className);
        }
        
        private readonly string _className;

        protected dynamic TestClass { get; private set; }
    }
}