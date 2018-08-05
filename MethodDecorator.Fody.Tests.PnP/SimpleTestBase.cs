using System.Linq;
using System.Reflection;
using Xunit;

namespace MethodDecorator.Fody.Tests.PnP
{
    public class SimpleTestBase : TestsBase
    {
        private static readonly Assembly _assembly = CreateAssembly();

        protected override Assembly Assembly
        {
            get { return _assembly; }
        }

        public SimpleTestBase()
        {
            Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords").Clear();
        }

        protected override dynamic RecordHost
        {
            get { return Assembly.GetStaticInstance("SimpleTest.PnP.TestRecords"); }
        }

        private static Assembly CreateAssembly()
        {
            var weaverHelper = new WeaverHelper(@"SimpleTest.PnP\SimpleTest.PnP.csproj");
            return weaverHelper.Weave();
        }

        protected void CheckMethod(Method iMethod)
        {
            var record = Records.SingleOrDefault(x => x.Item1 == iMethod);
            Assert.NotNull(record);
            Assert.Null(record.Item2);
        }

        protected void CheckMethod(Method iMethod, object[] iParams)
        {
            var record = Records.SingleOrDefault(x => x.Item1 == iMethod);
            Assert.NotNull(record);
            Assert.Equal(iParams, record.Item2);
        }

        protected void CheckMethod(Method iMethod, object[][] iParams)
        {
            var records = Records.Where(x => x.Item1 == iMethod).Select(x => x.Item2);
            Assert.NotEmpty(records);
            Assert.Equal(iParams, records);
        }
    }
}