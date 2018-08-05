using System.Globalization;

namespace SimpleTest
{
    public class InterceptingExtensionMethods
    {
        public string ReturnsString()
        {
            return "hello world".ToTitleCase();
        }
    }

    public static class StringExtensions
    {
        [Interceptor]
        public static string ToTitleCase(this string value)
        {
            TestRecords.RecordBody("ToTitleCase");
            return new CultureInfo("en-GB", false).TextInfo.ToTitleCase(value);
        }
    }
}