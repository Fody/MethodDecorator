using System;
using System.Reflection;

using AnotherAssemblyAttributeContainer;

using SimpleTest;

[assembly: AssemblyTitle("SimpleTest")]

[module: IntersectMethodsMarkedByAttribute(typeof(ObsoleteAttribute))]
[module: NoInitMethodDecorator]
[module: Interceptor]
[module: ExternalInterceptor]
[assembly: ExternalInterceptionAssemblyLevel]

// IAspectMatchingRule-based decorators
[assembly: DerivedMatchingDecorator(AttributeTargetTypes="SimpleTest.DerivedMatchingAssembly.*")]
[module: DerivedMatchingDecorator(AttributeTargetTypes = "SimpleTest.DerivedMatchingModule.*")]
[module: DerivedMatchingDecorator(AttributeTargetTypes = @"regex:^SimpleTest\.MatchingByRegex\..*Include$")]

[module: DerivedMatchingDecorator(AttributeTargetTypes = 
	"SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedA.*, " 
	+ "SimpleTest.MatchingCommaSeparated.MatchingCommaSeparatedB.*")]
