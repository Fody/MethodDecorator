using System;
using System.Threading.Tasks;
using Xunit;

public class WhenAsync : ClassTestsBase
{
    public WhenAsync() : base("SimpleTest.AsyncClass")
    {
    }

    [Fact]
    public void SimpleAsyncMethod()
    {
        Task x = TestClass.SimpleAsyncMethod();
        x.Wait();
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit});
    }

    [Fact]
    public void AsyncMethodWithResult()
    {
        Task<int> x = TestClass.SimpleAsyncMethodWithResult();
        var res = x.Result;
        Assert.Equal(1, res);
        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit});
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

        CheckMethodSeq(new[] {Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit});
    }
}