using System;

namespace SimpleTest
{
    public class InterceptingPropertyMethods
    {
        private int manualProperty;
        public int ManualProperty
        {
            [Interceptor]
            get { return manualProperty; }

            [Interceptor]
            set { manualProperty = value; }
        }

        public int ReadOnlyProperty
        {
            [Interceptor]
            get { return 42; }
        }

        public int WriteOnlyPropertyField;
        public int WriteOnlyProperty
        {
            [Interceptor]
            set { WriteOnlyPropertyField = value; }
        }

        public int ThrowingProperty
        {
            [Interceptor]
            get { throw new InvalidOperationException("Ooops"); }
        }
    }
}