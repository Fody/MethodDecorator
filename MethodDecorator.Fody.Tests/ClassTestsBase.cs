using MethodDecorator.Fody.Tests;

namespace MethodDecoratorEx.Fody.Tests {
    public abstract class ClassTestsBase<T> : TestsBase<T>
        where T : DecoratedSimpleTest, new() {
        protected dynamic TestClass { get; private set; }
        private readonly string _className;
        protected ClassTestsBase(string className) {
            this._className = className;
        }

        public override void SetFixture(T data) {
            base.SetFixture(data);
            this.TestClass = this.Assembly.GetInstance(this._className);
        }
    }
}