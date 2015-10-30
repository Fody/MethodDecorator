using System;
using System.Threading.Tasks;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenAsync: ClassTestsBase {
        public WhenAsync() : base("SimpleTest.AsyncClass") {}

        [Fact]
        public void SimpleAsyncMethod() {
            Task x = this.TestClass.SimpleAsyncMethod();
            x.Wait();
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit  });
        }

        [Fact]
        public void AsyncMethodWithResult() {
            Task<int> x = this.TestClass.SimpleAsyncMethodWithResult();
            var res = x.Result;
            Assert.Equal(1, res);
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnExit });
        }

        [Fact]
        public void AsyncMethodWithException() {
            try {
                Task<int> x = this.TestClass.SimpleAsyncMethodWithException();
                var res = x.Result;
            }
            catch (Exception) { }
            
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnContinuation, Method.OnException });
        }
    }
}