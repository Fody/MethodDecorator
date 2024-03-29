﻿public abstract class TestsBase
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

    protected void CheckMethodSeq(Method[] methods)
    {
        var coll = Records.Select(_ => _.Item1).ToArray();
        Assert.Equal(methods, coll);
    }

    protected void CheckException<TEx>(string message) where TEx : Exception
    {
        var args = GetRecordOfCallTo(Method.OnException).Item2;
        Assert.Equal(typeof(TEx), args[0]);
        Assert.Equal(message, args[1]);
    }

    protected void CheckInit(string instanceTypeName, string methodName, int argLength = 0)
    {
        var args = GetRecordOfCallTo(Method.Init).Item2;
        Assert.Equal(instanceTypeName, args[0]?.ToString());
        Assert.Equal(methodName, args[1].ToString());
        Assert.Equal(argLength, (int) args[2]);
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

    protected void CheckBody(string methodName, string extraInfo = null)
    {
        Assert.Contains(Records, _ => _.Item1 == Method.Body &&
                                     _.Item2[0] == methodName &&
                                     _.Item2[1] == extraInfo);
    }

    protected void CheckEntry()
    {
        Assert.Contains(Records, _ => _.Item1 == Method.OnEnter);
    }

    protected void CheckExit()
    {
        Assert.Contains(Records, _ => _.Item1 == Method.OnExit);
    }
}