namespace SimpleTest
{
    using System;

    public class InterceptingConstructors
    {
        public class SimpleConstructor
        {
            [Interceptor]
            public SimpleConstructor()
            {
                TestRecords.RecordBody("InterceptingConstructors+SimpleConstructor");
            }
        }

        public class ThrowingConstructor
        {
            [Interceptor]
            public ThrowingConstructor()
            {
                throw new InvalidOperationException("Ooops");
            }
        }
    }
}