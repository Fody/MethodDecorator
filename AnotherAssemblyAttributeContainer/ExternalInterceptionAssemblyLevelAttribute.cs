using System;
using System.Reflection;
using SimpleTest;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly)]
public class ExternalInterceptionAssemblyLevelAttribute : Attribute
{
    public void Init(object instance, MethodBase method, object[] args)
    {
        TestRecords.RecordInit(instance, method.DeclaringType.FullName + "." + method.Name, args.Length);
    }

    public void OnEntry()
    {
        TestRecords.RecordOnEntry();
    }

    public void OnExit()
    {
        TestRecords.RecordOnExit();
    }

    public void OnException(Exception exception)
    {
        TestRecords.RecordOnException(exception.GetType(), exception.Message);
    }
}