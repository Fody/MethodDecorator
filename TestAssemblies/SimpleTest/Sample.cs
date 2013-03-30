using System.Diagnostics;

namespace SimpleTest
{
    public class Sample
    {
        [Interceptor]
        public void Method()
        {
            Debug.WriteLine("Your Code");
        }
    }
}
