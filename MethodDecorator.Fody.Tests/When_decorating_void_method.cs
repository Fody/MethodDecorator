using System;
using System.Linq;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratingVoidMethod : ClassTestsBase<DecoratedSimpleTest> {
        public WhenDecoratingVoidMethod() : base("SimpleTest.InterceptingVoidMethods") { }

        [Fact]
        public void ShouldNotifyInit() {
            this.TestClass.WithoutArgs();
            this.CheckInit("SimpleTest.InterceptingVoidMethods.WithoutArgs");
        }

        [Fact]
        public void ShouldNotifyOfMethodEntry() {
            this.TestClass.WithoutArgs();
            this.CheckEntry();
        }

        [Fact]
        public void ShouldNotifyOfMethodEntryAndExit() {
            this.TestClass.WithoutArgs();
            this.CheckEntry();
            this.CheckExit();
        }

        [Fact]
        public void ShouldCallMethodBodyBetweenEnterAndExit() {
            this.TestClass.WithoutArgs();
            this.CheckEntry();
            this.CheckBody("VoidMethodWithoutArgs");
            this.CheckExit();
        }

        [Fact]
        public void ShouldNotifyOfThrownException() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(() => this.TestClass.ThrowingInvalidOperationException()));

            this.CheckInit("SimpleTest.InterceptingVoidMethods.ThrowingInvalidOperationException");
            this.CheckEntry();
            CheckException<InvalidOperationException>("Ooops");
        }

        [Fact]
        public void ShouldNotNotifyExitWhenMethodThrows() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(() => this.TestClass.ThrowingInvalidOperationException()));

            Assert.False(this.Records.Any(x => x.Item1 == Method.OnExit));
        }

        [Fact]
        public void ShouldReportOnEntryAndOnExitWithConditionalThrow() {
            this.TestClass.CondintionallyThrowingInvalidOperationException(shouldThrow: false);
            this.CheckInit("SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", 1);
            this.CheckEntry();
            this.CheckExit();
        }

        [Fact]
        public void ShouldReportOnEntryAndOnExceptionWithConditionalThrow() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(
                    () => this.TestClass.ConditionallyThrowingInvalidOperationException(shouldThrow: true)));

            this.CheckInit("SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", 1);
            this.CheckEntry();
            CheckException<InvalidOperationException>("Ooops");
        }

        // These should be a theory. Really need to sort out theory support in the reshaprer runner...
        [Fact]
        public void ShouldReportOnEntryAndExitWithMultipleReturns1() {
            this.TestClass.WithMultipleReturns(1);

            this.CheckMethodSeq(new []{ Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldReportOnEntryAndExitWithMultipleReturns2() {
            this.TestClass.WithMultipleReturns(2);

            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit });        
        }

        [Fact]
        public void ShouldReportOnEntryAndExitWithMultipleReturns3() {
            this.TestClass.WithMultipleReturns(3);

            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.Body,Method.Body, Method.OnExit });
        }

        [Fact]
        public void Should_report_entry_and_exception_with_multiple_returns_1() {
            Assert.Throws<InvalidOperationException>(
                new Assert.ThrowsDelegate(() => this.TestClass.WithMultipleReturnsAndExceptions(1, shouldThrow: true)));

            
            
            Assert.Equal(
                "Init: SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions [2]",
                this.TestMessages.Messages[0]);
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