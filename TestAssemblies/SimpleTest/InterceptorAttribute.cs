using System;
using System.Reflection;

namespace SimpleTest {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
    public class InterceptorAttribute : Attribute {
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