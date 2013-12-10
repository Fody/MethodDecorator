using System;
using System.Reflection;

public interface IMethodDecorator {
    void OnEntry(MethodBase method, object[] args);
    void OnExit(MethodBase method);
    void OnException(MethodBase method, Exception exception);
}