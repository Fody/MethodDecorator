using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MethodDecorator.Fody.Tests;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public abstract class TestsBase<T> : IUseFixture<T>
        where T : DecoratedSimpleTest, new() {
        protected Assembly Assembly { get; private set; }
        protected dynamic RecordHost { get; set; }

        protected IList<Tuple<Method, object[]>> Records {
            get {
                var records = (IList<Tuple<int, object[]>>)this.RecordHost.Records;
                return records.Select(x => new Tuple<Method, object[]>((Method)x.Item1, x.Item2)).ToList();
            }
        }
        
        public virtual void SetFixture(T data) {
            this.Assembly = data.Assembly;
            this.RecordHost = data.Assembly.GetStaticInstance("SimpleTest.TestRecords");
            this.RecordHost.Clear();
        }

        protected void CheckMethodSeq(Method[] methods) {
            Assert.Equal(methods, this.Records.Select(x => x.Item1).ToArray());
        }

        protected void CheckException<TEx>(string message) where TEx: Exception {
            var args = this.Records.Single(x => x.Item1 == Method.OnException).Item2;
            Assert.Equal(typeof(TEx), args[0]);
            Assert.Equal(message, args[1]);
        }

        protected void CheckInit(string methodName, int argLength = 0) {
            var args = this.Records.Single(x => x.Item1 == Method.Init).Item2;
            Assert.Equal(methodName, args[0].ToString());
            Assert.Equal(argLength, (int)args[1]);
        }
        protected void CheckBody(string methodName, string extraInfo = null) {
            var args = this.Records.Single(x => x.Item1 == Method.Body).Item2;
            Assert.Equal(methodName, args[0]);
            Assert.Equal(extraInfo,  args[1]);
        }

        protected void CheckEntry() {
            Assert.True(this.Records.Any(x=>x.Item1 == Method.OnEnter));
        }

        protected void CheckExit() {
            Assert.True(this.Records.Any(x => x.Item1 == Method.OnExit));
        }

    }
}