namespace MethodDecoratorEx.Fody.Tests
{
    using System;
    using System.Reflection;
    using global::MethodDecorator.Fody.Tests;
    using Xunit;

    public class DecoratingConstructors : SimpleTestBase
    {
        [Fact]
        public void ShouldReportOnEntryAndExit()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.InterceptingConstructors+SimpleConstructor");
            Assert.NotNull(testClass);
            CheckInit(null, "SimpleTest.InterceptingConstructors+SimpleConstructor..ctor", 0);
            CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.OnExit});
        }

        [Fact]
        public void ShouldReportOnEntryAndException()
        {
            var exception =
                Record.Exception(
                    () => Assembly.GetInstance("SimpleTest.InterceptingConstructors+ThrowingConstructor"));

            // This is because we're using reflection to create the instance.
            // It will wrap any exception
            if (exception is TargetInvocationException)
                exception = exception.InnerException;

            Assert.IsType<InvalidOperationException>(exception);

            CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnException});
            CheckInit(null, "SimpleTest.InterceptingConstructors+ThrowingConstructor..ctor", 0);
            CheckException<InvalidOperationException>("Ooops");
        }
    }
}