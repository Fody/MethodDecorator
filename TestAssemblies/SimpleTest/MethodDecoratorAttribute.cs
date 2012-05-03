using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
public abstract class MethodDecoratorAttribute : Attribute
{
    public abstract void OnEntry(string fullMethodName);
    public abstract void OnExit(string fullMethodName);
    public abstract void OnException(string fullMethodName, Exception exception);
}