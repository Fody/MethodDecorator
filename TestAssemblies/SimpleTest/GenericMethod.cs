namespace SimpleTest
{
    public class GenericMethod
    {
        [Interceptor]
        public T GetValue<T>(T value)
        {
            TestRecords.RecordBody("GenericMethod.GetValue", typeof(T).FullName);
            return value;
        }
    }
}