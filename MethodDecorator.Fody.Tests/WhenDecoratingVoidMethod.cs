using System;
using Xunit;

public class WhenDecoratingVoidMethod : ClassTestsBase
{
    public WhenDecoratingVoidMethod() : base("SimpleTest.InterceptingVoidMethods")
    {
    }

    [Fact]
    public void ShouldNotifyInit()
    {
        TestClass.WithoutArgs();
        CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithoutArgs");
    }

    [Fact]
    public void ShouldNotifyOfMethodEntry()
    {
        TestClass.WithoutArgs();
        CheckEntry();
    }

    [Fact]
    public void ShouldNotifyOfMethodEntryAndExit()
    {
        TestClass.WithoutArgs();
        CheckEntry();
        CheckExit();
    }

    [Fact]
    public void ShouldCallMethodBodyBetweenEnterAndExit()
    {
        TestClass.WithoutArgs();
        CheckEntry();
        CheckBody("VoidMethodWithoutArgs");
        CheckExit();
    }

    [Fact]
    public void ShouldNotifyOfThrownException()
    {
        Assert.Throws<InvalidOperationException>(new Action(() => TestClass.ThrowingInvalidOperationException()));

        CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.ThrowingInvalidOperationException");
        CheckEntry();
        CheckException<InvalidOperationException>("Ooops");
    }

    [Fact]
    public void ShouldNotNotifyExitWhenMethodThrows()
    {
        Assert.Throws<InvalidOperationException>(new Action(() => TestClass.ThrowingInvalidOperationException()));

        Assert.DoesNotContain(Records, x => x.Item1 == Method.OnExit);
    }

    [Fact]
    public void ShouldReportOnEntryAndOnExitWithConditionalThrow()
    {
        TestClass.ConditionallyThrowingInvalidOperationException(shouldThrow: false);
        CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", 1);
        CheckEntry();
        CheckExit();
    }

    [Fact]
    public void ShouldReportOnEntryAndOnExceptionWithConditionalThrow()
    {
        Assert.Throws<InvalidOperationException>(new Action(() => TestClass.ConditionallyThrowingInvalidOperationException(shouldThrow: true)));

        CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", 1);
        CheckEntry();
        CheckException<InvalidOperationException>("Ooops");
    }

    //TODO: These should be a theory. Really need to sort out theory support in the resharper runner...
    [Fact]
    public void ShouldReportOnEntryAndExitWithMultipleReturns1()
    {
        TestClass.WithMultipleReturns(1);

        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.OnExit});
    }

    [Fact]
    public void ShouldReportOnEntryAndExitWithMultipleReturns2()
    {
        TestClass.WithMultipleReturns(2);

        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit});
    }

    [Fact]
    public void ShouldReportOnEntryAndExitWithMultipleReturns3()
    {
        TestClass.WithMultipleReturns(3);

        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.Body, Method.OnExit});
    }

    [Fact]
    public void ShouldReportEntryAndExceptionWithMultipleReturns1()
    {
        Assert.Throws<InvalidOperationException>(new Action(() => TestClass.WithMultipleReturnsAndExceptions(1, shouldThrow: true)));

        CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", 2);
        CheckEntry();
        CheckBody("WithMultipleReturnsAndExceptions", "0");
        CheckException<InvalidOperationException>("Throwing at 1");
    }

    [Fact]
    public void ShouldReportEntryAndExceptionWithMultipleReturns2()
    {
        Assert.Throws<InvalidOperationException>(new Action(() => TestClass.WithMultipleReturnsAndExceptions(2, shouldThrow: true)));

        CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", 2);
        CheckEntry();
        CheckBody("WithMultipleReturnsAndExceptions", "0");
        CheckBody("WithMultipleReturnsAndExceptions", "1");
        CheckException<InvalidOperationException>("Throwing at 2");
    }

    [Fact]
    public void ShouldReportEntryAndExceptionWithMultipleReturns3()
    {
        Assert.Throws<InvalidOperationException>(new Action(() => TestClass.WithMultipleReturnsAndExceptions(3, shouldThrow: true)));

        CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", 2);
        CheckEntry();
        CheckBody("WithMultipleReturnsAndExceptions", "0");
        CheckBody("WithMultipleReturnsAndExceptions", "1");
        CheckBody("WithMultipleReturnsAndExceptions", "2");
        CheckException<InvalidOperationException>("Throwing at 3");
    }

    [Fact]
    public void ShouldReportEntryAndExitWithMethodWithMultipleReturnsEndingWithThrow()
    {
        TestClass.MultipleReturnValuesButEndingWithThrow(2);

        CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow", 1);
        CheckEntry();
        CheckBody("MultipleReturnValuesButEndingWithThrow", "0");
        CheckBody("MultipleReturnValuesButEndingWithThrow", "1");
        CheckExit();
    }

    [Fact]
    public void ShouldReportExceptionWithMethodWithMultipleReturnsEndingWithThrow()
    {
        Assert.Throws<InvalidOperationException>(new Action(() => TestClass.MultipleReturnValuesButEndingWithThrow(0)));

        CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow", 1);
        CheckEntry();
        CheckException<InvalidOperationException>("Ooops");
    }
}