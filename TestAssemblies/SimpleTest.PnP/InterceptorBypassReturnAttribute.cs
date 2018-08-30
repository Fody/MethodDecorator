using System;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorBypassReturnAttribute : AspectMatchingAttributeBase
    {
        object AlteredRetval { get; }

        public InterceptorBypassReturnAttribute(object iRetval)
        {
            AlteredRetval = iRetval;
        }

        public bool NeedBypass()
        {
            return true;
        }

        public object AlterRetval(object Retval)
        {
            return AlteredRetval;
        }
    }
}