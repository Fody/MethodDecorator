using System;
using System.Reflection;

namespace SimpleTest.Net2
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
    internal class InterceptorAttribute : Attribute
    {
        public void Init(object instance, MethodBase method, object[] args)
        {
            if (null == method) throw new ArgumentNullException("method");
            TestRecords.RecordInit(instance, method.DeclaringType.FullName + "." + method.Name, args.Length);
        }
        public void OnEntry()
        {
            TestRecords.RecordOnEntry();
        }

        public void OnExit()
        {
            TestRecords.RecordOnExit();
        }

        public void OnException(Exception exception)
        {
            TestRecords.RecordOnException(exception.GetType(), exception.Message);
        }
    }
}
