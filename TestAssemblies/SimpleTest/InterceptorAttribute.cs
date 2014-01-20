using System;
using System.Reflection;

namespace SimpleTest {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
    public class InterceptorAttribute : Attribute {
        public void Init(MethodBase method, object[] args) {
            TestRecords.RecordInit(method.DeclaringType.FullName + "." + method.Name, args.Length);
        }
        public void OnEntry() {
            TestRecords.RecordOnEntry();
        }

        public void OnExit() {
            TestRecords.RecordOnExit();
        }

        public void OnException(Exception exception) {
            TestRecords.RecordOnException(exception.GetType(), exception.Message);
        }
    }
}