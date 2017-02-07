using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using MethodDecorator.Fody.Interfaces;

namespace SimpleTest {
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Constructor)]
    public class DerivedFromInterfaceDecoratorAttribute : Attribute, IMethodDecorator {
        public void Init(object instance, MethodBase method, object[] args) {
            if (null == method) throw new ArgumentNullException("method");
            if (null == instance) throw new ArgumentNullException("instance");
            var methodDeclaration = method.DeclaringType.Name
                                    + "." + method.Name
                                    + "(" + string.Join(", ", args.Select(a => a.GetType().Name)) + ")";

            TestRecords.RecordInit(instance, methodDeclaration, args.Length);
        }

        public void OnEntry() {
            TestRecords.RecordOnExit();
        }

        public void OnExit() {
            TestRecords.RecordOnExit();
        }

        public void OnException(Exception exception) {
            TestRecords.RecordOnException(exception.GetType(), exception.Message);
        }

        public void TaskContinuation(Task task) {
            TestRecords.RecordOnContinuation();
        }
    }
}