using System;

namespace SimpleTest
{
    public class InterceptingVoidMethods
    {
        [Interceptor]
        public void WithoutArgs()
        {
            TestRecords.RecordBody("VoidMethodWithoutArgs");
        }

        [Interceptor]
        public void ThrowingInvalidOperationException()
        {
            TestRecords.RecordBody("VoidMethodThrowingInvalidOperationException");
            throw new InvalidOperationException("Ooops");
        }

        [Interceptor]
        public void ConditionallyThrowingInvalidOperationException(bool shouldThrow)
        {
            TestRecords.RecordBody("VoidMethodConditionallyThrowingInvalidOperationException", "enter");
            if (shouldThrow)
                throw new InvalidOperationException("Ooops");

            TestRecords.RecordBody("VoidMethodConditionallyThrowingInvalidOperationException", "not throw");
        }

        [Interceptor]
        public void WithMultipleReturns(int returnAt)
        {
            // This is compiled such that each return statement essentially becomes
            // a branch to the very last ret statement
            TestRecords.RecordBody("VoidMethodWithMultipleReturns", "0");
            if (returnAt == 1) return;
            TestRecords.RecordBody("VoidMethodWithMultipleReturns", "1");
            if (returnAt == 2) return;
            TestRecords.RecordBody("VoidMethodWithMultipleReturns", "2");
            if (returnAt == 3) return;
            TestRecords.RecordBody("VoidMethodWithMultipleReturns", "3");
        }

        [Interceptor]
        public void WithMultipleReturnsAndExceptions(int actAt, bool shouldThrow)
        {
            TestRecords.RecordBody("WithMultipleReturnsAndExceptions", "0");

            if (actAt == 1)
            {
                if (shouldThrow)
                    throw new InvalidOperationException("Throwing at 1");
                return;
            }

            TestRecords.RecordBody("WithMultipleReturnsAndExceptions", "1");

            if (actAt == 2)
            {
                if (shouldThrow)
                    throw new InvalidOperationException("Throwing at 2");
                return;
            }

            TestRecords.RecordBody("WithMultipleReturnsAndExceptions", "2");

            if (actAt == 3)
            {
                if (shouldThrow)
                    throw new InvalidOperationException("Throwing at 3");
                return;
            }

            TestRecords.RecordBody("WithMultipleReturnsAndExceptions", "3");
        }

        [Interceptor]
        public void MultipleReturnValuesButEndingWithThrow(int returnAt)
        {
            TestRecords.RecordBody("MultipleReturnValuesButEndingWithThrow", "0");
            if (returnAt == 1) return;
            TestRecords.RecordBody("MultipleReturnValuesButEndingWithThrow", "1");
            if (returnAt == 2) return;
            TestRecords.RecordBody("MultipleReturnValuesButEndingWithThrow", "2");
            throw new InvalidOperationException("Ooops");
        }
    }
}