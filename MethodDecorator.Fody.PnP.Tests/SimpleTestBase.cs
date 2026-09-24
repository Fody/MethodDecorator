using TUnit.Assertions.Enums;

// tests share the static TestRecords of the weaved assembly
[NotInParallel]
public class SimpleTestBase : TestsBase
{
    public SimpleTestBase()
    {
        WeaverHelperWrapper.Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords").Clear();
    }

    protected override dynamic RecordHost
    {
        get { return WeaverHelperWrapper.Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords"); }
    }

    protected async Task CheckMethod(Method iMethod)
    {
        var record = Records.SingleOrDefault(_ => _.Item1 == iMethod);
        await Assert.That(record).IsNotNull();
        await Assert.That(record.Item2).IsNull();
    }

    protected async Task CheckMethod(Method iMethod, object[] iParams)
    {
        var record = Records.SingleOrDefault(_ => _.Item1 == iMethod);
        await Assert.That(record).IsNotNull();
        await Assert.That(record.Item2).IsEquivalentTo(iParams, CollectionOrdering.Matching);
    }

    protected async Task CheckMethod(Method iMethod, object[][] iParams)
    {
        var records = Records.Where(_ => _.Item1 == iMethod).Select(_ => _.Item2);
        await Assert.That(records).IsNotEmpty();
        await Assert.That(records).IsEquivalentTo(iParams, CollectionOrdering.Matching);
    }
}