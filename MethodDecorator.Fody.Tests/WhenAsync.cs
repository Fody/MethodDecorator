public class WhenAsync() :
    ClassTestsBase("SimpleTest.AsyncClass")
{
    [Fact]
    public void SimpleAsyncMethod()
    {
        Task x = TestClass.SimpleAsyncMethod();
        x.Wait();
        CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit]);
    }

    [Fact]
    public void AsyncMethodWithResult()
    {
        Task<int> x = TestClass.SimpleAsyncMethodWithResult();
        var res = x.Result;
        Assert.Equal(1, res);
        CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit]);
    }

    [Fact]
    public void AsyncMethodWithException()
    {
        try
        {
            Task<int> x = TestClass.SimpleAsyncMethodWithException();
            var res = x.Result;
        }
        catch (Exception)
        {
        }

        CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit]);
    }
}