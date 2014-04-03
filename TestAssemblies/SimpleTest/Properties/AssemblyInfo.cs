using System.Reflection;

using AnotherAssemblyAttributeContainer;

using SimpleTest;

[assembly: AssemblyTitle("SimpleTest")]

[module: NoInitMethodDecorator]
[module: Interceptor]
[module: ExternalInterceptor]
[assembly: ExternalInterceptionAssemblyLevel]

