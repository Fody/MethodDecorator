namespace SimpleTest.PnP;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
class InterceptorAlterRetvalAttribute(object iRetval) :
    AspectMatchingAttributeBase
{
    public object AlteredRetval { get; } = iRetval;

    public object AlterRetval(object Retval)
    {
        return AlteredRetval;
    }
}