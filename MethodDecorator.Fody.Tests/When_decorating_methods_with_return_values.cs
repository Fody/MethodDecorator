using System;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_methods_with_return_values : IUseFixture<DecoratedSimpleTest>
    {
        [Fact]
        public void Should_continue_to_return_value()
        {
            int value = TestClass.ReturnsNumber();

            Assert.Equal(42, value);
        }

        [Fact]
        public void Should_notify_on_entry_and_exit()
        {
            int value = TestClass.ReturnsNumber();

            Assert.Equal(42, value);

            Assert.Contains("OnEntry: SimpleTest.MethodsReturningValues.ReturnsNumber", TestClass.Messages);
            Assert.Contains("OnExit: SimpleTest.MethodsReturningValues.ReturnsNumber", TestClass.Messages);
        }

        [Fact]
        public void Should_notify_of_exception()
        {
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() => TestClass.Throws()));

            Assert.Contains("OnEntry: SimpleTest.MethodsReturningValues.Throws", TestClass.Messages);
            Assert.Equal("OnException: SimpleTest.MethodsReturningValues.Throws - System.InvalidOperationException: Ooops", TestClass.Messages[1]);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            Assembly = data.Assembly;
            TestClass.Messages.Clear();
        }

        private Assembly Assembly { get; set; }
        private dynamic TestClass { get { return Assembly.GetInstance("SimpleTest.MethodsReturningValues"); } }
    }
}