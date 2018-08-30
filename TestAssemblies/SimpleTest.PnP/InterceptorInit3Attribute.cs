using System;
using System.Reflection;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorInit3Attribute : AspectMatchingAttributeBase
    {
        public void Init(object iInstance, MethodBase iMethod, object[] iParams)
        {
            TestRecords.Record(Method.Init, new[] { iInstance, iMethod.Name, iParams});
        }
    }
}
