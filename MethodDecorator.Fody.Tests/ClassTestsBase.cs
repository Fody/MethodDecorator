public class ClassTestsBase : SimpleTestBase
{
    protected ClassTestsBase(string className)
    {
        this.className = className;
    }

    string className;

    protected dynamic TestClass
    {
        get { return WeaverHelperWrapper.Assembly.GetInstance(className); }
    }
}