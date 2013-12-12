using System;
using System.Reflection;

namespace AnotherAssemblyAttributeContainer {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Module)]
    public class ExternalInterceptorAttribute : Attribute {
        public void OnEntry(MethodBase method, object[] args) {
            TestMessages.Record(
                args.Length > 0
                    ? string.Format("OnEntry: {0} [{1}]", method.DeclaringType.FullName + "." + method.Name, args.Length)
                    : string.Format("OnEntry: {0}", method.DeclaringType.FullName + "." + method.Name));
        }

        public void OnExit(MethodBase method) {
            TestMessages.Record(string.Format("OnExit: {0}", method.DeclaringType.FullName + "." + method.Name));
        }

        public void OnException(MethodBase method, Exception exception) {
            TestMessages.Record(string.Format("OnException: {0} - {1}: {2}", method.DeclaringType.FullName + "." + method.Name, exception.GetType(), exception.Message));
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly)]
    public class ExternalInterceptionAssemblyLevelAttribute : Attribute {
        public void OnEntry(MethodBase method, object[] args) {
            TestMessages.Record(
                args.Length > 0
                    ? string.Format("OnEntry: {0} [{1}]", method.DeclaringType.FullName + "." + method.Name, args.Length)
                    : string.Format("OnEntry: {0}", method.DeclaringType.FullName + "." + method.Name));
        }

        public void OnExit(MethodBase method) {
            TestMessages.Record(string.Format("OnExit: {0}", method.DeclaringType.FullName + "." + method.Name));
        }

        public void OnException(MethodBase method, Exception exception) {
            TestMessages.Record(string.Format("OnException: {0} - {1}: {2}", method.DeclaringType.FullName + "." + method.Name, exception.GetType(), exception.Message));
        }
    }
}