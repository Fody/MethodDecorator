namespace SimpleTest
{
    using System;

    public class MarkedWithInderectAttribute
    {
        [Obsolete]
        public void ObsoleteDecorated()
        {
            TestRecords.RecordBody("ObsoleteDecorated");
        }
    }
}