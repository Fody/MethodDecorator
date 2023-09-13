namespace SimpleTest.PnP;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
class InterceptorBypassAttribute(bool iBypass) :
    AspectMatchingAttributeBase
{
    public bool DoNeedBypass { get; } = iBypass;

    public bool NeedBypass()
    {
        return DoNeedBypass;
    }
}