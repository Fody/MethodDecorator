namespace SimpleTest
{
    using AnotherAssemblyAttributeContainer;

    public class MarkedFromAnotherAssembly
    {
        [ExternalInterceptor]
        public void ExternalInterceptorDecorated()
        {
            TestRecords.RecordBody("ExternalInterceptorDecorated");
        }

        [ExternalInterceptionAssemblyLevel]
        public void ExternalInterceptorAssemblyLevelDecorated()
        {
            TestRecords.RecordBody("ExternalInterceptorAssemblyLevelDecorated");
        }
    }
}