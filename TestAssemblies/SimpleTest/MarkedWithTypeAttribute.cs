using System;

namespace SimpleTest.MarkedWithTypeNS {
    public class MarkedWithTypeAttribute {
        public void AppliesToNamespace() {
            TestRecords.RecordBody("AppliesToNamespace");
        }

		[GlobalTypeDecorator(AttributeExclude = true)]
		public void TurnOffAtMethodLevel()
		{
			TestRecords.RecordBody("TurnOffAtMethodLevel");
		}

	}
}