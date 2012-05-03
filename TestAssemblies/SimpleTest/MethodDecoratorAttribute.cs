using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public abstract class MethodDecoratorAttribute : Attribute
{
    public abstract void OnEntry(MethodBase method);
    public abstract void OnExit(MethodBase method);
    public abstract void OnException(MethodBase method, Exception exception);
}