using System;
using System.Reflection;

using MethodDecorator.Fody.Tests;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class DecoratingConstructors : SimpleTestBase {
        [Fact]
        public void ShouldReportOnEntryAndExit() {
            dynamic testClass = Assembly.GetInstance("SimpleTest.InterceptingConstructors+SimpleConstructor");
            Assert.NotNull(testClass);
            this.CheckInit(null, "SimpleTest.InterceptingConstructors+SimpleConstructor..ctor", 0);
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldReportOnEntryAndException() {
            Exception exception =
                Record.Exception(
                    () => this.Assembly.GetInstance("SimpleTest.InterceptingConstructors+ThrowingConstructor"));

            // This is because we're using reflection to create the instance.
            // It will wrap any exception
            if (exception is TargetInvocationException)
                exception = exception.InnerException;

            Assert.IsType<InvalidOperationException>(exception);

            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnException });
            this.CheckInit(null, "SimpleTest.InterceptingConstructors+ThrowingConstructor..ctor", 0);
            this.CheckException<InvalidOperationException>("Ooops");
        }
    }
}