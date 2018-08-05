using System;

namespace SimpleTest
{
    public class MarkedFromTheDerivedDecorator
    {
        [DerivedDecorator]
        public void CanLogInitEntryAndExit()
        {
            TestRecords.RecordBody("CanLogInitEntryAndExit");
        }

        [DerivedDecorator]
        public void CanLogInitEntryAndException()
        {
            TestRecords.RecordBody("CanLogInitEntryAndException");
            throw new ApplicationException("boo!");
        }
    }
}