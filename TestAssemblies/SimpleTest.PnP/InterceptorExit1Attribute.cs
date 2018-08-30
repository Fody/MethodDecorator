using System;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorExit1Attribute : AspectMatchingAttributeBase
    {
        public void OnExit(object iRetval)
        {
            TestRecords.Record(Method.OnExit, new[] { iRetval });
        }
    }
}