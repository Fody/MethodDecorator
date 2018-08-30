using System;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorExit1ExceptionAttribute : AspectMatchingAttributeBase
    {
        public void OnExit(object iRetval)
        {
            TestRecords.Record(Method.OnExit, new[] { iRetval });
        }
        public void OnException(Exception iException)
        {
            TestRecords.Record(Method.OnException, new object[] { iException.Message });
        }
    }
}