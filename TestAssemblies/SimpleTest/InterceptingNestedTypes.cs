namespace SimpleTest;

public class InterceptingNestedTypes
{
    public class Nested
    {
        [Interceptor]
        public string StringMethod()
        {
            return "sausages";
        }
    }

    public class FirstLevel
    {
        public class SecondLevel
        {
            public class DeeplyNested
            {
                [Interceptor]
                public int NumberMethod()
                {
                    return 42;
                }
            }
        }
    }
}