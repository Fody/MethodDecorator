namespace SimpleTest
{
    public class MarkerTypes
    {
        [Interceptor]
        public void AttributeImplementsInterface()
        {
            TestMessages.Record("AttributeImplementsInterface: Body");
        }

        [InterceptorDerivedFromInterface]
        public void AttributeDerivesFromClassThatImplementsInterface()
        {
            TestMessages.Record("AttributeDerivesFromClassThatImplementsInterface: Body");
        }

        [InterceptorDerivedFromAbstractBaseClass]
        public void AttributeDerivesFromMethodDecoratorAttribute()
        {
            TestMessages.Record("AttributeDerivesFromMethodDecoratorAttribute: Body");
        }
    }
}