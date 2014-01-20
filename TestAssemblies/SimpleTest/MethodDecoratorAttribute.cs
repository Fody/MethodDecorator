using System;
using System.Reflection;

using SimpleTest;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public abstract class MethodDecoratorAttribute : Attribute {
    public abstract void Init(MethodBase method, object[] args);
    public abstract void OnEntry();
    public abstract void OnExit();
    public abstract void OnException(Exception exception);
}

public class InterceptorDerivedFromAbstractBaseClassAttribute : MethodDecoratorAttribute {
    public override void Init(MethodBase method, object[] args) {
        TestMessages.Record(string.Format("Init: {0} [{1}]", method.DeclaringType.FullName + "." + method.Name, args.Length));
    }
    public override void OnEntry() {
        TestMessages.Record("OnEntry");
    }

    public override void OnExit() {
        TestMessages.Record("OnExit");
    }

    public override void OnException(Exception exception) {
        TestMessages.Record(string.Format("OnException: {0}: {1}", exception.GetType(), exception.Message));
    }
}
