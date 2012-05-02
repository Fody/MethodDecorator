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
            throw new InvalidOperationException("Ooops");
        }

        [Interceptor]
        public void ExpectedVoidMethodWithoutArgs()
        {
            //var attribute = (InterceptorAttribute)MethodBase.GetCurrentMethod().GetCustomAttributes(typeof(InterceptorAttribute), false)[0];
            //attribute.OnEntry("SimpleTest.Class1.VoidMethodWithoutArgs");
            //try
            //{
            //    TestMessages.Record("VoidMethodWithoutArgs: Body");
            //    attribute.OnExit("SimpleTest.Class1.VoidMethodWithoutArgs");
            //}
            //catch (Exception __exception)
            //{
            //    attribute.OnException("SimpleTest.Class1.VoidMethodWithoutArgs", __exception);
            //    throw;
            //}
            var __attribute = (InterceptorAttribute)MethodBase.GetCurrentMethod().GetCustomAttributes(typeof(InterceptorAttribute), false)[0];
            if (__attribute == null)
                return;
            __attribute.OnEntry("SimpleTest.Class1.VoidMethodWithoutArgs");
            TestMessages.Record("VoidMethodWithoutArgs: Body");
            __attribute.OnExit("SimpleTest.Class1.VoidMethodWithoutArgs");
        }

        [Interceptor]
        public void ExpectedVoidMethodThrowingInvalidOperationException()
        {
            var attribute = (InterceptorAttribute)MethodBase.GetCurrentMethod().GetCustomAttributes(typeof(InterceptorAttribute), false)[0];
            attribute.OnEntry("SimpleTest.Class1.VoidMethodThrowingInvalidOperationException");
            try
            {
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
