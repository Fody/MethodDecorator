namespace MethodDecorator.Fody.Interfaces
{
    public interface IAspectMatchingRule
    {
        string AttributeTargetTypes { get; set; }
        bool AttributeExclude { get; set; }
        int AttributePriority { get; set; }
        int AspectPriority { get; set; }
    }
}