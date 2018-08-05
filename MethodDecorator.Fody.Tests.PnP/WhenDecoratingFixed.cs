using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using MethodDecorator.Fody.Tests;

namespace MethodDecorator.Fody.Tests.PnP
{
    public class WhenDecoratingFixeds : SimpleTestBase
    {
        [Fact]
        public void ShouldBypassFieldInitCalls()
        {
            dynamic testClass = Assembly.GetType("SimpleTest.PnP.InterceptedMethods", true);
            Assert.NotNull(testClass);

            Activator.CreateInstance(testClass, "Test");

            CheckMethod(Method.Init, new object[] { 11, "parameter", "property", "field" });
        }

        [Fact]
        public void ShouldBypassCtorCalls()
        {
            dynamic testClass = Assembly.GetType("SimpleTest.PnP.InterceptedMethods", true);
            Assert.NotNull(testClass);

            Activator.CreateInstance(testClass, 1);

            CheckMethod(Method.Init, new object[][] {  new object[] { 11, "parameter", "property", "field" },
                                                            new object[] { 12, "parameter", "property", "field" }});
        }

        [Fact]
        public void ShouldFixJumps()
        {
            var testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(13,testClass.SomeLongMethod());

            CheckMethod(Method.Init, new object[] { 0,null, null, null });
        }

        [Fact]
        public void ShouldAllow255Locals()
        {
            var testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.MethodWith255Locals();

            CheckMethod(Method.OnEnter);
            CheckMethod(Method.OnExit, new object[] { 260 });
        }

        [Fact]
        public void ShouldChangePriority()
        {
            var testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.InterceptedWithoutPriorities();
            CheckMethod(Method.Init, new object[][] { new object[] { 1, "Attr2", null, null }, new object[] { "Attr1" , 0, 0 } });
            RecordHost.Clear();

            testClass.InterceptedWithPriorities();
            CheckMethod(Method.Init, new object[][] { new object[] { "Attr1" , -1, 0 }, new object[] { 1, "Attr2", null, null } });
        }

        [Fact]
        public void ShouldPreferLastAttribute()
        {
            var testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.MultipleIntercepted();

            CheckMethod(Method.Init, new object[] { "attr1", 0, 1 });
        }

        [Fact]
        public void ShouldInterceptImplicitCastReturn()
        {
            var testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            IDisposable ret = testClass.InterceptedReturnsImplicitCasted();
            Assert.NotNull(ret);
        }
    }
}
