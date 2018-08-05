using System;
using Xunit;

public class WhenDecoratingByDerivedFromInterface : ClassTestsBase
{
    public WhenDecoratingByDerivedFromInterface() : base("SimpleTest.MarkedFromTheDerivedInterface")
    {
    }

    [Fact]
    public void ShouldNotifyInitEntryAndExit()
    {
        TestClass.CanLogInitEntryAndExit("something");
        CheckInit("SimpleTest.MarkedFromTheDerivedInterface", "MarkedFromTheDerivedInterface.CanLogInitEntryAndExit(String)", 1);
    }

    [Fact]
    public void ShouldNotifyOnInitEntryAndException()
    {
        var ex = Assert.Throws<ApplicationException>(() => { TestClass.CanLogInitEntryAndException(); });

        Assert.Equal("boo!", ex.Message);

        CheckInit("SimpleTest.MarkedFromTheDerivedInterface", "MarkedFromTheDerivedInterface.CanLogInitEntryAndException()");
        CheckException<ApplicationException>("boo!");
    }
}