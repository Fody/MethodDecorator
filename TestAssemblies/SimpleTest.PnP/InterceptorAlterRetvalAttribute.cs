using System;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorAlterRetvalAttribute : AspectMatchingAttributeBase
    {
        public object AlteredRetval { get; }

        public InterceptorAlterRetvalAttribute(object iRetval)
        {
            AlteredRetval = iRetval;
        }
        public object AlterRetval(object Retval)
        {
            return AlteredRetval;
        }
    }
}