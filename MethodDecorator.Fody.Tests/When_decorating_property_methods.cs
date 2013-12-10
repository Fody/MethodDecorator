using System;
using System.Reflection;

using Xunit;

namespace MethodDecorator.Fody.Tests
{
    public class When_decorating_property_methods : IUseFixture<DecoratedSimpleTest>
    {
        private Assembly assembly;
        private dynamic testClass;
        private dynamic testMessages;

        [Fact]
        public void Should_notify_on_entry_and_exit_for_manual_property()
        {
            testClass.ManualProperty = 199;
            int value = testClass.ManualProperty;

            Assert.Equal(199, value);

            Assert.Contains("OnEntry: SimpleTest.InterceptingPropertyMethods.set_ManualProperty [1]", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingPropertyMethods.set_ManualProperty", testMessages.Messages);

            Assert.Contains("OnEntry: SimpleTest.InterceptingPropertyMethods.get_ManualProperty", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingPropertyMethods.get_ManualProperty", testMessages.Messages);
        }

        [Fact]
        public void Should_notify_on_entry_and_exit_for_readonly_property_attributed_on_getter()
        {
            int value = testClass.ReadOnlyProperty;

            Assert.Equal(42, value);

            Assert.Contains("OnEntry: SimpleTest.InterceptingPropertyMethods.get_ReadOnlyProperty", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingPropertyMethods.get_ReadOnlyProperty", testMessages.Messages);
        }

        [Fact]
        public void Should_notify_on_entry_and_exit_for_writeonly_property_attributed_on_setter()
        {
            testClass.WriteOnlyProperty = 99;

            Assert.Equal(99, testClass.WriteOnlyPropertyField);

            Assert.Contains("OnEntry: SimpleTest.InterceptingPropertyMethods.set_WriteOnlyProperty [1]", testMessages.Messages);
            Assert.Contains("OnExit: SimpleTest.InterceptingPropertyMethods.set_WriteOnlyProperty", testMessages.Messages);
        }

        [Fact]
        public void Should_notify_on_entry_and_exception_for_property_getter()
        {
            Assert.Throws<InvalidOperationException>(() => testClass.ThrowingProperty);

            Assert.Contains("OnEntry: SimpleTest.InterceptingPropertyMethods.get_ThrowingProperty", testMessages.Messages);
            Assert.Contains("OnException: SimpleTest.InterceptingPropertyMethods.get_ThrowingProperty - System.InvalidOperationException: Ooops", testMessages.Messages);
        }

        public void SetFixture(DecoratedSimpleTest data)
        {
            assembly = data.Assembly;
            testClass = assembly.GetInstance("SimpleTest.InterceptingPropertyMethods");
            testMessages = assembly.GetStaticInstance("SimpleTest.TestMessages");
            testMessages.Clear();
        }
    }
}