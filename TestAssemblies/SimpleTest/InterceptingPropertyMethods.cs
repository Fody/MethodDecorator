namespace SimpleTest
{
    using System;

    public class InterceptingPropertyMethods
    {
        public int ManualProperty { [Interceptor] get; [Interceptor] set; }

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