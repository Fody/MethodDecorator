namespace SimpleTest
{
    public class InterceptingAbstractMethods : AbstractBaseClass
    {
        public override void AbstractMethod()
        {
            TestRecords.RecordBody("InterceptingAbstractMethods.AbstractMethod");
        }
    }

    public abstract class AbstractBaseClass
    {
        [Interceptor]
        public abstract void AbstractMethod();
    }
}