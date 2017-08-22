using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using MethodDecorator.Fody.Interfaces;

namespace SimpleTest.PnP
{
    internal class AspectMatchingAttributeBase: Attribute, IAspectMatchingRule
    {
        public string AttributeTargetTypes { get; set; }
        public bool AttributeExclude { get; set; }
        public int AttributePriority { get; set; }
        public int AspectPriority { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    internal class InterceptorInit1Attribute : AspectMatchingAttributeBase
    {
        public void Init(MethodBase iMethod)
        {
            TestRecords.Record(Method.Init, new object[] { iMethod.Name });
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    internal class InterceptorInit2Attribute : AspectMatchingAttributeBase
    {
        public void Init(object iInstance,MethodBase iMethod)
        {
            TestRecords.Record(Method.Init, new object[] { iInstance, iMethod.Name });
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    internal class InterceptorInit3Attribute : AspectMatchingAttributeBase
    {
        public void Init(object iInstance, MethodBase iMethod, object[] iParams)
        {
            TestRecords.Record(Method.Init, new object[] { iInstance, iMethod.Name, iParams});
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    internal class InterceptorEntryAttribute : AspectMatchingAttributeBase
    {
        public void OnEntry()
        {
            TestRecords.Record(Method.OnEnter, null);
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    internal class InterceptorExitAttribute : AspectMatchingAttributeBase
    {
        public void OnExit()
        {
            TestRecords.Record(Method.OnExit, null);
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    internal class InterceptorExit1Attribute : AspectMatchingAttributeBase
    {
        public void OnExit(object iRetval)
        {
            TestRecords.Record(Method.OnExit, new object[] { iRetval });
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    internal class InterceptorExceptionAttribute : AspectMatchingAttributeBase
    {
        public void OnException(Exception iException)
        {
            TestRecords.Record(Method.OnException, new object[] { iException.Message });
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    internal class InterceptorExit1ExceptionAttribute : AspectMatchingAttributeBase
    {
        public void OnExit(object iRetval)
        {
            TestRecords.Record(Method.OnExit, new object[] { iRetval });
        }
        public void OnException(Exception iException)
        {
            TestRecords.Record(Method.OnException, new object[] { iException.Message });
        }
    }
}
