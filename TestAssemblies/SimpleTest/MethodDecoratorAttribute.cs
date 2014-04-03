using System;
using System.Reflection;

using SimpleTest;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly)]
public abstract class MethodDecoratorAttribute : Attribute
{
    public abstract void Init(object instance, MethodBase method, object[] args);
    public abstract void OnEntry();
    public abstract void OnExit();
    public abstract void OnException(Exception exception);
}

public class InterceptorDerivedFromAbstractBaseClassAttribute : MethodDecoratorAttribute {
    public override void Init(object instance, MethodBase method, object[] args) {
        TestRecords.RecordInit(instance, method.DeclaringType.FullName + "." + method.Name, args.Length);
    }
    public override void OnEntry() {
        TestRecords.RecordOnEntry();
    }

    public override void OnExit() {
        TestRecords.RecordOnExit();
    }

    public override void OnException(Exception exception) {
        TestRecords.RecordOnException(exception.GetType(), exception.Message);
    }
}