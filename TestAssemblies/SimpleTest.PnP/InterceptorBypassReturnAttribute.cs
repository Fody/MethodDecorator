namespace SimpleTest.PnP;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
class InterceptorBypassReturnAttribute(object iRetval) :
    AspectMatchingAttributeBase
{
    object AlteredRetval { get; } = iRetval;

    public bool NeedBypass()
    {
        return true;
    }

    public object AlterRetval(object Retval)
    {
        return AlteredRetval;
    }
}