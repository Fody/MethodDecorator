namespace SimpleTest
{
    public class InterceptingAbstractMethods : AbstractBaseClass
    {
        public override void AbstractMethod()
        {
            TestMessages.Record("InterceptingAbstractMethods.AbstractMethod: Body");
        }
    }

    public abstract class AbstractBaseClass
    {
        [Interceptor]
        public abstract void AbstractMethod();
    }
}