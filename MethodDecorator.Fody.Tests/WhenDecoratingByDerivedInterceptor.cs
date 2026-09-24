public class WhenDecoratingByDerivedInterceptor : ClassTestsBase
{
    public WhenDecoratingByDerivedInterceptor() : base("SimpleTest.MarkedFromTheDerivedDecorator")
    {
    }

    [Test]
    public async Task ShouldNotifyInitEntryAndExit()
    {
        TestClass.CanLogInitEntryAndExit();
        await CheckInit("SimpleTest.MarkedFromTheDerivedDecorator", "MarkedFromTheDerivedDecorator.CanLogInitEntryAndExit()");
    }

    [Test]
    public async Task ShouldNotifyOnInitEntryAndException()
    {
        var ex = await Assert.That(() => { TestClass.CanLogInitEntryAndException(); }).Throws<ApplicationException>();

        await Assert.That((object) ex!.Message).IsEqualTo("boo!");

        await CheckInit("SimpleTest.MarkedFromTheDerivedDecorator", "MarkedFromTheDerivedDecorator.CanLogInitEntryAndException()");
        await CheckException<ApplicationException>("boo!");
    }
}