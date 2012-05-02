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
            testClass.VoidMethodWithoutArgs();

            Assert.Contains("OnEntry: SimpleTest.Class1.VoidMethodWithoutArgs", testClass.Messages);
        }

        [Fact]
        public void Should_notify_of_method_entry_and_exit()
        {
            var testClass = TestClass;
            testClass.VoidMethodWithoutArgs();

            Assert.Contains("OnEntry: SimpleTest.Class1.VoidMethodWithoutArgs", testClass.Messages);
            Assert.Contains("OnExit: SimpleTest.Class1.VoidMethodWithoutArgs", testClass.Messages);
        }

        [Fact]
        public void Should_call_method_body_between_enter_and_exit()
        {
            var testClass = TestClass;
            testClass.VoidMethodWithoutArgs();

            Assert.Equal("OnEntry: SimpleTest.Class1.VoidMethodWithoutArgs", testClass.Messages[0]);
            Assert.Equal("VoidMethodWithoutArgs: Body", testClass.Messages[1]);
            Assert.Equal("OnExit: SimpleTest.Class1.VoidMethodWithoutArgs", testClass.Messages[2]);
        }

        [Fact]
        public void Should_notify_of_thrown_exception()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.VoidMethodThrowingInvalidOperationException()));

            Assert.Contains("OnEntry: SimpleTest.Class1.VoidMethodThrowingInvalidOperationException", testClass.Messages);
            Assert.Contains("OnException: SimpleTest.Class1.VoidMethodThrowingInvalidOperationException - System.InvalidOperationException: Ooops", testClass.Messages);
        }

        [Fact]
        public void Should_not_notify_exit_when_method_throws()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.VoidMethodThrowingInvalidOperationException()));

            Assert.DoesNotContain("OnExit: SimpleTest.Class1.VoidMethodWithoutArgs", testClass.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_on_exit_with_conditional_throw()
        {
            var testClass = TestClass;
            testClass.VoidMethodConditionallyThrowingInvalidOperationException(shouldThrow: false);

            Assert.Contains("OnEntry: SimpleTest.Class1.VoidMethodConditionallyThrowingInvalidOperationException", testClass.Messages);
            Assert.Contains("OnExit: SimpleTest.Class1.VoidMethodConditionallyThrowingInvalidOperationException", testClass.Messages);
        }

        [Fact]
        public void Should_report_on_entry_and_on_exception_with_conditional_throw()
        {
            var testClass = TestClass;
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.VoidMethodConditionallyThrowingInvalidOperationException(shouldThrow: true)));

            Assert.Contains("OnEntry: SimpleTest.Class1.VoidMethodConditionallyThrowingInvalidOperationException", testClass.Messages);
            Assert.Equal("OnException: SimpleTest.Class1.VoidMethodConditionallyThrowingInvalidOperationException - System.InvalidOperationException: Ooops", Enumerable.Last(testClass.Messages));
        }

        // These should be a theory. Really need to sort out theory support in the reshaprer runner...
        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_1()
        {
            var testClass = TestClass;
            testClass.VoidMethodWithMultipleReturns(1);

            Assert.Equal("OnEntry: SimpleTest.Class1.VoidMethodWithMultipleReturns", testClass.Messages[0]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", testClass.Messages[1]);
            Assert.Equal("OnExit: SimpleTest.Class1.VoidMethodWithMultipleReturns", testClass.Messages[2]);
        }

        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_2()
        {
            var testClass = TestClass;
            testClass.VoidMethodWithMultipleReturns(2);

            Assert.Equal("OnEntry: SimpleTest.Class1.VoidMethodWithMultipleReturns", testClass.Messages[0]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", testClass.Messages[1]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 1", testClass.Messages[2]);
            Assert.Equal("OnExit: SimpleTest.Class1.VoidMethodWithMultipleReturns", testClass.Messages[3]);
        }

        [Fact]
        public void Should_report_on_entry_and_exit_with_multiple_returns_3()
        {
            var testClass = TestClass;
            testClass.VoidMethodWithMultipleReturns(3);

            Assert.Equal("OnEntry: SimpleTest.Class1.VoidMethodWithMultipleReturns", testClass.Messages[0]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 0", testClass.Messages[1]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 1", testClass.Messages[2]);
            Assert.Equal("VoidMethodWithMultipleReturns: Body - 2", testClass.Messages[3]);
            Assert.Equal("OnExit: SimpleTest.Class1.VoidMethodWithMultipleReturns", testClass.Messages[4]);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            Assembly = data.Assembly;
            TestClass.Messages.Clear();
        }

        private Assembly Assembly { get; set; }
        private dynamic TestClass { get { return Assembly.GetInstance("SimpleTest.Class1"); } }
    }
}