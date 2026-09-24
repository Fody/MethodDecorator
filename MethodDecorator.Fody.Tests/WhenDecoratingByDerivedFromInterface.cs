public class WhenDecoratingByDerivedFromInterface : ClassTestsBase
{
    public WhenDecoratingByDerivedFromInterface() : base("SimpleTest.MarkedFromTheDerivedInterface")
    {
    }

    [Test]
    public async Task ShouldNotifyInitEntryAndExit()
    {
        TestClass.CanLogInitEntryAndExit("something");
        await CheckInit("SimpleTest.MarkedFromTheDerivedInterface", "MarkedFromTheDerivedInterface.CanLogInitEntryAndExit(String)", 1);
    }

    [Test]
    public async Task ShouldNotifyOnInitEntryAndException()
    {
        var ex = await Assert.That(() => { TestClass.CanLogInitEntryAndException(); }).Throws<ApplicationException>();

        await Assert.That((object) ex!.Message).IsEqualTo("boo!");

        await CheckInit("SimpleTest.MarkedFromTheDerivedInterface", "MarkedFromTheDerivedInterface.CanLogInitEntryAndException()");
        await CheckException<ApplicationException>("boo!");
    }
}