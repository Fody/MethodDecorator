using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using MethodDecoratorEx.Fody.Tests;

namespace MethodDecorator.Fody.Tests.PnP
{
    public class WhenDecoratingWithParameters: SimpleTestBase
    {

        [Fact]
        public void ShouldReportInitWithAttrParameters()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.ExplicitIntercepted();

            this.CheckMethod(Method.Init, new object[] { 15, "parameter", "property", "field" });
        }

        [Fact]
        public void ShouldNotAffectNext()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.ExplicitIntercepted();
            testClass.ExplicitIntercepted();

            this.CheckMethod(Method.Init,
                new object[][] { new object[] { 15, "parameter", "property", "field" }, new object[] { 15, "parameter", "property", "field" } });

            this.CheckMethod(Method.OnExit,
                new object[][] { new object[] { 16, "parameter", "property", "field" }, new object[] { 16, "parameter", "property", "field" } });
        }

        [Fact]
        public void ShouldNotAffectInnerMethods()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.OuterMethod();

            this.CheckMethod(Method.Init,  
                new object[][] {  new object[] { 1, "parameter", "property", "field" }, new object[] { 1, "parameter", "property", "field" } });

            this.CheckMethod(Method.OnExit,
                new object[][] { new object[] { 2, "parameter", "property", "field" }, new object[] { 2, "parameter", "property", "field" } });
        }

        [Fact]
        public void ShouldImplicitIntercept()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedClass");
            Assert.NotNull(testClass);

            testClass.ImplicitIntercepted();

            this.CheckMethod(Method.Init, new object[] { 1, "class_parameter", "class_property", "class_field" });
        }

        [Fact]
        public void ShouldPreferExplicitIntercept()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedClass");
            Assert.NotNull(testClass);

            testClass.ExplicitIntercepted();

            this.CheckMethod(Method.Init, new object[] { 10, "method_parameter", "method_property", "method_field" });
        }

        [Fact]
        public void ShouldInterceptRetval()
        {
            dynamic testClass = Assembly.GetInstance("SimpleTest.PnP.InterceptedMethods");
            Assert.NotNull(testClass);

            testClass.InterceptedReturns10();
            testClass.InterceptedReturnsString();
            testClass.InterceptedReturnsType();

            this.CheckMethod(Method.OnExit, new object[][] { new object[] { 10 }, new object[] { "Intercepted" }, new object[] { testClass.GetType() } });
        }
    }
}
