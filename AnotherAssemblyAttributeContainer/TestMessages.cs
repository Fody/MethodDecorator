using System.Collections.Generic;

namespace AnotherAssemblyAttributeContainer
{
    public static class TestMessages
    {
        private static readonly IList<string> messages = new List<string>();

        public static void Clear()
        {
            messages.Clear();
        }

        public static void Record(string message)
        {
            messages.Add(message);
        }

        public static IList<string> Messages
        {
            get { return messages; }
        }
    }
}