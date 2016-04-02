using System;
using System.IO;

namespace SimpleTest.Net2
{
    public class InterceptingMethods {
        int finallyCalled;

        [Interceptor]
        public int ReturnsNumber() {
            try
            {
                using (var fs = new MemoryStream())
                {
                    TestRecords.RecordBody("ReturnsNumber");
                    return 42;
                }
            }
            catch(Exception)
            {
                // do nothing
                return 40;
            }
            finally
            {
                IncrementFinally();
            }
        }

        private void IncrementFinally()
        {
            finallyCalled += 1;
        }
    }
}