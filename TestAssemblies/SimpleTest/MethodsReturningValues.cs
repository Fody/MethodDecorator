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
        public string ReturnsString()
        {
            TestMessages.Record("ReturnsString: Body");
            return "hello world";
        }

        [Interceptor]
        public DateTime ReturnsDateTime()
        {
            return new DateTime(2012, 4, 1);
        }

        [Interceptor]
        public int Throws()
        {
            throw new InvalidOperationException("Ooops");
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
            TestMessages.Record("MultipleReturnValuesButEndingWithThrow: Body - 0");

            if (returnAt == 1)
                return 42;

            TestMessages.Record("MultipleReturnValuesButEndingWithThrow: Body - 1");

            if (returnAt == 2)
                return 163;

            TestMessages.Record("MultipleReturnValuesButEndingWithThrow: Body - 2");

            throw new InvalidOperationException("Ooops");
        }

        public IList<string> Messages
        {
            get { return TestMessages.Messages; }
        }
    }
}