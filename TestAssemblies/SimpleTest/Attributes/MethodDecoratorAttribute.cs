using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly)]
public abstract class MethodDecoratorAttribute : Attribute
{
    public abstract void Init(object instance, MethodBase method, object[] args);
    public abstract void OnEntry();
    public abstract void OnExit();
    public abstract void OnException(Exception exception);
}