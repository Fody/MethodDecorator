namespace SimpleTest
{
    public class GenericMethod
    {
        [Interceptor] 
        public T GetValue<T>(T value)
        {
            TestMessages.Record(string.Format("GenericMethod.GetValue<{0}> - body", typeof(T)));

            return value;
        }
    }
}