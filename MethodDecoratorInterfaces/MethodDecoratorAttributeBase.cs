using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MethodDecorator.Fody.Interfaces
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public class MethodDecoratorAttribute : Attribute, IMethodDecorator {
        public virtual void Init(object instance, MethodBase method, object[] args) {}

        public virtual void OnEntry() {}

        public virtual void OnExit() {}

        public virtual void OnException(Exception exception) {}

        public virtual void OnTaskContinuation(Task task) {}
    }
}