using System;
using System.Reflection;

namespace SimpleTest
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public class InterceptorAttribute : Attribute, IMethodDecorator
    {
        public void OnEntry(MethodBase method)
        {
            TestMessages.Record(string.Format("OnEntry: {0}", method.DeclaringType.FullName + "." + method.Name));
        }

        public void OnExit(MethodBase method)
        {
            TestMessages.Record(string.Format("OnExit: {0}", method.DeclaringType.FullName + "." + method.Name));
        }

        public void OnException(MethodBase method, Exception exception)
        {
            TestMessages.Record(string.Format("OnException: {0} - {1}: {2}", method.DeclaringType.FullName + "." + method.Name, exception.GetType(), exception.Message));
        }
    }

    public class InterceptorDerivedFromInterface : InterceptorAttribute
    {
    }
}