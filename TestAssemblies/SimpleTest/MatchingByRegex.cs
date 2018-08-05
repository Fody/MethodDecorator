namespace SimpleTest.MatchingByRegex
{
    public class MatchingByRegex
    {
        public void MethodMatchInclude()
        {
            TestRecords.RecordBody("MethodMatchInclude");
        }

        public void MethodMatchExclude()
        {
            TestRecords.RecordBody("MethodMatchExclude");
        }

        public string PropertyGetInclude
        {
            get
            {
                TestRecords.RecordBody("PropertyGetInclude");
                return "";
            }
        }

        public string PropertyGetExclude
        {
            get
            {
                TestRecords.RecordBody("PropertyGetExclude");
                return "";
            }
        }
    }
}