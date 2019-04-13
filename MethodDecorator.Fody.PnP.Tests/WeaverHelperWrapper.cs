using System.Reflection;
using Fody;

public static class WeaverHelperWrapper
{
    public static Assembly Assembly;
    public static TestResult TestResult;

    static WeaverHelperWrapper()
    {
        var moduleWeaver = new ModuleWeaver();

        TestResult = moduleWeaver.ExecuteTestRun(assemblyPath: "SimpleTest.PnP.dll"
#if NETCOREAPP2_1
            , runPeVerify: false
#endif
        );
        Assembly = TestResult.Assembly;
    }
}