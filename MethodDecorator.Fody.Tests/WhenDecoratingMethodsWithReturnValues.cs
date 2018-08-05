using System;

using Xunit;

public class WhenDecoratingMethodsWithReturnValues : ClassTestsBase
{
    public WhenDecoratingMethodsWithReturnValues() : base("SimpleTest.InterceptingMethodsWithReturnValues")
    {
    }

    [Fact]
    public void ShouldBeAbleToReturnPrimitiveType()
    {
        int value = TestClass.ReturnsNumber();
        Assert.Equal(42, value);
    }

    [Fact]
    public void ShouldBeAbleToReturnAReferenceType()
    {
        string value = TestClass.ReturnsString();
        Assert.Equal("hello world", value);
    }

    [Fact]
    public void ShouldBeAbleToReturnValueType()
    {
        DateTime value = TestClass.ReturnsDateTime();
        Assert.Equal(new DateTime(2012, 4, 1), value);
    }

    [Fact]
    public void ShouldNotifyOnEntryAndExit()
    {
        int value = TestClass.ReturnsNumber();
        Assert.Equal(42, value);

        CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.ReturnsNumber");
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.OnExit});
    }

    [Fact]
    public void ShouldNotifyOfException()
    {
        Assert.Throws<InvalidOperationException>(() => TestClass.Throws());

        CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.Throws");
        CheckEntry();
        CheckException<InvalidOperationException>("Ooops");
    }

    [Fact]
    public void ShouldReportEntryAndExitWithMultipleReturns1()
    {
        int value = TestClass.MultipleReturns(1);
        Assert.Equal(7, value);

        CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", 1);
        CheckBody("MultipleReturns", "0");
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.OnExit});
    }

    [Fact]
    public void ShouldReportEntryAndExitWithMultipleReturns2()
    {
        int value = TestClass.MultipleReturns(2);
        Assert.Equal(14, value);

        CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", 1);
        CheckBody("MultipleReturns", "0");
        CheckBody("MultipleReturns", "1");
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit});
    }

    [Fact]
    public void ShouldReportEntryAndExitWithMultipleReturns3()
    {
        int value = TestClass.MultipleReturns(3);
        Assert.Equal(21, value);

        CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", 1);
        CheckBody("MultipleReturns", "0");
        CheckBody("MultipleReturns", "1");
        CheckBody("MultipleReturns", "2");
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.Body, Method.OnExit});
    }

    [Fact]
    public void ShouldReportEntryAndExitWithMethodWithMultipleReturnsEndingWithThrow()
    {
        int value = TestClass.MultipleReturnValuesButEndingWithThrow(2);
        Assert.Equal(163, value);

        CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow", 1);
        CheckBody("MultipleReturnValuesButEndingWithThrow", "0");
        CheckBody("MultipleReturnValuesButEndingWithThrow", "1");
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit});
    }

    [Fact]
    public void ShouldReportExceptionWithMethodWithMultipleReturnsEndingWithThrow()
    {
        Assert.Throws<InvalidOperationException>(() => TestClass.MultipleReturnValuesButEndingWithThrow(3));

        CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow", 1);
        CheckBody("MultipleReturnValuesButEndingWithThrow", "0");
        CheckBody("MultipleReturnValuesButEndingWithThrow", "1");
        CheckBody("MultipleReturnValuesButEndingWithThrow", "2");
        CheckException<InvalidOperationException>("Ooops");
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.Body, Method.OnException});
    }
}