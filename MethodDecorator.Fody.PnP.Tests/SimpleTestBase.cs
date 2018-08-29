using System.Linq;
using System.Reflection;
using Xunit;

public class SimpleTestBase : TestsBase
{
    public static Assembly Assembly { get; }
    static SimpleTestBase()
    {
        var weaverHelper = new WeaverHelper(@"SimpleTest.PnP\SimpleTest.PnP.csproj");
        Assembly= weaverHelper.Weave();
    }

    public SimpleTestBase()
    {
        Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords").Clear();
    }

    protected override dynamic RecordHost
    {
        get { return Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords"); }
    }

    protected void CheckMethod(Method iMethod)
    {
        var record = Records.SingleOrDefault(x => x.Item1 == iMethod);
        Assert.NotNull(record);
        Assert.Null(record.Item2);
    }

    protected void CheckMethod(Method iMethod, object[] iParams)
    {
        var record = Records.SingleOrDefault(x => x.Item1 == iMethod);
        Assert.NotNull(record);
        Assert.Equal(iParams, record.Item2);
    }

    protected void CheckMethod(Method iMethod, object[][] iParams)
    {
        var records = Records.Where(x => x.Item1 == iMethod).Select(x => x.Item2);
        Assert.NotEmpty(records);
        Assert.Equal(iParams, records);
    }
}