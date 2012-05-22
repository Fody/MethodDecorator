using System;
using System.Reflection;

using SimpleTest;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public abstract class MethodDecoratorAttribute : Attribute
{
    public abstract void OnEntry(MethodBase method);
    public abstract void OnExit(MethodBase method);
    public abstract void OnException(MethodBase method, Exception exception);
}

public class InterceptorDerivedFromAbstractBaseClassAttribute : MethodDecoratorAttribute
{
    public override void OnEntry(MethodBase method)
    {
        TestMessages.Record(string.Format("OnEntry: {0}", method.DeclaringType.FullName + "." + method.Name));
    }

    public override void OnExit(MethodBase method)
    {
        TestMessages.Record(string.Format("OnExit: {0}", method.DeclaringType.FullName + "." + method.Name));
    }

    public override void OnException(MethodBase method, Exception exception)
    {
        TestMessages.Record(string.Format("OnException: {0} - {1}: {2}", method.DeclaringType.FullName + "." + method.Name, exception.GetType(), exception.Message));
    }
}
