using System;

namespace SimpleTest
{
    public class MarkedWithIndirectAttribute
    {
        [Obsolete]
        public void ObsoleteDecorated()
        {
            TestRecords.RecordBody("ObsoleteDecorated");
        }
    }
}