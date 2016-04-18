using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public abstract class TestsBase : IDisposable {
        private readonly static object _sync = new object();

        protected abstract Assembly Assembly { get; }

        protected abstract dynamic RecordHost { get; }

        protected IList<Tuple<Method, object[]>> Records {
            get {
#if NET2
                var records = (IList<object[]>)this.RecordHost.Records;
                return records.Select(x => new Tuple<Method, object[]>((Method)x[0], (object[])x[1])).ToList();
#else
                var records = (IList<Tuple<int, object[]>>)this.RecordHost.Records;
                return records.Select(x => new Tuple<Method, object[]>((Method)x.Item1, x.Item2)).ToList();
#endif
            }
        }

        protected TestsBase() {
            // almost global lock to prevent parrllel test run because we use static (
            Monitor.Enter(_sync);
        }

        protected void CheckMethodSeq(Method[] methods) {
            var coll = this.Records.Select(x => x.Item1).ToArray();
            Assert.Equal(methods, coll);
        }

        protected void CheckException<TEx>(string message) where TEx: Exception {
            var args = GetRecordOfCallTo(Method.OnException).Item2;
            Assert.Equal(typeof(TEx), args[0]);
            Assert.Equal(message, args[1]);
        }

        protected void CheckInit(string instanceTypeName, string methodName, int argLength = 0)
        {
            var args = GetRecordOfCallTo(Method.Init).Item2;
            Assert.Equal(instanceTypeName, args[0] == null ? null : args[0].ToString());
            Assert.Equal(methodName, args[1].ToString());
            Assert.Equal(argLength, (int)args[2]);
        }

        private Tuple<Method, object[]> GetRecordOfCallTo(Method method)
        {
            var record = this.Records.SingleOrDefault(x => x.Item1 == method);
            if (record == null)
            {
                throw new InvalidOperationException(method+" was not called.");
            }
            return record;
        }

        protected void CheckBody(string methodName, string extraInfo = null) {
            Assert.True(this.Records.Any(x => x.Item1 == Method.Body &&
                                              x.Item2[0] == methodName &&
                                              x.Item2[1] == extraInfo));
        }

        protected void CheckEntry() {
            Assert.True(this.Records.Any(x=>x.Item1 == Method.OnEnter));
        }

        protected void CheckExit() {
            Assert.True(this.Records.Any(x => x.Item1 == Method.OnExit));
        }

        public void Dispose() {
            Monitor.Exit(_sync);
        }
    }
}