using System;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorExitAttribute :
        AspectMatchingAttributeBase
    {
        public void OnExit()
        {
            TestRecords.Record(Method.OnExit);
        }
    }
}