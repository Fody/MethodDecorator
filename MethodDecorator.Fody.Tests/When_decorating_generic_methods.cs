using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_generic_methods : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        [Fact]
        public void Should_capture_on_entry_and_exit()
        {
            const string expected = "Hello world";
            var value = testClass.GetValue<string>(expected);

            Assert.Equal(expected, value);

            Assert.Contains("OnEntry: SimpleTest.GenericMethod.GetValue [1]", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.GenericMethod.GetValue", testMessages.Messages);
        }

        [Fact]
        public void Should_capture_on_entry_and_exit_when_parameter_value_type() {
            const int expected = 42;
            var value = testClass.GetValue<int>(expected);

            Assert.Equal(expected, value);

            Assert.Contains("OnEntry: SimpleTest.GenericMethod.GetValue [1]", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.GenericMethod.GetValue", testMessages.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testClass = assembly.GetInstance("SimpleTest.GenericMethod");
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        }
    }
}