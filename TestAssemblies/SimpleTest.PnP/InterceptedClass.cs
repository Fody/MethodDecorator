using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTest.PnP
{
    [InterceptorWithParams(1, "class_parameter", StringProperty = "class_property", StringField = "class_field")]
    public class InterceptedClass
    {
        public InterceptedClass()
        {
            TestRecords.Clear();
        }

        public void ImplicitIntercepted()
        {
            TestRecords.RecordBody("ImplicitIntercepted");
        }

        [InterceptorWithParams(10, "method_parameter", StringProperty = "method_property", StringField = "method_field")]
        public void ExplicitIntercepted()
        {
            TestRecords.RecordBody("ExplicitIntercepted");
        }
    }
}
