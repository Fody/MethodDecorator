using System;

namespace MethodDecoratorEx.Fody.Tests {
    public abstract class SimpleTestBase : ClassTestsBase<DecoratedSimpleTest>
    {
        protected SimpleTestBase(string className) : base(className) { }
        protected SimpleTestBase() : base(String.Empty) { }
    }
}