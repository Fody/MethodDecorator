namespace SimpleTest.DerivedMatchingAssembly
{
    public class DerivedMatchingAssembly
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