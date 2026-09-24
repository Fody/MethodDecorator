using TUnit.Assertions.Enums;

public abstract class TestsBase
{
    protected abstract dynamic RecordHost { get; }

    protected IList<Tuple<Method, object[]>> Records
    {
        get
        {
            var records = (IList<Tuple<int, object[]>>) RecordHost.Records;
            return records.Select(x => new Tuple<Method, object[]>((Method) x.Item1, x.Item2)).ToList();
        }
    }

    protected async Task CheckMethodSeq(Method[] methods)
    {
        var coll = Records.Select(_ => _.Item1).ToArray();
        await Assert.That(coll).IsEquivalentTo(methods, CollectionOrdering.Matching);
    }

    protected async Task CheckException<TEx>(string message) where TEx : Exception
    {
        var args = GetRecordOfCallTo(Method.OnException).Item2;
        await Assert.That(args[0]).IsEqualTo(typeof(TEx));
        await Assert.That(args[1]).IsEqualTo(message);
    }

    protected async Task CheckInit(string instanceTypeName, string methodName, int argLength = 0)
    {
        var args = GetRecordOfCallTo(Method.Init).Item2;
        await Assert.That(args[0]?.ToString()).IsEqualTo(instanceTypeName);
        await Assert.That(args[1].ToString()).IsEqualTo(methodName);
        await Assert.That((int) args[2]).IsEqualTo(argLength);
    }

    private Tuple<Method, object[]> GetRecordOfCallTo(Method method)
    {
        var record = Records.SingleOrDefault(_ => _.Item1 == method);
        if (record == null)
        {
            throw new InvalidOperationException(method + " was not called.");
        }

        return record;
    }

    protected async Task CheckBody(string methodName, string extraInfo = null)
    {
        await Assert.That(Records.Any(_ => _.Item1 == Method.Body &&
                                     _.Item2[0] == methodName &&
                                     _.Item2[1] == extraInfo)).IsTrue();
    }

    protected async Task CheckEntry()
    {
        await Assert.That(Records.Any(_ => _.Item1 == Method.OnEnter)).IsTrue();
    }

    protected async Task CheckExit()
    {
        await Assert.That(Records.Any(_ => _.Item1 == Method.OnExit)).IsTrue();
    }
}