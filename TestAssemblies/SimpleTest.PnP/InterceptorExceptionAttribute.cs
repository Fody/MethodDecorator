using System;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorExceptionAttribute : AspectMatchingAttributeBase
    {
        public void OnException(Exception iException)
        {
            TestRecords.Record(Method.OnException, new object[] { iException.Message });
        }
    }
}