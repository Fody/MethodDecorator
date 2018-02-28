using System;
using System.Linq;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class WhenDecoratingVoidMethod : ClassTestsBase {
        public WhenDecoratingVoidMethod() : base("SimpleTest.InterceptingVoidMethods") { }

        [Fact]
        public void ShouldNotifyInit() {
            this.TestClass.WithoutArgs();
            this.CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithoutArgs");
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
            Assert.Throws<InvalidOperationException>(new Action(() => this.TestClass.ThrowingInvalidOperationException()));

            this.CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.ThrowingInvalidOperationException");
            this.CheckEntry();
            CheckException<InvalidOperationException>("Ooops");
        }

        [Fact]
        public void ShouldNotNotifyExitWhenMethodThrows() {
            Assert.Throws<InvalidOperationException>(new Action(() => this.TestClass.ThrowingInvalidOperationException()));

            Assert.False(this.Records.Any(x => x.Item1 == Method.OnExit));
        }

        [Fact]
        public void ShouldReportOnEntryAndOnExitWithConditionalThrow() {
            this.TestClass.ConditionallyThrowingInvalidOperationException(shouldThrow: false);
            this.CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", 1);
            this.CheckEntry();
            this.CheckExit();
        }

        [Fact]
        public void ShouldReportOnEntryAndOnExceptionWithConditionalThrow() {
            Assert.Throws<InvalidOperationException>(new Action(() => this.TestClass.ConditionallyThrowingInvalidOperationException(shouldThrow: true)));

            this.CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", 1);
            this.CheckEntry();
            CheckException<InvalidOperationException>("Ooops");
        }

        // These should be a theory. Really need to sort out theory support in the reshaprer runner...
        [Fact]
        public void ShouldReportOnEntryAndExitWithMultipleReturns1() {
            this.TestClass.WithMultipleReturns(1);

            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldReportOnEntryAndExitWithMultipleReturns2() {
            this.TestClass.WithMultipleReturns(2);

            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldReportOnEntryAndExitWithMultipleReturns3() {
            this.TestClass.WithMultipleReturns(3);

            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldReportEntryAndExceptionWithMultipleReturns1() {
            Assert.Throws<InvalidOperationException>(new Action(() => this.TestClass.WithMultipleReturnsAndExceptions(1, shouldThrow: true)));

            this.CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", 2);
            this.CheckEntry();
            this.CheckBody("WithMultipleReturnsAndExceptions", "0");
            CheckException<InvalidOperationException>("Throwing at 1");
        }

        [Fact]
        public void ShouldReportEntryAndExceptionWithMultipleReturns2() {
            Assert.Throws<InvalidOperationException>(new Action(() => this.TestClass.WithMultipleReturnsAndExceptions(2, shouldThrow: true)));

            this.CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", 2);
            this.CheckEntry();
            this.CheckBody("WithMultipleReturnsAndExceptions", "0");
            this.CheckBody("WithMultipleReturnsAndExceptions", "1");
            CheckException<InvalidOperationException>("Throwing at 2");
        }

        [Fact]
        public void ShouldReportEntryAndExceptionWithMultipleReturns3() {
            Assert.Throws<InvalidOperationException>(new Action(() => this.TestClass.WithMultipleReturnsAndExceptions(3, shouldThrow: true)));

            this.CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", 2);
            this.CheckEntry();
            this.CheckBody("WithMultipleReturnsAndExceptions", "0");
            this.CheckBody("WithMultipleReturnsAndExceptions", "1");
            this.CheckBody("WithMultipleReturnsAndExceptions", "2");
            CheckException<InvalidOperationException>("Throwing at 3");
        }

        [Fact]
        public void ShouldReportEntryAndExitWithMethodWithMultipleReturnsEndingWithThrow() {
            this.TestClass.MultipleReturnValuesButEndingWithThrow(2);

            this.CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow", 1);
            this.CheckEntry();
            this.CheckBody("MultipleReturnValuesButEndingWithThrow", "0");
            this.CheckBody("MultipleReturnValuesButEndingWithThrow", "1");
            this.CheckExit();
        }

        [Fact]
        public void ShouldReportExceptionWithMethodWithMultipleReturnsEndingWithThrow() {
            Assert.Throws<InvalidOperationException>(new Action(() => this.TestClass.MultipleReturnValuesButEndingWithThrow(0)));

            this.CheckInit("SimpleTest.InterceptingVoidMethods","SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow", 1);
            this.CheckEntry();
            this.CheckException<InvalidOperationException>("Ooops");
        }
    }
}