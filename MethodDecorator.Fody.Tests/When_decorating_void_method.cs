using System;
using System.Linq;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_decorating_void_method : IUseFixture<DecoratedSimpleTest> {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        public void SetFixture(DecoratedSimpleTest data) {
            this.assembly = data.Assembly;
            this.testClass = this.assembly.GetInstance("SimpleTest.InterceptingVoidMethods");
            this.testMessages = this.assembly.GetStaticInstance("SimpleTest.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void ShouldNotifyInit() {
            this.testClass.WithoutArgs();

            Assert.Contains("Init: SimpleTest.InterceptingVoidMethods.WithoutArgs [0]", this.testMessages.Messages);
        }

        [Fact]
        public void Should_notify_of_method_entry() {
            this.testClass.WithoutArgs();

            Assert.Contains("OnEntry", this.testMessages.Messages);
        }

        [Fact]
        public void Should_notify_of_method_entry_and_exit() {
            this.testClass.WithoutArgs();

            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_call_method_body_between_enter_and_exit() {
            this.testClass.WithoutArgs();

            Assert.Equal("OnEntry", this.testMessages.Messages[1]);
            Assert.Equal("VoidMethodWithoutArgs: Body", this.testMessages.Messages[2]);
            Assert.Equal("OnExit", this.testMessages.Messages[3]);
        }

        [Fact]
        public void Should_notify_of_thrown_exception() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(() => this.testClass.ThrowingInvalidOperationException()));

            Assert.Contains(
                "Init: SimpleTest.InterceptingVoidMethods.ThrowingInvalidOperationException [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnException: System.InvalidOperationException: Ooops", this.testMessages.Messages);
        }

        [Fact]
        public void Should_not_notify_exit_when_method_throws() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(() => this.testClass.ThrowingInvalidOperationException()));

            Assert.DoesNotContain("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_on_exit_with_conditional_throw() {
            this.testClass.ConditionallyThrowingInvalidOperationException(shouldThrow: false);

            Assert.Contains(
                "Init: SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_on_exception_with_conditional_throw() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(
                    () => this.testClass.ConditionallyThrowingInvalidOperationException(shouldThrow: true)));

            Assert.Contains(
                "Init: SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Equal(
                "OnException: System.InvalidOperationException: Ooops",
                Enumerable.Last(this.testMessages.Messages));
        }

        // These should be a theory. Really need to sort out theory support in the reshaprer runner...
        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_1() {
            this.testClass.WithMultipleReturns(1);

            Assert.Equal(
                "Init: SimpleTest.InterceptingVoidMethods.WithMultipleReturns [1]",
                this.testMessages.Messages[0]);
            Assert.Equal("OnEntry", this.testMessages.Messages[1]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", this.testMessages.Messages[2]);
            Assert.Equal("OnExit", this.testMessages.Messages[3]);
        }

        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_2() {
            this.testClass.WithMultipleReturns(2);

            Assert.Equal(
                "Init: SimpleTest.InterceptingVoidMethods.WithMultipleReturns [1]",
                this.testMessages.Messages[0]);
            Assert.Equal("OnEntry", this.testMessages.Messages[1]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", this.testMessages.Messages[2]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 1", this.testMessages.Messages[3]);
            Assert.Equal("OnExit", this.testMessages.Messages[4]);
        }

        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_3() {
            this.testClass.WithMultipleReturns(3);

            Assert.Equal(
                "Init: SimpleTest.InterceptingVoidMethods.WithMultipleReturns [1]",
                this.testMessages.Messages[0]);
            Assert.Equal("OnEntry", this.testMessages.Messages[1]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", this.testMessages.Messages[2]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 1", this.testMessages.Messages[3]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 2", this.testMessages.Messages[4]);
            Assert.Equal("OnExit", this.testMessages.Messages[5]);
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_1() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(() => this.testClass.WithMultipleReturnsAndExceptions(1, shouldThrow: true)));

            Assert.Equal(
                "Init: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions [2]",
                this.testMessages.Messages[0]);
            Assert.Equal("OnEntry", this.testMessages.Messages[1]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 0", this.testMessages.Messages[2]);
            Assert.Equal("OnException: System.InvalidOperationException: Throwing at 1", this.testMessages.Messages[3]);
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_2() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(() => this.testClass.WithMultipleReturnsAndExceptions(2, shouldThrow: true)));

            Assert.Equal(
                "Init: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions [2]",
                this.testMessages.Messages[0]);
            Assert.Equal("OnEntry", this.testMessages.Messages[1]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 0", this.testMessages.Messages[2]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 1", this.testMessages.Messages[3]);
            Assert.Equal("OnException: System.InvalidOperationException: Throwing at 2", this.testMessages.Messages[4]);
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_3() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(() => this.testClass.WithMultipleReturnsAndExceptions(3, shouldThrow: true)));

            Assert.Equal(
                "Init: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions [2]",
                this.testMessages.Messages[0]);
            Assert.Equal("OnEntry", this.testMessages.Messages[1]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 0", this.testMessages.Messages[2]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 1", this.testMessages.Messages[3]);
            Assert.Equal("WithMultipleReturnsAndExceptions: Body - 2", this.testMessages.Messages[4]);
            Assert.Equal("OnException: System.InvalidOperationException: Throwing at 3", this.testMessages.Messages[5]);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_method_with_multiple_returns_ending_with_throw() {
            this.testClass.MultipleReturnValuesButEndingWithThrow(2);

            Assert.Equal(
                "Init: SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow [1]",
                this.testMessages.Messages[0]);
            Assert.Equal("OnEntry", this.testMessages.Messages[1]);
            Assert.Equal("MultipleReturnValuesButEndingWithThrow: Body - 0", this.testMessages.Messages[2]);
            Assert.Equal("MultipleReturnValuesButEndingWithThrow: Body - 1", this.testMessages.Messages[3]);
            Assert.Equal("OnExit", this.testMessages.Messages[4]);
        }

        [Fact]
        public void Should_report_exception_with_method_with_multiple_returns_ending_with_throw() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(() => this.testClass.MultipleReturnValuesButEndingWithThrow(0)));

            Assert.Contains(
                "Init: SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnException: System.InvalidOperationException: Ooops", this.testMessages.Messages);
        }
    }
}