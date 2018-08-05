using System;
using System.Threading.Tasks;
using System.Reflection;

namespace SimpleTest
{
    public class DerivedMatchingDecoratorAttribute : MatchingDecoratorAttribute
    {
        public override void Init(object instance, MethodBase method, object[] args)
        {
            if (null == method) throw new ArgumentNullException("method");
            TestRecords.RecordInit(instance, method.DeclaringType.FullName + "." + method.Name, args.Length);
        }

        public override void OnEntry()
        {
            TestRecords.RecordOnEntry();
        }

        public override void OnExit()
        {
            TestRecords.RecordOnExit();
        }

        public override void OnException(Exception exception)
        {
            TestRecords.RecordOnException(exception.GetType(), exception.Message);
        }

        public override void OnTaskContinuation(Task t)
        {
            TestRecords.RecordOnContinuation();
        }
    }
}