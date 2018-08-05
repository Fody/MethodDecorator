using System;

namespace SimpleTest
{
    public class InterceptingMethodsWithReturnValues
    {
        [Interceptor]
        public int ReturnsNumber()
        {
            TestRecords.RecordBody("ReturnsNumber");
            return 42;
        }

        [Interceptor]
        public string ReturnsString()
        {
            TestRecords.RecordBody("ReturnsString");
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
            TestRecords.RecordBody("MultipleReturns", "0");

            if (input == 1)
                return 7;

            TestRecords.RecordBody("MultipleReturns", "1");

            if (input == 2)
                return 14;

            TestRecords.RecordBody("MultipleReturns", "2");

            return input == 3 ? 21 : 28;
        }

        [Interceptor]
        public int MultipleReturnValuesButEndingWithThrow(int returnAt)
        {
            TestRecords.RecordBody("MultipleReturnValuesButEndingWithThrow", "0");

            if (returnAt == 1)
                return 42;

            TestRecords.RecordBody("MultipleReturnValuesButEndingWithThrow", "1");

            if (returnAt == 2)
                return 163;

            TestRecords.RecordBody("MultipleReturnValuesButEndingWithThrow", "2");

            throw new InvalidOperationException("Ooops");
        }

        [Interceptor]
        public int MultipleReturnValuesButWithEmbeddedThrow(int returnAt)
        {
            TestRecords.RecordBody("MultipleReturnValuesButWithEmbeddedThrow", "0");

            if (returnAt == 1)
                return 42;

            TestRecords.RecordBody("MultipleReturnValuesButWithEmbeddedThrow", "1");

            if (returnAt == 2)
                throw new InvalidOperationException("Ooops");

            TestRecords.RecordBody("MultipleReturnValuesButWithEmbeddedThrow", "2");
            TestRecords.RecordBody("MultipleReturnValuesButWithEmbeddedThrow", "2");

            return 164;
        }
    }
}