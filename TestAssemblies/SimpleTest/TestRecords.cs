namespace SimpleTest
{
    using System;
    using System.Collections.Generic;

    public enum Method
    {
        Init = 0,
        OnEnter = 1,
        Body = 2,
        OnExit = 3,
        OnException = 4,
        TaskContinuation = 5
    }

    public static class TestRecords
    {
        private static readonly IList<Tuple<int, object[]>> _records = new List<Tuple<int, object[]>>();

        public static IList<Tuple<int, object[]>> Records
        {
            get { return _records; }
        }

        public static void Clear()
        {
            _records.Clear();
        }

        public static void RecordOnEntry()
        {
            Record(Method.OnEnter);
        }

        public static void RecordOnExit()
        {
            Record(Method.OnExit);
        }

        public static void RecordOnException(Type exType, string exMessage)
        {
            Record(Method.OnException, new object[] {exType, exMessage});
        }

        public static void RecordInit(object instance, string methodName, int argLength)
        {
            Record(Method.Init, new[] {instance, methodName, argLength});
        }

        public static void RecordBody(string name, string extraInfo = null)
        {
            Record(Method.Body, new object[] {name, extraInfo});
        }

        public static void Record(Method method, object[] args = null)
        {
            _records.Add(new Tuple<int, object[]>((int) method, args));
        }

        public static void RecordTaskContinuation()
        {
            Record(Method.TaskContinuation);
        }
    }
}