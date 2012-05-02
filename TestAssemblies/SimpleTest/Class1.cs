using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleTest
{
    public class Class1
    {
        [Interceptor]
        public void VoidMethodWithoutArgs()
        {
            TestMessages.Record("VoidMethodWithoutArgs: Body");
        }

        [Interceptor]
        public void VoidMethodThrowingInvalidOperationException()
        {
            TestMessages.Record("VoidMethodThrowingInvalidOperationException: Body");
            throw new InvalidOperationException("Ooops");
        }

        [Interceptor]
        public void VoidMethodConditionallyThrowingInvalidOperationException(bool shouldThrow)
        {
            TestMessages.Record("VoidMethodConditionallyThrowingInvalidOperationException: Body");
            if (shouldThrow)
                throw new InvalidOperationException("Ooops");

            TestMessages.Record("VoidMethodConditionallyThrowingInvalidOperationException: Body2");
        }

        [Interceptor]
        public void VoidMethodWithMultipleReturns(int returnAt)
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

        [Interceptor]
        public void ExpectedVoidMethodWithoutArgs()
        {
            var attribute = (InterceptorAttribute)MethodBase.GetCurrentMethod().GetCustomAttributes(typeof(InterceptorAttribute), false)[0];
            attribute.OnEntry("SimpleTest.Class1.VoidMethodWithoutArgs");
            try
            {
                TestMessages.Record("VoidMethodWithoutArgs: Body");
                attribute.OnExit("SimpleTest.Class1.VoidMethodWithoutArgs");
            }
            catch (Exception __exception)
            {
                attribute.OnException("SimpleTest.Class1.VoidMethodWithoutArgs", __exception);
                throw;
            }
        }

        [Interceptor]
        public void ExpectedVoidMethodThrowingInvalidOperationException()
        {
            var attribute = (InterceptorAttribute)MethodBase.GetCurrentMethod().GetCustomAttributes(typeof(InterceptorAttribute), false)[0];
            attribute.OnEntry("SimpleTest.Class1.VoidMethodThrowingInvalidOperationException");
            try
            {
                TestMessages.Record("VoidMethodThrowingInvalidOperationException: Body");
                throw new InvalidOperationException("Ooops");
            }
            catch (Exception __exception)
            {
                attribute.OnException("SimpleTest.Class1.VoidMethodThrowingInvalidOperationException", __exception);
                throw;
            }
        }

        public IList<string> Messages
        {
            get { return TestMessages.Messages; }
        }
    }
}
