using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_extension_methods : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        [Fact]
        public void Should_intercept_extension_method()
        {
            var value = testClass.ReturnsString();

            Assert.Equal(3, testMessages.Messages.Count);
            Assert.Contains("OnEntry: SimpleTest.StringExtensions.ToTitleCase [1]", testMessages.Messages);
            Assert.Contains("ToTitleCase: In extension method", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.StringExtensions.ToTitleCase", testMessages.Messages);
            Assert.Equal("Hello World", value);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testClass = assembly.GetInstance("SimpleTest.InterceptingExtensionMethods");
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        } 
    }
}