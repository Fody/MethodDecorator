using System;
using Xunit;

public class WhenDecoratingByDerivedInterceptor : ClassTestsBase
{
    public WhenDecoratingByDerivedInterceptor() : base("SimpleTest.MarkedFromTheDerivedDecorator")
    {
    }

    [Fact]
    public void ShouldNotifyInitEntryAndExit()
    {
        TestClass.CanLogInitEntryAndExit();
        CheckInit("SimpleTest.MarkedFromTheDerivedDecorator", "MarkedFromTheDerivedDecorator.CanLogInitEntryAndExit()");
    }

    [Fact]
    public void ShouldNotifyOnInitEntryAndException()
    {
        var ex = Assert.Throws<ApplicationException>(() => { TestClass.CanLogInitEntryAndException(); });

        Assert.Equal("boo!", ex.Message);

        CheckInit("SimpleTest.MarkedFromTheDerivedDecorator", "MarkedFromTheDerivedDecorator.CanLogInitEntryAndException()");
        CheckException<ApplicationException>("boo!");
    }
}