using System;
using Xunit;

namespace MethodDecoratorEx.Fody.Tests
{
    public class WhenDecoratingByDerivedFromInterface : ClassTestsBase {
        public WhenDecoratingByDerivedFromInterface() : base("SimpleTest.MarkedFromTheDerivedInterface") { }

        [Fact]
        public void ShouldNotifyInitEntryAndExit() {
            this.TestClass.CanLogInitEntryAndExit("something");
            this.CheckInit("SimpleTest.MarkedFromTheDerivedInterface", "MarkedFromTheDerivedInterface.CanLogInitEntryAndExit(String)", 1);
        }

        [Fact]
        public void ShouldNotifyOnInitEntryAndException()
        {
            var ex = Assert.Throws<ApplicationException>(() =>
                {
                    this.TestClass.CanLogInitEntryAndException();
                });

            Assert.Equal("boo!", ex.Message);

            this.CheckInit("SimpleTest.MarkedFromTheDerivedInterface", "MarkedFromTheDerivedInterface.CanLogInitEntryAndException()");
            this.CheckException<ApplicationException>("boo!");
        }
    }
}