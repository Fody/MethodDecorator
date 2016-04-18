using System;
using System.Threading.Tasks;

namespace SimpleTest {
    public class AsyncClass {
        [Interceptor]
        public async Task SimpleAsyncMethod() {
            await Task.Delay(1).ConfigureAwait(false);
        }

        [Interceptor]
        public async Task<int> SimpleAsyncMethodWithResult() {
            return await Task.FromResult(1);
        }

        [Interceptor]
        public async Task<int> SimpleAsyncMethodWithException() {
            var res = await Task.FromResult(1);
            throw new Exception("123");
            return res;
        }
    }
}