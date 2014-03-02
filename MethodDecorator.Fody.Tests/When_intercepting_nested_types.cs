using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_intercepting_nested_types : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testMessages;

        [Fact]
        public void Should_decorate_method_in_nested_type()
        {
            var testClass = assembly.GetInstance("SimpleTest.InterceptingNestedTypes+Nested");
            var value = testClass.StringMethod();

            Assert.Equal("sausages", value);

            Assert.Contains("OnEntry: SimpleTest.InterceptingNestedTypes+Nested.StringMethod", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingNestedTypes+Nested.StringMethod", testMessages.Messages);
        }

        [Fact]
        public void Should_decorate_a_deeply_nested_type()
        {
            var testClass = assembly.GetInstance("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested");
            var value = testClass.NumberMethod();

            Assert.Equal(42, value);

            Assert.Contains("OnEntry: SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested.NumberMethod", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested.NumberMethod", testMessages.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        }         
    }
}