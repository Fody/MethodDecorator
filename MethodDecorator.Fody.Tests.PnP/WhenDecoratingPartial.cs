using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using MethodDecorator.Fody.Tests;

namespace MethodDecorator.Fody.Tests.PnP
{
    public class WhenDecoratingPartial : SimpleTestBase
    {
        [Fact]
        public void ShouldInterceptInit1()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(2, testClass.InterceptedInit1(1));

            this.CheckMethod(Method.Init, new object[] { "InterceptedInit1" });
        }
        [Fact]
        public void ShouldInterceptInit2()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(2, testClass.InterceptedInit2(1));

            this.CheckMethod(Method.Init, new object[] { testClass, "InterceptedInit2" });
        }

        [Fact]
        public void ShouldInterceptInit3()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(2, testClass.InterceptedInit3(1));

            this.CheckMethod(Method.Init, new object[] { testClass, "InterceptedInit3", new object[] { 1 } });
        }
        [Fact]
        public void ShouldInterceptEntry()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(2, testClass.InterceptedEntry(1));

            this.CheckMethod(Method.OnEnter);
        }
        [Fact]
        public void ShouldInterceptExit()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(2, testClass.InterceptedExit(1));

            this.CheckMethod(Method.OnExit);
        }
        [Fact]
        public void ShouldInterceptExit1()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(2, testClass.InterceptedExit1(1));

            this.CheckMethod(Method.OnExit, new object[] { 2 });
        }
        [Fact]
        public void ShouldInterceptException()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            try {
                testClass.InterceptedException(1);
            } catch (Exception e)
            {
                Assert.Equal(e.Message, "test");
            }
            this.CheckMethod(Method.OnException, new object[] { "test" });
        }
        [Fact]
        public void ShouldInterceptExceptionExit1()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            try
            {
                testClass.InterceptedExit1Exception(0);
            }
            catch (Exception e)
            {
                Assert.Equal(e.Message, "test");
            }

            Assert.Equal(2, testClass.InterceptedExit1Exception(1));

            this.CheckMethod(Method.OnExit, new object[] { 2 });
            this.CheckMethod(Method.OnException, new object[] { "test" });
        }
        
        [Fact]
        public void ShouldBypassMethod()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.BypassedMethod();

            this.CheckMethodSeq(new Method[] { });
        }

        [Fact]
        public void ShouldNotBypassMethod()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.NotBypassedMethod();

            this.CheckMethod(Method.Body);
        }

        [Fact]
        public void ShouldBypassBoolMethod()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.NotEqual(testClass.BypassedMethodRetTrue(),true);

            this.CheckMethodSeq(new Method[] { });
        }

        [Fact]
        public void ShouldAlterString()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(testClass.AlteredMethodString(), "altered");

            this.CheckMethod(Method.Body);
        }

        [Fact]
        public void ShouldAlterInt()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(testClass.AlteredMethodInt(), 2);

            this.CheckMethod(Method.Body);
        }

        [Fact]
        public void ShouldAlterBypassString()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(testClass.AlteredBypassedMethodString(), "altered");

            this.CheckMethodSeq(new Method[] { });
        }

        [Fact]
        public void ShouldAlterBypassInt()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            Assert.Equal(testClass.AlteredBypassedMethodInt(), 2);

            this.CheckMethodSeq(new Method[] { });
        }

        [Fact]
        public void ShouldAlterBypassVoid()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.AlteredBypassedMethodVoid();

            this.CheckMethodSeq(new Method[] { });
        }
    }
}
