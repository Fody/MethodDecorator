public class WhenDecoratingPropertyMethods() :
    ClassTestsBase("SimpleTest.InterceptingPropertyMethods")
{
    [Test]
    public async Task ShouldNotifyOnEntryAndExitForManualPropertySetter()
    {
        TestClass.ManualProperty = 199;
        await CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.set_ManualProperty", 1);
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnExit]);
    }

    [Test]
    public async Task ShouldNotifyOnEntryAndExitForManualPropertyGetter()
    {
        int value = TestClass.ManualProperty;
        await Assert.That((object) value).IsEqualTo(0);

        await CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.get_ManualProperty");
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnExit]);
    }

    [Test]
    public async Task ShouldNotifyOnEntryAndExitForReadonlyPropertyAttributedOnGetter()
    {
        int value = TestClass.ReadOnlyProperty;
        await Assert.That((object) value).IsEqualTo(42);

        await CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.get_ReadOnlyProperty");
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnExit]);
    }

    [Test]
    public async Task ShouldNotifyOnEntryAndExceptionForPropertyGetter()
    {
        await Assert.That(() => { var ignored = TestClass.ThrowingProperty; }).Throws<InvalidOperationException>();

        await CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.get_ThrowingProperty");
        await CheckEntry();
        await CheckException<InvalidOperationException>("Ooops");
    }
}