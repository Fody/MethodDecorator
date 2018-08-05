using System;
using Xunit;

public class WhenDecoratingPropertyMethods : ClassTestsBase
{
    public WhenDecoratingPropertyMethods() : base("SimpleTest.InterceptingPropertyMethods")
    {
    }

    [Fact]
    public void ShouldNotifyOnEntryAndExitForManualPropertySetter()
    {
        TestClass.ManualProperty = 199;
        CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.set_ManualProperty", 1);
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnExit});
    }

    [Fact]
    public void ShouldNotifyOnEntryAndExitForManualPropertyGetter()
    {
        int value = TestClass.ManualProperty;
        Assert.Equal(0, value);

        CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.get_ManualProperty");
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnExit});
    }

    [Fact]
    public void ShouldNotifyOnEntryAndExitForReadonlyPropertyAttributedOnGetter()
    {
        int value = TestClass.ReadOnlyProperty;
        Assert.Equal(42, value);

        CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.get_ReadOnlyProperty");
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnExit});
    }

    [Fact]
    public void ShouldNotifyOnEntryAndExceptionForPropertyGetter()
    {
        Assert.Throws<InvalidOperationException>(() => TestClass.ThrowingProperty);

        CheckInit("SimpleTest.InterceptingPropertyMethods", "SimpleTest.InterceptingPropertyMethods.get_ThrowingProperty");
        CheckEntry();
        CheckException<InvalidOperationException>("Ooops");
    }
}