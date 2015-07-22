namespace SimpleTest
{
    public class MarkedWithNoInit
    {
        [NoInitMethodDecorator]
        public void NoInitMethodDecorated()
        {
            TestRecords.RecordBody("NoInitMethodDecorated");
        }
    }
}