using System;
using System.Reflection;

namespace MethodDecorator.AOP
{
    public interface IMethodDecorator 
    {
        void Init(object instance, MethodBase method, object[] args);
        void OnEntry();
        void OnExit();
        void OnException(Exception exception);
    }

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
    }
}
