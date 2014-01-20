using System;
using System.Reflection;

using SimpleTest;

namespace AnotherAssemblyAttributeContainer {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Module)]
    public class ExternalInterceptorAttribute : Attribute {
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

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly)]
    public class ExternalInterceptionAssemblyLevelAttribute : Attribute {
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