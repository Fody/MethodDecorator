using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_using_different_marker_types : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        [Fact]
        public void Should_use_class_that_implements_IMethodDecorator()
        {
            testClass.AttributeImplementsInterface();

            Assert.Equal(3, testMessages.Messages.Count);
            Assert.Contains("OnEntry: SimpleTest.MarkerTypes.AttributeImplementsInterface", testMessages.Messages);
            Assert.Contains("AttributeImplementsInterface: Body", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.MarkerTypes.AttributeImplementsInterface", testMessages.Messages);
        }

        [Fact]
        public void Should_use_class_that_derives_from_IMethodDecorator_implementation()
        {
            testClass.AttributeDerivesFromClassThatImplementsInterface();

            Assert.Equal(3, testMessages.Messages.Count);
            Assert.Contains("OnEntry: SimpleTest.MarkerTypes.AttributeDerivesFromClassThatImplementsInterface", testMessages.Messages);
            Assert.Contains("AttributeDerivesFromClassThatImplementsInterface: Body", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.MarkerTypes.AttributeDerivesFromClassThatImplementsInterface", testMessages.Messages);
        }

        [Fact]
        public void Should_use_class_that_derives_from_abstract_base_class()
        {
            testClass.AttributeDerivesFromMethodDecoratorAttribute();

            Assert.Equal(3, testMessages.Messages.Count);
            Assert.Contains("OnEntry: SimpleTest.MarkerTypes.AttributeDerivesFromMethodDecoratorAttribute", testMessages.Messages);
            Assert.Contains("AttributeDerivesFromMethodDecoratorAttribute: Body", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.MarkerTypes.AttributeDerivesFromMethodDecoratorAttribute", testMessages.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testClass = assembly.GetInstance("SimpleTest.MarkerTypes");
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        }
    }
}