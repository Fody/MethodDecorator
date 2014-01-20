using System;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_decorating_methods_with_return_values : IUseFixture<DecoratedSimpleTest> {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        public void SetFixture(DecoratedSimpleTest data) {
            this.assembly = data.Assembly;
            this.testClass = this.assembly.GetInstance("SimpleTest.InterceptingMethodsWithReturnValues");
            this.testMessages = this.assembly.GetStaticInstance("SimpleTest.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void Should_be_able_to_return_primitive_type() {
            int value = this.testClass.ReturnsNumber();

            Assert.Equal(42, value);
        }

        [Fact]
        public void Should_be_able_to_return_a_reference_type() {
            string value = this.testClass.ReturnsString();

            Assert.Equal("hello world", value);
        }

        [Fact]
        public void Should_be_able_to_return_value_type() {
            DateTime value = this.testClass.ReturnsDateTime();

            Assert.Equal(new DateTime(2012, 4, 1), value);
        }

        [Fact]
        public void Should_notify_on_entry_and_exit() {
            int value = this.testClass.ReturnsNumber();

            Assert.Equal(42, value);
            dynamic x = this.testMessages.Messages;
            Assert.Contains(
                "Init: SimpleTest.InterceptingMethodsWithReturnValues.ReturnsNumber [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_notify_of_exception() {
            Assert.Throws<InvalidOperationException>(() => this.testClass.Throws());

            Assert.Contains(
                "Init: SimpleTest.InterceptingMethodsWithReturnValues.Throws [0]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Equal("OnException: System.InvalidOperationException: Ooops", this.testMessages.Messages[2]);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_multiple_returns_1() {
            int value = this.testClass.MultipleReturns(1);

            Assert.Equal(7, value);

            Assert.Contains(
                "Init: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 0", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_multiple_returns_2() {
            int value = this.testClass.MultipleReturns(2);

            Assert.Equal(14, value);

            Assert.Contains(
                "Init: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 0", this.testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 1", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_multiple_returns_3() {
            int value = this.testClass.MultipleReturns(3);

            Assert.Equal(21, value);

            Assert.Contains(
                "Init: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 0", this.testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 1", this.testMessages.Messages);
            Assert.Contains("MultipleReturns: Body - 2", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_report_entry_and_exit_with_method_with_multiple_returns_ending_with_throw() {
            int value = this.testClass.MultipleReturnValuesButEndingWithThrow(2);

            Assert.Equal(163, value);

            Assert.Contains(
                "Init: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 0", this.testMessages.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 1", this.testMessages.Messages);
            Assert.Contains("OnExit", this.testMessages.Messages);
        }

        [Fact]
        public void Should_report_exception_with_method_with_multiple_returns_ending_with_throw() {
            Assert.Throws<InvalidOperationException>(() => this.testClass.MultipleReturnValuesButEndingWithThrow(3));

            Assert.Contains(
                "Init: SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow [1]",
                this.testMessages.Messages);
            Assert.Contains("OnEntry", this.testMessages.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 0", this.testMessages.Messages);
            Assert.Contains("MultipleReturnValuesButEndingWithThrow: Body - 1", this.testMessages.Messages);
            Assert.Contains("OnException: System.InvalidOperationException: Ooops", this.testMessages.Messages);
        }
    }
}