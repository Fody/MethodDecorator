namespace SimpleTest;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
public class NoInitMethodDecoratorAttribute : Attribute {
    public void OnEntry() {
        TestRecords.RecordOnEntry();
    }

    public void OnExit() {
        TestRecords.RecordOnExit();
    }

    public void OnException(Exception exception) {
        TestRecords.RecordOnException(exception.GetType(), exception.Message);
    }
}