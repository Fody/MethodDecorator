namespace SimpleTest
{
    using System;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public class IntersectMethodsMarkedByAttribute : Attribute
    {
        //must have
        public IntersectMethodsMarkedByAttribute()
        {
        }

        public IntersectMethodsMarkedByAttribute(params Type[] types)
        {
            if (types.All(x => typeof (Attribute).IsAssignableFrom(x)))
            {
                throw new Exception("Meaningfull configuration exception");
            }
        }

        public void OnEntry()
        {
            TestRecords.RecordOnEntry();
        }

        public void OnExit()
        {
            TestRecords.RecordOnExit();
        }

        public void OnException(Exception exception)
        {
            TestRecords.RecordOnException(exception.GetType(), exception.Message);
        }
    }
}