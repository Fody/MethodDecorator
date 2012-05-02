using System;
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
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => testClass.ExpectedVoidMethodThrowingInvalidOperationException()));

            Assert.Contains("OnEntry: SimpleTest.Class1.VoidMethodThrowingInvalidOperationException", testClass.Messages);
            Assert.Contains("OnException: SimpleTest.Class1.VoidMethodThrowingInvalidOperationException - System.InvalidOperationException: Ooops", testClass.Messages);
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