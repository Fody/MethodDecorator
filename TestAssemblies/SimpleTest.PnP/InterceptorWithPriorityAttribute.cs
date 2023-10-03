using System.Reflection;

using MethodDecorator.Fody.Interfaces;

namespace SimpleTest.PnP;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, AllowMultiple = true)]
class InterceptorWithPriorityAttribute(string iParam) :
    Attribute,
    IAspectMatchingRule,
    IPartialDecoratorInit1
{
    public string AttributeTargetTypes { get; set; }
    public bool AttributeExclude { get; set; }
    public int AttributePriority { get; set; }
    public int AspectPriority { get; set; }

    public void Init(MethodBase iMethod)
    {
        TestRecords.Record(Method.Init, new object[] { iParam , AspectPriority, AttributePriority});
    }
}