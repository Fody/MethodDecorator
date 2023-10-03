namespace SimpleTest;

public class GenericType<T> {
    [Interceptor]
    public T GetValue(T value) {
        TestRecords.RecordBody("GenericType.GetValue", typeof(T).FullName);
        return value;
    }
}