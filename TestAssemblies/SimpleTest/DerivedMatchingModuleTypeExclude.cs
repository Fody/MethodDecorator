namespace SimpleTest.DerivedMatchingModule
{
    [DerivedMatchingDecorator(AttributeExclude = true)]
    public class DerivedMatchingModuleTypeExclude
    {
        public void ExcludeAtTypeLevel()
        {
            TestRecords.RecordBody("ExcludeAtTypeLevel");
        }

        [DerivedMatchingDecorator(AttributeExclude = false)]
        public void ReIncludeAtMethodLevel()
        {
            TestRecords.RecordBody("ReIncludeAtMethodLevel");
        }
    }
}