﻿namespace SimpleTest.PnP;

public static class TestRecords
{
    public static void Clear()
    {
        Records.Clear();
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
        Record(Method.OnException, [exType, exMessage]);
    }

    public static void RecordInit(object instance, string methodName, int argLength)
    {
        Record(Method.Init, [instance, methodName, argLength]);
    }

    public static void RecordBody(string name, string extraInfo = null)
    {
        Record(Method.Body, [name, extraInfo]);
    }

    internal static void Record(Method method, object[] args = null)
    {
        Records.Add( new( (int) method, args ));
    }

    public static IList<Tuple<int, object[]>> Records { get; } = new List<Tuple<int, object[]>>();

    public static void RecordOnContinuation()
    {
        Record(Method.OnContinuation);
    }
}