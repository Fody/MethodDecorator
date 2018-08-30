using System.Reflection;
using Fody;
#pragma warning disable 618

public static class WeaverHelperWrapper
{
    public static Assembly Assembly;
    public static TestResult TestResult;

    static WeaverHelperWrapper()
    {
        var moduleWeaver = new ModuleWeaver();

        TestResult = moduleWeaver.ExecuteTestRun(assemblyPath: "SimpleTest.dll");
        Assembly = TestResult.Assembly;
    }
}