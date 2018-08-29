using System.Reflection;

public static class WeaverHelperWrapper
{
    public static Assembly Assembly;

    static WeaverHelperWrapper()
    {
        var weaverHelper = new WeaverHelper(@"SimpleTest\SimpleTest.csproj");
        Assembly= weaverHelper.Weave();
    }
}