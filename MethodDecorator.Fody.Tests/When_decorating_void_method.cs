using System;
using System.Linq;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_void_method : IUseFixture<DecoratedSimpleTest>
    {
        [Fact]
        public void Should_notify_of_method_entry()
        {
            var testClass = TestClass;
            testClass.WithoutArgs();

            Assert.Contains("OnEntry: SimpleTest.VoidMethods.WithoutArgs", testClass.Messages);
        }

        [Fact]
        public void Should_notify_of_method_entry_and_exit()
        {
            var testClass = TestClass;
            testClass.WithoutArgs();

            Assert.Contains("OnEntry: SimpleTest.VoidMethods.WithoutArgs", testClass.Messages);
            Assert.Contains("OnExit: SimpleTest.VoidMethods.WithoutArgs", testClass.Messages);
        }

        [Fact]
        public void Should_call_method_body_between_enter_and_exit()
        {
            var testClass = TestClass;
            testClass.WithoutArgs();

            Assert.Equal("OnEntry: SimpleTest.VoidMethods.WithoutArgs", testClass.Messages[0]);
            Assert.Equal("VoidMethodWithoutArgs: Body", testClass.Messages[1]);
            Assert.Equal("OnExit: SimpleTest.VoidMethods.WithoutArgs", testClass.Messages[2]);
        }

        [Fact]
        public void Should_notify_of_thrown_exception()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.ThrowingInvalidOperationException()));

            Assert.Contains("OnEntry: SimpleTest.VoidMethods.ThrowingInvalidOperationException", testClass.Messages);
            Assert.Contains("OnException: SimpleTest.VoidMethods.ThrowingInvalidOperationException - System.InvalidOperationException: Ooops", testClass.Messages);
        }

        [Fact]
        public void Should_not_notify_exit_when_method_throws()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.ThrowingInvalidOperationException()));

            Assert.DoesNotContain("OnExit: SimpleTest.VoidMethods.WithoutArgs", testClass.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_on_exit_with_conditional_throw()
        {
            var testClass = TestClass;
            testClass.ConditionallyThrowingInvalidOperationException(shouldThrow: false);

            Assert.Contains("OnEntry: SimpleTest.VoidMethods.ConditionallyThrowingInvalidOperationException", testClass.Messages);
            Assert.Contains("OnExit: SimpleTest.VoidMethods.ConditionallyThrowingInvalidOperationException", testClass.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_on_exception_with_conditional_throw()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.ConditionallyThrowingInvalidOperationException(shouldThrow: true)));

            Assert.Contains("OnEntry: SimpleTest.VoidMethods.ConditionallyThrowingInvalidOperationException", testClass.Messages);
            Assert.Equal("OnException: SimpleTest.VoidMethods.ConditionallyThrowingInvalidOperationException - System.InvalidOperationException: Ooops", Enumerable.Last(testClass.Messages));
        }

        // These should be a theory. Really need to sort out theory support in the reshaprer runner...
        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_1()
        {
            var testClass = TestClass;
            testClass.WithMultipleReturns(1);

            Assert.Equal("OnEntry: SimpleTest.VoidMethods.WithMultipleReturns", testClass.Messages[0]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", testClass.Messages[1]);
            Assert.Equal("OnExit: SimpleTest.VoidMethods.WithMultipleReturns", testClass.Messages[2]);
        }

        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_2()
        {
            var testClass = TestClass;
            testClass.WithMultipleReturns(2);

            Assert.Equal("OnEntry: SimpleTest.VoidMethods.WithMultipleReturns", testClass.Messages[0]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", testClass.Messages[1]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 1", testClass.Messages[2]);
            Assert.Equal("OnExit: SimpleTest.VoidMethods.WithMultipleReturns", testClass.Messages[3]);
        }

        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_3()
        {
            var testClass = TestClass;
            testClass.WithMultipleReturns(3);

            Assert.Equal("OnEntry: SimpleTest.VoidMethods.WithMultipleReturns", testClass.Messages[0]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", testClass.Messages[1]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 1", testClass.Messages[2]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 2", testClass.Messages[3]);
            Assert.Equal("OnExit: SimpleTest.VoidMethods.WithMultipleReturns", testClass.Messages[4]);
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_1()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.WithMultipleReturnsAndExceptions(1, shouldThrow: true)));

            Assert.Equal("OnEntry: SimpleTest.VoidMethods.WithMultipleReturnsAndExceptions", testClass.Messages[0]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 0", testClass.Messages[1]);
            Assert.Equal("OnException: SimpleTest.VoidMethods.WithMultipleReturnsAndExceptions - System.InvalidOperationException: Throwing at 1", testClass.Messages[2]);
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_2()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.WithMultipleReturnsAndExceptions(2, shouldThrow: true)));

            Assert.Equal("OnEntry: SimpleTest.VoidMethods.WithMultipleReturnsAndExceptions", testClass.Messages[0]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 0", testClass.Messages[1]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 1", testClass.Messages[2]);
            Assert.Equal("OnException: SimpleTest.VoidMethods.WithMultipleReturnsAndExceptions - System.InvalidOperationException: Throwing at 2", testClass.Messages[3]);
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_3()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.WithMultipleReturnsAndExceptions(3, shouldThrow: true)));

            Assert.Equal("OnEntry: SimpleTest.VoidMethods.WithMultipleReturnsAndExceptions", testClass.Messages[0]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 0", testClass.Messages[1]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 1", testClass.Messages[2]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 2", testClass.Messages[3]);
            Assert.Equal("OnException: SimpleTest.VoidMethods.WithMultipleReturnsAndExceptions - System.InvalidOperationException: Throwing at 3", testClass.Messages[4]);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_method_with_multiple_returns_ending_with_throw()
        {
            var testClass = TestClass;
            testClass.MultipleReturnValuesButEndingWithThrow(2);

            Assert.Equal("OnEntry: SimpleTest.VoidMethods.MultipleReturnValuesButEndingWithThrow", testClass.Messages[0]);
            Assert.Equal("MultipleReturnValuesButEndingWithThrow: Body - 0", testClass.Messages[1]);
            Assert.Equal("MultipleReturnValuesButEndingWithThrow: Body - 1", testClass.Messages[2]);
            Assert.Equal("OnExit: SimpleTest.VoidMethods.MultipleReturnValuesButEndingWithThrow", testClass.Messages[3]);
        }

        [Fact]
        public void Should_report_exception_with_method_with_multiple_returns_ending_with_throw()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.MultipleReturnValuesButEndingWithThrow(0)));

            Assert.Contains("OnEntry: SimpleTest.VoidMethods.MultipleReturnValuesButEndingWithThrow", testClass.Messages);
            Assert.Contains("OnException: SimpleTest.VoidMethods.MultipleReturnValuesButEndingWithThrow - System.InvalidOperationException: Ooops", testClass.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            Assembly = data.Assembly;
            TestClass.Messages.Clear();
        }

        private Assembly Assembly { get; set; }
        private dynamic TestClass { get { return Assembly.GetInstance("SimpleTest.VoidMethods"); } }
    }
}