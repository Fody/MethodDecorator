using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using MethodDecorator.Fody.Tests;
using Xunit;

namespace MethodDecorator.Fody.Tests.PnP
{
    public class SimpleTestBase : TestsBase
    {
        private static readonly Assembly _assembly  = CreateAssembly();

        protected override Assembly Assembly
        {
            get {
                return _assembly;
            }
        }

        public SimpleTestBase()
        {
            Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords").Clear();
        }
        protected override dynamic RecordHost
        {
            get { return this.Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords"); }
        }

        private static Assembly CreateAssembly()
        {
            var weaverHelper = new WeaverHelper(@"SimpleTest.PnP\SimpleTest.PnP.csproj");
            return weaverHelper.Weave();
        }

        protected void CheckMethod(Method iMethod)
        {
            var record = this.Records.SingleOrDefault(x => x.Item1 == iMethod);
            Assert.NotNull(record);
            Assert.Null(record.Item2);
        }
        protected void CheckMethod(Method iMethod, object[] iParams)
        {
            var record = this.Records.SingleOrDefault(x => x.Item1 == iMethod);
            Assert.NotNull(record);
            Assert.Equal(iParams,record.Item2);
        }
        protected void CheckMethod(Method iMethod, object[][] iParams)
        {
            var records = this.Records.Where(x => x.Item1 == iMethod).Select( x => x.Item2);
            Assert.NotEmpty(records);
            Assert.Equal(iParams,records);
        }
    }
}
