using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_generic_types : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        [Fact]
        public void Should_capture_on_entry_and_exit()
        {
            const string expected = "Hello world";
            var value = testClass.GetValue(expected);

            Assert.Equal(expected, value);

            Assert.Contains("OnEntry: SimpleTest.GenericType`1.GetValue", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.GenericType`1.GetValue", testMessages.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testClass = assembly.GetInstance("SimpleTest.GenericType`1[[System.String, mscorlib]]");
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        }
    }
}