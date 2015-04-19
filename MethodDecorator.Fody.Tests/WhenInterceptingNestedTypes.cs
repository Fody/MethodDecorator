using System.Reflection;

using MethodDecorator.Fody.Tests;

using Xunit;

namespace MethodDecoratorEx.Fody.Tests {
    public class WhenInterceptingNestedTypes : SimpleTestBase {
        [Fact]
        public void ShouldDecorateMethodInNestedType() {
            dynamic testClass = this.Assembly.GetInstance("SimpleTest.InterceptingNestedTypes+Nested");
            dynamic value = testClass.StringMethod();

            Assert.Equal("sausages", value);

            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnExit });
            this.CheckInit("SimpleTest.InterceptingNestedTypes+Nested", "SimpleTest.InterceptingNestedTypes+Nested.StringMethod");
        }

        [Fact]
        public void ShouldDecorateADeeplyNestedType() {
            dynamic testClass =
                this.Assembly.GetInstance("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested");
            dynamic value = testClass.NumberMethod();

            Assert.Equal(42, value);

            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.OnExit });
            this.CheckInit("SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested", "SimpleTest.InterceptingNestedTypes+FirstLevel+SecondLevel+DeeplyNested.NumberMethod");
        }
    }
}