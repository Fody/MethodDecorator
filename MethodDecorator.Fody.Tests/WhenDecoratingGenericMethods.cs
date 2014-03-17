using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratingGenericMethods : ClassTestsBase<DecoratedSimpleTest> {
        public WhenDecoratingGenericMethods() : base("SimpleTest.GenericMethod") { }

        [Fact]
        public void ShouldCaptureOnEntryAndExit() {
            const string expected = "Hello world";
            dynamic value = this.TestClass.GetValue<string>(expected);
            Assert.Equal(expected, value);

            this.CheckInit("SimpleTest.GenericMethod", "SimpleTest.GenericMethod.GetValue", 1);
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldCaptureOnEntryAndExitWhenParameterValueType() {
            const int expected = 42;
            dynamic value = this.TestClass.GetValue<int>(expected);
            Assert.Equal(expected, value);

            this.CheckInit("SimpleTest.GenericMethod", "SimpleTest.GenericMethod.GetValue", 1);
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
        }
    }
}