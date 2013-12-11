using AnotherAssemblyAttributeContainer;

namespace SimpleTest
{
    public class MarkedFromAnotherAssembly
    {
        [ExternalInterceptor]
        public void ExternalInterceptorDecorated()
        {
            TestMessages.Record("ExternalInterceptorDecorated: Body");
        }
    }
}