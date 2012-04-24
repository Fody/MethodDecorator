using System;

[AttributeUsage(AttributeTargets.Method)]
public abstract class MethodDecoratorAttribute : Attribute
{
    //public abstract bool OnEntry(string fullMethodName, object[] arguments, out object returnValue);
    //public abstract object OnExit(string fullMethodName, object[] arguments, object returnValue);
    //public abstract void OnException(string fullMethodName, object[] arguments, Exception exception);
    public abstract void OnEntry(string fullMethodName);
    public abstract void OnExit(string fullMethodName);
    public abstract void OnException(string fullMethodName, Exception exception);
}