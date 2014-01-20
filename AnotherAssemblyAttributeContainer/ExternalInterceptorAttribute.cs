using System;
using System.Reflection;

namespace AnotherAssemblyAttributeContainer {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Module)]
    public class ExternalInterceptorAttribute : Attribute {
        public void Init(MethodBase method, object[] args) {
            TestMessages.Record(string.Format("Init: {0} [{1}]", method.DeclaringType.FullName + "." + method.Name, args.Length));
        }
        public void OnEntry() {
            TestMessages.Record("OnEntry");
        }

        public void OnExit() {
            TestMessages.Record("OnExit");
        }

        public void OnException(Exception exception) {
            TestMessages.Record(string.Format("OnException: {0}: {1}", exception.GetType(), exception.Message));
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly)]
    public class ExternalInterceptionAssemblyLevelAttribute : Attribute {
        public void Init(MethodBase method, object[] args) {
            TestMessages.Record(string.Format("Init: {0} [{1}]", method.DeclaringType.FullName + "." + method.Name, args.Length));
        }
        public void OnEntry() {
            TestMessages.Record("OnEntry");
        }

        public void OnExit() {
            TestMessages.Record("OnExit");
        }

        public void OnException(Exception exception) {
            TestMessages.Record(string.Format("OnException: {0}: {1}", exception.GetType(), exception.Message));
        }
    }
}