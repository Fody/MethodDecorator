using System;
using System.Collections.Generic;

namespace SimpleTest
{
    public class MethodsReturningValues
    {
        [Interceptor]
        public int ReturnsNumber()
        {
            TestMessages.Record("ReturnsNumber: Body");
            return 42;
        }

        [Interceptor]
        public int Throws()
        {
            throw new InvalidOperationException("Ooops");
        }

        [Interceptor]
        public string ReturnsString()
        {
            TestMessages.Record("ReturnsString: Body");
            return "hello world";
        }

        [Interceptor]
        public int MultipleReturns(int input)
        {
            TestMessages.Record("MultipleReturns: Body - 0");

            if (input == 1)
                return 7;

            TestMessages.Record("MultipleReturns: Body - 1");

            if (input == 2)
                return 14;

            TestMessages.Record("MultipleReturns: Body - 2");

            return input == 3 ? 21 : 28;
        }

        [Interceptor]
        public int MultipleReturnValuesButEndingWithThrow(int returnAt)
        {
            // TODO: No tests for this one!
            if (returnAt == 1)
                return 42;

            if (returnAt == 2)
                return 163;

            throw new InvalidOperationException("Ooops");
        }

        public IList<string> Messages
        {
            get { return TestMessages.Messages; }
        }
    }
}