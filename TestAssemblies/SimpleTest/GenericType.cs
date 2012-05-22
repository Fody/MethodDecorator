namespace SimpleTest
{
    public class GenericType<T>
    {
        [Interceptor]
        public T GetValue(T value)
        {
            TestMessages.Record(string.Format("GenericType<{0}>.GetValue - body", typeof(T)));

            return value;
        }
    }
}