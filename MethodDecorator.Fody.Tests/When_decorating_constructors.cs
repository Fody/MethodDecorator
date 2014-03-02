using System;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_constructors : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testMessages;

        [Fact]
        public void Should_report_on_entry_and_exit()
        {
            var testClass = assembly.GetInstance("SimpleTest.InterceptingConstructors+SimpleConstructor");

            Assert.NotNull(testClass);

            Assert.Contains("OnEntry: SimpleTest.InterceptingConstructors+SimpleConstructor..ctor", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingConstructors+SimpleConstructor..ctor", testMessages.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_exception()
        {
            var exception = Record.Exception(() => assembly.GetInstance("SimpleTest.InterceptingConstructors+ThrowingConstructor"));

            // This is because we're using reflection to create the instance.
            // It will wrap any exception
            if (exception is TargetInvocationException)
                exception = exception.InnerException;

            Assert.IsType<InvalidOperationException>(exception);

            Assert.Contains("OnEntry: SimpleTest.InterceptingConstructors+ThrowingConstructor..ctor", testMessages.Messages);
            Assert.Contains("OnException: SimpleTest.InterceptingConstructors+ThrowingConstructor..ctor - System.InvalidOperationException: Ooops", testMessages.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        }
    }
}