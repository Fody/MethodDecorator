using System;

namespace SimpleTest {
    public class MarkedWithInderectAttribute {
        [Obsolete]
        public void ObsoleteDecorated() {
            TestRecords.RecordBody("ObsoleteDecorated");
        }
    }
}