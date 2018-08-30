using System;
using MethodDecorator.Fody.Interfaces;

namespace SimpleTest.PnP
{
    class AspectMatchingAttributeBase: Attribute, IAspectMatchingRule
    {
        public string AttributeTargetTypes { get; set; }
        public bool AttributeExclude { get; set; }
        public int AttributePriority { get; set; }
        public int AspectPriority { get; set; }
    }
}