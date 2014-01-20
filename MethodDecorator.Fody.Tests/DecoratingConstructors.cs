using System;
using System.Reflection;

using MethodDecorator.Fody.Tests;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class DecoratingConstructors : TestsBase<DecoratedSimpleTest> {
        [Fact]
        public void ShouldReportOnEntryAndExit() {
            dynamic testClass = this.Assembly.GetInstance("SimpleTest.InterceptingConstructors+SimpleConstructor");

            Assert.NotNull(testClass);
            this.AssertMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
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

            this.AssertMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnException });
            this.CheckException<InvalidOperationException>("Ooops");
        }
    }
}