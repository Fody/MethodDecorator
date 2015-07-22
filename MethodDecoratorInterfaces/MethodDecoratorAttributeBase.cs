namespace MethodDecoratorInterfaces
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public class MethodDecoratorAttribute : Attribute, IMethodDecorator
    {
        public virtual void Init(object instance, MethodBase method, object[] args)
        {
        }

        public virtual void OnEntry()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnException(Exception exception)
        {
        }

        public virtual void TaskContinuation(Task task)
        {
        }
    }
}