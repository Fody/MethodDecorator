using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using MethodDecoratorEx.Fody.Tests;

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

            this.CheckMethod(Method.Init, new object[] { 11, "parameter", "property", "field" });
        }

        [Fact]
        public void ShouldFixJumps()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(13,testClass.SomeLongMethod());

            this.CheckMethod(Method.Init, new object[] { 0,null, null, null });
        }

        [Fact]
        public void ShouldAllow255Locals()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.MethodWith255Locals();

            this.CheckMethod(Method.OnEnter);
            this.CheckMethod(Method.OnExit, new object[] { 260 });
        }

        [Fact]
        public void ShouldChangePriority()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.InterceptedWithoutPriorities();
            this.CheckMethod(Method.Init, new object[][] { new object[] { 1, "Attr2", null, null }, new object[] { "Attr1" , 0, 0 } });
            this.RecordHost.Clear();

            testClass.InterceptedWithPriorities();
            this.CheckMethod(Method.Init, new object[][] { new object[] { "Attr1" , -1, 0 }, new object[] { 1, "Attr2", null, null } });
        }

        [Fact]
        public void ShouldPreferLastAttribute()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.MultipleIntercepted();

            this.CheckMethod(Method.Init, new object[] { "attr1", 0, 1 });
        }
    }
}
