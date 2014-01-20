using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests {
    public class When_decorating_abstract_methods : IUseFixture<DecoratedSimpleTest> {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        public void SetFixture(DecoratedSimpleTest data) {
            this.assembly = data.Assembly;
            this.testClass = this.assembly.GetInstance("SimpleTest.InterceptingAbstractMethods");
            this.testMessages = this.assembly.GetStaticInstance("SimpleTest.TestMessages");
            this.testMessages.Clear();
        }

        [Fact]
        public void Should_not_try_to_decorate_abstract_method() {
            this.testClass.AbstractMethod();

            Assert.Equal(1, this.testMessages.Messages.Count);
            Assert.Contains("InterceptingAbstractMethods.AbstractMethod: Body", this.testMessages.Messages);
        }
    }
}