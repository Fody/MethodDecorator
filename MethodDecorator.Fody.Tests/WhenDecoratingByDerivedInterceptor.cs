using System;
using Xunit;

namespace MethodDecoratorEx.Fody.Tests
{
    public class WhenDecoratingByDerivedInterceptor : SimpleTestBase {
        public WhenDecoratingByDerivedInterceptor() : base("SimpleTest.MarkedFromTheDerivedDecorator") { }

        [Fact]
        public void ShouldNotifyInitEntryAndExit() {
            this.TestClass.CanLogInitEntryAndExit();
            this.CheckInit("SimpleTest.MarkedFromTheDerivedDecorator", "MarkedFromTheDerivedDecorator.CanLogInitEntryAndExit()");
        }

        [Fact]
        public void ShouldNotifyOnInitEntryAndException()
        {
            var ex = Assert.Throws<ApplicationException>(() =>
                {
                    this.TestClass.CanLogInitEntryAndException();
                });

            Assert.Equal("boo!", ex.Message);

            this.CheckInit("SimpleTest.MarkedFromTheDerivedDecorator", "MarkedFromTheDerivedDecorator.CanLogInitEntryAndException()");
            this.CheckException<ApplicationException>("boo!");
        }
    }
}