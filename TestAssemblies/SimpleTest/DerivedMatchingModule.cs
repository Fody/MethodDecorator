namespace SimpleTest.DerivedMatchingModule
{
    public class DerivedMatchingModule
    {
        public void AppliesToNamespace()
        {
            TestRecords.RecordBody("AppliesToNamespace");
        }

        [DerivedMatchingDecorator(AttributeExclude = true)]
        public void TurnOffAtMethodLevel()
        {
            TestRecords.RecordBody("TurnOffAtMethodLevel");
        }
    }
}