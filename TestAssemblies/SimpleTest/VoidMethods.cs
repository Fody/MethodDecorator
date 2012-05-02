using System;
using System.Collections.Generic;

namespace SimpleTest
{
    public class VoidMethods
    {
        [Interceptor]
        public void WithoutArgs()
        {
            TestMessages.Record("VoidMethodWithoutArgs: Body");
        }

        [Interceptor]
        public void ThrowingInvalidOperationException()
        {
            TestMessages.Record("VoidMethodThrowingInvalidOperationException: Body");
            throw new InvalidOperationException("Ooops");
        }

        [Interceptor]
        public void ConditionallyThrowingInvalidOperationException(bool shouldThrow)
        {
            TestMessages.Record("VoidMethodConditionallyThrowingInvalidOperationException: Body");
            if (shouldThrow)
                throw new InvalidOperationException("Ooops");

            TestMessages.Record("VoidMethodConditionallyThrowingInvalidOperationException: Body2");
        }

        [Interceptor]
        public void WithMultipleReturns(int returnAt)
        {
            // This is compiled such that each return statement essentially becomes
            // a branch to the very last ret statement
            TestMessages.Record("VoidMethodWithMultipleReturns: Body - 0");

            if (returnAt == 1)
                return;

            TestMessages.Record("VoidMethodWithMultipleReturns: Body - 1");

            if (returnAt == 2)
                return;

            TestMessages.Record("VoidMethodWithMultipleReturns: Body - 2");

            if (returnAt == 3)
                return;

            TestMessages.Record("VoidMethodWithMultipleReturns: Body - 3");
        }

        public IList<string> Messages
        {
            get { return TestMessages.Messages; }
        }
    }
}
