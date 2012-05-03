using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_abstract_methods : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        [Fact]
        public void Should_not_try_to_decorate_abstract_method()
        {
            testClass.AbstractMethod();

            Assert.Equal(1, testMessages.Messages.Count);
            Assert.Contains("InterceptingAbstractMethods.AbstractMethod: Body", testMessages.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testClass = assembly.GetInstance("SimpleTest.InterceptingAbstractMethods");
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        }
    }
}