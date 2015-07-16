using System;
using System.Reflection;
using SimpleTest;

public class InterceptorDerivedFromAbstractBaseClassAttribute : MethodDecoratorAttribute
{
    public override void Init(object instance, MethodBase method, object[] args)
    {
        if (null == method) throw new ArgumentNullException("method");
        if (null == instance) throw new ArgumentNullException("instance");
        TestRecords.RecordInit(instance, method.DeclaringType.FullName + "." + method.Name, args.Length);
    }

    public override void OnEntry()
    {
        TestRecords.RecordOnEntry();
    }

    public override void OnExit()
    {
        TestRecords.RecordOnExit();
    }

    public override void OnException(Exception exception)
    {
        TestRecords.RecordOnException(exception.GetType(), exception.Message);
    }
}