using System;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorEntryAttribute :
        AspectMatchingAttributeBase
    {
        public void OnEntry()
        {
            TestRecords.Record(Method.OnEnter);
        }
    }
}