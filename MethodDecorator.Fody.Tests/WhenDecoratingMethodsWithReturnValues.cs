using System;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenDecoratingMethodsWithReturnValues : ClassTestsBase<DecoratedSimpleTest> {
        public WhenDecoratingMethodsWithReturnValues() : base("SimpleTest.InterceptingMethodsWithReturnValues") { }

        [Fact]
        public void ShouldBeAbleToReturnPrimitiveType() {
            int value = this.TestClass.ReturnsNumber();
            Assert.Equal(42, value);
        }

        [Fact]
        public void ShouldBeAbleToReturnAReferenceType() {
            string value = this.TestClass.ReturnsString();
            Assert.Equal("hello world", value);
        }

        [Fact]
        public void ShouldBeAbleToReturnValueType() {
            DateTime value = this.TestClass.ReturnsDateTime();
            Assert.Equal(new DateTime(2012, 4, 1), value);
        }

        [Fact]
        public void ShouldNotifyOnEntryAndExit() {
            int value = this.TestClass.ReturnsNumber();
            Assert.Equal(42, value);

            this.CheckInit("SimpleTest.InterceptingMethodsWithReturnValues.ReturnsNumber");
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldNotifyOfException() {
            Assert.Throws<InvalidOperationException>(() => this.TestClass.Throws());

            this.CheckInit("SimpleTest.InterceptingMethodsWithReturnValues.Throws");
            this.CheckEntry();
            CheckException<InvalidOperationException>("Ooops");
        }

        [Fact]
        public void ShouldReportEntryAndExitWithMultipleReturns1() {
            int value = this.TestClass.MultipleReturns(1);
            Assert.Equal(7, value);

            this.CheckInit("SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", 1);
            this.CheckBody("MultipleReturns", "0");
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldReportEntryAndExitWithMultipleReturns2() {
            int value = this.TestClass.MultipleReturns(2);
            Assert.Equal(14, value);

            this.CheckInit("SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", 1);
            this.CheckBody("MultipleReturns", "0");
            this.CheckBody("MultipleReturns", "1");
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldReportEntryAndExitWithMultipleReturns3() {
            int value = this.TestClass.MultipleReturns(3);
            Assert.Equal(21, value);

            this.CheckInit("SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", 1);
            this.CheckBody("MultipleReturns", "0");
            this.CheckBody("MultipleReturns", "1");
            this.CheckBody("MultipleReturns", "2");
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldReportEntryAndExitWithMethodWithMultipleReturnsEndingWithThrow() {
            int value = this.TestClass.MultipleReturnValuesButEndingWithThrow(2);
            Assert.Equal(163, value);

            this.CheckInit("SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow", 1);
            this.CheckBody("MultipleReturnValuesButEndingWithThrow", "0");
            this.CheckBody("MultipleReturnValuesButEndingWithThrow", "1");
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit });
        }

        [Fact]
        public void ShouldReportExceptionWithMethodWithMultipleReturnsEndingWithThrow() {
            Assert.Throws<InvalidOperationException>(() => this.TestClass.MultipleReturnValuesButEndingWithThrow(3));

            this.CheckInit("SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow", 1);
            this.CheckBody("MultipleReturnValuesButEndingWithThrow", "0");
            this.CheckBody("MultipleReturnValuesButEndingWithThrow", "1");
            this.CheckBody("MultipleReturnValuesButEndingWithThrow", "2");
            CheckException<InvalidOperationException>("Ooops");
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.Body, Method.OnException });
        }
    }
}