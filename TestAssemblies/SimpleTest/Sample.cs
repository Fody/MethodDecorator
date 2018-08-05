using System;
using System.Diagnostics;

namespace SimpleTest
{
    public class Sample
    {
        public void main()
        {
            var i = (IntersectMethodsMarkedByAttribute) Activator.CreateInstance(typeof(IntersectMethodsMarkedByAttribute));
        }

        [Interceptor]
        public void Method()
        {
            Debug.WriteLine("Your Code");
        }
    }
}