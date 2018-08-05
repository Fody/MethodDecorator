using System;

namespace SimpleTest
{
    public class MarkedFromTheDerivedInterface
    {
        [DerivedFromInterfaceDecorator]
        public void CanLogInitEntryAndExit(string text)
        {
            TestRecords.RecordBody("CanLogInitEntryAndExit");
        }

        [DerivedFromInterfaceDecorator]
        public void CanLogInitEntryAndException()
        {
            TestRecords.RecordBody("CanLogInitEntryAndException");
            throw new ApplicationException("boo!");
        }
    }
}