using System;
using System.Reflection;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorInit1Attribute : AspectMatchingAttributeBase
    {
        public void Init(MethodBase iMethod)
        {
            TestRecords.Record(Method.Init, new object[] { iMethod.Name });
        }
    }
}