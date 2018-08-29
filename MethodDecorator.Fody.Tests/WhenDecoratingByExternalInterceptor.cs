using System.Reflection;
using Xunit;

public class WhenDecoratingByExternalInterceptor : SimpleTestBase
{
    static Assembly externalAssembly;

    static WhenDecoratingByExternalInterceptor()
    {
        var path = WeaverHelperWrapper.Assembly.Location.Replace("SimpleTest2.dll", "AnotherAssemblyAttributeContainer.dll");
        externalAssembly = Assembly.LoadFile(path);
    }

    public WhenDecoratingByExternalInterceptor()
    {
        WeaverHelperWrapper.Assembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
        externalAssembly.GetStaticInstance("SimpleTest.TestRecords").Clear();
        TestClass = WeaverHelperWrapper.Assembly.GetInstance("SimpleTest.MarkedFromAnotherAssembly");
    }

    public dynamic TestClass { get; set; }

    protected override dynamic RecordHost
    {
        get { return externalAssembly.GetStaticInstance("SimpleTest.TestRecords"); }
    }

    [Fact]
    public void ShouldNotifyOnInitModuleRegistered()
    {
        TestClass.ExternalInterceptorDecorated();
        CheckInit("SimpleTest.MarkedFromAnotherAssembly", "SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorDecorated");
    }

    [Fact]
    public void ShouldNotifyOnInitAssemblyRegistered()
    {
        TestClass.ExternalInterceptorAssemblyLevelDecorated();
        CheckInit("SimpleTest.MarkedFromAnotherAssembly", "SimpleTest.MarkedFromAnotherAssembly.ExternalInterceptorAssemblyLevelDecorated");
    }
}