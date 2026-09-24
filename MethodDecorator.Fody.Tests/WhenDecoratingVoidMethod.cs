public class WhenDecoratingVoidMethod() :
    ClassTestsBase("SimpleTest.InterceptingVoidMethods")
{
    [Test]
    public async Task ShouldNotifyInit()
    {
        TestClass.WithoutArgs();
        await CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithoutArgs");
    }

    [Test]
    public async Task ShouldNotifyOfMethodEntry()
    {
        TestClass.WithoutArgs();
        await CheckEntry();
    }

    [Test]
    public async Task ShouldNotifyOfMethodEntryAndExit()
    {
        TestClass.WithoutArgs();
        await CheckEntry();
        await CheckExit();
    }

    [Test]
    public async Task ShouldCallMethodBodyBetweenEnterAndExit()
    {
        TestClass.WithoutArgs();
        await CheckEntry();
        await CheckBody("VoidMethodWithoutArgs");
        await CheckExit();
    }

    [Test]
    public async Task ShouldNotifyOfThrownException()
    {
        await Assert.That(() => { TestClass.ThrowingInvalidOperationException(); }).Throws<InvalidOperationException>();

        await CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.ThrowingInvalidOperationException");
        await CheckEntry();
        await CheckException<InvalidOperationException>("Ooops");
    }

    [Test]
    public async Task ShouldNotNotifyExitWhenMethodThrows()
    {
        await Assert.That(() => { TestClass.ThrowingInvalidOperationException(); }).Throws<InvalidOperationException>();

        await Assert.That(Records.Any(_ => _.Item1 == Method.OnExit)).IsFalse();
    }

    [Test]
    public async Task ShouldReportOnEntryAndOnExitWithConditionalThrow()
    {
        TestClass.ConditionallyThrowingInvalidOperationException(shouldThrow: false);
        await CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", 1);
        await CheckEntry();
        await CheckExit();
    }

    [Test]
    public async Task ShouldReportOnEntryAndOnExceptionWithConditionalThrow()
    {
        await Assert.That(() => { TestClass.ConditionallyThrowingInvalidOperationException(shouldThrow: true); }).Throws<InvalidOperationException>();

        await CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.ConditionallyThrowingInvalidOperationException", 1);
        await CheckEntry();
        await CheckException<InvalidOperationException>("Ooops");
    }

    //TODO: These should be a theory. Really need to sort out theory support in the resharper runner...
    [Test]
    public async Task ShouldReportOnEntryAndExitWithMultipleReturns1()
    {
        TestClass.WithMultipleReturns(1);

        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldReportOnEntryAndExitWithMultipleReturns2()
    {
        TestClass.WithMultipleReturns(2);

        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldReportOnEntryAndExitWithMultipleReturns3()
    {
        TestClass.WithMultipleReturns(3);

        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldReportEntryAndExceptionWithMultipleReturns1()
    {
        await Assert.That(() => { TestClass.WithMultipleReturnsAndExceptions(1, shouldThrow: true); }).Throws<InvalidOperationException>();

        await CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", 2);
        await CheckEntry();
        await CheckBody("WithMultipleReturnsAndExceptions", "0");
        await CheckException<InvalidOperationException>("Throwing at 1");
    }

    [Test]
    public async Task ShouldReportEntryAndExceptionWithMultipleReturns2()
    {
        await Assert.That(() => { TestClass.WithMultipleReturnsAndExceptions(2, shouldThrow: true); }).Throws<InvalidOperationException>();

        await CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", 2);
        await CheckEntry();
        await CheckBody("WithMultipleReturnsAndExceptions", "0");
        await CheckBody("WithMultipleReturnsAndExceptions", "1");
        await CheckException<InvalidOperationException>("Throwing at 2");
    }

    [Test]
    public async Task ShouldReportEntryAndExceptionWithMultipleReturns3()
    {
        await Assert.That(() => { TestClass.WithMultipleReturnsAndExceptions(3, shouldThrow: true); }).Throws<InvalidOperationException>();

        await CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.WithMultipleReturnsAndExceptions", 2);
        await CheckEntry();
        await CheckBody("WithMultipleReturnsAndExceptions", "0");
        await CheckBody("WithMultipleReturnsAndExceptions", "1");
        await CheckBody("WithMultipleReturnsAndExceptions", "2");
        await CheckException<InvalidOperationException>("Throwing at 3");
    }

    [Test]
    public async Task ShouldReportEntryAndExitWithMethodWithMultipleReturnsEndingWithThrow()
    {
        TestClass.MultipleReturnValuesButEndingWithThrow(2);

        await CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow", 1);
        await CheckEntry();
        await CheckBody("MultipleReturnValuesButEndingWithThrow", "0");
        await CheckBody("MultipleReturnValuesButEndingWithThrow", "1");
        await CheckExit();
    }

    [Test]
    public async Task ShouldReportExceptionWithMethodWithMultipleReturnsEndingWithThrow()
    {
        await Assert.That(() => { TestClass.MultipleReturnValuesButEndingWithThrow(0); }).Throws<InvalidOperationException>();

        await CheckInit("SimpleTest.InterceptingVoidMethods", "SimpleTest.InterceptingVoidMethods.MultipleReturnValuesButEndingWithThrow", 1);
        await CheckEntry();
        await CheckException<InvalidOperationException>("Ooops");
    }
}