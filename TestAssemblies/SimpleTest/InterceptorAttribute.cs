using System;

namespace SimpleTest
{
    public class InterceptorAttribute : MethodDecoratorAttribute
    {
        public override void OnEntry(string fullMethodName)
        {
            TestMessages.Record(string.Format("OnEntry: {0}", fullMethodName));
        }

        public override void OnExit(string fullMethodName)
        {
            TestMessages.Record(string.Format("OnExit: {0}", fullMethodName));
        }

        public override void OnException(string fullMethodName, Exception exception)
        {
            TestMessages.Record(string.Format("OnException: {0} - {1}: {2}", fullMethodName, exception.GetType(), exception.Message));
        }
    }
}