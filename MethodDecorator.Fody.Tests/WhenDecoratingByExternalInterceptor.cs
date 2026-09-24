public class WhenDecoratingByExternalInterceptor : SimpleTestBase
{
    public WhenDecoratingByExternalInterceptor()
    {
        WeaverHelperWrapper.Assembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
        TestClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.MarkedFromAnotherAssembly");
    }

    private dynamic TestClass { get; set; }

    [Test]
    public async Task ShouldNotifyOnInitModuleRegistered()
    {
        TestClass.ExternalInterceptorDecorated();
        await Assert.That((object) ExternalInterceptorAttribute.InitCount).IsEqualTo(1);
        await Assert.That((object) ExternalInterceptorAttribute.InitMethod.Name).IsEqualTo("ExternalInterceptorDecorated");
        await Assert.That((object) ExternalInterceptorAttribute.InitInstance.GetType().Name).IsEqualTo("MarkedFromAnotherAssembly");
    }

    [Test]
    public async Task ShouldNotifyOnInitAssemblyRegistered()
    {
        TestClass.ExternalInterceptorAssemblyLevelDecorated();
        await Assert.That((object) ExternalInterceptionAssemblyLevelAttribute.InitCount).IsEqualTo(1);
        await Assert.That((object) ExternalInterceptionAssemblyLevelAttribute.InitMethod.Name).IsEqualTo("ExternalInterceptorAssemblyLevelDecorated");
        await Assert.That((object) ExternalInterceptionAssemblyLevelAttribute.InitInstance.GetType().Name).IsEqualTo("MarkedFromAnotherAssembly");
    }
}