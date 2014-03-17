using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratingGenericTypes : ClassTestsBase<DecoratedSimpleTest> {
        public WhenDecoratingGenericTypes() : base("SimpleTest.GenericType`1[[System.String, mscorlib]]") { }

        [Fact]
        public void ShouldCaptureOnEntryAndExit() {
            const string expected = "Hello world";
            dynamic value = this.TestClass.GetValue(expected);
            Assert.Equal(expected, value);

            this.CheckInit("SimpleTest.GenericType`1[System.String]", "SimpleTest.GenericType`1.GetValue", 1);
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
        }
    }
}