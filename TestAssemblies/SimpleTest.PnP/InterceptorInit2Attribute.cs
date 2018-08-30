using System;
using System.Reflection;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorInit2Attribute : AspectMatchingAttributeBase
    {
        public void Init(object iInstance,MethodBase iMethod)
        {
            TestRecords.Record(Method.Init, new[] { iInstance, iMethod.Name });
        }
    }
}