public class WhenAsync() :
    ClassTestsBase("SimpleTest.AsyncClass")
{
    [Test]
    public async Task SimpleAsyncMethod()
    {
        Task x = TestClass.SimpleAsyncMethod();
        x.Wait();
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit]);
    }

    [Test]
    public async Task AsyncMethodWithResult()
    {
        Task<int> x = TestClass.SimpleAsyncMethodWithResult();
        var res = x.Result;
        await Assert.That((object) res).IsEqualTo(1);
        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit]);
    }

    [Test]
    public async Task AsyncMethodWithException()
    {
        try
        {
            Task<int> x = TestClass.SimpleAsyncMethodWithException();
            var res = x.Result;
        }
        catch (Exception)
        {
        }

        await CheckMethodSeq([Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit]);
    }
}