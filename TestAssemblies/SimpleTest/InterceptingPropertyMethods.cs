using System;

namespace SimpleTest
{
    public class InterceptingPropertyMethods
    {
        int manualProperty;

        public int ManualProperty
        {
            [Interceptor] get { return manualProperty; }

            [Interceptor] set { manualProperty = value; }
        }

        public int ReadOnlyProperty
        {
            [Interceptor] get { return 42; }
        }

        public int ThrowingProperty
        {
            [Interceptor] get { throw new InvalidOperationException("Ooops"); }
        }
    }
}