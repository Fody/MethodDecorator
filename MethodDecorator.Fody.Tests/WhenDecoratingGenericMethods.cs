namespace MethodDecoratorEx.Fody.Tests
{
    using Xunit;

    public class WhenDecoratingGenericMethods : ClassTestsBase
    {
        public WhenDecoratingGenericMethods() : base("SimpleTest.GenericMethod")
        {
        }

        [Fact]
        public void ShouldCaptureOnEntryAndExit()
        {
            const string expected = "Hello world";
            dynamic value = TestClass.GetValue<string>(expected);
            Assert.Equal(expected, value);

            CheckInit("SimpleTest.GenericMethod", "SimpleTest.GenericMethod.GetValue", 1);
            CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.OnExit});
        }

        [Fact]
        public void ShouldCaptureOnEntryAndExitWhenParameterValueType()
        {
            const int expected = 42;
            dynamic value = TestClass.GetValue<int>(expected);
            Assert.Equal(expected, value);

            CheckInit("SimpleTest.GenericMethod", "SimpleTest.GenericMethod.GetValue", 1);
            CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.OnExit});
        }
    }
}