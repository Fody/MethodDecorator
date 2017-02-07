using System;
using System.Reflection;

namespace MethodDecorator.Fody.Interfaces
{
    public interface IMethodDecorator {
        void Init(object instance, MethodBase method, object[] args);
        void OnEntry();
        void OnExit();
        void OnException(Exception exception);
        // Optional
        //public virtual void OnTaskContinuation(Task task) {}
    }
}