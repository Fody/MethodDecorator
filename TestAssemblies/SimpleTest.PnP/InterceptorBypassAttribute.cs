using System;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    class InterceptorBypassAttribute : AspectMatchingAttributeBase
    {
        public bool DoNeedBypass { get; }

        public InterceptorBypassAttribute(bool iBypass)
        {
            DoNeedBypass = iBypass;
        }
        public bool NeedBypass()
        {
            return DoNeedBypass;
        }
    }
}