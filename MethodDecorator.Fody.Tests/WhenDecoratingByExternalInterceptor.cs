using Xunit;

public class WhenDecoratingByExternalInterceptor : SimpleTestBase
{
    public WhenDecoratingByExternalInterceptor()
    {
        WeaverHelperWrapper.Assembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
        TestClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.MarkedFromAnotherAssembly");
    }

    public dynamic TestClass { get; set; }

    [Fact]
    public void ShouldNotifyOnInitModuleRegistered()
    {
        TestClass.ExternalInterceptorDecorated();
        Assert.Equal(1, ExternalInterceptorAttribute.InitCount);
        Assert.Equal("ExternalInterceptorDecorated", ExternalInterceptorAttribute.InitMethod.Name);
        Assert.Equal("MarkedFromAnotherAssembly", ExternalInterceptorAttribute.InitInstance.GetType().Name);
    }

    [Fact]
    public void ShouldNotifyOnInitAssemblyRegistered()
    {
        TestClass.ExternalInterceptorAssemblyLevelDecorated();
        Assert.Equal(1, ExternalInterceptionAssemblyLevelAttribute.InitCount);
        Assert.Equal("ExternalInterceptorAssemblyLevelDecorated", ExternalInterceptionAssemblyLevelAttribute.InitMethod.Name);
        Assert.Equal("MarkedFromAnotherAssembly", ExternalInterceptionAssemblyLevelAttribute.InitInstance.GetType().Name);
    }
}