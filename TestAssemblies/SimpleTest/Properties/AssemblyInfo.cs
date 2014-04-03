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

