using System;

namespace SimpleTest
{
    public class InterceptingConstructors
    {
        public class SimpleConstructor
        {
            [Interceptor]
            public SimpleConstructor()
            {
                TestMessages.Record("InterceptingConstructors+SimpleConstructor: .ctor");
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