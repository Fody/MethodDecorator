public class WhenDecoratingMethodsWithReturnValues() :
    ClassTestsBase("SimpleTest.InterceptingMethodsWithReturnValues")
{
    [Test]
    public async Task ShouldBeAbleToReturnPrimitiveType()
    {
        int value = TestClass.ReturnsNumber();
        await Assert.That((object) value).IsEqualTo(42);
    }

    [Test]
    public async Task ShouldBeAbleToReturnAReferenceType()
    {
        string value = TestClass.ReturnsString();
        await Assert.That((object) value).IsEqualTo("hello world");
    }

    [Test]
    public async Task ShouldBeAbleToReturnValueType()
    {
        DateTime value = TestClass.ReturnsDateTime();
        await Assert.That(value).IsEqualTo(new DateTime(2012, 4, 1));
    }

    [Test]
    public async Task ShouldNotifyOnEntryAndExit()
    {
        int value = TestClass.ReturnsNumber();
        await Assert.That((object) value).IsEqualTo(42);

        await CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.ReturnsNumber");
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldNotifyOfException()
    {
        await Assert.That(() => { TestClass.Throws(); }).Throws<InvalidOperationException>();

        await CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.Throws");
        await CheckEntry();
        await CheckException<InvalidOperationException>("Ooops");
    }

    [Test]
    public async Task ShouldReportEntryAndExitWithMultipleReturns1()
    {
        int value = TestClass.MultipleReturns(1);
        await Assert.That((object) value).IsEqualTo(7);

        await CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", 1);
        await CheckBody("MultipleReturns", "0");
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldReportEntryAndExitWithMultipleReturns2()
    {
        int value = TestClass.MultipleReturns(2);
        await Assert.That((object) value).IsEqualTo(14);

        await CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", 1);
        await CheckBody("MultipleReturns", "0");
        await CheckBody("MultipleReturns", "1");
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldReportEntryAndExitWithMultipleReturns3()
    {
        int value = TestClass.MultipleReturns(3);
        await Assert.That((object) value).IsEqualTo(21);

        await CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturns", 1);
        await CheckBody("MultipleReturns", "0");
        await CheckBody("MultipleReturns", "1");
        await CheckBody("MultipleReturns", "2");
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldReportEntryAndExitWithMethodWithMultipleReturnsEndingWithThrow()
    {
        int value = TestClass.MultipleReturnValuesButEndingWithThrow(2);
        await Assert.That((object) value).IsEqualTo(163);

        await CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow", 1);
        await CheckBody("MultipleReturnValuesButEndingWithThrow", "0");
        await CheckBody("MultipleReturnValuesButEndingWithThrow", "1");
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.OnExit]);
    }

    [Test]
    public async Task ShouldReportExceptionWithMethodWithMultipleReturnsEndingWithThrow()
    {
        await Assert.That(() => { TestClass.MultipleReturnValuesButEndingWithThrow(3); }).Throws<InvalidOperationException>();

        await CheckInit("SimpleTest.InterceptingMethodsWithReturnValues", "SimpleTest.InterceptingMethodsWithReturnValues.MultipleReturnValuesButEndingWithThrow", 1);
        await CheckBody("MultipleReturnValuesButEndingWithThrow", "0");
        await CheckBody("MultipleReturnValuesButEndingWithThrow", "1");
        await CheckBody("MultipleReturnValuesButEndingWithThrow", "2");
        await CheckException<InvalidOperationException>("Ooops");
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.Body, Method.Body, Method.Body, Method.OnException]);
    }
}