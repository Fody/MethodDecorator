using System;
using System.Reflection;

using MethodDecorator.Fody.Interfaces;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, AllowMultiple = true)]
    class InterceptorWithPriorityAttribute : Attribute, IAspectMatchingRule, IPartialDecoratorInit1
    {
        string _Param;

        public string AttributeTargetTypes { get; set; }
        public bool AttributeExclude { get; set; }
        public int AttributePriority { get; set; }
        public int AspectPriority { get; set; }

        public InterceptorWithPriorityAttribute(string iParam) { _Param = iParam; }

        public void Init(MethodBase iMethod)
        {
            TestRecords.Record(Method.Init, new object[] { _Param , AspectPriority, AttributePriority});
        }
    }
}
