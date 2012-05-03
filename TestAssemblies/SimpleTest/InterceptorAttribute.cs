using System;
using System.Reflection;

namespace SimpleTest
{
    public class InterceptorAttribute : MethodDecoratorAttribute
    {
        public override void OnEntry(MethodBase method)
        {
            TestMessages.Record(string.Format("OnEntry: {0}", method.DeclaringType.FullName + "." + method.Name));
        }

        public override void OnExit(MethodBase method)
        {
            TestMessages.Record(string.Format("OnExit: {0}", method.DeclaringType.FullName + "." + method.Name));
        }

        public override void OnException(MethodBase method, Exception exception)
        {
            TestMessages.Record(string.Format("OnException: {0} - {1}: {2}", method.DeclaringType.FullName + "." + method.Name, exception.GetType(), exception.Message));
        }
    }
}