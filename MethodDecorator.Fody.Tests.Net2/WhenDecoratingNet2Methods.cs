using MethodDecorator.Fody.Tests;
using Xunit;

namespace MethodDecoratorEx.Fody.Tests
{
    public class WhenDecoratingNet2Methods : SimpleTestBase {

        [Fact]
        public void ShouldReportOnEntryAndExit() {
            dynamic testClass = Assembly.GetInstance("SimpleTest.Net2.InterceptingMethods");
            Assert.NotNull(testClass);

            int value = testClass.ReturnsNumber();

            this.CheckInit("SimpleTest.Net2.InterceptingMethods", "SimpleTest.Net2.InterceptingMethods.ReturnsNumber");
            this.CheckMethodSeq(new[] { Method.Init, Method.OnEnter, Method.Body, Method.OnExit });
        }
    }
}
