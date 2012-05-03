using System;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_methods_with_return_values : IUseFixture<DecoratedSimpleTest>
    {
        [Fact]
        public void Should_be_able_to_return_primitive_type()
        {
            int value = TestClass.ReturnsNumber();

            Assert.Equal(42, value);
        }

        [Fact]
        public void Should_be_able_to_return_a_reference_type()
        {
            string value = TestClass.ReturnsString();

            Assert.Equal("hello world", value);
        }

        [Fact]
        public void Should_be_able_to_return_value_type()
        {
            DateTime value = TestClass.ReturnsDateTime();

            Assert.Equal(new DateTime(2012, 4, 1), value);
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
            Assert.Throws<InvalidOperationException>(() => TestClass.Throws());

            Assert.Contains("OnEntry: SimpleTest.MethodsReturningValues.Throws", TestClass.Messages);
            Assert.Equal("OnException: SimpleTest.MethodsReturningValues.Throws - System.InvalidOperationException: Ooops", TestClass.Messages[1]);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_multiple_returns_1()
        {
            int value = TestClass.MultipleReturns(1);

            Assert.Equal(7, value);

            Assert.Contains("OnEntry: SimpleTest.MethodsReturningValues.MultipleReturns", TestClass.Messages);
            Assert.Contains("MultipleReturns: Body - 0", TestClass.Messages);
            Assert.Contains("OnExit: SimpleTest.MethodsReturningValues.MultipleReturns", TestClass.Messages);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_multiple_returns_2()
        {
            int value = TestClass.MultipleReturns(2);

            Assert.Equal(14, value);

            Assert.Contains("OnEntry: SimpleTest.MethodsReturningValues.MultipleReturns", TestClass.Messages);
            Assert.Contains("MultipleReturns: Body - 0", TestClass.Messages);
            Assert.Contains("MultipleReturns: Body - 1", TestClass.Messages);
            Assert.Contains("OnExit: SimpleTest.MethodsReturningValues.MultipleReturns", TestClass.Messages);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_multiple_returns_3()
        {
            int value = TestClass.MultipleReturns(3);

            Assert.Equal(21, value);

            Assert.Contains("OnEntry: SimpleTest.MethodsReturningValues.MultipleReturns", TestClass.Messages);
            Assert.Contains("MultipleReturns: Body - 0", TestClass.Messages);
            Assert.Contains("MultipleReturns: Body - 1", TestClass.Messages);
            Assert.Contains("MultipleReturns: Body - 2", TestClass.Messages);
            Assert.Contains("OnExit: SimpleTest.MethodsReturningValues.MultipleReturns", TestClass.Messages);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_method_with_multiple_returns_ending_with_throw()
        {
            int value = TestClass.MultipleReturnValuesButEndingWithThrow(2);

            Assert.Equal(163, value);

            Assert.Contains("OnEntry: SimpleTest.MethodsReturningValues.MultipleReturnValuesButEndingWithThrow", TestClass.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 0", TestClass.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 1", TestClass.Messages);
            Assert.Contains("OnExit: SimpleTest.MethodsReturningValues.MultipleReturnValuesButEndingWithThrow", TestClass.Messages);
        }

        [Fact]
        public void Should_report_exception_with_method_with_multiple_returns_ending_with_throw()
        {
            Assert.Throws<InvalidOperationException>(() => TestClass.MultipleReturnValuesButEndingWithThrow(3));

            Assert.Contains("OnEntry: SimpleTest.MethodsReturningValues.MultipleReturnValuesButEndingWithThrow", TestClass.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 0", TestClass.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 1", TestClass.Messages);
            Assert.Contains("OnException: SimpleTest.MethodsReturningValues.MultipleReturnValuesButEndingWithThrow - System.InvalidOperationException: Ooops", TestClass.Messages);
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