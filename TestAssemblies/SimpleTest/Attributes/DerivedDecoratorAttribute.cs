using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleTest {
    /// <summary>
    /// Can derive from <see cref="MethodDecoratorAttribute"/>,
    /// override whatever methods you're interested in, and voila! Fody-based AOP.
    /// </summary>
    public class DerivedDecoratorAttribute : MethodDecorator.Fody.Interfaces.MethodDecoratorAttribute {
        public override void Init(object instance, MethodBase method, object[] args) {
            if (null == method) throw new ArgumentNullException("method");
            if (null == instance) throw new ArgumentNullException("instance");

            var methodDeclaration = method.DeclaringType.Name
                                    + "." + method.Name
                                    + "(" + string.Join(", ", args.Select(a => a.GetType().Name)) + ")";

            TestRecords.RecordInit(instance, methodDeclaration, args.Length);
        }

        public override void OnEntry() {
            TestRecords.RecordOnExit();
        }

        public override void OnExit() {
            TestRecords.RecordOnExit();
        }

        public override void OnException(Exception exception) {
            TestRecords.RecordOnException(exception.GetType(), exception.Message);
        }

        public override void OnTaskContinuation(Task task) {
            TestRecords.RecordOnContinuation();
        }
    }
}