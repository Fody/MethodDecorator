using System;
using System.Reflection;

using AnotherAssemblyAttributeContainer;

using SimpleTest;
using MethodDecoratorInterfaces;

[assembly: AssemblyTitle("SimpleTest")]

[module: IntersectMethodsMarkedByAttribute(typeof(ObsoleteAttribute))]
[module: NoInitMethodDecorator]
[module: Interceptor]
[module: ExternalInterceptor]
[assembly: ExternalInterceptionAssemblyLevel]

[assembly: GlobalTypeDecorator(AttributeTargetTypes="SimpleTest.MarkedWithTypeNS.*")]