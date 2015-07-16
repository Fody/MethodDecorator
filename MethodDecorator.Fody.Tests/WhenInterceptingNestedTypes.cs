namespace MethodDecoratorEx.Fody.Tests
{
    using global::MethodDecorator.Fody.Tests;
    using Xunit;

    public class WhenInterceptingNestedTypes : SimpleTestBase
    {
        [Fact]
        public void ShouldDecorateMethodInNestedType()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.InterceptingNestedTypes+Nested");
            dynamic value = testClass.StringMethod();

            Assert.Equal("sausages", value);

            CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnExit});
            CheckInit("SimpleTest.InterceptingNestedTypes+Nested",
                "SimpleTest.InterceptingNestedTypes+Nested.StringMethod");
        }

        [Fact]
        public void ShouldDecorateADeeplyNestedType()
        {
            dynamic testClass =
                Assembly.GetInstance("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested");
            dynamic value = testClass.NumberMethod();

            Assert.Equal(42, value);

            CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnExit});
            CheckInit("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested",
                "SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested.NumberMethod");
        }
    }
}