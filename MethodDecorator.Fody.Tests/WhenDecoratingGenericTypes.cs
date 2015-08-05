namespace MethodDecoratorEx.Fody.Tests
{
    using Xunit;

    public class WhenDecoratingGenericTypes : ClassTestsBase
    {
        public WhenDecoratingGenericTypes() : base("SimpleTest.GenericType`1[[System.String, mscorlib]]")
        {
        }

        [Fact]
        public void ShouldCaptureOnEntryAndExit()
        {
            const string expected = "Hello world";
            dynamic value = TestClass.GetValue(expected);
            Assert.Equal(expected, value);

            CheckInit("SimpleTest.GenericType`1[System.String]", "SimpleTest.GenericType`1.GetValue", 1);
            CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.OnExit});
        }
    }
}