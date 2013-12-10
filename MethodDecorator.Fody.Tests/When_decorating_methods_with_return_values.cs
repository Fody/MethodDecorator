using System;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_methods_with_return_values : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        [Fact]
        public void Should_be_able_to_return_primitive_type()
        {
            int value = testClass.ReturnsNumber();

            Assert.Equal(42, value);
        }

        [Fact]
        public void Should_be_able_to_return_a_reference_type()
        {
            string value = testClass.ReturnsString();

            Assert.Equal("hello world", value);
        }

        [Fact]
        public void Should_be_able_to_return_value_type()
        {
            DateTime value = testClass.ReturnsDateTime();

            Assert.Equal(new DateTime(2012, 4, 1), value);
        }

        [Fact]
        public void Should_notify_on_entry_and_exit()
        {
            int value = testClass.ReturnsNumber();

            Assert.Equal(42, value);
            var x = testMessages.Messages;
            Assert.Contains("OnEntry: SimpleTest.InterceptingMethodsWithReturnValues.ReturnsNumber", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingMethodsWithReturnValues.ReturnsNumber", testMessages.Messages);
        }

        [Fact]
        public void Should_notify_of_exception()
        {
            Assert.Throws<InvalidOperationException>(() => testClass.Throws());

            Assert.Contains("OnEntry: SimpleTest.InterceptingMethodsWithReturnValues.Throws", testMessages.Messages);
            Assert.Equal("OnException: SimpleTest.InterceptingMethodsWithReturnValues.Throws - System.InvalidOperationException: Ooops", testMessages.Messages[1]);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_multiple_returns_1()
        {
            int value = testClass.MultipleReturns(1);

            Assert.Equal(7, value);

            Assert.Contains("OnEntry: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns [1]", testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 0", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", testMessages.Messages);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_multiple_returns_2()
        {
            int value = testClass.MultipleReturns(2);

            Assert.Equal(14, value);

            Assert.Contains("OnEntry: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns [1]", testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 0", testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 1", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", testMessages.Messages);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_multiple_returns_3()
        {
            int value = testClass.MultipleReturns(3);

            Assert.Equal(21, value);

            Assert.Contains("OnEntry: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns [1]", testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 0", testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 1", testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 2", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", testMessages.Messages);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_method_with_multiple_returns_ending_with_throw()
        {
            int value = testClass.MultipleReturnValuesButEndingWithThrow(2);

            Assert.Equal(163, value);

            Assert.Contains("OnEntry: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow [1]", testMessages.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 0", testMessages.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 1", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow", testMessages.Messages);
        }

        [Fact]
        public void Should_report_exception_with_method_with_multiple_returns_ending_with_throw()
        {
            Assert.Throws<InvalidOperationException>(() => testClass.MultipleReturnValuesButEndingWithThrow(3));

            Assert.Contains("OnEntry: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow [1]", testMessages.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 0", testMessages.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 1", testMessages.Messages);
            Assert.Contains("OnException: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow - System.InvalidOperationException: Ooops", testMessages.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testClass = assembly.GetInstance("SimpleTest.InterceptingMethodsWithReturnValues");
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        }
    }
}