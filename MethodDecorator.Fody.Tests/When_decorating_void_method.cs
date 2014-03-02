using System;
using System.Linq;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_void_method : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        [Fact]
        public void Should_notify_of_method_entry()
        {
            testClass.WithoutArgs();

            Assert.Contains("OnEntry: SimpleTest.InterceptingVoidMethods.WithoutArgs", testMessages.Messages);
        }

        [Fact]
        public void Should_notify_of_method_entry_and_exit()
        {
            testClass.WithoutArgs();

            Assert.Contains("OnEntry: SimpleTest.InterceptingVoidMethods.WithoutArgs", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingVoidMethods.WithoutArgs", testMessages.Messages);
        }

        [Fact]
        public void Should_call_method_body_between_enter_and_exit()
        {
            testClass.WithoutArgs();

            Assert.Equal("OnEntry: SimpleTest.InterceptingVoidMethods.WithoutArgs", testMessages.Messages[0]);
            Assert.Equal("VoidMethodWithoutArgs: Body", testMessages.Messages[1]);
            Assert.Equal("OnExit: SimpleTest.InterceptingVoidMethods.WithoutArgs", testMessages.Messages[2]);
        }

        [Fact]
        public void Should_notify_of_thrown_exception()
        {
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.ThrowingInvalidOperationException()));

            Assert.Contains("OnEntry: SimpleTest.InterceptingVoidMethods.ThrowingInvalidOperationException", testMessages.Messages);
            Assert.Contains("OnException: SimpleTest.InterceptingVoidMethods.ThrowingInvalidOperationException - System.InvalidOperationException: Ooops", testMessages.Messages);
        }

        [Fact]
        public void Should_not_notify_exit_when_method_throws()
        {
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.ThrowingInvalidOperationException()));

            Assert.DoesNotContain("OnExit: SimpleTest.InterceptingVoidMethods.WithoutArgs", testMessages.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_on_exit_with_conditional_throw()
        {
            testClass.ConditionallyThrowingInvalidOperationException(shouldThrow: false);

            Assert.Contains("OnEntry: SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", testMessages.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_on_exception_with_conditional_throw()
        {
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.ConditionallyThrowingInvalidOperationException(shouldThrow: true)));

            Assert.Contains("OnEntry: SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", testMessages.Messages);
            Assert.Equal("OnException: SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException - System.InvalidOperationException: Ooops", Enumerable.Last(testMessages.Messages));
        }

        // These should be a theory. Really need to sort out theory support in the reshaprer runner...
        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_1()
        {
            testClass.WithMultipleReturns(1);

            Assert.Equal("OnEntry: SimpleTest.InterceptingVoidMethods.WithMultipleReturns", testMessages.Messages[0]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", testMessages.Messages[1]);
            Assert.Equal("OnExit: SimpleTest.InterceptingVoidMethods.WithMultipleReturns", testMessages.Messages[2]);
        }

        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_2()
        {
            testClass.WithMultipleReturns(2);

            Assert.Equal("OnEntry: SimpleTest.InterceptingVoidMethods.WithMultipleReturns", testMessages.Messages[0]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", testMessages.Messages[1]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 1", testMessages.Messages[2]);
            Assert.Equal("OnExit: SimpleTest.InterceptingVoidMethods.WithMultipleReturns", testMessages.Messages[3]);
        }

        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_3()
        {
            testClass.WithMultipleReturns(3);

            Assert.Equal("OnEntry: SimpleTest.InterceptingVoidMethods.WithMultipleReturns", testMessages.Messages[0]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", testMessages.Messages[1]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 1", testMessages.Messages[2]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 2", testMessages.Messages[3]);
            Assert.Equal("OnExit: SimpleTest.InterceptingVoidMethods.WithMultipleReturns", testMessages.Messages[4]);
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_1()
        {
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.WithMultipleReturnsAndExceptions(1, shouldThrow: true)));

            Assert.Equal("OnEntry: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", testMessages.Messages[0]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 0", testMessages.Messages[1]);
            Assert.Equal("OnException: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions - System.InvalidOperationException: Throwing at 1", testMessages.Messages[2]);
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_2()
        {
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.WithMultipleReturnsAndExceptions(2, shouldThrow: true)));

            Assert.Equal("OnEntry: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", testMessages.Messages[0]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 0", testMessages.Messages[1]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 1", testMessages.Messages[2]);
            Assert.Equal("OnException: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions - System.InvalidOperationException: Throwing at 2", testMessages.Messages[3]);
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_3()
        {
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.WithMultipleReturnsAndExceptions(3, shouldThrow: true)));

            Assert.Equal("OnEntry: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", testMessages.Messages[0]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 0", testMessages.Messages[1]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 1", testMessages.Messages[2]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 2", testMessages.Messages[3]);
            Assert.Equal("OnException: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions - System.InvalidOperationException: Throwing at 3", testMessages.Messages[4]);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_method_with_multiple_returns_ending_with_throw()
        {
            testClass.MultipleReturnValuesButEndingWithThrow(2);

            Assert.Equal("OnEntry: SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow", testMessages.Messages[0]);
            Assert.Equal("MultipleReturnValuesButEndingWithThrow: Body - 0", testMessages.Messages[1]);
            Assert.Equal("MultipleReturnValuesButEndingWithThrow: Body - 1", testMessages.Messages[2]);
            Assert.Equal("OnExit: SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow", testMessages.Messages[3]);
        }

        [Fact]
        public void Should_report_exception_with_method_with_multiple_returns_ending_with_throw()
        {
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.MultipleReturnValuesButEndingWithThrow(0)));

            Assert.Contains("OnEntry: SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow", testMessages.Messages);
            Assert.Contains("OnException: SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow - System.InvalidOperationException: Ooops", testMessages.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testClass = assembly.GetInstance("SimpleTest.InterceptingVoidMethods");
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        }
    }
}